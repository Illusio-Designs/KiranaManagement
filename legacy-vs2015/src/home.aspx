<%@ Page Language="C#" %>
<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="utf-8" /><meta name="viewport" content="width=device-width, initial-scale=1" />
  <title>Kirana — Fresh groceries, delivered</title>
  <link rel="stylesheet" href="assets/app.css" />
</head>
<body>
  <header class="appbar"><div class="container">
    <a class="logo" href="home.aspx"><span class="lm"><svg class="ic" viewBox="0 0 24 24"><path d="M11 20A7 7 0 0 1 4 13C4 8 8 4 20 4c0 12-4 16-9 16z"/><path d="M9 17c1-4 4-7 8-9"/></svg></span>Kirana</a>
    <nav class="navlinks"><a class="active" href="home.aspx">Home</a><a href="shop.aspx">Categories</a><a href="shop.aspx">Deals</a><a href="register.aspx">Sell on Kirana</a></nav>
    <div class="search"><span><svg class="ic" viewBox="0 0 24 24"><circle cx="11" cy="11" r="7"/><path d="M21 21l-4.3-4.3"/></svg></span><input placeholder="Search for products..." onkeyup="if(event.key==='Enter')location.href='shop.aspx?q='+encodeURIComponent(this.value)"></div>
    <div class="acts"><span data-acct=""><a class="icbtn" href="login.aspx"><svg class="ic" viewBox="0 0 24 24"><circle cx="12" cy="8" r="4"/><path d="M4 21c0-4 4-6 8-6s8 2 8 6"/></svg></a></span>
      <a class="cartpill" href="cart.aspx"><svg class="ic" viewBox="0 0 24 24"><circle cx="9" cy="21" r="1.6"/><circle cx="18" cy="21" r="1.6"/><path d="M2.5 3h2l2.2 12.4a2 2 0 0 0 2 1.6h8.3a2 2 0 0 0 2-1.6L22 8H6"/></svg> <span data-cart-count="">0</span></a></div>
  </div></header>

  <section class="hero"><div class="container">
    <div>
      <span class="eyebrow"><svg class="ic" viewBox="0 0 24 24"><path d="M11 20A7 7 0 0 1 4 13C4 8 8 4 20 4c0 12-4 16-9 16z"/></svg> Fresh Groceries, Delivered</span>
      <h1>Fresh Groceries,<br><span class="g">Delivered to You</span></h1>
      <p>Get the best quality products at unbeatable prices. Fast delivery from your neighbourhood stores to your doorstep.</p>
      <div class="cta"><a href="shop.aspx" class="btn btn-brand btn-lg">Shop Now <svg class="ic" viewBox="0 0 24 24"><path d="M5 12h14"/><path d="M13 6l6 6-6 6"/></svg></a>
        <a href="shop.aspx" class="btn btn-outline btn-lg">Explore Deals</a></div>
      <div class="trust">
        <div class="t"><span class="iconcircle sm"><svg class="ic" viewBox="0 0 24 24"><path d="M3 6h11v9H3z"/><path d="M14 9h4l3 3v3h-7z"/><circle cx="7" cy="18" r="1.7"/><circle cx="17" cy="18" r="1.7"/></svg></span><div><b>Free Delivery</b><span>On orders above &#8377;199</span></div></div>
        <div class="t"><span class="iconcircle sm mint"><svg class="ic" viewBox="0 0 24 24"><path d="M20 12l-8 8-8-8V4h8z"/><circle cx="8.5" cy="8.5" r="1.3"/></svg></span><div><b>Best Prices</b><span>Affordable every day</span></div></div>
        <div class="t"><span class="iconcircle sm amber"><svg class="ic" viewBox="0 0 24 24"><path d="M12 3l7 3v6c0 4-3 7-7 9-4-2-7-5-7-9V6z"/><path d="M9 12l2 2 4-4"/></svg></span><div><b>Fresh Quality</b><span>100% guaranteed</span></div></div>
      </div>
    </div>
    <div class="hero-art"><div class="wave"></div><div class="bag"><svg class="ic" style="width:170px;height:170px;color:var(--green-2)" viewBox="0 0 24 24"><path d="M6 8h12l-1 12H7z"/><path d="M9 8V6a3 3 0 0 1 6 0v2"/></svg></div>
      <div class="offbubble"><span>UP TO</span><b>50%</b><em>OFF</em></div></div>
  </div></section>

  <div class="container">
    <div class="center-h"><h2>Shop by Category</h2></div>
    <div class="cats" id="cats"></div>
    <div class="section-h" style="margin-top:34px"><h2>Best Deals for You</h2><a class="link" href="shop.aspx">View All Deals <svg class="ic" viewBox="0 0 24 24"><path d="M5 12h14"/><path d="M13 6l6 6-6 6"/></svg></a></div>
    <div class="products" id="deals"></div>

    <div class="featstrip">
      <div class="feat"><span class="iconcircle sm"><svg class="ic" viewBox="0 0 24 24"><path d="M4 4v6h6"/><path d="M20 20v-6h-6"/><path d="M20 9a8 8 0 0 0-14-3L4 8"/><path d="M4 15a8 8 0 0 0 14 3l2-2"/></svg></span><div><b>Easy Returns</b><span>Hassle-free returns</span></div></div>
      <div class="feat"><span class="iconcircle sm sky"><svg class="ic" viewBox="0 0 24 24"><path d="M4 13v-1a8 8 0 0 1 16 0v1"/><rect x="3" y="13" width="4" height="6" rx="1.5"/><rect x="17" y="13" width="4" height="6" rx="1.5"/></svg></span><div><b>24/7 Support</b><span>We're here to help</span></div></div>
      <div class="feat"><span class="iconcircle sm pink"><svg class="ic" viewBox="0 0 24 24"><rect x="3" y="9" width="18" height="12" rx="1.5"/><path d="M3 13h18M12 9v12"/></svg></span><div><b>Member Benefits</b><span>Exclusive offers</span></div></div>
      <div class="feat"><span class="iconcircle sm amber"><svg class="ic" viewBox="0 0 24 24"><path d="M12 3l7 3v6c0 4-3 7-7 9-4-2-7-5-7-9V6z"/><path d="M9 12l2 2 4-4"/></svg></span><div><b>Secure Payments</b><span>100% protected</span></div></div>
    </div>
  </div>

  <footer class="site-foot"><div class="container">
    <div class="cols">
      <div><div class="brandline"><span class="iconcircle sm solid"><svg class="ic" viewBox="0 0 24 24"><path d="M11 20A7 7 0 0 1 4 13C4 8 8 4 20 4c0 12-4 16-9 16z"/></svg></span>Kirana</div>
        <p class="about">Fresh groceries from your neighbourhood stores, delivered to your door. Quality you can trust, prices you'll love.</p></div>
      <div><h4>Shop</h4><a href="shop.aspx">Vegetables</a><a href="shop.aspx">Fruits</a><a href="shop.aspx">Dairy &amp; Eggs</a><a href="shop.aspx">Snacks</a></div>
      <div><h4>Company</h4><a href="register.aspx">Sell on Kirana</a><a href="#">About Us</a><a href="#">Contact</a></div>
      <div><h4>Help</h4><a href="orders.aspx">Track Order</a><a href="#">Returns</a><a href="#">FAQs</a></div>
    </div>
    <div class="copy"><span>© 2026 KiranaManagement — Multi-store grocery marketplace.</span><span>Made with care · Mansi, Hiral &amp; Zigma</span></div>
  </div></footer>

  <script src="assets/app.js"></script>
  <script>
  var CAT_ICONS = {
    'Vegetables':'<path d="M11 20A7 7 0 0 1 4 13C4 8 8 4 20 4c0 12-4 16-9 16z"/><path d="M9 17c1-4 4-7 8-9"/>',
    'Fruits':'<rect x="3" y="9" width="18" height="12" rx="1.5"/><path d="M3 13h18M12 9v12"/>',
    'Dairy & Eggs':'<path d="M12 3l8 4.5v9L12 21l-8-4.5v-9z"/><path d="M4 7.5l8 4.5 8-4.5M12 12v9"/>',
    'Snacks':'<path d="M6 8h12l-1 12H7z"/><path d="M9 8V6a3 3 0 0 1 6 0v2"/>',
    'Pulses & Grains':'<rect x="3" y="3" width="7" height="7" rx="1.5"/><rect x="14" y="3" width="7" height="7" rx="1.5"/><rect x="3" y="14" width="7" height="7" rx="1.5"/><rect x="14" y="14" width="7" height="7" rx="1.5"/>',
    'Cooking Essentials':'<path d="M4 9l1-5h14l1 5"/><path d="M5 9v11h14V9"/>'
  };
  var COLORS=['','pink','sky','amber','mint','purple'];
  var EMOJI={'Fruits':'🍎','Vegetables':'🍅','Dairy & Eggs':'🥛','Snacks':'🍟','Pulses & Grains':'🍚','Cooking Essentials':'🧂'};

  async function loadCats(){
    try{ var cats=await KA.api('/api/marketplace/categories'); var el=document.getElementById('cats'); var h='';
      (cats||[]).slice(0,6).forEach(function(c,i){
        var path=CAT_ICONS[c.name]||'<circle cx="12" cy="12" r="8"/>';
        h+='<a class="cat" href="shop.aspx?category='+encodeURIComponent(c.name)+'"><span class="iconcircle lg '+COLORS[i%COLORS.length]+'"><svg class="ic" viewBox="0 0 24 24">'+path+'</svg></span><div class="lbl2">'+c.name+'</div><div class="cnt">'+c.count+' items</div></a>';
      });
      el.innerHTML=h;
    }catch(e){}
  }
  async function loadDeals(){
    try{ var products=await KA.api('/api/marketplace/products?sort=discount'); var el=document.getElementById('deals'); var h=''; var n=0;
      (products||[]).forEach(function(p){ if(n>=5) return; var v=p.variants[0]; if(!v) return; n++;
        var emo=EMOJI[p.category]||'🧺'; var off=v.discountPercent>0?('<span class="disc">'+Math.round(v.discountPercent)+'% OFF</span>'):'';
        h+='<div class="pcard">'+off+'<div class="thumb">'+emo+'</div><div class="name">'+p.name+'</div><div class="wt">'+v.name+'</div>'+
           '<div class="pr"><span class="price">'+KA.money(v.sellingPrice)+'</span>'+(v.mrp>v.sellingPrice?'<span class="mrp">'+KA.money(v.mrp)+'</span>':'')+'</div>'+
           '<button class="btn-add" onclick="addDeal(\''+v.id+'\',\''+p.storeId+'\',\''+p.storeName.replace(/\x27/g,"")+'\',\''+p.name.replace(/\x27/g,"")+'\',\''+v.name.replace(/\x27/g,"")+'\','+v.mrp+','+v.sellingPrice+')">Add to Cart</button></div>';
      });
      el.innerHTML=h||'<p class="muted">No deals yet.</p>';
    }catch(e){ document.getElementById('deals').innerHTML='<p class="muted">Sign in to browse the marketplace.</p>'; }
  }
  function addDeal(id,sid,sname,pname,vname,mrp,price){ KA.cart.add({variantId:id,storeId:sid,storeName:sname,productName:pname,variantName:vname,mrp:mrp,price:price,qty:1}); KA.toast('Added to cart','ok'); }
  loadCats(); loadDeals();
  </script>
</body>
</html>
