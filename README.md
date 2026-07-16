# KiranaManagement

A multi-tenant SaaS platform for grocery / kirana stores. Every store
self-registers, waits for platform approval, and after approval manages its
**sales, purchases, online sales, order management, delivery, and accounting**
from one place — while a central Super Admin governs onboarding and platform
health.

The platform ships as three connected surfaces over a shared backend:

- **Customer Online-Order App** — browse a store, order, pay, and track delivery.
- **Driver App** — drivers receive assigned orders, navigate, and confirm delivery.
- **Store Management Dashboard** — run catalog, inventory, purchasing, POS,
  order fulfilment, delivery dispatch, accounting, and reports.

## Repository Structure
```
KiranaManagement/
 ├─ backend/         ASP.NET Core (.NET 8) Web API — shared backend for all clients
 ├─ frontend/        Store Management Dashboard + Super Admin console (web)
 ├─ app/             Customer Online-Order App + Driver App (mobile/PWA)
 ├─ legacy-vs2015/   VS 2015 version — ASP.NET Web API 2 + EF6 + web frontend (for the course requirement)
 └─ docs/            PRD and development guide
```

> **Two backends, pick one:** `backend/` is the modern **.NET 8** project (VS 2022 / VS Code).
> `legacy-vs2015/` is a **Visual Studio 2015-compatible** version (ASP.NET Web API 2 + EF6)
> for the course/assignment that requires VS 2015 — see [legacy-vs2015/README.md](legacy-vs2015/README.md).

## Tech Stack
- **Backend:** ASP.NET Core (C#, .NET 8 LTS) Web API
- **Database:** MySQL 8 (Entity Framework Core, Pomelo provider)
- **Real-time:** SignalR · **Background jobs:** Hangfire · **API docs:** Swagger

## Documentation
- [Product Requirements Document (PRD)](docs/PRD.md) — vision, scope, personas,
  functional & non-functional requirements, tech stack, data model, phasing, and risks.
- [Development Guide](docs/DEVELOPMENT.md) — setup, solution structure, multi-tenancy,
  migrations, auth, and a phase-by-phase build order.

## Authors
Mansi, Hiral, Zigma

## Status
Early planning. The PRD and development guide are in place; implementation follows
the phased build order in the guide.
