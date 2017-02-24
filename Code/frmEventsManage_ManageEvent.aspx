<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="frmEventsManage_ManageEvent.aspx.cs" Inherits="frmEventsManage_ManageEvent" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <style>
        td
        {
            width:300px;
            padding:10px;
        }
        .modalBackground
        {
            background-color: Black;
            filter: alpha(opacity=60);
            opacity: 0.6;
        }
        .modalPopup
        {
            background-color: #FFFFFF;
            width: 300px;
            border: 3px solid #0DA9D0;
            border-radius: 12px;
            padding:0
      
        }
        .modalPopup .header
        {
            background-color: #2FBDF1;
            height: 30px;
            color: White;
            line-height: 30px;
            text-align: center;
            font-weight: bold;
            border-top-left-radius: 6px;
            border-top-right-radius: 6px;
        }
        .modalPopup .body
        {
            min-height: 50px;
            line-height: 30px;
            text-align: center;
            font-weight: bold;
        }
        .modalPopup .footer
        {
            padding: 6px;
        }
        .modalPopup .yes, .modalPopup .no
        {
            height: 23px;
            color: White;
            line-height: 23px;
            text-align: center;
            font-weight: bold;
            cursor: pointer;
            border-radius: 4px;
        }
        .modalPopup .yes
        {
            background-color: #2FBDF1;
            border: 1px solid #0DA9D0;
        }
        .modalPopup .no
        {
            background-color: #9F9F9F;
            border: 1px solid #5C5C5C;
        }
 
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

    <table style="width:700px">
        <caption>Manage Events</caption>
        <tr>
            <td>Event name</td>
            <td><asp:TextBox ID="meEventNameTB" runat="server" Width="300px"></asp:TextBox></td>
        </tr>
        <tr>
            <td>Event start date</td>
            <td><telerik:RadDatePicker ID="meStartDateDP" runat="server" DateInput-EmptyMessage="MM/DD/YYYY"></telerik:RadDatePicker></td>
        </tr>
        <tr>
            <td></td><td><telerik:RadButton ID="meSubmitDateBTN" runat="server" Text="Find Event" OnClick="meSubmitDateBTN_Click"></telerik:RadButton></td>
        </tr>
    </table>
    <br />
    <table style="width:700px;display:none;" id="uploadTable" runat="server">
        <tr>
            <td>Upload Excel Data</td>
            <td>
                <asp:FileUpload ID="meExcelFU" runat="server" />
            </td>
        </tr>
        <tr>
            <td></td><td><telerik:RadButton ID="meUploadExcelBTN" runat="server" Text="Upload" OnClick="meUploadExcelBTN_Click"></telerik:RadButton></td>
        </tr>

    </table>


</asp:Content>

