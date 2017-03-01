<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="frmRegisterEventSchool.aspx.cs" Inherits="frmRegisterEventSchool" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <style>
        td { padding: 10px 30px 10px 30px; }
    </style>
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
        </table>
    </asp:Panel>
    </div>
    <div>
    <asp:Panel id="resSEventPanel" Visible="false" runat="server">
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
        </table>
    </asp:Panel>
        <telerik:RadButton ID="RadButton1" runat="server" Text="RadButton" OnClick="RadButton1_Click"></telerik:RadButton>
        </div>
</asp:Content>

