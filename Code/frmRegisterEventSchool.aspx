<%@ Page Title="" Language="C#" MasterPageFile="MasterPage.master" AutoEventWireup="true" CodeFile="frmRegisterEventSchool.aspx.cs" Inherits="frmRegisterEventSchool" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <style>
        td { padding: 10px 30px 10px 30px; }
        .modalBackground
        {
            position: absolute;
            z-index: 100;
            top: 0px;
            left: 0px;
            background-color: #000;
            filter: alpha(opacity=60);
            -moz-opacity: 0.6;
            opacity: 0.6;
        }
        .modalPopup
        {
            background-color: #FFFFFF;
            border-width: 3px;
            border-style: solid;
            border-color: black;
            padding-top: 10px;
            padding-left: 10px;
            width: 300px;
            height: 140px;
        }
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
            <telerik:RadGrid ID="resStatusRG" runat="server" CellSpacing="-1" DataSourceID="EventStatusDS" GridLines="Both" GroupPanelPosition="Top" OnItemCommand="resStatusRG_ItemCommand" OnItemDataBound="resStatusRG_ItemDataBound" ShowGroupPanel="True">
                <GroupingSettings CollapseAllTooltip="Collapse all groups" />
                <ClientSettings AllowDragToGroup="True">
                </ClientSettings>
                <MasterTableView AutoGenerateColumns="False" DataSourceID="EventStatusDS">
                    
                    <Columns>
                        <telerik:GridBoundColumn DataField="id" DataType="System.Int32" FilterControlAltText="Filter id column" HeaderText="Event ID" ReadOnly="True" SortExpression="id" UniqueName="id">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="EventName" FilterControlAltText="Filter EventName column" HeaderText="Event Name" SortExpression="EventName" UniqueName="EventName">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="StartDate" DataType="System.DateTime" FilterControlAltText="Filter StartDate column" HeaderText="StartDate" SortExpression="StartDate" UniqueName="StartDate">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="EventStatus" DataType="System.Int32" FilterControlAltText="Filter Active column" HeaderText="Event Status" SortExpression="EventStatus" UniqueName="EventStatus">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="OrganizedBy" FilterControlAltText="Filter OrganizedBy column" HeaderText="Organized By" SortExpression="OrganizedBy" UniqueName="OrganizedBy">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="ParentEventName" FilterControlAltText="Filter ParentEventName column" HeaderText="Parent Event Name" SortExpression="ParentEventName" UniqueName="ParentEventName">
                        </telerik:GridBoundColumn>
                        <telerik:GridTemplateColumn HeaderText="Upload Excel File" UniqueName="UploadExcelFile"> 
                                <ItemTemplate>
                                    <telerik:RadButton ID="resExcelBtn" runat="server" Text="Upload Excel File" CommandName="UploadExcel"/>
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
            <asp:Panel ID="PopupPanel" runat="server" CssClass="modalPopup" style = "display:none">
                <asp:FileUpload ID="resImageFU" runat="server" />
                <br />
                <telerik:RadButton ID="ButtonOk" runat="server" Text="OK" OnClick="ButtonOk_Click" />
                <telerik:RadButton ID="ButtonCancel" runat="server" Text="Cancel" />
            </asp:Panel>
            <ajaxToolkit:ModalPopupExtender ID="resPopUp" runat="server"
                Enabled="True" TargetControlID="tempButton" PopupControlID="PopupPanel" BackgroundCssClass="modalBackground"
                CancelControlID="ButtonCancel">
            </ajaxToolkit:ModalPopupExtender>

            <telerik:RadButton ID="tempButton" runat="server" style="display:none"/>
            <asp:SqlDataSource ID="EventStatusDS" runat="server" ConnectionString="<%$ ConnectionStrings:TestCS %>" SelectCommand="SELECT em1.EventName[EventName], em1.StartDate[StartDate], em1.EventStatus[EventStatus], em1.OrganisedBy[OrganizedBy], em1.id[id], em2.EventName[ParentEventName] FROM EventMaster em1 INNER JOIN dbo.EventMaster em2 ON em1.parentEventId = em2.id "></asp:SqlDataSource>
        </telerik:RadPageView>
     </telerik:RadMultiPage>
</asp:Content>

