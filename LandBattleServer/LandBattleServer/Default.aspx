<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="LandBattleServer._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="divv">
        <asp:Image ID="Image1" runat="server" BorderWidth="0px" Height="50px" ImageUrl="/Image/Logo.png" Width="50px"/>
        <div id="text">
            Land Battle 1.0<br />Size: ??MB
        </div>
        <p><a href="http://www.asp.net" class="btn btn-primary btn-lg">Download</a></p>
    </div>

</asp:Content>
