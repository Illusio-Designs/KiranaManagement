<%@ Page Language="C#" %>
<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="utf-8" /><meta name="viewport" content="width=device-width, initial-scale=1" />
  <title>My Store · KiranaManagement</title>
  <link rel="stylesheet" href="assets/app.css" />
</head>
<body>
  <script src="assets/app.js"></script>
  <script>
  if(KA.requireRole(['Owner','Manager'])){
    var content = KA.shell({ kind:'store', title:'My Store', active:'orders', links:[
      { key:'orders', label:'Orders', icon:'🛍️', href:'#orders' },
      { key:'products', label:'Products', icon:'📦', href:'#products' },
      { key:'inventory', label:'Inventory', icon:'📊', href:'#inventory' },
      { key:'pos', label:'POS / Billing', icon:'🧾', href:'#pos' },
      { key:'purchases', label:'Purchases', icon:'🚚', href:'#purchases' }
    ]});
    content.innerHTML = TEMPLATE();
    window.addEventListener('hashchange', sync);
    if(!location.hash) location.hash='#orders';
    addVariantRow(); loadProducts(); refreshVariants(); sync();
  }

  function TEMPLATE(){ return ''+
    // ORDERS (marketplace orders — consumer identity hidden)
    '<section class="view" id="v-orders"><div class="card"><div class="card-h"><h3>Marketplace orders</h3>'+
      '<span class="badge badge-gray">Customer details are private</span>'+
      '<button class="btn btn-sm btn-outline right" onclick="loadOrders()">Refresh</button></div>'+
      '<div class="card-b"><p class="muted" style="margin-top:0">You see the items to fulfil and the delivery area &amp; ETA — never the shopper&#39;s name, phone or full address.</p>'+
      '<div id="orders-list"></div></div></div></section>'+
    // PRODUCTS
    '<section class="view hidden" id="v-products"><div class="card"><div class="card-h"><h3>Add a product</h3></div><div class="card-b">'+
      '<div class="row"><div class="field" style="flex:1;min-width:200px"><label class="lbl">Name</label><input id="p-name" class="input"></div>'+
      '<div class="field" style="flex:1;min-width:200px"><label class="lbl">Brand</label><input id="p-brand" class="input"></div></div>'+
      '<div class="field"><label class="lbl">Description</label><input id="p-desc" class="input"></div>'+
      '<div class="lbl">Variants</div><div id="variants"></div>'+
      '<button class="btn btn-sm btn-outline" onclick="addVariantRow()">+ Add variant</button>'+
      '<div style="margin-top:12px"><button class="btn btn-brand" onclick="createProduct()">Save product</button></div>'+
      '<div id="p-out" style="margin-top:8px"></div></div></div>'+
      '<div class="card" style="margin-top:14px"><div class="card-h"><h3>My products</h3><button class="btn btn-sm btn-outline right" onclick="loadProducts()">Refresh</button></div>'+
      '<div class="card-b"><div id="product-list"></div></div></div></section>'+
    // INVENTORY
    '<section class="view hidden" id="v-inventory"><div class="card"><div class="card-h"><h3>Adjust stock</h3></div><div class="card-b">'+
      '<div class="row"><div class="field" style="flex:2;min-width:220px"><label class="lbl">Variant</label><select id="inv-variant" class="input"></select></div>'+
      '<div class="field" style="flex:1;min-width:120px"><label class="lbl">Change (+/-)</label><input id="inv-change" type="number" class="input"></div>'+
      '<div class="field" style="flex:1;min-width:140px"><label class="lbl">Reason</label><input id="inv-reason" class="input"></div>'+
      '<div class="field" style="align-self:flex-end"><button class="btn btn-brand" onclick="adjustStock()">Apply</button></div></div>'+
      '<div id="inv-out"></div></div></div>'+
      '<div class="card" style="margin-top:14px"><div class="card-h"><h3>Low stock</h3><button class="btn btn-sm btn-outline right" onclick="loadLowStock()">Refresh</button></div>'+
      '<div class="card-b"><div id="low-list"></div></div></div></section>'+
    // POS
    '<section class="view hidden" id="v-pos"><div class="card"><div class="card-h"><h3>New sale</h3></div><div class="card-b">'+
      '<div id="pos-lines"></div><button class="btn btn-sm btn-outline" onclick="posAddLine()">+ Add item</button>'+
      '<div class="row" style="margin-top:10px"><div class="field" style="flex:1;min-width:180px"><label class="lbl">Customer (optional)</label><input id="pos-customer" class="input"></div>'+
      '<div class="field" style="flex:1;min-width:140px"><label class="lbl">Payment</label><select id="pos-pay" class="input"><option value="0">Cash</option><option value="1">UPI</option><option value="2">Card</option><option value="3">Wallet</option></select></div>'+
      '<div class="field" style="flex:1;min-width:140px"><label class="lbl">Amount paid</label><input id="pos-paid" type="number" class="input"></div></div>'+
      '<button class="btn btn-brand" onclick="completeSale()">Complete sale</button><div id="pos-out" style="margin-top:8px"></div></div></div>'+
      '<div class="card" style="margin-top:14px"><div class="card-h"><h3>Recent sales</h3><button class="btn btn-sm btn-outline right" onclick="loadSales()">Refresh</button></div>'+
      '<div class="card-b"><div id="sales-list"></div></div></div></section>'+
    // PURCHASES
    '<section class="view hidden" id="v-purchases"><div class="card"><div class="card-h"><h3>Add supplier</h3></div><div class="card-b">'+
      '<div class="row"><div class="field" style="flex:1;min-width:150px"><label class="lbl">Name</label><input id="sup-name" class="input"></div>'+
      '<div class="field" style="flex:1;min-width:130px"><label class="lbl">Phone</label><input id="sup-phone" class="input"></div>'+
      '<div class="field" style="flex:1;min-width:160px"><label class="lbl">Email</label><input id="sup-email" class="input"></div>'+
      '<div class="field" style="flex:1;min-width:130px"><label class="lbl">GSTIN</label><input id="sup-gstin" class="input"></div>'+
      '<div class="field" style="align-self:flex-end"><button class="btn btn-brand" onclick="createSupplier()">Add</button></div></div>'+
      '<div id="sup-out"></div></div></div>'+
      '<div class="card" style="margin-top:14px"><div class="card-h"><h3>New purchase order</h3></div><div class="card-b">'+
      '<div class="field" style="max-width:340px"><label class="lbl">Supplier</label><select id="po-supplier" class="input"></select></div>'+
      '<div id="po-lines"></div><button class="btn btn-sm btn-outline" onclick="poAddLine()">+ Add item</button>'+
      '<div style="margin-top:12px"><button class="btn btn-brand" onclick="createPO()">Create PO</button></div><div id="po-out" style="margin-top:8px"></div></div></div>'+
      '<div class="card" style="margin-top:14px"><div class="card-h"><h3>Purchase orders</h3><button class="btn btn-sm btn-outline right" onclick="loadPOs()">Refresh</button></div>'+
      '<div class="card-b"><div id="po-list"></div></div></div></section>'; }

  function sync(){
    var key=(location.hash||'#orders').substring(1);
    ['orders','products','inventory','pos','purchases'].forEach(function(k){ document.getElementById('v-'+k).classList.toggle('hidden', k!==key); });
    Array.prototype.forEach.call(document.querySelectorAll('.ka-side nav a'), function(a){ a.classList.toggle('active', a.getAttribute('href')==='#'+key); });
    if(key==='orders') loadOrders();
    if(key==='inventory') loadLowStock();
    if(key==='pos') posInit();
    if(key==='purchases') purInit();
  }

  // ORDERS — masked marketplace orders for this store.
  async function loadOrders(){ try{ var list=await KA.api('/api/store/orders');
    if(!list||!list.length){ document.getElementById('orders-list').innerHTML='<p class="muted">No marketplace orders yet.</p>'; return; }
    var STAGES=['Placed','Accepted','Packed','OutForDelivery','Delivered','Cancelled'];
    var h='';
    list.forEach(function(o){
      var items=o.lines.map(function(l){ return l.productName+' · '+l.variantName+' × '+l.quantity; }).join(', ');
      var eta=o.etaMinutes?(o.etaMinutes+' min'):'—'; var dist=(o.distanceKm!=null)?(o.distanceKm+' km'):'—';
      var opts=STAGES.map(function(s){ return '<option value="'+s+'"'+(s===o.status?' selected':'')+'>'+s+'</option>'; }).join('');
      h+='<div class="card" style="margin-bottom:10px"><div class="card-b">'+
        '<div class="row"><strong>'+o.orderNumber+'</strong>'+
        '<span class="badge badge-brand">'+o.status+'</span>'+
        '<span class="right muted" style="font-size:12.5px">'+new Date(o.createdAt).toLocaleString()+'</span></div>'+
        '<div class="row" style="margin:8px 0"><span class="badge badge-gray">📍 '+o.deliveryArea+'</span>'+
        '<span class="badge badge-amber">ETA '+eta+'</span><span class="badge badge-gray">'+dist+'</span>'+
        '<span class="badge badge-green">'+o.itemCount+' items · '+KA.money(o.storeTotal)+'</span></div>'+
        '<div class="muted" style="font-size:13px">'+items+'</div>'+
        '<div class="row" style="margin-top:10px"><select class="input" style="max-width:200px" onchange="setOrderStatus(\''+o.id+'\',this.value)">'+opts+'</select></div>'+
        '</div></div>';
    });
    document.getElementById('orders-list').innerHTML=h;
  }catch(e){ document.getElementById('orders-list').innerHTML='<div class="alert alert-err">'+e+'</div>'; } }
  async function setOrderStatus(id,status){ try{ await KA.api('/api/store/orders/'+id+'/status','PUT',{ status:status }); KA.toast('Order marked '+status,'ok'); }catch(e){ KA.toast(e,'err'); } }

  var VARIANTS=[];
  async function refreshVariants(){ var products=await KA.api('/api/products'); VARIANTS=[];
    (products||[]).forEach(function(p){ (p.variants||[]).forEach(function(x){ VARIANTS.push({id:x.id,label:p.name+' — '+x.name+' (stk '+x.stockQuantity+')'}); }); }); }
  function variantOptions(){ return VARIANTS.map(function(x){ return '<option value="'+x.id+'">'+x.label+'</option>'; }).join(''); }

  // PRODUCTS
  function addVariantRow(){ var d=document.createElement('div'); d.className='row variant-row'; d.style.marginBottom='6px';
    d.innerHTML='<input class="input v-name" placeholder="Name e.g. 1 kg"><input class="input v-sku" placeholder="SKU">'+
      '<input class="input v-mrp" type="number" placeholder="MRP"><input class="input v-sell" type="number" placeholder="Selling">'+
      '<input class="input v-tax" type="number" placeholder="Tax %"><input class="input v-stock" type="number" placeholder="Stock">';
    document.getElementById('variants').appendChild(d); }
  async function createProduct(){ try{
    var rows=Array.prototype.map.call(document.querySelectorAll('.variant-row'),function(r){ return {
      name:r.querySelector('.v-name').value, sku:r.querySelector('.v-sku').value, unit:0, packSize:1,
      mrp:parseFloat(r.querySelector('.v-mrp').value)||0, sellingPrice:parseFloat(r.querySelector('.v-sell').value)||0,
      taxRatePercent:parseFloat(r.querySelector('.v-tax').value)||0, stockQuantity:parseInt(r.querySelector('.v-stock').value)||0, reorderLevel:0 }; });
    if(!rows.length){ document.getElementById('p-out').innerHTML='<div class="alert alert-err">Add a variant.</div>'; return; }
    var p=await KA.api('/api/products','POST',{ name:val('p-name'), brand:val('p-brand'), description:val('p-desc'), variants:rows });
    document.getElementById('p-out').innerHTML='<div class="alert alert-ok">Saved '+p.name+'.</div>'; loadProducts(); refreshVariants();
  }catch(e){ document.getElementById('p-out').innerHTML='<div class="alert alert-err">'+e+'</div>'; } }
  async function loadProducts(){ try{ var list=await KA.api('/api/products');
    if(!list||!list.length){ document.getElementById('product-list').innerHTML='<p class="muted">No products yet.</p>'; return; }
    var h=''; list.forEach(function(p){ h+='<div class="card" style="margin-bottom:10px"><div class="card-b"><strong>'+p.name+'</strong> '+(p.brand?('· '+p.brand):'')+
      '<table class="table" style="margin-top:6px"><thead><tr><th>Variant</th><th>MRP</th><th>Selling</th><th>Discount</th><th>Stock</th></tr></thead><tbody>';
      p.variants.forEach(function(x){ h+='<tr><td>'+x.name+'</td><td>'+KA.money(x.mrp)+'</td><td>'+KA.money(x.sellingPrice)+'</td><td>'+KA.money(x.discountAmount)+' ('+x.discountPercent+'%)</td><td>'+x.stockQuantity+'</td></tr>'; });
      h+='</tbody></table></div></div>'; });
    document.getElementById('product-list').innerHTML=h;
  }catch(e){ document.getElementById('product-list').innerHTML='<div class="alert alert-err">'+e+'</div>'; } }

  // INVENTORY
  async function invFill(){ await refreshVariants(); document.getElementById('inv-variant').innerHTML=variantOptions(); }
  async function adjustStock(){ try{ await KA.api('/api/products/variants/'+document.getElementById('inv-variant').value+'/adjust','POST',
    { changeQuantity:parseInt(val('inv-change'))||0, reason:val('inv-reason') });
    KA.toast('Stock updated','ok'); loadLowStock();
  }catch(e){ document.getElementById('inv-out').innerHTML='<div class="alert alert-err">'+e+'</div>'; } }
  async function loadLowStock(){ try{ await invFill(); var list=await KA.api('/api/products/low-stock');
    if(!list||!list.length){ document.getElementById('low-list').innerHTML='<p class="muted">Nothing low on stock. 👍</p>'; return; }
    var h='<table class="table"><thead><tr><th>Product</th><th>Variant</th><th>Stock</th><th>Reorder at</th></tr></thead><tbody>';
    list.forEach(function(x){ h+='<tr><td>'+x.productName+'</td><td>'+x.variantName+'</td><td><span class="badge badge-red">'+x.stockQuantity+'</span></td><td>'+x.reorderLevel+'</td></tr>'; });
    document.getElementById('low-list').innerHTML=h+'</tbody></table>';
  }catch(e){ document.getElementById('low-list').innerHTML='<div class="alert alert-err">'+e+'</div>'; } }

  // POS
  async function posInit(){ await refreshVariants(); if(!document.querySelectorAll('.pos-line').length) posAddLine(); }
  function posAddLine(){ var d=document.createElement('div'); d.className='row pos-line'; d.style.marginBottom='6px';
    d.innerHTML='<select class="input pos-v" style="flex:3">'+variantOptions()+'</select><input type="number" class="input pos-q" style="flex:1" placeholder="Qty" value="1">'+
      '<button class="btn btn-danger btn-sm" onclick="this.closest(\'.pos-line\').remove()">×</button>';
    document.getElementById('pos-lines').appendChild(d); }
  async function completeSale(){ try{
    var lines=Array.prototype.map.call(document.querySelectorAll('.pos-line'),function(r){ return { productVariantId:r.querySelector('.pos-v').value, quantity:parseInt(r.querySelector('.pos-q').value)||0 }; }).filter(function(l){return l.quantity>0;});
    if(!lines.length){ document.getElementById('pos-out').innerHTML='<div class="alert alert-err">Add an item.</div>'; return; }
    var body={ customerName:val('pos-customer'), paymentMode:parseInt(document.getElementById('pos-pay').value), lines:lines };
    var paid=val('pos-paid'); if(paid!=='') body.amountPaid=parseFloat(paid);
    var s=await KA.api('/api/sales','POST',body);
    document.getElementById('pos-out').innerHTML='<div class="alert alert-ok">Invoice '+s.invoiceNumber+' — Total '+KA.money(s.grandTotal)+' (tax '+KA.money(s.taxTotal)+', discount '+KA.money(s.discountTotal)+', change '+KA.money(s.changeDue)+')</div>';
    document.getElementById('pos-lines').innerHTML=''; posAddLine(); loadSales(); refreshVariants();
  }catch(e){ document.getElementById('pos-out').innerHTML='<div class="alert alert-err">'+e+'</div>'; } }
  async function loadSales(){ try{ var list=await KA.api('/api/sales');
    if(!list||!list.length){ document.getElementById('sales-list').innerHTML='<p class="muted">No sales yet.</p>'; return; }
    var h='<table class="table"><thead><tr><th>Invoice</th><th>Items</th><th>Total</th><th>Pay</th><th>When</th></tr></thead><tbody>';
    list.forEach(function(s){ h+='<tr><td>'+s.invoiceNumber+'</td><td>'+s.lines.length+'</td><td>'+KA.money(s.grandTotal)+'</td><td>'+s.paymentMode+'</td><td>'+new Date(s.createdAt).toLocaleString()+'</td></tr>'; });
    document.getElementById('sales-list').innerHTML=h+'</tbody></table>';
  }catch(e){ document.getElementById('sales-list').innerHTML='<div class="alert alert-err">'+e+'</div>'; } }

  // PURCHASES
  async function purInit(){ await refreshVariants(); await loadSuppliers(); if(!document.querySelectorAll('.po-line').length) poAddLine(); loadPOs(); }
  async function createSupplier(){ try{ await KA.api('/api/purchases/suppliers','POST',{ name:val('sup-name'), phone:val('sup-phone'), email:val('sup-email'), gstin:val('sup-gstin') });
    KA.toast('Supplier added','ok'); loadSuppliers();
  }catch(e){ document.getElementById('sup-out').innerHTML='<div class="alert alert-err">'+e+'</div>'; } }
  async function loadSuppliers(){ var list=await KA.api('/api/purchases/suppliers')||[];
    document.getElementById('po-supplier').innerHTML=list.map(function(s){ return '<option value="'+s.id+'">'+s.name+'</option>'; }).join(''); }
  function poAddLine(){ var d=document.createElement('div'); d.className='row po-line'; d.style.marginBottom='6px';
    d.innerHTML='<select class="input po-v" style="flex:3">'+variantOptions()+'</select><input type="number" class="input po-q" style="flex:1" placeholder="Qty" value="1">'+
      '<input type="number" class="input po-c" style="flex:1" placeholder="Unit cost"><button class="btn btn-danger btn-sm" onclick="this.closest(\'.po-line\').remove()">×</button>';
    document.getElementById('po-lines').appendChild(d); }
  async function createPO(){ try{
    var lines=Array.prototype.map.call(document.querySelectorAll('.po-line'),function(r){ return { productVariantId:r.querySelector('.po-v').value, quantity:parseInt(r.querySelector('.po-q').value)||0, unitCost:parseFloat(r.querySelector('.po-c').value)||0 }; }).filter(function(l){return l.quantity>0;});
    if(!lines.length){ document.getElementById('po-out').innerHTML='<div class="alert alert-err">Add an item.</div>'; return; }
    var po=await KA.api('/api/purchases/orders','POST',{ supplierId:document.getElementById('po-supplier').value, lines:lines });
    document.getElementById('po-out').innerHTML='<div class="alert alert-ok">Created '+po.poNumber+' (total '+KA.money(po.totalCost)+'). Click Receive to add stock.</div>';
    document.getElementById('po-lines').innerHTML=''; poAddLine(); loadPOs();
  }catch(e){ document.getElementById('po-out').innerHTML='<div class="alert alert-err">'+e+'</div>'; } }
  async function loadPOs(){ try{ var list=await KA.api('/api/purchases/orders');
    if(!list||!list.length){ document.getElementById('po-list').innerHTML='<p class="muted">No purchase orders.</p>'; return; }
    var h='<table class="table"><thead><tr><th>PO</th><th>Items</th><th>Total</th><th>Status</th><th></th></tr></thead><tbody>';
    list.forEach(function(p){ h+='<tr><td>'+p.poNumber+'</td><td>'+p.lines.length+'</td><td>'+KA.money(p.totalCost)+'</td><td>'+
      (p.status==='Received'?'<span class="badge badge-green">Received</span>':'<span class="badge badge-amber">Draft</span>')+'</td><td>'+
      (p.status==='Draft'?'<button class="btn btn-sm btn-brand" onclick="receivePO(\''+p.id+'\')">Receive</button>':'✓')+'</td></tr>'; });
    document.getElementById('po-list').innerHTML=h+'</tbody></table>';
  }catch(e){ document.getElementById('po-list').innerHTML='<div class="alert alert-err">'+e+'</div>'; } }
  async function receivePO(id){ try{ await KA.api('/api/purchases/orders/'+id+'/receive','POST'); KA.toast('Stock received','ok'); loadPOs(); refreshVariants(); }catch(e){ KA.toast(e,'err'); } }

  function val(id){ var el=document.getElementById(id); return el?el.value.trim():''; }
  </script>
</body>
</html>
