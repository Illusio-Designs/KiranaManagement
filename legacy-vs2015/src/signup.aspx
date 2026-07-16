<%@ Page Language="C#" %>
<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="utf-8" /><meta name="viewport" content="width=device-width, initial-scale=1" />
  <title>Sign up · KiranaManagement</title>
  <link rel="stylesheet" href="assets/app.css" />
</head>
<body>
  <div class="auth-wrap">
    <div class="auth-card">
      <div style="text-align:center;margin-bottom:14px">
        <div class="brand" style="justify-content:center;font-size:24px">🛒 Kirana</div>
        <div class="muted">Create your shopper account</div>
      </div>
      <div class="card"><div class="card-b">
        <div class="field"><label class="lbl">Full name</label><input id="name" class="input"></div>
        <div class="field"><label class="lbl">Email</label><input id="email" class="input"></div>
        <div class="field"><label class="lbl">Password</label><input id="pass" type="password" class="input"></div>
        <button class="btn btn-brand btn-block" onclick="signup()">Create account</button>
        <div id="out" style="margin-top:8px"></div>
        <hr>
        <div style="text-align:center" class="muted">Already have an account? <a href="login.aspx">Log in</a></div>
      </div></div>
    </div>
  </div>

  <script src="assets/app.js"></script>
  <script>
  function v(id){ return document.getElementById(id).value.trim(); }
  async function signup(){
    var out=document.getElementById('out');
    try{
      await KA.api('/api/auth/register-customer','POST',{ fullName:v('name'), email:v('email'), password:document.getElementById('pass').value });
      // auto-login
      var d = await KA.api('/api/auth/login','POST',{ email:v('email'), password:document.getElementById('pass').value });
      KA.setSession(d);
      location.href='shop.aspx';
    }catch(e){ out.innerHTML='<div class="alert alert-err">'+e+'</div>'; }
  }
  </script>
</body>
</html>
