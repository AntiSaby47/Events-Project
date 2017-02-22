<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="frmEventsManage_RegisterEvent.aspx.cs" Inherits="frmEventsManage_RegisterEvent" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <style>
        td
        {
            width:300px;
            padding:10px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <table style="width:700px">
        <caption>Register Event</caption>
        <tr>
            <td>Event Name</td>
            <td><asp:TextBox ID="reEventNameTB" runat="server" Width="300px"></asp:TextBox></td>
        </tr>
        <tr>
            <td>Certificate Type</td>
            <td><asp:DropDownList ID="reCertificateTypeDDL" runat="server">
                <asp:ListItem Value="E">Excellence</asp:ListItem>
                <asp:ListItem Value="P">Participation</asp:ListItem>
                <asp:ListItem Value="M">Merit</asp:ListItem>
                <asp:ListItem Value="R">Recognition</asp:ListItem>
                <asp:ListItem Value="T">Training</asp:ListItem>
                </asp:DropDownList></td>
        </tr>
        <tr>
            <td>Participation Year</td>
            <td><asp:TextBox ID="reParticipationYearTB" runat="server" Width="300px"></asp:TextBox></td>
            <asp:RegularExpressionValidator runat="server" ErrorMessage="Enter a valid year" ValidationExpression="[0-9]{4}" ControlToValidate="reParticipationYearTB"></asp:RegularExpressionValidator>
        </tr>
        <tr>
            <td>Start Date</td>
            <td>
                <telerik:RadDatePicker ID="reStartDateDP" runat="server" DateInput-EmptyMessage="MM/DD/YYYY"></telerik:RadDatePicker>
            </td>
        </tr>
        <tr>
            <td>End Date</td>
            <td>
                <telerik:RadDatePicker ID="reEndDateDP" runat="server" DateInput-EmptyMessage="MM/DD/YYYY"></telerik:RadDatePicker>
            </td>
        </tr>
        <tr>
            <td>Event Active</td>
            <td><asp:CheckBox ID="reIsActiveCB" runat="server" Checked="True" /></td>
        </tr>
        <tr>
            <td>Organized by</td>
            <td><asp:TextBox ID="reOrganizedByTB" runat="server" Width="300px"></asp:TextBox></td>
        </tr>
        <tr>
            <td></td><td><telerik:RadButton ID="reSubmitBTN" runat="server" Text="Register Event" OnClick="reSubmitBTN_Click"></telerik:RadButton></td>
        </tr>
    </table>
</asp:Content>

