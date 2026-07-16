<%@ Page Language="C#" %>
<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="utf-8" /><meta name="viewport" content="width=device-width, initial-scale=1" />
  <title>My orders · KiranaManagement</title>
  <link rel="stylesheet" href="assets/app.css" />
</head>
<body>
  <div class="topnav"><div class="container">
    <span class="brand">🛒 Kirana</span>
    <a class="navlink" href="shop.aspx">Shop</a>
    <a class="navlink right cartbtn" href="cart.aspx">🛍️ Cart<span class="count" data-cart-count>0</span></a>
    <span data-acct></span>
  </div></div>

  <div class="container" style="max-width:820px;margin-top:22px">
    <h2>My orders</h2>
    <div id="list"></div>
  </div>
  <div class="foot"><div class="container">© 2026 KiranaManagement</div></div>

  <script src="assets/app.js"></script>
  <script>
  if(!KA.token() || KA.role()!=='Customer'){ location.href='login.aspx'; }
  async function load(){
    try{
      var orders = await KA.api('/api/orders/mine');
      if(!orders || !orders.length){ document.getElementById('list').innerHTML='<div class="card"><div class="card-b muted">No orders yet. <a href="shop.aspx">Shop now →</a></div></div>'; return; }
      var h='';
      orders.forEach(function(o){
        h+='<div class="card" style="margin-bottom:12px"><div class="card-b">'+
          '<div class="row"><strong>'+o.orderNumber+'</strong><span class="right muted">'+new Date(o.createdAt).toLocaleString()+'</span></div>'+
          '<table class="table" style="margin-top:6px">';
        o.lines.forEach(function(l){ h+='<tr><td>'+l.productName+' · '+l.variantName+' × '+l.quantity+'</td><td class="muted">'+l.storeName+'</td><td class="right">'+KA.money(l.lineTotal)+'</td></tr>'; });
        h+='</table><div class="row"><div class="right"><strong>Total '+KA.money(o.grandTotal)+'</strong> (incl. delivery '+KA.money(o.deliveryFee)+')</div></div></div></div>';
      });
      document.getElementById('list').innerHTML=h;
    }catch(e){ document.getElementById('list').innerHTML='<div class="alert alert-err">'+e+'</div>'; }
  }
  load();
  </script>
</body>
</html>
