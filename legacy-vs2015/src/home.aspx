<%@ Page Language="C#" %>
<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="utf-8" /><meta name="viewport" content="width=device-width, initial-scale=1" />
  <title>KiranaManagement — Multi-store grocery platform</title>
  <link rel="stylesheet" href="assets/app.css" />
</head>
<body>
  <div class="topnav"><div class="container">
    <span class="brand">🛒 Kirana</span>
    <a class="navlink" href="shop.aspx">Shop</a>
    <a class="navlink right" href="cart.aspx">Cart</a>
    <a class="navlink" href="register.aspx">Register store</a>
    <a class="navlink" href="login.aspx">Login</a>
  </div></div>

  <div class="hero"><div class="container">
    <h1 style="font-size:34px;max-width:640px">One platform for every neighbourhood Kirana store</h1>
    <p style="max-width:620px;font-size:16px">Shop groceries from all your local stores in a single cart, or run your own store — catalog, inventory, billing, purchases and orders, all in one place.</p>
    <div style="margin-top:18px">
      <a href="shop.aspx" class="btn btn-accent">🛍️ Start shopping</a>
      <a href="register.aspx" class="btn btn-outline" style="margin-left:8px">Register your store</a>
    </div>
  </div></div>

  <div class="container" style="margin-top:26px">
    <div class="grid" style="grid-template-columns:repeat(auto-fill,minmax(240px,1fr))">
      <div class="card"><div class="card-b"><div style="font-size:30px">🛒</div><h3>Shop the marketplace</h3><p class="muted">Browse products from all approved stores and check out in one go.</p><a href="shop.aspx">Browse products →</a></div></div>
      <div class="card"><div class="card-b"><div style="font-size:30px">🏪</div><h3>Run your store</h3><p class="muted">Products with variants (MRP + discount), inventory, POS billing and purchases.</p><a href="login.aspx">Store login →</a></div></div>
      <div class="card"><div class="card-b"><div style="font-size:30px">🛡️</div><h3>Platform admin</h3><p class="muted">Approve new stores and keep the marketplace healthy.</p><a href="login.aspx">Admin login →</a></div></div>
    </div>

    <div class="card" style="margin-top:22px"><div class="card-b">
      <h3>How it works</h3>
      <div class="grid" style="grid-template-columns:repeat(auto-fill,minmax(200px,1fr));margin-top:6px">
        <div><strong>1. Register</strong><p class="muted">A store signs up and waits for admin approval.</p></div>
        <div><strong>2. Approve</strong><p class="muted">Super Admin reviews and activates the store.</p></div>
        <div><strong>3. Sell</strong><p class="muted">The store adds products; customers shop the marketplace.</p></div>
        <div><strong>4. Deliver</strong><p class="muted">Orders are placed, packed and delivered.</p></div>
      </div>
    </div></div>
  </div>

  <div class="foot"><div class="container">© 2026 KiranaManagement — Multi-store grocery marketplace · Built with ASP.NET Web API 2 + EF6</div></div>
  <script src="assets/app.js"></script>
</body>
</html>
