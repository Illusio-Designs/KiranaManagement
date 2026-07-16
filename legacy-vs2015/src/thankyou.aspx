<%@ Page Language="C#" %>
<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="utf-8" /><meta name="viewport" content="width=device-width, initial-scale=1" />
  <title>Order placed · Kirana</title>
  <link rel="stylesheet" href="assets/app.css" />
</head>
<body>
  <header class="appbar"><div class="container">
    <a class="logo" href="home.aspx"><span class="lm"><svg class="ic" viewBox="0 0 24 24"><path d="M11 20A7 7 0 0 1 4 13C4 8 8 4 20 4c0 12-4 16-9 16z"/><path d="M9 17c1-4 4-7 8-9"/></svg></span>Kirana</a>
    <div class="acts right"><a class="cartpill" href="cart.aspx"><svg class="ic" viewBox="0 0 24 24"><circle cx="9" cy="21" r="1.6"/><circle cx="18" cy="21" r="1.6"/><path d="M2.5 3h2l2.2 12.4a2 2 0 0 0 2 1.6h8.3a2 2 0 0 0 2-1.6L22 8H6"/></svg> <span data-cart-count="">0</span></a></div>
  </div></header>

  <div class="thanks"><div class="thanks-card">
    <span class="iconcircle xl solid"><svg class="ic" viewBox="0 0 24 24"><path d="M5 12l5 5 9-11"/></svg></span>
    <h1 style="font-size:32px;margin-top:18px">Thank you for your order!</h1>
    <p class="muted" style="font-size:16px" id="sub">Loading your order…</p>
    <div class="card card-b" style="text-align:left;margin:22px 0" id="detail"></div>
    <div class="row" style="justify-content:center">
      <a href="orders.aspx" class="btn btn-brand btn-lg">Track Order</a>
      <a href="shop.aspx" class="btn btn-outline btn-lg">Continue Shopping</a>
    </div>
  </div></div>

  <footer class="site-foot"><div class="container"><div class="copy"><span>© 2026 KiranaManagement — Multi-store grocery marketplace.</span></div></div></footer>

  <script src="assets/app.js"></script>
  <script>
  function qs(k){ var m=location.search.match(new RegExp('[?&]'+k+'=([^&]+)')); return m?decodeURIComponent(m[1]):''; }
  async function load(){
    var id=qs('id'); if(!id){ location.href='shop.aspx'; return; }
    try{
      var o = await KA.api('/api/orders/'+id);
      document.getElementById('sub').innerHTML='Your order <b>'+o.orderNumber+'</b> has been placed successfully and is being prepared.';
      var eta = o.etaMinutes ? (o.etaMinutes+' min') : 'soon';
      var h='<div class="sumrow"><span class="muted">Status</span><b>'+o.status+'</b></div>'+
        '<div class="sumrow"><span class="muted">Estimated delivery</span><b>'+eta+(o.distanceKm!=null?(' · '+o.distanceKm+' km'):'')+'</b></div>'+
        '<div class="sumrow"><span class="muted">Delivering to</span><b>'+(o.city||'')+' '+(o.pincode||'')+'</b></div>';
      o.lines.forEach(function(l){ h+='<div class="sumrow"><span>'+l.productName+' · '+l.variantName+' × '+l.quantity+'</span><b>'+KA.money(l.lineTotal)+'</b></div>'; });
      h+='<div class="sumrow"><span>Delivery</span><b>'+KA.money(o.deliveryFee)+'</b></div>'+
         '<div class="sumrow total"><span>Total paid</span><span>'+KA.money(o.grandTotal)+'</span></div>';
      document.getElementById('detail').innerHTML=h;
    }catch(e){ document.getElementById('detail').innerHTML='<div class="alert alert-err">'+e+'</div>'; }
  }
  load();
  </script>
</body>
</html>
