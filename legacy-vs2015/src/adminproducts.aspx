<%@ Page Title="Product approvals" Language="C#" MasterPageFile="~/Dashboard.master" AutoEventWireup="true" CodeBehind="adminproducts.aspx.cs" Inherits="Kirana.WebApi.adminproducts" %>
<asp:Content ID="ct" ContentPlaceHolderID="title" runat="server">Product approvals</asp:Content>
<asp:Content ID="cn" ContentPlaceHolderID="nav" runat="server">
  <div class="ka-navgroup"><div class="gl">Approvals</div><nav>
    <a href="admin.aspx" data-tip="Store approvals">
      <svg class="ic" viewBox="0 0 24 24"><path d="M4 9l1-5h14l1 5"/><path d="M5 9v11h14V9"/><path d="M10 20v-5h4v5"/></svg>
      <span class="lbl3">Store approvals</span></a>
    <a href="adminproducts.aspx" class="active" data-tip="Product approvals">
      <svg class="ic" viewBox="0 0 24 24"><path d="M12 3l8 4.5v9L12 21l-8-4.5v-9z"/><path d="M4 7.5l8 4.5 8-4.5M12 12v9"/></svg>
      <span class="lbl3">Product approvals</span></a>
  </nav></div>
</asp:Content>
<asp:Content ID="cm" ContentPlaceHolderID="main" runat="server">
  <div class="card"><div class="card-h"><h3>Products awaiting approval</h3>
    <span class="badge badge-amber right">Only approved products appear on the marketplace</span></div>
    <div class="card-b">
    <asp:Literal ID="litMsg" runat="server" />
    <asp:GridView ID="grid" runat="server" AutoGenerateColumns="false" CssClass="table" GridLines="None"
        DataKeyNames="Id" OnRowCommand="grid_RowCommand" EmptyDataText="Nothing pending. All caught up!" Width="100%">
      <Columns>
        <asp:BoundField DataField="Name" HeaderText="Product" />
        <asp:BoundField DataField="Brand" HeaderText="Brand" />
        <asp:BoundField DataField="Category" HeaderText="Category" />
        <asp:BoundField DataField="StoreName" HeaderText="Store" />
        <asp:BoundField DataField="Variants" HeaderText="Variants" />
        <asp:BoundField DataField="PriceRange" HeaderText="Price" />
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
