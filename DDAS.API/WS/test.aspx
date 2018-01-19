<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="test.aspx.cs" Inherits="DDAS.API.WS.test" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:TextBox runat="server" ID="txtBody" TextMode="MultiLine" Rows="10"></asp:TextBox>
            <asp:TextBox runat="server" ID="txtMode"></asp:TextBox>
            <asp:TextBox runat="server" ID="txtRecid"></asp:TextBox>
        </div>
        <div>
                
            <asp:Button runat="server" ID="btnTest" Text="test" OnClick="btnTest_Click" />
        </div>
    </form>
</body>
</html>
