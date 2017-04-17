<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="frmManageEventsPrintCertificates.aspx.cs" Inherits="frmEventsManage_PrintCerts" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <style>
        td { padding: 10px 30px 10px 30px; }
    </style>
    <telerik:RadTabStrip ID="RadTabStrip1" runat="server" SelectedIndex="0" MultiPageID="RadMultiPage1">
        <Tabs>
            <telerik:RadTab runat="server" Text="View Events" Font-Bold="true" Selected="True"></telerik:RadTab>
        </Tabs>
    </telerik:RadTabStrip>
    <telerik:RadMultiPage ID="RadMultiPage1" runat="server" SelectedIndex="0">
        <telerik:RadPageView ID="RadPageView1" runat="server">
            <telerik:RadGrid ID="repPrintCertsRG" runat="server" AllowFilteringByColumn="True" AllowPaging="True" AllowSorting="True" DataSourceID="repDS" GroupPanelPosition="Top" OnItemCommand="repPrintCertsRG_ItemCommand">
                <GroupingSettings CollapseAllTooltip="Collapse all groups" />
                <MasterTableView AutoGenerateColumns="False" DataSourceID="repDS">
                    <Columns>
                        <telerik:GridBoundColumn DataField="ParentEventName" FilterControlAltText="Filter ParentEventName column" HeaderText="Parent Event Name" SortExpression="ParentEventName" UniqueName="ParentEventName">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="id" Display="false" FilterControlAltText="Filter id column" HeaderText="Event id" SortExpression="id" UniqueName="id">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="EventName" FilterControlAltText="Filter EventName column" HeaderText="Event Name" SortExpression="EventName" UniqueName="EventName">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="EventStatus" Display="false" DataType="System.Int32" FilterControlAltText="Filter Active column" HeaderText="Event Status" SortExpression="EventStatus" UniqueName="EventStatus">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="StartDate" DataType="System.DateTime" DataFormatString="{0:dd/MM/yyyy}" FilterControlAltText="Filter StartDate column" HeaderText="Start Date" SortExpression="StartDate" UniqueName="StartDate">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="OrganisedBy" FilterControlAltText="Filter OrganisedBy column" HeaderText="OrganisedBy" SortExpression="Organised By" UniqueName="OrganisedBy">
                        </telerik:GridBoundColumn>
                        <telerik:GridTemplateColumn HeaderText="Download PDF" UniqueName="DownloadPDF"> 
                            <ItemTemplate>
                                <telerik:RadButton ID="repDownloadBtn" runat="server" Text="Download PDF" CommandName="DownloadPDF"/>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn HeaderText="Mark Printed" UniqueName="MarkPrinted"> 
                            <ItemTemplate>
                                <telerik:RadButton ID="repMarkPrintedBtn" runat="server" Text="Mark Printed" CommandName="MarkPrinted"/>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>

                    </Columns>
                    <GroupByExpressions>
                    <telerik:GridGroupByExpression>
                        <SelectFields>
                            <telerik:GridGroupByField FieldName="ParentEventName" />
                        </SelectFields>
                        <GroupByFields>
                            <telerik:GridGroupByField FieldName="ParentEventName"/>
                        </GroupByFields>
                    </telerik:GridGroupByExpression>
                </GroupByExpressions>
                </MasterTableView>
            </telerik:RadGrid>
            <asp:SqlDataSource ID="repDS" runat="server" ConnectionString="<%$ ConnectionStrings:NewUmsConnectionString %>" SelectCommand="(SELECT em1.EventName[EventName], em1.StartDate[StartDate], em1.EventStatus[EventStatus], em1.OrganisedBy[OrganisedBy], em1.id[id], em2.EventName[ParentEventName] FROM EventMaster em1 INNER JOIN dbo.EventMaster em2 ON em1.parentEventId = em2.id WHERE em1.EventStatus = 4 AND em1.ExcelName IS NOT NULL AND em1.ParentEventID IS NOT NULL) UNION (SELECT EventName[EventName], StartDate[StartDate], EventStatus[EventStatus], OrganisedBy[OrganisedBy], [id], 'N.A.'[ParentEventName] FROM EventMaster WHERE EventStatus = 4 AND ExcelName IS NOT NULL AND ParentEventID IS NULL)"></asp:SqlDataSource>
        </telerik:RadPageView>
    </telerik:RadMultiPage> 
</asp:Content>

