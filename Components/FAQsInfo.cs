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

using DotNetNuke.ComponentModel.DataAnnotations;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Tokens;
using System;
using System.Globalization;
using System.Web.Caching;

namespace DotNetNuke.Modules.FAQs
{
	
	/// <summary>
	/// Info class for FAQs
	/// </summary>
    [Serializable]
    [TableName("FAQs")]
    [PrimaryKey("ItemID", AutoIncrement = true)]
    [Scope("ModuleID")]
    [Cacheable("FAQs", CacheItemPriority.Normal, 20)]

	public class FAQsInfo:IPropertyAccess
    {
        #region Properties
        /// <summary>
	    /// Gets or sets the item id.
	    /// </summary>
	    /// <value>The item id.</value>
	    public int ItemID { get; set; }
		
		/// <summary>
		/// Gets or sets the module id.
		/// </summary>
		/// <value>The module id.</value>
		public int ModuleID { get; set; }
		
		/// <summary>
		/// Gets or sets the category id.
		/// </summary>
		/// <value>The category id.</value>
        public int? CategoryId { get; set; }
		
		/// <summary>
		/// Gets or sets the question.
		/// </summary>
		/// <value>The question.</value>
        public string Question { get; set; }
		
		/// <summary>
		/// Gets or sets the answer.
		/// </summary>
		/// <value>The answer.</value>
        public string Answer { get; set; }
		
		/// <summary>
		/// Gets or sets the created by user.
		/// </summary>
		/// <value>The created by user.</value>
        public string CreatedByUser { get; set; }
		
		/// <summary>
		/// Gets or sets the created date.
		/// </summary>
		/// <value>The created date.</value>
        public DateTime CreatedDate { get; set; }
		
		/// <summary>
		/// Gets or sets the date modified.
		/// </summary>
		/// <value>The date modified.</value>
        public DateTime DateModified { get; set; }
		
		/// <summary>
		/// Gets or sets the view count.
		/// </summary>
		/// <value>The view count.</value>
        public int ViewCount { get; set; }
		
		/// <summary>
		/// Gets or sets the view order.
		/// </summary>
		/// <value>The view order.</value>
        public int ViewOrder { get; set; }

        /// <summary>
        /// Gets or sets the order number.
        /// </summary>
        /// <value>The order number.</value>
        [IgnoreColumn]
        public int Index { get; set; }
		
		/// <summary>
		/// Gets or sets the visibility of the Faq-Item
		/// </summary>
		/// <value>the Hide flag. </value>
        public Boolean FaqHide { get; set; }
		
		/// <summary>
		/// Gets or sets the publish date.
		/// </summary>
		/// <value>The publish date.</value>
        public DateTime? PublishDate { get; set; }
		
		/// <summary>
		/// Gets or sets the expiration date.
		/// </summary>
		/// <value>The expiration date.</value>
        public DateTime? ExpireDate { get; set; }
		
		#endregion

		#region Implementation of IPropertyAccess

		public string GetProperty(string strPropertyName, string strFormat, CultureInfo formatProvider, UserInfo AccessingUser, Scope AccessLevel, ref bool PropertyNotFound)
		{

			PropertyNotFound = false;
		    FAQsController faqController;
			switch (strPropertyName.ToLower())
			{
				case "question":
					return PropertyAccess.FormatString(Question, strFormat);
				case "answer":
					return PropertyAccess.FormatString(Answer, strFormat);
				case "user":
			        UserInfo user = UserController.GetUserById(PortalSettings.Current.PortalId, Convert.ToInt32(CreatedByUser));
					return PropertyAccess.FormatString(user.DisplayName, strFormat);
				case "viewcount":
					return ViewCount.ToString(String.IsNullOrEmpty(strFormat) ? "g" : strFormat, formatProvider);
				case "vieworder":
					return ViewOrder.ToString(String.IsNullOrEmpty(strFormat) ? "g" : strFormat, formatProvider);
				case "categoryname":
                    faqController = new FAQsController();
                    return PropertyAccess.FormatString(faqController.GetCategory(CategoryId).FaqCategoryName, strFormat);
				case "categorydesc":
                    faqController = new FAQsController();
                    return PropertyAccess.FormatString(faqController.GetCategory(CategoryId).FaqCategoryDescription, strFormat);
				case "datecreated":
					return CreatedDate.ToString(String.IsNullOrEmpty(strFormat) ? "d" : strFormat, formatProvider);
				case "datemodified":
					return DateModified.ToString(String.IsNullOrEmpty(strFormat) ? "d" : strFormat, formatProvider);
				case "index":
					return Index.ToString(String.IsNullOrEmpty(strFormat) ? "g" : strFormat, formatProvider);
				default:
					PropertyNotFound = true;
					return String.Empty;
			}
		}

		[IgnoreColumn]
        public CacheLevel Cacheability
		{
			get { return CacheLevel.fullyCacheable; }
		}

		#endregion 
	}
	
}