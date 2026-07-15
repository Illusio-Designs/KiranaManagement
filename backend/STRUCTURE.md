# Backend Directory Structure

A detailed, opinionated layout for the **ASP.NET Core + EF Core + MySQL**
solution. It follows a **clean layered architecture** (Api → Application →
Domain, Infrastructure → Application/Domain) so every PRD domain has an obvious
home and multi-tenancy is enforced in one place.

> Rule of thumb: **Domain** = *what the business is* (entities, rules),
> **Application** = *what the app does* (use cases/services, DTOs),
> **Infrastructure** = *how it talks to the outside* (MySQL, email, payments),
> **Api** = *how the world talks to it* (HTTP controllers, SignalR).

---

## Top-level

```
backend/
 ├─ KiranaManagement.sln
 ├─ Directory.Build.props          # shared C# settings (nullable, langversion, warnings-as-errors)
 ├─ .editorconfig                  # code style
 ├─ global.json                    # pin the .NET SDK version
 ├─ src/
 │   ├─ Kirana.Api/
 │   ├─ Kirana.Application/
 │   ├─ Kirana.Domain/
 │   └─ Kirana.Infrastructure/
 └─ tests/
     ├─ Kirana.UnitTests/
     └─ Kirana.IntegrationTests/
```

---

## 1. `Kirana.Domain` — the core (no dependencies)

Pure C# entities, enums, and domain rules. No EF, no ASP.NET here.

```
Kirana.Domain/
 ├─ Common/
 │   ├─ BaseEntity.cs             # Id (Guid), CreatedAt, UpdatedAt
 │   ├─ ITenantEntity.cs          # StoreId — marks a row as tenant-owned
 │   └─ Enums/
 │       ├─ StoreStatus.cs        # Pending, Active, Suspended, Rejected, Closed
 │       ├─ OrderStatus.cs        # New, Accepted, Packed, Ready, OutForDelivery, Delivered, Cancelled
 │       ├─ DeliveryStatus.cs     # Assigned, Accepted, PickedUp, OutForDelivery, Delivered, Failed
 │       ├─ PaymentMode.cs        # Cash, UPI, Card, Wallet, StoreCredit, COD
 │       └─ PaymentStatus.cs
 ├─ Platform/                     # NOT tenant-scoped
 │   ├─ Store.cs                  # incl. GSTIN + PAN + country/state/city
 │   ├─ StoreDocument.cs          # KYC uploads (onboarding)
 │   ├─ StoreSettings.cs
 │   ├─ PlatformFeeConfig.cs      # commission % and/or flat per-order fee
 │   ├─ FeeLedger.cs              # accrued platform fees per order
 │   ├─ Settlement.cs
 │   ├─ Payout.cs
 │   └─ AuditLog.cs
 ├─ Geo/                          # seeded reference data (registration cascade)
 │   ├─ Country.cs
 │   ├─ State.cs
 │   └─ City.cs
 ├─ Customers/                    # marketplace shoppers (phone-OTP auth)
 │   ├─ Customer.cs
 │   └─ OtpCode.cs
 ├─ Advertising/                  # NOT tenant-scoped (brands span stores)
 │   ├─ Advertiser.cs             # a store or a brand
 │   ├─ Brand.cs
 │   ├─ AdCampaign.cs
 │   ├─ AdCreative.cs
 │   ├─ AdPlacement.cs
 │   ├─ AdImpression.cs
 │   ├─ AdClick.cs
 │   └─ AdvertiserWallet.cs
 ├─ Identity/
 │   ├─ User.cs
 │   ├─ Role.cs
 │   └─ Permission.cs
 ├─ Catalog/
 │   ├─ Product.cs
 │   ├─ Category.cs
 │   ├─ UnitOfMeasure.cs
 │   └─ Batch.cs
 ├─ Inventory/
 │   ├─ StockLedger.cs
 │   └─ StockAdjustment.cs
 ├─ Purchasing/
 │   ├─ Supplier.cs
 │   ├─ PurchaseOrder.cs
 │   ├─ GoodsReceipt.cs
 │   ├─ PurchaseInvoice.cs
 │   └─ PurchaseReturn.cs
 ├─ Sales/
 │   ├─ SalesInvoice.cs
 │   ├─ SalesLine.cs
 │   ├─ Payment.cs
 │   ├─ SalesReturn.cs
 │   └─ Customer.cs
 ├─ Orders/                       # marketplace: parent order spans stores (Order/Cart are platform-level)
 │   ├─ Cart.cs                   # platform-level; lines reference StoreId + ProductId
 │   ├─ CartLine.cs
 │   ├─ Order.cs                  # PARENT — one order number, one payment, customer + address
 │   ├─ StoreOrder.cs            # per-store PART of a parent order (tenant-scoped: StoreId)
 │   ├─ OrderLine.cs             # belongs to a StoreOrder
 │   ├─ OrderStatusHistory.cs
 │   └─ StoreOrderStatusHistory.cs
 ├─ Delivery/                     # platform-level; 3PL-first
 │   ├─ DeliveryProvider.cs       # a configured 3PL partner (Porter/Borzo/...)
 │   ├─ DeliveryProviderConfig.cs
 │   ├─ DeliveryTask.cs           # booked delivery for a parent order: provider, externalTaskId, trackingUrl, fee, state
 │   ├─ PickupPoint.cs            # one per contributing store on the task
 │   ├─ DeliveryStatusHistory.cs
 │   ├─ DeliveryWebhookEvent.cs   # raw inbound partner events (idempotency/audit)
 │   ├─ ProofOfDelivery.cs
 │   ├─ CODCollection.cs          # total for order, allocated per StoreOrder
 │   ├─ Driver.cs                 # OPTIONAL own-fleet fallback
 │   └─ DriverAvailability.cs     # OPTIONAL own-fleet fallback
 └─ Accounting/
     ├─ Account.cs               # chart of accounts
     ├─ JournalEntry.cs
     ├─ LedgerPosting.cs
     ├─ TaxRate.cs
     ├─ Receivable.cs
     └─ Payable.cs
```

> Folders here mirror **PRD §7 (Data Model)** one-to-one, so the model and the
> code stay in sync.

---

## 2. `Kirana.Application` — use cases (depends on Domain only)

Business operations, DTOs, validation, and the **interfaces** that
Infrastructure implements. One folder per feature; each holds its service, DTOs,
and validators.

```
Kirana.Application/
 ├─ Common/
 │   ├─ Interfaces/
 │   │   ├─ IAppDbContext.cs       # abstraction over the EF DbContext
 │   │   ├─ ICurrentTenant.cs      # exposes current StoreId (from JWT)
 │   │   ├─ ICurrentUser.cs        # current user id / role
 │   │   ├─ IEmailSender.cs
 │   │   ├─ ISmsSender.cs
 │   │   ├─ IPaymentGateway.cs
 │   │   ├─ IDeliveryProvider.cs   # 3PL courier seam: getQuote/createTask/cancel/track — Porter/Borzo/Shiprocket
 │   │   ├─ IMapProvider.cs        # geocode / route / ETA — Google Maps or Ola Maps behind one seam
 │   │   ├─ IPushSender.cs         # FCM / APNs push to driver & customer apps
 │   │   └─ IJwtTokenService.cs
 │   ├─ Models/                    # Result<T>, PagedResult<T>, error types
 │   └─ Behaviors/                 # validation / logging cross-cutting
 ├─ Stores/            (register, approve, reject — PRD §5.1)
 │   ├─ StoreService.cs
 │   ├─ Dtos/  (RegisterStoreDto, StoreDto, ApproveStoreDto ...)
 │   └─ Validators/
 ├─ Auth/              (login, refresh, roles — PRD §5.2, §8)
 ├─ Catalog/           (products, categories — PRD §5.3)
 ├─ Inventory/         (stock ledger, adjustments — PRD §5.3)
 ├─ Purchasing/        (PO, GRN, bills — PRD §5.4)
 ├─ Sales/             (POS billing, returns — PRD §5.5)
 ├─ Marketplace/       (unified catalog aggregation, multi-store cart, checkout, order SPLIT into StoreOrders — PRD §5.6)
 ├─ Orders/            (parent order + per-store parts, order manager — PRD §5.7)
 ├─ Delivery/          (3PL dispatch: quote/book/track/webhook + POD/COD; optional own-driver — PRD §5.8)
 ├─ Accounting/        (postings, GST, reports — PRD §5.9)
 ├─ Reporting/         (dashboards, analytics — PRD §5.10)
 ├─ Notifications/     (templates, dispatch — PRD §5.11)
 ├─ Monetization/      (platform-fee config, accrual, settlements/payouts — PRD §1.6, §5.12–5.13)
 └─ Advertising/       (advertisers/brands, campaigns, placements, billing — PRD §5.13)
```

**Pattern per feature folder:**
- `XxxService.cs` — the use cases (async methods, take/return DTOs).
- `Dtos/` — request/response objects (never expose Domain entities over HTTP).
- `Validators/` — FluentValidation rules for the DTOs.

---

## 3. `Kirana.Infrastructure` — external world (depends on Application + Domain)

EF Core, MySQL, migrations, and third-party integrations. This is where the
**multi-tenant query filter** lives.

```
Kirana.Infrastructure/
 ├─ Persistence/
 │   ├─ AppDbContext.cs            # implements IAppDbContext; DbSet<> for every entity
 │   ├─ Configurations/           # IEntityTypeConfiguration<T> per entity (keys, indexes, decimals)
 │   │   ├─ ProductConfiguration.cs
 │   │   ├─ OrderConfiguration.cs
 │   │   └─ ...
 │   ├─ Interceptors/
 │   │   └─ AuditableEntityInterceptor.cs   # sets CreatedAt/UpdatedAt + StoreId on save
 │   ├─ TenantQueryFilter.cs       # global filter: StoreId == currentTenant.StoreId
 │   ├─ Migrations/                # dotnet ef migrations output
 │   └─ Seed/
 │       └─ DbSeeder.cs            # default chart of accounts, roles, tax rates
 ├─ Identity/
 │   └─ JwtTokenService.cs         # implements IJwtTokenService
 ├─ Integrations/
 │   ├─ Payments/  (RazorpayGateway.cs / StripeGateway.cs : IPaymentGateway)
 │   ├─ Sms/       (TwilioSmsSender.cs : ISmsSender)
 │   ├─ Email/     (SmtpEmailSender.cs : IEmailSender)
 │   ├─ Whatsapp/
 │   ├─ Otp/       (DevOtpSender.cs : IOtpSender — dev logs the code; MSG91/SMS later)
 │   ├─ Delivery/  (PorterProvider.cs / BorzoProvider.cs / ShiprocketProvider.cs : IDeliveryProvider — see docs/DELIVERY_INTEGRATIONS.md)
 │   ├─ Maps/      (GoogleMapsProvider.cs / OlaMapsProvider.cs : IMapProvider — see docs/DRIVER_APP_INTEGRATIONS.md)
 │   └─ Push/      (FcmPushSender.cs / ApnsPushSender.cs : IPushSender)
 ├─ Storage/                        # LocalFileStorage.cs : IFileStorage (blob/S3 later)
 ├─ Identity/ … + GoogleTokenValidator.cs (IGoogleTokenValidator), JwtTokenService (+ customer tokens)
 ├─ Jobs/                          # Hangfire background jobs (notifications, sync, reports)
 └─ DependencyInjection.cs         # AddInfrastructure() — registers DbContext + services
```

---

## 4. `Kirana.Api` — HTTP surface (depends on Application + Infrastructure)

Thin controllers, SignalR hubs, middleware, and startup wiring.

```
Kirana.Api/
 ├─ Program.cs                     # builder + middleware pipeline
 ├─ appsettings.json
 ├─ appsettings.Development.json   # connection string, JWT (use user-secrets for real values)
 ├─ Controllers/
 │   ├─ StoresController.cs        # register, my-store
 │   ├─ AdminController.cs         # approve/reject stores, platform admin
 │   ├─ AuthController.cs          # login, refresh
 │   ├─ ProductsController.cs
 │   ├─ InventoryController.cs
 │   ├─ SuppliersController.cs
 │   ├─ PurchaseOrdersController.cs
 │   ├─ SalesController.cs         # POS billing, returns
 │   ├─ OrdersController.cs        # online orders, order manager
 │   ├─ DeliveriesController.cs    # 3PL dispatch/track + optional own-driver endpoints
 │   ├─ DeliveryWebhooksController.cs  # POST /api/webhooks/delivery/{provider} — verify sig, dedupe, update status
 │   ├─ AccountingController.cs
 │   ├─ ReportsController.cs
 │   ├─ AdvertisingController.cs  # advertisers, campaigns, placements, ad billing
 │   └─ SettlementsController.cs  # platform-fee statements & payouts (admin)
 ├─ Hubs/
 │   ├─ OrdersHub.cs               # live order status to customers/dashboard
 │   └─ DeliveryHub.cs             # assignment push + live driver location
 ├─ Middleware/
 │   ├─ TenantResolutionMiddleware.cs   # reads store_id claim -> ICurrentTenant
 │   └─ ExceptionHandlingMiddleware.cs  # maps errors to consistent JSON responses
 ├─ Filters/                       # authorization, validation filters
 └─ Extensions/
     └─ ServiceCollectionExtensions.cs  # AddApiServices(), Swagger, JWT, CORS, SignalR
```

---

## 5. `tests/`

```
tests/
 ├─ Kirana.UnitTests/              # business rules: stock math, ledger postings, tenant filter
 │   ├─ Sales/
 │   ├─ Accounting/
 │   └─ Delivery/
 └─ Kirana.IntegrationTests/       # spin up API + test MySQL, hit real endpoints
     ├─ Stores/  (register -> approve -> login flow)
     └─ Orders/  (order -> assign driver -> deliver)
```

---

## How a request flows (example: create a POS sale)

```
HTTP POST /api/sales
      │
      ▼
SalesController            (Kirana.Api)      ← validates model, calls the service
      │
      ▼
SalesService.CreateSale()  (Application)     ← business rules: check stock, price, tax
      │
      ├── IAppDbContext     → AppDbContext (Infrastructure) → MySQL   (stock ↓, invoice saved)
      ├── posts to Accounting service                                 (ledger entries)
      └── OrdersHub / notifications                                    (receipt via SMS/WhatsApp)
      │
      ▼
returns SaleDto → 201 Created
```

Every DB read/write passes through the **tenant query filter**, so a store can
only ever touch its own rows.

---

## Conventions recap

- **Add a feature** → create a folder in `Application/<Feature>`, entities in
  `Domain/<Feature>`, a controller in `Api/Controllers`, and a config +
  migration in `Infrastructure/Persistence`.
- **Controllers are thin** — no business logic, no EF queries. They call services.
- **DTOs cross the wire**, never Domain entities.
- **`StoreId` filtering lives in the data layer** — never trust the client.
- **One migration per schema change**, descriptive name.
- **Money = `decimal(18,2)`**, **IDs = `Guid`**, **async everywhere**.

See [../docs/DEVELOPMENT.md](../docs/DEVELOPMENT.md) for setup commands and the
phase-by-phase build order, and [../docs/PRD.md](../docs/PRD.md) for the full
requirements.
