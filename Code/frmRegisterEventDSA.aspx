<%@ Page  MasterPageFile="~/MasterPage.master" Language="C#" AutoEventWireup="true" CodeFile="frmRegisterEventDSA.aspx.cs" Inherits="frmRegisterEventDSA" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    
    <style>
        td { padding: 10px 30px 10px 30px; }
    </style>

    <telerik:RadTabStrip ID="RadTabStrip1" runat="server" SelectedIndex="0" MultiPageID="RadMultiPage1">
        <Tabs>
            <telerik:RadTab runat="server" Text="Initiate Event" Font-Bold="true" Selected="True"></telerik:RadTab>
            <telerik:RadTab runat="server" Text="Check Event Invites" Font-Bold="true"></telerik:RadTab>
        </Tabs>
    </telerik:RadTabStrip>

    <telerik:RadMultiPage ID="RadMultiPage1" runat="server" SelectedIndex="0">
        <telerik:RadPageView ID="RadPageView1" runat="server">
                <h3>Register Events - Division of Student Affairs</h3>
                <table style="margin-top:30px;">
                    <tr>
                        <td>Event Name</td>
                        <td><asp:TextBox ID="redENametxt" runat="server"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td>Start Date</td>
                        <td><telerik:RadDatePicker ID="redESDateDP" runat="server"></telerik:RadDatePicker></td>
                    </tr>
                    <tr>
                        <td>End Date</td>
                        <td><telerik:RadDatePicker ID="redEEDateDP" runat="server"></telerik:RadDatePicker></td>
                    </tr>
                    <tr>
                        <td>Organized By</td>
                        <td><asp:TextBox ID="redEOrganizedBy" runat="server"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td colspan="2" style="text-align:center;"><asp:CheckBox ID="redAllowSubEventCB" runat="server" Text="Invite Schools for sub events"/></td>
                    </tr>
                    <tr>
                        <td colspan="2" style="text-align:center;"><telerik:RadButton ID="redERegisterBtn" runat="server" Text="Register" OnClick="resERegisterBtn_Click" AutoPostBack="true"></telerik:RadButton></td>
                    </tr>
                </table>
         </telerik:RadPageView>
         <telerik:RadPageView ID="RadPageView2" runat="server">
               <h3>Invitation for Registeration</h3>
               <telerik:RadGrid ID="redSERegRequestRG" runat="server" AllowFilteringByColumn="True" AllowPaging="True" AllowSorting="True" DataSourceID="SERegReqDS" GroupPanelPosition="Top" OnItemCommand="redSERegRequestRG_ItemCommand" OnItemDataBound="redSERegRequestRG_ItemDataBound">
                   <GroupingSettings CollapseAllTooltip="Collapse all groups" />
                   <MasterTableView AutoGenerateColumns="False" DataSourceID="SERegReqDS">
                       <Columns>
                           <telerik:GridBoundColumn DataField="ParentEventName" FilterControlAltText="Filter ParentEventName column" HeaderText="Parent Event Name" SortExpression="ParentEventName" UniqueName="ParentEventName">
                            </telerik:GridBoundColumn>
                           <telerik:GridBoundColumn DataField="id" Display="false" FilterControlAltText="Filter id column" HeaderText="Event id" SortExpression="id" UniqueName="id">
                           </telerik:GridBoundColumn>
                           <telerik:GridBoundColumn DataField="EventName" FilterControlAltText="Filter EventName column" HeaderText="Event Name" SortExpression="EventName" UniqueName="EventName">
                           </telerik:GridBoundColumn>
                           <telerik:GridBoundColumn DataField="EventStatus" DataType="System.Int32" FilterControlAltText="Filter Active column" HeaderText="Event Status" SortExpression="EventStatus" UniqueName="EventStatus">
                            </telerik:GridBoundColumn>
                           <telerik:GridBoundColumn DataField="StartDate" DataType="System.DateTime" DataFormatString="{0:dd/MM/yyyy}" FilterControlAltText="Filter StartDate column" HeaderText="Start Date" SortExpression="StartDate" UniqueName="StartDate">
                           </telerik:GridBoundColumn>
                           <telerik:GridBoundColumn DataField="OrganisedBy" FilterControlAltText="Filter OrganisedBy column" HeaderText="OrganisedBy" SortExpression="Organised By" UniqueName="OrganisedBy">
                           </telerik:GridBoundColumn>
                           
                           <telerik:GridBoundColumn DataField="ParentEventID" Display="false" FilterControlAltText="Filter ParentEventID column" HeaderText="ParentEvent ID" SortExpression="ParentEventID" UniqueName="ParentEventID">
                           </telerik:GridBoundColumn>

                           <telerik:GridTemplateColumn HeaderText="Approve Registeration" UniqueName="ButtonColumn1"> 
                                <ItemTemplate>
                                    <telerik:RadButton ID="redSEReqAllowBtn" runat="server" Text="Approve" CommandName="Approve"/>
                                </ItemTemplate>
                           </telerik:GridTemplateColumn>
                           <telerik:GridTemplateColumn HeaderText="Reject Registeration" UniqueName="ButtonColumn2"> 
                                <ItemTemplate>
                                    <telerik:RadButton ID="redSEReqRejectBtn" runat="server" Text="Reject" CommandName="Reject" />
                                </ItemTemplate> 
                            </telerik:GridTemplateColumn>
                           <telerik:GridTemplateColumn HeaderText="View Excel File" UniqueName="ViewExcelFile"> 
                                <ItemTemplate>
                                    <telerik:RadButton ID="redExcelBtn" runat="server" Text="View Excel File" CommandName="ViewExcel"/>
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
             <asp:SqlDataSource ID="SERegReqDS" runat="server" ConnectionString="<%$ ConnectionStrings:TestCS %>" SelectCommand="SELECT em1.EventName[EventName], em1.StartDate[StartDate], em1.EventStatus[EventStatus], em1.OrganisedBy[OrganizedBy], em1.id[id], em2.EventName[ParentEventName] FROM EventMaster em1 INNER JOIN dbo.EventMaster em2 ON em1.parentEventId = em2.id WHERE em1.parentEventID IS NOT null"></asp:SqlDataSource>
         </telerik:RadPageView>
    </telerik:RadMultiPage> 
</asp:Content>

