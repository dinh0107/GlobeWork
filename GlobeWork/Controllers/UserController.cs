using Helpers;
using GlobeWork.DAL;
using GlobeWork.Models;
using GlobeWork.Filters;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Facebook;
using GoogleAuthentication.Services;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Web.Security;
using System.Net;
using RestSharp;
using Newtonsoft.Json;
using GlobeWork.ViewModel;
namespace GlobeWork.Controllers
{
    [MemberFilter, RoutePrefix("user")]
    public class UserController : Controller
    {
        // GET: User
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();
        private string Email => RouteData.Values["Email"].ToString();
        private new User User => _unitOfWork.UserRepository.Get(a => Email == Email).SingleOrDefault();
        public SelectList CitySelectList => new SelectList(_unitOfWork.CityRepository.Get(a => a.Active, q => q.OrderBy(a => a.Sort)), "Id", "Name");
        public SelectList DistrictSelectList(int? cityId) => new SelectList(_unitOfWork.DistrictRepository.Get(a => a.Active && a.CityId == cityId, q => q.OrderBy(a => a.Sort)), "Id", "Name");
        public ConfigSite ConfigSite => (ConfigSite)HttpContext.Application["ConfigSite"];


        [OverrideActionFilters, Route("dang-nhap")]
        public ActionResult Login()
        {
            var host = Request.Url.Scheme + "://" + Request.Url.Host + ":" + Request.Url.Port;
            //Facebook
            var fb = new FacebookClient();
            var facebookUrl = fb.GetLoginUrl(new
            {
                client_id = "872873711287852",
                redirect_uri = host + "/User/FacebookRedirect",
                scope = "public_profile,email"
            });
            //Google
            var clientId = "65707280933-nr9hupgoj3pbqavfc479hll1igo0jfkk.apps.googleusercontent.com";
            var url = host + "/User/CallBackGoogle";
            var respon = GoogleAuth.GetAuthUrl(clientId, url);
            // Linkend
            ViewBag.Linkedin = LinkedinLogin.LinkedinPackage.RedirectToLinkedinLoginUrl("7896gkl097iy60", "https://localhost:44327/User/CallBackLinked");
            ViewBag.GoogleUrl = respon;
            ViewBag.FacebookUrl = facebookUrl;
            return View();
        }
        [HttpPost, Route("dang-nhap"), OverrideActionFilters]
        public ActionResult Login(UserLoginModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _unitOfWork.UserRepository.Get(a => a.Email == model.Email).SingleOrDefault();
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
                var userData =  user.Avatar + "|" + user.Id + "|" + user.Email + "|" + user.FullName;
                var ticket = new FormsAuthenticationTicket(2, user.Email.ToLower(), DateTime.Now, DateTime.Now.AddDays(30), true,
                    userData, FormsAuthentication.FormsCookiePath);
                var encTicket = FormsAuthentication.Encrypt(ticket);
                Response.Cookies.Add(new HttpCookie(".ASPXAUTHMEMBER", encTicket));
                return RedirectToAction("Index", "Home", new { result = "success" });
            }
            return View(model);
        }
        [OverrideActionFilters]
        public ActionResult FacebookRedirect(string code)
        {
            var fb = new FacebookClient();
            dynamic result = fb.Get("/oauth/access_token", new
            {
                client_id = "872873711287852",
                client_secret = "a375a6962d4a46fbd920dd29d5acf604",
                redirect_uri = "https://localhost:44327/User/FacebookRedirect",
                code = code
            });
            fb.AccessToken = result.access_token;
            dynamic me = fb.Get("/me?fields=name,email");
            string name = me.name;
            string email = me.email;
            string id = me.id;
            string avatar = me.avatar;
            var user = _unitOfWork.UserRepository.GetQuery(a => a.FaceBookId == id).FirstOrDefault();
            if(user == null)
            {
                var Insertuser = new User
                {
                    Username = "",
                    Avatar = avatar,
                    FaceBookId = id,
                    Email = email,
                    FullName = name,
                    TypeRegister = TypeRegister.Facebook,
                };
                _unitOfWork.UserRepository.Insert(Insertuser);
                _unitOfWork.Save();
                var userData = Insertuser.Username + "|" + Insertuser.Avatar + "|" + Insertuser.Id + "|" + Insertuser.Email + "|" + Insertuser.FullName;
                var ticket = new FormsAuthenticationTicket(2, Insertuser.Email.ToLower(), DateTime.Now, DateTime.Now.AddDays(30), true,
                    userData, FormsAuthentication.FormsCookiePath);
                var encTicket = FormsAuthentication.Encrypt(ticket);
                Response.Cookies.Add(new HttpCookie(".ASPXAUTHMEMBER", encTicket));
                return RedirectToAction("Index", "Home");
            }
            else
            {
                var userData = user.Username + "|" + user.Avatar + "|" + user.Id + "|" + user.Email + "|" + user.FullName;
                var ticket = new FormsAuthenticationTicket(2, user.Email.ToLower(), DateTime.Now, DateTime.Now.AddDays(30), true,
                    userData, FormsAuthentication.FormsCookiePath);
                var encTicket = FormsAuthentication.Encrypt(ticket);
                Response.Cookies.Add(new HttpCookie(".ASPXAUTHMEMBER", encTicket));
                return RedirectToAction("Index", "Home");
            }
        }

        [OverrideActionFilters]
        public async Task<ActionResult> CallBackGoogle(string code)
        {
            var host = Request.Url.Scheme + "://" + Request.Url.Host + ":" + Request.Url.Port;
            var client_id = "65707280933-nr9hupgoj3pbqavfc479hll1igo0jfkk.apps.googleusercontent.com";
            var url = host + "/User/CallBackGoogle";
            var client_secret = "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";
            var token = await GoogleAuth.GetAuthAccessToken(code, client_id, client_secret, url);
            var userProfile = await GoogleAuth.GetProfileResponseAsync(token.AccessToken.ToString());
            JObject jsonObject = JObject.Parse(userProfile);
            string id = jsonObject["id"]?.ToString();
            string email = jsonObject["email"]?.ToString();
            var count = _unitOfWork.UserRepository.GetQuery(a => a.Email == email).Count();
            if(count >= 1)
            {
                var user = _unitOfWork.UserRepository.GetQuery(a => a.Email == email).FirstOrDefault();
                var userData = user.Avatar + "|" + user.Id + "|" + user.Email + "|" + user.FullName;
                var ticket = new FormsAuthenticationTicket(2, user.Email.ToLower(), DateTime.Now, DateTime.Now.AddDays(30), true,
                    userData, FormsAuthentication.FormsCookiePath);
                var encTicket = FormsAuthentication.Encrypt(ticket);
                Response.Cookies.Add(new HttpCookie(".ASPXAUTHMEMBER", encTicket));
                return RedirectToAction("Index", "Home");
            }
            else
            {
                var user = _unitOfWork.UserRepository.GetQuery(a => a.GoogleId == id).FirstOrDefault();
                if (user == null)
                {
                    var Insertuser = new User
                    {
                        Username = "",
                        Avatar = "",
                        GoogleId = id,
                        Email = email,
                        FullName = jsonObject["name"]?.ToString(),
                        TypeRegister = TypeRegister.Google,
                    };
                    _unitOfWork.UserRepository.Insert(Insertuser);
                    _unitOfWork.Save();
                    var userData = Insertuser.Avatar + "|" + Insertuser.Id + "|" + Insertuser.Email + "|" + Insertuser.FullName;
                    var ticket = new FormsAuthenticationTicket(2, Insertuser.Email.ToLower(), DateTime.Now, DateTime.Now.AddDays(30), true,
                        userData, FormsAuthentication.FormsCookiePath);
                    var encTicket = FormsAuthentication.Encrypt(ticket);
                    Response.Cookies.Add(new HttpCookie(".ASPXAUTHMEMBER", encTicket));
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    var userData = user.Avatar + "|" + user.Id + "|" + user.Email + "|" + user.FullName;
                    var ticket = new FormsAuthenticationTicket(2, user.Email.ToLower(), DateTime.Now, DateTime.Now.AddDays(30), true,
                        userData, FormsAuthentication.FormsCookiePath);
                    var encTicket = FormsAuthentication.Encrypt(ticket);
                    Response.Cookies.Add(new HttpCookie(".ASPXAUTHMEMBER", encTicket));
                    return RedirectToAction("Index", "Home");
                }
            }
        }
        [OverrideActionFilters]
        public ActionResult CallBackLinked(string code , string state)
        {
            var host = Request.Url.Scheme + "://" + Request.Url.Host + ":" + Request.Url.Port;
            try
            {
                var client = new RestClient("https://www.linkedin.com/oauth/v2/accessToken");
                var request = new RestRequest(Method.Post.ToString());
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                request.AddParameter("grant_type", "authorization_code");
                request.AddParameter("code", Request.QueryString["code"].ToString());
                request.AddParameter("redirect_uri", "https://localhost:44327/User/CallBackLinked");
                request.AddParameter("client_id", "7896gkl097iy60");
                request.AddParameter("client_secret", "ClgYHvkUDssxb30f");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View();
        }
        [Route("dang-xuat")]
        public ActionResult Logout()
        {
            var cookie = Request.Cookies[".ASPXAUTHMEMBER"];
            if (cookie != null)
            {
                cookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(cookie);
            }
            return RedirectToAction("Index", "Home");
        }

        [OverrideActionFilters, Route("dang-ky")]
        public ActionResult Register()
        {
            var host = Request.Url.Scheme + "://" + Request.Url.Host + ":" + Request.Url.Port;
            var fb = new FacebookClient();
            var facebookUrl = fb.GetLoginUrl(new
            {
                client_id = "872873711287852",
                redirect_uri = host + "/User/FacebookRedirect",
                scope = "public_profile,email"
            });
            var clientId = "65707280933-nr9hupgoj3pbqavfc479hll1igo0jfkk.apps.googleusercontent.com";
            var url = host + "/User/CallBackGoogle";
            var respon = GoogleAuth.GetAuthUrl(clientId, url);
            ViewBag.GoogleUrl = respon;
            ViewBag.FacebookUrl = facebookUrl;
            return View();
        }
        [Route("dang-ky")]
        [HttpPost, ValidateAntiForgeryToken, OverrideActionFilters]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var checkUser = _unitOfWork.UserRepository.GetQuery(a => a.Email.Equals(model.Email)).SingleOrDefault();
                if (checkUser != null)
                {
                    ModelState.AddModelError("", @"Email đã được sử dụng!! Vui lòng nhập Email khác");
                }
                else
                {
                    var hashPass = HtmlHelpers.ComputeHash(model.Password, "SHA256", null);
                    var user = new User
                    {
                        FullName = model.FullName,
                        Password = hashPass,
                        Email = model.Email,
                        Active = true,
                        TypeRegister = TypeRegister.Website,
                    };
                    _unitOfWork.UserRepository.Insert(user);
                    _unitOfWork.Save();
                    var userData = user.Avatar + "|" + user.Id + "|" + user.Email + "|" + user.FullName;
                    var ticket = new FormsAuthenticationTicket(2, user.Email.ToLower(), DateTime.Now, DateTime.Now.AddDays(30), true,
                        userData, FormsAuthentication.FormsCookiePath);
                    var encTicket = FormsAuthentication.Encrypt(ticket);
                    Response.Cookies.Add(new HttpCookie(".ASPXAUTHMEMBER", encTicket));
                    return RedirectToAction("Index", "Home", new { result = "success" });
                }
            }
            return View(model);
        }
        
        [Route("quen-mat-khau")]
        public ActionResult ForgotPassword(string result = "")
        {
            ViewBag.Result = result;
            return View();
        }
        [HttpPost, Route("quen-mat-khau")]
        public ActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            var email = _unitOfWork.UserRepository.GetQuery(a => a.Active && a.Email == model.Email).FirstOrDefault();
            if (email == null)
            {
                return RedirectToAction("ForgotPassword", new { result = "error" });
            }
            var setNewPasswordUrl = Request.Url?.GetLeftPart(UriPartial.Authority) + Url.Action("SetNewPasswordUrl", new { token = email.Token });
            var emailTemp = System.IO.File.ReadAllText(Server.MapPath("/EmailTemplates/ForgotPassword.html"));
            emailTemp = emailTemp.Replace("[FULLNAME]", email.FullName).Replace("[EMAIL]", model.Email).Replace("[URL]", setNewPasswordUrl);
            Task.Run(() => HtmlHelpers.SendEmail("gmail", "Yêu cầu lấy lại mật khẩu từ " + Request.Url?.Host, emailTemp, model.Email, ConfigSite.EmailConfigs, ConfigSite.EmailConfigs, ConfigSite.PassWordMail, ConfigSite.Title));
            return RedirectToAction("ForgotPassword", new { result = "sucsess" });
        }
        [Route("dat-lai-mat-khau")]
        public ActionResult SetNewPasswordUrl(string token, string result = "")
        {
            ViewBag.Result = result;

            var model = new SetNewPasswordViewModel
            {
                Token = token
            };
            return View(model);
        }
        [OverrideActionFilters]
        [Route("dat-lai-mat-khau"), HttpPost, ValidateAntiForgeryToken]
        public ActionResult SetNewPasswordUrl(SetNewPasswordViewModel model )
        {
            if (ModelState.IsValid)
            {
                var user = _unitOfWork.UserRepository.GetQuery(a => a.Active && a.Token == model.Token).FirstOrDefault();
                if (user == null)
                {
                    return RedirectToAction("Login");
                }
                user.Password = HtmlHelpers.ComputeHash(model.Password, "SHA256", null);
                user.Token = HtmlHelpers.RandomCode(50);
                _unitOfWork.Save();

                return RedirectToAction("SetNewPasswordUrl", new { result = "sucsess" });
            }
            return View(model);
        }
        [Route("chi-tiet-tai-khoan")]
        public ActionResult UserProfile()
        {
            var id = Convert.ToInt32(RouteData.Values["Id"]);
            var user = _unitOfWork.UserRepository.GetQuery(a => a.Id == id).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var model = new UserInfoViewModel
            {
                User = user,
            };
            return View(model);
        }

        public PartialViewResult InfoUser(int id)
        {
            var user = _unitOfWork.UserRepository.GetById(id);
            if (user == null)
            {
                return PartialView();
            }
            return PartialView(user);
        }
        [OverrideActionFilters]
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