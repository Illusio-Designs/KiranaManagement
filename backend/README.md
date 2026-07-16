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
1. `GET /api/geo/countries` → `…/{countryId}/states` → `…/states/{stateId}/cities`
   (seeded India data) to get ids for the location cascade.
2. `POST /api/auth/register-store` — a store self-registers with **GSTIN, PAN**
   and `countryId/stateId/cityId` → status `Pending`.
3. `POST /api/stores/{storeId}/documents` (multipart `file` + `documentType`) —
   upload KYC docs during onboarding.
4. `POST /api/auth/login` as the **SuperAdmin** → copy the JWT.
5. `GET /api/admin/stores/pending`, review docs at
   `GET /api/admin/stores/{id}/documents`, then `POST /api/admin/stores/{id}/approve`.
6. `POST /api/auth/login` (or **`POST /api/auth/google`** with a Google ID token)
   as the **store owner** → JWT carrying `store_id`.
7. With the owner token, `POST /api/products` then `GET /api/products` — you only
   ever see your own store's products (tenant isolation).

**Customer (marketplace) auth — phone OTP:**
- `POST /api/customer-auth/request-otp` `{ "phone": "+9199..." }` — in dev the code
  is **printed to the console** (dev OTP sender; wire a real SMS provider later).
- `POST /api/customer-auth/verify-otp` `{ "phone": "...", "code": "123456" }` → customer JWT.

**Config for optional integrations:** set `Google:ClientId` to enable Google
Sign-In; `Storage:BasePath` controls where uploaded documents are saved.

Run tests: `dotnet test`

## Phase 1 — Catalog (variants + MRP/discount), Inventory, Cart

Also implemented:

**Catalog (owner/manager, tenant-scoped):**
- `POST /api/categories`, `GET /api/categories`
- `POST /api/products` — a product with one or more **variants**; each variant has
  **MRP + selling price** (discount = MRP − selling price), unit/pack size, tax, stock.
- `GET /api/products`, `GET /api/products/{id}`, `POST /api/products/{id}/variants`,
  `PUT /api/variants/{id}`.

**Inventory (owner/manager/stock-clerk):**
- `POST /api/inventory/variants/{id}/adjust` (writes a stock-ledger entry),
  `GET /api/inventory/low-stock`, `GET /api/inventory/variants/{id}/ledger`.

**Marketplace + Cart (customer):**
- `GET /api/marketplace/stores/{storeId}/products` — browse a store's active catalog
  (cross-store, anonymous).
- Cart (customer JWT from OTP login): `GET /api/cart`, `POST /api/cart/items`
  (`{storeId, productVariantId, quantity}`), `PUT/DELETE /api/cart/items/{id}`,
  `DELETE /api/cart`. The summary groups items **by store** and returns MRP total,
  **total discount**, and an **estimated delivery fee** (config `Delivery` section;
  the real quote comes from a 3PL at checkout) plus the grand total.

**POS billing (owner/manager/cashier):**
- `POST /api/sales` — ring up a sale: `{ paymentMode, amountPaid?, lines: [{ productVariantId,
  quantity, unitPriceOverride? }] }`. Prices are tax-inclusive; the invoice returns
  taxable total, **GST component**, total discount vs MRP, grand total, and change due,
  and **stock is decremented** (with ledger entries). `GET /api/sales`, `GET /api/sales/{id}`.

**Purchase (owner/manager/stock-clerk):**
- Suppliers: `POST /api/suppliers`, `GET /api/suppliers`.
- Purchase orders: `POST /api/purchase-orders` (`{ supplierId, notes?, lines: [{ productVariantId,
  quantity, unitCost }] }`) → draft; `GET`, `GET /{id}`; `POST /api/purchase-orders/{id}/receive`
  — goods receipt that **increases stock** (with ledger entries) and marks the PO received.

## Phase 2 — Checkout & marketplace orders

Turns a customer cart into **one parent order split into per-store parts**.

**Checkout & order tracking (customer JWT):**
- `POST /api/checkout` `{ contactName, contactPhone, addressLine, city?, pincode?, paymentMode }`
  → creates **one `Order`** (single order number) split into a **`StoreOrder` per store**,
  decrements each store's stock (ledger), mocks online payment (or COD for Cash), clears the cart.
- `GET /api/orders`, `GET /api/orders/{id}` — the customer's orders with per-store groups.

**Store order queue (owner/manager/cashier/stock-clerk) — tenant-scoped:**
- `GET /api/store-orders?status=` — only this store's parts.
- `POST /api/store-orders/{id}/accept | /pack | /ready | /cancel` — advance the store's part;
  cancel restocks. The **parent order status is recomputed** from all parts
  (`Placed → Preparing → ReadyForPickup`; all-cancelled → `Cancelled`).

> Payment and delivery are mocked/estimated here; real payment gateway and **3PL
> delivery** land in Phase 3 (see [../docs/DELIVERY_INTEGRATIONS.md](../docs/DELIVERY_INTEGRATIONS.md)).

## Guides

## Guides
- [STRUCTURE.md](STRUCTURE.md) — detailed backend directory structure: what
  every project and folder holds, mapped to the PRD domains, plus a request-flow
  example and conventions.
- [../docs/DEVELOPMENT.md](../docs/DEVELOPMENT.md) — setup, packages,
  multi-tenancy, migrations, and the phase-by-phase build order.
