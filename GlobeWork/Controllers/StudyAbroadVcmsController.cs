using GlobeWork.DAL;
using GlobeWork.Models;
using GlobeWork.ViewModel;
using PagedList;
using Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Web.Security;

namespace GlobeWork.Controllers
{
    [Authorize]
    public class StudyAbroadVcmsController : BaseController
    {
        public ConfigSite ConfigSite => (ConfigSite)HttpContext.Application["ConfigSite"];
        private IEnumerable<StudyAbroadCategory> StudyAbroadCategories() => _unitOfWork.StudyAbroadCategoryRepository.GetQuery(a => a.Active, o => o.OrderBy(a => a.Sort));
        #region StudyAbroad
        public ActionResult ListStudyAbroad(string result, int? page, string EndTime, string careerIds, string StartTime, int? status, int? statusTime, string name, string sort = "date-asc")
        {
            var pageNumber = page ?? 1;
            var pageSize = 30;
            var study = _unitOfWork.StudyAbroadRepository.GetQuery(orderBy: a => a.OrderByDescending(l => l.CreateDate));
            if (!string.IsNullOrEmpty(result))
            {
                ViewBag.Result = result;
            }
            if (status != null)
            {
                switch (status)
                {
                    case 1:
                        study = study.Where(a => a.Active);
                        break;
                    case 2:
                        study = study.Where(a => !a.Active);
                        break;
                }
            }
            if (statusTime != null)
            {
                switch (statusTime)
                {
                    case 0:
                        study = study.Where(a => a.ExpirationDate < DateTime.Now);
                        break;
                    case 1:
                        study = study.Where(a => a.ExpirationDate > DateTime.Now || a.ExpirationDate == null);
                        break;
                }
            }
            if (StartTime != null)
            {
                if (DateTime.TryParse(StartTime, new CultureInfo("vi-VN"), DateTimeStyles.None, out var start))
                {
                    study = study.Where(a => a.Hot != null && DbFunctions.TruncateTime(a.Hot) >= DbFunctions.TruncateTime(start));
                }
            }
            if (EndTime != null && StartTime != null)
            {

                if (DateTime.TryParse(StartTime, new CultureInfo("vi-VN"), DateTimeStyles.None, out var start))
                {
                    study = study.Where(a => a.Hot != null && DbFunctions.TruncateTime(a.Hot) >= DbFunctions.TruncateTime(start));
                }
                if (DateTime.TryParse(EndTime, new CultureInfo("vi-VN"), DateTimeStyles.None, out var end))
                {
                    study = study.Where(a => a.Hot != null && DbFunctions.TruncateTime(a.Hot) <= DbFunctions.TruncateTime(end));
                }
            }

            switch (sort)
            {
                case "sort-asc":
                    study = study.OrderBy(a => a.Id);
                    break;
                case "sort-desc":
                    study = study.OrderByDescending(a => a.Id);
                    break;
                case "date-desc":
                    study = study.OrderBy(a => a.CreateDate);
                    break;
                case "date-asc":
                    study = study.OrderByDescending(a => a.CreateDate);
                    break;
                case "name":
                    study = study.OrderBy(a => a.Name);
                    break;
                default:
                    study = study.OrderByDescending(a => a.CreateDate);
                    break;
            }
            var model = new ListStudyVcmsViewModel
            {
                StudyAbroads = study.ToPagedList(pageNumber, pageSize),
                RankSelectList = RankSelectList,
                SkillSelectList = SkillSelectList,
                JobTypeSelectList = JobTypeSelectList,
                Careers = Careers,
                CitySelectList = CitySelectList,
                CareerIds = careerIds,
                Name = name,
                Sort = sort,
                Status = status,
                StatusTime = statusTime,
                StartTime = StartTime,
                EndTime = EndTime,
                Countries = _unitOfWork.CountryRepository.Get(orderBy: a => a.OrderByDescending(l => l.Name)),
            };
            return View(model);
        }

        public ActionResult UpdateStudyAbroadVcms(int Id = 0, string result = "")
        {
            ViewBag.Result = result;
            var study = _unitOfWork.StudyAbroadRepository.GetById(Id);
            if (study == null)
            {
                return RedirectToAction("ListStudyAbroad");
            }
            var model = new CreateStudyVcmsViewModel
            {
                StudyAbroad = study,
                //DateHot = 0,
                Careers = _unitOfWork.CareerRepository.GetQuery(a => a.Active, o => o.OrderByDescending(a => a.CreateDate)),
                StudyAbroadCategories = StudyAbroadCategories(),
                Citys = _unitOfWork.CityRepository.Get(a => a.Active, q => q.OrderBy(a => a.Sort)),
            };
            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult UpdateStudyAbroadVcms(CreateStudyVcmsViewModel model, FormCollection fc)
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
                study.Url = HtmlHelpers.ConvertToUnSign(null, model.StudyAbroad.Name);
                study.CategoryId = Convert.ToInt32(fc["CategoryId"]);
                study.CareerId = Convert.ToInt32(fc["CareerId"]);
                study.StudyAbroadCategory = _unitOfWork.StudyAbroadCategoryRepository.GetById(study.CategoryId) ?? null;
                study.Careers = _unitOfWork.CareerRepository.GetById(study.CareerId) ?? null;
                study.Name = model.StudyAbroad.Name;
                study.WageScholarship = model.StudyAbroad.WageScholarship;
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

                if (model.Date > 0)
                {
                    if (model.TruTien)
                    {
                        if (study.Company.Employer.Amount > 0)
                        {
                            int amountToSubtract = Convert.ToInt32((ConfigSite.PriceJob ?? 30000) * model.Date);
                            string formattedAmountToSubtract = amountToSubtract.ToString("#,0") + "đ";
                            if (amountToSubtract < study.Company.Employer.Amount)
                            {
                                study.Company.Employer.Amount -= amountToSubtract;
                                Utils.Utils.EmployerLog("Tài khoản bị trừ <strong>" + formattedAmountToSubtract + "</strong> để hiển thị tin <strong>" + study.Code + " </strong>" + "<strong class='text-danger'>" + model.Date + "</strong> ngày ở mục nổi bật", EmployerLogType.Deduction, study.Company.Employer.Id, amountToSubtract);
                                if (study.Hot != null)
                                {
                                    if (study.Hot < DateTime.Now)
                                    {
                                        study.Hot = DateTime.Now.AddDays(model.Date);
                                    }
                                    else
                                    {
                                        study.Hot = study.Hot?.AddDays(model.Date);
                                    }
                                }
                                else
                                {
                                    study.Hot = DateTime.Now.AddDays(model.Date);
                                }
                            }
                            else
                            {
                                ModelState.AddModelError("", @"Số dư của tài khoản này không đủ để hiển thị tin");
                                model.StudyAbroad = study;
                                model.Careers = _unitOfWork.CareerRepository.GetQuery(a => a.Active, o => o.OrderByDescending(a => a.CreateDate));
                                model.StudyAbroadCategories = StudyAbroadCategories();
                                model.Citys = _unitOfWork.CityRepository.Get(a => a.Active, q => q.OrderBy(a => a.Sort));
                                return View(model);
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", @"Số dư của tài khoản này không đủ để hiển thị tin");
                            model.Careers = _unitOfWork.CareerRepository.GetQuery(a => a.Active, o => o.OrderByDescending(a => a.CreateDate));
                            model.StudyAbroadCategories = StudyAbroadCategories();
                            model.Citys = _unitOfWork.CityRepository.Get(a => a.Active, q => q.OrderBy(a => a.Sort));
                            return View(model);
                        }
                    }
                    else
                    {
                        if (study.Hot != null)
                        {
                            if (study.Hot < DateTime.Now)
                            {
                                study.Hot = DateTime.Now.AddDays(model.Date);
                            }
                            else
                            {
                                study.Hot = study.Hot?.AddDays(model.Date);
                            }
                        }
                        else
                        {
                            study.Hot = DateTime.Now.AddDays(model.Date);
                        }
                    }

                }
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
        public bool QuickUpdateStudyAbroad(bool active, bool home, bool special, int Id = 0)
        {
            var study = _unitOfWork.StudyAbroadRepository.GetById(Id);
            if (study == null)
            {
                return false;
            }
            study.Active = active;
            _unitOfWork.Save();
            return true;
        }

        [HttpPost]
        public bool DeleteStudyAbroad(int id = 0)
        {
            var study = _unitOfWork.StudyAbroadRepository.GetById(id);
            if (study == null)
            {
                return  false;
            }

            var apply = _unitOfWork.ApplyJobRepository.GetQuery(a => a.StudyAbroadId == study.Id).ToList();
            var adv = _unitOfWork.AdviseRepository.GetQuery(a => a.StudyAbroadId == study.Id);
            foreach (var item in adv)
            {
                _unitOfWork.AdviseRepository.Delete(item);
            }
            foreach (var item in apply)
            {
                _unitOfWork.ApplyJobRepository.Delete(item);
            }

            _unitOfWork.StudyAbroadRepository.Delete(study);
            _unitOfWork.Save();
            return true;
        }

        #endregion

        public ActionResult ListApplyJob(int? page, string startTime, string endTime, string keyword, string sort, int? status, string submit)
        {
            var pageNumber = page ?? 1;
            var pageSize = 30;
            var applyJobs = _unitOfWork.ApplyJobRepository.GetQuery(a => a.StudyAbroadId != null, o => o.OrderByDescending(l => l.CreateDate));
            var count = applyJobs.Count();
            if (!string.IsNullOrEmpty(keyword))
            {
                applyJobs = applyJobs.Where(a => a.User.FullName.ToLower().Trim().Contains(keyword.ToLower().Trim()));
            }
            if (DateTime.TryParse(startTime, new CultureInfo("vi-VN"), DateTimeStyles.None, out var start))
            {
                applyJobs = applyJobs.Where(a => DbFunctions.TruncateTime(a.CreateDate) >= DbFunctions.TruncateTime(start));
            }
            if (DateTime.TryParse(endTime, new CultureInfo("vi-VN"), DateTimeStyles.None, out var end))
            {
                applyJobs = applyJobs.Where(a => DbFunctions.TruncateTime(a.CreateDate) <= DbFunctions.TruncateTime(end));
            }
            switch (status)
            {
                case 0:
                    applyJobs = applyJobs.Where(a => a.Status == ApplyJobStatus.Waiting);
                    break;
                case 1:
                    applyJobs = applyJobs.Where(a => a.Status == ApplyJobStatus.View);
                    break;
                case 2:
                    applyJobs = applyJobs.Where(a => a.Status == ApplyJobStatus.NoActive);
                    break;
                case 3:
                    applyJobs = applyJobs.Where(a => a.Status == ApplyJobStatus.NoActive);
                    break;
            }

            if (submit == "delete")
            {
                _unitOfWork.ApplyJobRepository.Delete(applyJobs);
                return RedirectToAction("ListApplyJob");
            }

            switch (sort)
            {
                case "sort-asc":
                    applyJobs = applyJobs.OrderBy(a => a.Id);
                    break;
                case "sort-desc":
                    applyJobs = applyJobs.OrderByDescending(a => a.Id);
                    break;
                case "date-asc":
                    applyJobs = applyJobs.OrderBy(a => a.CreateDate);
                    break;
                case "date-desc":
                    applyJobs = applyJobs.OrderByDescending(a => a.CreateDate);
                    break;
                case "name":
                    applyJobs = applyJobs.OrderBy(a => a.User.FullName);
                    break;
                default:
                    applyJobs = applyJobs.OrderByDescending(a => a.CreateDate);
                    break;
            }
            var model = new ListApplyJobViewModel
            {
                ApplyJobs = applyJobs.ToPagedList(pageNumber, pageSize),
                Keyword = keyword,
                Sort = sort,
                Status = status,
                StartTime = startTime,
                EndTime = endTime,
            };
            return View(model);
        }
        public ActionResult PreviewCanidate(int candidateId = 0)
        {
            var candidate = _unitOfWork.ApplyJobRepository.GetById(candidateId);
            if (candidate == null)
            {
                return RedirectToAction("ListCandidate", "UserVcms");
            }
            return View(candidate);
        }

        [HttpPost]
        public bool DeleteApplyJob(int applyJobId = 0)
        {

            var applyJob = _unitOfWork.ApplyJobRepository.GetById(applyJobId);
            if (applyJob == null)
            {
                return false;
            }
            _unitOfWork.ApplyJobRepository.Delete(applyJob);
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