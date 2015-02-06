//
// DotNetNuke® - http://www.dnnsoftware.com
// Copyright (c) 2002-2014
// by DNN Corp
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions
// of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.
//

using System;
using System.Web.UI.WebControls;
using DotNetNuke.Common;
using DotNetNuke.Entities.Users;
using DotNetNuke.UI.UserControls;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Security;

namespace DotNetNuke.Modules.FAQs
{
    [DNNtc.PackageProperties("DNN_FAQs")]
    [DNNtc.ModuleProperties("DNN_FAQs")]
    [DNNtc.ModuleControlProperties("Edit", "Edit FAQs", DNNtc.ControlType.Edit, "http://dnnfaq.codeplex.com/", false, true)]
    public partial class EditFAQs : PortalModuleBase
    {

        #region Members

        protected TextBox txtQuestionField;
        protected TextEditor teAnswerField;
        protected ModuleAuditControl ctlAudit;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the FAQ id.
        /// </summary>
        public int FaqId
        {
            get
            {
                if (!Null.IsNull(Request.QueryString["ItemId"]))
                {
                    try
                    {
                        return System.Convert.ToInt32(Request.QueryString["ItemId"]);
                    }
                    catch (Exception exc) //Module failed to load
                    {
                        Exceptions.ProcessModuleLoadException(this, exc);
                    }
                }
                else
                {
                    return Null.NullInteger;
                }

                return 0;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Populates the categories drop down.
        /// </summary>
        private void PopulateCategoriesDropDown()
        {
            FAQsController FAQsController = new FAQsController();

            foreach (CategoryInfo category in FAQsController.ListCategoriesHierarchical(ModuleId, false))
            {
                drpCategory.Items.Add(new ListItem(new string('.', category.Level * 3) + category.FaqCategoryName, category.FaqCategoryId.ToString()));
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        protected void Page_Load(System.Object sender, System.EventArgs e)
        {

            if (Page.IsPostBack == false)
            {

                cmdDelete.Attributes.Add("onClick", "javascript:return confirm(\'" + Localization.GetString("DeleteItem") + "\');");

                FAQsController FAQsController = new FAQsController();

                PopulateCategoriesDropDown();

                if (!Null.IsNull(FaqId))
                {

                    FAQsInfo FaqItem = FAQsController.GetFAQ(FaqId);

                    if (FaqItem != null)
                    {

                        if (FaqItem.CategoryId != null)
                        {
                            drpCategory.SelectedValue = FaqItem.CategoryId.ToString();
                        }

                        chkFaqHide.Checked = FaqItem.FaqHide;
                        datepickerPublishDate.SelectedDate = FaqItem.PublishDate;
                        datepickerExpireDate.SelectedDate = FaqItem.ExpireDate;
                        teAnswerField.Text = FaqItem.Answer;
                        txtQuestionField.Text = FaqItem.Question;
                        UserInfo user = UserController.GetUserById(PortalId, Convert.ToInt32(FaqItem.CreatedByUser));
                        ctlAudit.CreatedByUser = (user != null ? user.DisplayName : "");
                        if (FaqItem.DateModified == Null.NullDate)
                        {
                            ctlAudit.CreatedDate = FaqItem.CreatedDate.ToString();
                        }
                        else
                        {
                            ctlAudit.CreatedDate = FaqItem.DateModified.ToString();
                        }
                    }
                    else
                    {
                        Response.Redirect(Globals.NavigateURL(), true);
                    }

                }
                else
                {
                    cmdDelete.Visible = false;
                    ctlAudit.Visible = false;
                }

            }

        }

        /// <summary>
        /// Handles the Click event of the cmdUpdate control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        protected void cmdUpdate_Click(System.Object sender, System.EventArgs e)
        {

            try
            {
                   // We do not allow for script or markup in the question
                PortalSecurity objSecurity = new PortalSecurity();
                string question = objSecurity.InputFilter(txtQuestionField.Text, PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoMarkup);
                string answer = objSecurity.InputFilter(teAnswerField.Text, PortalSecurity.FilterFlag.NoScripting);

                FAQsController faqsController = new FAQsController();
                FAQsInfo faq;

                int? newCatID = null;
                if (drpCategory.SelectedValue != "-1")
                    newCatID = int.Parse(drpCategory.SelectedValue);

                 // Do we add of update? The Id will tell us
                if (FaqId != -1)
                {
                    faq = faqsController.GetFAQ(FaqId);
                    faq.CategoryId = newCatID;
                    faq.FaqHide = chkFaqHide.Checked;
                    faq.PublishDate = datepickerPublishDate.SelectedDate;
                    faq.ExpireDate = datepickerExpireDate.SelectedDate;
                    faq.Question = question;
                    faq.Answer = answer;
                    faq.DateModified = DateTime.Now;
                    faqsController.UpdateFAQ(faq);
                }
                else
                {
                    faq = new FAQsInfo
                              {
                                  ItemID = FaqId,
                                  CategoryId = newCatID,
                                  FaqHide = chkFaqHide.Checked,
                                  PublishDate = datepickerPublishDate.SelectedDate,
                                  ExpireDate = datepickerExpireDate.SelectedDate,
                                  Question = question,
                                  Answer = answer,
                                  CreatedByUser = UserId.ToString(),
                                  ViewCount = 0,
                                  DateModified = DateTime.Now,
                                  ModuleID = ModuleId,
                                  CreatedDate = DateTime.Now
                              };
                    faqsController.AddFAQ(faq);
                }
                Response.Redirect(Globals.NavigateURL(), true);
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }

        }

        /// <summary>
        /// Handles the Click event of the cmdCancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        protected void cmdCancel_Click(System.Object sender, System.EventArgs e)
        {
            try
            {
                Response.Redirect(Globals.NavigateURL(), true);
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// <summary>
        /// Handles the Click event of the cmdDelete control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        protected void cmdDelete_Click(System.Object sender, System.EventArgs e)
        {
            try
            {
                FAQsController FAQsController = new FAQsController();
                FAQsController.DeleteFAQ(FaqId);
                Response.Redirect(DotNetNuke.Common.Globals.NavigateURL());
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        #endregion

    }

}
