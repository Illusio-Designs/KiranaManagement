# Third-Party Service Costs (Operating Costs)

Indicative pricing (India, **2026**) for the external services the platform
depends on. These are **running/operating costs paid to third parties** — they
are **separate** from the cost of *building* the software (developer time).

> ⚠️ **Verify before committing.** Prices change often, most add **18% GST**,
> volume discounts apply at scale, and several logistics/maps vendors quote
> custom rates via sales. Figures below are ballparks to plan with, with sources.

---

## 1. Two kinds of cost

- **Per-transaction (variable)** — scales with orders: delivery, payment fees,
  notifications. This is the big one.
- **Fixed / monthly** — hosting, subscriptions, base fees.

The **dominant cost per order is delivery**. Payment fees are often **zero** for
grocery (small-value UPI). Everything else is fractions of a rupee.

---

## 2. Per-order (variable) costs

| Service | Typical price (2026) | Cost on a ~₹500 grocery order |
|---|---|---|
| **Delivery — Borzo (3PL)** | Flat **₹35 + ₹8/km** (≤5 kg, hyperlocal). ~₹43 @1 km, **₹75 @5 km**, ₹115 @10 km | **₹75–120** (higher for multi-store pickup / extra stops) |
| **Payment — UPI** | **0% (zero MDR)** for merchant txns **< ₹2,000** (NPCI/RBI mandate) | **₹0** (most grocery orders) |
| **Payment — cards/others** | **~2% + 18% GST** (premium cards/EMI ~3%) | ~₹10 if paid by card (₹0 if UPI) |
| **SMS notifications** (MSG91) | **₹0.15–0.20** per SMS (transactional) | ~₹0.30–0.60 (2–3 messages) |
| **WhatsApp (utility msg)** | Meta rates via MSG91, **no markup** (marketing ~₹0.86; utility lower) | ~₹1–2 if used instead of SMS |
| **Maps/location** | Free tiers cover early volume (see below) | ~₹0 early; fractions of ₹ at scale |
| **Push (FCM/APNs)** | **Free** | ₹0 |

**Takeaway:** on a typical UPI grocery order, **~₹75–120 goes to delivery** and
**almost nothing else**. Decide who bears delivery — customer (delivery fee),
store, or platform.

---

## 3. Fixed / monthly costs

| Service | Price (2026) | Notes |
|---|---|---|
| **Maps — Ola Maps** | **Free up to 5M calls/month** per API; paid ≈ 50% of Google's rates | India-first; generous free tier — good default. |
| **Maps — Google Maps Platform** | 10K free events/mo per Essentials SKU; then **Dynamic Maps $7 / Geocoding ~$5 / Static $2** per 1,000. Subscriptions from ~$100/mo (50K calls). No universal $200 credit anymore. | Best experience, priciest. |
| **Maps — Mappls (MapmyIndia)** | Custom, ~**$300/mo for 10K calls** | Contact sales. |
| **WhatsApp Business (MSG91)** | **₹500/month per number** (first 2 months waived) + per-message Meta rates | Only if using WhatsApp. |
| **SMS** | Pay-as-you-go (no monthly) | See per-order table. |
| **Payment gateway (Razorpay/Cashfree)** | **No setup fee, no AMC** | Pay per transaction only. |
| **Hosting** (cloud VM + MySQL + storage) | ~**₹3,000–15,000/month** for a small–mid setup; grows with load | Any cloud (AWS/Azure/GCP/DO/Hetzner). Managed MySQL costs more than self-hosted. |
| **Domain + TLS** | ~₹1,000/year; TLS free (Let's Encrypt) | — |

---

## 4. Worked example — one ₹500 UPI order (5 km, 2 stores)

| Line | Cost |
|---|---|
| Delivery (Borzo, ~5 km, +1 extra pickup) | ~₹90 |
| Payment (UPI < ₹2,000) | ₹0 |
| 2 SMS + 1 push | ~₹0.4 |
| Maps (tracking) | ~₹0 |
| **Total third-party cost** | **~₹90** |

Against this, the platform earns its **platform fee** (commission/flat per order)
+ any **delivery fee** charged to the customer + **ad revenue**. Delivery
economics are the thing to get right.

---

## 5. Cost-control levers

- **Pass delivery to the customer** (delivery fee) or set a **minimum order value**.
- **Cap stores per order** and prefer **same-area stores** to keep pickup cheap.
- **Push UPI** (0% MDR) over cards.
- **Start on Ola Maps' free tier**; move to Google only if you need its UX.
- **Cache geocoding**, fetch a route only when a trip starts.
- **Compare 3PL quotes** per order and negotiate rate cards at volume.

---

## 6. What this does *not* include

- **Software build cost** (developer salaries / agency fees) — the largest early
  cost, but internal, not a third-party fee.
- **Team, marketing, customer support, working capital.**
- **GST (18%)** on most of the fees above — budget for it.

---

## Sources
- Google Maps Platform pricing — https://mapsplatform.google.com/pricing/ ,
  https://developers.google.com/maps/billing-and-pricing/pricing
- Ola Maps pricing — https://maps.olakrutrim.com/pricing ,
  https://inc42.com/buzz/after-google-maps-cuts-prices-ola-maps-announces-new-pricing-structure-to-woo-developers/
- Mappls (MapmyIndia) — https://datarade.ai/data-providers/mapmyindia/profile
- Razorpay pricing / UPI MDR — https://razorpay.com/pricing/ ,
  https://razorpay.com/learn/upi-transaction-charges/
- Borzo hyperlocal pricing — https://intercom.help/wefast/en/articles/3046235-hyperlocal-delivery ,
  https://borzodelivery.com/in/api-integration
- MSG91 SMS & WhatsApp — https://msg91.com/in/pricing/sms ,
  https://msg91.com/guide/whatsapp-pricing-update-2026-and-save-with-msg91

*Figures are 2026 estimates for planning; confirm current rates with each vendor.*
