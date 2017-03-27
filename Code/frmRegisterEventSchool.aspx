<%@ Page Title="" Language="C#" MasterPageFile="MasterPage.master" AutoEventWireup="true" CodeFile="frmRegisterEventSchool.aspx.cs" Inherits="frmRegisterEventSchool" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <style>
        td { padding: 10px 30px 10px 30px; }
    </style>
    <telerik:RadTabStrip ID="RadTabStrip1" runat="server" SelectedIndex="0" MultiPageID="RadMultiPage1">
        <Tabs>
            <telerik:RadTab runat="server" Text="Initiate Event" Font-Bold="true" Selected="True"></telerik:RadTab>
            <telerik:RadTab runat="server" Text="Check Event Status" Font-Bold="true"></telerik:RadTab>
        </Tabs>
    </telerik:RadTabStrip>

    <telerik:RadMultiPage ID="RadMultiPage1" runat="server" SelectedIndex="0">
        <telerik:RadPageView ID="RadPageView1" runat="server">

            <h3>Event Registeration</h3>
            <div>
                <table>
                    <tr>
                        <td>Select Event Type</td>
                        <td>
                            <telerik:RadComboBox ID="resETypeCombo" runat="server" OnSelectedIndexChanged="resETypeCombo_SelectedIndexChanged" AutoPostBack="true" Width="300" EmptyMessage="Choose Event Category"><Items>
                                    <telerik:RadComboBoxItem runat="server" Text="Event"/>
                                    <telerik:RadComboBoxItem runat="server" Text="Sub Event"/>
                            </Items></telerik:RadComboBox>
                        </td>
                    </tr>
                </table>
            </div>
            <div>
            <asp:Panel id="resEventPanel" Visible="false" runat="server">
                <table style="margin-top:30px;">
                    <tr>
                        <td>Event Name</td>
                        <td><asp:TextBox ID="resENametxt" runat="server"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td>Start Date</td>
                        <td><telerik:RadDatePicker ID="resESDateDP" runat="server"></telerik:RadDatePicker></td>
                    </tr>
                    <tr>
                        <td>End Date</td>
                        <td><telerik:RadDatePicker ID="resEEDateDP" runat="server"></telerik:RadDatePicker></td>
                    </tr>
                    <tr>
                        <td>Organized By</td>
                        <td><asp:TextBox ID="resEOrganizedBy" runat="server"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td colspan="2"><telerik:RadButton runat="server" Text="Register" OnClick="resRegisterBtn_Click"></telerik:RadButton></td>
                    </tr>
                </table>
            </asp:Panel>
            </div>
            <div>
            <asp:Panel id="resSEventPanel" Style="left: 50%;" Visible="false" runat="server">
                <table style="margin-top:30px;">
                    <tr>
                        <td>Event Name</td>
                        <td><telerik:RadComboBox ID="resENameCombo" runat="server" Height="200" Width="375" EmptyMessage="Choose Event" MarkFirstMatch="true" EnableLoadOnDemand="true" ></telerik:RadComboBox></td>
                    </tr>
                    <tr>
                        <td>Sub Event Name</td>
                        <td><asp:TextBox ID="resSENameTxt" runat="server"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td>Organized By</td>
                        <td><asp:TextBox ID="resSEOrganizedByTxt" runat="server"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td colspan="2"><telerik:RadButton runat="server" Text="Register" OnClick="resRegisterBtn_Click"></telerik:RadButton></td>
                    </tr>
                </table>
            </asp:Panel>
            </div>
            </telerik:RadPageView>
        <telerik:RadPageView ID="RadPageView2" runat="server">
            <h3>Check Event Status</h3>
            <telerik:RadGrid ID="RadGrid1" runat="server" CellSpacing="-1" DataSourceID="EventStatusDS" GridLines="Both" GroupPanelPosition="Top">
                <GroupingSettings CollapseAllTooltip="Collapse all groups" />
                <MasterTableView AutoGenerateColumns="False" DataSourceID="EventStatusDS">
                    <Columns>
                        <telerik:GridBoundColumn DataField="EventName" FilterControlAltText="Filter EventName column" HeaderText="EventName" SortExpression="EventName" UniqueName="EventName">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="StartDate" DataType="System.DateTime" FilterControlAltText="Filter StartDate column" HeaderText="StartDate" SortExpression="StartDate" UniqueName="StartDate">
                        </telerik:GridBoundColumn>
                        <telerik:GridCheckBoxColumn DataField="Active" DataType="System.Boolean" FilterControlAltText="Filter Active column" HeaderText="Active" SortExpression="Active" UniqueName="Active">
                        </telerik:GridCheckBoxColumn>
                        <telerik:GridBoundColumn DataField="OrganizedBy" FilterControlAltText="Filter OrganizedBy column" HeaderText="OrganizedBy" SortExpression="OrganizedBy" UniqueName="OrganizedBy">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="id" DataType="System.Int32" FilterControlAltText="Filter id column" HeaderText="id" ReadOnly="True" SortExpression="id" UniqueName="id">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="EventName1" FilterControlAltText="Filter EventName1 column" HeaderText="EventName1" SortExpression="EventName1" UniqueName="EventName1">
                        </telerik:GridBoundColumn>
                    </Columns>
                </MasterTableView>
            </telerik:RadGrid>
            <asp:SqlDataSource ID="EventStatusDS" runat="server" ConnectionString="<%$ ConnectionStrings:TestCS %>" SelectCommand="SELECT em1.EventName[EventName], em1.StartDate[StartDate], em1.Active[Active], em1.OrganisedBy[OrganizedBy], em1.id[id], em2.EventName FROM EventMaster em1 INNER JOIN dbo.EventMaster em2 ON em1.id = em2.id 
WHERE em1.parentEventID IS NOT null AND em1.Active = 0 AND em1.id=em2.ParentEventID"></asp:SqlDataSource>
        </telerik:RadPageView>
     </telerik:RadMultiPage>
</asp:Content>

