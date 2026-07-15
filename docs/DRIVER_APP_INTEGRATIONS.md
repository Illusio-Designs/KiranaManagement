# Driver App Integrations — Maps, Location & Navigation

> **Scope note:** delivery is now **primarily fulfilled by third-party logistics
> partners** — see [DELIVERY_INTEGRATIONS.md](DELIVERY_INTEGRATIONS.md). This
> document applies to the **optional own-driver app** (fallback) and to the
> **customer-side live-tracking map** used regardless of who delivers.

**Goal:** give our (optional) Driver App and the customer tracking map the same
experience as **Swiggy** and **Instacart** — live map, turn-by-turn navigation,
multi-store pickup routing, real-time driver tracking, arrival detection, and
push notifications.

> **Important:** "Swiggy API" / "Instacart API" are **internal** — they are not
> public APIs you can call. What those apps actually use is a **stack of
> mapping + location + messaging services**. We integrate the same *categories*
> of service. This guide lists what to use and how it plugs into our
> ASP.NET Core backend and mobile app.

---

## 1. Capabilities → API mapping

| Capability (what the driver sees/does) | API category | Google Maps Platform | India-friendly alternatives |
|---|---|---|---|
| Show a live map in the app | **Maps SDK** | Maps SDK for Android/iOS, Maps JS API | Ola Maps, Mapmyindia (Mappls), Mapbox, HERE |
| Convert store/customer address ↔ GPS | **Geocoding** | Geocoding API | Ola Maps, Mappls, HERE |
| Turn-by-turn navigation to a stop | **Directions / Navigation** | Directions API / Navigation SDK | Mappls, HERE, Mapbox |
| ETA & driver-to-order distance | **Distance Matrix** | Distance Matrix API | Ola Maps, Mappls |
| Best order of multi-store pickups | **Route optimization** | Routes API (waypoint optimization) | Mapbox Optimization, HERE Tour Planning |
| Address auto-complete at checkout | **Places / Autocomplete** | Places API | Ola Maps, Mappls |
| Track driver live on customer map | **Live location streaming** | *(our own)* SignalR + device GPS | same (provider-agnostic) |
| Auto-detect "arrived at store/customer" | **Geofencing** | Geofencing (device SDK) | device SDK / server-side check |
| New-assignment & status alerts | **Push notifications** | **Firebase Cloud Messaging (FCM)** + **APNs** | same |

**Recommended default:** **Google Maps Platform** (closest to what Swiggy /
Instacart use; best coverage & docs). **If cost is a concern in India**, start
with **Ola Maps** or **Mapmyindia/Mappls** — both are cheaper and India-first.
Keep the provider behind an interface so it can be swapped (see §4).

---

## 2. How live tracking works (the Swiggy/Instacart pattern)

```
 Driver device (GPS)                 Backend (ASP.NET Core)             Customer app
 ─────────────────                   ──────────────────────            ────────────
 every 3–5 s while on a trip
   POST/stream lat,lng  ───────────▶  DeliveryHub (SignalR)
                                        │ store last location
                                        │ (Redis / in-memory)
                                        ├─ geofence check: near store? near customer?
                                        └─ broadcast location ──────────▶ live marker + ETA on map
```

- The **device** is the source of GPS. A PWA uses the browser
  `Geolocation.watchPosition()`; a native/MAUI app uses OS background location.
- The **backend** receives points over a **SignalR hub** (`DeliveryHub`), keeps
  the latest position (Redis for scale), runs **geofence** checks, and
  **broadcasts** to the customer watching that order.
- **Battery/data:** only track while a trip is active; sample every 3–5 s (or on
  meaningful movement), batch when backgrounded.

---

## 3. Multi-store pickup routing (our specific twist)

A marketplace order has **several stores to visit, then one customer** (PRD §5.8).

1. Collect the coordinates: each contributing store + the customer.
2. Call a **route-optimization / directions** API with the driver's start, all
   store waypoints, and the customer as the final destination, asking it to
   **optimize waypoint order**.
3. Show the driver the ordered stop list + turn-by-turn to the **next** stop.
4. As each store's pickup stop is marked collected, advance to the next stop;
   after the last store, navigate to the customer.

> Start simple: nearest-next ordering is fine for a first version; add true
> optimization (and cross-order batching, PRD §5.8 FR-8.15) later.

---

## 4. Backend design (keep it provider-agnostic)

Put every map call behind one interface in **`Kirana.Application`** so the
provider can change without touching business logic:

```csharp
// Kirana.Application/Common/Interfaces/IMapProvider.cs
public interface IMapProvider
{
    Task<GeoPoint> GeocodeAsync(string address, CancellationToken ct);
    Task<RouteResult> GetRouteAsync(GeoPoint origin, IReadOnlyList<GeoPoint> waypoints,
                                    GeoPoint destination, bool optimizeWaypoints, CancellationToken ct);
    Task<EtaResult> GetEtaAsync(GeoPoint from, GeoPoint to, CancellationToken ct);
}
```

Implement it per provider in **`Kirana.Infrastructure/Integrations/Maps/`**:
```
Integrations/Maps/
 ├─ GoogleMapsProvider.cs     : IMapProvider
 ├─ OlaMapsProvider.cs        : IMapProvider   (swap-in for India cost)
 └─ MapProviderOptions.cs     (API key, base URL — from config/secrets)
```
Register the chosen one in DI; the app depends only on `IMapProvider`.

**Real-time:** reuse the existing **`DeliveryHub`** (SignalR) for location
in/out. **Push:** add an `IPushSender` implemented with **FCM/APNs** in
`Integrations/Push/`.

**Keys & secrets:** map/push API keys live in config/`user-secrets` (dev) and
environment variables (prod) — **never** commit them. Restrict keys by
app/bundle id and by API, and set usage quotas/billing alerts.

---

## 5. Cost & rollout notes

- All these providers are **pay-as-you-go** (per map load / per API call). Set
  **billing alerts** and **daily quotas** from day one.
- Cut costs: cache geocoding results, only fetch a route when a trip starts,
  sample GPS sensibly, and prefer **Distance Matrix** over full Directions when
  you only need an ETA.
- **Phasing (matches PRD Phase 3 — Delivery):**
  1. Static map + manual navigation handoff (open Google/Ola Maps app) + FCM push.
  2. In-app map, live driver location via SignalR, geofence arrival.
  3. Optimized multi-store route + customer live ETA.
  4. Cross-order batching / advanced optimization.

---

## 6. Decisions needed

1. **Maps provider** — Google Maps Platform (best experience) vs Ola Maps /
   Mapmyindia (lower cost, India-first)? Recommend Google to start, keep the
   `IMapProvider` seam to switch.
2. **Driver app tech** — PWA first (fastest) vs native/.NET MAUI (better
   background location & navigation). Live tracking is smoother on native.
3. **Own drivers only**, or also allow **third-party logistics** dispatch later
   (Swiggy Genie / Dunzo-style)? Out of scope for v1 but the assignment layer
   can leave room for it.

See [PRD.md §5.8 and §9](PRD.md) and [../backend/STRUCTURE.md](../backend/STRUCTURE.md).
