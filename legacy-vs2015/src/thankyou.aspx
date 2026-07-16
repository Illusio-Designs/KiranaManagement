<%@ Page Language="C#" %>
<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="utf-8" /><meta name="viewport" content="width=device-width, initial-scale=1" />
  <title>Order placed · KiranaManagement</title>
  <link rel="stylesheet" href="assets/app.css" />
</head>
<body>
  <div class="topnav"><div class="container">
    <span class="brand">🛒 Kirana</span>
    <a class="navlink" href="shop.aspx">Shop</a>
    <a class="navlink right cartbtn" href="cart.aspx">🛍️ Cart<span class="count" data-cart-count>0</span></a>
    <span data-acct></span>
  </div></div>

  <div class="container" style="max-width:720px;margin-top:26px">
    <div class="card"><div class="card-b" style="text-align:center">
      <div style="font-size:56px">✅</div>
      <h2>Thank you! Your order is placed</h2>
      <p class="muted" id="sub">Loading your order…</p>
    </div></div>
    <div class="card" style="margin-top:14px"><div class="card-b" id="detail"></div></div>
    <div style="text-align:center;margin-top:16px"><a href="shop.aspx" class="btn btn-brand">Continue shopping</a></div>
  </div>

  <div class="foot"><div class="container">© 2026 KiranaManagement</div></div>

  <script src="assets/app.js"></script>
  <script>
  function qs(k){ var m=location.search.match(new RegExp('[?&]'+k+'=([^&]+)')); return m?decodeURIComponent(m[1]):''; }
  async function load(){
    var id=qs('id'); if(!id){ location.href='shop.aspx'; return; }
    try{
      var o = await KA.api('/api/orders/'+id);
      document.getElementById('sub').innerHTML='Order <strong>'+o.orderNumber+'</strong> · '+new Date(o.createdAt).toLocaleString();
      var h='<h3>Items</h3><table class="table"><thead><tr><th>Store</th><th>Item</th><th>Qty</th><th>Total</th></tr></thead><tbody>';
      o.lines.forEach(function(l){ h+='<tr><td>'+l.storeName+'</td><td>'+l.productName+' · '+l.variantName+'</td><td>'+l.quantity+'</td><td>'+KA.money(l.lineTotal)+'</td></tr>'; });
      h+='</tbody></table><div class="row"><div>Subtotal</div><div class="right">'+KA.money(o.subtotal)+'</div></div>'+
         '<div class="row"><div>Delivery</div><div class="right">'+KA.money(o.deliveryFee)+'</div></div>'+
         '<hr><div class="row"><div><strong>Total paid</strong></div><div class="right"><strong>'+KA.money(o.grandTotal)+'</strong></div></div>'+
         '<div class="muted" style="margin-top:10px">Delivering to: '+o.customerName+', '+o.phone+' — '+o.address+'</div>';
      document.getElementById('detail').innerHTML=h;
    }catch(e){ document.getElementById('detail').innerHTML='<div class="alert alert-err">'+e+'</div>'; }
  }
  load();
  </script>
</body>
</html>
