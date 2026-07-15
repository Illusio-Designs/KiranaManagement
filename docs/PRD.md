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
1. **Customer Online-Order App** — customers browse a store's catalog, order, pay, and track delivery.
2. **Driver App** — delivery drivers receive assigned orders, navigate, and update delivery status with proof of delivery.
3. **Store Management Dashboard** — store owners/staff run operations (catalog, inventory, purchase, POS, order fulfilment, delivery dispatch, accounting, reports).

Each store operates as an isolated tenant with its own data, staff, catalog, and books, while a central **Super Admin** governs onboarding, approvals, billing, and platform-wide health.

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

---

## 2. Scope

### 2.1 In Scope (v1)
- Store self-registration and admin approval workflow.
- Multi-tenant store management with role-based access.
- Product catalog & inventory management (batches, expiry, units).
- Purchase management (suppliers, purchase orders, goods receipt, supplier bills).
- In-store Sales / POS (billing, discounts, multiple payment modes, returns).
- **Customer Online-Order App** — storefront browse, cart, checkout, pay, order tracking (delivery/pickup).
- Order management (unified queue for online + phone orders, fulfilment states).
- **Driver App & delivery dispatch** — assign orders to drivers, live status, proof of delivery.
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
| **Online Customer** | Buys from a store's storefront/app. | Browse, order, pay, track delivery. |
| **Delivery Driver** | Delivers orders for a store. | See assigned orders, navigate, update status, capture proof of delivery, track earnings. |

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

### 4.4 Online Order (Customer App)
1. Customer browses the store's storefront/app, adds items to cart, checks out (delivery or pickup slot).
2. Pays online or chooses cash-on-delivery.
3. Order lands in the store's **Order Manager** as **New**.
4. Staff **accepts → picks/packs → ready**.
5. For delivery orders, staff **assign a driver** (see §4.5); for pickup, customer is notified it's ready.
6. Sale posts to inventory & accounts on fulfilment; customer gets live status updates and can track the driver.

### 4.5 Delivery (Driver App)
1. Store staff assign a **Ready** order to an available driver from the dashboard.
2. Driver receives a **push notification** and sees the order in their app (items, address, amount to collect if COD).
3. Driver **accepts → picks up from store → out for delivery** (navigates via map).
4. Customer sees live status (and optional driver location).
5. Driver **marks delivered** with **proof of delivery** (OTP, photo, or signature); for COD, records cash collected.
6. Order closes; delivery + any COD collection post to accounting; driver's completed-delivery count/earnings update.

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

### 5.6 Online Sales (Storefront)
- **FR-6.1 (P0)** Auto-provisioned storefront per approved store (unique URL/subdomain), sharing the same catalog & stock.
- **FR-6.2 (P0)** Customer browse, search, cart, and checkout (delivery or pickup).
- **FR-6.3 (P0)** Online payments (UPI/card/netbanking via gateway) + Cash on Delivery.
- **FR-6.4 (P0)** Real-time stock availability reflected online (no overselling).
- **FR-6.5 (P1)** Delivery zones, delivery charges, minimum order value, and slot selection.
- **FR-6.6 (P1)** Promotions/coupons applicable online.
- **FR-6.7 (P2)** Customer accounts with order history and reorder.
- **FR-6.8 (P2)** Basic storefront theming/branding per store.

### 5.7 Order Management
- **FR-7.1 (P0)** Unified order queue for online + manual (phone/WhatsApp) orders.
- **FR-7.2 (P0)** Order lifecycle: `New → Accepted → Packed → Ready → Out-for-delivery/Pickup → Delivered/Completed → Cancelled/Returned`.
- **FR-7.3 (P0)** Fulfilment posts to inventory and accounting.
- **FR-7.4 (P1)** Customer notifications on each status change.
- **FR-7.5 (P1)** Partial fulfilment / item substitution & refunds.
- **FR-7.6 (P0)** Assign/re-assign a **Ready** delivery order to a driver from the dashboard (see §5.8).

### 5.8 Driver App & Delivery Management
- **FR-8.1 (P0)** Driver onboarding: store adds a driver (name, phone, vehicle); driver logs into the Driver App.
- **FR-8.2 (P0)** Driver availability toggle (online/offline) and view of orders assigned to them.
- **FR-8.3 (P0)** Assigned-order detail: items, customer name & address, contact, COD amount to collect.
- **FR-8.4 (P0)** Delivery status flow in-app: `Assigned → Accepted → Picked-up → Out-for-delivery → Delivered` (+ `Failed/Returned`).
- **FR-8.5 (P0)** **Proof of delivery**: delivery OTP, photo, and/or signature capture.
- **FR-8.6 (P0)** COD handling: record cash collected; reconcile driver cash against the store.
- **FR-8.7 (P0)** Push notifications to the driver on new assignment / changes.
- **FR-8.8 (P1)** Map & navigation to the customer address; optional live driver location shared with customer.
- **FR-8.9 (P1)** Driver dashboard: today's deliveries, completed count, and earnings/collections summary.
- **FR-8.10 (P1)** Failed-delivery reasons and re-attempt / return-to-store handling.
- **FR-8.11 (P2)** Multi-order batch pickup and simple sequencing.
- **FR-8.12 (P2)** Driver ratings/feedback from customers.

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
- **FR-12.3 (P1)** Subscription plans & billing management for stores.
- **FR-12.4 (P1)** Platform health, usage metrics, and audit logs.
- **FR-12.5 (P2)** Announcements/broadcasts to stores.

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
- **Platform**: `SuperAdmin`, `SubscriptionPlan`, `Subscription`, `AuditLog`.
- **Store/Tenant**: `Store`, `StoreSettings`, `User`, `Role`, `Permission`.
- **Catalog/Inventory**: `Product`, `Category`, `UnitOfMeasure`, `Batch`, `StockLedger`, `StockAdjustment`.
- **Purchasing**: `Supplier`, `PurchaseOrder`, `GoodsReceipt`, `PurchaseInvoice`, `PurchaseReturn`.
- **Sales**: `SalesInvoice`, `SalesLine`, `Payment`, `SalesReturn`, `Customer`.
- **Online/Orders**: `Storefront`, `Cart`, `Order`, `OrderLine`, `OrderStatusHistory`, `DeliveryZone`.
- **Delivery/Driver**: `Driver`, `DriverAvailability`, `DeliveryAssignment`, `DeliveryStatusHistory`, `ProofOfDelivery`, `CODCollection`.
- **Accounting**: `Account` (CoA), `JournalEntry`, `LedgerPosting`, `TaxRate`, `Receivable`, `Payable`.
- **Notifications**: `NotificationTemplate`, `NotificationLog`.

> Every tenant-scoped entity carries a `store_id`; all queries are tenant-filtered.

---

## 8. System Architecture (Conceptual)
- **Clients (four surfaces over one backend)**:
  - **Store Management Dashboard** — web/mobile app for POS + operations + delivery dispatch.
  - **Customer Online-Order App** — customer PWA/mobile app (storefront, checkout, tracking).
  - **Driver App** — mobile app for delivery drivers (assignments, navigation, proof of delivery).
  - **Super Admin Console** — platform governance.
- **Backend**: Multi-tenant API services grouped by domain (Auth/Tenancy, Catalog/Inventory, Purchasing, Sales/POS, Online/Orders, **Delivery/Dispatch**, Accounting, Notifications).
- **Data**: **MySQL** relational database with tenant isolation (row-level `store_id`, enforced globally), object storage for documents/images.
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
- **Maps/Geo (P2)**: Delivery zone & address validation.

---

## 10. Release Plan / Phasing
| Phase | Theme | Key Deliverables |
|---|---|---|
| **Phase 0 — Foundation** | Tenancy & onboarding | Registration, approval workflow, RBAC, store provisioning, Super Admin console. |
| **Phase 1 — Core Operations** | Sell & stock | Catalog/inventory, POS billing, purchase + GRN, basic reports. |
| **Phase 2 — Online** | Reach customers | Customer order app (storefront, cart/checkout), online payments, order manager. |
| **Phase 3 — Delivery** | Fulfil at the door | Driver app, delivery dispatch/assignment, proof of delivery, COD reconciliation, live tracking. |
| **Phase 4 — Finance** | Books & compliance | Full accounting, GST reports, receivables/payables, exports. |
| **Phase 5 — Scale** | Depth & chains | Multi-outlet, offline POS, analytics, route batching, loyalty. |

---

## 11. Risks & Mitigations
| Risk | Impact | Mitigation |
|---|---|---|
| Slow/abandoned onboarding | Low activation | Guided wizard, sample data, fast approvals. |
| Connectivity issues at counter | Billing blocked | Offline POS with sync. |
| Financial posting errors | Wrong books, trust loss | Double-entry, idempotency, reconciliation, audits. |
| Tenant data leakage | Severe/compliance | Strict tenant isolation, tests, security reviews. |
| Overselling online vs offline | Customer dissatisfaction | Single shared real-time stock ledger. |
| Driver COD cash leakage | Financial loss | Per-driver COD tracking, mandatory reconciliation, POD required to close orders. |
| Failed / disputed deliveries | Refund cost, distrust | Proof of delivery (OTP/photo), failed-reason capture, re-attempt/return flow. |
| Scope creep | Delayed launch | Strict P0/P1/P2 prioritization and phasing. |

---

## 12. Open Questions
1. Target geography/tax regime beyond India for v1?
2. Pricing model for stores (per-store flat, tiered, transaction fee)?
3. Is multi-outlet needed in v1 or Phase 4?
4. Own storefront domain vs platform subdomain per store?
5. Delivery handled by stores themselves in v1 (no fleet)?
6. Depth of accounting required at launch (basic vs full double-entry)?

---

## 13. Glossary
- **Kirana**: A neighbourhood grocery store.
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

---

*End of document — v1.0 Draft. Feedback and answers to §12 will drive v1.1.*
