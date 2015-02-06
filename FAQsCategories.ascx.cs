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
using Telerik.Web.UI;

namespace DotNetNuke.Modules.FAQs
{
    [DNNtc.PackageProperties("DNN_FAQs")]
    [DNNtc.ModuleProperties("DNN_FAQs")]
	[DNNtc.ModuleControlProperties("EditCategories", "Edit FAQ Categories", DNNtc.ControlType.Edit, "http://dnnfaq.codeplex.com/", true, true)]
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
			treeCategories.UnselectAllNodes();
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
				RadTreeNode node = treeCategories.SelectedNode;
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
				RadTreeNode node = treeCategories.SelectedNode;
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

		/// <summary>
		/// Handles the NodeDataBound event of the treeCategories control (adds Tooltip)
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">instance containing the event data.</param>
		protected void treeCategories_NodeDataBound(object sender, RadTreeNodeEventArgs e)
		{
			e.Node.ToolTip = (string)DataBinder.Eval(e.Node.DataItem, "FaqCategoryDescription");
			e.Node.Value = DataBinder.Eval(e.Node.DataItem, "FaqCategoryId").ToString();
			e.Node.ImageUrl = IconController.IconURL("folder");
		}
		
		/// <summary>
		/// Handles the NodeClick event of the treeCategories control
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">instance containing the event data.</param>
		protected void treeCategories_NodeClick(object sender, RadTreeNodeEventArgs e)
		{
			int faqCategoryId = Convert.ToInt32(e.Node.Value);
			EditCategory(faqCategoryId);
			cmdDelete.Visible = true;
		}


		protected void treeCategories_HandleDrop(object sender, RadTreeNodeDragDropEventArgs e)
		{
			FAQsController FAQsController = new FAQsController();
			RadTreeNode sourceNode = e.SourceDragNode;
			RadTreeNode destNode = e.DestDragNode;
			RadTreeViewDropPosition dropPosition = e.DropPosition;

			if (destNode == null || sourceNode == destNode || sourceNode.IsAncestorOf(destNode))
				return;
			
			int sourceFaqCategoryId = Convert.ToInt32(sourceNode.Value);
			CategoryInfo sourceCategory = FAQsController.GetCategory(sourceFaqCategoryId);

			int destFaqCategoryId = Convert.ToInt32(destNode.Value);
			CategoryInfo destCategory = FAQsController.GetCategory(destFaqCategoryId);

			switch (dropPosition)
			{
				case RadTreeViewDropPosition.Over: // child
					// Change Treeview
					sourceNode.Owner.Nodes.Remove(sourceNode);
					destNode.Nodes.Add(sourceNode);
						
					// Change the ParentId of Source in database
					sourceCategory.FaqCategoryParentId = destCategory.FaqCategoryId;
					sourceCategory.ViewOrder = 999;
					FAQsController.UpdateCategory(sourceCategory);
					break;
				
				case RadTreeViewDropPosition.Above: // sibling - above
					sourceNode.Owner.Nodes.Remove(sourceNode);
					destNode.InsertBefore(sourceNode);
					sourceCategory.FaqCategoryParentId = destCategory.FaqCategoryParentId;
					sourceCategory.ViewOrder = destCategory.ViewOrder - 1;
					FAQsController.UpdateCategory(sourceCategory);
					
					break;
				
				case RadTreeViewDropPosition.Below: // sibling - below
					sourceNode.Owner.Nodes.Remove(sourceNode);
					destNode.InsertAfter(sourceNode);
					sourceCategory.FaqCategoryParentId = destCategory.FaqCategoryParentId;
					sourceCategory.ViewOrder = destCategory.ViewOrder + 1;
					FAQsController.UpdateCategory(sourceCategory);
					break;
			}
			FAQsController.ReorderCategory(sourceCategory.FaqCategoryParentId, ModuleId);
			panelAddEdit.Visible = false;
		}

		#endregion

		#region Private Methods

		private void BindData()
		{
			FAQsController FAQsController = new FAQsController();
            IEnumerable<CategoryInfo> cats = FAQsController.ListCategoriesHierarchical(ModuleId, false);

            treeCategories.Nodes.Clear();
			treeCategories.DataTextField = "FaqCategoryName";
			treeCategories.DataFieldID = "FaqCategoryId";
			treeCategories.DataFieldParentID = "FaqCategoryParentId";
			treeCategories.DataSource = cats;
			treeCategories.DataBind();
			if (!IsPostBack && treeCategories.Nodes.Count > 0)
				treeCategories.Nodes[0].Selected = true;
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
			drpParentCategory.Items.Add(new ListItem(Localization.GetString("SelectParentCategory.Text",this.LocalResourceFile), "-1"));
			FAQsController FAQsController = new FAQsController();
			foreach (CategoryInfo category in FAQsController.ListCategoriesHierarchical(ModuleId, false))
			{
				if (faqCategoryId != category.FaqCategoryId)
					drpParentCategory.Items.Add(new ListItem(new string('.',category.Level*3) + category.FaqCategoryName, category.FaqCategoryId.ToString()));
			}
		}
		#endregion
	}
}
