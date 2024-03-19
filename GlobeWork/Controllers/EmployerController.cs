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
using System.Threading.Tasks;
using Microsoft.Ajax.Utilities;

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
        private IEnumerable<StudyAbroadCategory> StudyAbroadCategories() => _unitOfWork.StudyAbroadCategoryRepository.GetQuery(a => a.Active, o => o.OrderBy(a => a.Sort));

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
            model.CitySelectList = CitySelectList;
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
            return RedirectToAction("EmployerLogin", "Employer");
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
            var email = _unitOfWork.EmployerRepository.GetQuery(a => a.Active && a.Email == model.Email).FirstOrDefault();
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
                var user = _unitOfWork.EmployerRepository.GetQuery(a => a.Active && a.Token == model.Token).FirstOrDefault();
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
            var company = _unitOfWork.CompanyRepository.Get(a => a.EmployerId == User.Id).FirstOrDefault();
            var model = new InserCompanyEmployerViewModel
            {
                Ranks = _unitOfWork.RankRepository.Get(orderBy: o => o.OrderBy(a => a.Name)),
                Careers = _unitOfWork.CareerRepository.GetQuery(a => a.Active && a.TypeCareer == TypeCareer.Activity),
                Company = _unitOfWork.CompanyRepository.Get(a => a.EmployerId == User.Id).FirstOrDefault(),
                Cities = _unitOfWork.CityRepository.Get(a => a.Active, q => q.OrderBy(a => a.Sort)),
                Countries = _unitOfWork.CountryRepository.Get(a => a.Active, q => q.OrderBy(a => a.Sort)),
            };
            return View(model);
        }
        [Route("cong-ty")]
        [HttpPost, ValidateInput(false)]
        public ActionResult Company(InserCompanyEmployerViewModel model, FormCollection fc)
        {
            var company = _unitOfWork.CompanyRepository.GetQuery(a => a.EmployerId == User.Id).FirstOrDefault();
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
                            model.Ranks = _unitOfWork.RankRepository.Get(orderBy: o => o.OrderBy(a => a.Name));
                            model.Careers = _unitOfWork.CareerRepository.Get(orderBy: o => o.OrderBy(a => a.Name));
                            model.Company = _unitOfWork.CompanyRepository.Get(a => a.EmployerId == User.Id).FirstOrDefault();
                            model.Cities = _unitOfWork.CityRepository.Get(a => a.Active, q => q.OrderBy(a => a.Sort));
                            return View(model);
                        }
                        else
                        {
                            if (avatar.ContentLength > 4000 * 1024)
                            {
                                isPost = false;
                                ModelState.AddModelError("", @"Dung lượng lớn hơn 4MB. Hãy thử lại");
                                model.Ranks = _unitOfWork.RankRepository.Get(orderBy: o => o.OrderBy(a => a.Name));
                                model.Careers = _unitOfWork.CareerRepository.Get(orderBy: o => o.OrderBy(a => a.Name));
                                model.Company = _unitOfWork.CompanyRepository.Get(a => a.EmployerId == User.Id).FirstOrDefault();
                                model.Cities = _unitOfWork.CityRepository.Get(a => a.Active, q => q.OrderBy(a => a.Sort));
                                return View(model);
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
                            model.Ranks = _unitOfWork.RankRepository.Get(orderBy: o => o.OrderBy(a => a.Name));
                            model.Careers = _unitOfWork.CareerRepository.Get(orderBy: o => o.OrderBy(a => a.Name));
                            model.Company = _unitOfWork.CompanyRepository.Get(a => a.EmployerId == User.Id).FirstOrDefault();
                            model.Cities = _unitOfWork.CityRepository.Get(a => a.Active, q => q.OrderBy(a => a.Sort));
                            return View(model);
                        }
                        else
                        {
                            if (cover.ContentLength > 4000 * 1024)
                            {
                                isPost = false;
                                ModelState.AddModelError("", @"Dung lượng lớn hơn 4MB. Hãy thử lại");
                                model.Ranks = _unitOfWork.RankRepository.Get(orderBy: o => o.OrderBy(a => a.Name));
                                model.Careers = _unitOfWork.CareerRepository.Get(orderBy: o => o.OrderBy(a => a.Name));
                                model.Company = _unitOfWork.CompanyRepository.Get(a => a.EmployerId == User.Id).FirstOrDefault();
                                model.Cities = _unitOfWork.CityRepository.Get(a => a.Active, q => q.OrderBy(a => a.Sort));
                                return View(model);
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
                        if (model.DateHot > 0)
                        {
                            if (User.Amount != 0)
                            {
                                int amountToSubtract = Convert.ToInt32((ConfigSite.PriceCompany ?? 30000) * model.DateHot);
                                string formattedAmountToSubtract = amountToSubtract.ToString("#,0") + "đ";
                                if (amountToSubtract < User.Amount)
                                {
                                    User.Amount -= amountToSubtract;
                                    Utils.Utils.EmployerLog("Tài khoản bị trừ <strong>" + formattedAmountToSubtract + "</strong> để hiển thị tin <strong>Công ty</strong>" + "<strong class='text-danger'>" + model.DateHot + "</strong> ngày ở mục nổi bật", EmployerLogType.Deduction, User.Id, amountToSubtract);
                                    model.Company.Vipdate = DateTime.Now.AddDays(model.DateHot);
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
                                    model.Ranks = _unitOfWork.RankRepository.Get(orderBy: o => o.OrderBy(a => a.Name));
                                    model.Careers = _unitOfWork.CareerRepository.Get(orderBy: o => o.OrderBy(a => a.Name));
                                    model.Company = _unitOfWork.CompanyRepository.Get(a => a.EmployerId == User.Id).FirstOrDefault();
                                    model.Cities = _unitOfWork.CityRepository.Get(a => a.Active, q => q.OrderBy(a => a.Sort));
                                    return View(model);
                                }
                            }
                            else
                            {
                                ModelState.AddModelError("", @"Số tiền trong tài khoản của bạn không đủ vui lòng nạp tiền để tiếp tục sử dụng");
                                model.Ranks = _unitOfWork.RankRepository.Get(orderBy: o => o.OrderBy(a => a.Name));
                                model.Careers = _unitOfWork.CareerRepository.Get(orderBy: o => o.OrderBy(a => a.Name));
                                model.Company = _unitOfWork.CompanyRepository.Get(a => a.EmployerId == User.Id).FirstOrDefault();
                                model.Cities = _unitOfWork.CityRepository.Get(a => a.Active, q => q.OrderBy(a => a.Sort));
                                return View(model);
                            }
                        }
                        model.Company.EmployerId = User.Id;
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
                        var urlCount = _unitOfWork.CompanyRepository.GetQuery(a => a.Url == model.Company.Url).Count();
                        if (urlCount > 1)
                        {
                            model.Company.Url += "-" + model.Company.EmployerId;
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
                        model.Ranks = _unitOfWork.RankRepository.Get(orderBy: o => o.OrderBy(a => a.Name));
                        model.Careers = _unitOfWork.CareerRepository.Get(orderBy: o => o.OrderBy(a => a.Name));
                        model.Company = _unitOfWork.CompanyRepository.Get(a => a.EmployerId == User.Id).FirstOrDefault();
                        model.Cities = _unitOfWork.CityRepository.Get(a => a.Active, q => q.OrderBy(a => a.Sort));
                        return View(model);
                    }
                    else
                    {
                        if (avatar.ContentLength > 4000 * 1024)
                        {
                            ModelState.AddModelError("", @"Dung lượng lớn hơn 4MB. Hãy thử lại");
                            model.Ranks = _unitOfWork.RankRepository.Get(orderBy: o => o.OrderBy(a => a.Name));
                            model.Careers = _unitOfWork.CareerRepository.Get(orderBy: o => o.OrderBy(a => a.Name));
                            model.Company = _unitOfWork.CompanyRepository.Get(a => a.EmployerId == User.Id).FirstOrDefault();
                            model.Cities = _unitOfWork.CityRepository.Get(a => a.Active, q => q.OrderBy(a => a.Sort));
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
                        model.Ranks = _unitOfWork.RankRepository.Get(orderBy: o => o.OrderBy(a => a.Name));
                        model.Careers = _unitOfWork.CareerRepository.Get(orderBy: o => o.OrderBy(a => a.Name));
                        model.Company = _unitOfWork.CompanyRepository.Get(a => a.EmployerId == User.Id).FirstOrDefault();
                        model.Cities = _unitOfWork.CityRepository.Get(a => a.Active, q => q.OrderBy(a => a.Sort));
                        return View(model);
                    }
                    else
                    {
                        if (cover.ContentLength > 4000 * 1024)
                        {
                            ModelState.AddModelError("", @"Dung lượng lớn hơn 4MB. Hãy thử lại");
                            model.Ranks = _unitOfWork.RankRepository.Get(orderBy: o => o.OrderBy(a => a.Name));
                            model.Careers = _unitOfWork.CareerRepository.Get(orderBy: o => o.OrderBy(a => a.Name));
                            model.Company = _unitOfWork.CompanyRepository.Get(a => a.EmployerId == User.Id).FirstOrDefault();
                            model.Cities = _unitOfWork.CityRepository.Get(a => a.Active, q => q.OrderBy(a => a.Sort));
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
                company.Age = model.Company.Age;
                company.CityId = Convert.ToInt32(fc["city"]);
                company.Email = model.Company.Email;
                company.Url = HtmlHelpers.ConvertToUnSign(null, company.Url ?? company.Name);
                _unitOfWork.CompanyRepository.Update(company);
                company.Careers.Clear();
                _unitOfWork.Save();
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
                if (model.DateHot > 0)
                {
                    if (User.Amount != 0)
                    {
                        int amountToSubtract = Convert.ToInt32((ConfigSite.PriceCompany ?? 30000) * model.DateHot);
                        string formattedAmountToSubtract = amountToSubtract.ToString("#,0") + "đ";
                        if (amountToSubtract < User.Amount)
                        {
                            User.Amount -= amountToSubtract;
                            Utils.Utils.EmployerLog("Tài khoản bị trừ <strong>" + formattedAmountToSubtract + "</strong> để hiển thị tin <strong>Công ty</strong>" + "<strong class='text-danger'>" + model.DateHot + "</strong> ngày ở mục nổi bật", EmployerLogType.Deduction, User.Id, amountToSubtract);
                            if (company.Vipdate != null)
                            {
                                if (company.Vipdate < DateTime.Now)
                                {
                                    company.Vipdate = DateTime.Now.AddDays(model.DateHot);
                                }
                                else
                                {
                                    company.Vipdate = company.Vipdate.AddDays(model.DateHot);
                                }
                            }
                            else
                            {
                                company.Vipdate = DateTime.Now.AddDays(model.DateHot);
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
                            model.Company = company;
                            model.Ranks = _unitOfWork.RankRepository.Get(orderBy: o => o.OrderBy(a => a.Name));
                            model.Careers = _unitOfWork.CareerRepository.Get(orderBy: o => o.OrderBy(a => a.Name));
                            model.Company = _unitOfWork.CompanyRepository.Get(a => a.EmployerId == User.Id).FirstOrDefault();
                            model.Cities = _unitOfWork.CityRepository.Get(a => a.Active, q => q.OrderBy(a => a.Sort));
                            return View(model);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", @"Số tiền trong tài khoản của bạn không đủ vui lòng nạp tiền để tiếp tục sử dụng");
                        model.Ranks = _unitOfWork.RankRepository.Get(orderBy: o => o.OrderBy(a => a.Name));
                        model.Company = company;
                        model.Careers = _unitOfWork.CareerRepository.Get(orderBy: o => o.OrderBy(a => a.Name));
                        model.Company = _unitOfWork.CompanyRepository.Get(a => a.EmployerId == User.Id).FirstOrDefault();
                        model.Cities = _unitOfWork.CityRepository.Get(a => a.Active, q => q.OrderBy(a => a.Sort));
                        return View(model);
                    }
                }

                var urlCount = _unitOfWork.CompanyRepository.GetQuery(a => a.Url == company.Url).Count();
                if (urlCount > 1)
                {
                    company.Url += "-" + company.EmployerId;
                    _unitOfWork.Save();
                }
                return RedirectToAction("Company", new { result = "update" });
            }
            model.Ranks = _unitOfWork.RankRepository.Get(orderBy: o => o.OrderBy(a => a.Name));
            model.Careers = _unitOfWork.CareerRepository.Get(orderBy: o => o.OrderBy(a => a.Name));
            model.Company = _unitOfWork.CompanyRepository.Get(a => a.EmployerId == User.Id).FirstOrDefault();
            model.Cities = _unitOfWork.CityRepository.Get(a => a.Active, q => q.OrderBy(a => a.Sort));
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

            var model = new EmployerViewModel
            {
                Employer = User,
                JobPost = _unitOfWork.JobPostRepository.GetQuery(a => a.CompanyId == User.Id).Count(),
                Article = _unitOfWork.ArticleRepository.GetQuery(a => a.EmployerId == User.Id).Count(),
                Study = _unitOfWork.StudyAbroadRepository.GetQuery(a => a.CompanyId == User.Id).Count(),
                Cv = _unitOfWork.ApplyJobRepository.GetQuery(a => a.CompanyId == User.Id).Count(),
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
        [Route("danh-sach-ung-tuyen")]
        public ActionResult ListCV(int? page, int? status, int jobId = 0, string name = "" , int type = 0)
        {
            var pageNumber = page ?? 1;
            var apply = _unitOfWork.ApplyJobRepository.GetQuery(a => a.CompanyId == User.Id && a.JobPostId != null, l => l.OrderByDescending(a => a.CreateDate));
            if (jobId > 0)
            {
                apply = apply.Where(a => a.JobPostId == jobId);
            }
            if (!string.IsNullOrEmpty(name))
            {
                apply = apply.Where(a => a.User.FullName.ToLower().Contains(name.ToLower()));
            }
            switch (type)
            {
                case 1:
                    apply = apply.Where(a => a.JobPostId != null);
                    break;
                case 2:
                    apply = apply.Where(a => a.StudyAbroadId != null);
                    break;
            }
            switch (status)
            {
                case 0:
                    apply = apply.Where(a => a.Status == ApplyJobStatus.Waiting);
                    break;
                case 1:
                    apply = apply.Where(a => a.Status == ApplyJobStatus.View);
                    break;
                case 2:
                    apply = apply.Where(a => a.Status == ApplyJobStatus.Active);
                    break;
                case 3:
                    apply = apply.Where(a => a.Status == ApplyJobStatus.NoActive);
                    break;
            }
            var model = new ListCvViewModel
            {
                ApplyJobs = apply.ToPagedList(pageNumber, 9),
                JobId = jobId,
                Status = status,
                Name = name,
                Type = type
            };
            return View(model);
        }
        [Route("danh-sach-ung-tuyen/du-hoc")]
        public ActionResult ListCVStudy(int? page, int? status, int jobId = 0, string name = "", int type = 0)
        {
            var pageNumber = page ?? 1;
            var apply = _unitOfWork.ApplyJobRepository.GetQuery(a => a.CompanyId == User.Id && a.StudyAbroadId != null, l => l.OrderByDescending(a => a.CreateDate));
            if (jobId > 0)
            {
                apply = apply.Where(a => a.JobPostId == jobId);
            }
            if (!string.IsNullOrEmpty(name))
            {
                apply = apply.Where(a => a.User.FullName.ToLower().Contains(name.ToLower()));
            }
            switch (type)
            {
                case 1:
                    apply = apply.Where(a => a.JobPostId != null);
                    break;
                case 2:
                    apply = apply.Where(a => a.StudyAbroadId != null);
                    break;
            }
            switch (status)
            {
                case 0:
                    apply = apply.Where(a => a.Status == ApplyJobStatus.Waiting);
                    break;
                case 1:
                    apply = apply.Where(a => a.Status == ApplyJobStatus.View);
                    break;
                case 2:
                    apply = apply.Where(a => a.Status == ApplyJobStatus.Active);
                    break;
                case 3:
                    apply = apply.Where(a => a.Status == ApplyJobStatus.NoActive);
                    break;
            }
            var model = new ListCvViewModel
            {
                ApplyJobs = apply.ToPagedList(pageNumber, 9),
                JobId = jobId,
                Status = status,
                Name = name,
                Type = type
            };
            return View(model);
        }
        [Route("xem")]
        public ActionResult ViewApply(int id)
        {
            var cv = _unitOfWork.ApplyJobRepository.GetById(id);
            if(cv == null)
            {
                return RedirectToAction("ListCv", "Employer");
            }
            cv.Status = ApplyJobStatus.View;
            _unitOfWork.Save();
            
            return RedirectToAction("UserProfile", "Home", new {url = cv.User.Url , type = 1});
        }
        [HttpPost]
        public JsonResult UpdateStatusCV(int id , int status = 0)
        {
            var cv = _unitOfWork.ApplyJobRepository.GetById(id);
            if (cv == null)
            {
                return Json(new { success = false, message = "Quá trình thực hiện không thành công" });
            }
            if(status == 1)
            {
                cv.Status = ApplyJobStatus.Active;
            }else if(status == 2)
            {
                cv.Status = ApplyJobStatus.NoActive;
            }
            _unitOfWork.Save();
            var ts = "Phù hợp";
            if(cv.Status == ApplyJobStatus.NoActive)
            {
                ts = "Chưa phù hợp";
            }
            var emailTemp = System.IO.File.ReadAllText(Server.MapPath("/EmailTemplates/Apply.html"));
            if(cv.JobPostId != null)
            {
                Utils.Utils.UserLog("Nhà tuyển dụng <strong>" + cv.JobPost.Company.Name + "</strong> đã đánh giá Cv của bạn là <strong>" + ts + "</strong>", false, cv.UserId);
                emailTemp = emailTemp.Replace("[FULLNAME]", cv.User.FullName).Replace("[JOB]", cv.JobPost.Name).Replace("[STATUS]", ts);
                var result = HtmlHelpers.SendEmail("gmail", "Thông báo kết quả tuyển dụng " + cv.JobPost.Company.Name, emailTemp, cv.User.Email, ConfigSite.EmailConfigs, ConfigSite.EmailConfigs, ConfigSite.PassWordMail, ConfigSite.Title);
            }
            if (cv.StudyAbroadId != null)
            {
                Utils.Utils.UserLog("Nhà tuyển dụng <strong>" + cv.StudyAbroad.Company.Name + "</strong> đã đánh giá Cv của bạn là <strong>" + ts + "</strong>", false, cv.UserId);
                emailTemp = emailTemp.Replace("[FULLNAME]", cv.User.FullName).Replace("[JOB]", cv.StudyAbroad.Name).Replace("[STATUS]", ts);
                var result = HtmlHelpers.SendEmail("gmail", "Thông báo kết quả tuyển dụng " + cv.StudyAbroad.Company.Name, emailTemp, cv.User.Email, ConfigSite.EmailConfigs, ConfigSite.EmailConfigs, ConfigSite.PassWordMail, ConfigSite.Title);
            }

            return Json(new { success = true, message = "Cập nhật thành công" });
        }
        [HttpPost]
        public JsonResult DeleteApply(int id)
        {
            var cv = _unitOfWork.ApplyJobRepository.GetById(id);
            if (cv == null)
            {
                return Json(new { success = false, message = "Quá trình thực hiện không thành công" });
            }
            _unitOfWork.ApplyJobRepository.Delete(cv);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Xóa thành công" });
        }
        [Route("thong-tin-chi-tiet")]
        public ActionResult ViewCv(int id)
        {
            var cv = _unitOfWork.ApplyJobRepository.GetById(id);
            if (cv == null)
            {
                return RedirectToAction("ListCv");
            }
            if(cv.Status != ApplyJobStatus.Active || cv.Status != ApplyJobStatus.NoActive)
            {
                cv.Status = ApplyJobStatus.View;
            }
            _unitOfWork.Save();
            return View(cv);
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
                Countries = _unitOfWork.CountryRepository.Get(a => a.Active, q => q.OrderBy(a => a.Sort)),
                Skills = _unitOfWork.SkillRepository.GetQuery(a => a.TypeSkill == TypeSkill.Skill),
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
                model.JobPost.Country = _unitOfWork.CountryRepository.GetById(model.JobPost.CounId);
                var company = _unitOfWork.CompanyRepository.GetQuery(a => a.EmployerId == User.Id).FirstOrDefault();
                if (company == null)
                {
                    return RedirectToAction("Company");
                }
                model.JobPost.CompanyId = company.EmployerId;
                model.JobPost.Code = Utils.Utils.GenerateRandomCode();
                if (model.DateHot > 0)
                {
                    if (User.Amount != 0)
                    {
                        int amountToSubtract = Convert.ToInt32((ConfigSite.PriceJob ?? 30000) * model.DateHot);
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
                            model.Countries = _unitOfWork.CountryRepository.Get(a => a.Active, q => q.OrderBy(a => a.Sort));
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
                        model.Countries = _unitOfWork.CountryRepository.Get(a => a.Active, q => q.OrderBy(a => a.Sort));
                        return View(model);
                    }
                }

                _unitOfWork.JobPostRepository.Insert(model.JobPost);
                _unitOfWork.Save();
                var cities = fc["city"];
                if (cities != null)
                {
                    //if (model.JobPost.Cities == null)
                    //{
                    //    model.JobPost.Cities = new List<City>();
                    //}
                    foreach (var id in cities.Split(','))
                    {
                        var cityId = Convert.ToInt32(id);
                        var city = _unitOfWork.CityRepository.GetById(cityId);
                        city?.JobPosts.Add(model.JobPost);
                    }
                    _unitOfWork.Save();
                }
                var skill = fc["skill"];
                if (skill != null)
                {
                    //if (model.JobPost.Skills == null)
                    //{
                    //    model.JobPost.Skills = new List<Skill>();
                    //}
                    foreach (var id in skill.Split(','))
                    {
                        var skillId = Convert.ToInt32(id);
                        var skil = _unitOfWork.SkillRepository.GetById(skillId);
                        skil?.Posts.Add(model.JobPost);
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
            model.Countries = _unitOfWork.CountryRepository.Get(a => a.Active, q => q.OrderBy(a => a.Sort));
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
                Skills = _unitOfWork.SkillRepository.GetQuery(a => a.TypeSkill == TypeSkill.Skill),
                Countries = _unitOfWork.CountryRepository.Get(a => a.Active, q => q.OrderBy(a => a.Sort)),
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
                return RedirectToAction("PostNews");
            }
            if (ModelState.IsValid)
            {

                jobs.Url = HtmlHelpers.ConvertToUnSign(null, jobs.Name);
                jobs.RankId = Convert.ToInt32(fc["RankId"]);
                jobs.CareerId = Convert.ToInt32(fc["CareerId"]);
                jobs.JobTypeId = Convert.ToInt32(fc["JobTypeId"]);
                //jobs.CityId = Convert.ToInt32(fc["CityId"]);
                jobs.Country = _unitOfWork.CountryRepository.GetById(model.JobPost.CounId);
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
                if (jobs.Cities.Any())
                {
                    jobs.Cities.Clear();
                }
                if (jobs.Skills.Any())
                {
                    jobs.Skills.Clear();
                }
                _unitOfWork.Save();

                var cities = fc["city"];
                if (cities != "")
                {
                    foreach (var Id in cities.Split(','))
                    {
                        var cityId = Convert.ToInt32(Id);
                        var city = _unitOfWork.CityRepository.GetById(cityId);
                        city?.JobPosts.Add(jobs);
                    }
                }
                var skill = fc["skill"];
                if (skill != null)
                {
                    foreach (var skills in skill.Split(','))
                    {
                        var skillId = Convert.ToInt32(skills);
                        var skil = _unitOfWork.SkillRepository.GetById(skillId);
                        skil?.Posts.Add(jobs);
                    }
                }
                _unitOfWork.Save();
                if (model.DateHot > 0)
                {
                    if (User.Amount != 0)
                    {
                        int amountToSubtract = Convert.ToInt32((ConfigSite.PriceJob ?? 30000) * model.DateHot);
                        string formattedAmountToSubtract = amountToSubtract.ToString("#,0") + "đ";
                        if (amountToSubtract < User.Amount)
                        {
                            User.Amount -= amountToSubtract;
                            Utils.Utils.EmployerLog("Tài khoản bị trừ <strong>" + formattedAmountToSubtract + "</strong> để hiển thị tin <strong>" + jobs.Code + " </strong>" + "<strong class='text-danger'>" + model.DateHot + "</strong> ngày ở mục nổi bật", EmployerLogType.Deduction, User.Id, amountToSubtract);
                            if (jobs.Hot != null)
                            {
                                if (jobs.Hot < DateTime.Now)
                                {
                                    jobs.Hot = DateTime.Now.AddDays(model.DateHot);
                                }
                                else
                                {
                                    jobs.Hot = jobs.Hot?.AddDays(model.DateHot);
                                }
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
                            model.JobPost = jobs;
                            model.Careers = _unitOfWork.CareerRepository.GetQuery(a => a.Active, o => o.OrderByDescending(a => a.CreateDate));
                            model.JobTypes = _unitOfWork.JobTypeRepository.GetQuery(orderBy: o => o.OrderByDescending(a => a.CreateDate));
                            model.Ranks = _unitOfWork.RankRepository.GetQuery(orderBy: o => o.OrderByDescending(a => a.CreateDate));
                            model.Skills = _unitOfWork.SkillRepository.Get(orderBy: a => a.OrderBy(l => l.Id));
                            model.Citys = _unitOfWork.CityRepository.Get(a => a.Active, q => q.OrderBy(a => a.Sort));
                            model.Countries = _unitOfWork.CountryRepository.Get(a => a.Active, q => q.OrderBy(a => a.Sort));
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
                        model.Countries = _unitOfWork.CountryRepository.Get(a => a.Active, q => q.OrderBy(a => a.Sort));
                        return View(model);
                    }
                }
                
                var job = _unitOfWork.JobPostRepository.GetQuery().AsNoTracking();
                if (job.Any(p => p.Url.ToLower().Trim() == jobs.Url.ToLower().Trim()))
                {
                    jobs.Url += "-" + jobs.Id;
                }
                _unitOfWork.Save();
                return RedirectToAction("ListNews");
            }
            model.Careers = _unitOfWork.CareerRepository.GetQuery(a => a.Active, o => o.OrderByDescending(a => a.CreateDate));
            model.JobTypes = _unitOfWork.JobTypeRepository.GetQuery(orderBy: o => o.OrderByDescending(a => a.CreateDate));
            model.Ranks = _unitOfWork.RankRepository.GetQuery(orderBy: o => o.OrderByDescending(a => a.CreateDate));
            model.Skills = _unitOfWork.SkillRepository.Get(orderBy: a => a.OrderBy(l => l.Id));
            model.Citys = _unitOfWork.CityRepository.Get(a => a.Active, q => q.OrderBy(a => a.Sort));
            model.Countries = _unitOfWork.CountryRepository.Get(a => a.Active, q => q.OrderBy(a => a.Sort));
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
                    Quantity = 1,
                    View = 0,
                },
                DateHot = 0
            };
            return View(model);
        }
        [Route("tin-du-hoc")]
        [HttpPost, ValidateInput(false)]
        public ActionResult StudyAbroad(InsertStudyAbroadViewModel model, FormCollection fc)
        {
            if (ModelState.IsValid)
            {
                model.StudyAbroad.Url = HtmlHelpers.ConvertToUnSign(null, model.StudyAbroad.Name);
                model.StudyAbroad.CategoryId = Convert.ToInt32(fc["CategoryId"]);
                model.StudyAbroad.CareerId = Convert.ToInt32(fc["CareerId"]);
                model.StudyAbroad.Careers = _unitOfWork.CareerRepository.GetById(model.StudyAbroad.CareerId) ?? null;
                model.StudyAbroad.StudyAbroadCategory = _unitOfWork.StudyAbroadCategoryRepository.GetById(model.StudyAbroad.CategoryId) ?? null;
                model.StudyAbroad.CompanyId = User.Id;
                model.StudyAbroad.Code = Utils.Utils.GenerateRandomCode();
                model.StudyAbroad.ListImage = fc["Pictures"];
                var company = _unitOfWork.CompanyRepository.GetQuery(a => a.EmployerId == User.Id).FirstOrDefault();
                if (company == null)
                {
                    return RedirectToAction("Company");
                }
                if (model.DateHot > 0)
                {
                    if (User.Amount != 0)
                    {
                        int amountToSubtract = Convert.ToInt32((ConfigSite.PriceStudyAbroad ?? 30000) * model.DateHot);
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
                var file = Request.Files["StudyAbroad.Image"];
                if (file == null || file.ContentLength <= 0)
                {
                    ModelState.AddModelError("", @"Hãy chọn 1 hình ảnh.");
                    model.StudyAbroadCategories = StudyAbroadCategories();
                    model.Careers = _unitOfWork.CareerRepository.GetQuery(a => a.Active, o => o.OrderByDescending(a => a.CreateDate));
                    model.Citys = _unitOfWork.CityRepository.Get(a => a.Active, q => q.OrderBy(a => a.Sort));

                    return View(model);
                }
                else
                {
                    if (!HtmlHelpers.CheckFileExt(file.FileName, "jpg|jpeg|png|gif|svg"))
                    {
                        ModelState.AddModelError("", @"Chỉ chấp nhận định dạng jpg, png, gif, jpeg, svg");
                        model.StudyAbroadCategories = StudyAbroadCategories();
                        model.Careers = _unitOfWork.CareerRepository.GetQuery(a => a.Active, o => o.OrderByDescending(a => a.CreateDate));
                        return View(model);
                    }

                    if (file.ContentLength > 4000 * 1024)
                    {
                        ModelState.AddModelError("", @"Dung lượng lớn hơn 4MB. Hãy thử lại");
                        model.StudyAbroadCategories = StudyAbroadCategories();
                        model.Careers = _unitOfWork.CareerRepository.GetQuery(a => a.Active, o => o.OrderByDescending(a => a.CreateDate));
                        return View(model);
                    }
                    var imgPath = "/images/studyAbroad/" + DateTime.Now.ToString("yyyy/MM/dd");
                    HtmlHelpers.CreateFolder(Server.MapPath(imgPath));
                    var imgFileName = HtmlHelpers.ConvertToUnSign(null, Path.GetFileNameWithoutExtension(file.FileName)) + "-" + DateTime.Now.Millisecond + Path.GetExtension(file.FileName);
                    model.StudyAbroad.Image = DateTime.Now.ToString("yyyy/MM/dd") + "/" + imgFileName;
                    file.SaveAs(Server.MapPath(Path.Combine(imgPath, imgFileName)));
                }
                _unitOfWork.StudyAbroadRepository.Insert(model.StudyAbroad);
                _unitOfWork.Save();
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
        [Route("cap-nhat-tin-du-hoc")]
        public ActionResult EditStudyAbroad(int id)
        {
            var study = _unitOfWork.StudyAbroadRepository.GetById(id);
            if (study == null)
            {
                return RedirectToAction("ListStudyAbroad");
            }
            var model = new InsertStudyAbroadViewModel
            {
                StudyAbroad = study,
                DateHot = 0,
                Careers = _unitOfWork.CareerRepository.GetQuery(a => a.Active, o => o.OrderByDescending(a => a.CreateDate)),
                StudyAbroadCategories = StudyAbroadCategories(),
                Citys = _unitOfWork.CityRepository.Get(a => a.Active, q => q.OrderBy(a => a.Sort)),
            };
            return View(model);
        }
        [Route("cap-nhat-tin-du-hoc")]
        [HttpPost, ValidateInput(false)]
        public ActionResult EditStudyAbroad(InsertStudyAbroadViewModel model, FormCollection fc)
        {
            var study = _unitOfWork.StudyAbroadRepository.GetById(model.StudyAbroad.Id);
            if (study == null)
            {
                return RedirectToAction("ListStudyAbroad");
            }
            if (ModelState.IsValid)
            {
                for (var i = 0; i < Request.Files.Count; i++)
                {
                    if (Request.Files[i] == null || Request.Files[i].ContentLength <= 0) continue;
                    if (!HtmlHelpers.CheckFileExt(Request.Files[i].FileName, "jpg|jpeg|png|gif")) continue;
                    if (Request.Files[i].ContentLength > 1024 * 1024 * 4) continue;

                    var imgFileName = HtmlHelpers.ConvertToUnSign(null, Path.GetFileNameWithoutExtension(Request.Files[i].FileName)) +
                        "-" + DateTime.Now.Millisecond + Path.GetExtension(Request.Files[i].FileName);
                    var imgPath = "/images/studyAbroad/" + DateTime.Now.ToString("yyyy/MM/dd");
                    HtmlHelpers.CreateFolder(Server.MapPath(imgPath));

                    var imgFile = DateTime.Now.ToString("yyyy/MM/dd") + "/" + imgFileName;

                    var newImage = System.Drawing.Image.FromStream(Request.Files[i].InputStream);
                    var fixSizeImage = HtmlHelpers.FixedSize(newImage, 1000, 1000, false);
                    HtmlHelpers.SaveJpeg(Server.MapPath(Path.Combine(imgPath, imgFileName)), fixSizeImage, 90);

                    if (Request.Files.Keys[i] == "StudyAbroad.Image")
                    {
                        study.Image = imgFile;
                    }
                }
                var file = Request.Files["StudyAbroad.Image"];
                if (file != null && file.ContentLength == 0)
                {
                    study.Image = fc["CurrentFile"] == "" ? null : fc["CurrentFile"];
                }
                study.ListImage = fc["Pictures"] == "" ? null : fc["Pictures"];
                if (model.DateHot > 0)
                {
                    if (User.Amount != 0)
                    {
                        int amountToSubtract = Convert.ToInt32((ConfigSite.PriceJob ?? 30000) * model.DateHot);
                        string formattedAmountToSubtract = amountToSubtract.ToString("#,0") + "đ";
                        if (amountToSubtract < User.Amount)
                        {
                            User.Amount -= amountToSubtract;
                            Utils.Utils.EmployerLog("Tài khoản bị trừ <strong>" + formattedAmountToSubtract + "</strong> để hiển thị tin <strong>" + study.Name + " </strong>" + "<strong class='text-danger'>" + model.DateHot + "</strong> ngày ở mục nổi bật", EmployerLogType.Deduction, User.Id, amountToSubtract);
                            if (study.Hot != null)
                            {
                                if (study.Hot < DateTime.Now)
                                {
                                    study.Hot = DateTime.Now.AddDays(model.DateHot);
                                }
                                else
                                {
                                    study.Hot = study.Hot?.AddDays(model.DateHot);
                                }
                            }
                            else
                            {
                                study.Hot = DateTime.Now.AddDays(model.DateHot);
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
                study.Url = HtmlHelpers.ConvertToUnSign(null, model.StudyAbroad.Name);
                study.CategoryId = Convert.ToInt32(fc["CategoryId"]);
                study.CareerId = Convert.ToInt32(fc["CareerId"]);
                study.StudyAbroadCategory = _unitOfWork.StudyAbroadCategoryRepository.GetById(study.CategoryId) ?? null;
                study.Careers = _unitOfWork.CareerRepository.GetById(study.CareerId) ?? null;
                study.Name = model.StudyAbroad.Name;
                study.Wages = model.StudyAbroad.Wages;
                study.Health = model.StudyAbroad.Health;
                study.Quantity = model.StudyAbroad.Quantity;
                study.Gender = model.StudyAbroad.Gender;
                study.Address = model.StudyAbroad.Address;
                study.ExpirationDate = model.StudyAbroad.ExpirationDate;
                study.LearningProblems = model.StudyAbroad.LearningProblems;
                study.Body = model.StudyAbroad.Body;
                study.Requirements = model.StudyAbroad.Requirements;
                study.Incentives = model.StudyAbroad.Incentives;
                study.TypeStudyAbroad = model.StudyAbroad.TypeStudyAbroad;
                _unitOfWork.Save();
                var stu = _unitOfWork.StudyAbroadRepository.GetQuery().AsNoTracking();
                if (stu.Any(p => p.Url.ToLower().Trim() == study.Url.ToLower().Trim()))
                {
                    study.Url += "-" + study.Id;
                    _unitOfWork.Save();
                }
                return RedirectToAction("ListStudyAbroad");
            }
            model.Careers = _unitOfWork.CareerRepository.GetQuery(a => a.Active, o => o.OrderByDescending(a => a.CreateDate));
            model.StudyAbroadCategories = StudyAbroadCategories();
            model.Citys = _unitOfWork.CityRepository.Get(a => a.Active, q => q.OrderBy(a => a.Sort));
            return View(model);
        }

        [HttpPost]
        public bool DeleteStudy(int id)
        {
            var study = _unitOfWork.StudyAbroadRepository.GetById(id);
            if (study == null)
            {
                return false;
            }
            var apply = _unitOfWork.ApplyJobRepository.GetQuery(a => a.StudyAbroadId == study.Id);
            foreach(var item in apply)
            {
                _unitOfWork.ApplyJobRepository.Delete(item);
                _unitOfWork.Save();
            }
            _unitOfWork.StudyAbroadRepository.Delete(study);
            _unitOfWork.Save();
            return true;
        }
        #endregion

        #region Article 
        [Route("danh-sach-bai-viet")]
        public ActionResult ListArticle(int? page, string name, int? catId, int? childId, string result = "")
        {
            ViewBag.Result = result;
            var pageNumber = page ?? 1;
            const int pageSize = 15;
            var article = _unitOfWork.ArticleRepository.GetQuery(a => a.EmployerId == User.Id , o => o.OrderByDescending(a => a.CreateDate));
            if (!string.IsNullOrEmpty(name))
            {
                article = article.Where(l => l.Subject.Contains(name));
            }
            var model = new ListArticleViewModel
            {
                Articles = article.ToPagedList(pageNumber, pageSize),
                CatId = catId,
                ChildId = childId,
                Name = name
            };
            return View(model);
        }
        [Route("them-bai-viet")]
        public ActionResult Article()
        {
            var model = new InsertArticleViewModel
            {
                Article = new Article { Active = true, IsAdmin = false },
                StudyAbroadCategories = StudyAbroadCategories(),
            };
            return View(model);
        }
        [Route("them-bai-viet")]
        [HttpPost, ValidateInput(false)]
        public ActionResult Article(InsertArticleViewModel model, FormCollection fc)
        {
            if (ModelState.IsValid)
            {
                var company = _unitOfWork.CompanyRepository.GetQuery(a => a.EmployerId == User.Id).FirstOrDefault();
                if (company == null)
                {
                    return RedirectToAction("Company");
                }
                var file = Request.Files["Article.Image"];
                if (file == null || file.ContentLength <= 0)
                {
                    ModelState.AddModelError("", @"Hãy chọn 1 hình ảnh.");
                    model.StudyAbroadCategories = StudyAbroadCategories();
                    return View(model);
                }
                else
                {
                    if (!HtmlHelpers.CheckFileExt(file.FileName, "jpg|jpeg|png|gif|svg"))
                    {
                        ModelState.AddModelError("", @"Chỉ chấp nhận định dạng jpg, png, gif, jpeg, svg");
                        model.StudyAbroadCategories = StudyAbroadCategories();
                        return View(model);
                    }

                    if (file.ContentLength > 4000 * 1024)
                    {
                        ModelState.AddModelError("", @"Dung lượng lớn hơn 4MB. Hãy thử lại");
                        model.StudyAbroadCategories = StudyAbroadCategories();
                        return View(model);
                    }

                    var imgPath = "/images/articles/" + DateTime.Now.ToString("yyyy/MM/dd");
                    HtmlHelpers.CreateFolder(Server.MapPath(imgPath));
                    var imgFileName = HtmlHelpers.ConvertToUnSign(null, Path.GetFileNameWithoutExtension(file.FileName)) + "-" + DateTime.Now.Millisecond + Path.GetExtension(file.FileName);
                    model.Article.Image = DateTime.Now.ToString("yyyy/MM/dd") + "/" + imgFileName;
                    file.SaveAs(Server.MapPath(Path.Combine(imgPath, imgFileName)));
                }
                
                model.Article.Url = HtmlHelpers.ConvertToUnSign(null, model.Article.Url ?? model.Article.Subject);
                var articles = _unitOfWork.ArticleRepository.GetQuery().AsNoTracking();
                if (articles.Any(p => p.Url.Trim() == model.Article.Url.Trim()))
                {
                    model.Article.Url += "-" + DateTime.Now.Millisecond;
                }
                model.Article.StudyAbroadCategoryId = Convert.ToInt32(fc["StudyAbroadCategoryId"]);
                model.Article.IsAdmin = false;
                model.Article.EmployerId = User.Id;
                model.Article.StudyAbroadCategory = _unitOfWork.StudyAbroadCategoryRepository.GetById(Convert.ToInt32(fc["StudyAbroadCategoryId"])) ?? null;
                if (model.DateHot > 0)
                {
                    if (User.Amount != 0)
                    {
                        int amountToSubtract = Convert.ToInt32((ConfigSite.PriceArticle ?? 30000) * model.DateHot);
                        string formattedAmountToSubtract = amountToSubtract.ToString("#,0") + "đ";
                        if (amountToSubtract < User.Amount)
                        {
                            User.Amount -= amountToSubtract;
                            Utils.Utils.EmployerLog("Tài khoản bị trừ <strong>" + formattedAmountToSubtract + "</strong> để hiển thị bài viết <strong>" + model.Article.Subject + " </strong>" + "<strong class='text-danger'>" + model.DateHot + "</strong> ngày ở mục nổi bật", EmployerLogType.Deduction, User.Id, amountToSubtract);
                            model.Article.Hot = DateTime.Now.AddDays(model.DateHot);
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
                            model.StudyAbroadCategories = StudyAbroadCategories();
                            return View(model);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", @"Số tiền trong tài khoản của bạn không đủ vui lòng nạp tiền để tiếp tục sử dụng");
                        model.StudyAbroadCategories = StudyAbroadCategories();
                        return View(model);
                    }
                }
                _unitOfWork.ArticleRepository.Insert(model.Article);
                _unitOfWork.Save();
                return RedirectToAction("ListArticle", new { result = "success" });
            }
            model.StudyAbroadCategories = StudyAbroadCategories();
            return View(model);
        }
        [Route("sua-bai-viet")]
        public ActionResult UpdateArticle(int articleId = 0)
        {
            var article = _unitOfWork.ArticleRepository.GetById(articleId);
            if (article == null)
            {
                return RedirectToAction("ListArticle");
            }
            var model = new InsertArticleViewModel
            {
                StudyAbroadCategories = StudyAbroadCategories(),
                Article = article,
            };
            return View(model);
        }
        [Route("sua-bai-viet")]
        [HttpPost, ValidateInput(false)]
        public ActionResult UpdateArticle(InsertArticleViewModel model, FormCollection fc)
        {
            var article = _unitOfWork.ArticleRepository.GetById(model.Article.Id);
            if (article == null)
            {
                return RedirectToAction("ListArticle");
            }
            if (ModelState.IsValid)
            {
                for (var i = 0; i < Request.Files.Count; i++)
                {
                    if (Request.Files[i] == null || Request.Files[i].ContentLength <= 0) continue;
                    if (!HtmlHelpers.CheckFileExt(Request.Files[i].FileName, "jpg|jpeg|png|gif")) continue;
                    if (Request.Files[i].ContentLength > 1024 * 1024 * 4) continue;

                    var imgFileName = HtmlHelpers.ConvertToUnSign(null, Path.GetFileNameWithoutExtension(Request.Files[i].FileName)) +
                        "-" + DateTime.Now.Millisecond + Path.GetExtension(Request.Files[i].FileName);
                    var imgPath = "/images/articles/" + DateTime.Now.ToString("yyyy/MM/dd");
                    HtmlHelpers.CreateFolder(Server.MapPath(imgPath));

                    var imgFile = DateTime.Now.ToString("yyyy/MM/dd") + "/" + imgFileName;

                    var newImage = System.Drawing.Image.FromStream(Request.Files[i].InputStream);
                    var fixSizeImage = HtmlHelpers.FixedSize(newImage, 1000, 1000, false);
                    HtmlHelpers.SaveJpeg(Server.MapPath(Path.Combine(imgPath, imgFileName)), fixSizeImage, 90);

                    if (Request.Files.Keys[i] == "Article.Image")
                    {
                        article.Image = imgFile;
                    }
                }
                var file = Request.Files["Article.Image"];  

                if (file != null && file.ContentLength == 0)
                {
                    article.Image = fc["CurrentFile"] == "" ? null : fc["CurrentFile"];
                }
                article.StudyAbroadCategoryId = Convert.ToInt32(fc["StudyAbroadCategoryId"]);
                article.Url = HtmlHelpers.ConvertToUnSign(null, model.Article.Url ?? model.Article.Subject);
                article.Subject = model.Article.Subject;
                article.Description = model.Article.Description;
                article.Body = model.Article.Body;
                article.Active = model.Article.Active;
                article.TypeArticle = model.Article.TypeArticle;
                article.StudyAbroadCategory = _unitOfWork.StudyAbroadCategoryRepository.GetById(Convert.ToInt32(fc["StudyAbroadCategoryId"])) ?? null;
                article.TitleMeta = model.Article.TitleMeta;
                article.DescriptionMeta = model.Article.DescriptionMeta;
                if (model.DateHot > 0)
                {
                    if (User.Amount != 0)
                    {
                        int amountToSubtract = Convert.ToInt32((ConfigSite.PriceArticle ?? 30000) * model.DateHot);
                        string formattedAmountToSubtract = amountToSubtract.ToString("#,0") + "đ";
                        if (amountToSubtract < User.Amount)
                        {
                            User.Amount -= amountToSubtract;
                            Utils.Utils.EmployerLog("Tài khoản bị trừ <strong>" + formattedAmountToSubtract + "</strong> để hiển thị bài viết <strong>" + article.Subject + " </strong>" + "<strong class='text-danger'>" + model.DateHot + "</strong> ngày ở mục nổi bật", EmployerLogType.Deduction, User.Id, amountToSubtract);
                            if (article.Hot != null)
                            {
                                if (article.Hot < DateTime.Now)
                                {
                                    article.Hot = DateTime.Now.AddDays(model.DateHot);
                                }
                                else
                                {
                                    article.Hot = article.Hot?.AddDays(model.DateHot);
                                }
                            }
                            else
                            {
                                article.Hot = DateTime.Now.AddDays(model.DateHot);
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
                            model.StudyAbroadCategories = StudyAbroadCategories();
                            return View(model);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", @"Số tiền trong tài khoản của bạn không đủ vui lòng nạp tiền để tiếp tục sử dụng");
                        model.StudyAbroadCategories = StudyAbroadCategories();
                        return View(model);
                    }
                }
                _unitOfWork.Save();

                var articles = _unitOfWork.ArticleRepository.GetQuery().AsNoTracking();
                if (articles.Any(p => p.Url.ToLower().Trim() == model.Article.Url.ToLower().Trim() && p.Id != model.Article.Id))
                {
                    article.Url += "-" + DateTime.Now.Millisecond;
                }

                _unitOfWork.Save();

                return RedirectToAction("ListArticle", new { result = "update" });
            }
            model.StudyAbroadCategories = StudyAbroadCategories();
            return View(model);
        }
        [HttpPost]
        public bool DeleteArticle(int articleId = 0)
        {

            var article = _unitOfWork.ArticleRepository.GetById(articleId);
            if (article == null)
            {
                return false;
            }
            _unitOfWork.ArticleRepository.Delete(article);
            _unitOfWork.Save();
            return true;
        }
        [HttpPost]
        public bool UpdateStatusArticle(int id, int type = 0)
        {
            var article = _unitOfWork.ArticleRepository.GetById(id);
            if (article == null)
            {
                return false;
            }
            if (type == 1)
            {
                article.Active = false;
            }
            else if (type == 2)
            {
                article.Active = true;
            }
            _unitOfWork.Save();
            return true;
        }
        #endregion

        public ActionResult OrderTracking()
        {
            return View();
        }

        [Route("nap-tien")]
        public ActionResult Naptien()
        {
            var user = User;
            var log = _unitOfWork.EmployerLogRepository.GetQuery(a => a.UserId == user.Id && a.EmployerLogType == EmployerLogType.PublicMoney);
            decimal tongnap = 0;
            if (log != null && log.Any())
            {
                foreach (var item in log)
                {
                    tongnap += item.Amount;
                }
            }
            var model = new NaptienViewModel
            {
                Employer = user,
                TongNap = tongnap,
            };
            return View(model);
        }

        [Route("lich-su-hoat-dong")]
        public ActionResult History()
        {
            var log = _unitOfWork.EmployerLogRepository.GetQuery(a => a.UserId == User.Id, o => o.OrderByDescending(a => a.CreateDate));
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
        public JsonResult GetCitiesJob(int? countruyId)
        {
            var cities = _unitOfWork.CityRepository
                .GetQuery(a => a.Active && a.CountruyId == countruyId, q => q.OrderBy(a => a.Sort)).Select(a => new { a.Id, a.Name });
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