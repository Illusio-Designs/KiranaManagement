<%@ Page Language="C#" %>
<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="utf-8" /><meta name="viewport" content="width=device-width, initial-scale=1" />
  <title>Admin · KiranaManagement</title>
  <link rel="stylesheet" href="assets/app.css" />
</head>
<body>
  <script src="assets/app.js"></script>
  <script>
  if(!KA.requireRole(['SuperAdmin'])) { }
  else {
    var content = KA.shell({ kind:'admin', title:'Super Admin', active:'pending', links:[
      { key:'pending', label:'Approvals', icon:'🕓', href:'#pending' },
      { key:'all', label:'All stores', icon:'🏪', href:'#all' }
    ]});
    content.innerHTML =
      '<div class="tiles" id="tiles"></div>'+
      '<section class="view" id="v-pending"><div class="card"><div class="card-h"><h3>Pending stores</h3>'+
        '<button class="btn btn-sm btn-outline right" onclick="loadPending()">Refresh</button></div>'+
        '<div class="card-b"><div id="pending-list"></div></div></div></section>'+
      '<section class="view hidden" id="v-all"><div class="card"><div class="card-h"><h3>All stores</h3>'+
        '<button class="btn btn-sm btn-outline right" onclick="loadAll()">Refresh</button></div>'+
        '<div class="card-b"><div id="all-list"></div></div></div></section>';

    window.addEventListener('hashchange', sync);
    sync();
  }

  function sync(){
    var key = (location.hash||'#pending').substring(1);
    ['pending','all'].forEach(function(k){ document.getElementById('v-'+k).classList.toggle('hidden', k!==key); });
    Array.prototype.forEach.call(document.querySelectorAll('.ka-side nav a'), function(a){
      a.classList.toggle('active', a.getAttribute('href')==='#'+key); });
    if(key==='pending') loadPending(); else loadAll();
  }
  function badge(s){ return s==='Active'?'<span class="badge badge-green">Active</span>':
    s==='Pending'?'<span class="badge badge-amber">Pending</span>':
    s==='Rejected'?'<span class="badge badge-red">Rejected</span>':'<span class="badge badge-gray">'+s+'</span>'; }

  async function loadPending(){
    try{
      var list = await KA.api('/api/stores/pending');
      document.getElementById('tiles').innerHTML='<div class="tile"><div class="k">Pending approval</div><div class="v">'+(list?list.length:0)+'</div></div>';
      if(!list||!list.length){ document.getElementById('pending-list').innerHTML='<p class="muted">Nothing pending. 🎉</p>'; return; }
      var h='<table class="table table-hover"><thead><tr><th>Store</th><th>Owner</th><th>Email</th><th>City</th><th>GSTIN</th><th></th></tr></thead><tbody>';
      list.forEach(function(s){ h+='<tr><td><strong>'+s.name+'</strong></td><td>'+s.ownerName+'</td><td>'+s.email+'</td><td>'+(s.city||'')+'</td><td>'+(s.gstin||'')+
        '</td><td><button class="btn btn-sm btn-brand" onclick="approve(\''+s.id+'\')">Approve</button> '+
        '<button class="btn btn-sm btn-danger" onclick="reject(\''+s.id+'\')">Reject</button></td></tr>'; });
      document.getElementById('pending-list').innerHTML=h+'</tbody></table>';
    }catch(e){ document.getElementById('pending-list').innerHTML='<div class="alert alert-err">'+e+'</div>'; }
  }
  async function loadAll(){
    try{
      var list = await KA.api('/api/stores');
      if(!list||!list.length){ document.getElementById('all-list').innerHTML='<p class="muted">No stores yet.</p>'; return; }
      var h='<table class="table table-hover"><thead><tr><th>Store</th><th>Owner</th><th>Email</th><th>Status</th></tr></thead><tbody>';
      list.forEach(function(s){ h+='<tr><td><strong>'+s.name+'</strong></td><td>'+s.ownerName+'</td><td>'+s.email+'</td><td>'+badge(s.status)+'</td></tr>'; });
      document.getElementById('all-list').innerHTML=h+'</tbody></table>';
    }catch(e){ document.getElementById('all-list').innerHTML='<div class="alert alert-err">'+e+'</div>'; }
  }
  async function approve(id){ try{ await KA.api('/api/stores/'+id+'/approve','POST'); KA.toast('Store approved','ok'); loadPending(); }catch(e){ KA.toast(e,'err'); } }
  async function reject(id){ var r=prompt('Reason for rejection?'); if(!r) return;
    try{ await KA.api('/api/stores/'+id+'/reject','POST',{ reason:r }); KA.toast('Store rejected'); loadPending(); }catch(e){ KA.toast(e,'err'); } }
  </script>
</body>
</html>
