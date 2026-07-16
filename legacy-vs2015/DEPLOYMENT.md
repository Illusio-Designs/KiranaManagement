# Kirana — Sub-domain deployment (Admin / Store / Consumer)

One code-base is served on three sub-domains. The portal is resolved from the
host name at runtime by `Data/PortalResolver.cs`, so **the same build** powers
all three — no separate projects.

| Sub-domain (prod)   | Local dev host           | Portal        | Sign-in methods                     | Lands on     |
|---------------------|--------------------------|---------------|-------------------------------------|--------------|
| `admin.kirana.com`  | `admin.kirana.local`     | Super Admin   | Email + password                    | `admin.aspx` |
| `store.kirana.com`  | `store.kirana.local`     | Store Owner   | Email + password, **Google**        | `store.aspx` |
| `kirana.com` / `www`| `kirana.local`           | Consumer      | **OTP**, email + password, register | `home.aspx`  |

`login.aspx` is a single **adaptive** page: it changes its title, the sign-in
methods it shows, and the role it admits based on the sub-domain. Each portal
only lets its own kind of user in (a customer trying `admin.` is refused with a
message pointing them to the right site).

---

## 1. web.config additions

Add inside `<configuration>`:

```xml
<appSettings>
  <!-- Enables "Continue with Google" on the store portal. Get this from the
       Google Cloud console (OAuth 2.0 Client ID, type "Web application"). -->
  <add key="GoogleClientId" value="YOUR_GOOGLE_CLIENT_ID.apps.googleusercontent.com" />
  <!-- true  = OTP code is returned in the API response for local testing
       false = production (wire a real SMS sender) -->
  <add key="OtpDevMode" value="true" />
</appSettings>
```

Register the root-routing module inside `<system.webServer>`:

```xml
<system.webServer>
  <modules>
    <add name="PortalRoutingModule" type="Kirana.WebApi.Data.PortalRoutingModule" />
  </modules>
</system.webServer>
```

(With the module in place you don't need a `<defaultDocument>` — `/` is routed
to each portal's landing page automatically.)

---

## 2. Local testing (VS 2015 / IIS Express)

### a) hosts file
Edit `C:\Windows\System32\drivers\etc\hosts` (as Administrator), add:

```
127.0.0.1  kirana.local
127.0.0.1  www.kirana.local
127.0.0.1  admin.kirana.local
127.0.0.1  store.kirana.local
```

### b) IIS Express bindings
In the project folder, open `.vs/config/applicationhost.config` (or
`%USERPROFILE%\Documents\IISExpress\config\applicationhost.config`), find your
site's `<bindings>` and add (keep the port your project uses, e.g. 44300):

```xml
<bindings>
  <binding protocol="http" bindingInformation="*:PORT:kirana.local" />
  <binding protocol="http" bindingInformation="*:PORT:www.kirana.local" />
  <binding protocol="http" bindingInformation="*:PORT:admin.kirana.local" />
  <binding protocol="http" bindingInformation="*:PORT:store.kirana.local" />
</bindings>
```

Then browse `http://admin.kirana.local:PORT/`, `http://store.kirana.local:PORT/`,
`http://kirana.local:PORT/`.

### c) No-hosts shortcut
Without editing hosts you can still test each portal by adding
`?portal=admin` / `?portal=store` / `?portal=consumer` to any URL — the resolver
honours that override on localhost.

> **Google note:** Google Identity Services only runs on `https` or
> `localhost`/`*.local`. For local Google testing use the `.local` hosts above
> and add each origin to the "Authorized JavaScript origins" of your OAuth
> client in the Google console.

---

## 3. Production (IIS)

Two equivalent options:

**A. One site, three host-header bindings** (simplest — one app pool, shared
Session): add three bindings to the site — `admin.kirana.com`,
`store.kirana.com`, `www.kirana.com` (+ `kirana.com`) — all pointing at the same
physical folder. Add the DNS `A`/`CNAME` records for each sub-domain.

**B. Three sites** pointing at the same published folder (separate app pools) if
you want to scale/isolate them independently. Session is per-app-pool, which is
fine because a user only ever uses one portal at a time.

TLS: issue a certificate covering `kirana.com`, `www`, `admin`, `store` (a SAN
or wildcard `*.kirana.com` cert) and bind it to each https binding.

---

## 4. How the pieces fit

- `Data/PortalResolver.cs` — host → `Portal` (Admin / Store / Consumer).
- `Data/PortalRoutingModule.cs` — routes `/` to the portal's landing page.
- `login.aspx` / `login.aspx.cs` — adaptive login; admits only the portal's role.
- `/api/auth/me` — server-side re-validation of a Google/OTP token before the
  ASP.NET session is established.
- `/api/auth/google`, `/api/auth/request-otp`, `/api/auth/verify-otp` — the
  external sign-in endpoints used by the store and consumer portals.
