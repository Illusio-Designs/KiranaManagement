# App — Customer & Driver Mobile Apps

The customer-facing and driver-facing apps. Both consume the same
[backend](../backend) Web API.

- **Customer Online-Order App** — browse a store, cart, checkout, pay, and track
  delivery. (PRD §5.6, §5.7)
- **Driver App** — drivers see assigned orders, navigate, update delivery status,
  and capture proof of delivery / COD. (PRD §5.8)

**Suggested approach:** start as responsive **PWAs** for fastest delivery, then
move to native / .NET MAUI later. Push notifications for order and delivery
updates.

## Suggested layout
```
app/
 ├─ customer/   (Customer Online-Order App)
 └─ driver/     (Driver App)
```

See [../docs/PRD.md](../docs/PRD.md) and [../docs/DEVELOPMENT.md](../docs/DEVELOPMENT.md).
