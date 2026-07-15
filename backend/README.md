# Backend — KiranaManagement API

The shared **ASP.NET Core (C#, .NET 8 LTS) Web API** that powers all three
client surfaces (store dashboard, customer app, driver app).

- **Framework:** ASP.NET Core Web API
- **Data:** Entity Framework Core (Pomelo) + **MySQL 8**
- **Real-time:** SignalR · **Background jobs:** Hangfire · **API docs:** Swagger

## Planned solution structure
```
backend/
 ├─ KiranaManagement.sln
 ├─ src/
 │   ├─ Kirana.Api            (Web API — controllers, SignalR hubs, startup)
 │   ├─ Kirana.Application    (services, DTOs, interfaces, business rules)
 │   ├─ Kirana.Domain         (entities/enums — the tables from PRD §7)
 │   └─ Kirana.Infrastructure (EF Core DbContext, migrations, integrations)
 └─ tests/
     ├─ Kirana.UnitTests
     └─ Kirana.IntegrationTests
```

## Guides
- [STRUCTURE.md](STRUCTURE.md) — detailed backend directory structure: what
  every project and folder holds, mapped to the PRD domains, plus a request-flow
  example and conventions.
- [../docs/DEVELOPMENT.md](../docs/DEVELOPMENT.md) — setup, packages,
  multi-tenancy, migrations, and the phase-by-phase build order.
