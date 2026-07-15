# Development Guide — KiranaManagement

**Stack:** ASP.NET Core (C#, .NET 8 LTS) Web API · Entity Framework Core (Pomelo MySQL) · MySQL 8 · SignalR · Hangfire
**Authors:** Mansi, Hiral, Zigma

This guide takes you from an empty machine to a running, multi-tenant backend
and gives you an order of work that matches the PRD phases. Read it alongside
[PRD.md](PRD.md).

---

## 1. Prerequisites

Install:
- **.NET 8 SDK** — https://dotnet.microsoft.com/download
- **MySQL Server 8.x** + **MySQL Workbench** (or DBeaver) for browsing data
- **Visual Studio 2022** (Community is fine) *or* **VS Code** + C# Dev Kit
- **Git**
- (Optional) **Docker** — easiest way to run MySQL locally

Verify:
```bash
dotnet --version      # 8.x
mysql --version       # 8.x
git --version
```

Install the EF Core CLI once (global tool):
```bash
dotnet tool install --global dotnet-ef
```

---

## 2. Recommended Solution Structure

The repository is split into three top-level folders — **`backend/`**,
**`frontend/`**, and **`app/`**. The ASP.NET Core solution lives under
**`backend/`** with a clean, layered structure so the PRD domains map to
projects:

```
KiranaManagement/
 ├─ backend/          ← the ASP.NET Core solution (this guide)
 │   ├─ KiranaManagement.sln
 │   ├─ src/
 │   │   ├─ Kirana.Api            (ASP.NET Core Web API -> startup, controllers, SignalR hubs)
 │   │   ├─ Kirana.Application    (services, DTOs, interfaces, business rules)
 │   │   ├─ Kirana.Domain         (entities/models, enums — the tables from PRD §7)
 │   │   └─ Kirana.Infrastructure (EF Core DbContext, migrations, repositories, integrations)
 │   └─ tests/
 │       ├─ Kirana.UnitTests
 │       └─ Kirana.IntegrationTests
 ├─ frontend/         ← Store Management Dashboard + Super Admin console (web)
 ├─ app/              ← Customer Online-Order App + Driver App (mobile/PWA)
 └─ docs/             ← PRD and this guide
```

Dependency direction: **Api → Application → Domain**, and **Infrastructure → Application/Domain**.
Domain depends on nothing.

### Create it
```bash
cd backend                       # the .NET solution lives here
dotnet new sln -n KiranaManagement

dotnet new webapi   -n Kirana.Api            -o src/Kirana.Api
dotnet new classlib -n Kirana.Application    -o src/Kirana.Application
dotnet new classlib -n Kirana.Domain         -o src/Kirana.Domain
dotnet new classlib -n Kirana.Infrastructure -o src/Kirana.Infrastructure
dotnet new xunit    -n Kirana.UnitTests       -o tests/Kirana.UnitTests
dotnet new xunit    -n Kirana.IntegrationTests -o tests/Kirana.IntegrationTests

dotnet sln add src/**/**.csproj tests/**/**.csproj

# project references
dotnet add src/Kirana.Application    reference src/Kirana.Domain
dotnet add src/Kirana.Infrastructure reference src/Kirana.Application src/Kirana.Domain
dotnet add src/Kirana.Api            reference src/Kirana.Application src/Kirana.Infrastructure
```

---

## 3. NuGet Packages

In **Kirana.Infrastructure** (data + integrations):
```bash
cd src/Kirana.Infrastructure
dotnet add package Pomelo.EntityFrameworkCore.MySql
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
```

In **Kirana.Api**:
```bash
cd ../Kirana.Api
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package Swashbuckle.AspNetCore        # Swagger
dotnet add package Hangfire.AspNetCore
dotnet add package Hangfire.MySqlStorage
# SignalR ships with ASP.NET Core — no package needed
```

---

## 4. Configure MySQL

### Run MySQL (Docker option)
```bash
docker run --name kirana-mysql -e MYSQL_ROOT_PASSWORD=root \
  -e MYSQL_DATABASE=kirana -p 3306:3306 -d mysql:8
```

### Connection string — `src/Kirana.Api/appsettings.Development.json`
```json
{
  "ConnectionStrings": {
    "Default": "server=localhost;port=3306;database=kirana;user=root;password=root;TreatTinyAsBoolean=true"
  },
  "Jwt": {
    "Issuer": "KiranaManagement",
    "Audience": "KiranaManagement",
    "Key": "REPLACE_WITH_A_LONG_RANDOM_SECRET_AT_LEAST_32_CHARS"
  }
}
```
> Do **not** commit real secrets. Use `dotnet user-secrets` locally and
> environment variables in production.

### Register the DbContext — `Program.cs`
```csharp
var cs = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseMySql(cs, ServerVersion.AutoDetect(cs)));
```

---

## 5. Multi-Tenancy (the most important design rule)

Every store is a **tenant**. The chosen model (PRD §8) is a **single MySQL
schema with a `StoreId` column on every tenant-owned table**, enforced
automatically so a developer can never forget it.

1. Give tenant entities a `StoreId`:
   ```csharp
   public interface ITenantEntity { Guid StoreId { get; set; } }
   ```
2. Resolve the current tenant per request (from the JWT claim `store_id`):
   ```csharp
   public interface ICurrentTenant { Guid StoreId { get; } }
   ```
3. Apply a **global query filter** in `AppDbContext.OnModelCreating` so every
   query is automatically scoped:
   ```csharp
   modelBuilder.Entity<Product>().HasQueryFilter(p => p.StoreId == _tenant.StoreId);
   // ...repeat for every tenant entity (or loop over them with reflection)
   ```
4. Set `StoreId` automatically on insert by overriding `SaveChanges`.

> **Platform-level** entities span stores and are **not** tenant-filtered:
> Store, SuperAdmin, PlatformFeeConfig, Advertiser/Brand, and the **marketplace
> order graph** — `Order` (parent), `Cart`, `Driver`, `DeliveryAssignment`.
> Their tenant-owned children **are** filtered: `StoreOrder`, `OrderLine`, and
> `PickupTask` each carry a `StoreId`, so a store sees only its part of a shared
> order. In short, these entities are **not**
> tenant-filtered. The Super Admin console operates across all stores.

---

## 6. Domain Model → Tables

Create the entities from **PRD §7** in `Kirana.Domain`. Suggested build order
(matches the phases):

| Group | Entities |
|---|---|
| Tenancy & users | `Store`, `StoreSettings`, `User`, `Role`, `Permission` |
| Catalog & stock | `Product`, `Category`, `UnitOfMeasure`, `Batch`, `StockLedger`, `StockAdjustment` |
| Purchasing | `Supplier`, `PurchaseOrder`, `GoodsReceipt`, `PurchaseInvoice`, `PurchaseReturn` |
| Sales / POS | `SalesInvoice`, `SalesLine`, `Payment`, `SalesReturn`, `Customer` |
| Marketplace / orders | `Cart`, `CartLine`, `Order` (parent), `StoreOrder` (per-store part), `OrderLine`, `OrderStatusHistory`, `StoreOrderStatusHistory` |
| Delivery / driver | `Driver`, `DriverAvailability`, `DeliveryAssignment` (parent order), `PickupTask` (per store), `DeliveryStatusHistory`, `ProofOfDelivery`, `CODCollection` |
| Accounting | `Account`, `JournalEntry`, `LedgerPosting`, `TaxRate`, `Receivable`, `Payable` |
| Notifications | `NotificationTemplate`, `NotificationLog` |

Keep money as `decimal(18,2)`, use `Guid` primary keys, and add `CreatedAt` /
`UpdatedAt` audit columns to every entity.

---

## 7. Migrations (EF Core → MySQL)

From the **`backend/`** folder:
```bash
# create the first migration
dotnet ef migrations add InitialCreate \
  --project src/Kirana.Infrastructure --startup-project src/Kirana.Api

# apply it to MySQL
dotnet ef database update \
  --project src/Kirana.Infrastructure --startup-project src/Kirana.Api
```
Repeat `migrations add <Name>` each time the model changes. **Never** edit the
database by hand — always go through a migration so every environment stays in
sync.

---

## 8. Authentication & Roles

1. Use **ASP.NET Core Identity** for users/passwords, backed by MySQL.
2. On login, issue a **JWT** containing `user_id`, `store_id`, and `role`.
3. Protect endpoints with `[Authorize(Roles = "Owner,Manager")]` per the RBAC
   matrix in PRD §3.2.
4. The **Driver** is a role whose token is scoped to their own deliveries only
   (PRD §5.8) — enforce this in the delivery endpoints, not just in the UI.
5. Add MFA for Super Admin / Owner later (PRD NFR).

---

## 9. API Design

- One controller group per domain: `StoresController`, `ProductsController`,
  `PurchaseOrdersController`, `SalesController`, `OrdersController`,
  `DeliveriesController`, `AccountingController`, `AdminController`.
- Return **DTOs**, never EF entities directly.
- Validate inputs (FluentValidation or data annotations).
- Document everything with **Swagger** (browse at `/swagger` in Development).
- Use **SignalR hubs** for: live order status to customers, new-assignment
  push to drivers, and dashboard live tiles.

---

## 10. Suggested Build Order (maps to PRD phases)

> Build vertically — one module fully (entity → migration → API → basic UI/test)
> before the next. Ship a thin slice end-to-end early.

1. **Phase 0 — Foundation**
   - Solution skeleton, MySQL connection, `AppDbContext`, Identity + JWT.
   - `Store` registration endpoint → status `Pending`.
   - Super Admin: approve/reject → status `Active`.
   - Multi-tenant query filter working (verify two stores can't see each other).
2. **Phase 1 — Core Operations**
   - Catalog & inventory CRUD + stock ledger.
   - Purchase order → goods receipt → supplier bill (stock goes up).
   - POS sale endpoint (stock goes down, invoice created).
3. **Phase 2 — Online Marketplace**
   - **Unified catalog** read API aggregating all active stores' products/stock.
   - **Multi-store cart** (lines carry `StoreId`), single checkout, **one payment**.
   - On checkout: create the **parent `Order`** and **split** it into per-store `StoreOrder`s (this split is the heart of the model — test it hard).
   - Payment gateway integration (start with a sandbox), COD.
   - Order Manager: each store sees & advances only its `StoreOrder`; parent status derives from parts.
4. **Phase 3 — Delivery (multi-store pickup)**
   - Driver entity + Driver App APIs; assign the **parent order** to one driver.
   - Generate a **`PickupTask` per contributing store**; driver marks each store collected before "out for delivery".
   - Single delivery, POD, COD total reconcile (allocated per `StoreOrder`).
   - SignalR push to drivers; live tracking.
5. **Phase 4 — Finance**
   - Double-entry postings **per `StoreOrder`** (each store's books) + POS/purchases; GST & core reports.
6. **Phase 5 — Monetize**
   - Platform-fee accrual per `StoreOrder`, settlements/payouts; advertiser accounts, ad campaigns/placements, ad billing.
7. **Phase 6 — Scale**
   - Multi-outlet, offline POS sync, analytics, pickup-route batching/optimization.

---

## 11. Running & Testing

```bash
# run the API (Swagger at https://localhost:5001/swagger)
dotnet run --project src/Kirana.Api

# run tests
dotnet test
```
- Write **unit tests** (xUnit + Moq) for business rules (stock math, ledger
  postings, tenant isolation).
- Write **integration tests** against a throwaway MySQL database.
- Add a CI pipeline (GitHub Actions) that runs `dotnet build` + `dotnet test`
  on every push. (Can add a `SessionStart` hook / workflow later.)

---

## 12. Coding Conventions

- C# nullable reference types **on**; treat warnings as errors in CI.
- Async all the way (`async`/`await`, `CancellationToken` on service calls).
- Repository/Service pattern; keep controllers thin.
- One migration per logical schema change, with a descriptive name.
- Never log secrets, card data, or full tokens.
- Keep `store_id` filtering in the data layer — do not rely on the UI for isolation.

---

## 13. First Week Checklist

- [ ] Install prerequisites (§1) and clone the repo.
- [ ] Create the solution + projects (§2) and add packages (§3).
- [ ] Get MySQL running and connected (§4).
- [ ] Add `Store` + `User` entities, first migration, `database update` (§5–7).
- [ ] Registration + approval endpoints working end-to-end.
- [ ] Login returns a JWT with `store_id`; a protected endpoint honours it.
- [ ] Two test stores cannot see each other's data (tenant isolation proven).

Once this slice works, you have the backbone — every other module plugs into
the same pattern.
