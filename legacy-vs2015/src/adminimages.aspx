<%@ Page Title="Catalog images" Language="C#" MasterPageFile="~/Dashboard.master" AutoEventWireup="true" CodeBehind="adminimages.aspx.cs" Inherits="Kirana.WebApi.adminimages" %>
<asp:Content ID="ct" ContentPlaceHolderID="title" runat="server">Catalog images</asp:Content>
<asp:Content ID="cn" ContentPlaceHolderID="nav" runat="server">
  <div class="ka-navgroup"><div class="gl">Approvals</div><nav>
    <a href="admin.aspx" data-tip="Store approvals">
      <svg class="ic" viewBox="0 0 24 24"><path d="M4 9l1-5h14l1 5"/><path d="M5 9v11h14V9"/><path d="M10 20v-5h4v5"/></svg>
      <span class="lbl3">Store approvals</span></a>
    <a href="adminproducts.aspx" data-tip="Product approvals">
      <svg class="ic" viewBox="0 0 24 24"><path d="M12 3l8 4.5v9L12 21l-8-4.5v-9z"/><path d="M4 7.5l8 4.5 8-4.5M12 12v9"/></svg>
      <span class="lbl3">Product approvals</span></a>
  </nav></div>
  <div class="ka-navgroup"><div class="gl">Catalog</div><nav>
    <a href="adminimages.aspx" class="active" data-tip="Catalog images">
      <svg class="ic" viewBox="0 0 24 24"><rect x="3" y="5" width="18" height="14" rx="2.5"/><circle cx="8.5" cy="10" r="1.6"/><path d="M21 16l-5-5-6 6"/></svg>
      <span class="lbl3">Catalog images</span></a>
  </nav></div>
</asp:Content>
<asp:Content ID="cm" ContentPlaceHolderID="main" runat="server">
  <div class="card" style="margin-bottom:16px"><div class="card-h"><h3>Add or replace a shared image</h3></div>
    <div class="card-b">
      <p class="muted" style="margin-top:0">Enter the exact product name (e.g. <b>Fresh Apples</b>). The image is shared by every store that lists that product.</p>
      <div class="row" style="align-items:flex-end">
        <div class="field" style="flex:1;min-width:220px"><label class="lbl">Product name</label>
          <asp:TextBox ID="txtName" runat="server" CssClass="input" /></div>
        <div class="field" style="flex:1;min-width:220px"><label class="lbl">Upload image</label>
          <asp:FileUpload ID="fileImg" runat="server" CssClass="input" /></div>
        <div class="field" style="flex:1;min-width:220px"><label class="lbl">…or paste an image URL</label>
          <asp:TextBox ID="txtUrl" runat="server" CssClass="input" /></div>
        <div class="field" style="align-self:flex-end">
          <asp:Button ID="btnSave" runat="server" CssClass="btn btn-brand" Text="Save image" OnClick="btnSave_Click" /></div>
      </div>
      <asp:Literal ID="litMsg" runat="server" />
    </div>
  </div>

  <div class="card"><div class="card-h"><h3>Shared master images</h3>
    <asp:Button ID="btnRefresh" runat="server" CssClass="btn btn-sm btn-outline right" Text="Refresh" OnClick="btnRefresh_Click" /></div>
    <div class="card-b">
      <asp:Repeater ID="rpt" runat="server">
        <HeaderTemplate><div class="cats" style="grid-template-columns:repeat(auto-fill,minmax(150px,1fr))"></HeaderTemplate>
        <ItemTemplate>
          <div class="cat" style="cursor:default">
            <img src='<%# Eval("ImageUrl") %>' alt="" style="width:84px;height:84px;object-fit:cover;border-radius:14px;border:1px solid var(--line)" />
            <div class="lbl2"><%# Eval("Name") %></div>
            <div class="cnt"><%# Eval("ImageName") %></div>
          </div>
        </ItemTemplate>
        <FooterTemplate></div></FooterTemplate>
      </asp:Repeater>
      <asp:Literal ID="litEmpty" runat="server" />
    </div>
  </div>
</asp:Content>
