<%@ Page Title="Login" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="Kirana.WebApi.login" %>
<asp:Content ID="c" ContentPlaceHolderID="main" runat="server">
  <div class="auth-wrap">
    <div class="auth-card card"><div class="card-b">
      <h2 style="text-align:center"><asp:Literal ID="litTitle" runat="server" /></h2>
      <p class="muted" style="text-align:center;margin-top:-4px"><asp:Literal ID="litSub" runat="server" /></p>

      <%-- Store owners: Continue with Google --%>
      <% if (CurrentPortal == Kirana.WebApi.Data.Portal.Store) { %>
        <% if (!string.IsNullOrEmpty(GoogleClientId)) { %>
          <div id="g_id_onload" data-client_id="<%= GoogleClientId %>" data-callback="onGoogle" data-auto_prompt="false"></div>
          <div class="g_id_signin" data-type="standard" data-theme="outline" data-text="continue_with" data-size="large" data-width="360" style="display:flex;justify-content:center"></div>
          <script src="https://accounts.google.com/gsi/client" async defer></script>
        <% } else { %>
          <div class="alert" style="font-size:12.5px">Set <b>GoogleClientId</b> in web.config &lt;appSettings&gt; to enable "Continue with Google".</div>
        <% } %>
        <div class="divider">or sign in with email</div>
      <% } %>

      <div class="field"><label class="lbl">Email</label>
        <asp:TextBox ID="txtEmail" runat="server" CssClass="input" /></div>
      <div class="field"><label class="lbl">Password</label>
        <asp:TextBox ID="txtPass" runat="server" TextMode="Password" CssClass="input" /></div>
      <asp:Button ID="btnLogin" runat="server" Text="Sign In" CssClass="btn btn-brand btn-block btn-lg" OnClick="btnLogin_Click" />

      <%-- Consumers: OTP login --%>
      <% if (CurrentPortal == Kirana.WebApi.Data.Portal.Consumer) { %>
        <div class="divider">or sign in with OTP</div>
        <div id="otpStep1">
          <div class="field"><label class="lbl">Mobile number</label>
            <input id="otpPhone" class="input" placeholder="+91 98765 43210"></div>
          <button type="button" class="btn btn-outline btn-block" onclick="requestOtp()">Send OTP</button>
        </div>
        <div id="otpStep2" class="hidden">
          <div class="field"><label class="lbl">Enter the 6-digit code</label>
            <input id="otpCode" class="input" maxlength="6" placeholder="______"></div>
          <button type="button" class="btn btn-brand btn-block" onclick="verifyOtp()">Verify &amp; continue</button>
          <div id="otpDev" class="muted" style="font-size:12px;margin-top:6px"></div>
        </div>
        <div id="otpMsg"></div>
      <% } %>

      <asp:HiddenField ID="hidToken" runat="server" />
      <asp:Button ID="btnEstablish" runat="server" OnClick="btnEstablish_Click" style="display:none" />
      <asp:Literal ID="litMsg" runat="server" />

      <% if (CurrentPortal == Kirana.WebApi.Data.Portal.Consumer) { %>
        <div class="muted" style="text-align:center;margin-top:14px;font-size:13px">
          New here? <a href="signup.aspx">Create a customer account</a></div>
      <% } %>
      <% if (CurrentPortal == Kirana.WebApi.Data.Portal.Store) { %>
        <div class="muted" style="text-align:center;margin-top:14px;font-size:13px">
          New store? <a href="register.aspx">Register your store</a></div>
      <% } %>
    </div></div>
  </div>

  <script>
    var ESTABLISH = { hid:'<%= hidToken.ClientID %>', btn:'<%= btnEstablish.ClientID %>' };
    function establish(token){
      document.getElementById(ESTABLISH.hid).value = token;
      document.getElementById(ESTABLISH.btn).click();
    }
    function onGoogle(resp){
      fetch('/api/auth/google', { method:'POST', headers:{'Content-Type':'application/json'},
        body: JSON.stringify({ idToken: resp.credential }) })
        .then(function(r){ return r.json().then(function(d){ return { ok:r.ok, d:d }; }); })
        .then(function(x){ if(!x.ok){ alert(x.d.error || 'Google sign-in failed'); return; } establish(x.d.token); });
    }
    function requestOtp(){
      var phone=document.getElementById('otpPhone').value.trim();
      if(!phone){ document.getElementById('otpMsg').innerHTML='<div class="alert alert-err">Enter your mobile number.</div>'; return; }
      fetch('/api/auth/request-otp',{method:'POST',headers:{'Content-Type':'application/json'},body:JSON.stringify({phone:phone})})
        .then(function(r){return r.json().then(function(d){return {ok:r.ok,d:d};});})
        .then(function(x){ if(!x.ok){ document.getElementById('otpMsg').innerHTML='<div class="alert alert-err">'+(x.d.error||'Could not send OTP')+'</div>'; return; }
          document.getElementById('otpStep1').classList.add('hidden');
          document.getElementById('otpStep2').classList.remove('hidden');
          if(x.d.devMode && x.d.code) document.getElementById('otpDev').textContent='Dev code: '+x.d.code; });
    }
    function verifyOtp(){
      var phone=document.getElementById('otpPhone').value.trim();
      var code=document.getElementById('otpCode').value.trim();
      fetch('/api/auth/verify-otp',{method:'POST',headers:{'Content-Type':'application/json'},body:JSON.stringify({phone:phone,code:code})})
        .then(function(r){return r.json().then(function(d){return {ok:r.ok,d:d};});})
        .then(function(x){ if(!x.ok){ document.getElementById('otpMsg').innerHTML='<div class="alert alert-err">'+(x.d.error||'Invalid code')+'</div>'; return; } establish(x.d.token); });
    }
  </script>
</asp:Content>
