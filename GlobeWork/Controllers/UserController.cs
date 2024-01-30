using Helpers;
using GlobeWork.DAL;
using GlobeWork.Models;
using GlobeWork.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Facebook;
using System.Configuration;
using GoogleAuthentication.Services;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web.Security;

namespace GlobeWork.Controllers
{
    [MemberFilter, RoutePrefix("user")]
    public class UserController : Controller
    {
        // GET: User
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();
        [Route("dang-nhap")]
        public ActionResult Login()
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
            return View();
        }

        [OverrideActionFilters]
        public async Task<ActionResult> CallBackGoogle(string code)
        {
            var client_id = "65707280933-nr9hupgoj3pbqavfc479hll1igo0jfkk.apps.googleusercontent.com";
            var url = "https://localhost:44327/User/CallBackGoogle";
            var client_secret = "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";
            var token = await GoogleAuth.GetAuthAccessToken(code, client_id, client_secret, url);
            var userProfile = await GoogleAuth.GetProfileResponseAsync(token.AccessToken.ToString());
            JObject jsonObject = JObject.Parse(userProfile);
            string id = jsonObject["id"]?.ToString();
            string email = jsonObject["email"]?.ToString();
            var user = _unitOfWork.UserRepository.GetQuery(a => a.GoogleId == id).FirstOrDefault();
            if(user == null) {
                var Insertuser = new User
                {
                    Username = "",
                    Avatar = "",
                    GoogleId = id,
                    Email = email,
                    FullName = jsonObject["name"]?.ToString(),
                    TypeRegister = TypeRegister.Google,
                    TypeUser = TypeUser.User,
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
        [Route("dang-ky")]
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
        public ActionResult ForgotPassword()
        {
            return View();
        }
        public ActionResult UserProfile()
        {
            return View();
        }
        public PartialViewResult InfoUser()
        {
            return PartialView();
        }
    }
}