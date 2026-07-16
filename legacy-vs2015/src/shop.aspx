<%@ Page Language="C#" %>
<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="utf-8" /><meta name="viewport" content="width=device-width, initial-scale=1" />
  <title>Shop · KiranaManagement</title>
  <link rel="stylesheet" href="assets/app.css" />
</head>
<body>
  <div class="topnav"><div class="container">
    <span class="brand">🛒 Kirana</span>
    <a class="navlink" href="shop.aspx">Shop</a>
    <a class="navlink right cartbtn" href="cart.aspx">🛍️ Cart<span class="count" data-cart-count="">0</span></a>
    <span data-acct=""></span>
  </div></div>

  <div class="hero"><div class="container">
    <h1>Fresh groceries from your neighbourhood stores</h1>
    <p>Browse products from all approved Kirana stores — one cart, one delivery.</p>
  </div></div>

  <div class="container">
    <div id="out"></div>
    <div class="products" id="grid"></div>
  </div>

  <div class="foot"><div class="container">© 2026 KiranaManagement — Multi-store grocery marketplace</div></div>

  <script src="assets/app.js"></script>
  <script>
  async function load(){
    try{
      var products = await KA.api('/api/marketplace/products');
      var grid = document.getElementById('grid'); grid.innerHTML='';
      if(!products || !products.length){ document.getElementById('out').innerHTML='<div class="alert">No products available yet.</div>'; return; }
      products.forEach(function(p){
        p.variants.forEach(function(v){
          var card = document.createElement('div'); card.className='pcard';
          var off = v.discountPercent>0 ? '<span class="off">'+v.discountPercent+'% off</span>' : '';
          card.innerHTML =
            '<div class="thumb">🧺</div>'+
            '<div class="store">'+p.storeName+'</div>'+
            '<div class="name">'+p.name+' · '+v.name+'</div>'+
            '<div style="margin:6px 0"><span class="price">'+KA.money(v.sellingPrice)+'</span>'+
              (v.mrp>v.sellingPrice?'<span class="mrp">'+KA.money(v.mrp)+'</span>':'')+' '+off+'</div>'+
            '<div class="muted" style="font-size:12px;margin-bottom:8px">'+(v.stockQuantity>0?('In stock: '+v.stockQuantity):'Out of stock')+'</div>';
          var btn = document.createElement('button'); btn.className='btn btn-brand btn-block'; btn.textContent='Add to cart';
          btn.disabled = v.stockQuantity<=0;
          btn.onclick = function(){
            KA.cart.add({ variantId:v.id, storeId:p.storeId, storeName:p.storeName, productName:p.name, variantName:v.name, mrp:v.mrp, price:v.sellingPrice, qty:1 });
            KA.toast('Added to cart','ok');
          };
          card.appendChild(btn); grid.appendChild(card);
        });
      });
    }catch(e){ document.getElementById('out').innerHTML='<div class="alert alert-err">'+e+'</div>'; }
  }
  load();
  </script>
</body>
</html>
