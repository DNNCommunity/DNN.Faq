<%@ Control Language="C#" Inherits="DotNetNuke.Modules.FAQs.Settings" AutoEventWireup="true" CodeBehind="Settings.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="SectionHead" Src="~/controls/SectionHeadControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<div class="dnnForm dnnFAQsSettings dnnClear">
    <div class="dnnFormItem">
    	<dnn:Label ID="lblShowCatagories" ControlName="chkShowCatagories" runat="server"/>
        <asp:CheckBox ID="chkShowCatagories" runat="server" AutoPostBack="True" OnCheckedChanged="chkShowCatagories_CheckedChanged" />
    </div>
	<div class="dnnFormItem">
		<dnn:Label ID="lblShowEmptyCategories" ControlName="chkShowEmptyCategories" runat="server" />
        <asp:CheckBox ID="chkShowEmptyCategories" runat="server" />
    </div>
    <asp:Panel runat="server" ID="pnlShowCategoryType">
        <div class="dnnFormItem">
            <dnn:Label ID="lblShowToolTips" ControlName="chkShowToolTips" runat="server">
            </dnn:Label>
            <asp:CheckBox ID="chkShowToolTips" runat="server" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblShowCategoryType" ControlName="rblShowCategoryType" runat="server">
            </dnn:Label>
            <asp:RadioButtonList ID="rblShowCategoryType" runat="server" CssClass="dnnFormRadioButtons">
                <asp:ListItem Value="0" ResourceKey="ShowCategoryTypeList">List with checkboxes</asp:ListItem>
                <asp:ListItem Value="1" ResourceKey="ShowCategoryTypeTree">Treeview</asp:ListItem>
                <asp:ListItem Value="2" ResourceKey="ShowCategoryTypeDropDown">Dropdown</asp:ListItem>
            </asp:RadioButtonList>
        </div>
    </asp:Panel>
    <div class="dnnFormItem">
        <dnn:Label ID="lblDefaultSorting" ControlName="lblDefaultSorting" runat="server">
        </dnn:Label>
        <asp:DropDownList ID="drpDefaultSorting" runat="server">
            <asp:ListItem Value="6" ResourceKey="OrderByViewOrder">Predefined Order</asp:ListItem>
            <asp:ListItem Value="0" ResourceKey="OrderByDateNew">Date New</asp:ListItem>
            <asp:ListItem Value="1" ResourceKey="OrderByDateOld">Date Old</asp:ListItem>
            <asp:ListItem Value="2" ResourceKey="OrderByPopularityHigh">Popularity High</asp:ListItem>
            <asp:ListItem Value="3" ResourceKey="OrderByPopularityLow">Popularity Low</asp:ListItem>
            <asp:ListItem Value="4" ResourceKey="OrderByDateCreatedReverse">Creation Date Descending</asp:ListItem>
            <asp:ListItem Value="5" ResourceKey="OrderByDateCreatedOriginal">Creation Date Ascending</asp:ListItem>
        </asp:DropDownList>
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lblUserSort" ControlName="chkUserSort" runat="server">
        </dnn:Label>
        <asp:CheckBox ID="chkUserSort" runat="server" />
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lblQuestionTemplate" ControlName="lblQuestionTemplate" runat="server">
        </dnn:Label>
        <asp:TextBox ID="txtQuestionTemplate" runat="server" Width="400px" CssClass="dnnFormInput" Height="104px" TextMode="MultiLine"></asp:TextBox>
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lblAnswerTemplate" ControlName="lblAnswerTemplate" runat="server">
        </dnn:Label>
        <asp:TextBox ID="txtAnswerTemplate" runat="server" Width="400px" CssClass="dnnFormInput" Height="104px" TextMode="MultiLine"></asp:TextBox>
        </td>
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lblLoadingTemplate" ControlName="lblLoadingTemplate" runat="server">
        </dnn:Label>
        <asp:TextBox ID="txtLoadingTemplate" runat="server" Width="400px" CssClass="dnnFormInput" Height="104px" TextMode="MultiLine"></asp:TextBox>
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="lblAvailableTokens" ControlName="lblAvailableTokens" runat="server">
        </dnn:Label>
        <asp:ListBox ID="lstAvailableTokens" runat="server" Height="169px" CssClass="dnnFormInput">
            <asp:ListItem Value="[FAQ:QUESTION]">[FAQ:QUESTION]</asp:ListItem>
            <asp:ListItem Value="[FAQ:ANSWER]">[FAQ:ANSWER]</asp:ListItem>
            <asp:ListItem Value="[FAQ:USER]">[FAQ:USER]</asp:ListItem>
            <asp:ListItem Value="[FAQ:VIEWCOUNT]">[FAQ:VIEWCOUNT]</asp:ListItem>
            <asp:ListItem Value="[FAQ:CATEGORYNAME]">[FAQ:CATEGORYNAME]</asp:ListItem>
            <asp:ListItem Value="[FAQ:CATEGORYDESC]">[FAQ:CATEGORYDESC]</asp:ListItem>
            <asp:ListItem Value="[FAQ:DATECREATED]">[FAQ:DATECREATED]</asp:ListItem>
            <asp:ListItem Value="[FAQ:DATEMODIFIED]">[FAQ:DATEMODIFIED]</asp:ListItem>
            <asp:ListItem Value="[FAQ:INDEX]">[FAQ:INDEX]</asp:ListItem>
        </asp:ListBox>
    </div>
</div>
