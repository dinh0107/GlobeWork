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

namespace GlobeWork.Controllers
{
    [Authorize]
    public class JobPostVcmsController : BaseController
    {
        #region JobPost
        public ActionResult ListJobPost(string result, int? page, string endTime, string careerIds, string cityIds, string jobTypeIds, string skillIds, string rankIds, string startTime, int? status, int? statusTime, string name, string sort = "date-asc")
        {
            var pageNumber = page ?? 1;
            var pageSize = 30;
            var jobPosts = _unitOfWork.JobPostRepository.GetQuery().AsNoTracking();
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
            if (statusTime != null)
            {
                switch (statusTime)
                {
                    case 0:
                        jobPosts = jobPosts.Where(a => a.ExpirationDate < DateTime.Now);
                        break;
                    case 1:
                        jobPosts = jobPosts.Where(a => a.ExpirationDate > DateTime.Now || a.ExpirationDate == null);
                        break;
                }
            }

            if (endTime != null && startTime != null)
            {

                if (DateTime.TryParse(startTime, new CultureInfo("vi-VN"), DateTimeStyles.None, out var start))
                {
                    jobPosts = jobPosts.Where(a => DbFunctions.TruncateTime(a.CreateDate) >= DbFunctions.TruncateTime(start));
                }
                if (DateTime.TryParse(endTime, new CultureInfo("vi-VN"), DateTimeStyles.None, out var end))
                {
                    jobPosts = jobPosts.Where(a => DbFunctions.TruncateTime(a.CreateDate) <= DbFunctions.TruncateTime(end));
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

            if (!string.IsNullOrEmpty(careerIds))
            {
                var tmp = careerIds.Split(',').Select(int.Parse).Cast<int?>().ToList();
                jobPosts = jobPosts.Where(a => a.Careers.Any(c => tmp.Contains(c.Id)));
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

            if (!string.IsNullOrEmpty(cityIds))
            {
                var tmp = cityIds.Split(',').Select(int.Parse).Cast<int?>().ToList();
                jobPosts = jobPosts.Where(a => tmp.Contains(a.CityId));
            }

            var model = new ListJobPostViewModel
            {
                JobPosts = jobPosts.ToPagedList(pageNumber, pageSize),
                RankSelectList = RankSelectList,
                SkillSelectList = SkillSelectList,
                JobTypeSelectList = JobTypeSelectList,
                Careers = Careers,
                CitySelectList = CitySelectList,
                CareerIds = careerIds,
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
                Careers = Careers,
                JobTypeSelectList = JobTypeSelectList,
                SkillSelectList = SkillSelectList,
                RankSelectList = RankSelectList,
                JobPost = jobPost,
            };
            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult UpdateJobPostVcms(CreatePostViewModel model, FormCollection fc)
        {
            var jobPost = _unitOfWork.JobPostRepository.GetById(model.JobPost.Id);
            if (jobPost == null)
            {
                return RedirectToAction("ListJobPost", "JobPostVcms");
            }
            if (ModelState.IsValid)
            {
                jobPost.Url = HtmlHelpers.ConvertToUnSign(null, model.JobPost.Url ?? model.JobPost.Name);
                jobPost.Name = model.JobPost.Name;
                jobPost.Body = model.JobPost.Body;
                jobPost.WebsiteUrl = model.JobPost.WebsiteUrl;
                jobPost.SalalyMax = model.JobPost.SalalyMax;
                jobPost.SalalyMin = model.JobPost.SalalyMin;
                jobPost.Email = model.JobPost.Email;
                jobPost.LastEditDate = DateTime.Now;
                var fcExpirationDate = fc["ExpirationDate"];
                if (!string.IsNullOrEmpty(fcExpirationDate))
                {
                    var expirationDate = DateTime.ParseExact(fcExpirationDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    jobPost.ExpirationDate = expirationDate;
                }
                else
                {
                    jobPost.ExpirationDate = null;

                }
                jobPost.Careers.Clear();
                jobPost.Skills.Clear();
                var careers = fc["career"];
                var listCareer = careers.Split((',')).Select(int.Parse).ToList();
                if (!string.IsNullOrEmpty(careers))
                {
                    foreach (var item in listCareer)
                    {
                        var careerItem = _unitOfWork.CareerRepository.GetById(item);
                        jobPost.Careers.Add(careerItem);
                    }
                }
                var skills = fc["skill"];
                if (!string.IsNullOrEmpty(skills))
                {
                    var listSkill = skills.Split((',')).ToList();
                    foreach (var item in listSkill)
                    {
                        if (string.IsNullOrWhiteSpace(item)) continue;
                        var newSkill = new Skill { SkillName = item.Trim() };
                        var allSkills = _unitOfWork.SkillRepository.GetQuery(a => a.SkillName == item.Trim()).FirstOrDefault();
                        if (allSkills == null)
                        {
                            jobPost.Skills.Add(newSkill);
                        }
                        else
                        {
                            allSkills.Posts.Add(jobPost);
                        }
                    }
                }
                var type = Convert.ToInt32(fc["type"]);
                if (type != 0)
                {
                    jobPost.JobTypeId = type;
                }

                var rank = Convert.ToInt32(fc["rank"]);
                if (rank != 0)
                {
                    jobPost.RankId = rank;
                }

                var cities = fc["city"];
                if (cities != "")
                {
                    foreach (var cId in cities.Split(','))
                    {
                        var city = _unitOfWork.CityRepository.GetById(Convert.ToInt32(cId));
                        jobPost.Cities.Add(city);
                    }
                }
                _unitOfWork.Save();

                var count = _unitOfWork.JobPostRepository.GetQuery(a => a.Url == jobPost.Url).Count();
                if (count > 1)
                {
                    jobPost.Url += "-" + jobPost.Id;
                    _unitOfWork.Save();
                }

                return RedirectToAction("ListJobPost", "JobPostVcms", new { result = "true" });
            }

            model.CitySelectList = CitySelectList;
            model.Careers = Careers;
            model.JobTypeSelectList = JobTypeSelectList;
            model.RankSelectList = RankSelectList;
            model.SkillSelectList = SkillSelectList;
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
            jobPost.Special = special;
            jobPost.ShowHome = home;
            _unitOfWork.Save();
            return true;
        }
        [HttpPost]

        public bool DeleteJobPost(int jobPostId = 0)
        {
            var jobPost = _unitOfWork.JobPostRepository.GetById(jobPostId);
            if (jobPost == null)
            {
                return false;
            }
            _unitOfWork.JobPostRepository.Delete(jobPost);
            _unitOfWork.Save();
            return true;
        }
        #endregion

        #region AppLyJob
        public ActionResult ListApplyJob(int? page, string startTime, string endTime, string keyword, string sort, int? status, string submit)
        {
            var pageNumber = page ?? 1;
            var pageSize = 30;
            var applyJobs = _unitOfWork.ApplyJobRepository.GetQuery().AsNoTracking();

            if (!string.IsNullOrEmpty(keyword))
            {
                applyJobs = applyJobs.Where(a => a.FullName.ToLower().Trim().Contains(keyword.ToLower().Trim()));
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
                    applyJobs = applyJobs.Where(a => a.Status == ApplyJob.ApplyJobStatus.Waiting);
                    break;
                case 1:

                    applyJobs = applyJobs.Where(a => a.Status == ApplyJob.ApplyJobStatus.Active);
                    break;
                case 2:
                    applyJobs = applyJobs.Where(a => a.Status == ApplyJob.ApplyJobStatus.NoActive);
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
                    applyJobs = applyJobs.OrderBy(a => a.FullName);
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
                applyJobs = applyJobs.Where(a => a.FullName.ToLower().Trim().Contains(keyword.ToLower().Trim()));
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
                    applyJobs = applyJobs.OrderBy(a => a.FullName);
                    break;
                default:
                    applyJobs = applyJobs.OrderByDescending(a => a.CreateDate);
                    break;
            }

            switch (status)
            {
                case 0:
                    {
                        applyJobs = applyJobs.Where(a => a.Status == ApplyJob.ApplyJobStatus.Waiting);
                        break;
                    }
                case 1:
                    {
                        applyJobs = applyJobs.Where(a => a.Status == ApplyJob.ApplyJobStatus.Active);
                        break;
                    }
                case 2:
                    {
                        applyJobs = applyJobs.Where(a => a.Status == ApplyJob.ApplyJobStatus.NoActive);
                        break;
                    }
                default:
                    {
                        applyJobs = applyJobs.OrderByDescending(a => a.CreateDate);
                        break;
                    }
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
            var candidate = _unitOfWork.CandidateRepository.GetById(candidateId);
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

        protected override void Dispose(bool disposing)
        {
            _unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}