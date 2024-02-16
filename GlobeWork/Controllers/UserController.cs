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
using System.Collections.Generic;
using Microsoft.Ajax.Utilities;
using System.IO;
using System.Drawing;
using PagedList;
namespace GlobeWork.Controllers
{
    [MemberFilter, RoutePrefix("user")]
    public class UserController : Controller
    {
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
            ViewBag.Linkedin = LinkedinLogin.LinkedinPackage.RedirectToLinkedinLoginUrl("7896gkl097iy60", "https://localhost:44327/User/CallBackLinked");
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
                var userData = user.Avatar + "|" + user.Id + "|" + user.Email + "|" + user.FullName + "|" + user.Url;
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
            if (user == null)
            {
                var Insertuser = new User
                {
                    Username = "",
                    Avatar = avatar,
                    Cover = null,
                    FaceBookId = id,
                    Email = email,
                    FullName = name,
                    Url = HtmlHelpers.ConvertToUnSign(null, name),
                    TypeRegister = TypeRegister.Facebook,
                };
                _unitOfWork.UserRepository.Insert(Insertuser);
                _unitOfWork.Save();
                var countUrl = _unitOfWork.UserRepository.GetQuery(a => a.Url == user.Url).Count();
                if (countUrl > 1)
                {
                    Insertuser.Url += "-" + Insertuser.Id;
                    _unitOfWork.Save();
                }
                var userData = Insertuser.Username + "|" + Insertuser.Avatar + "|" + Insertuser.Id + "|" + Insertuser.Email + "|" + Insertuser.FullName + "|" + Insertuser.Url;
                var ticket = new FormsAuthenticationTicket(2, Insertuser.Email.ToLower(), DateTime.Now, DateTime.Now.AddDays(30), true,
                    userData, FormsAuthentication.FormsCookiePath);
                var encTicket = FormsAuthentication.Encrypt(ticket);
                Response.Cookies.Add(new HttpCookie(".ASPXAUTHMEMBER", encTicket));
                return RedirectToAction("Index", "Home");
            }
            else
            {
                var userData = user.Username + "|" + user.Avatar + "|" + user.Id + "|" + user.Email + "|" + user.FullName + "|" + user.Url;
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
            string email = jsonObject["email"]?.ToString();
            var count = _unitOfWork.UserRepository.GetQuery(a => a.Email == email).Count();
            if (count >= 1)
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
                        Avatar = null,
                        Cover = null,
                        GoogleId = id,
                        Email = email,
                        FullName = jsonObject["name"]?.ToString(),
                        TypeRegister = TypeRegister.Google,
                        Url = HtmlHelpers.ConvertToUnSign(null, jsonObject["name"]?.ToString()),
                    };
                    _unitOfWork.UserRepository.Insert(Insertuser);
                    _unitOfWork.Save();
                    var countUrl = _unitOfWork.UserRepository.GetQuery(a => a.Url == user.Url).Count();
                    if (countUrl > 1)
                    {
                        Insertuser.Url += "-" + Insertuser.Id;
                        _unitOfWork.Save();
                    }
                    var userData = Insertuser.Avatar + "|" + Insertuser.Id + "|" + Insertuser.Email + "|" + Insertuser.FullName + "|" + Insertuser.Url;
                    var ticket = new FormsAuthenticationTicket(2, Insertuser.Email.ToLower(), DateTime.Now, DateTime.Now.AddDays(30), true,
                        userData, FormsAuthentication.FormsCookiePath);
                    var encTicket = FormsAuthentication.Encrypt(ticket);
                    Response.Cookies.Add(new HttpCookie(".ASPXAUTHMEMBER", encTicket));
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    var userData = user.Avatar + "|" + user.Id + "|" + user.Email + "|" + user.FullName + "|" + user.Url;
                    var ticket = new FormsAuthenticationTicket(2, user.Email.ToLower(), DateTime.Now, DateTime.Now.AddDays(30), true,
                        userData, FormsAuthentication.FormsCookiePath);
                    var encTicket = FormsAuthentication.Encrypt(ticket);
                    Response.Cookies.Add(new HttpCookie(".ASPXAUTHMEMBER", encTicket));
                    return RedirectToAction("Index", "Home");
                }
            }
        }
        [OverrideActionFilters]
        public ActionResult CallBackLinked(string code, string state)
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
                        Avatar = null,
                        Cover = null,
                        TypeRegister = TypeRegister.Website,
                        Url = HtmlHelpers.ConvertToUnSign(null, model.FullName),
                    };
                    _unitOfWork.UserRepository.Insert(user);
                    _unitOfWork.Save();
                    var count = _unitOfWork.UserRepository.GetQuery(a => a.Url == user.Url).Count();
                    if(count > 1)
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
        

        public PartialViewResult InfoUser(int id)
        {
            var user = _unitOfWork.UserRepository.GetById(id);
            if (user == null)
            {
                return PartialView();
            }
            var edu = _unitOfWork.EducationRepository.GetQuery(a => a.UserId == user.Id, o => o.OrderBy(a => a.Id));
            var exp = _unitOfWork.ExperienceRepository.GetQuery(a => a.UserId == user.Id, o => o.OrderBy(a => a.Id));
            var model = new ChangeInfoUserViewModel
            {
                ListEducations = edu,
                ListExperiences = exp,
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
            return PartialView(model);
        }
        [HttpPost]
        public bool UpdateCv(ChangeInfoUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return false;
            }

            if (model.Educations != null && model.Educations.Any())
            {
                foreach (var item in model.Educations)
                {
                    if (item != null && !string.IsNullOrWhiteSpace(item.StartDate) && !string.IsNullOrWhiteSpace(item.EndDate) && !string.IsNullOrWhiteSpace(item.Majors) && !string.IsNullOrWhiteSpace(item.School) && !string.IsNullOrWhiteSpace(item.Description))
                    {
                        var myEdu = _unitOfWork.EducationRepository.GetById(item.Id);
                        if (myEdu == null)
                        {
                            var education = new Education
                            {
                                StartDate = item.StartDate,
                                EndDate = item.EndDate,
                                School = item.School,
                                Description = item.Description,
                                UserId = User.Id,
                                Majors = item.Majors,
                            };
                            _unitOfWork.EducationRepository.Insert(education);
                        }
                        else
                        {
                            myEdu.StartDate = item.StartDate;
                            myEdu.EndDate = item.EndDate;
                            myEdu.School = item.School;
                            myEdu.Description = item.Description;
                            myEdu.UserId = User.Id;
                            myEdu.Majors = item.Majors;
                        }
                    }

                }
            }

            foreach (var item in model.Experiences)
            {
                if (item != null && !string.IsNullOrWhiteSpace(item.StartDate) && !string.IsNullOrWhiteSpace(item.EndDate) && !string.IsNullOrWhiteSpace(item.Position) && !string.IsNullOrWhiteSpace(item.Company) && !string.IsNullOrWhiteSpace(item.Description))
                {
                    var myExp = _unitOfWork.ExperienceRepository.GetById(item.Id);
                    if (myExp == null)
                    {
                        var experience = new Experiences
                        {
                            StartDate = item.StartDate,
                            EndDate = item.EndDate,
                            Position = item.Position,
                            Company = item.Company,
                            Description = item.Description,
                            UserId = User.Id
                        };
                        _unitOfWork.ExperienceRepository.Insert(experience);
                    }
                    else
                    {
                        myExp.StartDate = item.StartDate;
                        myExp.EndDate = item.EndDate;
                        myExp.Position = item.Position;
                        myExp.Company = item.Company;
                        myExp.Description = item.Description;
                    }
                }
            }

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
        public ActionResult ListFollow(int? page , string name)
        {
            var pageNumber = page ?? 1;
            var company = _unitOfWork.FollowRepository.GetQuery(a => a.UserId == User.Id, o => o.OrderBy(a => a.Id));
            var model = new UserCompanyViewModel
            {
                Follows = company.ToPagedList(pageNumber, 12)
            };
            return View(model);
        }
        [Route("danh-sach-cong-viec-da-luu")]
        public ActionResult ListLike(int? page)
        {
            var pageNumber = page ?? 1;
            var job = _unitOfWork.LikeRepository.GetQuery(a => a.UserID == User.Id , o => o.OrderBy(a => a.Id));
            var model = new UserLikeViewModel
            {
                Likes = job.ToPagedList(pageNumber, 12)
            };
            return View(model);
        }
        [Route("lich-su-ung-tuyen/json")]
        public JsonResult ListApplyJson()
        {
            var jobs = _unitOfWork.ApplyJobRepository.GetQuery(a => a.UserId == User.Id)
        .Select(a => new {
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
        public ActionResult ListApply(int? page , int? status )
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
            var model = new UserApplyViewModel
            {
                ApplyJobs = job.ToPagedList(pageNumber, 8),
                User = User,
                Status = status
            };
            
            return View(model);
        }
        public PartialViewResult Notify()
        {
            var log = _unitOfWork.UserLogRepository.GetQuery(a => a.UserId == User.Id, o => o.OrderByDescending(a => a.CreateDate));
            return PartialView(log);
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