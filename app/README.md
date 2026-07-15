# App — Customer & Driver Mobile Apps

The customer-facing and driver-facing apps. Both consume the same
[backend](../backend) Web API.

- **Customer Online-Order App (marketplace)** — browse a **unified catalog across
  all stores**, add items from **multiple stores to one cart**, pay **once**, and
  track a **single order** (one order number). (PRD §5.6–5.7)
- **Driver App (optional fallback)** — delivery is primarily fulfilled by
  **third-party logistics partners via API** (Porter/Borzo/Shadowfax/Shiprocket —
  see [../docs/DELIVERY_INTEGRATIONS.md](../docs/DELIVERY_INTEGRATIONS.md)). This
  own-driver app is only used when no 3PL is serviceable: the driver gets one
  order with a multi-store pickup list, collects from each store, delivers
  together, and captures proof of delivery / COD. (PRD §5.8)

**Suggested approach:** start as responsive **PWAs** for fastest delivery, then
move to native / .NET MAUI later. Push notifications for order and delivery
updates.

**Driver maps & navigation (Swiggy/Instacart-style):** the Driver App uses a
maps/location stack — Maps SDK, geocoding, directions, route optimization, live
GPS tracking (via SignalR), geofencing, and FCM/APNs push. Recommended provider
**Google Maps Platform** (Ola Maps / Mapmyindia as India cost alternatives),
kept behind an `IMapProvider` seam. See
[../docs/DRIVER_APP_INTEGRATIONS.md](../docs/DRIVER_APP_INTEGRATIONS.md).

## Suggested layout
```
app/
 ├─ customer/   (Customer Online-Order App)
 └─ driver/     (Driver App)
```

See [../docs/PRD.md](../docs/PRD.md) and [../docs/DEVELOPMENT.md](../docs/DEVELOPMENT.md).
