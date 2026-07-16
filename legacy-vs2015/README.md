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
 ├─ Models/        (Enums.cs, Store.cs, User.cs, Product.cs, ProductVariant.cs)
 ├─ Dtos/          (Dtos.cs)
 ├─ Data/          (KiranaDbContext.cs, PasswordHasher.cs)
 ├─ Controllers/   (StoresController.cs, ProductsController.cs)
 └─ index.html     (the web frontend — put this at the project root)
```

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
System.Data.Entity.Database.SetInitializer(
    new System.Data.Entity.CreateDatabaseIfNotExists<Kirana.WebApi.Data.KiranaDbContext>());
Kirana.WebApi.Data.KiranaDbContext.Seed();
```

## Step 5 — The web frontend

`index.html` is a **self-contained web UI** (Bootstrap + plain JavaScript `fetch`,
loaded from a CDN — no build step, no npm). It calls the API on the **same
origin**, so there's nothing to configure.

1. Put **`index.html`** at the **project root** (same level as `web.config`).
2. In Solution Explorer, right-click **`index.html` → Set As Start Page**.

It gives you tabs to: **register a store**, **approve** pending stores, and
**add products with variants** (showing MRP, selling price, and the derived
discount) and list them.

> Uses public CDNs for Bootstrap — so an internet connection is needed the first
> time. To go fully offline, `Install-Package bootstrap` and point the two CDN
> `<link>`/`<script>` tags at the local `Content/`/`Scripts/` files.

## Step 6 — Run

Press **F5**. The browser opens `index.html` (the frontend). Behind it the API
serves:

- `POST /api/stores/register` — register a store (status Pending)
- `GET  /api/stores/pending` — list pending stores
- `POST /api/stores/{id}/approve` · `POST /api/stores/{id}/reject`
- `POST /api/products` — create a product with variants (MRP + selling price)
- `GET  /api/products?storeId={id}` — list a store's products

You can also test the API directly with **Postman** or **curl**. (No Swagger in
this template; optionally `Install-Package Swashbuckle` to add it.)

**First flow:** Register a store → copy the returned **Store Id** → Approvals tab
→ Approve → Products tab → paste the Store Id, add a product with variants.

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
