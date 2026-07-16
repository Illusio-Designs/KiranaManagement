<%@ Page Language="C#" %>
<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="utf-8" /><meta name="viewport" content="width=device-width, initial-scale=1" />
  <title>My Orders · Kirana</title>
  <link rel="stylesheet" href="assets/app.css" />
</head>
<body>
  <header class="appbar"><div class="container">
    <a class="logo" href="home.aspx"><span class="lm"><svg class="ic" viewBox="0 0 24 24"><path d="M11 20A7 7 0 0 1 4 13C4 8 8 4 20 4c0 12-4 16-9 16z"/><path d="M9 17c1-4 4-7 8-9"/></svg></span>Kirana</a>
    <nav class="navlinks"><a href="home.aspx">Home</a><a href="shop.aspx">Categories</a></nav>
    <div class="acts right"><a class="cartpill" href="cart.aspx"><svg class="ic" viewBox="0 0 24 24"><circle cx="9" cy="21" r="1.6"/><circle cx="18" cy="21" r="1.6"/><path d="M2.5 3h2l2.2 12.4a2 2 0 0 0 2 1.6h8.3a2 2 0 0 0 2-1.6L22 8H6"/></svg> <span data-cart-count="">0</span></a></div>
  </div></header>

  <div class="container" style="padding-top:24px">
    <div class="section-h"><h2>My Orders</h2><span class="badge badge-brand" id="ocount"></span></div>
    <div id="list"></div>
  </div>

  <footer class="site-foot"><div class="container"><div class="copy"><span>© 2026 KiranaManagement — Multi-store grocery marketplace.</span></div></div></footer>

  <script src="assets/app.js"></script>
  <script>
  if(!KA.token() || KA.role()!=='Customer'){ location.href='login.aspx'; }
  function pill(s){ var m={'Delivered':'badge-green','OutForDelivery':'badge-amber','Placed':'badge-brand','Accepted':'badge-brand','Packed':'badge-amber','Cancelled':'badge-red'}; return '<span class="badge '+(m[s]||'badge-gray')+'">'+s+'</span>'; }
  async function load(){
    try{
      var orders = await KA.api('/api/orders/mine');
      document.getElementById('ocount').textContent=(orders?orders.length:0)+' orders';
      if(!orders || !orders.length){ document.getElementById('list').innerHTML='<div class="card"><div class="card-b muted">No orders yet. <a class="link" href="shop.aspx">Shop now →</a></div></div>'; return; }
      var h='<div class="tbl-wrap"><table class="table"><thead><tr><th>Order</th><th>Date</th><th>Items</th><th>Total</th><th>Status</th></tr></thead><tbody>';
      orders.forEach(function(o){
        var items=o.lines.reduce(function(s,l){return s+l.quantity;},0);
        h+='<tr><td><b>'+o.orderNumber+'</b></td><td class="muted">'+new Date(o.createdAt).toLocaleDateString()+'</td>'+
           '<td>'+items+' items</td><td><b>'+KA.money(o.grandTotal)+'</b></td><td>'+pill(o.status)+'</td></tr>';
      });
      document.getElementById('list').innerHTML=h+'</tbody></table></div>';
    }catch(e){ document.getElementById('list').innerHTML='<div class="alert alert-err">'+e+'</div>'; }
  }
  load();
  </script>
</body>
</html>
