using GlobeWork.DAL;
using GlobeWork.Filters;
using GlobeWork.Models;
using GlobeWork.ViewModel;
using Helpers;
using System;
using System.IO;
using System.Drawing;
using System.Linq;
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

        #region Login 
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
        #endregion

        [ChildActionOnly]
        public PartialViewResult Header()
        {
            return PartialView();
        }

        public PartialViewResult Sidebar()
        {
            return PartialView();
        }
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
                if(employ == null)
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
            return View();
        }
        public PartialViewResult ChangePassword()
        {
            return PartialView();
        }
        public ActionResult ListNews()
        {
            return View();
        }
        [Route("dashboad")]
        public ActionResult Index()
        {
            var employer = _unitOfWork.EmployerRepository.GetById(User.Id);
            if (employer == null)
            {
                return RedirectToAction("Login", "User");
            }
            var model = new EmployerViewModel
            {
                Employer = employer
            };
            return View(model);
        }
        public PartialViewResult NavProfile()
        {
            return PartialView();
        }
        [Route("cong-ty")]
        public ActionResult Company()
        {
            var company = _unitOfWork.CompanyRepository.Get(a => a.UserId == User.Id).FirstOrDefault();
            var model = new InserCompanyEmployerViewModel
            {
                Ranks = _unitOfWork.RankRepository.Get(orderBy: o => o.OrderBy(a => a.Name)),
                Careers = _unitOfWork.CareerRepository.Get(orderBy: o => o.OrderBy(a => a.Name)),
                Company = _unitOfWork.CompanyRepository.Get(a => a.UserId == User.Id).FirstOrDefault(),
            };
            return View(model);
        }
        [Route("cong-ty")]
        [HttpPost , ValidateInput(false)]
        public ActionResult Company(InserCompanyEmployerViewModel model , FormCollection fc)
        {
            var company = _unitOfWork.CompanyRepository.GetQuery(a => a.UserId == User.Id).FirstOrDefault();
            if(company == null)
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
                        _unitOfWork.CompanyRepository.Insert(model.Company);
                        var careers = fc["career"];
                        if (!string.IsNullOrEmpty(careers))
                        {
                            var listCareer = careers.Split((',')).Select(int.Parse).ToList();
                            foreach (var item in listCareer)
                            {
                                var careerItem = _unitOfWork.CareerRepository.GetById(item);
                                company.Careers.Add(careerItem);
                            }
                        }
                        _unitOfWork.Save();
                        return RedirectToAction("Company");
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
                    }
                    else
                    {
                        if (avatar.ContentLength > 4000 * 1024)
                        {
                            ModelState.AddModelError("", @"Dung lượng lớn hơn 4MB. Hãy thử lại");
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
                    }
                    else
                    {
                        if (cover.ContentLength > 4000 * 1024)
                        {
                            ModelState.AddModelError("", @"Dung lượng lớn hơn 4MB. Hãy thử lại");
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
                company.WebsiteUrl = model.Company.Url;
                company.Address = model.Company.Address;
                company.CompanySize = model.Company.CompanySize;
                company.EstablishmentDate = model.Company.EstablishmentDate;
                company.Phone = model.Company.Phone;
                company.Introduct = model.Company.Introduct;
                company.Product = model.Company.Product;
                company.GoogleMap = model.Company.GoogleMap;
                company.VideoYoutube = model.Company.VideoYoutube;
                company.Url = HtmlHelpers.ConvertToUnSign(null, company.Url ?? company.Name);
                _unitOfWork.CompanyRepository.Update(company);
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
                }
                _unitOfWork.Save();
                return RedirectToAction("Company");
            }

            model.Careers = _unitOfWork.CareerRepository.Get(orderBy: o => o.OrderBy(a => a.Name));
            return View(model);
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
            var model = new InsertEmployerViewModel
            {
                Careers = _unitOfWork.CareerRepository.GetQuery(a => a.Active, o => o.OrderByDescending(a => a.CreateDate)),
                JobTypes = _unitOfWork.JobTypeRepository.GetQuery(orderBy: o => o.OrderByDescending(a => a.CreateDate)),
                Ranks = _unitOfWork.RankRepository.GetQuery(orderBy: o => o.OrderByDescending(a => a.CreateDate))
            };
            return View(model);
        }
        //[HttpPost]
        //public ActionResult PostNews()
        //{
        //    return View();
        //}
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