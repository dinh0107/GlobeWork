using Helpers;
using GlobeWork.DAL;
using GlobeWork.Models;
using GlobeWork.ViewModel;
using PagedList;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Collections.Generic;
using System.Xml.Linq;
using Antlr.Runtime;

namespace GlobeWork.Controllers
{
    [Authorize]
    public class VcmsController : Controller
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();

        #region Login
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(AdminLoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var admin = _unitOfWork.AdminRepository.Get(a => a.Username == model.Username && a.Active).SingleOrDefault();

                if (admin != null && HtmlHelpers.VerifyHash(model.Password, "SHA256", admin.Password))
                {
                    var ticket = new FormsAuthenticationTicket(1, model.Username.ToLower(), DateTime.Now, DateTime.Now.AddDays(30), true,
                        admin.ToString(), FormsAuthentication.FormsCookiePath);

                    var encTicket = FormsAuthentication.Encrypt(ticket);
                    // Create the cookie.
                    Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));
                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Vcms");
                }
                ModelState.AddModelError("", @"Tên đăng nhập hoặc mật khẩu không chính xác.");
            }
            return View(model);
        }
        public RedirectToRouteResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Vcms");
        }
        #endregion

        #region Admin
        public ActionResult Index()
        {
            var model = new InfoAdminViewModel
            {
                Admins = _unitOfWork.AdminRepository.GetQuery().Count(),
                Job = _unitOfWork.JobPostRepository.GetQuery().Count(),
                Study = _unitOfWork.StudyAbroadRepository.GetQuery().Count(),
                ApplyJob = _unitOfWork.ApplyJobRepository.GetQuery(a => a.JobPostId != null).Count(),
                ApplyStudy = _unitOfWork.ApplyJobRepository.GetQuery(a => a.StudyAbroadId != null).Count(),
                Articles = _unitOfWork.ArticleRepository.GetQuery().Count(),
                Contacts = _unitOfWork.ContactRepository.GetQuery().Count(),
                Banners = _unitOfWork.BannerRepository.GetQuery().Count(),
                Employer = _unitOfWork.CompanyRepository.GetQuery().Count(),
                User = _unitOfWork.UserRepository.GetQuery().Count(),

                //Products = _unitOfWork.ProductRepository.GetQuery().Count(),
                //Orders = _unitOfWork.OrderRepository.GetQuery().Count(),
                //Showrooms = _unitOfWork.ShowRoomRepository.GetQuery().Count(),
            };
            return View(model);
        }
        [ChildActionOnly]
        public PartialViewResult ListAdmin()
        {
            var admins = _unitOfWork.AdminRepository.Get();
            return PartialView("ListAdmin", admins);
        }
        public ActionResult CreateAdmin(string result = "")
        {
            ViewBag.Result = result;
            return View();
        }
        [HttpPost]
        public ActionResult CreateAdmin(Admin model)
        {
            if (ModelState.IsValid)
            {
                var admin = _unitOfWork.AdminRepository.GetQuery(a => a.Username.Equals(model.Username)).SingleOrDefault();
                if (admin != null)
                {
                    ModelState.AddModelError("", @"Tên đăng nhập này có rồi");
                }
                else
                {
                    var hashPass = HtmlHelpers.ComputeHash(model.Password, "SHA256", null);
                    _unitOfWork.AdminRepository.Insert(new Admin { Username = model.Username, Password = hashPass, Active = model.Active });
                    _unitOfWork.Save();
                    return RedirectToAction("CreateAdmin", new { result = "success" });
                }
            }
            return View();
        }
        public ActionResult EditAdmin(int adminId = 0)
        {
            var admin = _unitOfWork.AdminRepository.GetById(adminId);
            if (admin == null)
            {
                return RedirectToAction("CreateAdmin");
            }

            var model = new EditAdminViewModel
            {
                Id = admin.Id,
                Username = admin.Username,
                Active = admin.Active,
            };

            return View(model);
        }
        [HttpPost]
        public ActionResult EditAdmin(EditAdminViewModel model)
        {
            if (ModelState.IsValid)
            {
                var admin = _unitOfWork.AdminRepository.GetById(model.Id);
                if (admin == null)
                {
                    return RedirectToAction("CreateAdmin");
                }
                if (admin.Username != model.Username)
                {
                    var exists = _unitOfWork.AdminRepository.GetQuery(a => a.Username.Equals(model.Username)).SingleOrDefault();
                    if (exists != null)
                    {
                        ModelState.AddModelError("", @"Tên đăng nhập này có rồi");
                        return View(model);
                    }
                    admin.Username = model.Username;
                }
                admin.Active = model.Active;
                if (model.Password != null)
                {
                    admin.Password = HtmlHelpers.ComputeHash(model.Password, "SHA256", null);
                }
                _unitOfWork.Save();
                return RedirectToAction("CreateAdmin", new { result = "update" });
            }
            return View(model);
        }
        public bool DeleteAdmin(string username)
        {
            var admin = _unitOfWork.AdminRepository.GetQuery(a => a.Username.Equals(username)).SingleOrDefault();
            if (admin == null)
            {
                return false;
            }
            if (username == "admin")
            {
                return false;
            }
            _unitOfWork.AdminRepository.Delete(admin);
            _unitOfWork.Save();
            return true;
        }
        public ActionResult ChangePassword(int result = 0)
        {
            ViewBag.Result = result;
            return View();
        }
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var admin = _unitOfWork.AdminRepository.GetQuery(a => a.Username.Equals(User.Identity.Name,
                StringComparison.OrdinalIgnoreCase)).SingleOrDefault();
                if (admin == null)
                {
                    return HttpNotFound();
                }
                if (HtmlHelpers.VerifyHash(model.OldPassword, "SHA256", admin.Password))
                {
                    admin.Password = HtmlHelpers.ComputeHash(model.Password, "SHA256", null);
                    _unitOfWork.Save();
                    return RedirectToAction("ChangePassword", new { result = 1 });
                }
                else
                {
                    ModelState.AddModelError("", @"Mật khẩu hiện tại không đúng!");
                    return View();
                }
            }
            return View(model);
        }
        #endregion

        #region ConfigSite
        public ActionResult ConfigSite(string result = "")
        {
            ViewBag.Result = result;
            var config = _unitOfWork.ConfigSiteRepository.Get().FirstOrDefault();
            var model = new ConfigViewModel
            {
                ConfigSite = config,
                PriceJob = config.PriceJob?.ToString("N0"),
                PriceStudyAbroad = config.PriceStudyAbroad?.ToString("N0"),
                PriceCompany = config.PriceCompany?.ToString("N0"),
                PriceArticle = config.PriceArticle?.ToString("N0"),
            };
            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult ConfigSite(ConfigViewModel model, FormCollection fc)
        {
            var config = _unitOfWork.ConfigSiteRepository.Get().FirstOrDefault();
            if (config == null)
            {
                _unitOfWork.ConfigSiteRepository.Insert(model.ConfigSite);
            }
            else
            {
                for (var i = 0; i < Request.Files.Count; i++)
                {
                    if (Request.Files[i] == null || Request.Files[i].ContentLength <= 0) continue;
                    if (!HtmlHelpers.CheckFileExt(Request.Files[i].FileName, "jpg|jpeg|png|gif")) continue;
                    if (Request.Files[i].ContentLength > 1024 * 1024 * 4) continue;

                    var imgFileName = HtmlHelpers.ConvertToUnSign(null, Path.GetFileNameWithoutExtension(Request.Files[i].FileName)) +
                        "-" + DateTime.Now.Millisecond + Path.GetExtension(Request.Files[i].FileName);
                    var imgPath = "/images/configs/" + DateTime.Now.ToString("yyyy/MM/dd");
                    HtmlHelpers.CreateFolder(Server.MapPath(imgPath));

                    var imgFile = DateTime.Now.ToString("yyyy/MM/dd") + "/" + imgFileName;

                    var newImage = Image.FromStream(Request.Files[i].InputStream);
                    var fixSizeImage = HtmlHelpers.FixedSize(newImage, 1000, 1000, false);
                    HtmlHelpers.SaveJpeg(Server.MapPath(Path.Combine(imgPath, imgFileName)), fixSizeImage, 90);

                    if (Request.Files.Keys[i] == "Image")
                    {
                        config.Image = imgFile;
                    }
                    else if (Request.Files.Keys[i] == "Favicon")
                    {
                        config.Favicon = imgFile;
                    }
                    else if (Request.Files.Keys[i] == "ImageShare")
                    {
                        config.ImageShare = imgFile;
                    }
                }
                if (model.PriceJob != null)
                {
                    config.PriceJob = Convert.ToDecimal(model.PriceJob.Replace(",", ""));
                }
                else
                {
                    config.PriceJob = null;
                }
                if (model.PriceStudyAbroad != null)
                {
                    config.PriceStudyAbroad = Convert.ToDecimal(model.PriceStudyAbroad.Replace(",", ""));
                }
                else
                {
                    config.PriceStudyAbroad = null;
                }
                if (model.PriceCompany != null)
                {
                    config.PriceCompany = Convert.ToDecimal(model.PriceCompany.Replace(",", ""));
                }
                else
                {
                    config.PriceCompany = null;
                }
                if (model.PriceArticle != null)
                {
                    config.PriceArticle = Convert.ToDecimal(model.PriceArticle.Replace(",", ""));
                }
                else
                {
                    config.PriceArticle = null;
                }
                config.Facebook = model.ConfigSite.Facebook;
                config.GoogleMap = model.ConfigSite.GoogleMap;
                config.Youtube = model.ConfigSite.Youtube;
                config.Instagram = model.ConfigSite.Instagram;
                config.Twitter = model.ConfigSite.Twitter;
                config.Title = model.ConfigSite.Title;
                config.Description = model.ConfigSite.Description;
                config.GoogleAnalytics = model.ConfigSite.GoogleAnalytics;
                config.Hotline = model.ConfigSite.Hotline;
                config.Email = model.ConfigSite.Email;
                config.LiveChat = model.ConfigSite.LiveChat;
                config.Place = model.ConfigSite.Place;
                config.AboutText = model.ConfigSite.AboutText;
                config.InfoFooter = model.ConfigSite.InfoFooter;
                config.EmailConfigs = model.ConfigSite.EmailConfigs;
                config.PassWordMail = model.ConfigSite.PassWordMail;
                config.NapTien = model.ConfigSite.NapTien;
                config.Hotline2 = model.ConfigSite.Hotline2;
                config.Hotline3 = model.ConfigSite.Hotline3;
                config.AboutContact = model.ConfigSite.AboutContact;
                //config.PriceJob = model.ConfigSite.PriceJob;
                //config.PriceStudyAbroad = model.ConfigSite.PriceStudyAbroad;
                //config.PriceCompany = model.ConfigSite.PriceCompany;
                _unitOfWork.Save();
                HttpContext.Application["ConfigSite"] = config;
                return RedirectToAction("ConfigSite", "Vcms", new { result = "success" });
            }
            return View("ConfigSite", model);
        }
        #endregion

        #region User
        public ActionResult ListUser(int? page, string name, string result)
        {
            ViewBag.Result = result;
            var pageNumber = page ?? 1;
            var pageSize = 15;
            var users = _unitOfWork.UserRepository.Get(orderBy: o => o.OrderByDescending(a => a.CreateDate));
            if (!string.IsNullOrEmpty(name))
            {
                users = users.Where(a => a.Username.Contains(name));
            }
            var model = new ListUserViewModel
            {
                Users = users.ToPagedList(pageNumber, pageSize),
                Name = name,
            };
            return View(model);
        }
        public ActionResult EditUser(int userId = 0)
        {
            var user = _unitOfWork.UserRepository.GetById(userId);
            if (user == null)
            {
                return RedirectToAction("ListUser");
            }

            var model = new EditUserViewModel
            {
                Id = user.Id,
                Username = user.Username,
                Active = user.Active,
            };

            return View(model);
        }
        [HttpPost]
        public ActionResult EditUser(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _unitOfWork.UserRepository.GetById(model.Id);
                if (user == null)
                {
                    return RedirectToAction("ListUser");
                }
                if (user.Username != model.Username)
                {
                    var exists = _unitOfWork.UserRepository.GetQuery(a => a.Username.Equals(model.Username)).SingleOrDefault();
                    if (exists != null)
                    {
                        ModelState.AddModelError("", @"Tên đăng nhập này có rồi");
                        return View(model);
                    }
                    user.Username = model.Username;
                }
                user.Active = model.Active;
                if (model.Password != null)
                {
                    user.Password = HtmlHelpers.ComputeHash(model.Password, "SHA256", null);
                }
                _unitOfWork.Save();
                return RedirectToAction("ListUser", new { result = "update" });
            }
            return View(model);
        }
        public bool DeleteUser(int userId)
        {
            var user = _unitOfWork.UserRepository.GetById(userId);
            if (user == null)
            {
                return false;
            }
            _unitOfWork.UserRepository.Delete(user);
            _unitOfWork.Save();
            return true;
        }
        #endregion

        #region Career
        public ActionResult ListCareers(int? page, string name, int type = 0 ,string result = "")
        {
            ViewBag.Result = result;
            var pageNumber = page ?? 1;
            var pageSize = 15;
            var careers = _unitOfWork.CareerRepository.Get(orderBy: o => o.OrderByDescending(a => a.CreateDate));
            if (!string.IsNullOrEmpty(name))
            {
                careers = careers.Where(a => a.Name.ToLower().Contains(name.ToLower()));
            }
            if (type > 0)
            {
                switch (type)
                {
                    case 1:
                        careers = careers.Where(a => a.TypeCareer == TypeCareer.Career);
                        break;
                    case 2:
                        careers = careers.Where(a => a.TypeCareer == TypeCareer.Activity);
                        break;
                }
            }
            var model = new ListCareerViewModel
            {
                Careers = careers.ToPagedList(pageNumber, pageSize),
                Name = name,
                Type = type
            };
            return View(model);
        }
        public ActionResult CreateCareer(int? result)
        {
            ViewBag.Result = result;
            var careers = _unitOfWork.CareerRepository.Get(p => p.ParentId == null);
            ViewBag.Careers = new SelectList(careers, "Id", "Name");
            var career = new Career { Active = true };
            return View(career);
        }
        [HttpPost]
        public ActionResult CreateCareer(Career model)
        {
            if (ModelState.IsValid)
            {
                var isPost = true;
                var file = Request.Files["Image"];
                if (file != null && file.ContentLength > 0)
                {
                    if (file.ContentType != "image/jpeg" & file.ContentType != "image/png" && file.ContentType != "image/gif")
                    {
                        ModelState.AddModelError("", @"Chỉ chấp nhận định dạng jpg, png, gif, jpeg");
                        isPost = false;
                    }
                    else
                    {
                        if (file.ContentLength > 4000 * 1024)
                        {
                            ModelState.AddModelError("", @"Dung lượng lớn hơn 4MB. Hãy thử lại");
                            isPost = false;
                        }
                        else
                        {
                            var imgPath = "/images/careers/" + DateTime.Now.ToString("yyyy/MM/dd");
                            HtmlHelpers.CreateFolder(Server.MapPath(imgPath));
                            var imgFileName = DateTime.Now.ToFileTimeUtc() + Path.GetExtension(file.FileName);

                            model.Image = DateTime.Now.ToString("yyyy/MM/dd") + "/" + imgFileName;

                            var newImage = Image.FromStream(file.InputStream);
                            var fixSizeImage = HtmlHelpers.FixedSize(newImage, 600, 600, false);
                            HtmlHelpers.SaveJpeg(Server.MapPath(Path.Combine(imgPath, imgFileName)), fixSizeImage, 90);
                            file.SaveAs(Server.MapPath(Path.Combine(imgPath, imgFileName)));
                        }
                    }
                }

                if (isPost)
                {
                    model.Code = HtmlHelpers.ConvertToUnSign(null, model.Code ?? model.Name);
                    _unitOfWork.CareerRepository.Insert(model);
                    _unitOfWork.Save();

                    var count = _unitOfWork.CareerRepository.GetQuery(a => a.Code == model.Code).Count();
                    if (count > 1)
                    {
                        model.Code += "-" + model.Id;
                        _unitOfWork.Save();
                    }

                    return RedirectToAction("CreateCareer", new { result = 0 });
                }
            }
            var careers1 = _unitOfWork.CareerRepository.Get(p => p.ParentId == null);
            ViewBag.Careers = new SelectList(careers1, "Id", "Name");
            return View(model);
        }
        public ActionResult UpdateCareer(int careerId = 0)
        {
            var career = _unitOfWork.CareerRepository.GetById(careerId);
            if (career == null)
            {
                return RedirectToAction("ListCareers", "Vcms", new { result = "update" });
            }
            var careers = _unitOfWork.CareerRepository.Get(p => p.ParentId == null);
            ViewBag.Careers = new SelectList(careers, "Id", "Name");
            return View(career);
        }
        [HttpPost]
        public ActionResult UpdateCareer(Career model, FormCollection fc)
        {
            if (ModelState.IsValid)
            {
                var isPost = true;
                var file = Request.Files["Image"];
                if (file != null && file.ContentLength > 0)
                {
                    if (file.ContentType != "image/jpeg" & file.ContentType != "image/png" && file.ContentType != "image/gif")
                    {
                        ModelState.AddModelError("", @"Chỉ chấp nhận định dạng jpg, png, gif, jpeg");
                        isPost = false;
                    }
                    else
                    {
                        if (file.ContentLength > 4000 * 1024)
                        {
                            ModelState.AddModelError("", @"Dung lượng lớn hơn 4MB. Hãy thử lại");
                            isPost = false;
                        }
                        else
                        {
                            var imgPath = "/images/careers/" + DateTime.Now.ToString("yyyy/MM/dd");
                            HtmlHelpers.CreateFolder(Server.MapPath(imgPath));
                            var imgFileName = DateTime.Now.ToFileTimeUtc() + Path.GetExtension(file.FileName);

                            model.Image = DateTime.Now.ToString("yyyy/MM/dd") + "/" + imgFileName;

                            var newImage = Image.FromStream(file.InputStream);
                            var fixSizeImage = HtmlHelpers.FixedSize(newImage, 600, 600, false);
                            HtmlHelpers.SaveJpeg(Server.MapPath(Path.Combine(imgPath, imgFileName)), fixSizeImage, 90);
                            file.SaveAs(Server.MapPath(Path.Combine(imgPath, imgFileName)));
                        }
                    }
                }
                else
                {
                    model.Image = fc["CurrentFile"];
                }

                if (isPost)
                {
                    model.Code = HtmlHelpers.ConvertToUnSign(null, model.Code ?? model.Name);
                    _unitOfWork.CareerRepository.Update(model);
                    _unitOfWork.Save();
                    var count = _unitOfWork.CareerRepository.GetQuery(a => a.Code == model.Code).Count();
                    if (count > 1)
                    {
                        model.Code += "-" + model.Id;
                        _unitOfWork.Save();
                    }

                    return RedirectToAction("ListCareers", new { result = "update" });
                }
            }
            var careers1 = _unitOfWork.CareerRepository.Get(p => p.ParentId == null);
            ViewBag.Careers = new SelectList(careers1, "Id", "Name");
            return View(model);
        }
        [HttpPost]
        public bool DeleteCareer(int careerId = 0)
        {
            var career = _unitOfWork.CareerRepository.GetById(careerId);
            if (career == null)
            {
                return false;
            }
            _unitOfWork.CareerRepository.Delete(career);
            _unitOfWork.Save();
            return true;
        }
        [HttpPost]
        public bool QuickUpdateCareer(bool active = false, bool home = false, bool hot = false, bool menu = false, int careerId = 0)
        {
            var career = _unitOfWork.CareerRepository.GetById(careerId);
            if (career == null)
            {
                return false;
            }
            career.ShowHome = home;
            career.Active = active;
            career.Hot = hot;
            career.Menu = menu;
            _unitOfWork.Save();
            return true;
        }
        #endregion

        #region Rank
        [ChildActionOnly]
        public ActionResult ListRank()
        {
            var ranks = _unitOfWork.RankRepository.Get(orderBy: o => o.OrderBy(a => a.Id));
            return PartialView(ranks);
        }
        public ActionResult CreateRank(string result = "")
        {
            ViewBag.Result = result;
            var ranks = new Rank { };
            return View(ranks);
        }
        [HttpPost]
        public ActionResult CreateRank(Rank model)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.RankRepository.Insert(model);
                _unitOfWork.Save();
                return RedirectToAction("CreateRank", new { result = "success" });
            }
            return View(model);
        }
        public ActionResult UpdateRank(int rankId = 0)
        {
            var rank = _unitOfWork.RankRepository.GetById(rankId);
            if (rank == null)
            {
                return RedirectToAction("CreateRank", "Vcms");
            }
            return View(rank);
        }
        [HttpPost]
        public ActionResult UpdateRank(Rank model)
        {

            if (ModelState.IsValid)
            {
                _unitOfWork.RankRepository.Update(model);
                _unitOfWork.Save();
                return RedirectToAction("CreateRank", new { result = "success" });
            }
            return View(model);
        }
        [HttpPost]
        public bool DeleteRank(int rankId = 0)
        {
            var rank = _unitOfWork.RankRepository.GetById(rankId);
            if (rank == null)
            {
                return false;
            }
            _unitOfWork.CareerRepository.Delete(rank);
            _unitOfWork.Save();
            return true;
        }
        #endregion

        #region JobType
        [ChildActionOnly]
        public ActionResult ListJobType()
        {
            var jobType = _unitOfWork.JobTypeRepository.Get(orderBy: o => o.OrderBy(a => a.Id));
            return PartialView(jobType);
        }
        public ActionResult CreateJobType(string result = "")
        {
            ViewBag.Result = result;
            var jobType = new JobType { };
            return View(jobType);
        }
        [HttpPost]
        public ActionResult CreateJobType(JobType model)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.JobTypeRepository.Insert(model);
                _unitOfWork.Save();
                return RedirectToAction("CreateJobType", new { result = "success" });
            }
            return View(model);
        }
        public ActionResult UpdateJobType(int jobtypeId = 0)
        {
            var career = _unitOfWork.JobTypeRepository.GetById(jobtypeId);
            if (career == null)
            {
                return RedirectToAction("CreateJobType", "Vcms");
            }
            return View(career);
        }
        [HttpPost]
        public ActionResult UpdateJobType(JobType model)
        {

            if (ModelState.IsValid)
            {
                _unitOfWork.JobTypeRepository.Update(model);
                _unitOfWork.Save();
                return RedirectToAction("CreateJobType", new { result = "success" });
            }
            return View(model);
        }
        [HttpPost]
        public bool DeleteJobType(int jobtypeId = 0)
        {
            var jobType = _unitOfWork.JobTypeRepository.GetById(jobtypeId);
            if (jobType == null)
            {
                return false;
            }
            _unitOfWork.JobTypeRepository.Delete(jobType);
            _unitOfWork.Save();
            return true;
        }
        #endregion

        #region Skill
        public ActionResult ListSkill(int? page, string name, int type = 0, string result = "")
        {
            ViewBag.Result = result;
            var pageNumber = page ?? 1;
            var pageSize = 15;

            var skills = _unitOfWork.SkillRepository.Get(orderBy: o => o.OrderByDescending(a => a.CreateDate));
            if (!string.IsNullOrEmpty(name))
            {
                skills = skills.Where(a => a.SkillName.ToLower().Contains(name.ToLower()));
            }
            if (type > 0)
            {
                switch (type)
                {
                    case 1:
                        skills = skills.Where(a => a.TypeSkill == TypeSkill.Skill);
                        break;
                    case 2:
                        skills = skills.Where(a => a.TypeSkill == TypeSkill.Keyword);
                        break;
                }
            }
            var model = new ListSkillViewModel
            {
                Skills = skills.ToPagedList(pageNumber, pageSize),
                Name = name,
                Type = type
            };
            return View(model);

        }
        public ActionResult CreateSkill(int? result)
        {
            ViewBag.Result = result;
            var skill = new Skill { CreateDate = DateTime.Now };
            return View(skill);
        }
        [HttpPost]
        public ActionResult CreateSkill(Skill model)
        {
            if (ModelState.IsValid)
            {
                model.Code = HtmlHelpers.ConvertToUnSign(null, model.Code ?? model.SkillName);
                _unitOfWork.SkillRepository.Insert(model);
                _unitOfWork.Save();
                var count = _unitOfWork.SkillRepository.GetQuery(a => a.Code == model.Code).Count();
                if (count > 1)
                {
                    model.Code += "-" + model.Id;
                    _unitOfWork.Save();
                }
                return RedirectToAction("CreateSkill", new { result = 0 });
            }
            return View(model);
        }
        public ActionResult UpdateSkill(int skillId = 0)
        {
            var skill = _unitOfWork.SkillRepository.GetById(skillId);
            if (skill == null)
            {
                return RedirectToAction("CreateSkill", "Vcms");
            }
            return View(skill);
        }
        [HttpPost]
        public ActionResult UpdateSkill(Skill model)
        {
            if (ModelState.IsValid)
            {
                model.Code = HtmlHelpers.ConvertToUnSign(null, model.Code ?? model.SkillName);
                _unitOfWork.SkillRepository.Update(model);
                _unitOfWork.Save();
                var count = _unitOfWork.SkillRepository.GetQuery(a => a.Code == model.Code).Count();
                if (count > 1)
                {
                    model.Code += "-" + model.Id;
                    _unitOfWork.Save();
                }
                return RedirectToAction("ListSkill", new { result = "update" });
            }
            return View(model);
        }
        [HttpPost]
        public bool QuickUpdateSkill(bool home = false, int skillId = 0)
        {
            var skill = _unitOfWork.SkillRepository.GetById(skillId);
            if (skill == null)
            {
                return false;
            }
            skill.ShowHome = home;
            _unitOfWork.Save();
            return true;
        }
        [HttpPost]
        public bool DeleteSkill(int skillId = 0)
        {
            var skill = _unitOfWork.SkillRepository.GetById(skillId);
            if (skill == null)
            {
                return false;
            }
            _unitOfWork.SkillRepository.Delete(skill);
            _unitOfWork.Save();
            return true;
        }
        #endregion

        #region Countruy
        public ActionResult ListCountruy(int? page, string name, string result = "")
        {
            ViewBag.Result = result;
            var pageNumber = page ?? 1;
            var pageSize = 10;
            var countruy = _unitOfWork.CountryRepository.Get(orderBy: a => a.OrderBy(l => l.Sort));
            if (!string.IsNullOrEmpty(name))
            {
                countruy = countruy.Where(a => a.Name.ToLower().Contains(name.ToLower()));
            }
            var model = new ListCountruyViewModel
            {
                Countries = countruy.ToPagedList(pageNumber, pageSize),
                Name = name
            };
            return View(model);
        }
        public ActionResult Countruy()
        {
            var countruy = new Country
            {
                Sort = 1,
            };
            return View(countruy);

        }
        [HttpPost]
        public ActionResult Countruy(Country country)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.CountryRepository.Insert(country);
                _unitOfWork.Save();
                return RedirectToAction("ListCountruy", new { result = "success" });
            }
            return View(country);
        }
        public ActionResult EditCountruy(int id = 0)
        {
            var countruy = _unitOfWork.CountryRepository.GetById(id);
            if (countruy == null)
            {
                return RedirectToAction("ListCountruy");
            }
            return View(countruy);
        }
        [HttpPost]
        public ActionResult EditCountruy(Country model)
        {
            var countruy = _unitOfWork.CountryRepository.GetById(model.Id);
            if (countruy == null)
            {
                return RedirectToAction("ListCountruy");
            }
            countruy.Name = model.Name;
            countruy.Sort = model.Sort;
            countruy.Active = model.Active;
            countruy.Footer = model.Footer;
            countruy.Hot = model.Hot;
            countruy.Job = model.Job;
            countruy.School = model.School;
            countruy.FilterJob = model.FilterJob;
            countruy.FilterStudy = model.FilterStudy;
            countruy.Filter = model.Filter;
            countruy.Scholarship = model.Scholarship;
            _unitOfWork.Save();
            return RedirectToAction("ListCountruy", new { result = "update" });
        }
        [HttpPost]
        public bool DeleteCountruy(int id)
        {
            var countruy = _unitOfWork.CountryRepository.GetById(id);
            if (countruy == null)
            {
                return false;
            }
            _unitOfWork.CountryRepository.Delete(countruy);
            _unitOfWork.Save();
            return true;
        }
        [HttpPost]
        public bool UpdateCountruy(int id, bool active, int sort)
        {
            var countruy = _unitOfWork.CountryRepository.GetById(id);
            if (countruy == null)
            {
                return false;
            }
            countruy.Active = active;
            countruy.Sort = sort;
            _unitOfWork.Save();
            return true;
        }
        #endregion

        #region City
        [ChildActionOnly]
        public PartialViewResult ListCity()
        {
            var cities = _unitOfWork.CityRepository.Get(orderBy: q => q.OrderBy(c => c.CountruyId));
            var model = new ListCityViewModel
            {
                Citys = cities,
                Countries = _unitOfWork.CountryRepository.GetQuery(a => a.Active, o => o.OrderBy(a => a.Id))
            };
            return PartialView(model);
        }
        public ActionResult City()
        {
            var model = new InsertCityViewModel
            {
                City = new City { Sort = 1, Active = true },
                Countries = _unitOfWork.CountryRepository.GetQuery(a => a.Active, o => o.OrderBy(c => c.Sort)),
            };
            return View(model);
        }
        [HttpPost]
        public ActionResult City(InsertCityViewModel model, FormCollection fc)
        {
            if (ModelState.IsValid)
            {
                model.City.CountruyId = Convert.ToInt32(fc["CountruyId"]);
                model.City.Country = _unitOfWork.CountryRepository.GetById(Convert.ToInt32(fc["CountruyId"]));
                _unitOfWork.CityRepository.Insert(model.City);
                _unitOfWork.Save();
                return RedirectToAction("City");
            }
            model.Countries = _unitOfWork.CountryRepository.GetQuery(a => a.Active, o => o.OrderBy(c => c.Sort));
            return View(model);
        }
        public ActionResult EditCity(int cityId = 0)
        {
            var city = _unitOfWork.CityRepository.GetById(cityId);
            if (city == null)
            {
                return RedirectToAction("City");
            }

            var model = new InsertCityViewModel
            {
                City = city,
                Countries = _unitOfWork.CountryRepository.GetQuery(a => a.Active, o => o.OrderBy(c => c.Sort)),
            };

            return View(model);
        }
        [HttpPost]
        public ActionResult EditCity(InsertCityViewModel model, FormCollection fc)
        {
            var city = _unitOfWork.CityRepository.GetById(model.City.Id);
            if (city == null)
            {
                return RedirectToAction("City");
            }
            if (ModelState.IsValid)
            {
                city.Name = model.City.Name;
                city.Sort = model.City.Sort;
                city.Active = model.City.Active;
                city.Home = model.City.Home;
                city.CountruyId = Convert.ToInt32(fc["CountruyId"]);
                city.Country = _unitOfWork.CountryRepository.GetById(Convert.ToInt32(fc["CountruyId"]));
                _unitOfWork.Save();
                return RedirectToAction("City");
            }
            return View(model);
        }
        [HttpPost]
        public bool DeleteCity(int cityId = 0)
        {
            var city = _unitOfWork.CityRepository.GetById(cityId);
            if (city == null)
            {
                return false;
            }

            _unitOfWork.CityRepository.Delete(city);
            _unitOfWork.Save();
            return true;
        }
        [HttpPost]
        public JsonResult QickUpdateCity(int sort = 1, bool active = false, int id = 0)
        {
            var city = _unitOfWork.CityRepository.GetById(id);
            if (city == null)
            {
                return Json(new { success = false, message = "Thành phố không tồn tại." });
            }

            city.Active = active;
            city.Sort = sort;
            _unitOfWork.Save();

            return Json(new { success = true, message = "Cập nhật thành công." });
        }
        #endregion

        #region StudyAbroad
        public PartialViewResult ListCat()
        {
            var cate = _unitOfWork.StudyAbroadCategoryRepository.Get(orderBy: a => a.OrderBy(l => l.Sort));
            return PartialView(cate);
        }
        public ActionResult StudyAbroadCategory()
        {
            var model = new InsertStudyAbroadCategoryViewModel
            {
                Countries = _unitOfWork.CountryRepository.Get(orderBy: a => a.OrderBy(l => l.Sort)),
                StudyAbroadCategory = new StudyAbroadCategory
                {
                    Active = true,
                    Sort = 1
                }
            };
            ViewBag.RootCats = new SelectList(
                    _unitOfWork.StudyAbroadCategoryRepository.Get(a => a.ParentId == null, q => q.OrderBy(a => a.Sort)), "Id", "CategoryName");
            return View(model);
        }
        [HttpPost]
        public ActionResult StudyAbroadCategory(InsertStudyAbroadCategoryViewModel model, FormCollection fc)
        {
            if (ModelState.IsValid)
            {
                for (var i = 0; i < Request.Files.Count; i++)
                {
                    if (Request.Files[i] == null || Request.Files[i].ContentLength <= 0) continue;
                    if (!HtmlHelpers.CheckFileExt(Request.Files[i].FileName, "jpg|jpeg|png|gif")) continue;
                    if (Request.Files[i].ContentLength > 1024 * 1024 * 4) continue;
                    var imgFileName = HtmlHelpers.ConvertToUnSign(null, Path.GetFileNameWithoutExtension(Request.Files[i].FileName)) +
                        "-" + DateTime.Now.Millisecond + Path.GetExtension(Request.Files[i].FileName);
                    var imgPath = "/images/category/" + DateTime.Now.ToString("yyyy/MM/dd");
                    HtmlHelpers.CreateFolder(Server.MapPath(imgPath));
                    var imgFile = DateTime.Now.ToString("yyyy/MM/dd") + "/" + imgFileName;
                    var newImage = Image.FromStream(Request.Files[i].InputStream);
                    var fixSizeImage = HtmlHelpers.FixedSize(newImage, 1500, 1700, false);
                    HtmlHelpers.SaveJpeg(Server.MapPath(Path.Combine(imgPath, imgFileName)), fixSizeImage, 90);
                    if (Request.Files.Keys[i] == "StudyAbroadCategory.Image")
                    {
                        model.StudyAbroadCategory.Image = imgFile;
                    }
                    if (Request.Files.Keys[i] == "StudyAbroadCategory.Banner")
                    {
                        model.StudyAbroadCategory.Banner = imgFile;
                    }
                }
                model.StudyAbroadCategory.Url = HtmlHelpers.ConvertToUnSign(null, model.StudyAbroadCategory.Url ?? model.StudyAbroadCategory.CategoryName);
                model.StudyAbroadCategory.CountryId = Convert.ToInt32(fc["CounId"]);
                model.StudyAbroadCategory.Country = _unitOfWork.CountryRepository.GetById(model.StudyAbroadCategory.CountryId);
                _unitOfWork.StudyAbroadCategoryRepository.Insert(model.StudyAbroadCategory);
                _unitOfWork.Save();
                var cat = _unitOfWork.StudyAbroadCategoryRepository.GetQuery(a => a.Url == model.StudyAbroadCategory.Url).Count();
                if (cat > 1)
                {
                    model.StudyAbroadCategory.Url += "-" + model.StudyAbroadCategory.Id;
                    _unitOfWork.Save();
                }
                return RedirectToAction("StudyAbroadCategory", new { result = "success" });
            }
            return View(model);
        }
        public ActionResult EditCategory(int id = 0)
        {
            var cat = _unitOfWork.StudyAbroadCategoryRepository.GetById(id);
            if (cat == null)
            {
                return RedirectToAction("StudyAbroadCategory");
            }
            var model = new InsertStudyAbroadCategoryViewModel
            {
                StudyAbroadCategory = cat,
                Countries = _unitOfWork.CountryRepository.Get(orderBy: a => a.OrderBy(l => l.Sort)),
            };
            ViewBag.RootCats = new SelectList(_unitOfWork.StudyAbroadCategoryRepository.Get(l => l.ParentId == null, a => a.OrderBy(c => c.Sort)), "Id", "CategoryName");
            return View(model);
        }
        [HttpPost]
        public ActionResult EditCategory(InsertStudyAbroadCategoryViewModel category, FormCollection fc)
        {
            if (ModelState.IsValid)
            {
                for (var i = 0; i < Request.Files.Count; i++)
                {
                    if (Request.Files[i] == null || Request.Files[i].ContentLength <= 0) continue;
                    if (!HtmlHelpers.CheckFileExt(Request.Files[i].FileName, "jpg|jpeg|png|gif")) continue;
                    if (Request.Files[i].ContentLength > 1024 * 1024 * 4) continue;

                    var imgFileName = HtmlHelpers.ConvertToUnSign(null, Path.GetFileNameWithoutExtension(Request.Files[i].FileName)) +
                        "-" + DateTime.Now.Millisecond + Path.GetExtension(Request.Files[i].FileName);
                    var imgPath = "/images/category/" + DateTime.Now.ToString("yyyy/MM/dd");
                    HtmlHelpers.CreateFolder(Server.MapPath(imgPath));

                    var imgFile = DateTime.Now.ToString("yyyy/MM/dd") + "/" + imgFileName;

                    var newImage = Image.FromStream(Request.Files[i].InputStream);
                    var fixSizeImage = HtmlHelpers.FixedSize(newImage, 1500, 1700, false);
                    HtmlHelpers.SaveJpeg(Server.MapPath(Path.Combine(imgPath, imgFileName)), fixSizeImage, 90);

                    if (Request.Files.Keys[i] == "StudyAbroadCategory.Image")
                    {
                        category.StudyAbroadCategory.Image = imgFile;
                    }
                    if (Request.Files.Keys[i] == "StudyAbroadCategory.Banner")
                    {
                        category.StudyAbroadCategory.Banner = imgFile;
                    }
                }

                var file = Request.Files["StudyAbroadCategory.Image"];

                if (file != null && file.ContentLength == 0)
                {
                    category.StudyAbroadCategory.Image = fc["CurrentFile"] == "" ? null : fc["CurrentFile"];
                }
                var banner = Request.Files["StudyAbroadCategory.Banner"];

                if (banner != null && banner.ContentLength == 0)
                {
                    category.StudyAbroadCategory.Banner = fc["CurrentBanner"] == "" ? null : fc["CurrentBanner"];
                }
                category.StudyAbroadCategory.Url = HtmlHelpers.ConvertToUnSign(null, category.StudyAbroadCategory.Url ?? category.StudyAbroadCategory.CategoryName);
                category.StudyAbroadCategory.CountryId = Convert.ToInt32(fc["CounId"]);
                category.StudyAbroadCategory.Country = _unitOfWork.CountryRepository.GetById(category.StudyAbroadCategory.CountryId);
                _unitOfWork.StudyAbroadCategoryRepository.Update(category.StudyAbroadCategory);
                var cat = _unitOfWork.StudyAbroadCategoryRepository.GetQuery(a => a.Url == category.StudyAbroadCategory.Url).Count();
                if (cat > 1)
                {
                    category.StudyAbroadCategory.Url += "-" + category.StudyAbroadCategory.Id;
                    _unitOfWork.Save();
                }
                _unitOfWork.Save();
                return RedirectToAction("StudyAbroadCategory", new { result = "update" });
            }
            category.Countries = _unitOfWork.CountryRepository.Get(orderBy: a => a.OrderBy(l => l.Sort));
            ViewBag.RootCats = new SelectList(_unitOfWork.StudyAbroadCategoryRepository.Get(l => l.ParentId == null, a => a.OrderBy(c => c.Sort)), "Id", "CategoryName");
            return View(category);
        }
        [HttpPost]
        public bool DeleteCategory(int id)
        {
            var cat = _unitOfWork.StudyAbroadCategoryRepository.GetById(id);
            if (cat == null)
            {
                return false;
            }
            _unitOfWork.StudyAbroadCategoryRepository.Delete(cat);
            _unitOfWork.Save();
            return true;
        }
        [HttpPost]
        public bool UpdateCategory(int id = 0, bool active = false, bool menu = false, int sort = 0)
        {
            var cat = _unitOfWork.StudyAbroadCategoryRepository.GetById(id);
            if (cat == null)
            {
                return false;
            }
            cat.Active = active;
            cat.Sort = sort;
            cat.ShowMenu = menu;
            _unitOfWork.Save();
            return true;
        }
        #endregion

        #region Text
        public ActionResult ListTitle()
        {
            string xmlFilePath = Server.MapPath("~/Text.xml");
            XDocument xmlDoc = XDocument.Load(xmlFilePath);

            List<Item> items = xmlDoc.Root.Elements("Item")
                .Select(item => new Item
                {
                    Sort = (int)item.Element("Sort"),
                    Title = item.Element("Title").Value,
                    Description = item.Element("Des").Value,
                })
                .ToList();
            return View(items);
        }
        public ActionResult EditTitle(int sort = 0)
        {
            string xmlFilePath = Server.MapPath("~/Text.xml");
            XDocument xmlDoc = XDocument.Load(xmlFilePath);
            XElement itemElement = xmlDoc.Descendants("Item")
                                         .FirstOrDefault(x => (int)x.Element("Sort") == sort);
            if (itemElement == null)
            {
                return RedirectToAction("ListTitle");
            }
            Item model = new Item
            {
                Sort = (int)itemElement.Element("Sort"),
                Title = itemElement.Element("Title").Value,
                Description = itemElement.Element("Des").Value,
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult EditTitle(Item updatedItem)
        {
            string xmlFilePath = Server.MapPath("~/Text.xml");
            XDocument xmlDoc = XDocument.Load(xmlFilePath);
            XElement itemElement = xmlDoc.Descendants("Item")
                                         .FirstOrDefault(x => (int)x.Element("Sort") == updatedItem.Sort);
            if (itemElement == null)
            {
                return RedirectToAction("ListTitle");
            }
            itemElement.SetElementValue("Title", updatedItem.Title);
            itemElement.SetElementValue("Des", updatedItem.Description);
            xmlDoc.Save(xmlFilePath);
            return RedirectToAction("ListTitle");
        }

        #endregion

        #region Parter 
        public ActionResult ListParter(int? page, string name)
        {
            var pageNumber = page ?? 1;
            var partner = _unitOfWork.ParterRepository.Get(orderBy: a => a.OrderBy(l => l.Sort));
            if (!string.IsNullOrEmpty(name))
            {
                partner = partner.Where(a => a.Name.Contains(name));
            }
            var model = new ListPartnerViewModel
            {
                Partners = partner.ToPagedList(pageNumber, 9),
                Name = name
            };
            return View(model);
        }

        public ActionResult Parter()
        {
            var Parter = new Partner
            {
                Sort = 1
            };
            return View(Parter);
        }
        [HttpPost]
        public ActionResult Parter(Partner model)
        {
            if (ModelState.IsValid)
            {
                var file = Request.Files["Image"];
                if (file == null || file.ContentLength <= 0)
                {
                    ModelState.AddModelError("", @"Hãy chọn 1 hình ảnh.");
                    return View(model);
                }

                if (!HtmlHelpers.CheckFileExt(file.FileName, "jpg|jpeg|png|gif|svg"))
                {
                    ModelState.AddModelError("", @"Chỉ chấp nhận định dạng jpg, png, gif, jpeg, svg");
                    return View(model);
                }

                if (file.ContentLength > 4000 * 1024)
                {
                    ModelState.AddModelError("", @"Dung lượng lớn hơn 4MB. Hãy thử lại");
                    return View(model);
                }

                var imgPath = "/images/partner/" + DateTime.Now.ToString("yyyy/MM/dd");
                HtmlHelpers.CreateFolder(Server.MapPath(imgPath));
                var imgFileName = HtmlHelpers.ConvertToUnSign(null, Path.GetFileNameWithoutExtension(file.FileName)) + "-" + DateTime.Now.Millisecond + Path.GetExtension(file.FileName);

                model.Image = DateTime.Now.ToString("yyyy/MM/dd") + "/" + imgFileName;
                file.SaveAs(Server.MapPath(Path.Combine(imgPath, imgFileName)));

                _unitOfWork.ParterRepository.Insert(model);
                _unitOfWork.Save();
                return RedirectToAction("ListParter");
            }
            return View(model);
        }

        public ActionResult UpdatePartner(int id)
        {
            var partner = _unitOfWork.ParterRepository.GetById(id);
            if (partner == null)
            {
                return RedirectToAction("Parter");
            }
            return View(partner);
        }
        [HttpPost]
        public ActionResult UpdatePartner(Partner model)
        {
            if (ModelState.IsValid)
            {
                var isPost = true;

                var partner = _unitOfWork.ParterRepository.GetById(model.Id);

                var file = Request.Files["Image"];
                if (file != null && file.ContentLength > 0)
                {
                    if (!HtmlHelpers.CheckFileExt(file.FileName, "jpg|jpeg|png|gif|svg"))
                    {
                        ModelState.AddModelError("", @"Chỉ chấp nhận định dạng jpg, png, gif, jpeg, svg");
                        isPost = false;
                    }
                    else
                    {
                        if (file.ContentLength > 4000 * 1024)
                        {
                            ModelState.AddModelError("", @"Dung lượng lớn hơn 4MB. Hãy thử lại");
                            isPost = false;
                        }
                        else
                        {
                            var imgPath = "/images/partner/" + DateTime.Now.ToString("yyyy/MM/dd");
                            HtmlHelpers.CreateFolder(Server.MapPath(imgPath));
                            //var imgFileName = DateTime.Now.ToFileTimeUtc() + Path.GetExtension(file.FileName);
                            var imgFileName = HtmlHelpers.ConvertToUnSign(null, Path.GetFileNameWithoutExtension(file.FileName)) + "-" + DateTime.Now.Millisecond + Path.GetExtension(file.FileName);
                            partner.Image = DateTime.Now.ToString("yyyy/MM/dd") + "/" + imgFileName;
                            file.SaveAs(Server.MapPath(Path.Combine(imgPath, imgFileName)));
                        }
                    }
                }

                if (isPost)
                {
                    partner.Name = model.Name;
                    partner.Sort = model.Sort;
                    partner.Url = model.Url;
                    partner.Footer = model.Footer;
                    partner.Active = model.Active;
                    _unitOfWork.ParterRepository.Update(partner);
                    _unitOfWork.Save();
                    return RedirectToAction("ListParter", new { result = "update" });
                }
            }
            return View(model);
        }

        [HttpPost]
        public bool DeletePartner(int id)
        {
            var partner = _unitOfWork.ParterRepository.GetById(id);
            if (partner == null)
            {
                return false;
            }
            _unitOfWork.ParterRepository.Delete(partner);
            _unitOfWork.Save();
            return true;
        }

        #endregion

        public ActionResult ListService(int? page , string name = "" , int type = 0 )
        {
            var pageNumber = page ?? 1;
            var service = _unitOfWork.ServiceRepository.Get(orderBy: a => a.OrderByDescending(o => o.CreateDate));
            if (!string.IsNullOrEmpty(name))
            {
                service = service.Where(a => a.Name.Contains(name));
            }
            if(type > 0)
            {
                switch(type)
                {
                    case 1:
                        service = service.Where(a => a.TypeService == TypeService.Job);
                        break;
                    case 2:
                        service = service.Where(a => a.TypeService == TypeService.StudyAbroad);
                        break;
                    //case 3:
                    //    service = service.Where(a => a.TypeService == TypeService.Company);
                    //    break;
                }
            }
            var model = new ListServiceViewModel
            {
                Services = service.ToPagedList(pageNumber , 10),
                Name = name,
                Type = type,
            };
            return View(model);
        }
        public ActionResult Service()
        {
            var model = new ServiceViewModel
            {
               Service = new Service { Sort = 1, IntDate = 1 , Active = true }
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult Service(ServiceViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Amount != null)
                {
                    model.Service.Amount = Convert.ToDecimal(model.Amount.Replace(",", ""));
                }
                for (var i = 0; i < Request.Files.Count; i++)
                {
                    if (Request.Files[i] == null || Request.Files[i].ContentLength <= 0) continue;
                    if (!HtmlHelpers.CheckFileExt(Request.Files[i].FileName, "jpg|jpeg|png|gif")) continue;
                    if (Request.Files[i].ContentLength > 1024 * 1024 * 4) continue;

                    var imgFileName = HtmlHelpers.ConvertToUnSign(null, Path.GetFileNameWithoutExtension(Request.Files[i].FileName)) +
                        "-" + DateTime.Now.Millisecond + Path.GetExtension(Request.Files[i].FileName);
                    var imgPath = "/images/service/" + DateTime.Now.ToString("yyyy/MM/dd");
                    HtmlHelpers.CreateFolder(Server.MapPath(imgPath));

                    var imgFile = DateTime.Now.ToString("yyyy/MM/dd") + "/" + imgFileName;

                    var newImage = Image.FromStream(Request.Files[i].InputStream);
                    var fixSizeImage = HtmlHelpers.FixedSize(newImage, 1000, 1000, false);
                    HtmlHelpers.SaveJpeg(Server.MapPath(Path.Combine(imgPath, imgFileName)), fixSizeImage, 90);

                    if (Request.Files.Keys[i] == "Service.Image")
                    {
                        model.Service.Image = imgFile;
                    }
                }

                _unitOfWork.ServiceRepository.Insert(model.Service);
                _unitOfWork.Save();
                return RedirectToAction("ListService" , new {result = "success"});
            }
            return View(model);
        }
        public ActionResult EditService(int id)
        {
            var service = _unitOfWork.ServiceRepository.GetById(id);
            if(service ==  null)
            {
                return RedirectToAction("Service");
            }
            var model = new ServiceViewModel
            {
                Service = service,
                Amount = service.Amount?.ToString("N0"),
            };
            return View(model);
        }
        [HttpPost]
        public ActionResult EditService(ServiceViewModel model , FormCollection fc)
        {
            var service = _unitOfWork.ServiceRepository.GetById(model.Service.Id);
            if(service == null)
            {
                return RedirectToAction("Service");
            }
            if (ModelState.IsValid)
            {
                if (model.Amount != null)
                {
                    service.Amount = Convert.ToDecimal(model.Amount.Replace(",", ""));
                }
                else
                {
                    service.Amount = null;
                }
                for (var i = 0; i < Request.Files.Count; i++)
                {
                    if (Request.Files[i] == null || Request.Files[i].ContentLength <= 0) continue;
                    if (!HtmlHelpers.CheckFileExt(Request.Files[i].FileName, "jpg|jpeg|png|gif")) continue;
                    if (Request.Files[i].ContentLength > 1024 * 1024 * 4) continue;

                    var imgFileName = HtmlHelpers.ConvertToUnSign(null, Path.GetFileNameWithoutExtension(Request.Files[i].FileName)) +
                        "-" + DateTime.Now.Millisecond + Path.GetExtension(Request.Files[i].FileName);
                    var imgPath = "/images/service/" + DateTime.Now.ToString("yyyy/MM/dd");
                    HtmlHelpers.CreateFolder(Server.MapPath(imgPath));

                    var imgFile = DateTime.Now.ToString("yyyy/MM/dd") + "/" + imgFileName;

                    var newImage = Image.FromStream(Request.Files[i].InputStream);
                    var fixSizeImage = HtmlHelpers.FixedSize(newImage, 1000, 1000, false);
                    HtmlHelpers.SaveJpeg(Server.MapPath(Path.Combine(imgPath, imgFileName)), fixSizeImage, 90);

                    if (Request.Files.Keys[i] == "Service.Image")
                    {
                        service.Image = imgFile;
                    }
                }
                var file = Request.Files["Service.Image"];

                if (file != null && file.ContentLength == 0)
                {
                    service.Image = fc["CurrentFile"] == "" ? null : fc["CurrentFile"];
                }
                service.Name = model.Service.Name;
                service.IntDate = model.Service.IntDate;
                service.Sort = model.Service.Sort;
                service.Description = model.Service.Description;
                service.Active = model.Service.Active;
                service.TypeService = model.Service.TypeService;
                _unitOfWork.Save();
                return RedirectToAction("ListService", new { result = "success" });
            }
            return View(model);
        }

        [HttpPost]
        public bool DeleteService(int id)
        {
            var service = _unitOfWork.ServiceRepository.GetById(id);
            if(service  == null)
            {
                return false;   
            }
            _unitOfWork.ServiceRepository.Delete(service);
            _unitOfWork.Save();
            return true;
        }
        protected override void Dispose(bool disposing)
        {
            _unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}