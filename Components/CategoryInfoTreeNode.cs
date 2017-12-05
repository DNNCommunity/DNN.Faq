namespace DotNetNuke.Modules.FAQs
{
    internal class CategoryInfoTreeNode
    {
        // treeCategories fails with int? FaqCategoryParentId
        // define a temp class that has no nullables
        // set null ints to Null.NullInt
        public int FaqCategoryParentId { get; set; }
        public int FaqCategoryId { get; set; }
        public int ModuleId { get; set; }
        public string FaqCategoryName { get; set; }
        public string FaqCategoryDescription { get; set; }
        public int Level { get; set; }
        public int ViewOrder { get; set; }

        public CategoryInfoTreeNode()
        {
        }
    }
}