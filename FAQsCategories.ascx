<%@ Control Language="C#" Inherits="DotNetNuke.Modules.FAQs.FAQsCategories" AutoEventWireup="true" CodeBehind="FAQsCategories.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web.Deprecated" %>
<div class="dnnForm dnnFAQsCategories dnnClear" style="display:grid; grid-template-columns: repeat(2, 1fr); grid-gap:30px;">
	<div style="grid-column: 1;">
		<asp:TreeView runat="server" ID="treeCategories" OnSelectedNodeChanged="treeCategories_SelectedNodeChanged"></asp:TreeView>
		<ul class="dnnActions dnnClear">
			<li>
				<asp:LinkButton ID="cmdAddNew" resourcekey="cmdAddNew" runat="server" CssClass="dnnPrimaryAction" CausesValidation="False" OnCommand="cmdAddNew_Click" /></li>
			<li>
				<asp:LinkButton ID="cmdGoBack" resourcekey="cmdGoBack" runat="server" CssClass="dnnSecondaryAction" CausesValidation="False" OnCommand="cmdGoBack_Click" /></li>
		</ul>
	</div>
	<div style="grid-column:2;">
		<asp:Panel ID="panelAddEdit" runat="server" Visible="False">
			<div class="dnnFormItem" id="rowFaqCategoryParentId" runat="server">
				<dnn:Label ID="plParentCategoryField" runat="server" ControlName="drpParentCategory"></dnn:Label>
				<asp:DropDownList ID="drpParentCategory" runat="server" />
			</div>
			<div class="dnnFormItem">
				<dnn:Label ID="plCategoryName" runat="server" ControlName="CategoryEdit"></dnn:Label>
				<asp:TextBox ID="txtCategoryName" runat="server" MaxLength="100" CssClass="dnnFormRequired"></asp:TextBox>
				<asp:RequiredFieldValidator ID="rqdCategoryName" runat="server" CssClass="dnnFormMessage dnnFormError" ErrorMessage="Category name is required" ControlToValidate="txtCategoryName" resourcekey="rqdCategoryName"></asp:RequiredFieldValidator>
			</div>
			<div class="dnnFormItem">
				<dnn:Label ID="plCategoryDescription" runat="server" ControlName="CategoryEdit"></dnn:Label>
				<asp:TextBox ID="txtCategoryDescription" runat="server" TextMode="MultiLine" MaxLength="250" CssClass="dnnFormRequired"></asp:TextBox>
				<asp:RequiredFieldValidator ID="rqdCategoryDescription" runat="server" CssClass="dnnFormMessage dnnFormError" ErrorMessage="Description is Required" ControlToValidate="txtCategoryDescription" resourcekey="rqdCategoryDescription"></asp:RequiredFieldValidator>
			</div>
			<ul class="dnnActions dnnClear">
				<li>
					<asp:LinkButton ID="cmdUpdate" resourcekey="cmdUpdate" runat="server" CssClass="dnnPrimaryAction" OnCommand="cmdUpdate_Click" /></li>
				<li>
					<asp:LinkButton ID="cmdCancel" resourcekey="cmdCancel" runat="server" CssClass="dnnSecondaryAction" OnCommand="cmdCancel_Click" CausesValidation="False" /></li>
				<li>
					<asp:LinkButton ID="cmdDelete" resourcekey="cmdDelete" runat="server" CssClass="dnnSecondaryAction" OnCommand="cmdDelete_Click" /></li>
			</ul>
		</asp:Panel>
	</div>
</div>
