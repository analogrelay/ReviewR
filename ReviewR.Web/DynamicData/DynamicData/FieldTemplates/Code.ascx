<%@ Control Language="C#" CodeBehind="Code.ascx.cs" Inherits="DynamicData.CodeField" %>

<pre class="prettyprint linenums" runat="server" visible="<%# ContainerType == System.Web.DynamicData.ContainerType.Item %>"><asp:Literal runat="server" ID="Literal1" Text="<%# FieldValueString %>" /></pre>
<asp:PlaceHolder runat="server" visible="<%# ContainerType == System.Web.DynamicData.ContainerType.List %>">
    <code class="prettyprint linenums" runat="server">
        <asp:Literal runat="server" ID="Literal2" Text="<%# FieldValueStringSummary %>" />
    </code>
</asp:PlaceHolder>

