<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="frmManageEventsHOS.aspx.cs" Inherits="frmManageEventsHOS" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <style> td { padding: 10px 30px 10px 10px;} </style>
    <telerik:RadTabStrip ID="RadTabStrip1" runat="server" SelectedIndex="0" MultiPageID="RadMultiPage1">
        <Tabs>
            <telerik:RadTab runat="server" Text="Approve Events" Font-Bold="true" Selected="True"></telerik:RadTab>
            <telerik:RadTab runat="server" Text="Approve Event Data" Font-Bold="true"></telerik:RadTab>
        </Tabs>
    </telerik:RadTabStrip>
    <telerik:RadMultiPage ID="RadMultiPage1" runat="server" SelectedIndex="0">
        <telerik:RadPageView ID="RadPageView1" runat="server">
            <div style="padding:30px;">
                Select School: <telerik:RadComboBox ID="rehSelectSchoolCB" runat="server" AutoPostBack="true" Width="350" EmptyMessage="Choose Your School" OnSelectedIndexChanged="rehSelectSchoolCB_SelectedIndexChanged"></telerik:RadComboBox>                
                <telerik:RadGrid style="margin-top:30px" ID="rehApproveEventsRG" runat="server" CellSpacing="-1" GridLines="Both" GroupPanelPosition="Top" ShowGroupPanel="True" Visible="false" OnItemCommand="rehApproveEventsRG_ItemCommand">
                    <GroupingSettings CollapseAllTooltip="Collapse all groups" />
                    <ClientSettings AllowDragToGroup="True">
                    </ClientSettings>
                    <MasterTableView AutoGenerateColumns="False">
                        <Columns>
                            <telerik:GridBoundColumn DataField="id" DataType="System.Int32" FilterControlAltText="Filter id column" HeaderText="Event ID" ReadOnly="True" SortExpression="id" UniqueName="id"></telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="EventName" FilterControlAltText="Filter EventName column" HeaderText="Event Name" SortExpression="EventName" UniqueName="EventName"></telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="StartDate" DataType="System.DateTime" FilterControlAltText="Filter StartDate column" HeaderText="StartDate" SortExpression="StartDate" UniqueName="StartDate"></telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="ParentEventName" FilterControlAltText="Filter ParentEventName column" HeaderText="Parent Event Name" SortExpression="ParentEventName" UniqueName="ParentEventName"></telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="ParentEventID" Display="false" UniqueName="ParentEventID"></telerik:GridBoundColumn>
                            <telerik:GridTemplateColumn HeaderText="Approve Event" UniqueName="ApproveEvent"> 
                                <ItemTemplate><telerik:RadButton ID="rehApproveEventBtn" runat="server" Text="Approve" CommandName="ApproveEvent"/></ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderText="Reject Event" UniqueName="RejectEvent"> 
                                <ItemTemplate><telerik:RadButton ID="rehRejectEventBtn" runat="server" Text="Reject" CommandName="RejectEvent"/></ItemTemplate>
                            </telerik:GridTemplateColumn>
                        </Columns>
                        <GroupByExpressions>
                            <telerik:GridGroupByExpression>
                                <SelectFields><telerik:GridGroupByField FieldName="ParentEventName" /></SelectFields>
                                <GroupByFields><telerik:GridGroupByField FieldName="ParentEventName"/></GroupByFields>
                            </telerik:GridGroupByExpression>
                        </GroupByExpressions>
                    </MasterTableView>
                </telerik:RadGrid>
            </div>
        </telerik:RadPageView>
        <telerik:RadPageView ID="RadPageView2" runat="server">
            <div style="padding:30px;">
                Select School: <telerik:RadComboBox ID="rehSelectSchool2CB" runat="server" AutoPostBack="true" Width="350" EmptyMessage="Choose Your School" OnSelectedIndexChanged="rehSelectSchool2CB_SelectedIndexChanged"></telerik:RadComboBox>
                <telerik:RadGrid style="margin-top:30px" ID="rehApproveExcelRG" runat="server" CellSpacing="-1" GridLines="Both" GroupPanelPosition="Top" ShowGroupPanel="True" Visible="false" OnItemCommand="rehApproveExcelRG_ItemCommand">
                    <GroupingSettings CollapseAllTooltip="Collapse all groups" />
                    <ClientSettings AllowDragToGroup="True">
                    </ClientSettings>
                    <MasterTableView AutoGenerateColumns="False">
                        <Columns>
                            <telerik:GridBoundColumn DataField="id" DataType="System.Int32" FilterControlAltText="Filter id column" HeaderText="Event ID" ReadOnly="True" SortExpression="id" UniqueName="id"></telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="EventName" FilterControlAltText="Filter EventName column" HeaderText="Event Name" SortExpression="EventName" UniqueName="EventName"></telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="StartDate" DataType="System.DateTime" FilterControlAltText="Filter StartDate column" HeaderText="StartDate" SortExpression="StartDate" UniqueName="StartDate"></telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="ParentEventName" FilterControlAltText="Filter ParentEventName column" HeaderText="Parent Event Name" SortExpression="ParentEventName" UniqueName="ParentEventName"></telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="ParentEventID" Visible="false" UniqueName="ParentEventID"></telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="ExcelName" Display="false" UniqueName="ExcelName"></telerik:GridBoundColumn>
                            <telerik:GridTemplateColumn HeaderText="View Data" UniqueName="ViewExcelFile"> 
                                    <ItemTemplate>
                                        <telerik:RadButton ID="rehViewExcelBtn" runat="server" Text="View Data" CommandName="ViewData"/>
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderText="Approve Data" UniqueName="ApproveData"> 
                                <ItemTemplate><telerik:RadButton ID="rehApproveDataBtn" runat="server" Text="Approve" CommandName="ApproveData"/></ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderText="Reject Data" UniqueName="RejectData"> 
                                <ItemTemplate><telerik:RadButton ID="rehRejectDataBtn" runat="server" Text="Reject" CommandName="RejectData"/></ItemTemplate>
                            </telerik:GridTemplateColumn>
                        </Columns>
                        <GroupByExpressions>
                            <telerik:GridGroupByExpression>
                                <SelectFields><telerik:GridGroupByField FieldName="ParentEventName" /></SelectFields>
                                <GroupByFields><telerik:GridGroupByField FieldName="ParentEventName"/></GroupByFields>
                            </telerik:GridGroupByExpression>
                        </GroupByExpressions>
                    </MasterTableView>
                </telerik:RadGrid>
            </div>
        </telerik:RadPageView>
    </telerik:RadMultiPage>
</asp:Content>


