<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="frmUploadStudentData.aspx.cs" Inherits="frmUploadStudentData" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

     <style>
        td { padding: 10px 30px 10px 30px; }
     </Style>
     <telerik:RadTabStrip ID="RadTabStrip1" runat="server" SelectedIndex="0" MultiPageID="RadMultiPage1">
        <Tabs>
            <telerik:RadTab runat="server" Text="Upload Certificates" Font-Bold="true" Selected="True"></telerik:RadTab>
            <telerik:RadTab runat="server" Text="Search Certificates" Font-Bold="true"></telerik:RadTab>
        </Tabs>
    </telerik:RadTabStrip>
     <telerik:RadMultiPage ID="RadMultiPage1" runat="server" SelectedIndex="0">
        <telerik:RadPageView ID="RadPageView1" runat="server">
            <h4>Student Documents Upload</h4>
         <table style="margin-top:30px">
             <tr>
                 <td>Registration No.</td>
                 <td><asp:TextBox ID="USDRegNotxt" runat="server"></asp:TextBox></td>
             </tr>
             <tr>
                 <td>File Category</td>
                 <td><telerik:RadComboBox ID="USDCertTypeCombo" runat="server"  Height="200" Width="330" EmptyMessage="Choose File Category" >
                     <Items>
                        <telerik:RadComboBoxItem runat="server" Text="Student Admission File" Value="A"/>
                        <telerik:RadComboBoxItem runat="server" Text="Record Cell Certificates(pass out students)" value="R"/>
                        <telerik:RadComboBoxItem runat="server" Text="Event Certificates" Value="E"/>
                     </Items>
                  </telerik:RadComboBox></td>
             </tr>
             <tr>
                 <td>Select</td>
                 <td><asp:FileUpload ID="USDFileUpload" runat="server" AllowMultiple="true"/></td>
             </tr>
             <tr>
                 <td colspan="2"><telerik:RadButton ID="USCUploadBtn" runat="server" Text="Upload" OnClick="USCUploadBtn_Click"></telerik:RadButton></td>
             </tr>
         </table>
        </telerik:RadPageView>
        <telerik:RadPageView ID="RadPageView2" runat="server">
             <h4>Student Documents Search</h4>
            <table id="table1" style="margin-top:30px">
             <tr>
                 <td>Registration No.</td>
                 <td><asp:TextBox ID="USDRegNotxt2" runat="server"></asp:TextBox></td>
             </tr>
             <tr>
                 <td>File Category</td>
                 <td><telerik:RadComboBox ID="USDCertTypeCombo2" runat="server"  Height="200" Width="330" EmptyMessage="Choose File Category" >
                     <Items>
                        <telerik:RadComboBoxItem runat="server" Text="Student Admission File" Value="A"/>
                        <telerik:RadComboBoxItem runat="server" Text="Record Cell Certificates(pass out students)" value="R"/>
                        <telerik:RadComboBoxItem runat="server" Text="Event Certificates" Value="E"/>
                     </Items>
                  </telerik:RadComboBox></td>
             </tr>
             <tr>
                 <td colspan="2">
                     <telerik:RadButton ID="Search" runat="server" Text="Search" OnClick="Search_Click"></telerik:RadButton></td>
             </tr>
            <tr>
                <td colspan="2">
                    <asp:Table ID="USDFilesTbl" runat="server"> 
                        <asp:TableHeaderRow><asp:TableHeaderCell>File</asp:TableHeaderCell><asp:TableHeaderCell>Download</asp:TableHeaderCell></asp:TableHeaderRow>
                    </asp:Table>  
                </td>
            </tr>
            </table>
        </telerik:RadPageView>
     </telerik:RadMultiPage>
</asp:Content>

