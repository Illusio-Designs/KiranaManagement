<%@ Page Title="Register store" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="register.aspx.cs" Inherits="Kirana.WebApi.register" %>
<asp:Content ID="c" ContentPlaceHolderID="main" runat="server">
  <div style="max-width:720px;margin:0 auto">
    <h2>Register your store</h2>
    <p class="muted">After you register, a Super Admin reviews and approves it — then you can log in and start selling.</p>
    <div class="card"><div class="card-b">
      <div class="row">
        <div class="field" style="flex:1;min-width:220px"><label class="lbl">Store name *</label><asp:TextBox ID="txtName" runat="server" CssClass="input" /></div>
        <div class="field" style="flex:1;min-width:220px"><label class="lbl">Owner name *</label><asp:TextBox ID="txtOwner" runat="server" CssClass="input" /></div>
      </div>
      <div class="row">
        <div class="field" style="flex:1;min-width:220px"><label class="lbl">Email *</label><asp:TextBox ID="txtEmail" runat="server" CssClass="input" /></div>
        <div class="field" style="flex:1;min-width:220px"><label class="lbl">Phone</label><asp:TextBox ID="txtPhone" runat="server" CssClass="input" /></div>
      </div>
      <div class="row">
        <div class="field" style="flex:1;min-width:220px"><label class="lbl">Password *</label><asp:TextBox ID="txtPass" runat="server" TextMode="Password" CssClass="input" /></div>
        <div class="field" style="flex:1;min-width:220px"><label class="lbl">City</label><asp:TextBox ID="txtCity" runat="server" CssClass="input" /></div>
      </div>
      <div class="row">
        <div class="field" style="flex:1;min-width:220px"><label class="lbl">GSTIN</label><asp:TextBox ID="txtGstin" runat="server" CssClass="input" /></div>
        <div class="field" style="flex:1;min-width:220px"><label class="lbl">PAN</label><asp:TextBox ID="txtPan" runat="server" CssClass="input" /></div>
      </div>
      <asp:Button ID="btnReg" runat="server" Text="Register store" CssClass="btn btn-brand btn-block" OnClick="btnReg_Click" />
      <asp:Literal ID="litMsg" runat="server" />
    </div></div>
  </div>
</asp:Content>
