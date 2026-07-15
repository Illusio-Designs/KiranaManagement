# Frontend — Store Management Dashboard

The web dashboard store owners and staff use to run operations: catalog,
inventory, purchasing, POS billing, order fulfilment, delivery dispatch,
accounting, and reports. Also hosts the **Super Admin console** (store approval
and platform governance).

- Consumes the [backend](../backend) Web API (JSON + SignalR for live updates).
- **Suggested approach:** ASP.NET Core **MVC / Razor Pages** (fastest for a .NET
  team) *or* a SPA (React / Angular / Blazor). Decide before scaffolding.

## Responsibilities (PRD surfaces)
- Store Management Dashboard — PRD §5.2–5.10
- Platform Super Admin Console — PRD §5.12

See [../docs/PRD.md](../docs/PRD.md) and [../docs/DEVELOPMENT.md](../docs/DEVELOPMENT.md).
