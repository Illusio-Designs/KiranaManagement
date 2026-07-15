# Product Requirements Document (PRD)
## Multi-Store Grocery Management System ("KiranaManagement")

| Field | Value |
|---|---|
| **Document Title** | PRD — Multi-Store Grocery Management System |
| **Product Name** | KiranaManagement (working title) |
| **Version** | 1.0 (Draft) |
| **Status** | Draft for Review |
| **Authors** | Mansi, Hiral, Zigma |
| **Last Updated** | 2026-07-15 |
| **Reviewers** | Product, Engineering, Design, Business |

---

## 1. Overview

### 1.1 Summary
KiranaManagement is a **multi-tenant SaaS platform** that lets any grocery / kirana store **self-register**, get **approved by a platform administrator**, and then run their entire business from one place: **inventory, purchasing, in-store sales (POS), online storefront sales, order management, delivery, and accounting**.

The platform ships as **three connected surfaces** around a shared backend:
1. **Customer Online-Order App** — a **single marketplace**: customers browse a **unified catalog aggregated across all stores' live inventory**, add items **from any number of stores into one cart**, and place **one order with one order number**. They order from the *app/platform*, not store-by-store.
2. **Driver App** — a driver is assigned that single order, **collects the items from every store involved in it, consolidates them, and delivers them together to the customer under the same order number**.
3. **Store Management Dashboard** — store owners/staff run operations (catalog, inventory, purchase, POS, order fulfilment, delivery dispatch, accounting, reports). For any marketplace order, each store sees and fulfils **only its own portion** (its pick-list) of that shared order.

### 1.1.1 Marketplace Ordering Model (key concept)
- One customer order (**one order number**) may contain items sourced from **multiple stores**.
- Internally the order is split into per-store **fulfilment parts** (sub-orders / pick-lists), one per contributing store, so each store prepares only its items and each store's sales, stock, and platform fee post to that store.
- A **single delivery** consolidates all parts: the driver does a **multi-pickup run** (one stop per store), then **one drop** to the customer, all tracked under the same parent order number.
- The customer experiences one basket, one payment, one order, one delivery — the multi-store split is invisible to them.

Each store operates as an isolated tenant with its own data, staff, catalog, and books, while a central **Super Admin** governs onboarding, approvals, fees, and platform-wide health. Marketplace orders are **platform-level** and span stores; each store is exposed only to its own part.

### 1.2 Problem Statement
Small and mid-sized grocery stores today run on paper registers, disconnected spreadsheets, or point solutions that don't talk to each other. As a result they:
- Lose track of stock and expiry, causing wastage and stockouts.
- Cannot reconcile purchases against supplier bills accurately.
- Miss out on online sales because setting up e-commerce is complex.
- Struggle with GST-compliant accounting and month-end closing.
- Have no consolidated view when they run more than one outlet.

There is no affordable, easy-to-adopt system that unifies **operations + online selling + accounting** for the neighbourhood grocery segment.

### 1.3 Vision
Give every grocery store — from a single kirana shop to a multi-outlet chain — an all-in-one, mobile-first platform to **sell more, waste less, and stay compliant**, with zero technical setup.

### 1.4 Goals & Objectives
| # | Goal | Objective |
|---|---|---|
| G1 | Frictionless onboarding | A store can register and be operational within 1 business day of approval. |
| G2 | Unified operations | One system for inventory, purchase, POS, and online orders — no double entry. |
| G3 | Online reach | Every approved store gets a ready-to-use online storefront. |
| G4 | Financial clarity | Auto-generated, GST-ready books and reports with minimal manual effort. |
| G5 | Trust & control | Approval-gated onboarding and strict per-store data isolation. |

### 1.5 Success Metrics (KPIs)
- **Onboarding**: ≥ 80% of registered stores complete verification; median approval time < 24h.
- **Activation**: ≥ 60% of approved stores record their first sale within 7 days.
- **Engagement**: ≥ 50% of active stores use both offline POS *and* online orders monthly.
- **Reliability**: 99.9% uptime; POS billing works in degraded/offline mode.
- **Accounting**: ≥ 70% of stores generate a monthly P&L / GST summary from the system.
- **Retention**: ≥ 85% month-over-month store retention after 90 days.

### 1.6 Business / Monetization Model
The platform is **not subscription-based**. There are **no fixed monthly plans** for
stores. Revenue comes from two streams:

1. **Platform Fee** — a fee the platform earns on business the platform enables,
   charged to the **store**. Configurable by the Super Admin as either:
   - a **commission** (percentage of order value, typically on **online orders**), and/or
   - a **flat per-transaction / per-order fee**.
   Fees are accrued per order, netted against store settlements/payouts, and
   visible to the store in a transparent fee statement.

2. **Advertising Revenue** — paid promotion within the customer app / storefront,
   sold to:
   - **Stores** — promote their own products/store to nearby customers (e.g.
     sponsored listings, top-of-search placement, banners).
   - **Brands** — pay to promote their products across many stores (e.g. a brand
     boosts its items in the category feed and product pages).

Implication for the product: onboarding stays free/low-friction (drives store
count), and monetization scales with **transactions and ad inventory**, not seats.
Advertising is a distinct capability with its own console, targeting, and billing.

> Because there is no subscription gate, the Super Admin console manages **fee
> configuration, settlements/payouts, and ad campaigns/billing** instead of plans.

---

## 2. Scope

### 2.1 In Scope (v1)
- Store self-registration and admin approval workflow.
- Multi-tenant store management with role-based access.
- Product catalog & inventory management (batches, expiry, units).
- Purchase management (suppliers, purchase orders, goods receipt, supplier bills).
- In-store Sales / POS (billing, discounts, multiple payment modes, returns).
- **Customer Online-Order App (marketplace)** — unified catalog across all stores, multi-store cart, single checkout/payment, one order number, order tracking.
- Order management — one **parent order** split into per-store **parts (pick-lists)**; each store fulfils only its part.
- **Driver App & delivery dispatch** — assign one order to a driver for **multi-store pickup + single delivery**, live status, proof of delivery.
- Accounting (ledgers, GST, receivables/payables, P&L, reports).
- Notifications (email/SMS/WhatsApp/in-app/push).
- Platform Super Admin console.

### 2.2 Out of Scope (v1 — candidates for later)
- Full-blown WMS / multi-warehouse logistics optimization.
- Third-party marketplace (Amazon/Flipkart) listing sync.
- Third-party fleet aggregator integration (own driver app is **in scope**; external logistics APIs are later).
- Advanced route optimization across multiple drivers/orders (basic assignment is in scope).
- Advanced demand forecasting / AI replenishment.
- Loyalty program engine (basic points only, if any).
- Payroll / HR management.

### 2.3 Assumptions
- Stores have at least one smartphone; POS may also run on a tablet/desktop browser.
- Primary market is India (GST, INR, UPI) — architecture stays localizable.
- Internet is intermittent; POS must tolerate short offline windows.

---

## 3. User Personas & Roles

### 3.1 Personas
| Persona | Description | Key Needs |
|---|---|---|
| **Platform Super Admin** | Operates the SaaS platform. | Approve stores, monitor platform, manage billing/plans. |
| **Store Owner** | Registers and owns a store (tenant). | Full control of their store, reports, staff, finances. |
| **Store Manager** | Runs day-to-day operations. | Inventory, purchasing, order fulfilment, reports (no billing/plan control). |
| **Cashier / POS Operator** | Bills customers at the counter. | Fast billing, returns, cash handling. |
| **Inventory / Stock Clerk** | Manages stock and receiving. | Stock in/out, goods receipt, stock counts. |
| **Accountant** | Manages the books. | Ledgers, GST, receivables/payables, reconciliation. |
| **Online Customer** | Shops the marketplace app across all stores. | One unified catalog, one cart spanning multiple stores, one order/payment, one delivery, order tracking. |
| **Delivery Driver** | Fulfils a marketplace order end-to-end. | See the assigned order, a **multi-store pickup list** (which items from which store), navigate store-to-store, collect & consolidate, deliver together, capture proof of delivery, track earnings. |

### 3.2 Role → Permission Matrix (high level)
| Capability | Super Admin | Owner | Manager | Cashier | Stock Clerk | Accountant |
|---|:--:|:--:|:--:|:--:|:--:|:--:|
| Approve/reject stores | ✅ | — | — | — | — | — |
| Manage store profile & staff | — | ✅ | ◎ | — | — | — |
| Catalog & inventory | — | ✅ | ✅ | View | ✅ | View |
| Purchases & suppliers | — | ✅ | ✅ | — | ✅ | View |
| POS billing & returns | — | ✅ | ✅ | ✅ | — | — |
| Online order fulfilment | — | ✅ | ✅ | ◎ | ◎ | — |
| Accounting & reports | — | ✅ | View | — | — | ✅ |
| Billing/subscription | — | ✅ | — | — | — | — |

✅ Full · ◎ Configurable · View = read-only · — None

> Roles and permissions are configurable per store; the matrix above is the recommended default.
>
> **Delivery Driver** is a separate app role scoped to *only* their assigned deliveries — they can view assigned order details (customer name, address, items, amount to collect for COD), update delivery status, and capture proof of delivery. They have **no** access to catalog, inventory, pricing, accounting, or other stores' data.

---

## 4. User Journeys

### 4.1 Store Onboarding & Approval
1. Store owner visits the platform and clicks **Register your store**.
2. Fills business details (store name, owner, address, GSTIN, category, contact) and uploads KYC/business documents.
3. Submits → account state becomes **Pending Approval**; owner gets a confirmation.
4. Super Admin reviews the application in the admin console.
5. Super Admin **approves** (store becomes **Active**) or **rejects/requests-more-info** (owner is notified with reason).
6. On approval, the store is provisioned (default settings, sample catalog optional, storefront URL) and the owner completes a **setup wizard** (taxes, payment methods, staff, first products).

### 4.2 Daily In-Store Sale (POS)
1. Cashier logs in → opens POS.
2. Scans/searches items → cart builds with price & tax.
3. Applies discount / coupon if any.
4. Selects payment mode (cash / UPI / card / credit).
5. Confirms → invoice generated, stock decremented, sale posted to accounts, receipt shared (print/SMS/WhatsApp).

### 4.3 Purchase / Restock
1. Manager creates a **Purchase Order** to a supplier.
2. Goods arrive → **Goods Receipt** records actual quantities & batches/expiry.
3. Supplier **bill** is entered/attached → payable created, stock increased.
4. Payment recorded against the bill.

### 4.4 Marketplace Order (Customer App)
1. Customer opens the app and browses the **unified catalog across all nearby stores' live inventory**; each item shows which store it comes from.
2. Customer adds items to **one cart** — items may come from **several stores** — and checks out.
3. Pays **once** (online or cash-on-delivery) for the whole cart.
4. The platform creates **one order with one order number** and **splits it into per-store parts** (one pick-list per contributing store).
5. Each contributing store sees only **its part** in its Order Manager and **accepts → picks/packs → ready**.
6. Once all parts are ready (or ready enough to start the run), the platform **assigns one driver** for the whole order (see §4.5).
7. Each store's items post to that store's inventory & accounts; the platform fee accrues per store. The customer gets live status for the single order and can track the driver.

### 4.5 Delivery — Multi-Store Pickup (Driver App)
1. The platform assigns the marketplace order to an available driver; the driver gets a **push notification**.
2. The driver's app shows **one order** with a **multi-stop pickup list**: each store's name, address, and the exact items to collect there, plus the customer's delivery address and COD amount (if any).
3. Driver navigates store-to-store and **collects each store's items**, marking each store's pickup **collected/verified** as they go (all under the same order number).
4. When all stores are collected, the order becomes **Out for Delivery**; driver makes **one drop** to the customer.
5. Customer sees live status (and optional driver location) for the single order.
6. Driver **marks delivered** with **proof of delivery** (OTP, photo, or signature); for COD, records total cash collected for the whole order.
7. Order closes; each store's portion + any COD posts to accounting; the driver's completed-delivery count/earnings update.

---

## 5. Functional Requirements

Each requirement is tagged with a priority: **P0** (must-have, v1), **P1** (should-have), **P2** (nice-to-have / later).

### 5.1 Store Registration & Approval
- **FR-1.1 (P0)** Public self-registration form capturing business identity, owner, contact, address, category, and GSTIN/tax IDs.
- **FR-1.2 (P0)** Document upload for verification (business proof, ID, GST certificate).
- **FR-1.3 (P0)** Store lifecycle states: `Pending → Active → Suspended → Rejected → Closed`.
- **FR-1.4 (P0)** Super Admin approval queue with approve / reject / request-more-info and mandatory reason on rejection.
- **FR-1.5 (P0)** Email/SMS notifications at every state change.
- **FR-1.6 (P1)** Re-submission flow for rejected/incomplete applications.
- **FR-1.7 (P1)** Configurable onboarding checklist / setup wizard post-approval.

### 5.2 Store (Tenant) & User Management
- **FR-2.1 (P0)** Each store is an isolated tenant; data never crosses stores.
- **FR-2.2 (P0)** Owner can invite staff and assign roles.
- **FR-2.3 (P0)** Role-based access control (RBAC) per §3.2, customizable per store.
- **FR-2.4 (P1)** Multi-outlet support: one owner can operate several stores under one account with a consolidated view.
- **FR-2.5 (P1)** Store profile & settings (business hours, tax config, payment methods, delivery zones, receipt template).

### 5.3 Catalog & Inventory
- **FR-3.1 (P0)** Product master: name, SKU/barcode, category, unit of measure, tax rate, purchase & selling price, images.
- **FR-3.2 (P0)** Stock tracking with quantity on hand, reorder level, and low-stock alerts.
- **FR-3.3 (P0)** Batch / lot and **expiry** tracking; near-expiry alerts.
- **FR-3.4 (P1)** Variants & bundles (e.g., pack sizes), weight-based items.
- **FR-3.5 (P1)** Stock adjustments and periodic stock-take/counts with variance report.
- **FR-3.6 (P1)** Bulk import/export of catalog (CSV/Excel).
- **FR-3.7 (P2)** Barcode generation and label printing.

### 5.4 Purchase Management
- **FR-4.1 (P0)** Supplier master (contact, GSTIN, payment terms).
- **FR-4.2 (P0)** Purchase Orders (draft → sent → partially/fully received → closed).
- **FR-4.3 (P0)** Goods Receipt Note (GRN) updating stock with batch/expiry.
- **FR-4.4 (P0)** Supplier bills / purchase invoices creating accounts **payable**.
- **FR-4.5 (P1)** Purchase returns / debit notes.
- **FR-4.6 (P1)** Landed cost / price history per supplier.

### 5.5 Sales — In-Store POS
- **FR-5.1 (P0)** Fast billing screen: barcode scan, quick search, keypad; cart with live totals & tax.
- **FR-5.2 (P0)** Discounts (line & bill level), coupons, price overrides (permissioned).
- **FR-5.3 (P0)** Multiple payment modes: cash, UPI, card, wallet, store credit; split payment.
- **FR-5.4 (P0)** GST-compliant invoice generation; print & digital receipt (SMS/WhatsApp/email).
- **FR-5.5 (P0)** Sales returns / refunds with stock and ledger reversal.
- **FR-5.6 (P0)** Real-time stock decrement on sale.
- **FR-5.7 (P1)** **Offline mode**: bill locally when internet drops; auto-sync on reconnect.
- **FR-5.8 (P1)** Held/parked bills; day-open/day-close cash reconciliation (Z-report).
- **FR-5.9 (P2)** Customer profiles & basic loyalty points.

### 5.6 Online Sales — Marketplace (Customer App)
- **FR-6.1 (P0)** **Unified catalog** aggregating **all approved, active stores' live inventory** into one browsable marketplace; each product shows its source store, price, and availability.
- **FR-6.2 (P0)** **Multi-store cart**: the customer adds items from **any number of stores** into a single cart.
- **FR-6.3 (P0)** Browse, search, and filter across stores (by category, price, store, availability, distance).
- **FR-6.4 (P0)** **Single checkout** for the whole cart: **one payment** (UPI/card/netbanking via gateway, or COD) covering items from all stores.
- **FR-6.5 (P0)** On checkout, create **one parent order with one order number** and automatically **split it into per-store parts** (sub-orders / pick-lists).
- **FR-6.6 (P0)** Real-time, per-store stock availability (no overselling); items from an out-of-stock/closed store are handled gracefully at cart/checkout.
- **FR-6.7 (P1)** Serviceability by delivery area (only show stores that can deliver to the customer's location); delivery charge, minimum order value, and slot selection at the **order** level.
- **FR-6.8 (P1)** Promotions/coupons (platform-wide and store/brand-sponsored).
- **FR-6.9 (P2)** Customer accounts with order history and one-tap reorder across stores.

### 5.7 Order Management (Parent Order + Per-Store Parts)
- **FR-7.1 (P0)** A **parent order** (single order number) holds one or more **store parts**; each store part is the unit each store fulfils.
- **FR-7.2 (P0)** **Store-part queue** in each store's dashboard showing only that store's items for the order.
- **FR-7.3 (P0)** Store-part lifecycle: `New → Accepted → Packed → Ready` (per store).
- **FR-7.4 (P0)** **Parent-order lifecycle**: `Placed → (parts being prepared) → Ready-for-Pickup → Assigned → Out-for-Delivery → Delivered/Completed → Cancelled/Returned`, derived from its parts + delivery state.
- **FR-7.5 (P0)** Each store part posts **its own** items to **its own** inventory & accounting; platform fee accrues per store part.
- **FR-7.6 (P0)** Assign the **whole parent order** to a single driver for a **multi-store pickup + single delivery** (see §5.8).
- **FR-7.7 (P1)** Customer notifications on parent-order status changes (customer never sees the internal split).
- **FR-7.8 (P1)** Item unavailability at pick time: substitution, partial fulfilment, or per-item refund — reflected on the parent order.
- **FR-7.9 (P1)** Manual (phone/WhatsApp) orders can also be captured and, if needed, span stores the same way.

### 5.8 Driver App & Delivery Management (Multi-Store Pickup)
- **FR-8.1 (P0)** Driver onboarding: platform/store adds a driver (name, phone, vehicle); driver logs into the Driver App.
- **FR-8.2 (P0)** Driver availability toggle (online/offline) and view of orders assigned to them.
- **FR-8.3 (P0)** **One assigned order = a multi-stop pickup list**: for each contributing store, show store name, address, and the exact items to collect there; plus the customer's delivery address and total COD to collect.
- **FR-8.4 (P0)** **Per-store pickup tracking**: driver marks each store's items **collected/verified**; the order can't go out for delivery until **all** stores are collected (or explicitly handled).
- **FR-8.5 (P0)** Delivery status flow: `Assigned → Accepted → Collecting (per-store) → All-Collected → Out-for-delivery → Delivered` (+ `Failed/Returned`).
- **FR-8.6 (P0)** **Proof of delivery**: delivery OTP, photo, and/or signature at the single drop.
- **FR-8.7 (P0)** COD handling: record **total** cash collected for the whole order; reconcile driver cash; allocate collection back to each store part.
- **FR-8.8 (P0)** Push notifications to the driver on new assignment / changes.
- **FR-8.9 (P1)** **Maps & turn-by-turn navigation** (Swiggy/Instacart-style) to each store then the customer, via a maps provider (Google Maps Platform / Ola Maps / Mapmyindia). See [DRIVER_APP_INTEGRATIONS.md](DRIVER_APP_INTEGRATIONS.md).
- **FR-8.10 (P1)** **Optimized multi-store pickup route** (order the stops efficiently) using a Directions/Route-Optimization API.
- **FR-8.11 (P1)** **Live driver location** streamed to the backend and shown to the customer on a map; **ETA** shown to the customer.
- **FR-8.12 (P1)** **Geofencing** to auto-detect arrival at a store / the customer and prompt the next action.
- **FR-8.13 (P1)** Driver dashboard: today's deliveries, stores visited, completed count, and earnings/collections summary.
- **FR-8.14 (P1)** Failed / partial pickup handling (a store can't fulfil): proceed with available items, flag the missing part, trigger refund/substitution on the parent order.
- **FR-8.15 (P2)** Batching multiple parent orders and pickup-route optimization across them.
- **FR-8.16 (P2)** Driver ratings/feedback from customers.

### 5.9 Accounting
- **FR-9.1 (P0)** Auto-posting of sales, purchases, returns, and payments to a double-entry ledger.
- **FR-9.2 (P0)** Chart of accounts with sensible retail defaults.
- **FR-9.3 (P0)** Accounts **Receivable** (customer credit) & **Payable** (suppliers) with aging.
- **FR-9.4 (P0)** **GST**: tax capture on sales/purchases; GST summary/output & input reports.
- **FR-9.5 (P0)** Core reports: Sales, Purchase, P&L, Cash/Bank book, Tax summary, Stock valuation.
- **FR-9.6 (P1)** Expense entry (rent, utilities, salaries).
- **FR-9.7 (P1)** Bank/cash reconciliation.
- **FR-9.8 (P1)** Export to CSV/PDF; accountant-friendly exports (e.g., Tally-compatible).
- **FR-9.9 (P2)** Financial year close & opening balance carry-forward.

### 5.10 Reporting & Analytics
- **FR-10.1 (P0)** Store dashboard: today's sales, orders, low stock, dues, deliveries in progress.
- **FR-10.2 (P1)** Trends: top products, sales by category/period, margins.
- **FR-10.3 (P1)** Multi-outlet consolidated dashboard for chain owners.
- **FR-10.4 (P2)** Scheduled report emails.

### 5.11 Notifications
- **FR-11.1 (P0)** Transactional notifications (approval, order status, delivery updates, low stock, dues) via email/SMS/WhatsApp/in-app/push.
- **FR-11.2 (P1)** Per-user notification preferences.

### 5.12 Platform Super Admin Console
- **FR-12.1 (P0)** Store approval queue and store directory with states.
- **FR-12.2 (P0)** Suspend/reactivate/close stores.
- **FR-12.3 (P0)** **Platform fee configuration** — set commission % and/or flat
  per-order fee (globally, per category, or per store).
- **FR-12.4 (P1)** **Settlements & payouts** — per-store fee statements, netting
  of platform fees against collections, and payout records.
- **FR-12.5 (P1)** Platform health, usage metrics, and audit logs.
- **FR-12.6 (P2)** Announcements/broadcasts to stores.

### 5.13 Advertising & Platform Monetization
Revenue module for promoted placements sold to stores and brands (PRD §1.6).
- **FR-13.1 (P1)** **Advertiser accounts** for stores and **brands** (brand is a
  platform-level advertiser that can span many stores).
- **FR-13.2 (P1)** **Ad campaigns** — create a campaign with budget, duration,
  target (category, location/pincode, store), and creative (image/text).
- **FR-13.3 (P1)** **Ad placements** in the customer app: sponsored product
  listings, top-of-search/category slots, and home/banner slots.
- **FR-13.4 (P1)** **Platform fee accrual** — record the platform fee on each
  eligible (online) order into a fee ledger for settlement.
- **FR-13.5 (P1)** Advertiser billing — charge for ads (prepaid wallet or
  invoiced spend) with campaign spend tracking.
- **FR-13.6 (P2)** Ad performance metrics — impressions, clicks, and attributed
  orders per campaign.
- **FR-13.7 (P2)** Ad review/approval by Super Admin before a campaign goes live.
- **FR-13.8 (P2)** Frequency capping and clearly labelled "Sponsored" placements.

---

## 6. Non-Functional Requirements
| Category | Requirement |
|---|---|
| **Performance** | POS actions < 300ms perceived; page loads < 2s on 3G; support 100+ concurrent stores in v1. |
| **Availability** | 99.9% uptime target; POS degrades gracefully offline (FR-5.7). |
| **Scalability** | Horizontally scalable, multi-tenant; grow to thousands of stores. |
| **Security** | Encryption in transit (TLS) and at rest; RBAC; per-tenant data isolation; secure auth (MFA for admins). |
| **Privacy/Compliance** | GST-compliant invoicing; PII handling per applicable data-protection law; audit trails. |
| **Reliability** | Automated backups & point-in-time recovery; idempotent financial postings. |
| **Usability** | Mobile-first, low-training UI; multilingual-ready (English + regional). |
| **Observability** | Centralized logging, metrics, and alerting. |
| **Maintainability** | Modular services, documented APIs, automated tests & CI. |

---

## 7. Data Model (High-Level Entities)
- **Platform**: `SuperAdmin`, `PlatformFeeConfig`, `FeeLedger` (accrued platform fees), `Settlement`, `Payout`, `AuditLog`.
- **Advertising**: `Advertiser` (store or brand), `Brand`, `AdCampaign`, `AdCreative`, `AdPlacement`, `AdImpression`, `AdClick`, `AdvertiserWallet`.
- **Store/Tenant**: `Store`, `StoreSettings`, `StoreServiceArea` (delivery serviceability / zone), `User`, `Role`, `Permission`.
- **Catalog/Inventory**: `Product`, `Category`, `UnitOfMeasure`, `Batch`, `StockLedger`, `StockAdjustment`.
- **Purchasing**: `Supplier`, `PurchaseOrder`, `GoodsReceipt`, `PurchaseInvoice`, `PurchaseReturn`.
- **Sales**: `SalesInvoice`, `SalesLine`, `Payment`, `SalesReturn`, `Customer`.
- **Marketplace/Orders** (platform-level, span stores):
  - `Cart`, `CartLine` (each line references a `store_id` + `product_id`).
  - `Order` (**parent** — the single order number, customer, delivery address, total payment).
  - `StoreOrder` (**per-store part** of a parent order — belongs to one `store_id`; the unit each store fulfils and the unit that posts to that store's books).
  - `OrderLine` (belongs to a `StoreOrder`).
  - `OrderStatusHistory`, `StoreOrderStatusHistory`.
  - `Payment` (at the **parent-order** level, one payment for the whole cart).
- **Delivery/Driver**: `Driver`, `DriverAvailability`, `DeliveryAssignment` (assigns a **parent order** to a driver), `PickupTask` (**one per store** on that order — items to collect at a store + collected status), `DeliveryStatusHistory`, `ProofOfDelivery`, `CODCollection` (total for the order, allocated per `StoreOrder`).
- **Accounting**: `Account` (CoA), `JournalEntry`, `LedgerPosting`, `TaxRate`, `Receivable`, `Payable`.
- **Notifications**: `NotificationTemplate`, `NotificationLog`.

> **Tenancy note:** `Order`, `Cart`, `DeliveryAssignment`, and the `Driver` pool are **platform-level** (they span stores). `StoreOrder`, `OrderLine`, `PickupTask`, and all catalog/inventory/sales/accounting rows carry a `store_id` and are tenant-filtered — so each store sees only **its own part** of a shared order, never the whole basket or other stores' items.

---

## 8. System Architecture (Conceptual)
- **Clients (four surfaces over one backend)**:
  - **Store Management Dashboard** — web/mobile app for POS + operations + delivery dispatch.
  - **Customer Online-Order App** — customer PWA/mobile app: **unified marketplace** catalog, multi-store cart, single checkout, order tracking.
  - **Driver App** — mobile app for drivers (multi-store pickup list, navigation, proof of delivery).
  - **Super Admin Console** — platform governance.
- **Backend**: API services grouped by domain (Auth/Tenancy, Catalog/Inventory, Purchasing, Sales/POS, **Marketplace/Orders**, **Delivery/Dispatch**, Accounting, Monetization/Advertising, Notifications). A **Marketplace service** aggregates all stores' catalog/stock and orchestrates order splitting into per-store parts.
- **Data**: **MySQL** relational database. **Tenant-scoped** rows (catalog, inventory, `StoreOrder`, sales, accounting) carry `store_id` and are globally filtered; **platform-level** rows (`Order`, `Cart`, `Driver`, `DeliveryAssignment`, advertising) span stores. Object storage for documents/images.
- **Integrations**: Payment gateway (UPI/cards), SMS/WhatsApp/email providers, GST/e-invoicing (later).
- **Cross-cutting**: AuthN/AuthZ (RBAC + MFA), audit logging, background jobs (notifications, sync, reports), offline sync for POS.

### 8.1 Technology Stack
| Layer | Technology | Notes |
|---|---|---|
| **Backend / API** | **ASP.NET Core (C#) Web API** (.NET 8 LTS) | RESTful JSON APIs; modular by domain. |
| **ORM / Data access** | **Entity Framework Core** with the **Pomelo MySQL provider** | Code-first migrations; global query filter on `store_id` for tenant isolation. |
| **Database** | **MySQL 8.x** | Single multi-tenant schema with row-level `store_id`. |
| **Auth** | ASP.NET Core Identity + **JWT** (access/refresh tokens) | RBAC via roles/claims; MFA for admins. |
| **Store Dashboard (web)** | ASP.NET Core **MVC / Razor Pages**, or a SPA (React/Angular/Blazor) on the API | Team's choice; MVC/Razor is fastest for a .NET team. |
| **Customer Order App** | Responsive web (PWA) first; mobile app later | Consumes the same Web API. |
| **Driver App** | Mobile (PWA first, native/MAUI later) | Consumes the same Web API; push notifications. |
| **Real-time** | **SignalR** | Live order status, driver location, dashboard updates. |
| **Maps & Location (Driver App)** | **Google Maps Platform** (default) or Ola Maps / Mapmyindia | Maps SDK, geocoding, directions, distance-matrix, route optimization; behind an `IMapProvider` seam. See [DRIVER_APP_INTEGRATIONS.md](DRIVER_APP_INTEGRATIONS.md). |
| **Push notifications** | **FCM** (Android) + **APNs** (iOS) | Driver assignment & status alerts. |
| **Background jobs** | **Hangfire** (or `IHostedService`) | Notifications, offline-sync processing, scheduled reports. |
| **Caching** | In-memory / **Redis** (optional, for scale) | Sessions, hot catalog data. |
| **API docs** | **Swagger / OpenAPI** (Swashbuckle) | Contract for all client apps. |
| **Testing** | **xUnit** + Moq; integration tests on a test MySQL DB | CI-gated. |
| **Hosting** | Kestrel behind Nginx/IIS; Linux or Windows; Docker-friendly | Cloud VM or container platform. |

> This is the committed stack for v1: **ASP.NET Core Web API + Entity Framework Core + MySQL**, with SignalR for real-time and Hangfire for background work. See [DEVELOPMENT.md](DEVELOPMENT.md) for the developer setup and build guide.

---

## 9. Integrations
- **Payments**: UPI, cards, netbanking, wallets (via gateway); COD.
- **Messaging**: SMS, WhatsApp Business, Email.
- **Accounting/Tax**: GST reports now; e-invoicing / e-way bill and Tally export later.
- **Maps & Location (Driver App — Swiggy/Instacart-style)**: the driver app relies
  on a maps/location stack rather than any single "Swiggy API" (those are internal).
  We integrate the same *categories* of service:
  - **Maps SDK** — render the map in the driver app.
  - **Geocoding / Reverse geocoding** — store & customer address ↔ lat/long.
  - **Directions / Routing** — turn-by-turn navigation to each store then the customer.
  - **Distance Matrix / ETA** — arrival estimates and driver-to-order matching.
  - **Route optimization** — order the multi-store pickup stops efficiently.
  - **Live location** — stream the driver's GPS to the backend (SignalR) for
    customer tracking; **geofencing** to auto-detect arrival at a store/customer.
  - **Push notifications** — new assignment / status via FCM (Android) & APNs (iOS).
  - **Recommended provider:** **Google Maps Platform** (what Swiggy/Instacart largely
    use). **Alternatives:** **Ola Maps** or **MapmyIndia/Mappls** (India, lower cost),
    **Mapbox**, or **HERE**. See [driver integrations guide](DRIVER_APP_INTEGRATIONS.md).
- **Third-party logistics (later)**: optionally dispatch to external fleets
  (e.g. Swiggy Genie / Dunzo-style delivery-as-a-service) instead of own drivers.

---

## 10. Release Plan / Phasing
| Phase | Theme | Key Deliverables |
|---|---|---|
| **Phase 0 — Foundation** | Tenancy & onboarding | Registration, approval workflow, RBAC, store provisioning, Super Admin console. |
| **Phase 1 — Core Operations** | Sell & stock | Catalog/inventory, POS billing, purchase + GRN, basic reports. |
| **Phase 2 — Online** | Reach customers | Customer order app (storefront, cart/checkout), online payments, order manager. |
| **Phase 3 — Delivery** | Fulfil at the door | Driver app, delivery dispatch/assignment, proof of delivery, COD reconciliation, live tracking. |
| **Phase 4 — Finance** | Books & compliance | Full accounting, GST reports, receivables/payables, exports. |
| **Phase 5 — Monetize** | Fees & ads | Platform-fee accrual & settlements/payouts, advertiser accounts (stores + brands), ad campaigns/placements in the customer app, advertiser billing. |
| **Phase 6 — Scale** | Depth & chains | Multi-outlet, offline POS, analytics, route batching, loyalty. |

---

## 11. Risks & Mitigations
| Risk | Impact | Mitigation |
|---|---|---|
| Slow/abandoned onboarding | Low activation | Guided wizard, sample data, fast approvals. |
| Connectivity issues at counter | Billing blocked | Offline POS with sync. |
| Financial posting errors | Wrong books, trust loss | Double-entry, idempotency, reconciliation, audits. |
| Tenant data leakage | Severe/compliance | Strict tenant isolation, tests, security reviews. |
| Overselling online vs offline | Customer dissatisfaction | Single shared real-time stock ledger. |
| One store in a multi-store order can't fulfil | Delayed/partial order | Per-store part status, substitution/partial-refund flow, driver flags missing part at pickup. |
| Multi-store pickup adds delivery time/cost | Slow delivery, thin margins | Serviceability by area, suggested pickup route, later batching/route optimization. |
| Splitting/attribution errors across stores | Wrong store books & fees | `StoreOrder` is the posting unit; per-part accounting and per-part fee accrual with tests. |
| Driver COD cash leakage | Financial loss | Per-driver COD tracking, mandatory reconciliation, POD required to close orders. |
| Failed / disputed deliveries | Refund cost, distrust | Proof of delivery (OTP/photo), failed-reason capture, re-attempt/return flow. |
| Scope creep | Delayed launch | Strict P0/P1/P2 prioritization and phasing. |

---

## 12. Open Questions
1. Target geography/tax regime beyond India for v1?
2. **Monetization decided:** platform fee (commission and/or flat per-order) + advertising revenue (stores + brands). *Remaining*: default commission %, and is the fee on online orders only or also in-store POS?
3. Is multi-outlet needed in v1 or Phase 5?
4. Own storefront domain vs platform subdomain per store?
5. Delivery handled by store's own drivers in v1 (own driver app; no third-party fleet)?
6. Depth of accounting required at launch (basic vs full double-entry)?
7. Advertising billing: prepaid wallet vs post-paid invoicing for brands at launch?

---

## 13. Glossary
- **Kirana**: A neighbourhood grocery store.
- **Marketplace**: The unified customer app where all stores' inventory is browsable and shoppable together.
- **Parent Order**: The single customer order (one order number) that may span multiple stores.
- **Store Order / Part**: The portion of a parent order belonging to one store — the unit that store fulfils and books.
- **Pickup Task**: A driver's collection stop at one store for a given parent order.
- **Multi-Store Pickup**: A driver collecting items from several stores for one order, then making a single delivery.
- **Tenant**: An isolated store account on the platform.
- **POS**: Point of Sale (in-store billing).
- **GRN**: Goods Receipt Note.
- **GST/GSTIN**: Goods & Services Tax / taxpayer identification number (India).
- **P&L**: Profit & Loss statement.
- **CoA**: Chart of Accounts.
- **RBAC**: Role-Based Access Control.
- **PWA**: Progressive Web App.
- **COD**: Cash on Delivery.
- **POD**: Proof of Delivery (OTP, photo, or signature confirming handover).
- **Platform Fee**: Commission and/or flat per-order fee the platform charges a store on enabled business.
- **Settlement / Payout**: Netting of platform fees against collections and paying the store its balance.
- **Advertiser**: A store or brand that pays for promoted placements.
- **Brand**: A platform-level advertiser (e.g. an FMCG company) promoting products across many stores.
- **CPC / CPM**: Cost-per-click / cost-per-thousand-impressions ad pricing models.

---

*End of document — v1.0 Draft. Feedback and answers to §12 will drive v1.1.*
