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
using System.Collections;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Common;
using DotNetNuke.Entities.Icons;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Security;
using System.Linq;

namespace DotNetNuke.Modules.FAQs
{
    partial class FAQsCategories : PortalModuleBase
    {
        #region Event Handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Page.IsPostBack == false)
                {
                    BindData();

                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// <summary>
        /// Handles the Click event of the cmdAddNew control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        protected void cmdAddNew_Click(Object sender, EventArgs e)
        {
            panelAddEdit.Visible = true;
            txtCategoryDescription.Text = "";
            txtCategoryName.Text = "";
            PopulateCategoriesDropDown(-1);
            cmdDelete.Visible = false;
        }

        /// <summary>
        /// Handles the Click event of the cmdGoBack control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        protected void cmdGoBack_Click(Object sender, EventArgs e)
        {
            Response.Redirect(Globals.NavigateURL());
        }

        /// <summary>
        /// Handles the Click event of the cmdUpdate control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        protected void cmdUpdate_Click(Object sender, EventArgs e)
        {

            FAQsController faqsController = new FAQsController();
            CategoryInfo categoryItem = new CategoryInfo();
            PortalSecurity objSecurity = new PortalSecurity();

            int parentCategoryId = Convert.ToInt32(drpParentCategory.SelectedValue);
            if (parentCategoryId < 0)
                parentCategoryId = 0;

            // We do not allow for script or markup
            categoryItem.FaqCategoryParentId = parentCategoryId;
            categoryItem.FaqCategoryName = objSecurity.InputFilter(txtCategoryName.Text, PortalSecurity.FilterFlag.NoMarkup | PortalSecurity.FilterFlag.NoScripting);
            categoryItem.FaqCategoryDescription = objSecurity.InputFilter(txtCategoryDescription.Text, PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoMarkup);
            categoryItem.ModuleId = ModuleId;

            try
            {
                TreeNode node = treeCategories.SelectedNode;
                if (node != null)
                {
                    categoryItem.FaqCategoryId = Convert.ToInt32(node.Value);
                    CategoryInfo originalCategoryItem = faqsController.GetCategory(categoryItem.FaqCategoryId);
                    categoryItem.ViewOrder = originalCategoryItem.ViewOrder;
                    faqsController.UpdateCategory(categoryItem);
                }
                else
                {
                    categoryItem.ViewOrder = 999;
                    faqsController.AddCategory(categoryItem);
                }
                faqsController.ReorderCategory(categoryItem.FaqCategoryParentId, ModuleId);
                Response.Redirect(Request.RawUrl);
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
        protected void cmdDelete_Click(Object sender, EventArgs e)
        {
            FAQsController faqsController = new FAQsController();
            try
            {
                TreeNode node = treeCategories.SelectedNode;
                if (node != null)
                {
                    int faqCategoryId = Convert.ToInt32(node.Value);
                    faqsController.DeleteCategory(faqCategoryId);
                }
                Response.Redirect(Request.RawUrl);
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
            panelAddEdit.Visible = false;
        }

        #endregion

        #region Private Methods

        private void PopulateTreeView(List<CategoryInfoTreeNode> list, int ParentId, TreeNodeCollection treeNode)
        {
            foreach (var row in list.Where(x => x.FaqCategoryParentId == ParentId))
            {
                TreeNode child = new TreeNode
                {
                    Text = row.FaqCategoryName,
                    Value = row.FaqCategoryId.ToString()
                };

                treeNode.Add(child);
                var dtChild = list.Where(x => x.FaqCategoryParentId == ParentId).ToList();
                if (dtChild.Count > 0)
                    PopulateTreeView(list, row.FaqCategoryId, child.ChildNodes);
            }
        }

        private void BindData()
        {
            FAQsController FAQsController = new FAQsController();
            IEnumerable<CategoryInfo> cats = FAQsController.ListCategoriesHierarchical(ModuleId, false);
            // treeCategories fails with int? FaqCategoryParentId
            // define a temp class that has no nullables
            // set null ints to Null.NullInt
            var lst = new List<CategoryInfoTreeNode>();
            foreach (CategoryInfo cat in cats)
            {   
                lst.Add(cat.ToTreeNode());
            }

            PopulateTreeView(lst, 0, treeCategories.Nodes);
        }
        private void EditCategory(int faqCategoryId)
        {
            FAQsController faqsController = new FAQsController();
            panelAddEdit.Visible = true;
            PopulateCategoriesDropDown(faqCategoryId);
            CategoryInfo categoryItem = faqsController.GetCategory(faqCategoryId);
            int? parentCategoryId = categoryItem.FaqCategoryParentId;
            drpParentCategory.SelectedValue = (parentCategoryId == null ? "-1" : parentCategoryId.ToString());
            txtCategoryName.Text = categoryItem.FaqCategoryName;
            txtCategoryDescription.Text = categoryItem.FaqCategoryDescription;
        }

        /// <summary>
        /// Populates the (Parent-)categories drop down.
        /// </summary>
        private void PopulateCategoriesDropDown(int faqCategoryId)
        {
            drpParentCategory.Items.Clear();
            drpParentCategory.Items.Add(new ListItem(Localization.GetString("SelectParentCategory.Text", this.LocalResourceFile), "-1"));
            FAQsController FAQsController = new FAQsController();
            foreach (CategoryInfo category in FAQsController.ListCategoriesHierarchical(ModuleId, false))
            {
                if (faqCategoryId != category.FaqCategoryId)
                    drpParentCategory.Items.Add(new ListItem(new string('.', category.Level * 3) + category.FaqCategoryName, category.FaqCategoryId.ToString()));
            }
        }
        #endregion

        protected void treeCategories_SelectedNodeChanged(object sender, EventArgs e)
        {
            int faqCategoryId = Convert.ToInt32(treeCategories.SelectedNode.Value);
            EditCategory(faqCategoryId);
            cmdDelete.Visible = true;
        }
    }
}
