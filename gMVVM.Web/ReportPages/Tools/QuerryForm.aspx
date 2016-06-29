<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="QuerryForm.aspx.cs" Inherits="Web.Tools.QuerryForm" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
       
    
    </div>
        <p>
            <asp:TextBox ID="TextBox1" runat="server" Height="234px" TextMode="MultiLine" ToolTip="Nhập chuổi truy vấn" Width="876px"></asp:TextBox>
        </p>
        <p>
            <asp:Label ID="Label1" runat="server" Text="Label"></asp:Label>
        </p>
        <p>
            <asp:Button ID="Button1" runat="server" OnClick="Button1_Click1" Text="Execute" />
            <asp:Button ID="Button2" runat="server" OnClick="Button2_Click1" Text="Clear" />
        </p>

        
        <asp:Panel ID="Panel1" runat="server">
            <asp:GridView ID="GridView1" runat="server" Width="865px">
            </asp:GridView>
        </asp:Panel>
    </form>
</body>
</html>
