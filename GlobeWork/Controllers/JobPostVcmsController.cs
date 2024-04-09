using Helpers;
using GlobeWork.Filters;
using GlobeWork.Models;
using GlobeWork.ViewModel;
using PagedList;
using System;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using GlobeWork.Controllers;
using System.Web.Security;
using System.Web;

namespace GlobeWork.Controllers
{
    [Authorize]
    public class JobPostVcmsController : BaseController
    {
        public ConfigSite ConfigSite => (ConfigSite)HttpContext.Application["ConfigSite"];
        #region JobPost
        public ActionResult ListJobPost(string result, int? page, string endTime, int? careerId, int? cityIds, string jobTypeIds, string skillIds, string rankIds, string startTime, int? status, int? statusTime, string name, string sort = "date-asc")
        {
            var pageNumber = page ?? 1;
            var pageSize = 30;
            var jobPosts = _unitOfWork.JobPostRepository.GetQuery(orderBy: q => q.OrderByDescending(a => a.CreateDate));
            if (!string.IsNullOrEmpty(result))
            {
                ViewBag.Result = result;
            }
            if (status != null)
            {
                switch (status)
                {
                    case 1:
                        jobPosts = jobPosts.Where(a => a.Active);
                        break;
                    case 2:
                        jobPosts = jobPosts.Where(a => !a.Active);
                        break;
                }
            }
            if (!string.IsNullOrEmpty(name))
            {
                jobPosts = jobPosts.Where(a => a.Name.Contains(name));
            }
            if (startTime != null)
            {
                if (DateTime.TryParse(startTime, new CultureInfo("vi-VN"), DateTimeStyles.None, out var start))
                {
                    jobPosts = jobPosts.Where(a => a.Hot != null && DbFunctions.TruncateTime(a.Hot) >= DbFunctions.TruncateTime(start));
                }
            }

            if (startTime != null && endTime != null)
            {
                if (DateTime.TryParse(startTime, new CultureInfo("vi-VN"), DateTimeStyles.None, out var start) &&
                DateTime.TryParse(endTime, new CultureInfo("vi-VN"), DateTimeStyles.None, out var end))
                {
                    jobPosts = jobPosts.Where(a => a.Hot != null &&
                                                     DbFunctions.TruncateTime(a.Hot) >= DbFunctions.TruncateTime(start) &&
                                                     DbFunctions.TruncateTime(a.Hot) <= DbFunctions.TruncateTime(end));
                }
            }
            if (statusTime != null)
            {
                switch (statusTime)
                {
                    case 0:
                        jobPosts = jobPosts.Where(a => a.ExpirationDate > DateTime.Now || a.ExpirationDate == null);
                        break;
                    case 1:
                        jobPosts = jobPosts.Where(a => a.ExpirationDate < DateTime.Now);
                        break;
                }
            }
            switch (sort)
            {
                case "sort-asc":
                    jobPosts = jobPosts.OrderBy(a => a.Id);
                    break;
                case "sort-desc":
                    jobPosts = jobPosts.OrderByDescending(a => a.Id);
                    break;
                case "date-desc":
                    jobPosts = jobPosts.OrderBy(a => a.CreateDate);
                    break;
                case "date-asc":
                    jobPosts = jobPosts.OrderByDescending(a => a.CreateDate);
                    break;
                case "name":
                    jobPosts = jobPosts.OrderBy(a => a.Name);
                    break;
                default:
                    jobPosts = jobPosts.OrderByDescending(a => a.CreateDate);
                    break;
            }

            if (!string.IsNullOrEmpty(skillIds))
            {
                var tmp = skillIds.Split(',').Select(int.Parse).Cast<int?>().ToList();
                jobPosts = jobPosts.Where(a => a.Skills.Any(c => tmp.Contains(c.Id)));
            }

            if (!string.IsNullOrEmpty(jobTypeIds))
            {
                var tmp = jobTypeIds.Split(',').Select(int.Parse).Cast<int?>().ToList();
                jobPosts = jobPosts.Where(a => tmp.Contains(a.JobTypeId));
            }

            if (!string.IsNullOrEmpty(rankIds))
            {
                var tmp = rankIds.Split(',').Select(int.Parse).Cast<int?>().ToList();
                jobPosts = jobPosts.Where(a => tmp.Contains(a.RankId));
            }

            if (cityIds != null)
            {
                jobPosts = jobPosts.Where(a => a.CityId == cityIds);
            }
            if (careerId != null)
            {
                jobPosts = jobPosts.Where(a => a.CareerId == careerId);
            }

            var model = new ListJobPostViewModel
            {
                JobPosts = jobPosts.ToPagedList(pageNumber, pageSize),
                RankSelectList = RankSelectList,
                SkillSelectList = SkillSelectList,
                JobTypeSelectList = JobTypeSelectList,
                Careers = Careers,
                CitySelectList = CitySelectList,
                Cities = Cities,
                CareerIds = careerId,
                CityIds = cityIds,
                SkillIds = skillIds,
                RankIds = rankIds,
                JobTypeIds = jobTypeIds,
                Name = name,
                Sort = sort,
                Status = status,
                StatusTime = statusTime,
                StartTime = startTime,
                EndTime = endTime,
            };
            //if (careerIds != null)
            //{
            //    model.CareerIds = careerIds.Select(int.Parse).ToList();
            //}
            //if (cityIds != null)
            //{
            //    model.CityIds = cityIds.Select(int.Parse).ToList();
            //}
            //if (rankIds != null)
            //{
            //    model.RankIds = rankIds.Select(int.Parse).ToList();
            //}
            //if (skillIds != null)
            //{
            //    model.SkillIds = skillIds.Select(int.Parse).ToList();
            //}
            //if (jobTypeIds != null)
            //{
            //    model.JobTypeIds = jobTypeIds.Select(int.Parse).ToList();
            //}
            return View(model);
        }

        public ActionResult UpdateJobPostVcms(int jobPostId = 0, string result = "")
        {
            ViewBag.Result = result;
            var jobPost = _unitOfWork.JobPostRepository.GetById(jobPostId);
            if (jobPost == null)
            {
                return RedirectToAction("ListJobPost", "JobPostVcms");
            }
            var model = new CreatePostViewModel
            {
                CitySelectList = CitySelectList,
                Careers = _unitOfWork.CareerRepository.GetQuery(a => a.Active && a.TypeCareer == TypeCareer.Career),
                JobTypeSelectList = JobTypeSelectList,
                SkillSelectList = SkillSelectList,
                RankSelectList = RankSelectList,
                JobPost = jobPost,
                Countries = _unitOfWork.CountryRepository.GetQuery(a => a.Active, o => o.OrderBy(a => a.Sort)),
                Date = 0,
            };
            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult UpdateJobPostVcms(CreatePostViewModel model, FormCollection fc)
        {
            var jobs = _unitOfWork.JobPostRepository.GetById(model.JobPost.Id);
            if (jobs == null)
            {
                return RedirectToAction("ListJobPost");
            }
            if (ModelState.IsValid)
            {
                jobs.Url = HtmlHelpers.ConvertToUnSign(null, jobs.Name);
                jobs.RankId = Convert.ToInt32(fc["RankId"]);
                jobs.CareerId = Convert.ToInt32(fc["CareerId"]);
                jobs.JobTypeId = Convert.ToInt32(fc["JobTypeId"]);
                jobs.Image = fc["Pictures"] == "" ? null : fc["Pictures"];
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
                jobs.CounId = model.JobPost.CounId;
                jobs.Country = _unitOfWork.CountryRepository.GetById(model.JobPost.CounId);
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
                var job = _unitOfWork.JobPostRepository.GetQuery().AsNoTracking();
                if (job.Any(p => p.Url.ToLower().Trim() == jobs.Url.ToLower().Trim()))
                {
                    jobs.Url += "-" + jobs.Id;
                }
                _unitOfWork.Save();
                
                if (model.Date > 0)
                {
                    if (model.TruTien)
                    {
                        if (jobs.Company.Employer.Amount > 0)
                        {
                            int amountToSubtract = Convert.ToInt32((ConfigSite.PriceJob ?? 30000) * model.Date);
                            string formattedAmountToSubtract = amountToSubtract.ToString("#,0") + "đ";
                            if (amountToSubtract < jobs.Company.Employer.Amount)
                            {
                                jobs.Company.Employer.Amount -= amountToSubtract;
                                Utils.Utils.EmployerLog("Tài khoản bị trừ <strong>" + formattedAmountToSubtract + "</strong> để hiển thị tin <strong>" + jobs.Code + " </strong>" + "<strong class='text-danger'>" + model.Date + "</strong> ngày ở mục nổi bật", EmployerLogType.Deduction, jobs.Company.Employer.Id, amountToSubtract);
                                if (jobs.Hot != null)
                                {
                                    if (jobs.Hot < DateTime.Now)
                                    {
                                        jobs.Hot = DateTime.Now.AddDays(model.Date);
                                    }
                                    else
                                    {
                                        jobs.Hot = jobs.Hot?.AddDays(model.Date);
                                    }
                                }
                                else
                                {
                                    jobs.Hot = DateTime.Now.AddDays(model.Date);
                                }
                            }
                            else
                            {
                                ModelState.AddModelError("", @"Số dư của tài khoản này không đủ để hiển thị tin");
                                model.Careers = _unitOfWork.CareerRepository.GetQuery(a => a.Active, o => o.OrderByDescending(a => a.CreateDate));
                                model.JobPost = jobs;
                                model.JobTypeSelectList = JobTypeSelectList;
                                model.SkillSelectList = SkillSelectList;
                                model.RankSelectList = RankSelectList;
                                model.CitySelectList = CitySelectList;
                                return View(model);
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", @"Số dư của tài khoản này không đủ để hiển thị tin");
                            model.Careers = _unitOfWork.CareerRepository.GetQuery(a => a.Active && a.TypeCareer == TypeCareer.Career, o => o.OrderByDescending(a => a.CreateDate));
                            model.JobPost = jobs;
                            model.JobTypeSelectList = JobTypeSelectList;
                            model.SkillSelectList = SkillSelectList;
                            model.RankSelectList = RankSelectList;
                            model.CitySelectList = CitySelectList;
                            return View(model);
                        }
                    }
                    else
                    {
                        if (jobs.Hot != null)
                        {
                            if (jobs.Hot < DateTime.Now)
                            {
                                jobs.Hot = DateTime.Now.AddDays(model.Date);
                            }
                            else
                            {
                                jobs.Hot = jobs.Hot?.AddDays(model.Date);
                            }
                        }
                        else
                        {
                            jobs.Hot = DateTime.Now.AddDays(model.Date);
                        }
                    }
                    
                }
                _unitOfWork.Save();
                return RedirectToAction("ListJobPost");
            }
            model.Careers = _unitOfWork.CareerRepository.GetQuery(a => a.Active && a.TypeCareer == TypeCareer.Career, o => o.OrderByDescending(a => a.CreateDate));
            model.JobPost = jobs;
            model.JobTypeSelectList = JobTypeSelectList;
            model.SkillSelectList = SkillSelectList;
            model.RankSelectList = RankSelectList;
            return View(model);
        }

        [HttpPost]
        public bool QuickUpdateJobPost(bool active, bool home, bool special, int jobPostId = 0)
        {
            var jobPost = _unitOfWork.JobPostRepository.GetById(jobPostId);
            if (jobPost == null)
            {
                return false;
            }
            jobPost.Active = active;
            //jobPost.Special = special;
            //jobPost.ShowHome = home;
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
            var apply = _unitOfWork.ApplyJobRepository.GetQuery(a => a.JobPostId == job.Id);
            foreach(var item in apply)
            {
                _unitOfWork.ApplyJobRepository.Delete(item);
            }
            _unitOfWork.JobPostRepository.Delete(job);
            _unitOfWork.Save();
            return true;
        }
        #endregion

        #region AppLyJob
        public ActionResult ListApplyJob(int? page, string startTime, string endTime, string keyword, string sort, int? status, string submit)
        {
            var pageNumber = page ?? 1;
            var pageSize = 30;
            var applyJobs = _unitOfWork.ApplyJobRepository.GetQuery(a => a.JobPostId != null, o => o.OrderByDescending(l => l.CreateDate));

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

        public ActionResult ListApplyByJobPost(int jobPostId, int? page, string startTime, string endTime, string keyword, string sort, int? status)
        {
            var pageNumber = page ?? 1;
            var pageSize = 15;
            var applyJobs = _unitOfWork.ApplyJobRepository.GetQuery(a => a.JobPostId == jobPostId, o => o.OrderByDescending(a => a.CreateDate));
            if (!string.IsNullOrEmpty(keyword))
            {
                applyJobs = applyJobs.Where(a => a.User.FullName.ToLower().Trim().Contains(keyword.ToLower().Trim()));
            }

            if (endTime != null && startTime != null)
            {
                if (DateTime.TryParse(startTime, new CultureInfo("vi-VN"), DateTimeStyles.None, out var start))
                {
                    applyJobs = applyJobs.Where(a => DbFunctions.TruncateTime(a.CreateDate) >= DbFunctions.TruncateTime(start));
                }
                if (DateTime.TryParse(endTime, new CultureInfo("vi-VN"), DateTimeStyles.None, out var end))
                {
                    applyJobs = applyJobs.Where(a => DbFunctions.TruncateTime(a.CreateDate) <= DbFunctions.TruncateTime(end));
                }
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

            var model = new ListApplyJobViewModel
            {
                ApplyJobs = applyJobs.ToPagedList(pageNumber, pageSize),
                Keyword = keyword,
                Sort = sort,
                Status = status,
                StartTime = startTime,
                EndTime = endTime,
                jobPostId = jobPostId,

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
        #endregion

        public JsonResult GetCitiesJob(int? countruyId)
        {
            var cities = _unitOfWork.CityRepository
                .GetQuery(a => a.Active && a.CountruyId == countruyId, q => q.OrderBy(a => a.Sort)).Select(a => new { a.Id, a.Name });
            return Json(cities, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            _unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}