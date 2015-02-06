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
using System.Web.UI.HtmlControls;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Localization;

namespace DotNetNuke.Modules.FAQs
{
    [DNNtc.PackageProperties("DNN_FAQs")]
    [DNNtc.ModuleProperties("DNN_FAQs")]
	[DNNtc.ModuleControlProperties("Settings", "FAQ Settings", DNNtc.ControlType.Admin, "http://dnnfaq.codeplex.com/", true, true)]
	public partial class Settings : ModuleSettingsBase
	{
		
		#region Members
		
		protected HtmlTable tblHTMLTemplates;
		
		#endregion
		
		#region Public Methods
		
		/// <summary>
		/// Loads the settings.
		/// </summary>
		public override void LoadSettings()
		{
			
			try
			{
				
				if (! Null.IsNull(Settings["ShowCategories"]))
				{
					chkShowCatagories.Checked = Convert.ToBoolean(Settings["ShowCategories"]);
					pnlShowCategoryType.Visible = chkShowCatagories.Checked;
				}
				else
				{
					chkShowCatagories.Checked = false;
					pnlShowCategoryType.Visible = false;
				}

				if (!Null.IsNull(Settings["ShowEmptyCategories"]))
				{
					chkShowEmptyCategories.Checked = Convert.ToBoolean(Settings["ShowEmptyCategories"]);
				}
				else
				{
					chkShowEmptyCategories.Checked = false;
				}

				if (!Null.IsNull(Settings["ShowToolTips"]))
				{
					chkShowToolTips.Checked = Convert.ToBoolean(Settings["ShowToolTips"]);
				}
				else
				{
					chkShowToolTips.Checked = false;
				}
				
				if (!Null.IsNull(Settings["ShowCategoryType"]))
				{
					rblShowCategoryType.SelectedValue = (string)Settings["ShowCategoryType"];
				}
				else
				{
					rblShowCategoryType.SelectedIndex = 0;
				}

				if (!Null.IsNull(Settings["UserSort"]))
				{
					chkUserSort.Checked = Convert.ToBoolean(Settings["UserSort"]);
				}
				else
				{
					chkUserSort.Checked = false;
				}
				
				if (! Null.IsNull(Settings["FaqQuestionTemplate"]))
				{
					txtQuestionTemplate.Text = Convert.ToString(Settings["FaqQuestionTemplate"]);
				}
				else
				{
					txtQuestionTemplate.Text = Localization.GetString("DefaultQuestionTemplate", this.LocalResourceFile);
				}
				
				if (! Null.IsNull(Settings["FaqAnswerTemplate"]))
				{
					txtAnswerTemplate.Text = Convert.ToString(Settings["FaqAnswerTemplate"]);
				}
				else
				{
					txtAnswerTemplate.Text = Localization.GetString("DefaultAnswerTemplate", this.LocalResourceFile);
				}
				
				if (! Null.IsNull(Settings["FaqLoadingTemplate"]))
				{
					txtLoadingTemplate.Text = Convert.ToString(Settings["FaqLoadingTemplate"]);
				}
				else
				{
					txtLoadingTemplate.Text = Localization.GetString("DefaultLoadingTemplate", this.LocalResourceFile);
				}
				
				if (! Null.IsNull(Settings["FaqDefaultSorting"]))
				{
					drpDefaultSorting.SelectedValue = Convert.ToString(Settings["FaqDefaultSorting"]);
				}
				
			}
			catch (Exception exc) //Module failed to load
			{
				DotNetNuke.Services.Exceptions.Exceptions.ProcessModuleLoadException(this, exc);
			}
			
		}
		
		/// <summary>
		/// Updates the settings.
		/// </summary>
		public override void UpdateSettings()
		{
			
			try
			{
				
				ModuleController modController = new ModuleController();
				
				modController.UpdateModuleSetting(ModuleId, "ShowCategories", chkShowCatagories.Checked.ToString());
				modController.UpdateModuleSetting(ModuleId, "ShowEmptyCategories", chkShowEmptyCategories.Checked.ToString());
				modController.UpdateModuleSetting(ModuleId, "ShowToolTips", chkShowToolTips.Checked.ToString());
				modController.UpdateModuleSetting(ModuleId, "ShowCategoryType", rblShowCategoryType.SelectedValue);
				modController.UpdateModuleSetting(ModuleId, "UserSort", chkUserSort.Checked.ToString());
				modController.UpdateModuleSetting(ModuleId, "FaqQuestionTemplate", txtQuestionTemplate.Text);
				modController.UpdateModuleSetting(ModuleId, "FaqAnswerTemplate", txtAnswerTemplate.Text);
				modController.UpdateModuleSetting(ModuleId, "FaqLoadingTemplate", txtLoadingTemplate.Text);
				modController.UpdateModuleSetting(ModuleId, "FaqDefaultSorting", drpDefaultSorting.SelectedValue);
				
			}
			catch (Exception exc) //Module failed to load
			{
				DotNetNuke.Services.Exceptions.Exceptions.ProcessModuleLoadException(this, exc);
			}
			
		}
		
		#endregion

		protected void chkShowCatagories_CheckedChanged(object sender, EventArgs e)
		{
			pnlShowCategoryType.Visible = chkShowCatagories.Checked;
		}
		
	}
	
}
