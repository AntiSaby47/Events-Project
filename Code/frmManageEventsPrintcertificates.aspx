<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="frmManageEventsPrintCertificates.aspx.cs" Inherits="frmEventsManage_PrintCerts" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
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
            <telerik:RadTab runat="server" Text="View Events" Font-Bold="true" Selected="True"></telerik:RadTab>
            <telerik:RadTab runat="server" Text="Search Certificates" Font-Bold="true" Selected="True"></telerik:RadTab>
            <telerik:RadTab runat="server" Text="Edit Certificate Data" Font-Bold="true"></telerik:RadTab>

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
                        <telerik:GridTemplateColumn HeaderText="View Event Data" UniqueName="ViewExcel"> 
                            <ItemTemplate>
                                <telerik:RadButton ID="repViewExcelBtn" runat="server" Text="View" CommandName="ViewExcel"/>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
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
        <telerik:RadPageView ID="RadPageView2" runat="server">
            <table style="margin-top:30px;">
                <tr>
                    <td>Student Type</td>
                    <td>
                        <asp:RadioButton ID="repLPUStudentRB" Text="LPU Student" GroupName="StudentType" runat="server" AutoPostBack="true" OnCheckedChanged="StudentType_CheckedChanged"/>
                        <asp:RadioButton ID="repOutsiderRB" Text="Outside Student" GroupName="StudentType" runat="server" AutoPostBack="true" OnCheckedChanged="StudentType_CheckedChanged" />
                    </td>
                </tr>
                <tr runat="server" id="rowRegNo" visible="false">
                    <td>Registration Number</td>
                    <td><asp:TextBox ID="repRegNoTB" runat="server"></asp:TextBox></td>
                </tr>
                <tr runat="server" id="rowCertNo" visible="false">
                    <td>Certificate Number</td>
                    <td><asp:TextBox ID="repCertNoTB" runat="server"></asp:TextBox></td>
                </tr>
                <tr runat="server" id="rowButton" visible="false">
                    <td></td>
                    <td><telerik:RadButton ID="repFindCertBtn" runat="server" Text="Submit" OnClick="repFindCertBtn_Click" AutoPostBack="true"></telerik:RadButton></td>
                </tr>
            </table>
            <telerik:RadGrid ID="repCertInfoRG" runat="server" AllowFilteringByColumn="True" AllowPaging="True" AllowSorting="True" GroupPanelPosition="Top" Visible="false" OnItemCommand="repCertInfoRG_ItemCommand">
                <GroupingSettings CollapseAllTooltip="Collapse all groups" />
                <MasterTableView AutoGenerateColumns="False">
                    <Columns>
                        <telerik:GridBoundColumn DataField="StudentName" FilterControlAltText="Filter StudentName column" HeaderText="Student Name" SortExpression="StudentName" UniqueName="StudentName"></telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="ParentEventName" FilterControlAltText="Filter ParentEventName column" HeaderText="Parent Event Name" SortExpression="ParentEventName" UniqueName="ParentEventName"></telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="EventName" FilterControlAltText="Filter EventName column" HeaderText="Event Name" SortExpression="EventName" UniqueName="EventName"></telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="Position" FilterControlAltText="Filter Position column" HeaderText="Position" SortExpression="Position" UniqueName="Position"></telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="StartDate" DataType="System.DateTime" DataFormatString="{0:dd/MM/yyyy}" FilterControlAltText="Filter StartDate column" HeaderText="Start Date" SortExpression="StartDate" UniqueName="StartDate"></telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="EndDate" DataType="System.DateTime" DataFormatString="{0:dd/MM/yyyy}" FilterControlAltText="Filter EndDate column" HeaderText="End Date" SortExpression="EndDate" UniqueName="EndDate"></telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="OrganisedBy" FilterControlAltText="Filter OrganisedBy column" HeaderText="OrganisedBy" SortExpression="Organised By" UniqueName="OrganisedBy"></telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="CertificateType" FilterControlAltText="Filter CertificateType column" HeaderText="CertificateType" SortExpression="Certificate Type" UniqueName="CertificateType"></telerik:GridBoundColumn>
                        <telerik:GridTemplateColumn HeaderText="Download Sample" UniqueName="DownloadSample"> 
                            <ItemTemplate>
                                <telerik:RadButton runat="server" Text="Download Sample" CommandName="DownloadSample"/>
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
        </telerik:RadPageView>
        <telerik:RadPageView ID="RadPageView3" runat="server">
            <table>
                <tr>
                    <td>Select School: </td>
                    <td><telerik:RadComboBox ID="MEPCEditLoadSchoolsCB" runat="server" AutoPostBack="true" Width="350" EmptyMessage="Choose Your School" OnSelectedIndexChanged="MEPCEditLoadSchoolsCB_SelectedIndexChanged"></telerik:RadComboBox></td>
                </tr>
                <tr>
                    <td>Select Event: </td>
                    <td><telerik:RadComboBox ID="MEPCEditLoadEventsCB" runat="server" Width="350" EmptyMessage="Choose Event"></telerik:RadComboBox></td>
                </tr>
                <tr runat="server" id="udrowDDL" visible="false">
                    <td>Certificate Format: </td>
                    <td>
                        <asp:DropDownList ID="MEPCEditCertFormatsDDL" runat="server" Width="250">
                            <asp:ListItem Value="None">Select Format</asp:ListItem>
                            <asp:ListItem Value="P">Certificate of Participation</asp:ListItem>
                            <asp:ListItem Value="M">Certificate of Merit</asp:ListItem>
                            <asp:ListItem Value="E">Certificate of Excellence</asp:ListItem>
                            <asp:ListItem Value="R">Certificate of Recognition</asp:ListItem>
                            <asp:ListItem Value="T">Certificate of Training</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <telerik:RadButton ID="MEPCEditSearchBtn" runat="server" Text="Search" OnClick="MEPCEditSearchBtn_Click"></telerik:RadButton>
                    </td>
                </tr>
            </table>    <!--style="margin-top:30px"-->
                <telerik:RadGrid ID="MEPCEditStudentDataRG" runat="server" AllowFilteringByColumn="True" AllowPaging="True" AllowSorting="True" GroupPanelPosition="Top" OnItemCommand="MEPCEditStudentDataRG_ItemCommand" CellSpacing="-1" GridLines="Both">
                    <GroupingSettings CollapseAllTooltip="Collapse all groups" />
                    <MasterTableView AutoGenerateColumns="False">
                        <Columns>
                            <telerik:GridBoundColumn DataField="CertificateNumber" FilterControlAltText="Filter CertificateNumber column" HeaderText="CertificateNumber" SortExpression="CertificateNumber" UniqueName="CertificateNumber" DataType="System.Int32" ReadOnly="True"></telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="certificateType" FilterControlAltText="Filter certificateType column" HeaderText="certificateType" SortExpression="certificateType" UniqueName="certificateType" ReadOnly="True"></telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="StudentName" FilterControlAltText="Filter StudentName column" HeaderText="StudentName" SortExpression="StudentName" UniqueName="StudentName" ReadOnly="True"></telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="FatherName" FilterControlAltText="Filter FatherName column" HeaderText="FatherName" SortExpression="FatherName" UniqueName="FatherName" ReadOnly="True"></telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="HusbandName" FilterControlAltText="Filter HusbandName column" HeaderText="HusbandName" SortExpression="HusbandName" UniqueName="HusbandName" ReadOnly="True"></telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="CollegeOrSchool" FilterControlAltText="Filter CollegeOrSchool column" HeaderText="CollegeOrSchool" SortExpression="CollegeOrSchool" UniqueName="CollegeOrSchool" ReadOnly="True"></telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="RegisterationNumber" FilterControlAltText="Filter RegisterationNumber column" HeaderText="RegisterationNumber" SortExpression="RegisterationNumber" UniqueName="RegisterationNumber" DataType="System.Int64" ReadOnly="True"></telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="ProgramName" FilterControlAltText="Filter ProgramName column" HeaderText="ProgramName" SortExpression="ProgramName" UniqueName="ProgramName" ReadOnly="True"></telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="Position" FilterControlAltText="Filter Position column" HeaderText="Position" SortExpression="Position" UniqueName="Position" ReadOnly="True"></telerik:GridBoundColumn>
                            <telerik:GridCheckBoxColumn DataField="IsLPUStudent" DataType="System.Boolean" FilterControlAltText="Filter IsLPUStudent column" HeaderText="IsLPUStudent" ReadOnly="True" SortExpression="IsLPUStudent" UniqueName="IsLPUStudent">
                            </telerik:GridCheckBoxColumn>
                            <telerik:GridBoundColumn DataField="StartDate" DataType="System.DateTime" FilterControlAltText="Filter StartDate column" HeaderText="StartDate" SortExpression="StartDate" UniqueName="StartDate" ReadOnly="True"></telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="EndDate" FilterControlAltText="Filter EndDate column" HeaderText="EndDate" ReadOnly="True" SortExpression="EndDate" UniqueName="EndDate" DataType="System.DateTime"></telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="EventID" FilterControlAltText="Filter EventID column" HeaderText="EventID" SortExpression="EventID" UniqueName="EventID" ReadOnly="True"></telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="eventCategoryID" UniqueName="eventCategoryID" FilterControlAltText="Filter eventCategoryID column" HeaderText="eventCategoryID" ReadOnly="True" SortExpression="eventCategoryID"></telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="OrganisedBy" FilterControlAltText="Filter OrganisedBy column" HeaderText="OrganisedBy" ReadOnly="True" SortExpression="OrganisedBy" UniqueName="OrganisedBy">
                            </telerik:GridBoundColumn>
                        </Columns>
                    </MasterTableView>
                </telerik:RadGrid>
                
                <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:NewUmsConnectionString %>" SelectCommand="pEventCertificateData" SelectCommandType="StoredProcedure">
                    <SelectParameters>
                        <asp:Parameter DefaultValue="188" Name="eid" Type="String" />
                    </SelectParameters>
            </asp:SqlDataSource>
                
                <asp:Panel ID="PopupPanel" runat="server" CssClass="modalPopup" style = "display:none">
                    <telerik:RadButton ID="ButtonOk" runat="server" Text="OK" />
                    <telerik:RadButton ID="ButtonCancel" runat="server" Text="Cancel" />
                </asp:Panel>
                <ajaxToolkit:ModalPopupExtender ID="PopUp" runat="server"
                    Enabled="True" TargetControlID="tempButton" PopupControlID="PopupPanel" BackgroundCssClass="modalBackground"
                    CancelControlID="ButtonCancel">
                </ajaxToolkit:ModalPopupExtender>
                <telerik:RadButton ID="tempButton" runat="server" style="display:none"/>
        </telerik:RadPageView>
    </telerik:RadMultiPage> 
</asp:Content>

