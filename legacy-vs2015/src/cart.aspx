<%@ Page Language="C#" %>
<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="utf-8" /><meta name="viewport" content="width=device-width, initial-scale=1" />
  <title>Cart · Kirana</title>
  <link rel="stylesheet" href="assets/app.css" />
</head>
<body>
  <header class="appbar"><div class="container">
    <a class="logo" href="home.aspx"><span class="lm"><svg class="ic" viewBox="0 0 24 24"><path d="M11 20A7 7 0 0 1 4 13C4 8 8 4 20 4c0 12-4 16-9 16z"/><path d="M9 17c1-4 4-7 8-9"/></svg></span>Kirana</a>
    <nav class="navlinks"><a href="home.aspx">Home</a><a href="shop.aspx">Categories</a></nav>
    <div class="acts right"><a class="cartpill" href="cart.aspx"><svg class="ic" viewBox="0 0 24 24"><circle cx="9" cy="21" r="1.6"/><circle cx="18" cy="21" r="1.6"/><path d="M2.5 3h2l2.2 12.4a2 2 0 0 0 2 1.6h8.3a2 2 0 0 0 2-1.6L22 8H6"/></svg> <span data-cart-count="">0</span></a></div>
  </div></header>

  <div class="container" style="padding-top:24px">
    <h2 style="margin-bottom:18px">Your Cart <span class="muted" style="font-size:15px;font-weight:600" id="count"></span></h2>
    <div class="split">
      <div class="card card-b" id="items"></div>
      <div class="card card-b" id="summary"></div>
    </div>
  </div>

  <footer class="site-foot"><div class="container"><div class="copy"><span>© 2026 KiranaManagement — Multi-store grocery marketplace.</span></div></div></footer>

  <script src="assets/app.js"></script>
  <script>
  var EMOJI={'default':'🧺'};
  function render(){
    var items = KA.cart.get();
    document.getElementById('count').textContent = items.length ? ('· '+items.length+' items') : '';
    var box = document.getElementById('items');
    if(!items.length){ box.innerHTML='<p class="muted">Your cart is empty. <a class="link" href="shop.aspx">Start shopping →</a></p>';
      document.getElementById('summary').innerHTML=''; return; }
    var h='';
    items.forEach(function(x){
      h+='<div class="line-item"><div class="li-thumb">🧺</div>'+
        '<div style="flex:1"><div style="font-weight:800">'+x.productName+'</div><div class="muted" style="font-size:12.5px">'+x.variantName+' · '+x.storeName+'</div></div>'+
        '<div class="qty"><button onclick="chg(\''+x.variantId+'\',-1)">−</button><span>'+x.qty+'</span><button onclick="chg(\''+x.variantId+'\',1)">+</button></div>'+
        '<div style="width:84px;text-align:right;font-weight:900">'+KA.money(x.price*x.qty)+'</div></div>';
    });
    h+='<a class="link" href="shop.aspx" style="margin-top:14px;display:inline-flex">← Continue shopping</a>';
    box.innerHTML=h;
    var sub=KA.cart.subtotal(); var del=sub>=500?0:40; var fee=6; var gst=Math.round(sub*0.05);
    document.getElementById('summary').innerHTML=
      '<h3 style="margin-bottom:10px">Order Summary</h3>'+
      '<div class="sumrow"><span>Subtotal</span><b>'+KA.money(sub)+'</b></div>'+
      '<div class="sumrow"><span>Delivery fee</span><b>'+(del===0?'<span class="badge badge-green">FREE</span>':KA.money(del))+'</b></div>'+
      '<div class="sumrow"><span>Platform fee</span><b>'+KA.money(fee)+'</b></div>'+
      '<div class="sumrow"><span>Taxes (GST)</span><b>'+KA.money(gst)+'</b></div>'+
      '<div class="sumrow total"><span>Total</span><span>'+KA.money(sub+del+fee+gst)+'</span></div>'+
      '<a href="checkout.aspx" class="btn btn-brand btn-block btn-lg" style="margin-top:14px">Checkout →</a>'+
      '<div class="muted" style="font-size:12px;text-align:center;margin-top:10px">Secure checkout</div>';
  }
  function chg(id,d){ var it=KA.cart.get().filter(function(x){return x.variantId===id;})[0]; if(it){ KA.cart.setQty(id, it.qty+d); render(); } }
  render();
  </script>
</body>
</html>
