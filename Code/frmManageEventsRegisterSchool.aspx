<%@ Page Title="" Language="C#" MasterPageFile="Masterpage.master" AutoEventWireup="true" CodeFile="frmManageEventsRegisterSchool.aspx.cs" Inherits="frmRegisterEventSchool" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <style>
        td { padding: 10px 30px 10px 10px;}
    </style>
    <telerik:RadTabStrip ID="RadTabStrip1" runat="server" SelectedIndex="0" MultiPageID="RadMultiPage1">
        <Tabs>
            <telerik:RadTab runat="server" Text="Initiate Event" Font-Bold="true" Selected="True"></telerik:RadTab>
            <telerik:RadTab runat="server" Text="Check Event Status" Font-Bold="true"></telerik:RadTab>
            <telerik:RadTab runat="server" Text="Upload Certificate Data" Font-Bold="true"></telerik:RadTab>
        </Tabs>
    </telerik:RadTabStrip>

    <telerik:RadMultiPage ID="RadMultiPage1" runat="server" SelectedIndex="0">
        <telerik:RadPageView ID="RadPageView1" runat="server">
            <div style="margin-top:30px">
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
                        <td><telerik:RadComboBox ID="resEOrganizedBy" runat="server" AutoPostBack="true" Width="250" EmptyMessage="Choose Your School"></telerik:RadComboBox></td>
                    </tr>
                    <tr><td colspan="2"><telerik:RadButton runat="server" Text="Register" OnClick="resRegisterBtn_Click"></telerik:RadButton></td></tr>
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
                        <td><telerik:RadComboBox ID="resSEOrganizedByCombo" runat="server"  AutoPostBack="true" Width="250" EmptyMessage="Choose Your School"></telerik:RadComboBox></td>
                    </tr>
                    <tr>
                        <td colspan="2"><telerik:RadButton runat="server" Text="Register" OnClick="resRegisterBtn_Click"></telerik:RadButton></td>
                    </tr>
                </table>
            </asp:Panel>
            </div>
            </telerik:RadPageView>
        <telerik:RadPageView ID="RadPageView2" runat="server">
            <div style="padding:30px;">
                Select School: <telerik:RadComboBox ID="SelectSchool" runat="server" AutoPostBack="true" Width="350" EmptyMessage="Choose Your School" OnSelectedIndexChanged="SelectSchool_SelectedIndexChanged"></telerik:RadComboBox>
                <telerik:RadGrid style="margin-top:30px" ID="resStatusRG" runat="server" CellSpacing="-1" GridLines="Both" GroupPanelPosition="Top" OnItemCommand="resStatusRG_ItemCommand" OnItemDataBound="resStatusRG_ItemDataBound" ShowGroupPanel="True" Visible="false">
                    <GroupingSettings CollapseAllTooltip="Collapse all groups" />
                    <ClientSettings AllowDragToGroup="True">
                    </ClientSettings>
                    <MasterTableView AutoGenerateColumns="False" >
                        <Columns>
                            <telerik:GridBoundColumn DataField="id" DataType="System.Int32" FilterControlAltText="Filter id column" HeaderText="Event ID" ReadOnly="True" SortExpression="id" UniqueName="id">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="EventName" FilterControlAltText="Filter EventName column" HeaderText="Event Name" SortExpression="EventName" UniqueName="EventName">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="StartDate" DataFormatString="{0:dd/MM/yyyy}" DataType="System.DateTime" FilterControlAltText="Filter StartDate column" HeaderText="Start Date" SortExpression="StartDate" UniqueName="StartDate">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="EventStatus" FilterControlAltText="Filter Active column" HeaderText="Event Status" SortExpression="EventStatus" UniqueName="EventStatus">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="OrganizedBy" Display="false" UniqueName="OrganizedBy">
                            </telerik:GridBoundColumn>
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
            </div>
        </telerik:RadPageView>
        <telerik:RadPageView ID="RadPageView3" runat="server">
            <table style="margin-top:30px">
                <tr>
                    <td>Select Event Type</td>
                    <td>
                        <telerik:RadComboBox ID="resUDEventTypeCB" runat="server" AutoPostBack="true" Width="300" EmptyMessage="Choose Event Category" OnSelectedIndexChanged="resUDEventTypeCB_SelectedIndexChanged">
                            <Items>
                                <telerik:RadComboBoxItem runat="server" Text="Event"/>
                                <telerik:RadComboBoxItem runat="server" Text="Sub Event"/>
                            </Items>
                        </telerik:RadComboBox>
                    </td>
                </tr>
                <tr runat="server" id="udrowEvent" visible="false">
                    <td>Event</td>
                    <td><telerik:RadComboBox ID="resUDEventCB" runat="server" AutoPostBack="true" Width="300" EmptyMessage="Choose Event" OnSelectedIndexChanged="resUDEventCB_SelectedIndexChanged"></telerik:RadComboBox></td>
                </tr>
                <tr runat="server" id="udrowSubEvent" visible="false">
                    <td>Sub-Event</td>
                    <td><telerik:RadComboBox ID="resUDSubEventCB" runat="server" AutoPostBack="true" Width="300" EmptyMessage="Choose Sub-Event"></telerik:RadComboBox></td>
                </tr>
                <tr runat="server" id="udrowDDL" visible="false">
                    <td>Certificate Format</td>
                    <td>
                        <asp:DropDownList ID="resUDCertFormatsDDL" runat="server">
                            <asp:ListItem Value="None">Select Format</asp:ListItem>
                            <asp:ListItem Value="P">Certificate of Participation</asp:ListItem>
                            <asp:ListItem Value="M">Certificate of Merit</asp:ListItem>
                            <asp:ListItem Value="E">Certificate of Excellence</asp:ListItem>
                            <asp:ListItem Value="R">Certificate of Recognition</asp:ListItem>
                            <asp:ListItem Value="T">Certificate of Training</asp:ListItem>
                        </asp:DropDownList>
                        <telerik:RadButton id="resDownloadFormatsBtn" runat="server" Text="Download Formats" OnClick="resDownloadFormatsBtn_Click"></telerik:RadButton>
                    </td>
                </tr>
                <tr runat="server" id="udrowFU" visible="false">
                    <td>Select Data File</td>
                    <td><asp:FileUpload runat="server" id="resUDDataFileFU" /></td>
                </tr>
                <tr  runat="server" id="udrowBtn" visible="false"><td></td><td><telerik:RadButton id="resUDUploadBtn" runat="server" Text="Upload" OnClick="resUDUploadBtn_Click"></telerik:RadButton></td></tr>
            </table>
        </telerik:RadPageView>
     </telerik:RadMultiPage>
</asp:Content>

