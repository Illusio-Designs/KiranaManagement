# Backend — KiranaManagement API

The shared **ASP.NET Core (C#, .NET 8 LTS) Web API** that powers all three
client surfaces (store dashboard, customer app, driver app).

- **Framework:** ASP.NET Core Web API
- **Data:** Entity Framework Core (Pomelo) + **MySQL 8**
- **Real-time:** SignalR · **Background jobs:** Hangfire · **API docs:** Swagger

## Solution structure
```
backend/
 ├─ KiranaManagement.sln
 ├─ src/
 │   ├─ Kirana.Api            (Web API — controllers, startup, JWT, Swagger)
 │   ├─ Kirana.Application    (services, DTOs, interfaces, business rules)
 │   ├─ Kirana.Domain         (entities/enums — the tables from PRD §7)
 │   └─ Kirana.Infrastructure (EF Core DbContext + tenant filter, JWT, seed)
 └─ tests/
     └─ Kirana.UnitTests
```

## Quickstart (Phase 0 — onboarding & auth)

Phase 0 is implemented: multi-tenant `AppDbContext`, `Store`/`User`, JWT auth,
and the **register → approve → login** flow, plus a tenant-scoped `Products`
endpoint that proves store isolation.

**Prerequisites:** .NET 8 SDK and a running MySQL 8 (see
[../docs/DEVELOPMENT.md](../docs/DEVELOPMENT.md) §4 for a Docker one-liner).

```bash
cd backend
# 1. point appsettings.Development.json at your MySQL (default: localhost root/root, db 'kirana')
dotnet restore
dotnet run --project src/Kirana.Api      # Swagger at http://localhost:5080/swagger
```

On first run (Development) the app **creates the schema** (`EnsureCreated`) and
**seeds a SuperAdmin** — default `superadmin@kirana.local` / `Admin@12345`
(override via the `Seed` section). Production switches to EF migrations
(DEVELOPMENT.md §7).

**Try the flow (Swagger or curl):**
1. `POST /api/auth/register-store` — a store self-registers → status `Pending`.
2. `POST /api/auth/login` as the **SuperAdmin** → copy the JWT.
3. `GET /api/admin/stores/pending` then `POST /api/admin/stores/{id}/approve`
   (send the SuperAdmin token) → store becomes `Active`.
4. `POST /api/auth/login` as the **store owner** (only works once approved) → JWT
   carrying `store_id`.
5. With the owner token, `POST /api/products` then `GET /api/products` — you only
   ever see your own store's products (tenant isolation).

Run tests: `dotnet test`

## Guides

## Guides
- [STRUCTURE.md](STRUCTURE.md) — detailed backend directory structure: what
  every project and folder holds, mapped to the PRD domains, plus a request-flow
  example and conventions.
- [../docs/DEVELOPMENT.md](../docs/DEVELOPMENT.md) — setup, packages,
  multi-tenancy, migrations, and the phase-by-phase build order.
