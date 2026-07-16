# Kirana — Node + Expo + MySQL edition (`next` branch)

A rebuild of the Kirana marketplace on a modern JS stack:

- **backend/** — Node.js + Express + **Sequelize (MySQL)** REST API
- **mobile/** — **Expo (React Native)** consumer app

The API mirrors the legacy endpoints so behaviour is identical (auth, marketplace,
orders, approvals, shared images).

---

## 1. Backend (Node + MySQL)

### Prerequisites
- Node.js 18+
- MySQL running locally; create an empty database:
  ```sql
  CREATE DATABASE kirana CHARACTER SET utf8mb4;
  ```

### Setup
```bash
cd next-app/backend
cp .env.example .env      # set DB_USER / DB_PASSWORD / DB_NAME
npm install
npm start                 # creates tables (sequelize.sync) + seeds demo data
```
API runs at `http://localhost:4000`. Tables auto-create; the seed adds a Super
Admin, a demo store, approved products and master images.

Standalone reseed: `npm run seed`.

### Demo logins
- Super Admin — `superadmin@kirana.local` / `Admin@12345`
- Store owner — `demo@store.local` / `Demo@12345`
- Consumer — sign in with **OTP** (dev mode returns the code in the response)

### Key endpoints
| Area | Route |
|---|---|
| Auth | `POST /api/auth/login` · `/register-customer` · `/google` · `/request-otp` · `/verify-otp` · `GET /me` |
| Marketplace | `GET /api/marketplace/products?q=&category=&sort=` · `/categories` |
| Orders | `POST /api/orders` · `GET /mine` · `GET /:id` |
| Store | `GET /api/products` · `POST /api/products` · `GET /api/store/orders` (masked) |
| Admin | `GET /api/stores/pending` · `/api/products/pending-approval` · approve/reject |
| Catalog | `GET /api/catalog/image?name=` · `GET /images` · `PUT /image` |
| Uploads | `POST /api/uploads/image` (multipart `file`) |

Auth = `X-Auth-Token` header (returned by login). JSON only.

---

## 2. Mobile (Expo)

```bash
cd next-app/mobile
npm install
npm start                 # open in Expo Go, or press a/i/w
```

Set the API URL in `src/api.js` if needed (Android emulator uses `10.0.2.2`).

Screens: **Home/Shop** (search + categories + add to cart), **Cart**,
**Checkout** (place order), **Orders**, **Login** (OTP + email).

---

## 3. Status

**Done:** full API (auth incl. Google/OTP, marketplace search/filter/sort,
cart→order with geo ETA, store + product approval, masked store orders,
shared master images, file uploads) + Expo consumer app (shop→cart→checkout→orders,
OTP/email login).

**Next:** store & admin screens in the app (or a small web admin), real Google
sign-in button in Expo, payments, push notifications. See the root
`legacy-vs2015/ROADMAP.md` for the full product roadmap.
