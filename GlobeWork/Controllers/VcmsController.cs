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
                //Articles = _unitOfWork.ArticleRepository.GetQuery().Count(),
                //Contacts = _unitOfWork.ContactRepository.GetQuery().Count(),
                //Banners = _unitOfWork.BannerRepository.GetQuery().Count(),
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
            return View(config);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult ConfigSite(ConfigSite model, FormCollection fc)
        {
            var config = _unitOfWork.ConfigSiteRepository.Get().FirstOrDefault();
            if (config == null)
            {
                _unitOfWork.ConfigSiteRepository.Insert(model);
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

                config.Facebook = model.Facebook;
                config.GoogleMap = model.GoogleMap;
                config.Youtube = model.Youtube;
                config.Instagram = model.Instagram;
                config.Twitter = model.Twitter;
                config.Title = model.Title;
                config.Description = model.Description;
                config.GoogleAnalytics = model.GoogleAnalytics;
                config.Hotline = model.Hotline;
                config.Email = model.Email;
                config.LiveChat = model.LiveChat;
                config.Place = model.Place;
                config.AboutText = model.AboutText;
                config.InfoFooter = model.InfoFooter;
                config.EmailConfigs = model.EmailConfigs;
                config.PassWordMail = model.PassWordMail;
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
        [HttpPost]
        public ActionResult ListCareer(int? page, string name, string result = "")
        {
            ViewBag.Result = result;
            var pageNumber = page ?? 1;
            var pageSize = 15;
            var careers = _unitOfWork.CareerRepository.Get(orderBy: o => o.OrderBy(a => a.Name));
            if (!string.IsNullOrEmpty(name))
            {
                careers = careers.Where(a => a.Name.ToLower().Contains(name.ToLower()));
            }
            var model = new ListCareerViewModel
            {
                Careers = careers.ToPagedList(pageNumber, pageSize),
                Name = name,
            };
            return View(model);
        }
        public ActionResult ListCareers(int? page, string name, string result = "")
        {
            ViewBag.Result = result;
            var pageNumber = page ?? 1;
            var pageSize = 15;
            var careers = _unitOfWork.CareerRepository.Get(orderBy: o => o.OrderBy(a => a.Name));
            if (!string.IsNullOrEmpty(name))
            {
                careers = careers.Where(a => a.Name.ToLower().Contains(name.ToLower()));
            }
            var model = new ListCareerViewModel
            {
                Careers = careers.ToPagedList(pageNumber, pageSize),
                Name = name,
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
                    model.Url = HtmlHelpers.ConvertToUnSign(null, model.Url ?? model.Name);
                    _unitOfWork.CareerRepository.Insert(model);
                    _unitOfWork.Save();

                    var count = _unitOfWork.CareerRepository.GetQuery(a => a.Url == model.Url).Count();
                    if (count > 1)
                    {
                        model.Url += "-" + model.Id;
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
                    model.Url = HtmlHelpers.ConvertToUnSign(null, model.Url ?? model.Name);
                    _unitOfWork.CareerRepository.Update(model);
                    _unitOfWork.Save();
                    var count = _unitOfWork.CareerRepository.GetQuery(a => a.Url == model.Url).Count();
                    if (count > 1)
                    {
                        model.Url += "-" + model.Id;
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
        public bool QuickUpdateCareer(bool active = false, bool home = false, int careerId = 0)
        {
            var career = _unitOfWork.CareerRepository.GetById(careerId);
            if (career == null)
            {
                return false;
            }
            career.ShowHome = home;
            career.Active = active;
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
        public ActionResult ListSkill(int? page, string name, string result = "")
        {
            ViewBag.Result = result;
            var pageNumber = page ?? 1;
            var pageSize = 15;

            var skills = _unitOfWork.SkillRepository.Get(orderBy: o => o.OrderBy(a => a.SkillName));
            if (!string.IsNullOrEmpty(name))
            {
                skills = skills.Where(a => a.SkillName.ToLower().Contains(name.ToLower()));
            }
            var model = new ListSkillViewModel
            {
                Skills = skills.ToPagedList(pageNumber, pageSize),
                Name = name
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
                _unitOfWork.SkillRepository.Insert(model);
                _unitOfWork.Save();
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
                _unitOfWork.SkillRepository.Update(model);
                _unitOfWork.Save();
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
        public ActionResult  ListCountruy(int? page , string name , string result ="")
        {
            ViewBag.Result = result;
            var pageNumber  = page ?? 1;
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
            if(ModelState.IsValid)
            {
                _unitOfWork.CountryRepository.Insert(country);
                _unitOfWork.Save();
                return RedirectToAction("ListCountruy", new { result  = "success" } );
            }
            return View(country);
        }
        public ActionResult EditCountruy(int id = 0)
        {
            var countruy = _unitOfWork.CountryRepository.GetById(id);
            if(countruy == null)
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
            _unitOfWork.Save();
            return RedirectToAction("ListCountruy", new { result = "update" });
        }
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
        public bool UpdateCountruy(int id , bool active, int sort)
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
            var cities = _unitOfWork.CityRepository.Get(orderBy: q => q.OrderBy(c => c.CounId));
            return PartialView("ListCity", cities);
        }
        public ActionResult City()
        {
            var model = new InsertCityViewModel
            {
                City = new City { Sort = 1, Active = true },
                Countries = _unitOfWork.CountryRepository.GetQuery(a => a.Active , o => o.OrderBy(c => c.Sort)),
            };
            return View(model);
        }
        [HttpPost]
        public ActionResult City(InsertCityViewModel model , FormCollection fc)
        {
            if (ModelState.IsValid)
            {
                model.City.CounId = Convert.ToInt32(fc["CountruyId"]);
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
        public ActionResult EditCity(InsertCityViewModel model , FormCollection fc)
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
                city.CounId = Convert.ToInt32(fc["CountruyId"]);
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
        #endregion
        protected override void Dispose(bool disposing)
        {
            _unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}