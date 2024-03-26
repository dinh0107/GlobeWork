using Helpers;
using GlobeWork.Filters;
using GlobeWork.Models;
using GlobeWork.Utils;
using GlobeWork.ViewModel;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace GlobeWork.Controllers
{
    [Authorize]
    public class UserVcmsController : BaseController
    {
        public ConfigSite ConfigSite => (ConfigSite)HttpContext.Application["ConfigSite"];
        #region User
        public ActionResult ListUser(int? page, string startTime, string endTime, int? status, string email, string name, int? typeUser)
        {   

            var pageNumber = page ?? 1;
            var pageSize = 30;

            var users = _unitOfWork.UserRepository.GetQuery(orderBy: q => q.OrderByDescending(p => p.Id)).AsNoTracking();
            if (!string.IsNullOrEmpty(name))
            {
                users = users.Where(p => p.Email.ToLower().Contains(email.ToLower()));
            }
            if (!string.IsNullOrEmpty(email))
            {
                users = users.Where(a => a.Email.Contains(email));
            }
            if (endTime != null && startTime != null)
            {

                if (DateTime.TryParse(startTime, new CultureInfo("vi-VN"), DateTimeStyles.None, out var start))
                {
                    users = users.Where(a => DbFunctions.TruncateTime(a.CreateDate) >= DbFunctions.TruncateTime(start));
                }
                if (DateTime.TryParse(endTime, new CultureInfo("vi-VN"), DateTimeStyles.None, out var end))
                {
                    users = users.Where(a => DbFunctions.TruncateTime(a.CreateDate) <= DbFunctions.TruncateTime(end));
                }
            }

            if (status != null)
            {
                switch (status)
                {
                    case 1:
                        users = users.Where(a => a.Active);
                        break;
                    case 2:
                        users = users.Where(a => !a.Active);
                        break;
                }
            }
            var model = new ListMemberViewModel
            {
                Users = users.ToPagedList(pageNumber, pageSize),
                Name = name,
                Status = status,
                TypeUser = typeUser,
                StartTime = startTime,
                EndTime = endTime,
            };
            return View(model);
        }

        public ActionResult EditUser(int userId = 0, int result = 0)
        {
            var user = _unitOfWork.UserRepository.GetById(userId);
            if (user == null)
            {
                return RedirectToAction("ListUser");
            }
            var model = new AdminEditUserViewModel
            {
                Id = userId,
                Username = user.Username,
                Avatar = user.Avatar,
                CreateDate = user.CreateDate,
                EmailRegister = user.Email,
                Phone = user.Phone,
                Active = user.Active
            };
            ViewBag.Result = result;
            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult EditUser(AdminEditUserViewModel model)
        {
            var user = _unitOfWork.UserRepository.GetById(model.Id);
            if (user == null)
            {
                return RedirectToAction("ListUser");
            }

            if (ModelState.IsValid)
            {
                var isPost = true;
                var file = Request.Files["Avatar"];
                if (file != null && file.ContentLength > 0)
                {
                    if (file.ContentType != "image/jpeg" & file.ContentType != "image/png" && file.ContentType != "image/gif")
                    {
                        ModelState.AddModelError("", @"Chỉ chấp nhận định dạng jpg, png, gif, jpeg");
                    }
                    else
                    {
                        if (file.ContentLength > 4000 * 1024)
                        {
                            ModelState.AddModelError("", @"Dung lượng lớn hơn 4MB. Hãy thử lại");
                        }
                        else
                        {
                            var imgPath = "/images/user/" + DateTime.Now.ToString("yyyy/MM/dd");
                            HtmlHelpers.CreateFolder(Server.MapPath(imgPath));
                            var imgFileName = DateTime.Now.ToFileTimeUtc() + Path.GetExtension(file.FileName);

                            if (System.IO.File.Exists(Server.MapPath("/images/user/" + model.Avatar)))
                            {
                                System.IO.File.Delete(Server.MapPath("/images/user/" + model.Avatar));
                            }
                            model.Avatar = DateTime.Now.ToString("yyyy/MM/dd") + "/" + imgFileName;

                            var newImage = Image.FromStream(file.InputStream);
                            var fixSizeImage = HtmlHelpers.FixedSize(newImage, 600, 600, false);
                            HtmlHelpers.SaveJpeg(Server.MapPath(Path.Combine(imgPath, imgFileName)), fixSizeImage, 90);
                        }
                    }
                }
                if (isPost)
                {
                    if (model.Password != null)
                    {
                        user.Password = HtmlHelpers.ComputeHash(model.Password, "SHA256", null);
                    }
                    user.Phone = model.Phone;
                    user.Avatar = model.Avatar;
                    user.Active = model.Active;
                    _unitOfWork.Save();
                    return RedirectToAction("ListUser", new { result = 1 });
                }
            }
            return View(model);
        }

        [HttpPost]
        public bool DeleteUser(int userId = 0)
        {
            var user = _unitOfWork.UserRepository.GetById(userId);
            if (user == null)
            {
                return false;
            }
            var advises = _unitOfWork.AdviseRepository.GetQuery(a => a.UserId == userId).ToList();
            foreach (var advise in advises)
            {
                _unitOfWork.AdviseRepository.Delete(advise);
            }
            _unitOfWork.Save();
            _unitOfWork.UserRepository.Delete(user);
            _unitOfWork.Save();
            return true;
        }
        #endregion

        #region  Employer
        public ActionResult ListEmployer(int? page,  int? status, string email, string name)
        {

            var pageNumber = page ?? 1;
            var pageSize = 30;

            var users = _unitOfWork.EmployerRepository.GetQuery(orderBy: q => q.OrderByDescending(p => p.Id)).AsNoTracking();
            if (!string.IsNullOrEmpty(email))
            {
                users = users.Where(a => a.Email.Contains(email));
            }
            if (!string.IsNullOrEmpty(name))
            {
                users = users.Where(a => a.FullName.Contains(name));
            }
            if (status != null)
            {
                switch (status)
                {
                    case 1:
                        users = users.Where(a => a.Active);
                        break;
                    case 2:
                        users = users.Where(a => !a.Active);
                        break;
                }
            }
            var model = new ListEmployerViewModel
            {
                Employers = users.ToPagedList(pageNumber, pageSize),
                Name = name,
                Status = status,
            };
            return View(model);
        }

        public ActionResult EditEmployer(int userId = 0, int result = 0)
        {
            var user = _unitOfWork.EmployerRepository.GetById(userId);
            if (user == null)
            {
                return RedirectToAction("ListUser");
            }
            var model = new EmployerEditUserViewModel
            {
                Id = userId,
                Avatar = user.Avatar,
                CreateDate = user.CreateDate,
                EmailRegister = user.Email,
                Phone = user.PhoneNumber,
                Active = user.Active,
            };
            ViewBag.Result = result;
            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult EditEmployer(EmployerEditUserViewModel model)
        {
            var user = _unitOfWork.EmployerRepository.GetById(model.Id);
            if (user == null)
            {
                return RedirectToAction("ListEmployer");
            }

            if (ModelState.IsValid)
            {
                var isPost = true;
                var file = Request.Files["Avatar"];
                if (file != null && file.ContentLength > 0)
                {
                    if (file.ContentType != "image/jpeg" & file.ContentType != "image/png" && file.ContentType != "image/gif")
                    {
                        ModelState.AddModelError("", @"Chỉ chấp nhận định dạng jpg, png, gif, jpeg");
                    }
                    else
                    {
                        if (file.ContentLength > 4000 * 1024)
                        {
                            ModelState.AddModelError("", @"Dung lượng lớn hơn 4MB. Hãy thử lại");
                        }
                        else
                        {
                            var imgPath = "/images/employer/" + DateTime.Now.ToString("yyyy/MM/dd");
                            HtmlHelpers.CreateFolder(Server.MapPath(imgPath));
                            var imgFileName = DateTime.Now.ToFileTimeUtc() + Path.GetExtension(file.FileName);

                            if (System.IO.File.Exists(Server.MapPath("/images/employer/" + model.Avatar)))
                            {
                                System.IO.File.Delete(Server.MapPath("/images/employer/" + model.Avatar));
                            }
                            model.Avatar = DateTime.Now.ToString("yyyy/MM/dd") + "/" + imgFileName;

                            var newImage = Image.FromStream(file.InputStream);
                            var fixSizeImage = HtmlHelpers.FixedSize(newImage, 600, 600, false);
                            HtmlHelpers.SaveJpeg(Server.MapPath(Path.Combine(imgPath, imgFileName)), fixSizeImage, 90);
                        }
                    }
                }
                if (isPost)
                {
                    if (model.Password != null)
                    {
                        user.Password = HtmlHelpers.ComputeHash(model.Password, "SHA256", null);
                    }
                    user.PhoneNumber = model.Phone;
                    user.Avatar = model.Avatar;
                    user.Active = model.Active;
                    _unitOfWork.Save();
                    return RedirectToAction("ListEmployer", new { result = 1 });
                }
            }
            return View(model);
        }
        [HttpPost]
        public bool DeleteEmployer(int userId = 0)
        {
            var user = _unitOfWork.EmployerRepository.GetById(userId);
            if (user == null)
            {
                return false;
            }
            _unitOfWork.EmployerRepository.Delete(user);
            _unitOfWork.Save();
            return true;
        }

        public PartialViewResult PublicMoney(int id)
        {
            var user = _unitOfWork.EmployerRepository.GetById(id);
            var model = new PublicMoneyViewModel
            {
                Id = user.Id,
                //Price = user.Amount.ToString("N0"),
            };
            return PartialView(model);
        }
        [HttpPost]
        public ActionResult PublicMoney(PublicMoneyViewModel model)
        {
            var user = _unitOfWork.EmployerRepository.GetById(model.Id);
            if(user == null)
            {
                return PartialView(model);
            }
            if(model.Price != null)
            {
                user.Amount += Convert.ToDecimal(model.Price.Replace(",", ""));
                Utils.Utils.EmployerLog("Nạp tiền thành công <strong>" + model.Price + "đ" + " </strong> vào tài khoản" , EmployerLogType.PublicMoney , user.Id, Convert.ToDecimal(model.Price.Replace(",", "")));
                _unitOfWork.Save();
            }
            return RedirectToAction("ListEmployer");
        }

        public ActionResult History(int? page, int id , string startTime, string endTime )
        {
            var pageNumber = page ?? 1;
            var pageSize = 30;
            var log = _unitOfWork.EmployerLogRepository.GetQuery(a => a.UserId == id && a.EmployerLogType == EmployerLogType.PublicMoney, o => o.OrderByDescending(a => a.CreateDate));
            if (startTime != null)
            {
                if (DateTime.TryParse(startTime, new CultureInfo("vi-VN"), DateTimeStyles.None, out var start))
                {
                    log = log.Where(a => a.CreateDate != null && DbFunctions.TruncateTime(a.CreateDate) >= DbFunctions.TruncateTime(start));
                }
            }

            if (startTime != null && endTime != null)
            {
                if (DateTime.TryParse(startTime, new CultureInfo("vi-VN"), DateTimeStyles.None, out var start) &&
                DateTime.TryParse(endTime, new CultureInfo("vi-VN"), DateTimeStyles.None, out var end))
                {
                    log = log.Where(a => a.CreateDate != null &&
                                                     DbFunctions.TruncateTime(a.CreateDate) >= DbFunctions.TruncateTime(start) &&
                                                     DbFunctions.TruncateTime(a.CreateDate) <= DbFunctions.TruncateTime(end));
                }
            }
            var model = new HistotyEmployerViewModel{
                EmployerLogs = log.ToPagedList(pageNumber , pageSize),
                StartTime = startTime,
                EndTime = endTime,
                Id = id,
            };
            return View(model);
        }
        [HttpPost]
        public bool DeleteHistory(int id)
        {
            var log = _unitOfWork.EmployerLogRepository.GetById(id);
            if(log == null)
            {
                return false;
            }
            _unitOfWork.EmployerLogRepository.Delete(log);
            _unitOfWork.Save();
            return true;
        }
        #endregion

        #region Company
        public ActionResult ListCompany(int? page, string startTime, string endTime, string name, string email, string[] careerIds, string[] cityIds, int? status)
        {
            var pageNumber = page ?? 1;
            var pageSize = 30;
            //Khóa tài khoản
            var companies = _unitOfWork.CompanyRepository.GetQuery(orderBy: o => o.OrderByDescending(a => a.EmployerId));

            //switch (status)
            //{
            //    case 1:
            //        companies = companies.Where(a => a.User.AdminActive);
            //        break;
            //    default:
            //        companies = companies.Where(a => !a.User.AdminActive);
            //        break;
            //}

            if (!string.IsNullOrEmpty(name))
            {
                companies = companies.Where(p => p.Name.ToLower().Contains(name.ToLower()));
            }
            if (!string.IsNullOrEmpty(email))
            {
                companies = companies.Where(a => a.Email.Contains(email));
            }
            //if (DateTime.TryParse(startTime, new CultureInfo("vi-VN"), DateTimeStyles.None, out var start))
            //{
            //    companies = companies.Where(a => DbFunctions.TruncateTime(a.Employer.CreateDate) >= DbFunctions.TruncateTime(start));
            //}

            if (DateTime.TryParse(startTime, new CultureInfo("vi-VN"), DateTimeStyles.None, out var start))
            {
                companies = companies.Where(a => DbFunctions.TruncateTime(a.Employer.CreateDate) >= DbFunctions.TruncateTime(start));
            }
            if (DateTime.TryParse(endTime, new CultureInfo("vi-VN"), DateTimeStyles.None, out var end))
            {
                companies = companies.Where(a => DbFunctions.TruncateTime(a.Employer.CreateDate) >= DbFunctions.TruncateTime(end));
            }

            if (careerIds != null)
            {
                var tmp = careerIds.Select(int.Parse).Cast<int?>().ToList();
                companies = companies.Where(a => a.Careers.Any(c => tmp.Contains(c.Id)));
            }
            if (cityIds != null)
            {
                var tmp = cityIds.Select(int.Parse).Cast<int?>().ToList();
                companies = companies.Where(p => tmp.Contains(p.CityId));
            }
            var model = new AdminListCompanyViewModel
            {
                Companies = companies.ToPagedList(pageNumber, pageSize),
                Name = name,
                Email = email,
                StartTime = startTime,
                CityIds = new List<int>(),
                CareerIds = new List<int>(),
                EndTime = endTime,
                CitySelectList = CitySelectList,
                CareerSelectList = CareerSelectList,
                Status = status
            };
            if (cityIds != null)
            {
                model.CityIds = cityIds.Select(int.Parse).ToList();
            }
            if (careerIds != null)
            {
                model.CareerIds = careerIds.Select(int.Parse).ToList();
            }
            return View(model);
        }

        public ActionResult EditCompany(string result = "", int companyId = 0)
        {
            ViewBag.Result = result;
            var company = _unitOfWork.CompanyRepository.GetById(companyId);
            if (company == null)
            {
                return RedirectToAction("ListCompany");
            }
            var model = new AdminEditCompanyViewModel
            {
                Company = company,
                Ranks = _unitOfWork.RankRepository.Get(orderBy: o => o.OrderBy(a => a.Name)),
                Careers = _unitOfWork.CareerRepository.GetQuery(a => a.Active && a.TypeCareer == TypeCareer.Activity),
                Cities = _unitOfWork.CityRepository.Get(orderBy: o => o.OrderBy(a => a.Sort)),
            };
            return View(model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult EditCompany(AdminEditCompanyViewModel model, FormCollection fc)
        {
            var company = _unitOfWork.CompanyRepository.GetById(model.Company.EmployerId);
            if (company == null)
            {
                return RedirectToAction("ListCompany");
            }
            if (ModelState.IsValid)
            {
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
                company.LastEditDate = DateTime.Now;
                company.Url = HtmlHelpers.ConvertToUnSign(null, company.Url ?? company.Name);
                _unitOfWork.CompanyRepository.Update(company);
                if (company.Careers.Any())
                {
                    company.Careers.Clear();
                }
                _unitOfWork.Save();
                if (model.Date > 0)
                {
                    if (model.TruTien)
                    {
                        if (company.Employer.Amount > 0)
                        {
                            int amountToSubtract = Convert.ToInt32((ConfigSite.PriceJob ?? 30000) * model.Date);
                            string formattedAmountToSubtract = amountToSubtract.ToString("#,0") + "đ";
                            if (amountToSubtract < company.Employer.Amount)
                            {
                                company.Employer.Amount -= amountToSubtract;
                                Utils.Utils.EmployerLog("Tài khoản bị trừ <strong>" + formattedAmountToSubtract + "</strong> để hiển thị <strong> Công ty </strong>" + "<strong class='text-danger'>" + model.Date + "</strong> ngày ở mục nổi bật", EmployerLogType.Deduction, company.Employer.Id, amountToSubtract);
                                if (company.Vipdate != null)
                                {
                                    if (company.Vipdate < DateTime.Now)
                                    {
                                        company.Vipdate = DateTime.Now.AddDays(model.Date);
                                    }
                                    else
                                    {
                                        company.Vipdate = company.Vipdate.AddDays(model.Date);
                                    }
                                }
                                else
                                {
                                    company.Vipdate = DateTime.Now.AddDays(model.Date);
                                }
                            }
                            else
                            {
                                ModelState.AddModelError("", @"Số dư của tài khoản này không đủ để hiển thị tin");
                                model.Careers = _unitOfWork.CareerRepository.GetQuery(a => a.Active && a.TypeCareer == TypeCareer.Career);
                                return View(model);
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", @"Số dư của tài khoản này không đủ để hiển thị tin");
                            model.Careers = _unitOfWork.CareerRepository.GetQuery(a => a.Active && a.TypeCareer == TypeCareer.Career);
                            return View(model);
                        }
                    }
                    else
                    {
                        if (company.Vipdate != null)
                        {
                            if (company.Vipdate < DateTime.Now)
                            {
                                company.Vipdate = DateTime.Now.AddDays(model.Date);
                            }
                            else
                            {
                                company.Vipdate = company.Vipdate.AddDays(model.Date);
                            }
                        }
                        else
                        {
                            company.Vipdate = DateTime.Now.AddDays(model.Date);
                        }
                    }

                }
                _unitOfWork.Save();
                var careers = fc["career"];
                if (!string.IsNullOrEmpty(careers))
                {
                    foreach (var item in careers.Split(','))
                    {
                        var id = Convert.ToInt32(item);
                        var careerItem = _unitOfWork.CareerRepository.GetById(id);
                        company.Careers.Add(careerItem);
                    }
                    _unitOfWork.Save();
                }
                return RedirectToAction("ListCompany");
            }
            model.Careers = _unitOfWork.CareerRepository.GetQuery(a => a.Active && a.TypeCareer == TypeCareer.Career);
            return View(model);
        }
        [HttpPost]
        public bool QuickUpdateCompany(bool home, bool active, int companyId = 0)
        {
            var company = _unitOfWork.CompanyRepository.GetById(companyId);
            if (company == null)
            {
                return false;
            }
            company.ShowHome = home;
            company.Employer.Active = active;
            _unitOfWork.Save();
            return true;

        }
        [HttpPost]
        public bool DeleteCompany(int companyId = 0)
        {

            var company = _unitOfWork.CompanyRepository.GetById(companyId);
            if (company == null)
            {
                return false;
            }
            _unitOfWork.CompanyRepository.Delete(company);
            _unitOfWork.Save();
            return true;
        }
        public ActionResult UploadJobs(int? count, int result = 0)
        {
            var companies = _unitOfWork.CompanyRepository.Get();
            var model = new UploadJobViewModel
            {
                CompaniesSelectList = new SelectList(companies, "UserId", "Name"),
            };
            ViewBag.Result = result;
            ViewBag.Count = count;
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult UploadJobs(FormCollection fc)
        {
            var companyId = Convert.ToInt32(fc["CompanyId"]);
            var company = _unitOfWork.CompanyRepository.GetById(companyId);
            var cities = _unitOfWork.CityRepository.GetQuery();
            var types = _unitOfWork.JobTypeRepository.GetQuery();
            var careers = _unitOfWork.CareerRepository.GetQuery();
            var companies = _unitOfWork.CompanyRepository.GetQuery();

            var model = new UploadJobViewModel
            {
                CompaniesSelectList = new SelectList(companies, "UserId", "Name")
            };

            if (ModelState.IsValid)
            {
                var count = 0;

                var file = Request.Files["fileUploadJobs"];
                if (file != null && file.ContentLength > 0 && !string.IsNullOrEmpty(file.FileName))
                {
                    if (!HtmlHelpers.CheckFileExt(file.FileName, "xls|xlsx"))
                    {
                        ModelState.AddModelError("", @"Chấp nhận File .xls, .xlsx");
                        return View(model);
                    }
                    if (file.ContentLength > 100 * 1024 * 1024)
                    {
                        ModelState.AddModelError("", @"File lớn hơn 100MB, vui lòng thử lại");
                        return View(model);
                    }
                    return RedirectToAction("UploadJobs", new { result = 1, count });
                }
            }

            return View(model);
        }
        #endregion

        #region Candidate
        public ActionResult ListCandidate(int? page, string startTime, string endTime, string name, int? status, string email, string[] careerIds, string[] cityIds, string[] jobTypeIds, string[] skillIds, string[] rankIds)
        {
            var pageNumber = page ?? 1;
            var pageSize = 30;
            var candidates = _unitOfWork.CandidateRepository.GetQuery(a => !a.User.Active, o => o.OrderByDescending(a => a.UserId));
            if (!string.IsNullOrEmpty(name))
            {
                candidates = candidates.Where(p => p.Name.ToLower().Contains(name.ToLower()));
            }
            if (!string.IsNullOrEmpty(email))
            {
                candidates = candidates.Where(a => a.Email.Contains(email));
            }
            if (endTime != null && startTime != null)
            {
                if (DateTime.TryParse(startTime, new CultureInfo("vi-VN"), DateTimeStyles.None, out var start))
                {
                    candidates = candidates.Where(a => DbFunctions.TruncateTime(a.User.CreateDate) >= DbFunctions.TruncateTime(start));
                }
                if (DateTime.TryParse(endTime, new CultureInfo("vi-VN"), DateTimeStyles.None, out var end))
                {
                    candidates = candidates.Where(a => DbFunctions.TruncateTime(a.User.CreateDate) <= DbFunctions.TruncateTime(end));
                }
            }
            if (careerIds != null)
            {
                var tmp = careerIds.Select(int.Parse).Cast<int?>().ToList();
                candidates = candidates.Where(a => a.Careers.Any(c => tmp.Contains(c.Id)));
            }
            if (skillIds != null)
            {
                var tmp = skillIds.Select(int.Parse).Cast<int?>().ToList();
                candidates = candidates.Where(a => a.Skills.Any(c => tmp.Contains(c.Id)));
            }
            if (jobTypeIds != null)
            {
                var tmp = jobTypeIds.Select(int.Parse).Cast<int?>().ToList();
                candidates = candidates.Where(a => a.JobTypes.Any(c => tmp.Contains(c.Id)));
            }
            if (rankIds != null)
            {
                var tmp = rankIds.Select(int.Parse).Cast<int?>().ToList();
                candidates = candidates.Where(a => a.Ranks.Any(c => tmp.Contains(c.Id)));
            }
            if (cityIds != null)
            {
                var tmp = cityIds.Select(int.Parse).Cast<int?>().ToList();
                candidates = candidates.Where(a => a.Citys.Any(c => tmp.Contains(c.Id)));
            }
            switch (status)
            {
                case 1:
                    candidates = candidates.Where(a => a.ActiveProfile);
                    break;
                case 2:
                    candidates = candidates.Where(a => !a.ActiveProfile);
                    break;
            }
            var model = new AdminListCandidateViewModel
            {
                Candidates = candidates.ToPagedList(pageNumber, pageSize),
                RankSelectList = RankSelectList,
                SkillSelectList = SkillSelectList,
                JobTypeSelectList = JobTypeSelectList,
                CareerSelectList = CareerSelectList,
                CitySelectList = CitySelectList,
                CareerIds = new List<int>(),
                CityIds = new List<int>(),
                SkillIds = new List<int>(),
                RankIds = new List<int>(),
                JobTypeIds = new List<int>(),
                Name = name,
                Email = email,
                Status = status,
                StartTime = startTime,
                EndTime = endTime,
            };
            if (careerIds != null)
            {
                model.CareerIds = careerIds.Select(int.Parse).ToList();
            }
            if (cityIds != null)
            {
                model.CityIds = cityIds.Select(int.Parse).ToList();
            }
            if (rankIds != null)
            {
                model.RankIds = rankIds.Select(int.Parse).ToList();
            }
            if (skillIds != null)
            {
                model.SkillIds = skillIds.Select(int.Parse).ToList();
            }
            if (jobTypeIds != null)
            {
                model.JobTypeIds = jobTypeIds.Select(int.Parse).ToList();
            }
            return View(model);
        }

        public ActionResult PreviewCanidate(int candidateId = 0)
        {
            var candidate = _unitOfWork.CandidateRepository.GetById(candidateId);
            if (candidate == null)
            {
                return RedirectToAction("ListCandidate");
            }
            return View(candidate);
        }

        public ActionResult EditCandidate(string result = "", int candidateId = 0)
        {
            ViewBag.Result = result;
            var candidate = _unitOfWork.CandidateRepository.GetById(candidateId);
            if (candidate == null)
            {
                return RedirectToAction("ListCandidate");
            }
            var model = new AdminEditCandidateViewModel
            {
                Candidate = candidate,
                JobTypeSelectList = JobTypeSelectList,
                CareerSelectList = CareerSelectList,
                CitySelectList = CitySelectList,
                RankSelectList = RankSelectList,
            };

            return View(model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult EditCandidate(AdminEditCandidateViewModel model, FormCollection fc)
        {
            var candidate = _unitOfWork.CandidateRepository.GetById(model.Candidate.UserId);
            if (candidate == null)
            {
                return RedirectToAction("ListCandidate");
            }
            if (ModelState.IsValid)
            {
                var isPost = true;

                if (candidate.User.Username != model.Candidate.User.Username)
                {
                    var checkExits = _unitOfWork.UserRepository
                        .GetQuery(a => a.Username == model.Candidate.User.Username).FirstOrDefault();

                    if (checkExits != null)
                    {
                        ModelState.AddModelError("", @"Tên đăng nhập đã sử dụng rồi.");
                        isPost = false;
                    }
                    else
                    {
                        candidate.User.Username = model.Candidate.User.Username;
                    }
                }
                if (candidate.User.Email != model.Candidate.User.Email)
                {
                    var checkExits = _unitOfWork.UserRepository.GetQuery(a => a.Email == model.Candidate.User.Email).FirstOrDefault();

                    if (checkExits != null)
                    {
                        ModelState.AddModelError("", @"Email đã sử dụng rồi.");
                        isPost = false;
                    }
                    else
                    {
                        candidate.User.Email = model.Candidate.User.Email;
                    }
                }

                if (isPost)
                {
                    var file = Request.Files["Candidate.User.Avatar"];
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
                                var imgPath = "/images/user/" + DateTime.Now.ToString("yyyy/MM/dd");
                                HtmlHelpers.CreateFolder(Server.MapPath(imgPath));
                                var imgFileName = DateTime.Now.ToFileTimeUtc() + Path.GetExtension(file.FileName);

                                candidate.User.Avatar = DateTime.Now.ToString("yyyy/MM/dd") + "/" + imgFileName;

                                var newImage = Image.FromStream(file.InputStream);
                                var fixSizeImage = HtmlHelpers.FixedSize(newImage, 600, 600, false);
                                HtmlHelpers.SaveJpeg(Server.MapPath(Path.Combine(imgPath, imgFileName)), fixSizeImage, 90);
                                file.SaveAs(Server.MapPath(Path.Combine(imgPath, imgFileName)));

                            }
                        }

                    }

                    if (isPost)
                    {
                        var isCv = true;
                        var fileCv = Request.Files["Candidate.FileCv"];
                        if (fileCv != null && fileCv.ContentLength > 0)
                        {
                            if (fileCv.ContentType != "application/pdf")
                            {
                                isCv = false;
                                ModelState.AddModelError("", @"Chỉ chấp nhận định dạng pdf");
                            }
                            else
                            {
                                if (fileCv.ContentLength > 4000 * 1024)
                                {
                                    isCv = false;
                                    ModelState.AddModelError("", @"Dung lượng lớn hơn 4MB. Hãy thử lại");
                                }
                                else
                                {
                                    var CvPath = "/fileCv/" + DateTime.Now.ToString("yyyy/MM/dd");
                                    HtmlHelpers.CreateFolder(Server.MapPath(CvPath));
                                    var CvFileName = HtmlHelpers.ConvertToUnSign(null, Path.GetFileNameWithoutExtension(fileCv.FileName)) + "-" + DateTime.Now.Millisecond + Path.GetExtension(fileCv.FileName);

                                    candidate.FileCv = DateTime.Now.ToString("yyyy/MM/dd") + "/" + CvFileName;
                                    fileCv.SaveAs(Server.MapPath(Path.Combine(CvPath, CvFileName)));
                                }
                            }

                        }
                        else
                        {
                            candidate.FileCv = fc["CurrentFile"] == "" ? null : fc["CurrentFile"];
                        }
                        if (isCv)
                        {
                            var bd = fc["DateOfBirth"];
                            if (DateTime.TryParse(bd, new CultureInfo("vi-VN"), DateTimeStyles.None, out var bid))
                            {
                                candidate.DateOfBirth = bid;
                            }

                            if (model.Password != null)
                            {
                                candidate.User.Password = HtmlHelpers.ComputeHash(model.Password, "SHA256", null);
                            }
                            candidate.Name = model.Candidate.Name;
                            candidate.Experience = model.Candidate.Experience;
                            candidate.Genders = model.Candidate.Genders;
                            candidate.CareerTitle = model.Candidate.CareerTitle;
                            candidate.Introduce = model.Candidate.Introduce;
                            candidate.Address = model.Candidate.Address;
                            candidate.ActiveProfile = Convert.ToBoolean(fc["ActiveProfile"]);
                            candidate.Education = model.Candidate.Education;
                            candidate.Experience = model.Candidate.Experience;
                            candidate.SalalyMin = model.Candidate.SalalyMin;
                            //candidate.User.Linkedin = model.Candidate.User.Linkedin;
                            //candidate.User.Facebook = model.Candidate.User.Facebook;
                            //candidate.User.Twitter = model.Candidate.User.Twitter;
                            //candidate.User.Youtube = model.Candidate.User.Youtube;
                            //candidate.User.Zalo = model.Candidate.User.Zalo;
                            //candidate.User.Instagram = model.Candidate.User.Instagram;
                            candidate.User.Phone = model.Candidate.User.Phone;
                            candidate.Citys.Clear();
                            candidate.Ranks.Clear();
                            candidate.Careers.Clear();
                            candidate.JobTypes.Clear();
                            var careers = fc["career"];
                            if (!string.IsNullOrEmpty(careers))
                            {
                                var listCareer = careers.Split((',')).Select(int.Parse).ToList();
                                foreach (var item in listCareer)
                                {
                                    var careerItem = _unitOfWork.CareerRepository.GetById(item);
                                    candidate.Careers.Add(careerItem);
                                }
                            }
                            var ranks = fc["rank"];
                            if (!string.IsNullOrEmpty(ranks))
                            {
                                var listRank = ranks.Split((',')).Select(int.Parse).ToList();
                                foreach (var item in listRank)
                                {
                                    var rankItem = _unitOfWork.RankRepository.GetById(item);
                                    candidate.Ranks.Add(rankItem);
                                }
                            }
                            var citys = fc["city"];
                            if (!string.IsNullOrEmpty(citys))
                            {
                                var listCity = citys.Split((',')).Select(int.Parse).ToList();
                                foreach (var item in listCity)
                                {
                                    var cityItem = _unitOfWork.CityRepository.GetById(item);
                                    candidate.Citys.Add(cityItem);
                                }
                            }
                            var types = fc["type"];
                            if (!string.IsNullOrEmpty(types))
                            {
                                var listtype = types.Split((',')).Select(int.Parse).ToList();
                                foreach (var item in listtype)
                                {
                                    var typeItem = _unitOfWork.JobTypeRepository.GetById(item);
                                    candidate.JobTypes.Add(typeItem);
                                }
                            }

                            candidate.Url = HtmlHelpers.ConvertToUnSign(null, model.Candidate.Name);
                            var count = _unitOfWork.CandidateRepository.GetQuery(a => a.Url == candidate.Url).Count();
                            if (count > 1)
                            {
                                candidate.Url += "-" + candidate.UserId;
                            }
                            _unitOfWork.Save();
                            return RedirectToAction("EditCandidate", new { candidateId = candidate.UserId, result = "success" });
                        }
                    }
                }
            }
            model.RankSelectList = RankSelectList;
            model.CitySelectList = CitySelectList;
            model.CareerSelectList = CareerSelectList;
            model.RankSelectList = RankSelectList;
            return View(model);
        }

        [HttpPost]
        public bool QuickUpdateCandidate(bool activeProfile, int candidateId = 0)
        {
            var candidate = _unitOfWork.CandidateRepository.GetById(candidateId);
            if (candidate == null)
            {
                return false;
            }
            candidate.ActiveProfile = activeProfile;
            _unitOfWork.Save();
            return true;

        }

        [HttpPost]
        public bool DeleteCandidate(int candidateId = 0)
        {

            var candidate = _unitOfWork.CandidateRepository.GetById(candidateId);
            if (candidate == null)
            {
                return false;
            }
            _unitOfWork.CandidateRepository.Delete(candidate);
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