<%@ Page Title="Login" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="Kirana.WebApi.login" %>
<asp:Content ID="c" ContentPlaceHolderID="main" runat="server">
  <div style="max-width:420px;margin:20px auto">
    <h2>Sign in</h2>
    <div class="card"><div class="card-b">
      <div class="field"><label class="lbl">Email</label>
        <asp:TextBox ID="txtEmail" runat="server" CssClass="input" /></div>
      <div class="field"><label class="lbl">Password</label>
        <asp:TextBox ID="txtPass" runat="server" TextMode="Password" CssClass="input" /></div>
      <asp:Button ID="btnLogin" runat="server" Text="Login" CssClass="btn btn-brand btn-block" OnClick="btnLogin_Click" />
      <asp:Literal ID="litMsg" runat="server" />
      <hr />
      <div class="muted" style="text-align:center">New store? <a href="register.aspx">Register</a> · <a href="signup.aspx">Customer sign up</a></div>
      <div class="muted" style="text-align:center;font-size:12px;margin-top:6px">
        Admin: superadmin@kirana.local / Admin@12345 · Store: demo@store.local / Demo@12345</div>
    </div></div>
  </div>
</asp:Content>
