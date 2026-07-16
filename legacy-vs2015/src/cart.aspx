<%@ Page Language="C#" %>
<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="utf-8" /><meta name="viewport" content="width=device-width, initial-scale=1" />
  <title>Cart · KiranaManagement</title>
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
    <h2>Your cart</h2>
    <div class="card"><div class="card-b" id="items"></div></div>
    <div class="card" style="margin-top:14px"><div class="card-b" id="summary"></div></div>
  </div>

  <div class="foot"><div class="container">© 2026 KiranaManagement</div></div>

  <script src="assets/app.js"></script>
  <script>
  function render(){
    var items = KA.cart.get();
    var box = document.getElementById('items');
    if(!items.length){ box.innerHTML='<p class="muted">Your cart is empty. <a href="shop.aspx">Start shopping →</a></p>';
      document.getElementById('summary').innerHTML=''; return; }
    var h='<table class="table"><thead><tr><th>Item</th><th>Price</th><th>Qty</th><th>Total</th><th></th></tr></thead><tbody>';
    items.forEach(function(x){
      h+='<tr><td><strong>'+x.productName+'</strong> · '+x.variantName+'<div class="muted" style="font-size:12px">'+x.storeName+'</div></td>'+
        '<td>'+KA.money(x.price)+'</td>'+
        '<td><button class="btn btn-sm btn-outline" onclick="chg(\''+x.variantId+'\',-1)">−</button> '+x.qty+' '+
        '<button class="btn btn-sm btn-outline" onclick="chg(\''+x.variantId+'\',1)">+</button></td>'+
        '<td>'+KA.money(x.price*x.qty)+'</td>'+
        '<td><button class="btn btn-sm btn-danger" onclick="rm(\''+x.variantId+'\')">Remove</button></td></tr>';
    });
    box.innerHTML=h+'</tbody></table>';
    var sub=KA.cart.subtotal(); var del=sub>=500?0:40;
    document.getElementById('summary').innerHTML=
      '<div class="row"><div>Subtotal</div><div class="right">'+KA.money(sub)+'</div></div>'+
      '<div class="row"><div>Delivery '+(del===0?'(free over ₹500)':'')+'</div><div class="right">'+KA.money(del)+'</div></div>'+
      '<hr><div class="row"><div><strong>Total</strong></div><div class="right"><strong>'+KA.money(sub+del)+'</strong></div></div>'+
      '<a href="checkout.aspx" class="btn btn-brand btn-block" style="margin-top:12px">Proceed to checkout</a>';
  }
  function chg(id,d){ var it=KA.cart.get().filter(function(x){return x.variantId===id;})[0]; if(it){ KA.cart.setQty(id, it.qty+d); render(); } }
  function rm(id){ KA.cart.remove(id); render(); }
  render();
  </script>
</body>
</html>
