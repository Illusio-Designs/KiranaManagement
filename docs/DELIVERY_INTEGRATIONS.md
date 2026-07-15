# Delivery Integrations — Third-Party Logistics (3PL)

**Model:** the platform does **not run its own delivery fleet**. For each order
it **books a delivery task with a third-party courier partner via their API**,
then tracks the assigned rider and processes status updates until the order is
delivered. An **own-driver app** exists only as an optional fallback
([DRIVER_APP_INTEGRATIONS.md](DRIVER_APP_INTEGRATIONS.md)).

---

## 1. Candidate partners (India)

| Partner | Type | Multi-stop pickup? | Notes |
|---|---|---|---|
| **Porter** | On-demand hyperlocal | Yes (multiple stops) | Good for multi-store pickup. |
| **Borzo** (ex-WeFast) | On-demand courier | Yes (multi-point orders) | Same-day, multi-point routes. |
| **Shadowfax** | Hyperlocal + last-mile | Varies by product | Wide coverage. |
| **Shiprocket (Quick)** | Hyperlocal aggregator | Aggregates several couriers | One integration, many couriers. |
| **Pidge / LoadShare / Zypp** | Hyperlocal/EV last-mile | Varies | City-dependent. |

> Availability, pricing, COD support, and multi-stop capability **differ per
> partner and city** — confirm against each partner's current API docs before
> committing. Start with **one** partner, then add more behind the same seam.

---

## 2. Standard 3PL integration flow

```
Order parts READY
      │
      ▼
1. getQuote(pickup points, drop, package, COD?)   ──▶ partner returns price + ETA + serviceable?
      │
2. createTask(...)                                 ──▶ partner returns taskId + trackingUrl (+ rider once assigned)
      │
3. status updates  ◀── partner WEBHOOK (preferred) / polling
      │   map to our states: Searching → RiderAssigned → AtStore → PickedUp → OutForDelivery → Delivered
      │   relay live tracking + ETA to the customer (same order number)
      ▼
4. Delivered  ──▶ POD (OTP/photo) + COD confirmation returned; reconcile courier fee & COD
      │
      └─ (Failed/Cancelled) ──▶ refund/substitution on parent order; optional failover to next partner
```

Every partner differs in payload/field names — that's exactly why we wrap them.

---

## 3. Multi-store pickup — the key constraint

A marketplace order can span several stores (PRD §1.1.1), but delivery APIs vary:

- **Preferred:** use a partner/mode that supports **multiple pickup points** in a
  single task (Porter, Borzo). One rider collects from all stores → single drop.
- **Fallback A — single consolidated pickup:** if a partner only supports one
  pickup, restrict/route the order so items are gathered at one point first
  (rarely practical for kirana — avoid).
- **Fallback B — one task per store:** book a task per store to the customer.
  This **breaks "deliver together"** and multiplies delivery cost — use only if no
  multi-stop partner is serviceable, and surface it as a product decision.
- **Guardrail:** at cart/checkout, prefer stores within one **serviceable
  delivery area** (`StoreServiceArea`) so multi-store pickup stays cheap and fast.

> Decision to confirm: **cap the number of stores per order** (e.g. ≤ 3) to keep
> pickup time and cost reasonable?

---

## 4. Backend design (provider-agnostic)

One interface in **`Kirana.Application`**, implemented per partner in
**`Kirana.Infrastructure/Integrations/Delivery/`**:

```csharp
// Kirana.Application/Common/Interfaces/IDeliveryProvider.cs
public interface IDeliveryProvider
{
    string Name { get; }
    Task<DeliveryQuote> GetQuoteAsync(DeliveryRequest req, CancellationToken ct);
    Task<DeliveryTaskResult> CreateTaskAsync(DeliveryRequest req, CancellationToken ct);
    Task CancelTaskAsync(string externalTaskId, CancellationToken ct);
    Task<DeliveryTrackingInfo> TrackAsync(string externalTaskId, CancellationToken ct);
    DeliveryStatus MapStatus(string partnerStatus);   // partner state -> our enum
}
```
```
Integrations/Delivery/
 ├─ PorterProvider.cs        : IDeliveryProvider
 ├─ BorzoProvider.cs         : IDeliveryProvider
 ├─ ShiprocketProvider.cs    : IDeliveryProvider
 └─ DeliveryProviderOptions.cs   (API keys, base URLs — from config/secrets)
```

- A **`DeliveryDispatchService`** picks the partner per order (serviceability →
  quote/price → ETA), calls `CreateTaskAsync`, and persists a **`DeliveryTask`**.
- **Webhooks:** a `DeliveriesController` endpoint (e.g. `POST /api/webhooks/delivery/{provider}`)
  receives partner callbacks, **verifies the signature**, stores a
  `DeliveryWebhookEvent` (idempotent by event id), maps status, updates the
  `DeliveryTask` + order, and pushes the update to the customer via **SignalR**.
- **Polling fallback:** a Hangfire job calls `TrackAsync` for in-flight tasks in
  case a webhook is missed.
- **Failover:** if `CreateTask` fails or the partner cancels, try the next
  configured partner.

---

## 5. Money, COD & reconciliation

- **Delivery fee:** store the courier's charge on the `DeliveryTask`; configure
  who bears it — a **customer delivery fee**, the **store**, or the **platform**.
- **COD:** the partner collects cash and settles it to us per their cycle; we
  **allocate the collection back to each `StoreOrder`** and reconcile against the
  partner's settlement report.
- **Platform fee** (PRD §1.6) still accrues per `StoreOrder`, independent of who
  delivers.

---

## 6. Security & ops

- Partner **API keys** live in config/`user-secrets` (dev) and env vars (prod) —
  never committed.
- **Verify webhook signatures**; treat inbound events as **idempotent** (dedupe by
  event/task id).
- Log all partner requests/responses (without secrets) for dispute resolution.
- Set **timeouts + retries** on partner calls; degrade gracefully (queue + retry)
  if a partner API is down.

---

## 7. Rollout (matches PRD Phase 3 — Delivery)

1. Integrate **one** partner: quote → createTask → webhook status → POD/COD.
2. Customer live tracking (partner tracking URL or rider location on our map).
3. **Multi-store pickup** via a multi-stop-capable partner.
4. **Provider selection + failover** across 2–3 partners (price/ETA/serviceability).
5. Optional **own-driver fallback** when no partner is serviceable.

---

## 8. Decisions needed

1. **Which partner(s) first?** Recommend one that supports **multi-stop pickup**
   (Porter or Borzo) so the multi-store model works from day one.
2. **Cap stores per order** (e.g. ≤ 3) to control pickup cost/time?
3. **Who pays delivery** — customer, store, or platform (or split)?
4. **COD** — support at launch, or start prepaid-only to simplify reconciliation?

See [PRD.md §5.8 and §9](PRD.md) and [../backend/STRUCTURE.md](../backend/STRUCTURE.md).
