<%@ Page Language="C#" MasterPageFile="~/DynamicData/Site.master" CodeBehind="List.aspx.cs" Inherits="DynamicData.List" %>

<%@ Register src="~/DynamicData/DynamicData/Content/GridViewPager.ascx" tagname="GridViewPager" tagprefix="asp" %>

<asp:Content ID="headContent" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:DynamicDataManager ID="DynamicDataManager1" runat="server" AutoLoadForeignKeys="true">
        <DataControls>
            <asp:DataControlReference ControlID="GridView1" />
        </DataControls>
    </asp:DynamicDataManager>

    <h1><%= table.DisplayName%></h1>

    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="DD">
                <asp:ValidationSummary ID="ValidationSummary1" runat="server" EnableClientScript="true"
                    HeaderText="List of validation errors" CssClass="DDValidator" />
                <asp:DynamicValidator runat="server" ID="GridViewValidator" ControlToValidate="GridView1" Display="None" CssClass="DDValidator" />

                <asp:QueryableFilterRepeater runat="server" ID="FilterRepeater">
                    <ItemTemplate>
                        <asp:Label runat="server" Text='<%# Eval("DisplayName") %>' OnPreRender="Label_PreRender" />
                        <asp:DynamicFilter runat="server" ID="DynamicFilter" OnFilterChanged="DynamicFilter_FilterChanged" /><br />
                    </ItemTemplate>
                </asp:QueryableFilterRepeater>
                <br />
            </div>

            <asp:GridView ID="GridView1" runat="server" DataSourceID="GridDataSource" EnablePersistedSelection="true"
                AllowPaging="True" AllowSorting="True" CssClass="ddtable table-bordered table-striped"
                RowStyle-CssClass="td" HeaderStyle-CssClass="th" CellPadding="6">
                <Columns>
                    <asp:TemplateField ItemStyle-Width="250px">
                        <ItemTemplate>
                            <asp:DynamicHyperLink runat="server" Action="Edit" CssClass="btn">
                                <i class="icon-edit"></i>
                                Edit
                            </asp:DynamicHyperLink>
                            <asp:LinkButton runat="server" CommandName="Delete" CssClass="btn btn-danger" OnClientClick='return confirm("Are you sure you want to delete this item?");'>
                                <i class="icon-trash icon-white"></i>
                                Delete
                            </asp:LinkButton>
                            <asp:DynamicHyperLink runat="server" CssClass="btn btn-primary">
                                <i class="icon-list icon-white"></i>
                                Details
                            </asp:DynamicHyperLink>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>

                <PagerStyle CssClass="DDFooter"/>        
                <PagerTemplate>
                    <ul class="pagination pagination-centered">
                        <li>
                            <asp:LinkButton CommandName="Page" CommandArgument="First" runat="server"><i class="icon-fast-backward"></i></asp:LinkButton>
                        </li>
                        <li>
                            <asp:LinkButton CommandName="Page" CommandArgument="Prev" runat="server"><i class="icon-backward"></i></asp:LinkButton>
                        </li>
                        <asp:Repeater DataSource="<%# Enumerable.Range(-2, 5).Select(i => i + GridView1.PageIndex).Where(i => i >= 0) %>" runat="server" OnItemCommand="Unnamed_ItemCommand">
                            <ItemTemplate>
                                <li runat="server" class='<%# ((int)Container.DataItem) == GridView1.PageIndex ? "active" : "" %>'>
                                    <asp:LinkButton CommandName="Page" CommandArgument="<%# (int)Container.DataItem %>" runat="server"><%# (int)Container.DataItem + 1 %></asp:LinkButton>
                                </li>
                            </ItemTemplate>
                        </asp:Repeater>
                        <li>
                            <asp:LinkButton CommandName="Page" CommandArgument="Next" runat="server"><i class="icon-forward"></i></asp:LinkButton>
                        </li>
                        <li>
                            <asp:LinkButton CommandName="Page" CommandArgument="Last" runat="server"><i class="icon-fast-forward"></i></asp:LinkButton>
                        </li>
                    </ul>
                    <%--<asp:GridViewPager runat="server" />--%>
                </PagerTemplate>
                <EmptyDataTemplate>
                    There are currently no items in this table.
                </EmptyDataTemplate>
            </asp:GridView>

            <asp:EntityDataSource ID="GridDataSource" runat="server" EnableDelete="true" />
            
            <asp:QueryExtender TargetControlID="GridDataSource" ID="GridQueryExtender" runat="server">
                <asp:DynamicFilterExpression ControlID="FilterRepeater" />
            </asp:QueryExtender>

            <br />

            <div>
                <asp:DynamicHyperLink ID="InsertHyperLink" runat="server" Action="Insert" CssClass="btn"><i class="icon-plus"></i> Insert</asp:DynamicHyperLink>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

