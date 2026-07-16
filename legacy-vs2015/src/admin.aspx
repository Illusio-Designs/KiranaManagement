<%@ Page Title="Admin" Language="C#" MasterPageFile="~/Dashboard.master" AutoEventWireup="true" CodeBehind="admin.aspx.cs" Inherits="Kirana.WebApi.admin" %>
<asp:Content ID="ct" ContentPlaceHolderID="title" runat="server">Store approvals</asp:Content>
<asp:Content ID="cn" ContentPlaceHolderID="nav" runat="server">
  <div class="ka-navgroup"><div class="gl">General</div><nav>
    <a href="admin.aspx" class="active" data-tip="Store approvals">
      <svg class="ic" viewBox="0 0 24 24"><circle cx="12" cy="12" r="8.5"/><path d="M12 7v5l3.5 2"/></svg>
      <span class="lbl3">Store approvals</span></a>
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
