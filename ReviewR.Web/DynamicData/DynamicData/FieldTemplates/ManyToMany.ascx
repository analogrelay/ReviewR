<%@ Control Language="C#" CodeBehind="ManyToMany.ascx.cs" Inherits="DynamicData.ManyToManyField" %>

<asp:Repeater ID="Repeater1" runat="server">
    <ItemTemplate>
      <asp:DynamicHyperLink runat="server" CssClass="label label-info"></asp:DynamicHyperLink>
    </ItemTemplate>
</asp:Repeater>

