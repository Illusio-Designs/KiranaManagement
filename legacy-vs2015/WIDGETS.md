# Kirana — Widget & Component Checklist

Tick each item as you verify it. `[x]` = built & shown in screenshots, `[ ]` = not started.

## 1. Storefront (consumer — green)
- [x] Sticky top app bar (logo, nav links, search, account, cart pill)
- [x] Hero banner (eyebrow, headline, CTA buttons, trust badges, offer bubble)
- [x] "Shop by Category" round-icon cards
- [x] Product card (discount badge, image, name, weight, price + MRP, Add to Cart)
- [x] "Best Deals" product grid
- [x] Feature strip (Easy Returns / 24-7 Support / Member Benefits / Secure Payments)
- [x] Category filter tabs (with live counts)
- [x] Search box (name / brand / category)
- [x] Sort-by dropdown (Popular / Price / Discount / Name)
- [x] Fixed footer (4 columns + copyright)

## 2. Cart & Checkout
- [x] Cart line item (thumb, name, qty stepper, price)
- [x] Order summary panel (subtotal, delivery, GST, total)
- [x] Checkout stepper (Cart → Delivery & Payment → Confirmation)
- [x] Address form (name, phone, address, city, pincode)
- [x] "Use my current location" geo capture + ETA note
- [x] Payment method selector
- [x] Thank-you / order confirmation page

## 3. Dashboard shell (admin + store — green)
- [x] Collapsible sidebar (expand / collapse toggle)
- [x] Full-height sidebar (connected to page bottom)
- [x] Grouped nav (Manage / Business / Profile) with active highlight
- [x] Icon-only collapsed rail + hover tooltips
- [x] Top bar (title, search, notifications, avatar)
- [x] Sticky footer bar

## 4. Dashboard widgets
- [x] Stat tile (round icon, value, % change up/down)
- [x] Bar chart (revenue overview)
- [x] Donut / pie chart + legend (sales by category)
- [x] Filter bar widget (multi-select chips + date range + Apply)
- [x] Data table (sortable headers, status pills, row hover)
- [x] Table toolbar (status tabs, Show-by, Sort-by)
- [x] Pagination control
- [x] Low-stock alert list (store)
- [x] Masked marketplace orders list (store — no consumer PII)
- [ ] Line chart (trend) — *optional, not built yet*
- [ ] Sparkline in stat tiles — *optional, not built yet*
- [ ] Activity / audit feed — *optional, not built yet*

## 5. Form fields (component library)
- [x] Text input / password
- [x] Textarea
- [x] Single select (pill + standard)
- [x] Multi-select chips (removable + dropdown checklist)
- [x] Checkbox group
- [x] Radio group
- [x] Toggle switch
- [x] Date picker
- [x] Date-range picker
- [x] Search field
- [x] Combobox (searchable select)
- [x] File upload (drag & drop)

## 6. Buttons, badges, feedback
- [x] Buttons (primary, accent, outline, danger, icon, sizes)
- [x] Round line-icons (sizes + color variants)
- [x] Status badges (green / amber / red / gray / brand)
- [x] Alerts (success / error)
- [x] Toast notifications

## 7. Auth (subdomain portals)
- [x] Adaptive login page (one file, three portals)
- [x] admin.kirana.com — email + password
- [x] store.kirana.com — email + Continue with Google
- [x] kirana.com — email + OTP (send → verify) + register
- [x] Portal role guard (wrong portal is refused)
- [x] Store registration page
- [x] Customer sign-up page

## 8. Backend features
- [x] Marketplace search / category / sort API
- [x] Geo-based delivery ETA + distance fee
- [x] Store orders API with consumer data masked
- [x] Order status workflow (Placed → Delivered / Cancelled)
- [x] Google ID-token validation (store owner)
- [x] OTP request / verify (consumer)
- [x] `/api/auth/me` token re-validation
- [x] Subdomain portal resolver + root routing module
