<%@ Page Title="Sign up" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="signup.aspx.cs" Inherits="Kirana.WebApi.signup" %>
<asp:Content ID="c" ContentPlaceHolderID="main" runat="server">
  <div style="max-width:420px;margin:20px auto">
    <h2>Create your shopper account</h2>
    <div class="card"><div class="card-b">
      <div class="field"><label class="lbl">Full name</label><asp:TextBox ID="txtName" runat="server" CssClass="input" /></div>
      <div class="field"><label class="lbl">Email</label><asp:TextBox ID="txtEmail" runat="server" CssClass="input" /></div>
      <div class="field"><label class="lbl">Password</label><asp:TextBox ID="txtPass" runat="server" TextMode="Password" CssClass="input" /></div>
      <asp:Button ID="btnSignup" runat="server" Text="Create account" CssClass="btn btn-brand btn-block" OnClick="btnSignup_Click" />
      <asp:Literal ID="litMsg" runat="server" />
      <hr />
      <div class="muted" style="text-align:center">Already have an account? <a href="login.aspx">Log in</a></div>
    </div></div>
  </div>
</asp:Content>
