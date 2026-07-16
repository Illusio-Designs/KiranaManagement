# Kirana — System Roadmap

A multi-store grocery **marketplace** (like Zepto / Blinkit) where many neighbourhood
stores sell on one platform, consumers shop across stores in a single cart, and a
platform admin governs the catalog. Built for the course on **Visual Studio 2015 —
ASP.NET Web API 2 + Web Forms, Entity Framework 6, SQL Server LocalDB**.

Authors: **Mansi, Hiral & Zigma**

---

## 1. Who uses it (actors)

| Actor | Portal (sub-domain) | Signs in with | Can do |
|---|---|---|---|
| **Consumer** | `kirana.com` | OTP + email/password | Browse, search, cart, checkout, track orders |
| **Store Owner** | `store.kirana.com` | Email/password + **Google** | Products, inventory, POS, purchases, marketplace orders |
| **Super Admin** | `admin.kirana.com` | Email/password | Approve stores & products, oversee platform |
| **Delivery (3PL)** | API/webhook | Provider integration | Fulfil deliveries (Swiggy/Instacart-style seam) |

One code-base serves all three portals; the **sub-domain decides the portal** at
runtime (`PortalResolver` + `PortalRoutingModule`).

---

## 2. Architecture

```
Browser (Web Forms .aspx + shared app.css/app.js)
   │  server controls + code-behind  │  fetch() JSON
   ▼                                 ▼
ASP.NET Web Forms pages  ──calls──►  ASP.NET Web API 2  ──►  EF6  ──►  SQL Server LocalDB
(Site.master, Dashboard.master)      (/api/* controllers)         (KiranaDbContext + Seed)
```

- **Auth:** simple session token (`X-Auth-Token`) + ASP.NET `Session`.
- **Design system:** one `app.css` (green storefront + green dashboards), round SVG icons.
- **JSON only** (XML formatter removed), camelCase.

---

## 3. Feature modules

### Storefront (consumer)
- Home (hero, categories, best deals), Shop (search + category filter + sort),
  Cart, Checkout, Thank-you, My Orders.
- **Geo checkout:** capture lat/lng → distance-based delivery ETA & fee.
- No guest orders — customer login required.

### Store dashboard
- Products (with **category** + **image**), Inventory (stock + low-stock),
  POS billing, Purchases (suppliers, PO, receive).
- **Marketplace orders** — items + delivery area + ETA, **consumer identity hidden**.
- Collapsible green sidebar.

### Admin dashboard
- **Store approvals** and **Product approvals**.
- Overview widgets: stat tiles, bar/donut/line charts, activity feed, data tables.

### Platform governance
- **Store approval:** a store is `Pending` → admin approves → `Active` (can sign in & sell).
- **Product approval:** a product is `Pending` → admin approves → live on marketplace.

### Monetization
- Platform fee on orders + advertising (no subscription).

---

## 4. Catalog & product-image flow (master record + reuse)

Generic items (broccoli, milk) are sold by many stores but should share **one image**.

```
Store adds a product (name + optional image)
        │
        ▼
   Is there a master image for this NAME?
     ├── YES → reuse & display it (store can leave image blank)
     └── NO  → store uploads/provides an image
                  → it is saved as the MASTER (named after the product)
                  → every future store with the same name reuses it
```

- Master store: `CatalogImage { NameKey, Name, ImageName, ImageUrl }` keyed by
  normalized product name.
- Resolution on display: **product's own image → master image (by name) → category icon**.
- Form UX: typing a name checks the catalog and shows *"Catalog image found (reused)"*
  or *"No catalog image yet — add one"*.

---

## 5. Data model (core)

`Store` · `User` (roles incl. Customer; Google/OTP providers) · `Product`
(Category, ImageUrl, **Status** Pending/Approved/Rejected) · `ProductVariant`
(MRP/price/stock) · `CatalogImage` (shared images) · `SalesInvoice/Line` (POS) ·
`Supplier/PurchaseOrder/Line` · `Order/OrderLine` (geo + status) · `OtpCode`.

---

## 6. Roadmap

### ✅ Done
- [x] Onboarding & auth (store register → admin approval; customer signup)
- [x] Sub-domain portals (admin / store / consumer) + adaptive login
- [x] Store-owner **Google** sign-in, consumer **OTP** login
- [x] Catalog with variants (MRP + discount), inventory, low-stock
- [x] POS billing, suppliers & purchase orders
- [x] Marketplace: search, category filter, sort
- [x] Cart → geo checkout → order, delivery ETA/fee, thank-you, order history
- [x] Store marketplace-order view with **consumer data masked**
- [x] **Store approval** + **Product approval** workflows
- [x] **Shared master product images** (auto-create by name + reuse)
- [x] Premium green UI, collapsible sidebar, full component library

### 🔜 Next (recommended)
- [ ] **Real file upload** for images (save to `/uploads`) instead of URL
- [ ] Admin **master-catalog manager** (curate/replace shared images & items)
- [ ] Rejection reason prompt (admin types a reason; shown to the store)
- [ ] Product **edit/deactivate**; variant edit
- [ ] Coupons / promotions & the advertising surface
- [ ] Ratings & reviews

### 🌟 Future
- [ ] Barcode → auto image/details (Open Food Facts)
- [ ] Live delivery tracking (map) via the 3PL webhook
- [ ] Payments gateway (UPI/cards) integration
- [ ] Analytics dashboard (sales trends, top products, payout reports)
- [ ] Notifications (SMS/email/push) for orders & approvals

---

## 7. Run it locally (summary)

1. Open in **Visual Studio 2015**; **Include In Project** any new files (Show All Files).
2. `web.config`: connection string + `<globalization utf-8>` + `<defaultDocument>home.aspx`
   + `PortalRoutingModule` + `GoogleClientId`/`OtpDevMode` (see `DEPLOYMENT.md`).
3. First run recreates the DB (`DropCreateDatabaseIfModelChanges`) and seeds a
   Super Admin + demo store + approved products + master images.
4. Browse `home.aspx`. Demo logins: `superadmin@kirana.local / Admin@12345`,
   `demo@store.local / Demo@12345`.

See also: `DEPLOYMENT.md` (sub-domains/IIS) and `WIDGETS.md` (UI checklist).
