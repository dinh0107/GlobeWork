using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GlobeWork.Controllers
{
    public class HomeController : Controller
    {
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