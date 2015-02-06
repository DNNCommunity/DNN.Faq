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
using System.Web.Caching;

namespace DotNetNuke.Modules.FAQs
{
	
	/// <summary>
	/// Main info class for the supporting categories
	/// </summary>
	[TableName("FAQsCategory")]
    [PrimaryKey("FaqCategoryId",AutoIncrement = true)]
    [Scope("ModuleId")]
    [Cacheable("FAQsCategory",CacheItemPriority.Normal,20)]
	public class CategoryInfo
	{
	    /// <summary>
	    /// Gets or sets the FAQ category parent id.
	    /// </summary>
	    /// <value>The FAQ category parent id.</value>
	    public int? FaqCategoryParentId { get; set; }
		
		
		/// <summary>
		/// Gets or sets the FAQ category id.
		/// </summary>
		/// <value>The FAQ category id.</value>
		public int FaqCategoryId { get; set; }
		
		
		/// <summary>
		/// Gets or sets the module id.
		/// </summary>
		/// <value>The module id.</value>
		public int ModuleId { get; set; }
		
		/// <summary>
		/// Gets or sets the name of the FAQ category.
		/// </summary>
		/// <value>The name of the FAQ category.</value>
		public string FaqCategoryName { get; set; }
		
		
		/// <summary>
		/// Gets or sets the FAQ category description.
		/// </summary>
		/// <value>The FAQ category description.</value>
		public string FaqCategoryDescription { get; set; }
		

		/// <summary>
		/// Gets or sets the module hierarchical level.
		/// </summary>
		/// <value>The module hierarchical level.</value>
		[IgnoreColumn]
        public int Level { get; set; }
		

		/// <summary>
		/// Gets or sets the view order between childs of one node.
		/// </summary>
		/// <value>The view order.</value>
		public int ViewOrder { get; set; }
     }
}

