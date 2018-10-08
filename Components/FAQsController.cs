#region Copyright
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
#endregion

#region Usings
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Search.Entities;
#endregion

namespace DotNetNuke.Modules.FAQs
{

    /// <summary>
    /// Main controller class for FAQs
    /// </summary>
    [DNNtc.BusinessControllerClass()]
    public class FAQsController : ModuleSearchBase, IPortable // : ISearchable <--- Do not remove this comment, it is required to make DNNtc packager to the old ISearchable is implemented to generate proper manifest
    {
        public const int MAX_DESCRIPTION_LENGTH = 100;

        #region Public FAQ Methods

        /// <summary>
        /// Gets the FAQ.
        /// </summary>
        /// <param name="faqId">The FAQ id.</param>
        /// <returns>FAQInfo object</returns>
        public FAQsInfo GetFAQ(int faqId)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<FAQsInfo>();
                return rep.GetById(faqId);
            }
        }

        /// <summary>
        /// Lists the FAQ.
        /// </summary>
        /// <param name="moduleID">List FAQs for this specific module id (scope)</param>
        /// <param name="orderBy">Order result by.</param>
        /// <param name="showHidden">if true, show definitely all FAQs (Admin)</param>
        /// <returns>Arrarylist of FAQs</returns>
        public IEnumerable<FAQsInfo> ListFAQ(int moduleID, int orderBy, bool showHidden)
        {
            IEnumerable<FAQsInfo> faqs;
            string sql = "SELECT *" +
              " FROM {databaseOwner}[{objectQualifier}FAQs] " +
              " where ModuleId = @0" +
              " and (@2 = 1 OR FaqHide = 0)" +
              " and ((PublishDate IS NULL AND ExpireDate IS NULL) OR" +
              "     (PublishDate IS NULL AND ExpireDate >= DATEADD(dd, 0, DATEDIFF(dd, 0, GETDATE()))) OR" +
              "     (PublishDate < GetDate() AND ExpireDate IS NULL) OR" +
              "     (PublishDate < GetDate() AND ExpireDate >= DATEADD(dd, 0, DATEDIFF(dd, 0, GETDATE()))) OR" +
              "	   (@2 = 1))" +
              " ORDER BY" +
              "  CASE WHEN @1=0 THEN DateModified END DESC," +
              "  CASE WHEN @1=1 THEN DateModified END ASC, " +
              "  CASE WHEN @1=2 THEN ViewCount END DESC," +
              "  CASE WHEN @1=3 THEN ViewCount END ASC," +
              "  CASE WHEN @1=4 THEN CreatedDate END DESC," +
              "  CASE WHEN @1=5 THEN CreatedDate END ASC," +
              "  CASE WHEN @1=6 THEN ViewOrder END ASC";

            using (IDataContext ctx = DataContext.Instance())
            {
                faqs = ctx.ExecuteQuery<FAQsInfo>(CommandType.Text, sql, moduleID, orderBy, showHidden);
                return faqs;
            }
        }

        /// <summary>
        /// Adds a new FAQ to the database.
        /// </summary>
        /// <param name="faq">Info object containing faq information</param>
        /// <returns>ItemID (primary key) of new FAQ record</returns>
        public int AddFAQ(FAQsInfo faq)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<FAQsInfo>();
                rep.Insert(faq);
                return faq.ItemID;
            }
        }

        /// <summary>
        /// Updates the FAQ in the database.
        /// </summary>
        /// <param name="faq">Info object containing faq information</param>
        public void UpdateFAQ(FAQsInfo faq)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<FAQsInfo>();
                rep.Update(faq);
            }
        }

        /// <summary>
        /// Deletes the FAQ.
        /// </summary>
        /// <param name="faqId">The ItemId of FAQ record to delete</param>
        public void DeleteFAQ(int faqId)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<FAQsInfo>();
                rep.Delete(new FAQsInfo() { ItemID = faqId });
            }
        }

        /// <summary>
        /// Swap Vieworder From FAQ records with ItemId1 and ItemId2
        /// </summary>
        /// <param name="faqId1">The ItemId of the first FAQ record</param>
        /// <param name="faqId2">The ItemId of the second FAQ record</param>
        /// <param name="moduleId">ID of the module</param>
        public void ReorderFAQ(int faqId1, int faqId2, int moduleId)
        {
            string sql = "WITH tmpSwappedOrder(ItemId,ViewOrder) AS" +
                         " (" +
                         "	SELECT @1 AS ItemId,ViewOrder FROM {databaseOwner}[{objectQualifier}FAQs] WHERE ItemId = @0" +
                         "	UNION" +
                         "	SELECT @0 AS ItemId,ViewOrder FROM {databaseOwner}[{objectQualifier}FAQs] WHERE ItemId = @1" +
                         " )" +
                         " UPDATE {databaseOwner}[{objectQualifier}FAQs] SET ViewOrder = (SELECT ViewOrder FROM tmpSwappedOrder s WHERE s.ItemId = {databaseOwner}[{objectQualifier}FAQs].ItemId)" +
                         " WHERE {databaseOwner}[{objectQualifier}FAQs].ItemId IN (SELECT ItemId FROM tmpSwappedOrder);" +
                         " " +
                         " WITH tmpReorder(ViewOrder,ItemId) AS" +
                         " (" +
                         "	SELECT TOP 1000 row_number() OVER (ORDER BY f.ViewOrder) as rank, f.ItemId" +
                         "	FROM {databaseOwner}[{objectQualifier}FAQs] f" +
                         "	WHERE f.ModuleId = @2" +
                         "	ORDER BY rank " +
                         " )" +
                         " UPDATE {databaseOwner}[{objectQualifier}FAQs] " +
                         "	SET ViewOrder = (SELECT ViewOrder FROM tmpReorder r WHERE r.ItemId = {databaseOwner}[{objectQualifier}FAQs].ItemId)" +
                         "	WHERE ModuleId = @2";
            using (IDataContext ctx = DataContext.Instance())
            {
                ctx.Execute(CommandType.Text, sql, faqId1, faqId2, moduleId);
            }
        }

        /// <summary>
        /// Increments the view count for one FAQ record.
        /// </summary>
        /// <param name="faqId">The ItemID of the FAQ to increment.</param>
        public void IncrementViewCount(int faqId)
        {
            FAQsInfo faq = GetFAQ(faqId);
            faq.ViewCount++;
            UpdateFAQ(faq);
        }

        #endregion

        #region Public Category Methods

        /// <summary>
        /// Gets a category.
        /// </summary>
        /// <param name="faqCategoryId">The id of the category to return.</param>
        /// <returns>Category info object</returns>
        public CategoryInfo GetCategory(int? faqCategoryId)
        {
            CategoryInfo category;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<CategoryInfo>();
                category = rep.GetById(faqCategoryId);
            }
            return category;
        }

        /// <summary>
        /// Retrieves all or only used categories for a module (unsorted).
        /// </summary>
        /// <param name="moduleId">The module id.</param>
        /// <param name="onlyUsedCategories">true if only categories should be returned that are used in Faq's</param>
        /// <returns>IEnumerable of CategoryInfo objects</returns>
        public IEnumerable<CategoryInfo> ListCategories(int moduleId, bool onlyUsedCategories)
        {
            IEnumerable<CategoryInfo> categories;
            string sql = "SELECT [FaqCategoryId], [ModuleId]," +
                         " CASE WHEN [FaqCategoryParentId] IS NULL THEN 0 ELSE [FaqCategoryParentId] END AS [FaqCategoryParentId]," +
                         " [FaqCategoryName], [FaqCategoryDescription],0 As [Level],[ViewOrder]" +
                         " FROM {databaseOwner}[{objectQualifier}FAQsCategory] " +
                         " WHERE [ModuleId] = @0" +
                         " AND ([FaqCategoryId] IN (SELECT CategoryId FROM {databaseOwner}[{objectQualifier}FAQs]) OR @1=0)";

            using (IDataContext ctx = DataContext.Instance())
            {
                categories = ctx.ExecuteQuery<CategoryInfo>(CommandType.Text, sql, moduleId, onlyUsedCategories);
            }
            return categories;
        }

        /// <summary>
        /// Retrieves all or only used categories hierarchical (additional level info, sorted).
        /// </summary>
        /// <param name="moduleId">The module id.</param>
        /// <param name="onlyUsedCategories">true if only categories are returned that are used in Faq's</param>
        /// <returns>IEnumerable of CategoryInfo objects</returns>
        public IEnumerable<CategoryInfo> ListCategoriesHierarchical(int moduleId, bool onlyUsedCategories)
        {
            IEnumerable<CategoryInfo> categories;
            string sql = "{objectQualifier}FAQCategoryListHierarchical";

            using (IDataContext ctx = DataContext.Instance())
            {
                categories = ctx.ExecuteQuery<CategoryInfo>(CommandType.StoredProcedure, sql, moduleId, onlyUsedCategories);
            }

            // Reorder by hierarchy
            var orderedCats = new List<CategoryInfo>();
            var firstLevelCats = categories.Where(c => c.Level == 0);
            foreach(var firstLevelCat in firstLevelCats)
            {
                orderedCats.Add(firstLevelCat);
                orderedCats.AddRange(GetChildrenCategories(categories, firstLevelCat));
            }

            return orderedCats;
        }

        /// <summary>
        /// Adds the category.
        /// </summary>
        /// <param name="category">category info object to be inserted in the category table.</param>
        /// <returns>new category id</returns>
        public int AddCategory(CategoryInfo category)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<CategoryInfo>();
                if (category.FaqCategoryParentId == 0)
                    category.FaqCategoryParentId = null;
                rep.Insert(category);
                return category.FaqCategoryId;
            }
        }

        /// <summary>
        /// Updates a category.
        /// </summary>
        /// <param name="category">category info object to be inserted in the category table.</param>
        public void UpdateCategory(CategoryInfo category)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<CategoryInfo>();
                if (category.FaqCategoryParentId == 0)
                    category.FaqCategoryParentId = null;
                rep.Update(category);
            }
        }

        /// <summary>
        /// Deletes a category.
        /// </summary>
        /// <param name="faqCategoryId">id of category to be deleted</param>
        public void DeleteCategory(int faqCategoryId)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<CategoryInfo>();
                rep.Delete("WHERE FaqCategoryId = @0", faqCategoryId);
            }
        }

        /// <summary>
        /// Renumbering the vieworder field for one specific parent category
        /// </summary>
        /// <param name="faqParentCategoryId">ID of parent category, whos childs should be renumbered</param>
        /// <param name="moduleId">The Module id</param>
        public void ReorderCategory(int? faqParentCategoryId, int moduleId)
        {
            string sql = "WITH tmpReorder(ViewOrder,FAQCategoryId) AS" +
                         " (" +
                         "  SELECT TOP 1000 row_number() OVER (ORDER BY f.ViewOrder) as rank, f.FAQCategoryId" +
                         "  FROM {databaseOwner}[{objectQualifier}FAQsCategory] f" +
                         "  WHERE f.ModuleId = @0" +
                         "  AND f.FaqCategoryParentId " + (faqParentCategoryId == 0 ? " IS NULL" : " = @1") +
                         "  ORDER BY rank " +
                         " )" +
                         " UPDATE {databaseOwner}[{objectQualifier}FAQsCategory] " +
                         " SET ViewOrder = r.ViewOrder " +
                         " FROM tmpReorder r " +
                         " WHERE r.FAQCategoryId = {databaseOwner}[{objectQualifier}FAQsCategory].FAQCategoryId" +
                         " AND ModuleId = @0" +
                         " AND FaqCategoryParentId " + (!faqParentCategoryId.HasValue || faqParentCategoryId == 0 ? " IS NULL" : " = @1");

            using (IDataContext ctx = DataContext.Instance())
            {
                ctx.Execute(CommandType.Text, sql, moduleId, faqParentCategoryId);
            }
        }
        #endregion

        #region Optional Interfaces

        public override IList<SearchDocument> GetModifiedSearchDocuments(ModuleInfo modInfo, DateTime beginDate)
        {
            var searchDocuments = new List<SearchDocument>();

            var FAQs = ListFAQ(Convert.ToInt32(modInfo.ModuleID), 0, true);

            foreach (object objFaq in FAQs)
            {
                var faq = ((FAQsInfo)objFaq);
                if (faq.FaqHide)
                    continue;

                // Remove all HTML overhead from the content
                string strContent = HtmlUtils.Clean(faq.Answer, false);

                // And create an entry that will fit
                string strDescription = HtmlUtils.Clean(faq.Question, false);
                strDescription = strDescription.Length <= MAX_DESCRIPTION_LENGTH ? strDescription : HtmlUtils.Shorten(strDescription, 100, "...");

                var searchDoc = new SearchDocument
                {
                    UniqueKey = String.Format("faqid={0}", faq.ItemID),
                    PortalId = modInfo.PortalID,
                    Title = modInfo.ModuleTitle,
                    Description = strDescription,
                    Body = strContent,
                    ModifiedTimeUtc = DateTime.Now.ToUniversalTime() // faq.DateModified.ToUniversalTime()
                };

                searchDocuments.Add(searchDoc);

            }

            return searchDocuments;
        }


        ///// <summary>
        ///// Retrieve the search item collection (ISearchable interface).
        ///// </summary>
        ///// <param name="modInfo">Module info object</param>
        ///// <returns>Collection of SearchItems</returns>
        //public SearchItemInfoCollection GetSearchItems(ModuleInfo modInfo)
        //{
        //    var searchItemCollection = new SearchItemInfoCollection();
        //    var FAQs = ListFAQ(Convert.ToInt32(modInfo.ModuleID), 0, true);

        //    foreach (object objFaq in FAQs)
        //    {
        //        var faq = ((FAQsInfo)objFaq);
        //        if (faq.FaqHide)
        //            continue;

        //        int UserId = Null.NullInteger;
        //        int.TryParse(faq.CreatedByUser, out UserId);

        //        // Remove all HTML overhead from the content
        //        string strContent = HtmlUtils.Clean(faq.Question + " " + faq.Answer, false);

        //        // And create an entry that will fit
        //        string strDescription = HtmlUtils.Clean(faq.Question, false);
        //        strDescription = strDescription.Length <= MAX_DESCRIPTION_LENGTH ? strContent : HtmlUtils.Shorten(strContent, 100, "...");

        //        string guid = String.Format("faqid={0}", faq.ItemID);

        //        var searchItem = new SearchItemInfo(modInfo.ModuleTitle, strDescription, UserId,
        //            faq.DateModified, modInfo.ModuleID, faq.ItemID.ToString(), strContent, guid);
        //        searchItemCollection.Add(searchItem);
        //    }
        //    return searchItemCollection;
        //}

        /// <summary>
        /// Exports the module (IPortable interface).
        /// </summary>
        /// <param name="moduleID">The module ID.</param>
        /// <returns>XML output</returns>
        public string ExportModule(int moduleID)
        {
            IEnumerable arrCats = ListCategoriesHierarchical(moduleID, false);
            var categorys = new XElement("categories", from CategoryInfo cat in arrCats
                                                       select new XElement("cat",
                                                           new XElement("categoryid", cat.FaqCategoryId),
                                                           new XElement("categoryparentid", cat.FaqCategoryParentId),
                                                           new XElement("categoryname", new XCData(cat.FaqCategoryName)),
                                                           new XElement("categorydescription", new XCData(cat.FaqCategoryDescription)),
                                                           new XElement("vieworder", cat.ViewOrder)));

            IEnumerable<FAQsInfo> arrFAQs = ListFAQ(moduleID, 0, true);
            var faqs = new XElement("faqs", from FAQsInfo faq in arrFAQs
                                            select new XElement("faq",
                                                new XElement("question", new XCData(faq.Question)),
                                                new XElement("answer", new XCData(faq.Answer)),
                                                new XElement("categoryid", faq.CategoryId),
                                                new XElement("creationdate", faq.CreatedDate),
                                                new XElement("datemodified", faq.DateModified),
                                                new XElement("faqhide", faq.FaqHide),
                                                new XElement("publishdate", faq.PublishDate),
                                                new XElement("expiredate", faq.ExpireDate),
                                                new XElement("vieworder", faq.ViewOrder)));



            XElement root = new XElement("Root");
            root.Add(faqs);
            root.Add(categorys);
            return root.ToString();
        }


        /// <summary>
        /// Imports the module (IPortable interface)
        /// </summary>
        /// <param name="moduleID">The module ID.</param>
        /// <param name="content">The content.</param>
        /// <param name="version">The version.</param>
        /// <param name="userId">The user id.</param>
        public void ImportModule(int moduleID, string content, string version, int userId)
        {
            Version vers = new Version(version);
            if (vers > new Version("5.0.0"))
            {
                XElement xRoot = XElement.Parse(content);
                Dictionary<int, int> idTrans = new Dictionary<int, int>();

                // First we import the categories
                List<CategoryInfo> lstCategories = new List<CategoryInfo>();
                XElement xCategories = xRoot.Element("categories");
                foreach (var xCategory in xCategories.Elements())
                {
                    // translate the parentid's to new values
                    int oldParentId = Int32.Parse(xCategory.Element("categoryparentid").Value, CultureInfo.InvariantCulture);
                    int newParentId = 0;
                    if (oldParentId > 0 && idTrans.ContainsKey(oldParentId))
                        newParentId = idTrans[oldParentId];

                    // Fill category properties
                    CategoryInfo category = new CategoryInfo();
                    category.ModuleId = moduleID;
                    category.FaqCategoryParentId = newParentId;
                    category.FaqCategoryName = xCategory.Element("categoryname").Value;
                    category.FaqCategoryDescription = xCategory.Element("categorydescription").Value;
                    category.ViewOrder = Int32.Parse(xCategory.Element("vieworder").Value, CultureInfo.InvariantCulture);

                    // add category and save old and new id to translation dictionary
                    int oldCategoryId = Int32.Parse(xCategory.Element("categoryid").Value, CultureInfo.InvariantCulture);
                    int newCategoryId = AddCategory(category);
                    idTrans.Add(oldCategoryId, newCategoryId);
                }

                // Next import the faqs
                List<FAQsInfo> lstFaqs = new List<FAQsInfo>();
                XElement xFaqs = xRoot.Element("faqs");
                foreach (var xFaq in xFaqs.Elements())
                {
                    // translate id with help of translation dictionary build before
                    int oldCategoryId = -1;
                    int? newCategoryId = null;

                    Int32.TryParse(xFaq.Element("categoryid").Value, out oldCategoryId);
                    
                    if (oldCategoryId > 0 && idTrans.ContainsKey(oldCategoryId))
                        newCategoryId = idTrans[oldCategoryId];

                    // Fill FAQs properties
                    FAQsInfo faq = new FAQsInfo();
                    faq.ModuleID = moduleID;
                    faq.Question = xFaq.Element("question").Value;
                    faq.Answer = xFaq.Element("answer").Value;
                    faq.CategoryId = newCategoryId;
                    faq.CreatedDate = DateTime.Parse(xFaq.Element("creationdate").Value);
                    faq.DateModified = DateTime.Now;
                    faq.FaqHide = Boolean.Parse(xFaq.Element("faqhide").Value);

                    // These dates might be emtpy
                    try
                    {
                        faq.PublishDate = DateTime.Parse(xFaq.Element("publishdate").Value);
                        if (faq.PublishDate.HasValue && faq.PublishDate.Value == Null.NullDate)
                        {
                            faq.PublishDate = null;
                        }
                    }
                    catch (Exception)
                    {
                        faq.PublishDate = null;
                    }

                    try
                    {
                        faq.ExpireDate = DateTime.Parse(xFaq.Element("expiredate").Value);
                        if (faq.ExpireDate.HasValue && faq.ExpireDate.Value == Null.NullDate)
                        {
                            faq.ExpireDate = null;
                        }
                    }

                    catch (Exception)
                    {
                        faq.ExpireDate = null;
                    }

                    // Add Faq to database
                    AddFAQ(faq);
                }
            }
            else
            {
                ArrayList catNames = new ArrayList();
                ArrayList Question = new ArrayList();
                XmlNode xmlFaq;
                XmlNode xmlFaqs = Globals.GetContent(content, "faqs");

                //' check if category exists. if not create category
                foreach (XmlNode tempLoopVar_xmlFAQ in xmlFaqs)
                {
                    xmlFaq = tempLoopVar_xmlFAQ;
                    if ((xmlFaq["catname"].InnerText != Null.NullString) && (!catNames.Contains(xmlFaq["catname"].InnerText)))
                    {
                        catNames.Add(xmlFaq["catname"].InnerText);

                        CategoryInfo objCat = new CategoryInfo();
                        objCat.ModuleId = moduleID;
                        objCat.FaqCategoryName = xmlFaq["catname"].InnerText;
                        objCat.FaqCategoryDescription = xmlFaq["catdescription"].InnerText;

                        AddCategory(objCat);
                    }
                }
                // check is question is empty. if empty is category.
                int loop = 0;
                foreach (XmlNode tempLoopVar_xmlFAQ in xmlFaqs)
                {
                    loop++;
                    xmlFaq = tempLoopVar_xmlFAQ;

                    if (xmlFaq["question"].InnerText != Null.NullString && xmlFaq["question"].InnerText != string.Empty)
                    {
                        FAQsInfo objFAQs = new FAQsInfo();
                        objFAQs.ModuleID = moduleID;
                        objFAQs.Question = xmlFaq["question"].InnerText;
                        objFAQs.Answer = xmlFaq["answer"].InnerText;
                        string faqCategoryName = xmlFaq["catname"].InnerText;

                        // Check if creationdate exists in export, if nothing set current date. else import
                        if (xmlFaq["creationdate"] == null)
                        {
                            objFAQs.CreatedDate = DateTime.Now;
                            objFAQs.DateModified = DateTime.Now;
                        }
                        else
                        {
                            objFAQs.CreatedDate = DateTime.Parse(xmlFaq["creationdate"].InnerText);
                            objFAQs.DateModified = DateTime.Parse(xmlFaq["datemodified"].InnerText);
                        }

                        if (xmlFaq["vieworder"] != null)
                        {
                            objFAQs.ViewOrder = int.Parse(xmlFaq["vieworder"].InnerText);
                        }
                        else
                        {
                            objFAQs.ViewOrder = loop;
                        }
                        objFAQs.CreatedByUser = userId.ToString();

                        bool foundCat = false;
                        foreach (CategoryInfo objCat in ListCategories(moduleID, false))
                        {
                            if (faqCategoryName == objCat.FaqCategoryName)
                            {
                                objFAQs.CategoryId = objCat.FaqCategoryId;
                                foundCat = true;
                                break;
                            }
                        }

                        if (!foundCat)
                        {
                            objFAQs.CategoryId = null;
                        }

                        AddFAQ(objFAQs);
                    }
                }
            }

        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Processes the faq tokens.
        /// </summary>
        /// <param name="faqItem">The FAQ item.</param>
        /// <param name="template">The template.</param>
        /// <returns>Answers in which all tokens are processed</returns>
        public string ProcessTokens(FAQsInfo faqItem, string template)
        {
            // For compability issues we need to convert old tokens to new tokens (sigh...)
            StringBuilder compatibleTemplate = new StringBuilder(template);
            compatibleTemplate.Replace("[ANSWER]", "[FAQ:ANSWER]");
            compatibleTemplate.Replace("[CATEGORYNAME]", "[FAQ:CATEGORYNAME]");
            compatibleTemplate.Replace("[CATEGORYDESC]", "[FAQ:CATEGORYDESC]");
            compatibleTemplate.Replace("[USER]", "[FAQ:USER]");
            compatibleTemplate.Replace("[VIEWCOUNT]", "[FAQ:VIEWCOUNT]");
            compatibleTemplate.Replace("[DATECREATED]", "[FAQ:DATECREATED]");
            compatibleTemplate.Replace("[DATEMODIFIED]", "[FAQ:DATEMODIFIED]");
            compatibleTemplate.Replace("[QUESTION]", "[FAQ:QUESTION]");
            compatibleTemplate.Replace("[INDEX]", "[FAQ:INDEX]");

            // Now we can call TokenReplace
            FAQsTokenReplace tokenReplace = new FAQsTokenReplace(faqItem);
            return tokenReplace.ReplaceFAQsTokens(template);
        }

        private List<CategoryInfo> GetChildrenCategories(IEnumerable<CategoryInfo> categories, CategoryInfo category)
        {
            var childrens = new List<CategoryInfo>();
            foreach (var subCategory in categories.Where(c => c.FaqCategoryParentId == category.FaqCategoryId))
            {
                childrens.Add(subCategory);
                childrens.AddRange(GetChildrenCategories(categories, subCategory));
            }
            return childrens;
        }

        #endregion
    }

}
