<%@ Page Language="C#" %>
<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="utf-8" /><meta name="viewport" content="width=device-width, initial-scale=1" />
  <title>Login · KiranaManagement</title>
  <link rel="stylesheet" href="assets/app.css" />
</head>
<body>
  <div class="auth-wrap">
    <div class="auth-card">
      <div style="text-align:center;margin-bottom:14px">
        <div class="brand" style="justify-content:center;font-size:24px">🛒 Kirana</div>
        <div class="muted">Sign in to your account</div>
      </div>
      <div class="card"><div class="card-b">
        <div class="field"><label class="lbl">Email</label><input id="email" class="input" placeholder="you@example.com"></div>
        <div class="field"><label class="lbl">Password</label><input id="pass" type="password" class="input" placeholder="••••••••"></div>
        <button class="btn btn-brand btn-block" onclick="login()">Login</button>
        <div id="out" style="margin-top:8px"></div>
        <hr>
        <div style="text-align:center" class="muted">New store? <a href="register.aspx">Register here</a> · <a href="shop.aspx">Shop as customer</a></div>
        <div style="text-align:center;margin-top:8px;font-size:12px" class="muted">
          Admin: superadmin@kirana.local / Admin@12345<br>Store: demo@store.local / Demo@12345</div>
      </div></div>
    </div>
  </div>

  <script src="assets/app.js"></script>
  <script>
  async function login(){
    var out=document.getElementById('out');
    try{
      var d = await KA.api('/api/auth/login','POST',{ email:document.getElementById('email').value.trim(), password:document.getElementById('pass').value });
      KA.setSession(d);
      if(d.role==='SuperAdmin') location.href='admin.aspx';
      else if(d.role==='Customer') location.href='shop.aspx';
      else location.href='store.aspx';
    }catch(e){ out.innerHTML='<div class="alert alert-err">'+e+'</div>'; }
  }
  document.getElementById('pass').addEventListener('keydown',function(e){ if(e.key==='Enter') login(); });
  </script>
</body>
</html>
