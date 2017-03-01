<%@ Page  MasterPageFile="~/MasterPage.master" Language="C#" AutoEventWireup="true" CodeFile="frmRegisterEventDSA.aspx.cs" Inherits="frmRegisterEventDSA" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    
    <style>
        td { padding: 10px 30px 10px 30px; }
    </style>

    <telerik:RadTabStrip ID="RadTabStrip1" runat="server" SelectedIndex="1" MultiPageID="RadMultiPage1">
        <Tabs>
            <telerik:RadTab runat="server" Text="Initiate Event" Font-Bold="true"></telerik:RadTab>
            <telerik:RadTab runat="server" Text="Check Event Invites" Font-Bold="true" Selected="True"></telerik:RadTab>
        </Tabs>
    </telerik:RadTabStrip>

    <telerik:RadMultiPage ID="RadMultiPage1" runat="server" SelectedIndex="1">
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
               <telerik:RadGrid ID="redSERegRequestRG" runat="server" AllowFilteringByColumn="True" AllowPaging="True" AllowSorting="True" DataSourceID="SERegReqDS" GroupPanelPosition="Top">
                   <GroupingSettings CollapseAllTooltip="Collapse all groups" />
                   <MasterTableView AutoGenerateColumns="False" DataSourceID="SERegReqDS">
                       <Columns>
                           <telerik:GridBoundColumn DataField="EventName" FilterControlAltText="Filter EventName column" HeaderText="Event Name" SortExpression="EventName" UniqueName="EventName">
                           </telerik:GridBoundColumn>
                           <telerik:GridBoundColumn DataField="StartDate" DataType="System.DateTime" DataFormatString="{0:dd/MM/yyyy}" FilterControlAltText="Filter StartDate column" HeaderText="Start Date" SortExpression="StartDate" UniqueName="StartDate">
                           </telerik:GridBoundColumn>
                           <telerik:GridCheckBoxColumn DataField="Active" DataType="System.Boolean" FilterControlAltText="Filter Active column" HeaderText="Active" SortExpression="Active" UniqueName="Active">
                           </telerik:GridCheckBoxColumn>
                           <telerik:GridBoundColumn DataField="OrganisedBy" FilterControlAltText="Filter OrganisedBy column" HeaderText="OrganisedBy" SortExpression="Organised By" UniqueName="OrganisedBy">
                           </telerik:GridBoundColumn>

                           <telerik:GridTemplateColumn HeaderText="Approve Registeration" UniqueName="ButtonColumn1"> 
                                <ItemTemplate>
                                    <telerik:RadButton ID="redSEReqAllowBtn" runat="server" Text="Approve" CommandName="IssueCertificate" />
                                </ItemTemplate>
                           </telerik:GridTemplateColumn>
                           <telerik:GridTemplateColumn HeaderText="Reject Registeration" UniqueName="ButtonColumn2"> 
                                <ItemTemplate>
                                    <telerik:RadButton ID="redSEReqRejectBtn" runat="server" Text="Reject" CommandName="IssueCertificate" />
                                </ItemTemplate> 
                            </telerik:GridTemplateColumn>

                       </Columns>
                   </MasterTableView>
               </telerik:RadGrid>
               <asp:SqlDataSource ID="SERegReqDS" runat="server" ConnectionString="<%$ ConnectionStrings:NewUmsConnectionString %>" SelectCommand="SELECT EventName, StartDate, Active, OrganisedBy FROM EventMaster WHERE parentEventID is not null AND (Active = 1) AND StartDate>=GETDATE();"></asp:SqlDataSource>
         </telerik:RadPageView>
    </telerik:RadMultiPage> 
</asp:Content>

