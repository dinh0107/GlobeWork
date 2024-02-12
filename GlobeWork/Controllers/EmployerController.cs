using Helpers;
using GlobeWork.DAL;
using GlobeWork.Filters;
using GlobeWork.Models;
using GlobeWork.ViewModel;
using System;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using PagedList;
using System.Data.Entity;

namespace GlobeWork.Controllers
{
    [EmployerFilter, RoutePrefix("app")]
    public class EmployerController : Controller
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();
        private int Id => Convert.ToInt32(RouteData.Values["Id"]);
        private new Employer User => _unitOfWork.EmployerRepository.GetById(Id);
        public SelectList CitySelectList => new SelectList(_unitOfWork.CityRepository.Get(a => a.Active, q => q.OrderBy(a => a.Sort)), "Id", "Name");
        public SelectList DistrictSelectList(int? cityId) => new SelectList(_unitOfWork.DistrictRepository.Get(a => a.Active && a.CityId == cityId, q => q.OrderBy(a => a.Sort)), "Id", "Name");
        public ConfigSite ConfigSite => (ConfigSite)HttpContext.Application["ConfigSite"];
        private IEnumerable<StudyAbroadCategory> StudyAbroadCategories() => _unitOfWork.StudyAbroadCategoryRepository.GetQuery(a => a.Active , o => o.OrderBy(a => a.Sort));

        #region Login 
        [OverrideActionFilters, Route("nha-tuyen-dung")]
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
                        Avatar = null,
                    };
                    _unitOfWork.EmployerRepository.Insert(user);
                    _unitOfWork.Save();
                    var userData = user.Avatar + "|" + user.Id + "|" + user.Email + "|" + user.FullName + "|" + user.Amount;
                    var ticket = new FormsAuthenticationTicket(2, model.Email.ToLower(), DateTime.Now, DateTime.Now.AddDays(30), true,
                        userData, FormsAuthentication.FormsCookiePath);
                    var encTicket = FormsAuthentication.Encrypt(ticket);
                    Response.Cookies.Add(new HttpCookie(".ASPXAUTHEMPLOYER", encTicket));
                    return RedirectToAction("Index", "Employer");
                }
            }
            model.Ranks = _unitOfWork.RankRepository.Get();
            return View(model);
        }
        [OverrideActionFilters, Route("dang-nhap/nha-tuyen-dung")]
        public ActionResult EmployerLogin()
        {
            var cookie = Request.Cookies[".ASPXAUTHEMPLOYER"];
            if (cookie != null)
            {
                return RedirectToAction("Index");
            }
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
                var userData = user.Avatar + "|" + user.Id + "|" + user.Email + "|" + user.FullName + "|" + user.Amount;
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
        #endregion

        #region Notify

        public PartialViewResult Notify()
        {
            var log = _unitOfWork.EmployerLogRepository.GetQuery(a => a.UserId == User.Id, o => o.OrderByDescending(a => a.CreateDate));
            return PartialView(log);
        }
        #endregion

        #region  Infomation
        [Route("cap-nhat-thong-tin")]
        public PartialViewResult Personal(string result = "")
        {
            ViewBag.Result = result;
            var model = new ChangeInfoViewModel
            {
                Employer = User,
                Ranks = _unitOfWork.RankRepository.GetQuery(orderBy: a => a.OrderBy(l => l.Name))
            };
            return PartialView(model);
        }
        [Route("cap-nhat-thong-tin")]
        [HttpPost]
        public ActionResult Personal(ChangeInfoViewModel model)
        {
            if (ModelState.IsValid)
            {
                var employ = _unitOfWork.EmployerRepository.GetById(model.Employer.Id);
                if (employ == null)
                {
                    return RedirectToAction("Index");
                }
                var isPost = true;
                var file = Request.Files["Employer.Avatar"];
                if (file != null && file.ContentLength > 0)
                {
                    if (file.ContentType != "image/jpeg" & file.ContentType != "image/png" && file.ContentType != "image/gif")
                    {
                        isPost = false;
                        ModelState.AddModelError("", @"Chỉ chấp nhận định dạng jpg, png, gif, jpeg");
                    }
                    else
                    {
                        if (file.ContentLength > 4000 * 1024)
                        {
                            isPost = false;
                            ModelState.AddModelError("", @"Dung lượng lớn hơn 4MB. Hãy thử lại");
                        }
                        else
                        {
                            var imgPath = "/images/employer/" + DateTime.Now.ToString("yyyy/MM/dd");
                            HtmlHelpers.CreateFolder(Server.MapPath(imgPath));
                            var imgFileName = DateTime.Now.ToFileTimeUtc() + Path.GetExtension(file.FileName);

                            model.Employer.Avatar = DateTime.Now.ToString("yyyy/MM/dd") + "/" + imgFileName;

                            var newImage = System.Drawing.Image.FromStream(file.InputStream);
                            var fixSizeImage = HtmlHelpers.FixedSize(newImage, 600, 600, false);
                            HtmlHelpers.SaveJpeg(Server.MapPath(Path.Combine(imgPath, imgFileName)), fixSizeImage, 90);
                            file.SaveAs(Server.MapPath(Path.Combine(imgPath, imgFileName)));
                            employ.Avatar = model.Employer.Avatar;
                        }
                    }
                }
                if (isPost)
                {
                    employ.FullName = model.Employer.FullName;
                    employ.PhoneNumber = model.Employer.PhoneNumber;
                    employ.Email = model.Employer.Email;
                    employ.Gender = model.Employer.Gender;
                    employ.RankId = model.Employer.RankId;
                    _unitOfWork.Save();
                    var userData = employ.Avatar + "|" + employ.Id + "|" + employ.Email + "|" + employ.FullName;
                    var ticket = new FormsAuthenticationTicket(2, employ.Email.ToLower(), DateTime.Now, DateTime.Now.AddDays(30), true,
                        userData, FormsAuthentication.FormsCookiePath);
                    var encTicket = FormsAuthentication.Encrypt(ticket);
                    Response.Cookies.Add(new HttpCookie(".ASPXAUTHEMPLOYER", encTicket));
                    return RedirectToAction("Personal", "Employer", new { result = "success" });
                }
            }
            model.Ranks = _unitOfWork.RankRepository.GetQuery(orderBy: a => a.OrderBy(l => l.Name));
            return View(model);
        }

        [Route("doi-mat-khau")]
        public ActionResult ChangePassword(string result = "")
        {
            ViewBag.Result = result;
            return View();
        }
        [Route("doi-mat-khau"), HttpPost, ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (!HtmlHelpers.VerifyHash(model.OldPassword, "SHA256", User.Password))
                {
                    ModelState.AddModelError("", @"Mật khẩu hiện tại không chính xác. Vui lòng thử lại");
                    return View(model);
                }
                User.Password = HtmlHelpers.ComputeHash(model.Password, "SHA256", null);
                Utils.Utils.EmployerLog("Đổi mật khẩu", EmployerLogType.ChangePassword, User.Id, 0);
                _unitOfWork.Save();
                return RedirectToAction("ChangePassword", new { result = "success" });
            }
            return View(model);
        }
        [Route("cong-ty")]
        public ActionResult Company(string result = "")
        {
            ViewBag.Result = result;
            var company = _unitOfWork.CompanyRepository.Get(a => a.UserId == User.Id).FirstOrDefault();
            var model = new InserCompanyEmployerViewModel
            {
                Ranks = _unitOfWork.RankRepository.Get(orderBy: o => o.OrderBy(a => a.Name)),
                Careers = _unitOfWork.CareerRepository.Get(orderBy: o => o.OrderBy(a => a.Name)),
                Company = _unitOfWork.CompanyRepository.Get(a => a.UserId == User.Id).FirstOrDefault(),
                Cities = _unitOfWork.CityRepository.Get(a => a.Active, q => q.OrderBy(a => a.Sort)),
            };
            return View(model);
        }
        [Route("cong-ty")]
        [HttpPost, ValidateInput(false)]
        public ActionResult Company(InserCompanyEmployerViewModel model, FormCollection fc)
        {
            var company = _unitOfWork.CompanyRepository.GetQuery(a => a.UserId == User.Id).FirstOrDefault();
            if (company == null)
            {
                if (ModelState.IsValid)
                {
                    var isPost = true;
                    var avatar = Request.Files["Company.Avatar"];
                    var cover = Request.Files["Company.Cover"];
                    if (avatar != null && avatar.ContentLength > 0)
                    {
                        if (avatar.ContentType != "image/jpeg" & avatar.ContentType != "image/png" && avatar.ContentType != "image/gif")
                        {
                            isPost = false;
                            ModelState.AddModelError("", @"Chỉ chấp nhận định dạng jpg, png, gif, jpeg");
                        }
                        else
                        {
                            if (avatar.ContentLength > 4000 * 1024)
                            {
                                isPost = false;
                                ModelState.AddModelError("", @"Dung lượng lớn hơn 4MB. Hãy thử lại");
                            }
                            else
                            {
                                var imgPath = "/images/company/" + DateTime.Now.ToString("yyyy/MM/dd");
                                HtmlHelpers.CreateFolder(Server.MapPath(imgPath));
                                var imgFileName = DateTime.Now.ToFileTimeUtc() + Path.GetExtension(avatar.FileName);
                                model.Company.Avatar = DateTime.Now.ToString("yyyy/MM/dd") + "/" + imgFileName;
                                var newImage = System.Drawing.Image.FromStream(avatar.InputStream);
                                var fixSizeImage = HtmlHelpers.FixedSize(newImage, 600, 600, false);
                                HtmlHelpers.SaveJpeg(Server.MapPath(Path.Combine(imgPath, imgFileName)), fixSizeImage, 90);
                                avatar.SaveAs(Server.MapPath(Path.Combine(imgPath, imgFileName)));
                            }
                        }
                    }
                    if (cover != null && cover.ContentLength > 0)
                    {
                        if (cover.ContentType != "image/jpeg" & cover.ContentType != "image/png" && cover.ContentType != "image/gif")
                        {
                            isPost = false;
                            ModelState.AddModelError("", @"Chỉ chấp nhận định dạng jpg, png, gif, jpeg");
                        }
                        else
                        {
                            if (cover.ContentLength > 4000 * 1024)
                            {
                                isPost = false;
                                ModelState.AddModelError("", @"Dung lượng lớn hơn 4MB. Hãy thử lại");
                            }
                            else
                            {
                                var imgPath = "/images/company/" + DateTime.Now.ToString("yyyy/MM/dd");
                                HtmlHelpers.CreateFolder(Server.MapPath(imgPath));
                                var imgFileName = DateTime.Now.ToFileTimeUtc() + Path.GetExtension(cover.FileName);
                                model.Company.Cover = DateTime.Now.ToString("yyyy/MM/dd") + "/" + imgFileName;
                                var newImage = System.Drawing.Image.FromStream(cover.InputStream);
                                var fixSizeImage = HtmlHelpers.FixedSize(newImage, 1200, 1200, false);
                                HtmlHelpers.SaveJpeg(Server.MapPath(Path.Combine(imgPath, imgFileName)), fixSizeImage, 90);
                                cover.SaveAs(Server.MapPath(Path.Combine(imgPath, imgFileName)));
                            }
                        }
                    }
                    if (isPost)
                    {
                        model.Company.UserId = User.Id;
                        model.Company.Url = HtmlHelpers.ConvertToUnSign(null, model.Company.Url ?? model.Company.Name);
                        model.Company.CityId = Convert.ToInt32(fc["city"]);
                        _unitOfWork.CompanyRepository.Insert(model.Company);
                        _unitOfWork.Save();
                        var careers = fc["career"];
                        if (!string.IsNullOrEmpty(careers))
                        {
                            var listCareer = careers.Split((',')).Select(int.Parse).ToList();
                            if (model.Company.Careers == null)
                            {
                                model.Company.Careers = new List<Career>();
                            }
                            foreach (var item in listCareer)
                            {
                                var careerItem = _unitOfWork.CareerRepository.GetById(item);
                                model.Company.Careers.Add(careerItem);
                            }
                            _unitOfWork.Save();
                        }
                        var urlCount = _unitOfWork.CompanyRepository.GetQuery(a => a.Url == model.Company.Url ).Count();
                        if(urlCount > 1)
                        {
                            model.Company.Url += "-" + model.Company.UserId;
                            _unitOfWork.Save();
                        }
                        return RedirectToAction("Company", new { result = "success" });
                    }

                }
            }
            else
            {
                var avatar = Request.Files["Company.Avatar"];
                var cover = Request.Files["Company.Cover"];
                if (avatar != null && avatar.ContentLength > 0)
                {
                    if (avatar.ContentType != "image/jpeg" & avatar.ContentType != "image/png" && avatar.ContentType != "image/gif")
                    {
                        ModelState.AddModelError("", @"Chỉ chấp nhận định dạng jpg, png, gif, jpeg");
                        model.Careers = _unitOfWork.CareerRepository.Get(orderBy: o => o.OrderBy(a => a.Name));
                        return View(model);
                    }
                    else
                    {
                        if (avatar.ContentLength > 4000 * 1024)
                        {
                            ModelState.AddModelError("", @"Dung lượng lớn hơn 4MB. Hãy thử lại");
                            model.Careers = _unitOfWork.CareerRepository.Get(orderBy: o => o.OrderBy(a => a.Name));
                            return View(model);
                        }
                        else
                        {
                            var imgPath = "/images/company/" + DateTime.Now.ToString("yyyy/MM/dd");
                            HtmlHelpers.CreateFolder(Server.MapPath(imgPath));
                            var imgFileName = DateTime.Now.ToFileTimeUtc() + Path.GetExtension(avatar.FileName);
                            company.Avatar = DateTime.Now.ToString("yyyy/MM/dd") + "/" + imgFileName;
                            var newImage = System.Drawing.Image.FromStream(avatar.InputStream);
                            var fixSizeImage = HtmlHelpers.FixedSize(newImage, 600, 600, false);
                            HtmlHelpers.SaveJpeg(Server.MapPath(Path.Combine(imgPath, imgFileName)), fixSizeImage, 90);
                            avatar.SaveAs(Server.MapPath(Path.Combine(imgPath, imgFileName)));
                        }
                    }
                }
                if (cover != null && cover.ContentLength > 0)
                {
                    if (cover.ContentType != "image/jpeg" & cover.ContentType != "image/png" && cover.ContentType != "image/gif")
                    {
                        ModelState.AddModelError("", @"Chỉ chấp nhận định dạng jpg, png, gif, jpeg");
                        model.Careers = _unitOfWork.CareerRepository.Get(orderBy: o => o.OrderBy(a => a.Name));
                        return View(model);
                    }
                    else
                    {
                        if (cover.ContentLength > 4000 * 1024)
                        {
                            ModelState.AddModelError("", @"Dung lượng lớn hơn 4MB. Hãy thử lại");
                            model.Careers = _unitOfWork.CareerRepository.Get(orderBy: o => o.OrderBy(a => a.Name));
                            return View(model);
                        }
                        else
                        {
                            var imgPath = "/images/company/" + DateTime.Now.ToString("yyyy/MM/dd");
                            HtmlHelpers.CreateFolder(Server.MapPath(imgPath));
                            var imgFileName = DateTime.Now.ToFileTimeUtc() + Path.GetExtension(cover.FileName);
                            company.Cover = DateTime.Now.ToString("yyyy/MM/dd") + "/" + imgFileName;
                            var newImage = System.Drawing.Image.FromStream(cover.InputStream);
                            var fixSizeImage = HtmlHelpers.FixedSize(newImage, 1200, 1200, false);
                            HtmlHelpers.SaveJpeg(Server.MapPath(Path.Combine(imgPath, imgFileName)), fixSizeImage, 90);
                            cover.SaveAs(Server.MapPath(Path.Combine(imgPath, imgFileName)));
                        }
                    }
                }
                company.Name = model.Company.Name;
                company.WebsiteUrl = model.Company.WebsiteUrl;
                company.Address = model.Company.Address;
                company.CompanySize = model.Company.CompanySize;
                company.EstablishmentDate = model.Company.EstablishmentDate;
                company.Phone = model.Company.Phone;
                company.Introduct = model.Company.Introduct;
                company.Product = model.Company.Product;
                company.GoogleMap = model.Company.GoogleMap;
                company.VideoYoutube = model.Company.VideoYoutube;
                company.CityId = Convert.ToInt32(fc["city"]);
                company.Url = HtmlHelpers.ConvertToUnSign(null, company.Url ?? company.Name);
                _unitOfWork.CompanyRepository.Update(company);
                _unitOfWork.Save();
                company.Careers.Clear();
                var careers = fc["career"];
                if (!string.IsNullOrEmpty(careers))
                {
                    var listCareer = careers.Split((',')).Select(int.Parse).ToList();
                    foreach (var item in listCareer)
                    {
                        var careerItem = _unitOfWork.CareerRepository.GetById(item);
                        company.Careers.Add(careerItem);
                    }
                    _unitOfWork.Save();
                }
                var urlCount = _unitOfWork.CompanyRepository.GetQuery(a => a.Url == company.Url).Count();
                if (urlCount > 1)
                {
                    company.Url += "-" + company.UserId;
                    _unitOfWork.Save();
                }
                return RedirectToAction("Company", new { result = "update" });
            }
            model.Careers = _unitOfWork.CareerRepository.Get(orderBy: o => o.OrderBy(a => a.Name));
            return View(model);
        }
        #endregion

        [ChildActionOnly]
        public PartialViewResult Header()
        {
            return PartialView();
        }
        [ChildActionOnly]
        public PartialViewResult Sidebar()
        {
            return PartialView();
        }
        
        [Route("dashboad")]
        public ActionResult Index(string result = "")
        {
            ViewBag.Result = result;
            var log = _unitOfWork.EmployerLogRepository.GetQuery(a => a.UserId == User.Id && a.EmployerLogType == EmployerLogType.Deduction);
            decimal totalAmount = 0; 
            if (log != null)
            {
                foreach (var item in log)
                {
                    totalAmount += item.Amount;
                }
            }
            var model = new EmployerViewModel
            {
                Employer = User,
                Amount = totalAmount,
                JobPost = _unitOfWork.JobPostRepository.GetQuery(a => a.CompanyId == User.Id).Count(),

            };
            return View(model);
        }

        public PartialViewResult NavProfile()
        {
            return PartialView();
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

        #region Job
        [Route("danh-sach-tin-tuyen-dung")]
        public ActionResult ListNews(int? page, string name, int type = 0)
        {
            var pageNumber = page ?? 1;
            var job = _unitOfWork.JobPostRepository.GetQuery(a => a.CompanyId == User.Id, o => o.OrderByDescending(a => a.CreateDate));
            if (!string.IsNullOrEmpty(name))
            {
                job = job.Where(a => a.Name.ToLower().Contains(name.ToLower()) || a.Code.ToLower().Contains(name.ToLower()));
            }
            switch (type)
            {
                case 1:
                    job = job.Where(a => a.Active);
                    break;
                case 2:
                    job = job.Where(a => !a.Active);
                    break;
                case 3:
                    job = job.Where(a => a.Hot != null);
                    break;
            }
            var model = new ListMyJobPost
            {
                JobPosts = job.ToPagedList(pageNumber, 12),
                Name = name,
                Type = type,
            };
            return View(model);
        }

        [Route("dang-tin-tuyen-dung")]
        public ActionResult PostNews()
        {
            var model = new InsertEmployerViewModel
            {
                Careers = _unitOfWork.CareerRepository.GetQuery(a => a.Active, o => o.OrderByDescending(a => a.CreateDate)),
                JobTypes = _unitOfWork.JobTypeRepository.GetQuery(orderBy: o => o.OrderByDescending(a => a.CreateDate)),
                Ranks = _unitOfWork.RankRepository.GetQuery(orderBy: o => o.OrderByDescending(a => a.CreateDate)),
                Citys = _unitOfWork.CityRepository.Get(a => a.Active, q => q.OrderBy(a => a.Sort)),
                Skills = _unitOfWork.SkillRepository.Get(orderBy: a => a.OrderBy(l => l.Id)),
                JobPost = new JobPost
                {
                    Active = true,
                    Quantity = 1
                },
                DateHot = 0
            };
            return View(model);
        }
        [Route("dang-tin-tuyen-dung")]
        [HttpPost, ValidateInput(false)]
        public ActionResult PostNews(InsertEmployerViewModel model, FormCollection fc)
        {
            if (ModelState.IsValid)
            {
                model.JobPost.Url = HtmlHelpers.ConvertToUnSign(null, model.JobPost.Name);
                model.JobPost.RankId = Convert.ToInt32(fc["RankId"]);
                model.JobPost.CareerId = Convert.ToInt32(fc["CareerId"]);
                model.JobPost.JobTypeId = Convert.ToInt32(fc["JobTypeId"]);
                //model.JobPost.CityId = Convert.ToInt32(fc["CityId"]);
                model.JobPost.Image = fc["Pictures"];
                var company = _unitOfWork.CompanyRepository.GetQuery(a => a.UserId == User.Id).FirstOrDefault();
                if(company == null)
                {
                    return RedirectToAction("Company");
                }
                model.JobPost.CompanyId = company.UserId;
                model.JobPost.Code = Utils.Utils.GenerateRandomCode();
                if (model.DateHot > 0)
                {
                    if (User.Amount != 0)
                    {
                        int amountToSubtract = 1000 * model.DateHot;
                        string formattedAmountToSubtract = amountToSubtract.ToString("#,0") + "đ";
                        if (amountToSubtract < User.Amount)
                        {
                            User.Amount -= amountToSubtract;
                            Utils.Utils.EmployerLog("Tài khoản bị trừ <strong>" + formattedAmountToSubtract + "</strong> để hiển thị tin <strong>" + model.JobPost.Code + " </strong>" + "<strong class='text-danger'>" + model.DateHot + "</strong> ngày ở mục nổi bật", EmployerLogType.Deduction, User.Id, amountToSubtract);
                            model.JobPost.Hot = DateTime.Now.AddDays(model.DateHot);
                            //Update cookie
                            HttpCookie cookie = Request.Cookies[".ASPXAUTHEMPLOYER"];
                            if (cookie != null && !string.IsNullOrEmpty(cookie.Value))
                            {
                                var userData = User.Avatar + "|" + User.Id + "|" + User.Email + "|" + User.FullName + "|" + User.Amount;
                                var ticket = new FormsAuthenticationTicket(2, User.Email.ToLower(), DateTime.Now, DateTime.Now.AddDays(30), true,
                                    userData, FormsAuthentication.FormsCookiePath);
                                var encTicket = FormsAuthentication.Encrypt(ticket);
                                HttpCookie updatedCookie = new HttpCookie(".ASPXAUTHEMPLOYER");
                                updatedCookie.Value = encTicket;
                                updatedCookie.Expires = DateTime.Now.AddDays(30);
                                Response.SetCookie(updatedCookie);
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", @"Số tiền trong tài khoản của bạn không đủ vui lòng nạp tiền để tiếp tục sử dụng");
                            model.Careers = _unitOfWork.CareerRepository.GetQuery(a => a.Active, o => o.OrderByDescending(a => a.CreateDate));
                            model.JobTypes = _unitOfWork.JobTypeRepository.GetQuery(orderBy: o => o.OrderByDescending(a => a.CreateDate));
                            model.Ranks = _unitOfWork.RankRepository.GetQuery(orderBy: o => o.OrderByDescending(a => a.CreateDate));
                            model.Citys = _unitOfWork.CityRepository.Get(a => a.Active, q => q.OrderBy(a => a.Sort));
                            model.Skills = _unitOfWork.SkillRepository.Get(orderBy: a => a.OrderBy(l => l.Id));
                            return View(model);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", @"Số tiền trong tài khoản của bạn không đủ vui lòng nạp tiền để tiếp tục sử dụng");
                        model.Careers = _unitOfWork.CareerRepository.GetQuery(a => a.Active, o => o.OrderByDescending(a => a.CreateDate));
                        model.JobTypes = _unitOfWork.JobTypeRepository.GetQuery(orderBy: o => o.OrderByDescending(a => a.CreateDate));
                        model.Ranks = _unitOfWork.RankRepository.GetQuery(orderBy: o => o.OrderByDescending(a => a.CreateDate));
                        model.Skills = _unitOfWork.SkillRepository.Get(orderBy: a => a.OrderBy(l => l.Id));
                        model.Citys = _unitOfWork.CityRepository.Get(a => a.Active, q => q.OrderBy(a => a.Sort));
                        return View(model);
                    }
                }

                _unitOfWork.JobPostRepository.Insert(model.JobPost);
                _unitOfWork.Save();
                var cities = fc["city"];
                if (cities != null)
                {
                    if (model.JobPost.Cities == null)
                    {
                        model.JobPost.Cities = new List<City>();
                    }
                    foreach (var cId in cities.Split(','))
                    {
                        var city = _unitOfWork.CityRepository.GetById(Convert.ToInt32(cId));
                        model.JobPost.Cities.Add(city);
                    }
                    _unitOfWork.Save();
                }
                var skill = fc["skill"];
                if (skill != null)
                {
                    if (model.JobPost.Skills == null)
                    {
                        model.JobPost.Skills = new List<Skill>();
                    }
                    foreach (var skills in skill.Split(','))
                    {
                        var skil = _unitOfWork.SkillRepository.GetById(Convert.ToInt32(skills));
                        model.JobPost.Skills.Add(skil);
                    }
                    _unitOfWork.Save();
                }
                var jobs = _unitOfWork.JobPostRepository.GetQuery().AsNoTracking();
                if (jobs.Any(p => p.Url.ToLower().Trim() == model.JobPost.Url.ToLower().Trim()))
                {
                    model.JobPost.Url += "-" + model.JobPost.Id;
                    _unitOfWork.Save();
                }
                return RedirectToAction("ListNews");
            }
            model.Careers = _unitOfWork.CareerRepository.GetQuery(a => a.Active, o => o.OrderByDescending(a => a.CreateDate));
            model.JobTypes = _unitOfWork.JobTypeRepository.GetQuery(orderBy: o => o.OrderByDescending(a => a.CreateDate));
            model.Ranks = _unitOfWork.RankRepository.GetQuery(orderBy: o => o.OrderByDescending(a => a.CreateDate));
            model.Citys = _unitOfWork.CityRepository.Get(a => a.Active, q => q.OrderBy(a => a.Sort));
            model.Skills = _unitOfWork.SkillRepository.Get(orderBy: a => a.OrderBy(l => l.Id));

            return View(model);
        }

        [Route("cap-nhat-tin-tuyen-dung")]
        public ActionResult EditJob(int id)
        {
            var jobs = _unitOfWork.JobPostRepository.GetById(id);
            if (jobs == null)
            {
                return RedirectToAction("PostNews");
            }
            var model = new InsertEmployerViewModel
            {
                JobPost = jobs,
                DateHot = 0,
                Careers = _unitOfWork.CareerRepository.GetQuery(a => a.Active, o => o.OrderByDescending(a => a.CreateDate)),
                JobTypes = _unitOfWork.JobTypeRepository.GetQuery(orderBy: o => o.OrderByDescending(a => a.CreateDate)),
                Ranks = _unitOfWork.RankRepository.GetQuery(orderBy: o => o.OrderByDescending(a => a.CreateDate)),
                Citys = _unitOfWork.CityRepository.Get(a => a.Active, q => q.OrderBy(a => a.Sort)),
                Skills = _unitOfWork.SkillRepository.Get(orderBy: a => a.OrderBy(l => l.Id)),
            };
            return View(model);
        }
        [Route("cap-nhat-tin-tuyen-dung")]
        [HttpPost, ValidateInput(false)]
        public ActionResult EditJob(InsertEmployerViewModel model, FormCollection fc)
        {
            var jobs = _unitOfWork.JobPostRepository.GetById(model.JobPost.Id);
            if (jobs == null)
            {
                RedirectToAction("PostNews");
            }
            if (ModelState.IsValid)
            {

                jobs.Url = HtmlHelpers.ConvertToUnSign(null, jobs.Name);
                jobs.RankId = Convert.ToInt32(fc["RankId"]);
                jobs.CareerId = Convert.ToInt32(fc["CareerId"]);
                jobs.JobTypeId = Convert.ToInt32(fc["JobTypeId"]);
                //jobs.CityId = Convert.ToInt32(fc["CityId"]);
                jobs.Image = fc["Pictures"] == "" ? null : fc["Pictures"];
                jobs.CompanyId = User.Id;
                jobs.Name = model.JobPost.Name;
                jobs.ExpirationDate = model.JobPost.ExpirationDate;
                jobs.Quantity = model.JobPost.Quantity;
                jobs.Gender = model.JobPost.Gender;
                jobs.Experiences = model.JobPost.Experiences;
                jobs.Address = model.JobPost.Address;
                jobs.Body = model.JobPost.Body;
                jobs.Requirement = model.JobPost.Requirement;
                jobs.Benefits = model.JobPost.Benefits;
                jobs.WorkLocation = model.JobPost.WorkLocation;
                jobs.LastEditDate = DateTime.Now;
                jobs.Active = model.JobPost.Active;
                jobs.SalalyMin = model.JobPost.SalalyMin;
                jobs.SalalyMax = model.JobPost.SalalyMax;
                jobs.Wages = model.JobPost.Wages;

                if (model.DateHot > 0)
                {
                    if (User.Amount != 0)
                    {
                        int amountToSubtract = 1000 * model.DateHot;
                        string formattedAmountToSubtract = amountToSubtract.ToString("#,0") + "đ";
                        if (amountToSubtract < User.Amount)
                        {
                            User.Amount -= amountToSubtract;
                            Utils.Utils.EmployerLog("Tài khoản bị trừ <strong>" + formattedAmountToSubtract + "</strong> để hiển thị tin <strong>" + jobs.Code + " </strong>" + "<strong class='text-danger'>" + model.DateHot + "</strong> ngày ở mục nổi bật", EmployerLogType.Deduction, User.Id, amountToSubtract);
                            if (jobs.Hot != null)
                            {
                                jobs.Hot = jobs.Hot?.AddDays(model.DateHot);
                            }
                            else
                            {
                                jobs.Hot = DateTime.Now.AddDays(model.DateHot);
                            }
                            //Update cookie
                            HttpCookie cookie = Request.Cookies[".ASPXAUTHEMPLOYER"];
                            if (cookie != null && !string.IsNullOrEmpty(cookie.Value))
                            {
                                var userData = User.Avatar + "|" + User.Id + "|" + User.Email + "|" + User.FullName + "|" + User.Amount;
                                var ticket = new FormsAuthenticationTicket(2, User.Email.ToLower(), DateTime.Now, DateTime.Now.AddDays(30), true,
                                    userData, FormsAuthentication.FormsCookiePath);
                                var encTicket = FormsAuthentication.Encrypt(ticket);
                                HttpCookie updatedCookie = new HttpCookie(".ASPXAUTHEMPLOYER");
                                updatedCookie.Value = encTicket;
                                updatedCookie.Expires = DateTime.Now.AddDays(30);
                                Response.SetCookie(updatedCookie);
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", @"Số tiền trong tài khoản của bạn không đủ vui lòng nạp tiền để tiếp tục sử dụng");
                            model.Careers = _unitOfWork.CareerRepository.GetQuery(a => a.Active, o => o.OrderByDescending(a => a.CreateDate));
                            model.JobTypes = _unitOfWork.JobTypeRepository.GetQuery(orderBy: o => o.OrderByDescending(a => a.CreateDate));
                            model.Ranks = _unitOfWork.RankRepository.GetQuery(orderBy: o => o.OrderByDescending(a => a.CreateDate));
                            model.Skills = _unitOfWork.SkillRepository.Get(orderBy: a => a.OrderBy(l => l.Id));
                            model.Citys = _unitOfWork.CityRepository.Get(a => a.Active, q => q.OrderBy(a => a.Sort));
                            return View(model);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", @"Số tiền trong tài khoản của bạn không đủ vui lòng nạp tiền để tiếp tục sử dụng");
                        model.Careers = _unitOfWork.CareerRepository.GetQuery(a => a.Active, o => o.OrderByDescending(a => a.CreateDate));
                        model.JobTypes = _unitOfWork.JobTypeRepository.GetQuery(orderBy: o => o.OrderByDescending(a => a.CreateDate));
                        model.Ranks = _unitOfWork.RankRepository.GetQuery(orderBy: o => o.OrderByDescending(a => a.CreateDate));
                        model.Skills = _unitOfWork.SkillRepository.Get(orderBy: a => a.OrderBy(l => l.Id));
                        model.Citys = _unitOfWork.CityRepository.Get(a => a.Active, q => q.OrderBy(a => a.Sort));
                        return View(model);
                    }
                }
                _unitOfWork.Save();
                jobs.Cities.Clear();
                var cities = fc["city"];
                if (cities != "")
                {
                    if (jobs.Cities == null)
                    {
                        jobs.Cities = new List<City>();
                    }
                    foreach (var cId in cities.Split(','))
                    {
                        var city = _unitOfWork.CityRepository.GetById(Convert.ToInt32(cId));
                        jobs.Cities.Add(city);
                    }
                    _unitOfWork.Save();
                }
                jobs.Skills.Clear();
                var skill = fc["skill"];
                if (skill != null)
                {
                    if (jobs.Skills == null)
                    {
                        jobs.Skills = new List<Skill>();
                    }
                    foreach (var skills in skill.Split(','))
                    {
                        var skil = _unitOfWork.SkillRepository.GetById(Convert.ToInt32(skills));
                        jobs.Skills.Add(skil);
                    }
                    _unitOfWork.Save();
                }
                var job = _unitOfWork.JobPostRepository.GetQuery().AsNoTracking();
                if (job.Any(p => p.Url.ToLower().Trim() == jobs.Url.ToLower().Trim()))
                {
                    jobs.Url += "-" + jobs.Id;
                    _unitOfWork.Save();
                }
                return RedirectToAction("ListNews");
            }
            model.Careers = _unitOfWork.CareerRepository.GetQuery(a => a.Active, o => o.OrderByDescending(a => a.CreateDate));
            model.JobTypes = _unitOfWork.JobTypeRepository.GetQuery(orderBy: o => o.OrderByDescending(a => a.CreateDate));
            model.Ranks = _unitOfWork.RankRepository.GetQuery(orderBy: o => o.OrderByDescending(a => a.CreateDate));
            model.Citys = _unitOfWork.CityRepository.Get(a => a.Active, q => q.OrderBy(a => a.Sort));
            model.Skills = _unitOfWork.SkillRepository.Get(orderBy: a => a.OrderBy(l => l.Id));
            return View(model);
        }
        [HttpPost]
        public bool UpdateStatusJob(int id, int type = 0)
        {
            var job = _unitOfWork.JobPostRepository.GetById(id);
            if (job == null)
            {
                return false;
            }
            if (type == 1)
            {
                job.Active = false;
            }
            else if (type == 2)
            {
                job.Active = true;
            }
            job.LastEditDate = DateTime.Now;
            _unitOfWork.Save();
            return true;
        }
        [HttpPost]
        public bool DeleteJob(int id)
        {
            var job = _unitOfWork.JobPostRepository.GetById(id);
            if (job == null)
            {
                return false;
            }
            _unitOfWork.JobPostRepository.Delete(job);
            _unitOfWork.Save();
            return true;
        }
        #endregion

        #region studyAbroad
        [Route("danh-sach-tin-du-hoc")]
        public ActionResult ListStudyAbroad(int? page, string name, int type = 0)
        {
            var pageNumber = page ?? 1;
            var studyAbroad = _unitOfWork.StudyAbroadRepository.GetQuery(a => a.CompanyId == User.Id, o => o.OrderByDescending(a => a.CreateDate));
            if (!string.IsNullOrEmpty(name))
            {
                studyAbroad = studyAbroad.Where(a => a.Name.ToLower().Contains(name.ToLower()) || a.Code.ToLower().Contains(name.ToLower()));
            }
            switch (type)
            {
                case 1:
                    studyAbroad = studyAbroad.Where(a => a.Active);
                    break;
                case 2:
                    studyAbroad = studyAbroad.Where(a => !a.Active);
                    break;
                case 3:
                    studyAbroad = studyAbroad.Where(a => a.Hot != null);
                    break;
            }
            var model = new ListMyJobStudyAbroadViewModel
            {
                StudyAbroads = studyAbroad.ToPagedList(pageNumber, 12),
                Name = name,
                Type = type,
            };
            return View(model);
        }
        [Route("tin-du-hoc")]
        public ActionResult StudyAbroad()
        {
            var model = new InsertStudyAbroadViewModel
            {
                Careers = _unitOfWork.CareerRepository.GetQuery(a => a.Active, o => o.OrderByDescending(a => a.CreateDate)),
                StudyAbroadCategories = StudyAbroadCategories(),
                Citys = _unitOfWork.CityRepository.Get(a => a.Active, q => q.OrderBy(a => a.Sort)),
                StudyAbroad = new StudyAbroad
                {
                    Active = true,
                    Quantity = 1
                },
                DateHot = 0
            };
            return View(model);
        }
        [Route("tin-du-hoc")]
        [HttpPost, ValidateInput(false)]
        public ActionResult StudyAbroad(InsertStudyAbroadViewModel model, FormCollection fc)
        {
            if(ModelState.IsValid)
            {
                model.StudyAbroad.Url = HtmlHelpers.ConvertToUnSign(null, model.StudyAbroad.Name);
                model.StudyAbroad.CategoryId = Convert.ToInt32(fc["CategoryId"]);
                model.StudyAbroad.CompanyId = User.Id;
                model.StudyAbroad.Code = Utils.Utils.GenerateRandomCode();
                if (model.DateHot > 0)
                {
                    if (User.Amount != 0)
                    {
                        int amountToSubtract = 1000 * model.DateHot;
                        string formattedAmountToSubtract = amountToSubtract.ToString("#,0") + "đ";
                        if (amountToSubtract < User.Amount)
                        {
                            User.Amount -= amountToSubtract;
                            Utils.Utils.EmployerLog("Tài khoản bị trừ <strong>" + formattedAmountToSubtract + "</strong> để hiển thị tin <strong>" + model.StudyAbroad.Code + " </strong>" + "<strong class='text-danger'>" + model.DateHot + "</strong> ngày ở mục nổi bật", EmployerLogType.Deduction, User.Id, amountToSubtract);
                            model.StudyAbroad.Hot = DateTime.Now.AddDays(model.DateHot);
                            //Update cookie
                            HttpCookie cookie = Request.Cookies[".ASPXAUTHEMPLOYER"];
                            if (cookie != null && !string.IsNullOrEmpty(cookie.Value))
                            {
                                var userData = User.Avatar + "|" + User.Id + "|" + User.Email + "|" + User.FullName + "|" + User.Amount;
                                var ticket = new FormsAuthenticationTicket(2, User.Email.ToLower(), DateTime.Now, DateTime.Now.AddDays(30), true,
                                    userData, FormsAuthentication.FormsCookiePath);
                                var encTicket = FormsAuthentication.Encrypt(ticket);
                                HttpCookie updatedCookie = new HttpCookie(".ASPXAUTHEMPLOYER");
                                updatedCookie.Value = encTicket;
                                updatedCookie.Expires = DateTime.Now.AddDays(30);
                                Response.SetCookie(updatedCookie);
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", @"Số tiền trong tài khoản của bạn không đủ vui lòng nạp tiền để tiếp tục sử dụng");
                            model.Careers = _unitOfWork.CareerRepository.GetQuery(a => a.Active, o => o.OrderByDescending(a => a.CreateDate));
                            model.StudyAbroadCategories = StudyAbroadCategories();
                            model.Citys = _unitOfWork.CityRepository.Get(a => a.Active, q => q.OrderBy(a => a.Sort));
                            return View(model);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", @"Số tiền trong tài khoản của bạn không đủ vui lòng nạp tiền để tiếp tục sử dụng");
                        model.Careers = _unitOfWork.CareerRepository.GetQuery(a => a.Active, o => o.OrderByDescending(a => a.CreateDate));
                        model.StudyAbroadCategories = StudyAbroadCategories();
                        model.Citys = _unitOfWork.CityRepository.Get(a => a.Active, q => q.OrderBy(a => a.Sort));
                        return View(model);
                    }
                }

                _unitOfWork.StudyAbroadRepository.Insert(model.StudyAbroad);
                _unitOfWork.Save();
                var cities = fc["city"];
                if (cities != null)
                {
                    if (model.StudyAbroad.Cities == null)
                    {
                        model.StudyAbroad.Cities = new List<City>();
                    }
                    foreach (var cId in cities.Split(','))
                    {
                        var city = _unitOfWork.CityRepository.GetById(Convert.ToInt32(cId));
                        model.StudyAbroad.Cities.Add(city);
                    }
                    _unitOfWork.Save();
                }
                var careers = fc["careers"];
                if (careers != null)
                {
                    if (model.StudyAbroad.Careers == null)
                    {
                        model.StudyAbroad.Careers = new List<Career>();
                    }
                    foreach (var cId in careers.Split(','))
                    {
                        var career = _unitOfWork.CareerRepository.GetById(Convert.ToInt32(cId));
                        model.StudyAbroad.Careers.Add(career);
                    }
                    _unitOfWork.Save();
                }
                var stu = _unitOfWork.StudyAbroadRepository.GetQuery().AsNoTracking();
                if (stu.Any(p => p.Url.ToLower().Trim() == model.StudyAbroad.Url.ToLower().Trim()))
                {
                    model.StudyAbroad.Url += "-" + model.StudyAbroad.Id;
                    _unitOfWork.Save();
                }
                return RedirectToAction("ListStudyAbroad");
            }
            model.Careers = _unitOfWork.CareerRepository.GetQuery(a => a.Active, o => o.OrderByDescending(a => a.CreateDate));
            model.StudyAbroadCategories = StudyAbroadCategories();
            model.Citys = _unitOfWork.CityRepository.Get(a => a.Active, q => q.OrderBy(a => a.Sort));
            return View(model);
        }
        #endregion
        public ActionResult OrderTracking()
        {
            return View();
        }
        [Route("lich-su-hoat-dong")]
        public ActionResult History()
        {
            var log = _unitOfWork.EmployerLogRepository.GetQuery(a => a.UserId == User.Id , o => o.OrderByDescending(a => a.CreateDate));
            return View(log);
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