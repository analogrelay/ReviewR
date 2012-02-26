<%@ Control Language="C#" CodeBehind="Children.ascx.cs" Inherits="DynamicData.ChildrenField" %>

<asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="<%# GetChildrenPath() %>" CssClass="btn btn-info">
    <i class="icon-folder-open icon-white"></i>
    <asp:Literal ID="Literal1" runat="server" />
</asp:HyperLink>

