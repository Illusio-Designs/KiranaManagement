<%@ Page Language="C#" %>
<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="utf-8" /><meta name="viewport" content="width=device-width, initial-scale=1" />
  <title>Shop · KiranaManagement</title>
  <link rel="stylesheet" href="assets/app.css" />
</head>
<body>
  <header class="appbar"><div class="container">
    <a class="logo" href="home.aspx"><span class="lm"><svg class="ic" viewBox="0 0 24 24"><path d="M11 20A7 7 0 0 1 4 13C4 8 8 4 20 4c0 12-4 16-9 16z"/><path d="M9 17c1-4 4-7 8-9"/></svg></span>Kirana</a>
    <nav class="navlinks">
      <a href="home.aspx">Home</a><a class="active" href="shop.aspx">Categories</a><a href="shop.aspx">Deals</a>
    </nav>
    <div class="search">
      <span><svg class="ic" viewBox="0 0 24 24"><circle cx="11" cy="11" r="7"/><path d="M21 21l-4.3-4.3"/></svg></span>
      <input id="q" placeholder="Search for products, brands..." onkeyup="if(event.key==='Enter')load()">
    </div>
    <div class="acts">
      <span data-acct=""><a class="icbtn" href="login.aspx"><svg class="ic" viewBox="0 0 24 24"><circle cx="12" cy="8" r="4"/><path d="M4 21c0-4 4-6 8-6s8 2 8 6"/></svg></a></span>
      <a class="cartpill" href="cart.aspx"><svg class="ic" viewBox="0 0 24 24"><circle cx="9" cy="21" r="1.6"/><circle cx="18" cy="21" r="1.6"/><path d="M2.5 3h2l2.2 12.4a2 2 0 0 0 2 1.6h8.3a2 2 0 0 0 2-1.6L22 8H6"/></svg> <span data-cart-count="">0</span></a>
    </div>
  </div></header>

  <div class="container" style="padding-top:22px">
    <div class="row" style="margin-bottom:14px">
      <div id="cats" class="tabs"></div>
      <div class="right sort">Sort by
        <select id="sort" class="select-pill" onchange="load()">
          <option value="popular">Popular</option>
          <option value="price_asc">Price: Low to High</option>
          <option value="price_desc">Price: High to Low</option>
          <option value="discount">Biggest Discount</option>
          <option value="name">Name (A–Z)</option>
        </select>
      </div>
    </div>
    <div class="section-h"><h2 id="heading">Fresh Picks</h2><span class="muted" id="count"></span></div>
    <div id="out"></div>
    <div class="products" id="grid"></div>
  </div>

  <footer class="site-foot"><div class="container">
    <div class="copy"><span>© 2026 KiranaManagement — Multi-store grocery marketplace.</span><span>Made with care · Mansi, Hiral &amp; Zigma</span></div>
  </div></footer>

  <script src="assets/app.js"></script>
  <script>
  var activeCategory = '';
  var EMOJI = { 'Fruits':'🍎','Vegetables':'🍅','Dairy & Eggs':'🥛','Snacks':'🍟','Pulses & Grains':'🍚','Cooking Essentials':'🧂','Household':'🧴','Beverages':'🧃' };

  async function loadCategories(){
    try{
      var cats = await KA.api('/api/marketplace/categories');
      var el = document.getElementById('cats');
      var html = '<a class="'+(activeCategory===''?'active':'')+'" onclick="setCat(\'\')">All</a>';
      (cats||[]).forEach(function(c){
        html += '<a class="'+(activeCategory===c.name?'active':'')+'" onclick="setCat(\''+c.name.replace(/'/g,"\\'")+'\')">'+c.name+' ('+c.count+')</a>';
      });
      el.innerHTML = html;
    }catch(e){}
  }
  function setCat(name){ activeCategory=name; loadCategories(); load(); }

  async function load(){
    try{
      var q = document.getElementById('q').value.trim();
      var sort = document.getElementById('sort').value;
      var path = '/api/marketplace/products?sort='+encodeURIComponent(sort);
      if(q) path += '&q='+encodeURIComponent(q);
      if(activeCategory) path += '&category='+encodeURIComponent(activeCategory);

      var products = await KA.api(path);
      document.getElementById('heading').textContent = activeCategory || (q ? 'Results for "'+q+'"' : 'Fresh Picks');
      var grid = document.getElementById('grid'); grid.innerHTML=''; document.getElementById('out').innerHTML='';
      var n=0;
      if(!products || !products.length){ document.getElementById('out').innerHTML='<div class="alert">No products match your search.</div>'; document.getElementById('count').textContent=''; return; }
      products.forEach(function(p){
        var emo = EMOJI[p.category] || '🧺';
        p.variants.forEach(function(v){ n++;
          var card = document.createElement('div'); card.className='pcard';
          var off = v.discountPercent>0 ? '<span class="disc">'+Math.round(v.discountPercent)+'% OFF</span>' : '';
          card.innerHTML = off +
            '<div class="thumb">'+emo+'</div>'+
            '<div class="store">'+p.storeName+'</div>'+
            '<div class="name">'+p.name+'</div>'+
            '<div class="wt">'+v.name+'</div>'+
            '<div class="pr"><span class="price">'+KA.money(v.sellingPrice)+'</span>'+
              (v.mrp>v.sellingPrice?'<span class="mrp">'+KA.money(v.mrp)+'</span>':'')+'</div>';
          var btn = document.createElement('button'); btn.className='btn-add';
          btn.innerHTML = '<svg class="ic" viewBox="0 0 24 24"><circle cx="9" cy="21" r="1.6"/><circle cx="18" cy="21" r="1.6"/><path d="M2.5 3h2l2.2 12.4a2 2 0 0 0 2 1.6h8.3a2 2 0 0 0 2-1.6L22 8H6"/></svg> Add to Cart';
          if(v.stockQuantity<=0){ btn.disabled=true; btn.textContent='Out of stock'; }
          btn.onclick = function(){
            KA.cart.add({ variantId:v.id, storeId:p.storeId, storeName:p.storeName, productName:p.name, variantName:v.name, mrp:v.mrp, price:v.sellingPrice, qty:1 });
            KA.toast('Added to cart','ok');
          };
          card.appendChild(btn); grid.appendChild(card);
        });
      });
      document.getElementById('count').textContent = n+' products';
    }catch(e){ document.getElementById('out').innerHTML='<div class="alert alert-err">'+e+'</div>'; }
  }
  loadCategories(); load();
  </script>
</body>
</html>
