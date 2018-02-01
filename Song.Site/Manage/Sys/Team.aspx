﻿<%@ Page Language="C#" MasterPageFile="~/Manage/ManagePage.Master" AutoEventWireup="true"
    Codebehind="Team.aspx.cs" Inherits="Song.Site.Manage.Sys.Team" Title="员工列表" %>

<%@ Register Src="../Utility/toolsBar.ascx" TagName="toolsBar" TagPrefix="uc1" %>
<%@ Register Src="../Utility/Pager.ascx" TagName="Pager" TagPrefix="uc2" %>
<%@ Register Assembly="WeiSha.WebControl" Namespace="WeiSha.WebControl" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphMain" runat="server">
    <div id="header">
        <uc1:toolsBar ID="ToolsBar1" runat="server" WinPath="Team_Edit.aspx" GvName="GridView1"
            WinWidth="600" WinHeight="400" OnDelete="DeleteEvent"/>
        <div class="searchBox">
            <cc1:DropDownTree ID="ddlDepart" runat="server" Width="150" IdKeyName="dep_id"
                ParentIdKeyName="dep_PatId" TaxKeyName="dep_Tax">
            </cc1:DropDownTree>           
            名称：<asp:TextBox ID="tbSear" runat="server" Width="115" MaxLength="10"></asp:TextBox>&nbsp;<asp:Button ID="btnSear" runat="server" Width="100"
                    Text="查询" OnClick="btnsear_Click" />
        </div>
    </div>
    <cc1:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" SelectBoxKeyName="SelectBox"
        ShowSelectBox="True">
        <EmptyDataTemplate>
            没有满足条件的信息！
        </EmptyDataTemplate>
        <Columns>
            <asp:TemplateField HeaderText="序号">
                <itemstyle cssclass="center" width="40" />
                <itemtemplate>
<%# Container.DataItemIndex   + Pager1.Size*(Pager1.Index-1) + 1 %>
</itemtemplate>
            </asp:TemplateField>
             <asp:TemplateField HeaderText="操作">
                <itemtemplate>
<cc1:RowDelete id="btnDel" onclick="btnDel_Click" runat="server"></cc1:RowDelete> 
<cc1:RowEdit id="btnEdit" runat="server" ></cc1:RowEdit> 
</itemtemplate>
                <itemstyle cssclass="center" width="44px" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="ID">
                <itemstyle cssclass="center" width="60px" />
                <itemtemplate>
<%# Eval("team_id","{0}")%> 
</itemtemplate>
            </asp:TemplateField>
           
            <asp:TemplateField HeaderText="工作组">
                <itemstyle cssclass="center" />
                <itemtemplate>
               
<%# Eval("team_name", "{0}")%>
</itemtemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="所在院系">
                <itemstyle cssclass="center" />
                <itemtemplate>
               
<%# Eval("Dep_Name", "{0}")%>
</itemtemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="专业">
                <itemstyle cssclass="center" />
                <itemtemplate>
               
<%# Eval("sbj_Name", "{0}")%>
</itemtemplate>
            </asp:TemplateField>
            
        </Columns>
    </cc1:GridView>
    <br />
    <uc2:Pager ID="Pager1" runat="server" Size="15" OnPageChanged="BindData" />
</asp:Content>
