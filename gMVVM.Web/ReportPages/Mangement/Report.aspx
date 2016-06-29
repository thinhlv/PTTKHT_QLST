<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Report.aspx.cs" Inherits="gMVVM.Web.ReportPages.Report_SQL" %>

<%@ Register assembly="CrystalDecisions.Web, Version=13.0.2000.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" namespace="CrystalDecisions.Web" tagprefix="CR" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <asp:Label ID="Label1" runat="server"></asp:Label>        
        <asp:HyperLink ID="HyperLink1" runat="server"></asp:HyperLink>
        <CR:CrystalReportViewer ID="CrystalReportViewer" runat="server" AutoDataBind="true" OnUnload="CrystalReportViewer_Unload" />
    
    </div>
    </form>
</body>
</html>
