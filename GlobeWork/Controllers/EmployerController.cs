using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace GlobeWork.Controllers
{
    public class EmployerController : Controller
    {
        // GET: Employer

        public PartialViewResult Header()
        {
            return PartialView();
        }

        public PartialViewResult Sidebar()
        {
            return PartialView();
        }
        public PartialViewResult Personal()
        {
            return PartialView();
        }

        public PartialViewResult ChangePassword()
        {
            return PartialView();
        }

        public ActionResult Index()
        {
            return View();
        }

        //public ActionResult Profile()
        //{
        //    return View();
        //}

        public ActionResult Service()
        {
            return View();
        }

        public ActionResult ShoppingCart()
        {
            return View();
        }
    }
}