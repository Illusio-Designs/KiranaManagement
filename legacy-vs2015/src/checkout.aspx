<%@ Page Language="C#" %>
<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="utf-8" /><meta name="viewport" content="width=device-width, initial-scale=1" />
  <title>Checkout · KiranaManagement</title>
  <link rel="stylesheet" href="assets/app.css" />
</head>
<body>
  <header class="appbar"><div class="container">
    <a class="logo" href="home.aspx"><span class="lm"><svg class="ic" viewBox="0 0 24 24"><path d="M11 20A7 7 0 0 1 4 13C4 8 8 4 20 4c0 12-4 16-9 16z"/><path d="M9 17c1-4 4-7 8-9"/></svg></span>Kirana</a>
    <div class="acts right"><a class="cartpill" href="cart.aspx"><svg class="ic" viewBox="0 0 24 24"><circle cx="9" cy="21" r="1.6"/><circle cx="18" cy="21" r="1.6"/><path d="M2.5 3h2l2.2 12.4a2 2 0 0 0 2 1.6h8.3a2 2 0 0 0 2-1.6L22 8H6"/></svg> <span data-cart-count="">0</span></a></div>
  </div></header>

  <div class="container" style="padding-top:24px">
    <div class="steps">
      <div class="step"><span class="num">1</span> Cart</div>
      <div class="step active"><span class="num">2</span> Delivery &amp; Payment</div>
      <div class="step"><span class="num">3</span> Confirmation</div>
    </div>
    <div class="split">
      <div class="card card-b">
        <h3 style="margin-bottom:12px">Delivery Address</h3>
        <div class="row"><div class="field" style="flex:1"><label class="lbl">Full name</label><input id="c-name" class="input"></div>
          <div class="field" style="flex:1"><label class="lbl">Phone</label><input id="c-phone" class="input"></div></div>
        <div class="field"><label class="lbl">Address</label><textarea id="c-addr" class="input" rows="2"></textarea></div>
        <div class="row"><div class="field" style="flex:1"><label class="lbl">City</label><input id="c-city" class="input"></div>
          <div class="field" style="flex:1"><label class="lbl">Pincode</label><input id="c-pin" class="input"></div></div>

        <div class="field">
          <label class="lbl">Delivery location (for accurate ETA)</label>
          <button type="button" class="btn btn-outline" onclick="detectLocation()">
            <svg class="ic" viewBox="0 0 24 24"><path d="M12 21c5-5 7-8 7-11a7 7 0 0 0-14 0c0 3 2 6 7 11z"/><circle cx="12" cy="10" r="2.5"/></svg>
            Use my current location
          </button>
          <div id="geo" class="muted" style="font-size:12.5px;margin-top:8px">Location not set — delivery fee is estimated from cart total.</div>
        </div>

        <button class="btn btn-brand btn-block btn-lg" style="margin-top:6px" onclick="placeOrder()">Place Order</button>
        <div id="out" style="margin-top:8px"></div>
      </div>
      <div class="card card-b" id="summary"></div>
    </div>
  </div>

  <footer class="site-foot"><div class="container">
    <div class="copy"><span>© 2026 KiranaManagement — Multi-store grocery marketplace.</span></div>
  </div></footer>

  <script src="assets/app.js"></script>
  <script>
  // No guest orders — must be logged in as a customer.
  if(!KA.token() || KA.role()!=='Customer'){ alert('Please log in as a customer to check out.'); location.href='login.aspx'; }
  var geo = { lat:null, lng:null };

  function detectLocation(){
    var el=document.getElementById('geo');
    if(!navigator.geolocation){ el.textContent='Geolocation is not supported by this browser.'; return; }
    el.textContent='Detecting your location…';
    navigator.geolocation.getCurrentPosition(function(pos){
      geo.lat=pos.coords.latitude; geo.lng=pos.coords.longitude;
      el.innerHTML='<span class="badge badge-green">Location set</span> '+geo.lat.toFixed(4)+', '+geo.lng.toFixed(4)+' — ETA will be computed from the nearest store.';
    }, function(){ el.textContent='Could not get location. You can still place the order.'; });
  }

  function summary(){
    var items=KA.cart.get();
    if(!items.length){ location.href='cart.aspx'; return; }
    var nameEl=document.getElementById('c-name'); if(nameEl && !nameEl.value) nameEl.value=KA.uname();
    var sub=KA.cart.subtotal();
    var h='<h3 style="margin-bottom:10px">Order Summary</h3>';
    items.forEach(function(x){ h+='<div class="sumrow"><span>'+x.productName+' · '+x.variantName+' × '+x.qty+'</span><b>'+KA.money(x.price*x.qty)+'</b></div>'; });
    h+='<div class="sumrow"><span>Subtotal</span><b>'+KA.money(sub)+'</b></div>'+
       '<div class="sumrow"><span>Delivery</span><b>'+(sub>=500?'<span class="badge badge-green">FREE</span>':'calculated at checkout')+'</b></div>'+
       '<div class="sumrow total"><span>Estimated Total</span><span>'+KA.money(sub)+'+</span></div>';
    document.getElementById('summary').innerHTML=h;
  }

  async function placeOrder(){
    var out=document.getElementById('out');
    var name=document.getElementById('c-name').value.trim();
    var phone=document.getElementById('c-phone').value.trim();
    var addr=document.getElementById('c-addr').value.trim();
    var city=document.getElementById('c-city').value.trim();
    var pin=document.getElementById('c-pin').value.trim();
    if(!name||!phone||!addr){ out.innerHTML='<div class="alert alert-err">Please fill name, phone and address.</div>'; return; }
    var lines=KA.cart.get().map(function(x){ return { productVariantId:x.variantId, quantity:x.qty }; });
    try{
      var order = await KA.api('/api/orders','POST',{ customerName:name, phone:phone, address:addr, city:city, pincode:pin, latitude:geo.lat, longitude:geo.lng, lines:lines });
      KA.cart.clear();
      location.href='thankyou.aspx?id='+order.id;
    }catch(e){ out.innerHTML='<div class="alert alert-err">'+e+'</div>'; }
  }
  summary();
  </script>
</body>
</html>
