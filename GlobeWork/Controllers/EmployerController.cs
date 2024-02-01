using GlobeWork.DAL;
using GlobeWork.Filters;
using GlobeWork.Models;
using GlobeWork.ViewModel;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI.WebControls;

namespace GlobeWork.Controllers
{
    [EmployerFilter, RoutePrefix("app")]
    public class EmployerController : Controller
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();
        private string Email => RouteData.Values["Email"].ToString();
        private new Employer User => _unitOfWork.EmployerRepository.Get(a => Email == Email).SingleOrDefault();
        public SelectList CitySelectList => new SelectList(_unitOfWork.CityRepository.Get(a => a.Active, q => q.OrderBy(a => a.Sort)), "Id", "Name");
        public SelectList DistrictSelectList(int? cityId) => new SelectList(_unitOfWork.DistrictRepository.Get(a => a.Active && a.CityId == cityId, q => q.OrderBy(a => a.Sort)), "Id", "Name");
        public ConfigSite ConfigSite => (ConfigSite)HttpContext.Application["ConfigSite"];

        [Route("nha-tuyen-dung")]
        public ActionResult Employer()
        {
            var rank = _unitOfWork.RankRepository.Get();
            var model = new EmployerRegisterViewModel
            {
                Ranks = rank,
                CitySelectList = CitySelectList,
            };
            return View(model);
        }
        [Route("nha-tuyen-dung")]
        [HttpPost, ValidateAntiForgeryToken, OverrideActionFilters]
        public ActionResult Employer(EmployerRegisterViewModel model, FormCollection fc)
        {
            if (ModelState.IsValid)
            {
                var checkUser = _unitOfWork.EmployerRepository.GetQuery(a => a.Email.Equals(model.Email)).SingleOrDefault();
                if (checkUser != null)
                {
                    ModelState.AddModelError("", @"Email đã được sử dụng!! Vui lòng nhập Email khác");
                }
                else
                {
                    var hashPass = HtmlHelpers.ComputeHash(model.Password, "SHA256", null);
                    var user = new Employer
                    {
                        FullName = model.FullName,
                        Password = hashPass,
                        Email = model.Email,
                        Active = true,
                        CompanyName = model.CompanyName,
                        Gender = model.Gender,
                        PhoneNumber = model.PhoneNumber,
                        RankId = Convert.ToInt32(fc["RankId"]),
                        CityId = model.CityId,
                        DistrictId = model.DistrictId,
                        Amount = 0,
                        Avatar = "",
                    };
                    _unitOfWork.EmployerRepository.Insert(user);
                    _unitOfWork.Save();
                    var userData = user.Avatar + "|" + user.Id + "|" + user.Email + "|" + user.FullName;
                    var ticket = new FormsAuthenticationTicket(2, model.Email.ToLower(), DateTime.Now, DateTime.Now.AddDays(30), true,
                        userData, FormsAuthentication.FormsCookiePath);
                    var encTicket = FormsAuthentication.Encrypt(ticket);
                    Response.Cookies.Add(new HttpCookie(".ASPXAUTHEMPLOYER", encTicket));
                    return RedirectToAction("Index", "Employer");
                }
            }
            return View(model);
        }
        [OverrideActionFilters, Route("dang-nhap/nha-tuyen-dung")]
        public ActionResult EmployerLogin()
        {
            return View();
        }
        [HttpPost, Route("dang-nhap/nha-tuyen-dung"), OverrideActionFilters]
        public ActionResult EmployerLogin(UserLoginModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _unitOfWork.EmployerRepository.Get(a => a.Email == model.Email).SingleOrDefault();
                if (user == null || !HtmlHelpers.VerifyHash(model.Password, "SHA256", user.Password))
                {
                    ModelState.AddModelError("", @"Tên đăng nhập hoặc mật khẩu không chính xác.");
                    return View(model);
                }
                if (!user.Active)
                {
                    ModelState.AddModelError("", @"Tài khoản tạm thời bị khóa. Vui lòng liên hệ với quản trị viên.");
                    return View(model);
                }
                var userData = user.Avatar + "|" + user.Id + "|" + user.Email + "|" + user.FullName;
                var ticket = new FormsAuthenticationTicket(2, user.Email.ToLower(), DateTime.Now, DateTime.Now.AddDays(30), true,
                    userData, FormsAuthentication.FormsCookiePath);
                var encTicket = FormsAuthentication.Encrypt(ticket);
                Response.Cookies.Add(new HttpCookie(".ASPXAUTHEMPLOYER", encTicket));
                return RedirectToAction("Index", "Employer", new { result = "success" });
            }
            return View(model);
        }
        [Route("dang-xuat")]
        public ActionResult Logout()
        {
            var cookie = Request.Cookies[".ASPXAUTHEMPLOYER"];
            if (cookie != null)
            {
                cookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(cookie);
            }
            return RedirectToAction("Index", "Home");
        }

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

        public ActionResult Profile()
        {
            return View();
        }

        public ActionResult Service()
        {
            return View();
        }

        public ActionResult ShoppingCart()
        {
            return View();
        }

        public ActionResult Checkout()
        {
            return View();
        }

        public ActionResult PostNews()
        {
            return View();
        }

        public ActionResult OrderTracking()
        {
            return View();
        }

        public ActionResult History()
        {
            return View();
        }
        public JsonResult CheckEmail(string email)
        {
            var user = _unitOfWork.UserRepository.GetQuery(a => a.Email == email).SingleOrDefault();

            if (user == null)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }

            return Json("Email đã được sử dụng, vui lòng thử lại", JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCities(string city = "")
        {
            var cities = _unitOfWork.CityRepository
                .GetQuery(a => a.Active && a.Name.ToLower().Contains(city.ToLower()), q => q.OrderBy(a => a.Sort)).Select(a => new { a.Id, a.Name });
            return Json(cities, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDistrict(int? cityId)
        {
            var districts = _unitOfWork.DistrictRepository
                .GetQuery(a => a.Active && a.CityId == cityId, q => q.OrderBy(a => a.Sort)).Select(a => new { a.Id, a.Name });
            return Json(districts, JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            _unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}