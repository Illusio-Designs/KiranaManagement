<%@ Page Language="C#" %>
<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="utf-8" /><meta name="viewport" content="width=device-width, initial-scale=1" />
  <title>Checkout · KiranaManagement</title>
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
    <h2>Checkout</h2>
    <div class="row" style="align-items:flex-start">
      <div class="card" style="flex:1;min-width:280px"><div class="card-b">
        <h3>Delivery details</h3>
        <div class="field"><label class="lbl">Full name</label><input id="c-name" class="input"></div>
        <div class="field"><label class="lbl">Phone</label><input id="c-phone" class="input"></div>
        <div class="field"><label class="lbl">Address</label><textarea id="c-addr" class="input" rows="3"></textarea></div>
        <button class="btn btn-brand btn-block" onclick="placeOrder()">Place order</button>
        <div id="out" style="margin-top:8px"></div>
      </div></div>
      <div class="card" style="flex:1;min-width:260px"><div class="card-b" id="summary"></div></div>
    </div>
  </div>

  <div class="foot"><div class="container">© 2026 KiranaManagement</div></div>

  <script src="assets/app.js"></script>
  <script>
  // No guest orders — must be logged in as a customer.
  if(!KA.token() || KA.role()!=='Customer'){
    alert('Please log in as a customer to check out.');
    location.href='login.aspx';
  }
  function summary(){
    var items=KA.cart.get();
    if(!items.length){ location.href='cart.aspx'; return; }
    var nameEl=document.getElementById('c-name'); if(nameEl && !nameEl.value) nameEl.value=KA.uname();
    var sub=KA.cart.subtotal(); var del=sub>=500?0:40;
    var h='<h3>Order summary</h3><table class="table">';
    items.forEach(function(x){ h+='<tr><td>'+x.productName+' · '+x.variantName+' × '+x.qty+'</td><td class="right">'+KA.money(x.price*x.qty)+'</td></tr>'; });
    h+='</table><div class="row"><div>Subtotal</div><div class="right">'+KA.money(sub)+'</div></div>'+
       '<div class="row"><div>Delivery</div><div class="right">'+KA.money(del)+'</div></div>'+
       '<hr><div class="row"><div><strong>Total</strong></div><div class="right"><strong>'+KA.money(sub+del)+'</strong></div></div>';
    document.getElementById('summary').innerHTML=h;
  }
  async function placeOrder(){
    var out=document.getElementById('out');
    var name=document.getElementById('c-name').value.trim();
    var phone=document.getElementById('c-phone').value.trim();
    var addr=document.getElementById('c-addr').value.trim();
    if(!name||!phone||!addr){ out.innerHTML='<div class="alert alert-err">Please fill name, phone and address.</div>'; return; }
    var lines=KA.cart.get().map(function(x){ return { productVariantId:x.variantId, quantity:x.qty }; });
    try{
      var order = await KA.api('/api/orders','POST',{ customerName:name, phone:phone, address:addr, lines:lines });
      KA.cart.clear();
      location.href='thankyou.aspx?id='+order.id;
    }catch(e){ out.innerHTML='<div class="alert alert-err">'+e+'</div>'; }
  }
  summary();
  </script>
</body>
</html>
