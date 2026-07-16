<%@ Page Title="Admin" Language="C#" MasterPageFile="~/Dashboard.master" AutoEventWireup="true" CodeBehind="admin.aspx.cs" Inherits="Kirana.WebApi.admin" %>
<asp:Content ID="ct" ContentPlaceHolderID="title" runat="server">Store approvals</asp:Content>
<asp:Content ID="cn" ContentPlaceHolderID="nav" runat="server">
  <div class="ka-navgroup"><div class="gl">Approvals</div><nav>
    <a href="admin.aspx" class="active" data-tip="Store approvals">
      <svg class="ic" viewBox="0 0 24 24"><path d="M4 9l1-5h14l1 5"/><path d="M5 9v11h14V9"/><path d="M10 20v-5h4v5"/></svg>
      <span class="lbl3">Store approvals</span></a>
    <a href="adminproducts.aspx" data-tip="Product approvals">
      <svg class="ic" viewBox="0 0 24 24"><path d="M12 3l8 4.5v9L12 21l-8-4.5v-9z"/><path d="M4 7.5l8 4.5 8-4.5M12 12v9"/></svg>
      <span class="lbl3">Product approvals</span></a>
  </nav></div>
  <div class="ka-navgroup"><div class="gl">Catalog</div><nav>
    <a href="adminimages.aspx" data-tip="Catalog images">
      <svg class="ic" viewBox="0 0 24 24"><rect x="3" y="5" width="18" height="14" rx="2.5"/><circle cx="8.5" cy="10" r="1.6"/><path d="M21 16l-5-5-6 6"/></svg>
      <span class="lbl3">Catalog images</span></a>
  </nav></div>
</asp:Content>
<asp:Content ID="cm" ContentPlaceHolderID="main" runat="server">
  <div class="card"><div class="card-h"><h3>Pending stores</h3></div><div class="card-b">
    <asp:Literal ID="litMsg" runat="server" />
    <asp:GridView ID="grid" runat="server" AutoGenerateColumns="false" CssClass="table" GridLines="None"
        DataKeyNames="Id" OnRowCommand="grid_RowCommand" EmptyDataText="Nothing pending. 🎉" Width="100%">
      <Columns>
        <asp:BoundField DataField="Name" HeaderText="Store" />
        <asp:BoundField DataField="OwnerName" HeaderText="Owner" />
        <asp:BoundField DataField="Email" HeaderText="Email" />
        <asp:BoundField DataField="City" HeaderText="City" />
        <asp:BoundField DataField="Gstin" HeaderText="GSTIN" />
        <asp:TemplateField HeaderText="">
          <ItemTemplate>
            <asp:LinkButton runat="server" CssClass="btn btn-sm btn-brand" CommandName="Approve" CommandArgument='<%# Eval("Id") %>' Text="Approve" />
            <asp:LinkButton runat="server" CssClass="btn btn-sm btn-danger" CommandName="Reject" CommandArgument='<%# Eval("Id") %>' Text="Reject" />
          </ItemTemplate>
        </asp:TemplateField>
      </Columns>
    </asp:GridView>
  </div></div>
</asp:Content>
