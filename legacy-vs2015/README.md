# KiranaManagement — Visual Studio 2015 version

> This folder is a **VS 2015-compatible** version of the backend, for the
> course/assignment that requires Visual Studio 2015. It uses the stack VS 2015
> supports natively:
>
> - **.NET Framework 4.6.2** (C# 6)
> - **ASP.NET Web API 2** (REST API)
> - **Entity Framework 6** (Code First)
> - **SQL Server LocalDB** (ships with VS 2015) — or MySQL, see below
>
> The modern `../backend/` project (.NET 8 / EF Core) is unchanged and is **not**
> for VS 2015. Use one or the other.

The `.csproj` and `web.config` that a Web API project needs contain lots of
NuGet-generated assembly binding-redirects that are painful to hand-write. So
instead of shipping a half-broken project, this folder ships the **C# source
files** and a recipe to create the project **inside VS 2015** (which generates
the correct config for you), then add these files.

---

## Step 1 — Create the project in VS 2015

1. Open **Visual Studio 2015**.
2. **File → New → Project… → Visual C# → Web → ASP.NET Web Application**.
   - Name it **`Kirana.WebApi`**.
   - **Framework** dropdown (top of the dialog): **.NET Framework 4.6.2**.
     (If 4.6.2 isn't listed, install the "4.6.2 Developer Pack", or pick 4.6.1.)
3. In the template list choose **Empty**, and **tick "Web API"** under
   "Add folders and core references for". Click **OK**.

VS creates a Web API 2 project with correct references and `web.config`.

## Step 2 — Install the NuGet packages

**Tools → NuGet Package Manager → Package Manager Console**, then run:

```powershell
Install-Package EntityFramework -Version 6.4.4
```

(Web API packages come with the template. `EntityFramework` is the only extra
one for the LocalDB setup.)

## Step 3 — Add the source files

Copy the files from **`legacy-vs2015/src/`** into the `Kirana.WebApi` project,
keeping the folders:

```
Kirana.WebApi/
 ├─ Models/        (Enums, Store, User, Product, ProductVariant, Sales, Purchasing)
 ├─ Dtos/          (Dtos.cs)
 ├─ Data/          (KiranaDbContext.cs, PasswordHasher.cs, AuthUtil.cs)
 ├─ Controllers/   (StoreApiController, AuthController, StoresController,
 │                  ProductsController, SalesController, PurchasesController)
 └─ *.html         (login.html, register.html, admin.html, store.html, index.html
                    — put these at the project root)
```

Store-owner API (all require the `X-Auth-Token` header):
- **Catalog:** `POST/GET /api/products`
- **Inventory:** `POST /api/products/variants/{id}/adjust`, `GET /api/products/low-stock`
- **POS:** `POST/GET /api/sales`
- **Purchases:** `POST/GET /api/purchases/suppliers`, `POST/GET /api/purchases/orders`,
  `POST /api/purchases/orders/{id}/receive`

The **store dashboard** (`store.html`) has tabs for Products, Inventory, POS, and Purchases.

In VS: right-click the project → **Add → Existing Item…**, select the files
(or drag them from Explorer into the Solution Explorer). Make sure the
namespace at the top of each file matches your project's default namespace —
they use **`Kirana.WebApi`**; if you named the project differently, do a
find-and-replace on that namespace.

## Step 4 — Connection string + database init

Open **`web.config`** and add a connection string inside `<configuration>`
(LocalDB comes with VS 2015):

```xml
<connectionStrings>
  <add name="KiranaDb"
       connectionString="Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=KiranaDb;Integrated Security=True;MultipleActiveResultSets=True"
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

Open **`Global.asax.cs`** and add these lines at the **top** of
`Application_Start()` so the database is created and seeded on first run:

```csharp
// Dev: recreate the schema whenever the model changes (wipes data — fine for a course).
System.Data.Entity.Database.SetInitializer(
    new System.Data.Entity.DropCreateDatabaseIfModelChanges<Kirana.WebApi.Data.KiranaDbContext>());
Kirana.WebApi.Data.KiranaDbContext.Seed();
```

> Use `DropCreateDatabaseIfModelChanges` during development so that when you add
> columns (e.g. the login `SessionToken`) the database rebuilds automatically.
> For a stable database that never drops data, switch to
> `CreateDatabaseIfNotExists` once the model is final.

## Step 5 — The web frontend

The frontend is a set of **self-contained HTML pages** (Bootstrap + plain
JavaScript `fetch`, CDN — no build step, no npm). They call the API on the
**same origin**, so there's nothing to configure. Put all of them at the
**project root** (same level as `web.config`):

| Page | Who | Purpose |
|---|---|---|
| `login.html` | everyone | Sign in. Redirects by role: Super Admin → `admin.html`, store owner → `store.html`. **Set this as the Start Page.** |
| `register.html` | public | Store self-registration (status → Pending). |
| `admin.html` | Super Admin | Approve / reject pending stores (login-protected). |
| `store.html` | store owner | Add products with variants (MRP / selling price / discount) and list them (login-protected). |
| `index.html` | — | Just redirects to `login.html`. |

In Solution Explorer, right-click **`login.html` → Set As Start Page**.

**Auth:** login returns a token; the pages store it and send it in the
`X-Auth-Token` header. Super-admin and store-owner endpoints require it.

> Uses public CDNs for Bootstrap — so an internet connection is needed the first
> time. To go fully offline, `Install-Package bootstrap` and point the two CDN
> `<link>`/`<script>` tags at the local `Content/`/`Scripts/` files.

## Step 6 — Run

Press **F5**. The browser opens `login.html`. Behind it the API serves:

- `POST /api/auth/login` — sign in (returns a token)
- `POST /api/stores/register` — register a store (public, status Pending)
- `GET  /api/stores/pending` · `POST /api/stores/{id}/approve` · `.../reject` — **SuperAdmin only**
- `POST /api/products` · `GET /api/products` — **store owner only** (uses their own store)

You can also test the API with **Postman**/**curl** (send `X-Auth-Token` for the
protected endpoints).

**Full flow to demo:**
1. `register.html` — register a store → it's **Pending**.
2. `login.html` — sign in as **Super Admin** (`superadmin@kirana.local` /
   `Admin@12345`) → `admin.html` → **Approve** the store.
3. `login.html` — sign in as the **store owner** (the email/password used at
   registration) → `store.html` → add products with variants.

---

## Using MySQL instead of LocalDB (optional)

If your course specifically needs MySQL:

1. `Install-Package MySql.Data` and `Install-Package MySql.Data.Entity` (EF6).
2. In `web.config`, set the connection string provider to MySQL:
   ```xml
   <add name="KiranaDb"
        connectionString="server=localhost;port=3306;database=kirana;uid=root;pwd=root"
        providerName="MySql.Data.MySqlClient" />
   ```
3. Add the EF provider config `web.config` entries that the MySQL package
   documents, and annotate `KiranaDbContext` with
   `[DbConfigurationType(typeof(MySql.Data.Entity.MySqlEFConfiguration))]`.

LocalDB is simpler and recommended for the assignment unless MySQL is mandatory.

---

## What's included vs. the modern backend

This VS 2015 version implements the **core**: store registration + approval,
users, and catalog with **product variants (MRP + selling price → discount)**.
The full feature set (OTP/Google auth, marketplace cart, checkout/orders, POS,
purchasing, 3PL delivery) lives in the modern `../backend/` project and can be
ported here module-by-module on the same Web API 2 + EF6 pattern. Ask and I'll
port the next module you need.
