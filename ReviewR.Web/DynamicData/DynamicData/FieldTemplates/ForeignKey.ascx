<%@ Control Language="C#" CodeBehind="ForeignKey.ascx.cs" Inherits="DynamicData.ForeignKeyField" %>

<asp:HyperLink ID="HyperLink1" runat="server" CssClass="label label-info"
    Text="<%# GetDisplayString() %>"
    NavigateUrl="<%# GetNavigateUrl() %>"  />

