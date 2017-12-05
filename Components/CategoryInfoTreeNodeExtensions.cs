﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DotNetNuke.Modules.FAQs
{
    internal static class CategoryInfoTreeNodeExtensions
    {
        internal static CategoryInfoTreeNode ToTreeNode(this CategoryInfo categoryInfo)
        {
            return new CategoryInfoTreeNode
            {
                FaqCategoryParentId = categoryInfo.FaqCategoryParentId.HasValue ? categoryInfo.FaqCategoryParentId.Value : -1,
                FaqCategoryId = categoryInfo.FaqCategoryId,
                ModuleId = categoryInfo.ModuleId,
                FaqCategoryName = categoryInfo.FaqCategoryName,
                FaqCategoryDescription = categoryInfo.FaqCategoryDescription,
                Level = categoryInfo.Level,
                ViewOrder = categoryInfo.ViewOrder
            };
        }
    }
}