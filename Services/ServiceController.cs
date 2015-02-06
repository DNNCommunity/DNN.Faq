using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DotNetNuke.Web.Api;
//using DotNetNuke.Modules.FAQs;

namespace DotNetNuke.Modules.FAQs.Services
{
    public class ServiceController : DnnApiController
    {
        public class GetCategoriesDTO
        {
            public int ModuleId { get; set; }
            public bool OnlyUsedCategories { get; set; }
        }

        public class GetFaqDTO
        {
            public int FaqId { get; set; }
        }

        public class ListFaqDTO
        {
            public int ModuleId { get; set; }
            public int OrderBy { get; set; }
            public bool ShowHidden { get; set; }
        }

        
        [AllowAnonymous] 
        [HttpPost]
        public HttpResponseMessage GetCategories(GetCategoriesDTO dto, string output)
        {
            FAQsController controller = new FAQsController();
            return GenerateOutput(controller.ListCategoriesHierarchical(dto.ModuleId,dto.OnlyUsedCategories),output);
        }

        [AllowAnonymous]
        [HttpPost]
        public HttpResponseMessage GetFaq(GetFaqDTO dto, string output)
        {
            FAQsController controller = new FAQsController();
            return GenerateOutput(controller.GetFAQ(dto.FaqId), output);
        }

        [AllowAnonymous]
        [HttpPost]
        public HttpResponseMessage ListFaq(ListFaqDTO dto, string output)
        {
            FAQsController controller = new FAQsController();
            return GenerateOutput(controller.ListFAQ(dto.ModuleId,dto.OrderBy,dto.ShowHidden), output);
        }

        [RequireHost]
        [HttpGet]
        public HttpResponseMessage HelloWorld()
        {
            return Request.CreateResponse(HttpStatusCode.OK, "Hello Service World!");
        }

        private HttpResponseMessage  GenerateOutput (object results, string output)
        {
            switch (output)
            {
                case "xml":
                    return Request.CreateResponse(HttpStatusCode.OK, results, "application/xml");
                case "json":
                    return Request.CreateResponse(HttpStatusCode.OK, results, "application/json");
                default:
                    return Request.CreateResponse(HttpStatusCode.OK,results);

            }
            
        }
    }
} 