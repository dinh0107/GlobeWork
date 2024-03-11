using Helpers;
using GlobeWork.DAL;
using GlobeWork.Models;
using GlobeWork.Filters;
using System;
using System.Net;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Facebook;
using GoogleAuthentication.Services;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Web.Security;
using RestSharp;
using GlobeWork.ViewModel;
using System.IO;
using System.Drawing;
using PagedList;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Ajax.Utilities;

namespace GlobeWork.Controllers
{
    [MemberFilter, RoutePrefix("user")]
    public class UserController : Controller
    {
        private const string LinkedInClientId = "86aar6msj716ck";
        private const string LinkedInClientSecret = "86aar6msj716ck";
        private const string RedirectUri = "https://localhost:44327/User/CallBackLinked";
        private static readonly HttpClient _httpClient = new HttpClient();
        public string code = "AQWYllV6BcuSfHDRtpXCd0yY-YlAj_W3iIEbKVXvgDyhf_D2cLq-HjDp92McPFw_pktGvDNG0FVdXD6oDL9N4g4M8Y05-dtK2FNGobhO-lnop-uE0L9Azm3Nx4irqeAbVqhNQJdgRzFBS1HJlBPVXmPgIu32oQq1Joi_IpVvqbjbMccThKf4MDBS3AuJ14A2dCUBWpFNdDv8r8ZDaKFezc1gk0ZxyVxPG6WynYFwlo31aceQAXgvBan4f9cX4xER02iiJT1m-CQNszUFEwIHWdtqnjCPi90wSSoDIuzkjnhQ8XYOUSCo-bwv2QTYNg02Lb5AursP5cm3coZuPbYn20rR8Q3ORA";
        public ActionResult Authenticate()
        {
            var authUrl = $"https://www.linkedin.com/oauth/v2/authorization?response_type=code&client_id={LinkedInClientId}&redirect_uri={RedirectUri}&scope=r_liteprofile%20r_emailaddress";
            return Redirect(authUrl);
        }

        // GET: User
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();
        private int Id => Convert.ToInt32(RouteData.Values["Id"]);
        private new User User => _unitOfWork.UserRepository.GetById(Id);
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
            ViewBag.Url = $"https://www.linkedin.com/oauth/v2/authorization?response_type=code&client_id={LinkedInClientId}&redirect_uri={RedirectUri}&scope=openid,profile,email,w_member_social";
            ViewBag.GoogleUrl = respon;
            ViewBag.FacebookUrl = facebookUrl;
            return View();
        }
        [HttpPost, Route("dang-nhap"), OverrideActionFilters]
        public ActionResult Login(UserLoginModel model, string returnUrl)
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

                var userData = user.Avatar + "|" + user.Id + "|" + user.Email + "|" + user.FullName + "|" + user.Url + "|" + user.AvatarSocial;
                var ticket = new FormsAuthenticationTicket(2, user.Email.ToLower(), DateTime.Now, DateTime.Now.AddDays(30), true,
                    userData, FormsAuthentication.FormsCookiePath);
                var encTicket = FormsAuthentication.Encrypt(ticket);
                Response.Cookies.Add(new HttpCookie(".ASPXAUTHMEMBER", encTicket));

                if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                {
                    return Redirect(returnUrl);
                }

                return RedirectToAction("Index", "Home", new { result = "success" });
            }
            return View(model);
        }

        [OverrideActionFilters]
        public ActionResult FacebookRedirect(string code, string returnUrl)
        {

            var host = Request.Url.Scheme + "://" + Request.Url.Host + ":" + Request.Url.Port;
            var fb = new FacebookClient();
            dynamic result = fb.Get("/oauth/access_token", new
            {
                client_id = "872873711287852",
                client_secret = "a375a6962d4a46fbd920dd29d5acf604",
                redirect_uri = host + "/User/FacebookRedirect",
                code = code
            });
            fb.AccessToken = result.access_token;
            dynamic me = fb.Get("/me?fields=name,email,picture");
            string name = me.name;
            string email = me.email;
            string id = me.id;
            string avatar = me.picture.data.url;
            var user = _unitOfWork.UserRepository.GetQuery(a => a.FaceBookId == id).FirstOrDefault();
            if (user == null)
            {
                var Insertuser = new User
                {
                    Username = "",
                    Avatar = null,
                    AvatarSocial = avatar,
                    Cover = null,
                    FaceBookId = id,
                    Email = email,
                    FullName = name,
                    Url = HtmlHelpers.ConvertToUnSign(null, name),
                    TypeRegister = TypeRegister.Facebook,
                };
                _unitOfWork.UserRepository.Insert(Insertuser);
                _unitOfWork.Save();
                var countUrl = _unitOfWork.UserRepository.GetQuery(a => a.Url == Insertuser.Url).Count();
                if (countUrl > 1)
                {
                    Insertuser.Url += "-" + Insertuser.Id;
                    _unitOfWork.Save();
                }
                var userData = Insertuser.Avatar + "|" + Insertuser.Id + "|" + Insertuser.Email + "|" + Insertuser.FullName + "|" + Insertuser.Url + "|" + Insertuser.AvatarSocial;
                var ticket = new FormsAuthenticationTicket(2, Insertuser.Email.ToLower(), DateTime.Now, DateTime.Now.AddDays(30), true,
                    userData, FormsAuthentication.FormsCookiePath);
                var encTicket = FormsAuthentication.Encrypt(ticket);
                Response.Cookies.Add(new HttpCookie(".ASPXAUTHMEMBER", encTicket));
                return RedirectToAction("Index", "Home");
            }
            else
            {
                var userData = user.Avatar + "|" + user.Id + "|" + user.Email + "|" + user.FullName + "|" + user.Url + "|" + user.AvatarSocial;
                var ticket = new FormsAuthenticationTicket(2, user.Email.ToLower(), DateTime.Now, DateTime.Now.AddDays(30), true,
                    userData, FormsAuthentication.FormsCookiePath);
                var encTicket = FormsAuthentication.Encrypt(ticket);
                Response.Cookies.Add(new HttpCookie(".ASPXAUTHMEMBER", encTicket));
                if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                {
                    return Redirect(returnUrl);
                }
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
            string picture = jsonObject["picture"] ?.ToString();
            string email = jsonObject["email"]?.ToString();
            var count = _unitOfWork.UserRepository.GetQuery(a => a.Email == email).Count();
            if (count >= 1)
            {
                var user = _unitOfWork.UserRepository.GetQuery(a => a.Email == email).FirstOrDefault();
                var userData = user.Avatar + "|" + user.Id + "|" + user.Email + "|" + user.FullName + "|" + user.Url + "|" + user.AvatarSocial;
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
                        Avatar = null,
                        AvatarSocial = picture,
                        Cover = null,
                        GoogleId = id,
                        Email = email,
                        FullName = jsonObject["name"]?.ToString(),
                        TypeRegister = TypeRegister.Google,
                        Url = HtmlHelpers.ConvertToUnSign(null, jsonObject["name"]?.ToString()),
                    };
                    _unitOfWork.UserRepository.Insert(Insertuser);
                    _unitOfWork.Save();
                    var countUrl = _unitOfWork.UserRepository.GetQuery(a => a.Url == Insertuser.Url).Count();
                    if (countUrl > 1)
                    {
                        Insertuser.Url += "-" + Insertuser.Id;
                        _unitOfWork.Save();
                    }
                    var userData = Insertuser.Avatar + "|" + Insertuser.Id + "|" + Insertuser.Email + "|" + Insertuser.FullName + "|" + Insertuser.Url + "|" + Insertuser.AvatarSocial;
                    var ticket = new FormsAuthenticationTicket(2, Insertuser.Email.ToLower(), DateTime.Now, DateTime.Now.AddDays(30), true,
                        userData, FormsAuthentication.FormsCookiePath);
                    var encTicket = FormsAuthentication.Encrypt(ticket);
                    Response.Cookies.Add(new HttpCookie(".ASPXAUTHMEMBER", encTicket));
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    var userData = user.Avatar + "|" + user.Id + "|" + user.Email + "|" + user.FullName + "|" + user.Url + "|" + user.AvatarSocial;
                    var ticket = new FormsAuthenticationTicket(2, user.Email.ToLower(), DateTime.Now, DateTime.Now.AddDays(30), true,
                        userData, FormsAuthentication.FormsCookiePath);
                    var encTicket = FormsAuthentication.Encrypt(ticket);
                    Response.Cookies.Add(new HttpCookie(".ASPXAUTHMEMBER", encTicket));
                    return RedirectToAction("Index", "Home");
                }
            }
        }
        [OverrideActionFilters]
        public async Task<ActionResult> CallBackLinked(string code = "AQWYllV6BcuSfHDRtpXCd0yY-YlAj_W3iIEbKVXvgDyhf_D2cLq-HjDp92McPFw_pktGvDNG0FVdXD6oDL9N4g4M8Y05-dtK2FNGobhO-lnop-uE0L9Azm3Nx4irqeAbVqhNQJdgRzFBS1HJlBPVXmPgIu32oQq1Joi_IpVvqbjbMccThKf4MDBS3AuJ14A2dCUBWpFNdDv8r8ZDaKFezc1gk0ZxyVxPG6WynYFwlo31aceQAXgvBan4f9cX4xER02iiJT1m-CQNszUFEwIHWdtqnjCPi90wSSoDIuzkjnhQ8XYOUSCo-bwv2QTYNg02Lb5AursP5cm3coZuPbYn20rR8Q3ORA")
        {
            using (var client = new HttpClient())
            {
                var tokenEndpoint = "https://www.linkedin.com/oauth/v2/accessToken";
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "authorization_code"),
                    new KeyValuePair<string, string>("code", code),
                    new KeyValuePair<string, string>("redirect_uri", RedirectUri),
                    new KeyValuePair<string, string>("client_id", LinkedInClientId),
                    new KeyValuePair<string, string>("client_secret", LinkedInClientSecret)
                });

                var response = await client.PostAsync(tokenEndpoint, content);
                var tokenResponseContent = await response.Content.ReadAsStringAsync();
                var tokenResponse = JObject.Parse(tokenResponseContent);

                var accessToken = tokenResponse["access_token"].ToString();

                var profileEndpoint = "https://api.linkedin.com/v2/userinfo";
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
                var profileResponse = await client.GetAsync(profileEndpoint);
                var profileDataContent = await profileResponse.Content.ReadAsStringAsync();
                var profileData = JObject.Parse(profileDataContent);

                var email = profileData["emailAddress"].ToString();
                var firstName = profileData["firstName"].ToString();
                var lastName = profileData["lastName"].ToString();
                var id = profileData["id"].ToString();
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
                        Avatar = null,
                        Cover = null,
                        TypeRegister = TypeRegister.Website,
                        Url = HtmlHelpers.ConvertToUnSign(null, model.FullName),
                    };
                    _unitOfWork.UserRepository.Insert(user);
                    _unitOfWork.Save();
                    var count = _unitOfWork.UserRepository.GetQuery(a => a.Url == user.Url).Count();
                    if (count > 1)
                    {
                        user.Url += "-" + user.Id;
                        _unitOfWork.Save();
                    }
                    var userData = user.Avatar + "|" + user.Id + "|" + user.Email + "|" + user.FullName + "|" + user.Url;
                    var ticket = new FormsAuthenticationTicket(2, user.Email.ToLower(), DateTime.Now, DateTime.Now.AddDays(30), true,
                        userData, FormsAuthentication.FormsCookiePath);
                    var encTicket = FormsAuthentication.Encrypt(ticket);
                    Response.Cookies.Add(new HttpCookie(".ASPXAUTHMEMBER", encTicket));
                    return RedirectToAction("Index", "Home", new { result = "success" });
                }
            }
            return View(model);
        }

        [OverrideActionFilters, Route("quen-mat-khau")]
        public ActionResult ForgotPassword(string result = "")
        {
            ViewBag.Result = result;
            return View();
        }
        [OverrideActionFilters, HttpPost, Route("quen-mat-khau")]
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
        [OverrideActionFilters, Route("dat-lai-mat-khau")]
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
        public ActionResult SetNewPasswordUrl(SetNewPasswordViewModel model)
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

        //[Route("doi-mat-khau")]
        //public ActionResult ChangePassword(int result = 0)
        //{
        //    ViewBag.Result = result;
        //    var model = new UserPasswordViewModel
        //    {
        //        User = _unitOfWork.UserRepository.GetById(User.Id),
        //    };
        //    return View(model);
        //}

        //[Route("doi-mat-khau"), HttpPost, ValidateAntiForgeryToken]
        //public ActionResult ChangePassword(UserPasswordViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        if (User == null || !HtmlHelpers.VerifyHash(model.Password, "SHA256", User.Password))
        //        {
        //            ModelState.AddModelError("", @"Tên đăng nhập hoặc mật khẩu không chính xác.");
        //            return View(model);
        //        }
        //        User.Password = HtmlHelpers.ComputeHash(model.Password, "SHA256", null);
        //        _unitOfWork.Save();
        //        return RedirectToAction("ChangePassword", new { result = 1 });
        //    }
        //    return View(model);
        //}

        public ActionResult InfoUser(int id)
        {
            var user = _unitOfWork.UserRepository.GetById(id);
            if (user == null)
            {
                return View();
            }
            var model = new ChangeInfoUserViewModel
            {
                FullName = user.FullName,
                Classtify = user.Classtify,
                Gender = user.Gender,
                Address = user.Address,
                Email = user.Email,
                Phone = user.Phone,
                Url = user.Url,
                Description = user.Description,
                Year = Convert.ToInt32(user.DateOfBirth?.ToString("yyyy")),
                Month = Convert.ToInt32(user.DateOfBirth?.ToString("MM")),
                Date = Convert.ToInt32(user.DateOfBirth?.ToString("dd")),
            };
            return View(model);
        }

        [HttpPost]
        public bool UpdateCv(ChangeInfoUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return false;
            }

            //if (model.Educations != null && model.Educations.Any())
            //{
            //    foreach (var item in model.Educations)
            //    {
            //        if (item != null && !string.IsNullOrWhiteSpace(item.StartDate) && !string.IsNullOrWhiteSpace(item.EndDate) && !string.IsNullOrWhiteSpace(item.Majors) && !string.IsNullOrWhiteSpace(item.School) && !string.IsNullOrWhiteSpace(item.Description))
            //        {
            //            var myEdu = _unitOfWork.EducationRepository.GetById(item.Id);
            //            if (myEdu == null)
            //            {
            //                var education = new Education
            //                {
            //                    StartDate = item.StartDate,
            //                    EndDate = item.EndDate,
            //                    School = item.School,
            //                    Description = item.Description,
            //                    UserId = User.Id,
            //                    Majors = item.Majors,
            //                };
            //                _unitOfWork.EducationRepository.Insert(education);
            //            }
            //            else
            //            {
            //                myEdu.StartDate = item.StartDate;
            //                myEdu.EndDate = item.EndDate;
            //                myEdu.School = item.School;
            //                myEdu.Description = item.Description;
            //                myEdu.UserId = User.Id;
            //                myEdu.Majors = item.Majors;
            //            }
            //        }

            //    }
            //}

            //foreach (var item in model.Experiences)
            //{
            //    if (item != null && !string.IsNullOrWhiteSpace(item.StartDate) && !string.IsNullOrWhiteSpace(item.EndDate) && !string.IsNullOrWhiteSpace(item.Position) && !string.IsNullOrWhiteSpace(item.Company) && !string.IsNullOrWhiteSpace(item.Description))
            //    {
            //        var myExp = _unitOfWork.ExperienceRepository.GetById(item.Id);
            //        if (myExp == null)
            //        {
            //            var experience = new Experiences
            //            {
            //                StartDate = item.StartDate,
            //                EndDate = item.EndDate,
            //                Position = item.Position,
            //                Company = item.Company,
            //                Description = item.Description,
            //                UserId = User.Id
            //            };
            //            _unitOfWork.ExperienceRepository.Insert(experience);
            //        }
            //        else
            //        {
            //            myExp.StartDate = item.StartDate;
            //            myExp.EndDate = item.EndDate;
            //            myExp.Position = item.Position;
            //            myExp.Company = item.Company;
            //            myExp.Description = item.Description;
            //        }
            //    }
            //}

            User.FullName = model.FullName;
            User.Classtify = model.Classtify;
            User.Email = model.Email;
            User.Address = model.Address;
            User.Phone = model.Phone;
            User.Description = model.Description;
            User.Gender = model.Gender;
            User.Url = model.Url;
            if (model.Date > 0 && model.Month > 0 && model.Year > 0)
            {
                User.DateOfBirth = new DateTime(model.Year, model.Month, model.Date);
            }
            _unitOfWork.Save();
            return true;
        }

        [HttpPost]
        public JsonResult CheckUrl(string url)
        {
            var user = _unitOfWork.UserRepository.GetQuery(a => a.Url == url).Count();
            if (user > 1)
            {
                return Json(false);
            }
            return Json(true);
        }
        [HttpPost]
        public JsonResult ChangImage(FormCollection fc)
        {
            if (fc != null)
            {
                var type = Convert.ToInt32(fc["type"]);
                var file = Request.Files["image"];
                if (file != null && file.ContentLength > 0)
                {
                    if (!HtmlHelpers.CheckFileExt(file.FileName, "jpg|jpeg|png|gif"))
                    {
                        return Json(new { success = false, message = "Chỉ chấp nhận định dạng jpg, png, gif, jpeg" });
                    }
                    else
                    {
                        if (file.ContentLength > 4000 * 1024)
                        {
                            return Json(new { success = false, message = "Dung lượng lớn hơn 4MB. Hãy thử lại" });
                        }
                        else
                        {
                            var imgFileName = HtmlHelpers.ConvertToUnSign(null, Path.GetFileNameWithoutExtension(file.FileName)) + "-" + DateTime.Now.Millisecond + Path.GetExtension(file.FileName);
                            var imgPath = "/images/user/" + DateTime.Now.ToString("yyyy/MM/dd");
                            HtmlHelpers.CreateFolder(Server.MapPath(imgPath));
                            if (type == 1)
                            {
                                User.Cover = DateTime.Now.ToString("yyyy/MM/dd") + "/" + imgFileName;
                            }
                            else
                            {
                                User.Avatar = DateTime.Now.ToString("yyyy/MM/dd") + "/" + imgFileName;
                            }
                            var newImage = Image.FromStream(file.InputStream);
                            var fixSizeImage = HtmlHelpers.FixedSize(newImage, 1200, 600, false);
                            HtmlHelpers.SaveJpeg(Server.MapPath(Path.Combine(imgPath, imgFileName)), fixSizeImage, 90);
                            _unitOfWork.Save();
                            var coverUrl = Url.Content(Path.Combine(imgPath, imgFileName));
                            return Json(new { success = true, message = "Hình ảnh đã được tải lên thành công.", coverUrl = coverUrl });
                        }
                    }
                }

            }
            return Json(new { success = false, message = "Đã xảy ra lỗi khi xử lý hình ảnh" });
        }
        [HttpPost]
        public JsonResult RemoveInfo(int id, int type)
        {
            try
            {
                switch (type)
                {
                    case 1:
                        var edu = _unitOfWork.EducationRepository.GetById(id);
                        if (edu == null)
                        {
                            return Json(new { success = false, message = "Không tìm thấy thông tin giáo dục" });
                        }
                        _unitOfWork.EducationRepository.Delete(edu);
                        break;
                    case 2:
                        var exp = _unitOfWork.ExperienceRepository.GetById(id);
                        if (exp == null)
                        {
                            return Json(new { success = false, message = "Không tìm thấy thông tin kinh nghiệm" });
                        }
                        _unitOfWork.ExperienceRepository.Delete(exp);
                        break;
                    default:
                        return Json(new { success = false, message = "Loại không hợp lệ" });
                }

                _unitOfWork.Save();

                return Json(new { success = true, message = "Xóa thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Đã xảy ra lỗi khi xử lý" });
            }
        }
        [Route("danh-sach-cong-ty-theo-doi")]
        public ActionResult ListFollow(int? page, string name)
        {
            var pageNumber = page ?? 1;
            var company = _unitOfWork.FollowRepository.GetQuery(a => a.UserId == User.Id, o => o.OrderBy(a => a.Id));
            var model = new UserCompanyViewModel
            {
                Follows = company.ToPagedList(pageNumber, 12),
            };
            return View(model);
        }
        [Route("danh-sach-cong-viec-da-luu")]
        public ActionResult ListLike(int? page)
        {
            var pageNumber = page ?? 1;
            var job = _unitOfWork.LikeRepository.GetQuery(a => a.UserID == User.Id && a.JobId != null, o => o.OrderBy(a => a.Id));
            var model = new UserLikeViewModel
            {
                Likes = job.ToPagedList(pageNumber, 12),
                User = _unitOfWork.UserRepository.GetById(User.Id),
            };
            return View(model);
        }
        [Route("lich-su-ung-tuyen/json")]
        public JsonResult ListApplyJson()
        {
            var jobs = _unitOfWork.ApplyJobRepository.GetQuery(a => a.UserId == User.Id)
        .Select(a => new
        {
            a.JobPost.Company.Name,
            a.Body,
            a.Url,
            a.Status,
            a.TypeApply,
            a.CreateDate
        })
        .ToList();
            return Json(jobs, JsonRequestBehavior.AllowGet);
        }

        [Route("lich-su-ung-tuyen")]
        public ActionResult ListApply()
        {
            return View(User);
        }
        public PartialViewResult GetApply(int? page, int? status, int? id , int? type)
        {
            var pageNumber = page ?? 1;
            var job = _unitOfWork.ApplyJobRepository.GetQuery(a => a.UserId == User.Id, o => o.OrderByDescending(a => a.CreateDate));
            switch (status)
            {
                case 0:
                    job = job.Where(a => a.Status == ApplyJobStatus.Waiting);
                    break;
                case 1:
                    job = job.Where(a => a.Status == ApplyJobStatus.View);
                    break;
                case 2:
                    job = job.Where(a => a.Status == ApplyJobStatus.Active);
                    break;
                case 3:
                    job = job.Where(a => a.Status == ApplyJobStatus.NoActive);
                    break;
            }
            switch (type)
            {
                case 1:
                     job = job.Where(a => a.JobPostId != null);
                    break;
                    case 2:
                    job = job.Where(a => a.StudyAbroadId != null);
                    break;
                default:
                    job = job.Where(a => a.JobPostId != null);
                    break;
            }
            var noti = _unitOfWork.UserLogRepository.GetById(id);
            if (noti != null)
            {
                noti.Status = true;
                _unitOfWork.Save();
            }
            var model = new UserApplyViewModel
            {
                ApplyJobs = job.ToPagedList(pageNumber, 8),
                User = User,
                Status = status
            };
            return PartialView(model);
        }
        public PartialViewResult Notify()
        {
            var log = _unitOfWork.UserLogRepository.GetQuery(a => a.UserId == User.Id, o => o.OrderByDescending(a => a.CreateDate));
            return PartialView(log);
        }

        #region Edu
        public PartialViewResult Education()
        {
            return PartialView();
        }
        [HttpPost]
        public JsonResult Education(InsertEduViewModel model)
        {
            if (ModelState.IsValid)
            {

                model.Education.StartDate = model.StarMonth.ToString()+ "/" + model.StarYear.ToString();
                model.Education.EndDate = model.EndMonth.ToString () + "/" + model.EndYear.ToString();
                if (model.Education.Active || model.EndMonth == null || model.EndYear == null)
                {
                    model.Education.EndDate = "Hiện tại";
                    model.Education.Active = true;
                }
                model.Education.UserId = User.Id;
                _unitOfWork.EducationRepository.Insert(model.Education);
                _unitOfWork.Save();
                return Json(new { success = true, message = "Thêm thành công" });

            };
            return Json(new { success = false, message = "Quá trình thực hiện không thành công" });
        }

        public PartialViewResult EditEducation(int id)
        {
            var edu = _unitOfWork.EducationRepository.GetById(id);
            if(edu == null)
            {
                return PartialView();
            }
            var model = new InsertEduViewModel
            {
                Education = edu,
            };
            return PartialView(model);
        }
        [HttpPost]
        public JsonResult EditEducation(InsertEduViewModel model)
        {
            var edu = _unitOfWork.EducationRepository.GetById(model.Education.Id);
            if(edu == null)
            {
                return Json(new { success = false, message = "Quá trình thực hiện không thành công" });
            }
            if (ModelState.IsValid)
            {
                edu.Majors = model.Education.Majors;
                edu.School = model.Education.School;
                edu.Description = model.Education.Description;
                edu.Active = model.Education.Active;
                edu.StartDate = model.StarMonth.ToString() + "/" + model.StarYear.ToString();
                edu.EndDate = model.EndMonth.ToString() + "/" + model.EndYear.ToString();
                if (edu.Active || model.EndMonth == null || model.EndYear == null)
                {
                    edu.EndDate = "Hiện tại";
                    edu.Active = true;
                }
                _unitOfWork.Save();
                return Json(new { success = true, message = "Cập nhật thành công" });
            }
            return Json(new { success = false, message = "Quá trình thực hiện không thành công" });
        }
        [HttpPost]
        public JsonResult RemoveEdu(int id)
        {
            var edu = _unitOfWork.EducationRepository.GetById(id);
            if (edu == null)
            {
                return Json(new { success = false, message = "Quá trình thực hiện không thành công" });
            }
            _unitOfWork.EducationRepository.Delete(edu);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Xóa thành công" });

        }
        #endregion

        #region Experience
        public PartialViewResult Experience()
        {
            return PartialView();   
        }
        [HttpPost]
        public JsonResult Experience(InsertExperienceViewModel model, FormCollection fc)
        {
            if (ModelState.IsValid)
            {
                model.Experiences.Image = fc["Pictures"];
                model.Experiences.StartDate = model.StarMonth.ToString() + "/" + model.StarYear.ToString();
                model.Experiences.EndDate = model.EndMonth.ToString() + "/" + model.EndYear.ToString();
                if (model.Experiences.Active || model.EndMonth == null || model.EndYear == null)
                {
                    model.Experiences.EndDate = "Hiện tại";
                    model.Experiences.Active = true;
                }
                model.Experiences.UserId = User.Id;
                _unitOfWork.ExperienceRepository.Insert(model.Experiences);
                _unitOfWork.Save();
                return Json(new { success = true, message = "Thêm thành công" });
            }
            return Json(new { success = false, message = "Quá trình thực hiện không thành công" });
        }

        public PartialViewResult UpdateExperience(int id)
        {
            var exp = _unitOfWork.ExperienceRepository.GetById(id);
            if(exp == null)
            {
                return PartialView();
            }
            var model = new InsertExperienceViewModel
            {
                Experiences = exp
            };
            return PartialView(model);
        }
        [HttpPost]
        public JsonResult UpdateExperience(InsertExperienceViewModel model , FormCollection fc)
        {
            var exp = _unitOfWork.ExperienceRepository.GetById(model.Experiences.Id);
            if (exp == null)
            {
                return Json(new { success = false, message = "Quá trình thực hiện không thành công" });
            }
            if (ModelState.IsValid)
            {
                exp.Company = model.Experiences.Company;
                exp.Position = model.Experiences.Position;
                exp.Active = model.Experiences.Active;
                exp.StartDate = model.StarMonth.ToString() + "/" + model.StarYear.ToString();
                exp.EndDate = model.EndMonth.ToString() + "/" + model.EndYear.ToString();
                if (exp.Active || model.EndMonth == null || model.EndYear == null)
                {
                    exp.EndDate = "Hiện tại";
                    exp.Active = true;
                }
                exp.Description = model.Experiences.Description;
                exp.Image = fc["Pictures"] == "" ? null : fc["Pictures"];
                _unitOfWork.Save();
                return Json(new { success = true, message = "Thêm thành công" });
            }
           return Json(new { success = false, message = "Quá trình thực hiện không thành công" });
        }
        [HttpPost]
        public JsonResult RemoveExperience(int id)
        {
            var exp = _unitOfWork.ExperienceRepository.GetById(id);
            if (exp == null)
            {
                return Json(new { success = false, message = "Quá trình thực hiện không thành công" });
            }
            _unitOfWork.ExperienceRepository.Delete(exp);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Xóa thành công" });

        }
        #endregion

        #region Skill
        public PartialViewResult Skill ()
        {
            return PartialView();
        }

        [HttpPost]
        public JsonResult Skill(UserSkill model , FormCollection fc)
        {
            if (ModelState.IsValid)
            {
                model.Star = Convert.ToInt32(fc["rating"]);
                model.UserId = User.Id;
                _unitOfWork.UserSkillRepository.Insert(model);
                _unitOfWork.Save();
                return Json(new { success = true, message = "Thêm thành công" });
            }
            return Json(new { success = false, message = "Quá trình thực hiện không thành công" });
        }
        public ActionResult UpdateSkill(int id)
        {
            var skill = _unitOfWork.UserSkillRepository.GetById(id);
            if(skill == null)
            {
                return View();
            }
            return View(skill);
        }

        [HttpPost]
        public JsonResult UpdateSkill(UserSkill model, FormCollection fc)
        {
            var skill = _unitOfWork.UserSkillRepository.GetById(model.Id);
            if (skill == null)
            {
                return Json(new { success = false, message = "Quá trình thực hiện không thành công" });
            }
            if (ModelState.IsValid)
            {
                skill.Name = model.Name;
                skill.Description = model.Description;
                skill.Star = Convert.ToInt32(fc["rating"]);
                _unitOfWork.Save();
                return Json(new { success = true, message = "Cập nhật thành công" });
            };
            return Json(new { success = false, message = "Quá trình thực hiện không thành công" });
        }

        [HttpPost]
        public JsonResult RemoveSkill(int id)
        {
            var skill = _unitOfWork.UserSkillRepository.GetById(id);
            if (skill == null)
            {
                return Json(new { success = false, message = "Quá trình thực hiện không thành công" });
            }
            _unitOfWork.UserSkillRepository.Delete(skill);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Xóa thành công" });

        }
        #endregion

        #region Certificate
        public PartialViewResult Certificate()
        {
            return PartialView();
        }

        [HttpPost]
        public JsonResult Certificate(InsertCertificateViewModel model, FormCollection fc)
        {
            if (ModelState.IsValid)
            {
                model.Certificate.Image = fc["Pictures"];
                model.Certificate.StartDate = model.StarMonth.ToString() + "/" + model.StarYear.ToString();
                model.Certificate.EndDate = model.EndMonth.ToString() + "/" + model.EndYear.ToString();
                if (model.Certificate.Active || model.EndMonth == null || model.EndYear == null)
                {
                    model.Certificate.EndDate = "Không thời hạn";
                    model.Certificate.Active = true;
                }
                model.Certificate.UserId = User.Id;
                _unitOfWork.CertificateRepository.Insert(model.Certificate);
                _unitOfWork.Save();
                return Json(new { success = true, message = "Thêm thành công" });
            }
            return Json(new { success = false, message = "Quá trình thực hiện không thành công" });
        }
        public PartialViewResult EditCertificate(int id)
        {
            var cer = _unitOfWork.CertificateRepository.GetById(id);
            if(cer == null)
            {
                return PartialView();
            }
            var model = new InsertCertificateViewModel
            {
                Certificate = cer
            };
            return PartialView(model);
        }
        [HttpPost]
        public JsonResult EditCertificate(InsertCertificateViewModel model, FormCollection fc)
        {
            var cer = _unitOfWork.CertificateRepository.GetById(model.Certificate.Id);
            if (cer == null)
            {
                return Json(new { success = false, message = "Quá trình thực hiện không thành công" });
            }
            cer.Name = model.Certificate.Name;
            cer.Description = model.Certificate.Description;
            cer.Active = model.Certificate.Active;
            cer.StartDate = model.StarMonth.ToString() + "/" + model.StarYear.ToString();
            cer.EndDate = model.EndMonth.ToString() + "/" + model.EndYear.ToString();
            if (cer.Active || model.EndMonth == null || model.EndYear == null)
            {
                cer.EndDate = "Không thời hạn";
                cer.Active = true;
            }
            cer.Description = model.Certificate.Description;
            cer.Image = fc["Pictures"] == "" ? null : fc["Pictures"];
            _unitOfWork.Save();
            return Json(new { success = true, message = "Thêm thành công" });
        }

        [HttpPost]
        public JsonResult RemoveCertificate(int id)
        {
            var certificate = _unitOfWork.CertificateRepository.GetById(id);
            if (certificate == null)
            {
                return Json(new { success = false, message = "Quá trình thực hiện không thành công" });
            }
            _unitOfWork.CertificateRepository.Delete(certificate);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Xóa thành công" });

        }

        #endregion

        #region Project
        public PartialViewResult Project()
        {
            return PartialView();
        }
        [HttpPost]
        public JsonResult Project(InsertProjectViewModel model, FormCollection fc)
        {
            if (ModelState.IsValid)
            {
                model.Project.Image = fc["Pictures"];
                model.Project.StartDate = model.StarMonth.ToString() + "/" + model.StarYear.ToString();
                model.Project.EndDate = model.EndMonth.ToString() + "/" + model.EndYear.ToString();
                model.Project.UserId = User.Id;
                _unitOfWork.ProjectRepository.Insert(model.Project);
                _unitOfWork.Save();
                return Json(new { success = true, message = "Thêm thành công" });
            }
            return Json(new { success = false, message = "Quá trình thực hiện không thành công" });
        }

        public PartialViewResult EditProject(int id)
        {
            var project = _unitOfWork.ProjectRepository.GetById(id);
            if (project == null)
            {
                return PartialView();
            }
            var model = new InsertProjectViewModel
            {
                Project = project,
            };
            return PartialView(model);
        }
        [HttpPost]
        public JsonResult EditProject(InsertProjectViewModel model, FormCollection fc)
        {
            var project = _unitOfWork.ProjectRepository.GetById(model.Project.Id);
            if (project == null)
            {
                return Json(new { success = false, message = "Quá trình thực hiện không thành công" });
            }
            project.StartDate = model.StarMonth.ToString() + "/" + model.StarYear.ToString();
            project.EndDate = model.EndMonth.ToString() + "/" + model.EndYear.ToString();
            project.Name = model.Project.Name;
            project.Customer = model.Project.Customer;
            project.Postion = model.Project.Postion;
            project.Member = model.Project.Member;
            project.Tech = model.Project.Tech;
            project.Description = model.Project.Description;
            project.Image = fc["Pictures"] == "" ? null : fc["Pictures"];
            _unitOfWork.Save();
            return Json(new { success = true, message = "Thêm thành công" });
        }

        [HttpPost]
        public JsonResult RemoveProject(int id)
        {
            var project = _unitOfWork.ProjectRepository.GetById(id);
            if (project == null)
            {
                return Json(new { success = false, message = "Quá trình thực hiện không thành công" });
            }
            _unitOfWork.ProjectRepository.Delete(project);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Xóa thành công" });

        }
        #endregion

        #region Activity 
        public PartialViewResult Activity()
        {
            return PartialView();
        }
        [HttpPost]
        public JsonResult Activity(InsertActivityViewModel model , FormCollection fc)
        {
            if (ModelState.IsValid)
            {
                model.Activity.Image = fc["Pictures"];
                model.Activity.StartDate = model.StarMonth.ToString() + "/" + model.StarYear.ToString();
                model.Activity.EndDate = model.EndMonth.ToString() + "/" + model.EndYear.ToString();
                if (model.Activity.Active || model.EndMonth == null || model.EndYear == null)
                {
                    model.Activity.EndDate = "Hiện tại";
                    model.Activity.Active = true;
                }
                model.Activity.UserId = User.Id;
                _unitOfWork.ActivityRepository.Insert(model.Activity);
                _unitOfWork.Save();
                return Json(new { success = true, message = "Thêm thành công" });
            }
            return Json(new { success = false, message = "Quá trình thực hiện không thành công" });
        }

        public PartialViewResult UpdateActivity(int id)
        {
            var activity = _unitOfWork.ActivityRepository.GetById(id);
            if(activity == null)
            {
                return PartialView();
            }
            var model = new InsertActivityViewModel
            {
                Activity = activity
            };
            return PartialView(model);
        }

        [HttpPost]
        public JsonResult UpdateActivity(InsertActivityViewModel model, FormCollection fc)
        {
            var activity = _unitOfWork.ActivityRepository.GetById(model.Activity.Id);
            if (activity == null)
            {
                return Json(new { success = false, message = "Quá trình thực hiện không thành công" });
            }
            if (ModelState.IsValid)
            {
                activity.Image = fc["Pictures"] == "" ? null : fc["Pictures"];
                activity.StartDate = model.StarMonth.ToString() + "/" + model.StarYear.ToString();
                activity.EndDate = model.EndMonth.ToString() + "/" + model.EndYear.ToString();
                activity.Active = model.Activity.Active;
                if (activity.Active || model.EndMonth == null || model.EndYear == null)
                {
                    activity.EndDate = "Hiện tại";
                    activity.Active = true;

                }
                activity.Name = model.Activity.Name;
                activity.Position = model.Activity.Position;
                activity.Description = model.Activity.Description; 
                _unitOfWork.Save();
                return Json(new { success = true, message = "Thêm thành công" });
            }
            return Json(new { success = false, message = "Quá trình thực hiện không thành công" });
        }

        [HttpPost]
        public JsonResult RemoveActivity(int id)
        {
            var ac = _unitOfWork.ActivityRepository.GetById(id);
            if (ac == null)
            {
                return Json(new { success = false, message = "Quá trình thực hiện không thành công" });
            }
            _unitOfWork.ActivityRepository.Delete(ac);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Xóa thành công" });

        }
        #endregion

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
        [OverrideActionFilters]
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