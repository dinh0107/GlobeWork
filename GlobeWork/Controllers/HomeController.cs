using Helpers;
using GlobeWork.DAL;
using GlobeWork.Filters;
using GlobeWork.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace GlobeWork.Controllers
{
    [MemberLoginFilter]
    public class HomeController : Controller
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();
        private static string Email => WebConfigurationManager.AppSettings["email"];
        private static string Password => WebConfigurationManager.AppSettings["password"];
        public ConfigSite ConfigSite => (ConfigSite)HttpContext.Application["ConfigSite"];

        public PartialViewResult Header()
        {
            return PartialView();
        }
        public PartialViewResult Footer()
        {
            return PartialView();
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult StudyAbroad()
        {
            return View();
        }
        public ActionResult StudyAbroadCategory()
        {
            return View();
        }
        public ActionResult StudyAbroadDetail()
        {
            return View();
        }
         public ActionResult Job()
        {
            return View();
        }
        public ActionResult JobDetail()
        {
            return View();
        }
        public ActionResult Employer()
        {
            return View();
        }
        public ActionResult Recruitment()
        {
            return View();
        }

        public ActionResult ListCompany()
        {
            return View();
        }
        public ActionResult CompanyTop()
        {
            return View();
        }

            
        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }
    }
}