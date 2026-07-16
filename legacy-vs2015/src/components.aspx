<%@ Page Language="C#" %>
<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="utf-8" /><meta name="viewport" content="width=device-width, initial-scale=1" />
  <title>Component Library · Kirana</title>
  <link rel="stylesheet" href="assets/app.css" />
</head>
<body>
  <header class="appbar"><div class="container">
    <a class="logo" href="home.aspx"><span class="lm"><svg class="ic" viewBox="0 0 24 24"><path d="M11 20A7 7 0 0 1 4 13C4 8 8 4 20 4c0 12-4 16-9 16z"/><path d="M9 17c1-4 4-7 8-9"/></svg></span>Kirana</a>
    <nav class="navlinks"><a href="home.aspx">Home</a><a href="shop.aspx">Categories</a></nav>
    <div class="acts right"><a class="cartpill" href="cart.aspx"><svg class="ic" viewBox="0 0 24 24"><circle cx="9" cy="21" r="1.6"/><circle cx="18" cy="21" r="1.6"/><path d="M2.5 3h2l2.2 12.4a2 2 0 0 0 2 1.6h8.3a2 2 0 0 0 2-1.6L22 8H6"/></svg> <span data-cart-count="">0</span></a></div>
  </div></header>

  <div class="container" style="padding-top:26px;padding-bottom:20px">
    <div class="section-h"><h2>Component Library</h2><span class="badge badge-brand">Design System v2</span></div>
    <p class="muted" style="margin-top:-8px">Reusable widgets used across the storefront and dashboards.</p>

    <div class="card card-b comp-sec" style="margin-top:16px"><h3>Round Icons</h3>
      <div class="swatch">
        <span class="iconcircle xl"><svg class="ic" viewBox="0 0 24 24"><circle cx="9" cy="21" r="1.6"/><circle cx="18" cy="21" r="1.6"/><path d="M2.5 3h2l2.2 12.4a2 2 0 0 0 2 1.6h8.3a2 2 0 0 0 2-1.6L22 8H6"/></svg></span>
        <span class="iconcircle lg mint"><svg class="ic" viewBox="0 0 24 24"><path d="M11 20A7 7 0 0 1 4 13C4 8 8 4 20 4c0 12-4 16-9 16z"/></svg></span>
        <span class="iconcircle lg sky"><svg class="ic" viewBox="0 0 24 24"><path d="M3 6h11v9H3z"/><path d="M14 9h4l3 3v3h-7z"/><circle cx="7" cy="18" r="1.7"/><circle cx="17" cy="18" r="1.7"/></svg></span>
        <span class="iconcircle lg amber"><svg class="ic" viewBox="0 0 24 24"><path d="M20 12l-8 8-8-8V4h8z"/></svg></span>
        <span class="iconcircle lg pink"><svg class="ic" viewBox="0 0 24 24"><rect x="3" y="9" width="18" height="12" rx="1.5"/><path d="M3 13h18M12 9v12"/></svg></span>
        <span class="iconcircle md"><svg class="ic" viewBox="0 0 24 24"><circle cx="12" cy="8" r="4"/><path d="M4 21c0-4 4-6 8-6s8 2 8 6"/></svg></span>
      </div>
    </div>

    <div class="card card-b comp-sec"><h3>Buttons</h3>
      <div class="swatch">
        <button class="btn btn-brand">Primary</button><button class="btn btn-accent">Accent</button>
        <button class="btn btn-outline">Outline</button><button class="btn btn-danger">Danger</button>
        <button class="btn btn-icon btn-brand"><svg class="ic" viewBox="0 0 24 24"><path d="M12 5v14M5 12h14"/></svg></button>
        <button class="btn btn-sm btn-outline">Small</button><button class="btn btn-lg btn-brand">Large</button>
      </div>
    </div>

    <div class="card card-b comp-sec"><h3>Badges &amp; Alerts</h3>
      <div class="swatch" style="margin-bottom:14px">
        <span class="badge badge-green">Completed</span><span class="badge badge-amber">Pending</span>
        <span class="badge badge-red">Cancelled</span><span class="badge badge-gray">Draft</span><span class="badge badge-brand">New</span>
      </div>
      <div class="alert alert-ok">Order placed successfully.</div>
      <div class="alert alert-err">Something went wrong. Please try again.</div>
    </div>

    <div class="card card-b comp-sec"><h3>Form Controls · Date · Date-range · Search</h3>
      <div class="row" style="align-items:flex-end">
        <div class="field" style="flex:1;min-width:200px"><label class="lbl">Full name</label><input class="input" value="Priya Sharma"></div>
        <div class="field"><label class="lbl">Single select</label><br><select class="select-pill"><option>All categories</option><option>Fruits</option></select></div>
      </div>
      <div class="row" style="align-items:flex-end;margin-top:6px">
        <div class="field"><label class="lbl">Date range</label>
          <div class="daterange"><div class="search"><svg class="ic" viewBox="0 0 24 24"><rect x="3" y="5" width="18" height="16" rx="2.5"/><path d="M3 10h18M8 3v4M16 3v4"/></svg><input value="01 Jul 2026" style="width:104px"></div><span class="sep">→</span><div class="search"><svg class="ic" viewBox="0 0 24 24"><rect x="3" y="5" width="18" height="16" rx="2.5"/><path d="M3 10h18M8 3v4M16 3v4"/></svg><input value="16 Jul 2026" style="width:104px"></div></div></div>
        <div class="field" style="flex:1;min-width:220px"><label class="lbl">Search</label><div class="search"><svg class="ic" viewBox="0 0 24 24"><circle cx="11" cy="11" r="7"/><path d="M21 21l-4.3-4.3"/></svg><input placeholder="Search products, stores..."></div></div>
      </div>
    </div>

    <div class="card card-b comp-sec"><h3>Multi-select chips · Checkbox · Radio · Toggle</h3>
      <div class="field" style="max-width:520px"><label class="lbl">Categories</label>
        <div class="ms"><span class="chip">Fruits <b>×</b></span><span class="chip">Vegetables <b>×</b></span><span class="chip">Dairy &amp; Eggs <b>×</b></span><input placeholder="Add more…"></div></div>
      <div class="row" style="gap:40px;align-items:flex-start;margin-top:8px">
        <div><div class="lbl">Checkbox</div>
          <label class="opt"><input type="checkbox" checked><span class="bx"><svg class="ic" viewBox="0 0 24 24"><path d="M5 12l5 5 9-11"/></svg></span> In stock only</label><br>
          <label class="opt"><input type="checkbox"><span class="bx"><svg class="ic" viewBox="0 0 24 24"><path d="M5 12l5 5 9-11"/></svg></span> On offer</label></div>
        <div><div class="lbl">Radio</div>
          <label class="opt"><input type="radio" name="r" checked><span class="bx rd"></span> All stores</label><br>
          <label class="opt"><input type="radio" name="r"><span class="bx rd"></span> Nearby only</label></div>
        <div><div class="lbl">Toggle</div>
          <label class="switch"><input type="checkbox" checked><span class="tk"></span> Notifications</label></div>
      </div>
    </div>

    <div class="card card-b comp-sec"><h3>Combobox · File upload</h3>
      <div class="row" style="gap:24px;align-items:flex-start">
        <div class="field" style="flex:1;min-width:240px"><label class="lbl">Combobox (searchable)</label>
          <div class="combo"><div class="search"><svg class="ic" viewBox="0 0 24 24"><circle cx="11" cy="11" r="7"/><path d="M21 21l-4.3-4.3"/></svg><input value="Ba"></div>
            <div class="combo-menu"><div class="ms-opt hl">Banana</div><div class="ms-opt">Basmati Rice</div><div class="ms-opt">Bakery Bread</div></div></div></div>
        <div class="field" style="flex:1;min-width:240px"><label class="lbl">File upload</label>
          <div class="file-drop"><span class="iconcircle md mint"><svg class="ic" viewBox="0 0 24 24"><path d="M12 3l8 4.5v9L12 21l-8-4.5v-9z"/></svg></span><div><b>Click to upload</b> or drag &amp; drop</div><div style="font-size:12px">PNG, JPG or PDF up to 5 MB</div></div></div>
      </div>
    </div>

    <div class="comp-sec"><h3>Data Table · Show / Sort / Pagination</h3>
      <div class="tbl-wrap">
        <div class="tbl-toolbar"><div class="tabs"><a class="active">All</a><a>Completed</a><a>Pending</a><a>Cancelled</a></div>
          <div class="show right">Show <select class="select-pill"><option>10</option></select></div>
          <div class="sort">Sort by <select class="select-pill"><option>Date</option></select></div></div>
        <table class="table"><thead><tr><th class="sortable desc">Client<span class="arw">▾</span></th><th>Date</th><th>Price</th><th>City</th><th>Status</th></tr></thead><tbody>
          <tr><td><b>Aarav Mehta</b></td><td class="muted">16 Jul 2026</td><td><b>₹1,240</b></td><td>Mumbai</td><td><span class="badge badge-green">Completed</span></td></tr>
          <tr><td><b>Diya Nair</b></td><td class="muted">15 Jul 2026</td><td><b>₹680</b></td><td>Pune</td><td><span class="badge badge-amber">In Progress</span></td></tr>
        </tbody></table>
        <div class="pager"><span class="muted">Showing 1–2 of 128</span><div class="pages"><a class="disabled">‹</a><a class="active">1</a><a>2</a><a>3</a><a>›</a></div></div>
      </div>
    </div>

    <div class="card card-b comp-sec"><h3>Stat Tiles (with sparklines)</h3>
      <div class="tiles" style="margin-bottom:0">
        <div class="tile"><span class="iconcircle md"><svg class="ic" viewBox="0 0 24 24"><rect x="3" y="6" width="18" height="13" rx="2.5"/><path d="M3 10h18"/></svg></span>
          <div><div class="k">Revenue</div><div class="v">₹4.82L</div><svg class="spark" viewBox="0 0 72 28" preserveAspectRatio="none"><polyline class="ln" points="0,22 12,18 24,20 36,10 48,13 60,6 72,4"/></svg></div></div>
        <div class="tile"><span class="iconcircle md mint"><svg class="ic" viewBox="0 0 24 24"><path d="M6 8h12l-1 12H7z"/><path d="M9 8V6a3 3 0 0 1 6 0v2"/></svg></span>
          <div><div class="k">Orders</div><div class="v">1,284</div><svg class="spark" viewBox="0 0 72 28" preserveAspectRatio="none"><polyline class="ln" points="0,16 12,18 24,10 36,14 48,8 60,11 72,6"/></svg></div></div>
        <div class="tile"><span class="iconcircle md sky"><svg class="ic" viewBox="0 0 24 24"><circle cx="9" cy="8" r="3.5"/><path d="M2 20c0-3.5 3-5.5 7-5.5s7 2 7 5.5"/></svg></span>
          <div><div class="k">Customers</div><div class="v">642</div><svg class="spark down" viewBox="0 0 72 28" preserveAspectRatio="none"><polyline class="ln" points="0,6 12,9 24,7 36,12 48,10 60,16 72,18"/></svg></div></div>
      </div>
    </div>

    <div class="dash-grid comp-sec">
      <div class="panel"><h3>Line / Trend chart</h3>
        <div class="linechart"><svg viewBox="0 0 600 190" preserveAspectRatio="none">
          <line class="gl" x1="0" y1="48" x2="600" y2="48"/><line class="gl" x1="0" y1="95" x2="600" y2="95"/><line class="gl" x1="0" y1="142" x2="600" y2="142"/>
          <polyline class="ln" points="0,150 100,120 200,135 300,80 400,95 500,45 600,55"/>
          <circle class="dot" cx="0" cy="150" r="4"/><circle class="dot" cx="100" cy="120" r="4"/><circle class="dot" cx="200" cy="135" r="4"/><circle class="dot" cx="300" cy="80" r="4"/><circle class="dot" cx="400" cy="95" r="4"/><circle class="dot" cx="500" cy="45" r="4"/><circle class="dot" cx="600" cy="55" r="4"/>
        </svg></div></div>
      <div class="panel"><h3>Recent Activity feed</h3>
        <div class="feed">
          <div class="fitem"><span class="fdot"><svg class="ic" viewBox="0 0 24 24"><path d="M6 8h12l-1 12H7z"/><path d="M9 8V6a3 3 0 0 1 6 0v2"/></svg></span><div class="fbody"><b>New order <span class="fx">#KIR-0042</span></b><div class="ft">Andheri West · 3 items · 2 min ago</div></div></div>
          <div class="fitem"><span class="fdot amber"><svg class="ic" viewBox="0 0 24 24"><path d="M12 3l8 4.5v9L12 21l-8-4.5v-9z"/></svg></span><div class="fbody"><b>Low stock: Milk 1L</b><div class="ft">4 left · 18 min ago</div></div></div>
          <div class="fitem"><span class="fdot sky"><svg class="ic" viewBox="0 0 24 24"><path d="M3 6h11v9H3z"/><path d="M14 9h4l3 3v3h-7z"/></svg></span><div class="fbody"><b>Order out for delivery</b><div class="ft">#KIR-0039 · 32 min ago</div></div></div>
          <div class="fitem"><span class="fdot pink"><svg class="ic" viewBox="0 0 24 24"><circle cx="12" cy="8" r="4"/><path d="M4 21c0-4 4-6 8-6s8 2 8 6"/></svg></span><div class="fbody"><b>New customer signed up</b><div class="ft">via OTP · 1 hr ago</div></div></div>
        </div>
      </div>
    </div>
  </div>

  <footer class="site-foot"><div class="container"><div class="copy"><span>© 2026 KiranaManagement — Component Library.</span><span>Made with care · Mansi, Hiral &amp; Zigma</span></div></div></footer>
  <script src="assets/app.js"></script>
</body>
</html>
