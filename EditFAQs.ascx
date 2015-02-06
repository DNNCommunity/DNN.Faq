<%@ Control Language="C#" Inherits="DotNetNuke.Modules.FAQs.EditFAQs" AutoEventWireup="true" CodeBehind="EditFAQs.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="Portal" TagName="Audit" Src="~/controls/ModuleAuditControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="TextEditor" Src="~/controls/TextEditor.ascx" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web" %>
<div class="dnnForm dnnEditFAQs dnnClear">
    <div class="dnnFormItem">
        <dnn:Label id="plQuestionField" runat="server" controlname="txtQuestionField">
        </dnn:Label>
        <asp:TextBox ID="txtQuestionField" runat="server" MaxLength="400" Width="400px" TextMode="SingleLine" CssClass="dnnFormRequired"></asp:TextBox>
        <asp:RequiredFieldValidator ID="valRequiredTitle" runat="server" resourcekey="valRequiredTitle" ControlToValidate="txtQuestionField" ErrorMessage="Question is required" CssClass="dnnFormMessage dnnFormError"></asp:RequiredFieldValidator>
    </div>
    <div class="dnnFormItem">
        <dnn:Label id="plAnswerField" runat="server" controlname="teAnswerField">
        </dnn:Label>
        <div class="dnnRight">
            <dnn:Texteditor choosemode="False" controlid="teAnswerField" height="300" id="teAnswerField" runat="server" width="600">
            </dnn:Texteditor></div>
    </div>
    <div class="dnnFormItem">
        <dnn:Label id="plCategoryField" runat="server" controlname="drpCategory">
        </dnn:Label>
        <asp:DropDownList ID="drpCategory" runat="server">
            <asp:ListItem Value="-1" resourcekey="SelectCategory">Select Category</asp:ListItem>
        </asp:DropDownList>
    </div>
    <div class="dnnFormItem">
        <dnn:Label id="plFaqHide" runat="server" controlname="chkFaqHide">
        </dnn:Label>
        <asp:CheckBox ID="chkFaqHide" runat="server" />
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="plPublishDate" runat="server" ControlName="datepickerPublishDate" />
        <dnn:DnnDatePicker ID="datepickerPublishDate" runat="server" />&nbsp;
        <asp:CompareValidator ID="valPublishDate" resourcekey="valPublishDate.ErrorMessage" Operator="DataTypeCheck" Type="Date" runat="server" Display="Dynamic" ControlToValidate="datepickerPublishDate" CssClass="dnnFormMessage dnnFormError" />
    </div>
    <div class="dnnFormItem">
        <dnn:Label ID="plExpireDate" runat="server" ControlName="datepickerExpireDate" />
        <dnn:DnnDatePicker ID="datepickerExpireDate" runat="server" />&nbsp;
        <asp:CompareValidator ID="valExpireDate" resourcekey="valExpireDate.ErrorMessage" Operator="DataTypeCheck" Type="Date" runat="server" Display="Dynamic" ControlToValidate="datepickerExpireDate" CssClass="dnnFormMessage dnnFormError" />
        <asp:CompareValidator ID="val2ExpireDate" resourcekey="val2ExpireDate.ErrorMessage" Operator="GreaterThanEqual" Type="Date" runat="server" Display="Dynamic" ControlToValidate="datepickerExpireDate" ControlToCompare="datepickerPublishDate" CssClass="dnnFormMessage dnnFormError" />
    </div>
    <ul class="dnnActions dnnClear">
        <li>
            <asp:LinkButton ID="cmdUpdate" resourcekey="cmdUpdate" runat="server" CssClass="dnnPrimaryAction" OnCommand="cmdUpdate_Click" /></li>
        <li>
            <asp:LinkButton ID="cmdCancel" resourcekey="cmdCancel" runat="server" CssClass="dnnSecondaryAction" OnCommand="cmdCancel_Click" CausesValidation="False" /></li>
        <li>
            <asp:LinkButton ID="cmdDelete" resourcekey="cmdDelete" runat="server" CssClass="dnnSecondaryAction" OnCommand="cmdDelete_Click" CausesValidation="False" /></li>
    </ul>
    <Portal:Audit ID="ctlAudit" runat="server">
    </Portal:Audit>
</div>
