<%@ Page Language="C#" %>
<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="utf-8" /><meta name="viewport" content="width=device-width, initial-scale=1" />
  <title>Register store · KiranaManagement</title>
  <link rel="stylesheet" href="assets/app.css" />
</head>
<body>
  <div class="topnav"><div class="container">
    <span class="brand">🛒 Kirana</span>
    <a class="navlink" href="shop.aspx">Shop</a>
    <a class="navlink right" href="login.aspx">Login</a>
  </div></div>

  <div class="container" style="max-width:720px;margin-top:24px">
    <h2>Register your store</h2>
    <p class="muted">After you register, a Super Admin reviews and approves it — then you can log in and start selling.</p>
    <div class="card"><div class="card-b">
      <div class="row">
        <div class="field" style="flex:1;min-width:220px"><label class="lbl">Store name *</label><input id="name" class="input"></div>
        <div class="field" style="flex:1;min-width:220px"><label class="lbl">Owner name *</label><input id="owner" class="input"></div>
      </div>
      <div class="row">
        <div class="field" style="flex:1;min-width:220px"><label class="lbl">Email *</label><input id="email" class="input"></div>
        <div class="field" style="flex:1;min-width:220px"><label class="lbl">Phone</label><input id="phone" class="input"></div>
      </div>
      <div class="row">
        <div class="field" style="flex:1;min-width:220px"><label class="lbl">Password *</label><input id="pass" type="password" class="input"></div>
        <div class="field" style="flex:1;min-width:220px"><label class="lbl">City</label><input id="city" class="input"></div>
      </div>
      <div class="row">
        <div class="field" style="flex:1;min-width:220px"><label class="lbl">GSTIN</label><input id="gstin" class="input"></div>
        <div class="field" style="flex:1;min-width:220px"><label class="lbl">PAN</label><input id="pan" class="input"></div>
      </div>
      <button class="btn btn-brand btn-block" onclick="register()">Register store</button>
      <div id="out" style="margin-top:8px"></div>
    </div></div>
  </div>

  <div class="foot"><div class="container">© 2026 KiranaManagement</div></div>

  <script src="assets/app.js"></script>
  <script>
  function v(id){ return document.getElementById(id).value.trim(); }
  async function register(){
    var out=document.getElementById('out');
    try{
      var s = await KA.api('/api/stores/register','POST',{ storeName:v('name'), ownerName:v('owner'), email:v('email'),
        phone:v('phone'), password:v('pass'), city:v('city'), gstin:v('gstin'), pan:v('pan') });
      out.innerHTML='<div class="alert alert-ok">Registered! Status: <strong>'+s.status+'</strong>. Please wait for admin approval, then <a href="login.aspx">log in</a>.</div>';
    }catch(e){ out.innerHTML='<div class="alert alert-err">'+e+'</div>'; }
  }
  </script>
</body>
</html>
