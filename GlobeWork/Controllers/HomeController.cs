﻿using Helpers;
using GlobeWork.DAL;
using GlobeWork.Filters;
using GlobeWork.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using GlobeWork.ViewModel;
using LinkedinLogin.Models;
using Com.CloudRail.SI.ServiceCode.Commands;
using PagedList;
using System.Xml.Linq;
using Microsoft.Ajax.Utilities;
using System.IO;
using System.Runtime.CompilerServices;
using System.Web.Routing;
using Com.CloudRail.SI.Interfaces;
using System.Globalization;
using System.Threading.Tasks;
using Antlr.Runtime;
using System.Data.Entity;
using GlobeWork.Migrations;
using System.Web.Security;
using Rotativa;
using System.Security.Policy;

namespace GlobeWork.Controllers
{
    [MemberLoginFilter]
    public class HomeController : Controller
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();
        public ConfigSite ConfigSite => (ConfigSite)HttpContext.Application["ConfigSite"];
        private int Id => Convert.ToInt32(RouteData.Values["Id"]);
        private new User User => _unitOfWork.UserRepository.GetById(Id);

        [ChildActionOnly]
        public PartialViewResult Header()
        {
            var article = _unitOfWork.ArticleRepository.GetQuery(a => a.Active && a.Menu, o => o.OrderBy(a => a.Sort));
            var cat = _unitOfWork.StudyAbroadCategoryRepository.GetQuery(a => a.Active && a.ShowMenu, o => o.OrderBy(a => a.Sort));
            var career = _unitOfWork.CareerRepository.GetQuery(a => a.Active && a.Menu, o => o.OrderBy(a => a.Sort));
            var countruy = _unitOfWork.CountryRepository.GetQuery(a => a.Active && a.Job, o => o.OrderBy(a => a.Sort));
            var model = new HeaderViewModel
            {
                Countries = countruy,
                Articles = article,
                StudyAbroadCategories = cat,
                Careers = career,
                Scholarship = _unitOfWork.StudyAbroadCategoryRepository.GetQuery(a => a.Active && a.Scholarship, o => o.OrderBy(a => a.Sort))
            };
            return PartialView(model);
        }
        [ChildActionOnly]
        public PartialViewResult Footer()
        {
            var career = _unitOfWork.CareerRepository.GetQuery(a => a.Active && a.Footer, o => o.OrderBy(a => a.Sort));
            var article = _unitOfWork.ArticleRepository.GetQuery(a => a.Active && a.Footer, o => o.OrderByDescending(a => a.CreateDate));
            var countruy = _unitOfWork.CountryRepository.GetQuery(a => a.Active && a.Footer, o => o.OrderBy(a => a.Sort));
            var model = new FooterViewModel
            {
                Careers = career,
                Articles = article,
                Countries = countruy,
                Parters = _unitOfWork.ParterRepository.GetQuery(a => a.Active && a.Footer, o => o.OrderBy(a => a.Sort)),
            };
            return PartialView(model);
        }
        public ActionResult Index()
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
            var banner = _unitOfWork.BannerRepository.GetQuery(a => a.Active && a.GroupId == 1, o => o.OrderBy(a => a.Sort));
            var count = _unitOfWork.CompanyRepository.GetQuery(a => a.Vipdate > DateTime.Now, o => o.OrderByDescending(a => a.Vipdate), 10).Count();
            var model = new HomeViewModel
            {
                Careers = _unitOfWork.CareerRepository.GetQuery(a => a.Hot && (a.Active && a.TypeCareer == TypeCareer.Career), o => o.OrderBy(a => a.Sort), 16),
                SearchCareers = _unitOfWork.CareerRepository.GetQuery(a => a.Active),
                Countries = _unitOfWork.CountryRepository.GetQuery(a => a.Active && a.FilterJob, o => o.OrderBy(a => a.Sort)),
                JobTypes = _unitOfWork.JobTypeRepository.Get(),
                Ranks = _unitOfWork.RankRepository.Get(),
                Items = items,
                Banners = banner,
                Skills = _unitOfWork.SkillRepository.GetQuery(a => a.ShowHome && a.TypeSkill == TypeSkill.Keyword, o => o.OrderBy(a => a.SkillName), 10),
                Companies = _unitOfWork.CompanyRepository.GetQuery(a => a.Vipdate > DateTime.Now, o => o.OrderByDescending(a => a.Vipdate), 10),
                StudyAbroads = _unitOfWork.StudyAbroadRepository.GetQuery(a => a.Active && (a.Hot != null && a.Hot > DateTime.Now && a.TypeStudyAbroad == TypeStudyAbroad.Scholarship), o => o.OrderByDescending(a => a.CreateDate), 10)
            };
            return View(model);
        }
        public PartialViewResult GetFilter(int type = 0)
        {
            var model = new GetFilterViewModel
            {
                Countries = _unitOfWork.CountryRepository.GetQuery(a => a.Active && a.Filter, o => o.OrderBy(a => a.Sort)),
                Careers = _unitOfWork.CareerRepository.GetQuery(a => a.Active && (a.ShowHome && a.TypeCareer == TypeCareer.Career), o => o.OrderByDescending(a => a.CreateDate))
            };
            return PartialView(model);
        }

        public PartialViewResult GetJob(int? wage, int? exp, int cityId = 0, int careerId = 0)
        {
            var job = _unitOfWork.JobPostRepository.GetQuery(a => a.Active, o => o.OrderByDescending(a => a.Hot)).Take(60);
            if (cityId > 0)
            {
                job = job.Where(a => a.CounId == cityId);
            }
            if (careerId > 0)
            {
                job = job.Where(a => a.CareerId == careerId);
            }
            if (wage != null)
            {
                switch (wage)
                {
                    case 0:
                        job = job.Where(a => a.Wages == Wage.Duoi10);
                        break;
                    case 1:
                        job = job.Where(a => a.Wages == Wage.Tu10den15);
                        break;
                    case 2:
                        job = job.Where(a => a.Wages == Wage.Tu15den20);
                        break;
                    case 3:
                        job = job.Where(a => a.Wages == Wage.Tu20den25);
                        break;
                    case 4:
                        job = job.Where(a => a.Wages == Wage.Tu25den30);
                        break;
                    case 5:
                        job = job.Where(a => a.Wages == Wage.Tu30den50);
                        break;
                    case 6:
                        job = job.Where(a => a.Wages == Wage.Tren50);
                        break;
                    case 7:
                        job = job.Where(a => a.Wages == Wage.ThoaThuan);
                        break;
                }
            }
            if (exp != null)
            {
                switch (exp)
                {
                    case 0:
                        job = job.Where(a => a.Experiences == Experience.Chuaco);
                        break;
                    case 1:
                        job = job.Where(a => a.Experiences == Experience.Motnamtroxuong);
                        break;
                    case 2:
                        job = job.Where(a => a.Experiences == Experience.Hainam);
                        break;
                    case 3:
                        job = job.Where(a => a.Experiences == Experience.Banam);
                        break;
                    case 4:
                        job = job.Where(a => a.Experiences == Experience.tu4den5);
                        break;
                    case 5:
                        job = job.Where(a => a.Experiences == Experience.tren5);
                        break;
                }
            }
            var like = _unitOfWork.LikeRepository.GetQuery(a => a.UserID == User.Id);
            var model = new GetJobHotViewModel
            {
                JobPosts = job,
                Likes = like
            };
            return PartialView(model);
        }


        [Route("viec-lam")]
        public ActionResult Job(int? page, string name = "", int cityId = 0, int careerId = 0, int wage = 0)
        {
            var pageNumber = page ?? 1;
            var job = _unitOfWork.JobPostRepository.GetQuery(a => a.Active, o => o.OrderByDescending(a => a.CreateDate));
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
            if (!string.IsNullOrEmpty(name))
            {
                job = job.Where(a => a.Name.ToLower().Contains(name.ToLower()));
            }
            if (careerId > 0)
            {
                job = job.Where(a => a.CareerId == careerId);
            }
            if (cityId > 0)
            {
                job = job.Where(a => a.CounId == cityId);
            }
            switch (wage)
            {
                case 1:
                    job = job.Where(a => a.Wages == Wage.Duoi10);
                    break;
                case 2:
                    job = job.Where(a => a.Wages == Wage.Tu10den15);
                    break;
                case 3:
                    job = job.Where(a => a.Wages == Wage.Tu15den20);
                    break;
                case 4:
                    job = job.Where(a => a.Wages == Wage.Tu20den25);
                    break;
                case 5:
                    job = job.Where(a => a.Wages == Wage.Tu25den30);
                    break;
                case 6:
                    job = job.Where(a => a.Wages == Wage.Tu30den50);
                    break;
                case 7:
                    job = job.Where(a => a.Wages == Wage.Tren50);
                    break;
                case 8:
                    job = job.Where(a => a.Wages == Wage.ThoaThuan);
                    break;
            }
            var like = _unitOfWork.LikeRepository.GetQuery(a => a.UserID == User.Id);
            var model = new JobHotViewModel
            {
                JobPosts = job.ToPagedList(pageNumber, 9),
                Items = items,
                Name = name,
                CityId = cityId,
                Wage = wage,
                CareerId = careerId,
                Likes = like,
                TotalJob = _unitOfWork.JobPostRepository.GetQuery(a => a.Active).Count(),
                JobNew = _unitOfWork.JobPostRepository.GetQuery(a => a.Active && a.CreateDate == DateTime.Now).Count(),
                Banners = _unitOfWork.BannerRepository.GetQuery(a => a.Active && a.GroupId == 3, o => o.OrderBy(a => a.Sort)),
                Careers = _unitOfWork.CareerRepository.GetQuery(a => a.Active && a.TypeCareer == TypeCareer.Career, o => o.OrderByDescending(a => a.CreateDate)),
                Countries = _unitOfWork.CountryRepository.GetQuery(a => a.Active && a.FilterJob, o => o.OrderBy(a => a.Sort)),
                CareerHot = _unitOfWork.CareerRepository.GetQuery(a => a.Hot && a.Active, o => o.OrderBy(a => a.Sort), 16),
                Companies = _unitOfWork.CompanyRepository.GetQuery(a => a.Vipdate != null && a.Vipdate > DateTime.Now, o => o.OrderByDescending(a => a.Vipdate), 10),
            };
            return View(model);
        }
        [Route("job/chi-tiet/{url}.html")]
        public ActionResult JobDetail(string url)
        {
            var job = _unitOfWork.JobPostRepository.GetQuery(a => a.Active && a.Url == url).FirstOrDefault();
            if (job == null)
            {
                return RedirectToAction("NotFound", "Home", new { status = 404 });
            }
            var jobCompany = _unitOfWork.JobPostRepository.GetQuery(a => a.Active && (a.CompanyId == job.CompanyId && a.Id != job.Id), o => o.OrderByDescending(a => a.CreateDate), 5);
            var jobRelated = _unitOfWork.JobPostRepository.GetQuery(a => a.Active && (a.CounId == job.CounId && a.Id != job.Id), o => o.OrderByDescending(a => a.Hot), 5);
            var jobCareers = job.Company.Careers.Select(c => c.Id).ToList();
            var companyRelate = _unitOfWork.CompanyRepository.GetQuery(a => a.EmployerId != job.Company.EmployerId && a.Careers.Any(c => jobCareers.Contains(c.Id))).Take(5);
            var follow = _unitOfWork.FollowRepository.GetQuery(a => a.UserId == User.Id);
            var like = _unitOfWork.LikeRepository.GetQuery(a => a.UserID == User.Id);
            job.View += 1;
            _unitOfWork.Save();
            var companyJob = _unitOfWork.CompanyRepository.GetQuery(a => a.EmployerId != job.Company.EmployerId && a.Careers.Any(c => jobCareers.Contains(c.Id))).FirstOrDefault();
            var model = new JobDetailViewModel
            {
                Job = job,
                JobCompanys = jobCompany,
                JobPosts = jobRelated,
                Companies = companyRelate,
                Follows = follow,
                Likes = like,
                CompanyJob = companyJob,
                ApplyJobs = _unitOfWork.ApplyJobRepository.GetQuery(a => a.UserId == User.Id && a.JobPostId == job.Id)
            };
            return View(model);
        }

        [Route("viec-lam-tot-nhat")]
        public ActionResult JobHot(int? page, string name = "", int cityId = 0, int careerId = 0, int wage = 0)
        {
            var pageNumber = page ?? 1;
            var job = _unitOfWork.JobPostRepository.GetQuery(a => a.Active, o => o.OrderByDescending(a => a.Hot));
            if (!string.IsNullOrEmpty(name))
            {
                job = job.Where(a => a.Name.ToLower().Contains(name.ToLower()));
            }
            if (careerId > 0)
            {
                job = job.Where(a => a.CareerId == careerId);
            }
            if (cityId > 0)
            {
                job = job.Where(a => a.CounId == cityId);
            }
            switch (wage)
            {
                case 1:
                    job = job.Where(a => a.Wages == Wage.Duoi10);
                    break;
                case 2:
                    job = job.Where(a => a.Wages == Wage.Tu10den15);
                    break;
                case 3:
                    job = job.Where(a => a.Wages == Wage.Tu15den20);
                    break;
                case 4:
                    job = job.Where(a => a.Wages == Wage.Tu20den25);
                    break;
                case 5:
                    job = job.Where(a => a.Wages == Wage.Tu25den30);
                    break;
                case 6:
                    job = job.Where(a => a.Wages == Wage.Tu30den50);
                    break;
                case 7:
                    job = job.Where(a => a.Wages == Wage.Tren50);
                    break;
                case 8:
                    job = job.Where(a => a.Wages == Wage.ThoaThuan);
                    break;
            }
            var like = _unitOfWork.LikeRepository.GetQuery(a => a.UserID == User.Id);
            var model = new JobHotViewModel
            {
                JobPosts = job.ToPagedList(pageNumber, 15),
                Name = name,
                CityId = cityId,
                Wage = wage,
                CareerId = careerId,
                Likes = like,
                Careers = _unitOfWork.CareerRepository.GetQuery(a => a.Active && a.TypeCareer == TypeCareer.Career, o => o.OrderByDescending(a => a.CreateDate)),
                Countries = _unitOfWork.CountryRepository.GetQuery(a => a.Active && a.FilterJob, o => o.OrderBy(a => a.Sort)),
            };
            return View(model);
        }
        [Route("tim-kiem")]
        public ActionResult Search(int? page, int jobTypeId = 0, int rankId = 0, string keyword = "", int countruyId = 0, int careerId = 0, int wage = 0, int companyId = 0)
        {
            var pageNumber = page ?? 1;
            var job = _unitOfWork.JobPostRepository.GetQuery(a => a.Active, o => o.OrderByDescending(a => a.Hot));
            if (!string.IsNullOrEmpty(keyword))
            {
                job = job.Where(a => a.Name.Contains(keyword));
            }
            if (companyId > 0)
            {
                job = job.Where(a => a.CompanyId == companyId);
            }
            if (careerId > 0)
            {
                job = job.Where(a => a.CareerId == careerId);
            }
            if (countruyId > 0)
            {
                job = job.Where(a => a.CounId == countruyId);
            }
            if (jobTypeId > 0)
            {
                job = job.Where(a => a.JobTypeId == jobTypeId);
            }
            if (rankId > 0)
            {
                job = job.Where(a => a.RankId == rankId);
            }
            switch (wage)
            {
                case 1:
                    job = job.Where(a => a.Wages == Wage.Duoi10);
                    break;
                case 2:
                    job = job.Where(a => a.Wages == Wage.Tu10den15);
                    break;
                case 3:
                    job = job.Where(a => a.Wages == Wage.Tu15den20);
                    break;
                case 4:
                    job = job.Where(a => a.Wages == Wage.Tu20den25);
                    break;
                case 5:
                    job = job.Where(a => a.Wages == Wage.Tu25den30);
                    break;
                case 6:
                    job = job.Where(a => a.Wages == Wage.Tu30den50);
                    break;
                case 7:
                    job = job.Where(a => a.Wages == Wage.Tren50);
                    break;
                case 8:
                    job = job.Where(a => a.Wages == Wage.ThoaThuan);
                    break;
            }
            var like = _unitOfWork.LikeRepository.GetQuery(a => a.UserID == User.Id);
            var model = new SearchViewModel
            {
                JobPosts = job.ToPagedList(pageNumber, 9),
                Keyword = keyword,
                CountruyId = countruyId,
                Wage = wage,
                CareerId = careerId,
                CompanyId = companyId,
                Likes = like,
                Careers = _unitOfWork.CareerRepository.GetQuery(a => a.Active && a.TypeCareer == TypeCareer.Career, o => o.OrderByDescending(a => a.CreateDate)),
                Countries = _unitOfWork.CountryRepository.GetQuery(a => a.Active && a.FilterJob, o => o.OrderBy(a => a.Sort))
            };
            return View(model);
        }
        [Route("tim-viec-lam-tu-van/{url}")]
        public ActionResult JobCategory(int? page, string url, int wage = 0, int cityId = 0, string name = "", int type = 0)
        {
            var pageNumber = page ?? 1;
            var category = _unitOfWork.CareerRepository.GetQuery(a => a.Active && (a.Code == url && a.TypeCareer == TypeCareer.Career)).FirstOrDefault();
            if (category == null)
            {
                return RedirectToAction("NotFound", "Home", new { status = 404 });
            }
            var job = _unitOfWork.JobPostRepository.GetQuery(a => a.Active && a.CareerId == category.Id, o => o.OrderByDescending(a => a.Hot));
            if (!string.IsNullOrEmpty(name))
            {
                job = job.Where(a => a.Name.ToLower().Contains(name.ToLower()));
            }
            switch (wage)
            {
                case 1:
                    job = job.Where(a => a.Wages == Wage.Duoi10);
                    break;
                case 2:
                    job = job.Where(a => a.Wages == Wage.Tu10den15);
                    break;
                case 3:
                    job = job.Where(a => a.Wages == Wage.Tu15den20);
                    break;
                case 4:
                    job = job.Where(a => a.Wages == Wage.Tu20den25);
                    break;
                case 5:
                    job = job.Where(a => a.Wages == Wage.Tu25den30);
                    break;
                case 6:
                    job = job.Where(a => a.Wages == Wage.Tu30den50);
                    break;
                case 7:
                    job = job.Where(a => a.Wages == Wage.Tren50);
                    break;
                case 8:
                    job = job.Where(a => a.Wages == Wage.ThoaThuan);
                    break;
            }
            if (cityId > 0)
            {
                job = job.Where(a => a.CounId == cityId);
            }
            if (type > 0)
            {
                job = job.Where(a => a.JobTypeId == type);
            }
            var like = _unitOfWork.LikeRepository.GetQuery(a => a.UserID == User.Id);
            var model = new JobCategoryViewMpdel
            {
                Career = category,
                JobPosts = job.ToPagedList(pageNumber, 9),
                Countries = _unitOfWork.CountryRepository.GetQuery(a => a.Active, o => o.OrderBy(a => a.Active)),
                jobTypes = _unitOfWork.JobTypeRepository.Get(orderBy: a => a.OrderBy(l => l.Id)),
                CityId = cityId,
                Wage = wage,
                Name = name,
                Likes = like,
                Url = url,
                TypeId = type
            };
            return View(model);
        }

        [Route("job/viec-lam-theo-nganh-nghe")]
        public ActionResult JobInCategory(int? page, string keyword, int wage = 0, int level = 0, int careerId = 0, int cityId = 0)
        {
            var pageNumber = page ?? 1;
            var job = _unitOfWork.JobPostRepository.GetQuery(a => a.Active, o => o.OrderByDescending(a => a.Hot));
            if (!string.IsNullOrEmpty(keyword))
            {
                job = job.Where(a => a.Name.Contains(keyword));
            }
            if (level > 0)
            {
                job = job.Where(a => a.RankId == level);
            }
            if (careerId > 0)
            {
                job = job.Where(a => a.CareerId == careerId);
            }
            if (cityId > 0)
            {
                job = job.Where(a => a.CounId == cityId);
            }
            switch (wage)
            {
                case 1:
                    job = job.Where(a => a.Wages == Wage.Duoi10);
                    break;
                case 2:
                    job = job.Where(a => a.Wages == Wage.Tu10den15);
                    break;
                case 3:
                    job = job.Where(a => a.Wages == Wage.Tu15den20);
                    break;
                case 4:
                    job = job.Where(a => a.Wages == Wage.Tu20den25);
                    break;
                case 5:
                    job = job.Where(a => a.Wages == Wage.Tu25den30);
                    break;
                case 6:
                    job = job.Where(a => a.Wages == Wage.Tu30den50);
                    break;
                case 7:
                    job = job.Where(a => a.Wages == Wage.Tren50);
                    break;
                case 8:
                    job = job.Where(a => a.Wages == Wage.ThoaThuan);
                    break;
            }
            var like = _unitOfWork.LikeRepository.GetQuery(a => a.UserID == User.Id);
            var caree = _unitOfWork.CareerRepository.GetById(careerId);
            var model = new JobInCategoryViewModel
            {
                JobPosts = job.ToPagedList(pageNumber, 9),
                Keywords = keyword,
                CareerId = careerId,
                Level = level,
                Wage = wage,
                CityId = cityId,
                Careers = _unitOfWork.CareerRepository.GetQuery(a => a.Active && a.TypeCareer == TypeCareer.Career, o => o.OrderByDescending(a => a.CreateDate)),
                Career = caree,
                Countries = _unitOfWork.CountryRepository.GetQuery(a => a.Active, o => o.OrderBy(a => a.Sort)),
                Likes = like
            };
            return View(model);
        }
        [Route("job/viec-lam-theo-muc-luong")]
        public ActionResult JobInWage()
        {
            return View();
        }
        public PartialViewResult GetJobInWage(int? page, string keyword, string sort = "")
        {
            var pageNumber = page ?? 1;
            var job = _unitOfWork.JobPostRepository.GetQuery(a => a.Active, o => o.OrderByDescending(a => a.Hot));
            if (!string.IsNullOrEmpty(keyword))
            {
                job = job.Where(a => a.Name.Contains(keyword));
            }
            switch (sort)
            {
                case "low-to-high":
                    job = job.OrderBy(a => a.Wages);
                    break;
                case "high-to-low":
                    job = job.OrderByDescending(a => a.Wages);
                    break;
            }
            var like = _unitOfWork.LikeRepository.GetQuery(a => a.UserID == User.Id);
            var model = new JobInWageViewModel
            {
                JobPosts = job.ToPagedList(pageNumber, 9),
                Sort = sort,
                Keywords = keyword,
                Likes = like,
            };
            return PartialView(model);
        }
        [Route("job/viec-lam-theo-quoc-gia/{countruyId}")]
        public ActionResult JobInCountruy(int? page, string keyword, int wage = 0, int level = 0, int careerId = 0, int countruyId = 0)
        {
            var pageNumber = page ?? 1;
            var job = _unitOfWork.JobPostRepository.GetQuery(a => a.Active, o => o.OrderByDescending(a => a.Hot));
            if (!string.IsNullOrEmpty(keyword))
            {
                job = job.Where(a => a.Name.Contains(keyword));
            }
            if (level > 0)
            {
                job = job.Where(a => a.RankId == level);
            }
            if (careerId > 0)
            {
                job = job.Where(a => a.CareerId == careerId);
            }
            if (countruyId > 0)
            {
                job = job.Where(a => a.CounId == countruyId);
            }
            switch (wage)
            {
                case 1:
                    job = job.Where(a => a.Wages == Wage.Duoi10);
                    break;
                case 2:
                    job = job.Where(a => a.Wages == Wage.Tu10den15);
                    break;
                case 3:
                    job = job.Where(a => a.Wages == Wage.Tu15den20);
                    break;
                case 4:
                    job = job.Where(a => a.Wages == Wage.Tu20den25);
                    break;
                case 5:
                    job = job.Where(a => a.Wages == Wage.Tu25den30);
                    break;
                case 6:
                    job = job.Where(a => a.Wages == Wage.Tu30den50);
                    break;
                case 7:
                    job = job.Where(a => a.Wages == Wage.Tren50);
                    break;
                case 8:
                    job = job.Where(a => a.Wages == Wage.ThoaThuan);
                    break;
            }
            var like = _unitOfWork.LikeRepository.GetQuery(a => a.UserID == User.Id);
            var caree = _unitOfWork.CareerRepository.GetById(careerId);
            var model = new JobInCountruyViewModel
            {
                JobPosts = job.ToPagedList(pageNumber, 9),
                Keywords = keyword,
                CareerId = careerId,
                Level = level,
                Wage = wage,
                CountryId = countruyId,
                Careers = _unitOfWork.CareerRepository.GetQuery(a => a.Active && a.TypeCareer == TypeCareer.Career, o => o.OrderByDescending(a => a.CreateDate)),
                Career = caree,
                Countries = _unitOfWork.CountryRepository.GetQuery(a => a.Active && a.FilterJob, a => a.OrderBy(l => l.Id)),
                Likes = like
            };
            return View(model);
        }

        #region Action Job
        [HttpPost]
        public JsonResult Follow(int id)
        {
            var company = _unitOfWork.CompanyRepository.GetQuery(a => a.EmployerId == id).FirstOrDefault();
            if (company == null)
            {
                return Json(new { success = false, message = "Công ty không tồn tại." });
            }
            var existingFollow = _unitOfWork.FollowRepository.GetQuery(a => a.UserId == User.Id && a.CompanyId == company.EmployerId).Any();
            if (existingFollow)
            {
                return Json(new { success = false, message = "Bạn đã theo dõi công ty này trước đó." });
            }

            var data = new Follow()
            {
                UserId = User.Id,
                CompanyId = company.EmployerId,
            };
            data.Company = company;
            _unitOfWork.FollowRepository.Insert(data);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Theo dõi công ty thành công." });
        }

        [HttpPost]
        public JsonResult UnFollow(int id)
        {
            var company = _unitOfWork.CompanyRepository.GetQuery(a => a.EmployerId == id).FirstOrDefault();
            if (company == null)
            {
                return Json(new { success = false, message = "Quá trình thực hiện không thành công" });
            }
            var follow = _unitOfWork.FollowRepository.GetQuery(a => a.UserId == User.Id && a.CompanyId == company.EmployerId).FirstOrDefault();
            if (company == null)
            {
                return Json(new { success = false, message = "Quá trình thực hiện không thành công" });
            }
            _unitOfWork.FollowRepository.Delete(follow);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Bỏ theo dõi công ty thành công !!!" });
        }

        [HttpPost]
        public JsonResult Likejob(int id)
        {
            var job = _unitOfWork.JobPostRepository.GetById(id);
            if (job == null)
            {
                return Json(new { success = false, message = "Quá trình thực hiện không thành công" });
            }
            var likes = _unitOfWork.LikeRepository.GetQuery(a => a.UserID == User.Id && a.JobId == job.Id).Count();
            if (likes >= 1)
            {
                return Json(new { success = true, message = "Lưu tin thành công !!!" });
            }
            var like = new Like()
            {
                JobId = job.Id,
                UserID = User.Id,
                JobPost = job,
            };
            _unitOfWork.LikeRepository.Insert(like);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Lưu tin thành công !!!" });
        }
        [HttpPost]
        public JsonResult UnLike(int id)
        {
            var job = _unitOfWork.JobPostRepository.GetById(id);
            if (job == null)
            {
                return Json(new { success = false, message = "Quá trình thực hiện không thành công" });
            }
            var like = _unitOfWork.LikeRepository.GetQuery(a => a.UserID == User.Id && a.JobId == job.Id).FirstOrDefault();
            if (like == null)
            {
                return Json(new { success = false, message = "Quá trình thực hiện không thành công" });
            }
            _unitOfWork.LikeRepository.Delete(like);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Bỏ lưu tin thành công !!!" });
        }

        [HttpPost]
        public JsonResult LikeStudy(int id)
        {
            var study = _unitOfWork.StudyAbroadRepository.GetById(id);
            if (study == null)
            {
                return Json(new { success = false, message = "Quá trình thực hiện không thành công" });
            }
            var likes = _unitOfWork.LikeRepository.GetQuery(a => a.UserID == User.Id && a.StudyAbroadId == study.Id).Count();
            if (likes >= 1)
            {
                return Json(new { success = true, message = "Lưu tin thành công !!!" });
            }
            var like = new Like()
            {
                StudyAbroadId = study.Id,
                UserID = User.Id,
                StudyAbroad = study,
            };
            _unitOfWork.LikeRepository.Insert(like);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Lưu tin thành công !!!" });
        }
        [HttpPost]
        public JsonResult UnStudy(int id)
        {
            var study = _unitOfWork.StudyAbroadRepository.GetById(id);
            if (study == null)
            {
                return Json(new { success = false, message = "Quá trình thực hiện không thành công" });
            }
            var like = _unitOfWork.LikeRepository.GetQuery(a => a.UserID == User.Id && a.StudyAbroadId == study.Id).FirstOrDefault();
            if (like == null)
            {
                return Json(new { success = false, message = "Quá trình thực hiện không thành công" });
            }
            _unitOfWork.LikeRepository.Delete(like);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Bỏ lưu tin thành công !!!" });
        }
        #endregion

        [Route("cong-ty/{url}")]
        public ActionResult Employer(string url, string name)
        {
            var company = _unitOfWork.CompanyRepository.GetQuery(a => a.Url == url).FirstOrDefault();
            if (company == null)
            {
                return RedirectToAction("NotFound", "Home", new { status = 404 });
            }
            var job = _unitOfWork.JobPostRepository.GetQuery(a => a.CompanyId == company.EmployerId, l => l.OrderByDescending(a => a.Hot), 6);
            if (!string.IsNullOrEmpty(name))
            {
                job = job.Where(a => a.Name.ToLower().Contains(name.ToLower()));
            }
            var follow = _unitOfWork.FollowRepository.GetQuery(a => a.UserId == User.Id && a.CompanyId == company.EmployerId);
            var like = _unitOfWork.LikeRepository.GetQuery(a => a.UserID == User.Id);
            var model = new DetailCompanyViewModel
            {
                JobPosts = job,
                Company = company,
                Follows = follow,
                Likes = like,
                Name = name,
                Url = url
            };
            return View(model);
        }
        [Route("cong-ty/tin-tuyen-dung")]
        public ActionResult Recruitment(int? page, string url, string name, int careerId = 0, int cityId = 0)
        {
            var pageNumber = page ?? 1;
            var company = _unitOfWork.CompanyRepository.GetQuery(a => a.Url == url).FirstOrDefault();
            if (company == null)
            {
                return RedirectToAction("NotFound", "Home", new { status = 404 });
            }
            var job = _unitOfWork.JobPostRepository.GetQuery(a => a.Active && a.CompanyId == company.EmployerId, o => o.OrderByDescending(a => a.Hot));
            if (!string.IsNullOrEmpty(name))
            {
                job = job.Where(a => a.Name.ToLower().Contains(name.ToLower()));
            }
            if (careerId > 0)
            {
                job = job.Where(a => a.CareerId == careerId);
            }
            if (cityId > 0)
            {
                job = job.Where(a => a.CounId == cityId);
            }
            var follow = _unitOfWork.FollowRepository.GetQuery(a => a.UserId == User.Id && a.CompanyId == company.EmployerId);
            var like = _unitOfWork.LikeRepository.GetQuery(a => a.UserID == User.Id);
            var model = new CompanyRecruitmentViewModel
            {
                JobPosts = job.ToPagedList(pageNumber, 9),
                Company = company,
                Follows = follow,
                Likes = like,
                Name = name,
                CareerId = careerId,
                CountryId = cityId,
                Url = url,
                Careers = _unitOfWork.CareerRepository.GetQuery(a => a.Active && a.TypeCareer == TypeCareer.Career, o => o.OrderByDescending(a => a.CreateDate)),
                Countries = _unitOfWork.CountryRepository.GetQuery(a => a.Active && a.FilterJob, o => o.OrderBy(a => a.Sort)),
            };
            return View(model);
        }
        [Route("doanh-nghiep")]
        public ActionResult ListCompany()
        {
            var hot = _unitOfWork.CompanyRepository.GetQuery(a => a.Vipdate >= DateTime.Now);
            var news = _unitOfWork.CompanyRepository.Get(orderBy: l => l.OrderByDescending(a => a.EmployerId));
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
            var model = new ListCompanyViewModel
            {
                HotCompanies = hot,
                Companies = news,
                Items = items
            };
            return View(model);
        }
        [Route("doanh-nghiep/top-doanh-nghiep")]
        public ActionResult CompanyTop(int? page)
        {
            var pageNumber = page ?? 1;
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
            var company = _unitOfWork.CompanyRepository.GetQuery(a => a.Vipdate >= DateTime.Now, o => o.OrderByDescending(a => a.Vipdate));
            var model = new HotCompanyViewModel
            {
                Companies = company.ToPagedList(pageNumber, 6),
                Items = items
            };
            return View(model);
        }
        [Route("tim-kiem-cong-ty")]
        public ActionResult SearchCompany(int? page, string name)
        {
            var pageNumber = page ?? 1;
            var company = _unitOfWork.CompanyRepository.GetQuery(a => a.Name.Contains(name), o => o.OrderByDescending(a => a.Vipdate));
            var model = new SearchCompanyViewModel
            {
                Companies = company.ToPagedList(pageNumber, 6),
                Name = name
            };
            return View(model);
        }

        [Route("cong-ty/tin-du-hoc")]
        public ActionResult StudyAbroadCom(int? page, string url, string name, int cityId = 0)
        {
            var pageNumber = page ?? 1;
            var company = _unitOfWork.CompanyRepository.GetQuery(a => a.Url == url).FirstOrDefault();
            if (company == null)
            {
                return RedirectToAction("NotFound", "Home", new { status = 404 });
            }
            var study = _unitOfWork.StudyAbroadRepository.GetQuery(a => a.Active && a.CompanyId == company.EmployerId, o => o.OrderByDescending(a => a.Hot));
            if (!string.IsNullOrEmpty(name))
            {
                study = study.Where(a => a.Name.ToLower().Contains(name.ToLower()));
            }
            if (cityId > 0)
            {
                study = study.Where(a => a.StudyAbroadCategory.CountryId == cityId);
            }
            var follow = _unitOfWork.FollowRepository.GetQuery(a => a.UserId == User.Id && a.CompanyId == company.EmployerId);
            var like = _unitOfWork.LikeRepository.GetQuery(a => a.UserID == User.Id);
            var model = new CompanyStudyViewModel
            {
                StudyAbroads = study.ToPagedList(pageNumber, 9),
                Company = company,
                Follows = follow,
                Likes = like,
                Name = name,
                CountryId = cityId,
                Url = url,
                Careers = _unitOfWork.CareerRepository.GetQuery(a => a.Active, o => o.OrderByDescending(a => a.CreateDate)),
                Countries = _unitOfWork.CountryRepository.Get(orderBy: a => a.OrderBy(l => l.Id))
            };
            return View(model);
        }

        #region StudyAbroad

        [Route("du-hoc")]
        public ActionResult StudyAbroad()
        {
            var studyAbroad = _unitOfWork.StudyAbroadRepository.GetQuery(a => a.Active && a.TypeStudyAbroad == TypeStudyAbroad.Scholarship, o => o.OrderByDescending(a => a.Hot), 10);
            var hot = _unitOfWork.StudyAbroadRepository.GetQuery(a => a.Active /*&& a.Hot >= DateTime.Now*/, o => o.OrderByDescending(a => a.CreateDate)).Take(20);
            var article = _unitOfWork.ArticleRepository.GetQuery(a => a.Active && (a.ShowHot && a.TypeArticle == TypeArticle.Article), o => o.OrderByDescending(a => a.Hot), 6);
            var model = new StudyAbroadViewModel
            {
                NewStudyAbroad = studyAbroad,
                HotArticle = article,
                HotStudyAbroad = hot,
                Countries = _unitOfWork.CountryRepository.GetQuery(a => a.Active && a.FilterStudy, o => o.OrderBy(a => a.Sort)),
                Banners = _unitOfWork.BannerRepository.GetQuery(a => a.Active && a.GroupId == 2, o => o.OrderBy(a => a.Sort)),
                Skills = _unitOfWork.SkillRepository.GetQuery(a => a.ShowHome, o => o.OrderBy(a => a.SkillName), 10),
            };
            return View(model);
        }
        [Route("du-hoc/{url}")]
        public ActionResult StudyAbroadCategory(string url)
        {
            var cat = _unitOfWork.StudyAbroadCategoryRepository.GetQuery(a => a.Active && a.Url == url).FirstOrDefault();
            if (cat == null)
            {
                return RedirectToAction("Index");
            }
            var study = _unitOfWork.StudyAbroadRepository.GetQuery(a => a.Active && (a.Hot >= DateTime.Now && a.CategoryId == cat.Id), o => o.OrderByDescending(a => a.Hot), 30);
            var article = _unitOfWork.ArticleRepository.GetQuery(a => a.Active && (a.StudyAbroadCategoryId == cat.Id && a.TypeArticle == TypeArticle.Article), o => o.OrderByDescending(a => a.CreateDate), 6);
            var studyAbroad = _unitOfWork.StudyAbroadRepository.GetQuery(a => a.Active && (a.CategoryId == cat.Id && a.TypeStudyAbroad == TypeStudyAbroad.Scholarship), o => o.OrderByDescending(a => a.Hot), 10);
            var model = new StudyAbroadCategoryViewModel
            {
                StudyAbroadCategory = cat,
                HotStudyAbroad = study,
                HotArticle = article,
                NewStudyAbroad = studyAbroad,
                Countries = _unitOfWork.CountryRepository.GetQuery(a => a.Active && a.FilterStudy, o => o.OrderBy(a => a.Sort)),
                Banners = _unitOfWork.BannerRepository.GetQuery(a => a.Active && a.GroupId == 2, o => o.OrderBy(a => a.Sort)),
                Skills = _unitOfWork.SkillRepository.GetQuery(a => a.ShowHome, o => o.OrderBy(a => a.SkillName), 10),
                FooterCountries = _unitOfWork.CountryRepository.GetQuery(a => a.Active && a.Hot, o => o.OrderBy(a => a.Sort), 9),
                CountriesStudy = _unitOfWork.CountryRepository.GetQuery(a => a.Active && a.Scholarship, o => o.OrderBy(a => a.Sort), 9),
                CountriesSchool = _unitOfWork.CountryRepository.GetQuery(a => a.Active && a.School, o => o.OrderBy(a => a.Sort), 9),
                Careers = _unitOfWork.CareerRepository.GetQuery(a => a.Active && a.StudyHot, o => o.OrderBy(a => a.Sort), 9)
            };
            return View(model);
        }
        [Route("du-hoc-moi-nhat/{url}")]
        public ActionResult StudyAbroadCategoryUrl(int? page, string url)
        {
            var pageNumber = page ?? 1;
            var cat = _unitOfWork.StudyAbroadCategoryRepository.GetQuery(a => a.Active && a.Url == url).FirstOrDefault();
            if (cat == null)
            {
                return RedirectToAction("NotFound", "Home", new { status = 404 });
            }
            var study = _unitOfWork.StudyAbroadRepository.GetQuery(a => a.Active && a.CategoryId == cat.Id, o => o.OrderByDescending(a => a.Hot));
            var like = _unitOfWork.LikeRepository.GetQuery(a => a.UserID == User.Id);
            var model = new StudyAbroadUrlViewModel
            {
                StudyAbroadCategory = cat,
                StudyAbroads = study.ToPagedList(pageNumber, 9),
                Countries = _unitOfWork.CountryRepository.GetQuery(a => a.Active && a.FilterStudy, o => o.OrderBy(a => a.Sort)),
                Skills = _unitOfWork.SkillRepository.GetQuery(a => a.ShowHome, o => o.OrderBy(a => a.SkillName), 10),
                Likes = like
            };
            return View(model);
        }
        [Route("du-hoc/{url}.html")]
        public ActionResult StudyAbroadDetail(string url)
        {
            var study = _unitOfWork.StudyAbroadRepository.GetQuery(a => a.Active && a.Url == url).FirstOrDefault();
            if (study == null)
            {
                return RedirectToAction("NotFound", "Home", new { status = 404 });
            }
            var studyCompany = _unitOfWork.StudyAbroadRepository.GetQuery(a => a.Active && (a.Id != study.Id && a.CompanyId == study.CompanyId), o => o.OrderByDescending(a => a.CreateDate), 5);
            var relate = _unitOfWork.StudyAbroadRepository.GetQuery(a => a.Active && (a.Id != study.Id && a.CategoryId == study.CategoryId), o => o.OrderByDescending(a => a.CreateDate), 5);
            var follow = _unitOfWork.FollowRepository.GetQuery(a => a.UserId == User.Id);
            var like = _unitOfWork.LikeRepository.GetQuery(a => a.UserID == User.Id);
            study.View += 1;
            _unitOfWork.Save();
            var careers = study.Company.Careers.Select(c => c.Id).ToList();
            var companyStudy = _unitOfWork.CompanyRepository.GetQuery(a => a.EmployerId != study.Company.EmployerId && a.Careers.Any(c => careers.Contains(c.Id))).FirstOrDefault();
            var model = new StudyDetailViewModel
            {
                StudyAbroad = study,
                StudyAbroadCompanys = studyCompany,
                StudyAbroads = relate,
                Follows = follow,
                CompanyStudy = companyStudy,
                Likes = like,
                ApplyJobs = _unitOfWork.ApplyJobRepository.GetQuery(a => a.UserId == User.Id && a.StudyAbroadId == study.Id)
            };
            return View(model);
        }
        [Route("tim-kiem-du-hoc")]
        public ActionResult SearchStudyAbroad(int? page, string name, int countruyId = 0, int companyId = 0)
        {
            var pageNumber = page ?? 1;
            var study = _unitOfWork.StudyAbroadRepository.GetQuery(a => a.Active, o => o.OrderByDescending(a => a.Hot));
            if (!string.IsNullOrEmpty(name))
            {
                study = study.Where(a => a.Name.Contains(name));
            }
            if (countruyId > 0)
            {
                study = study.Where(a => a.StudyAbroadCategory.CountryId == countruyId);
            }
            if (companyId > 0)
            {
                study = study.Where(a => a.CompanyId == companyId);
            }
            var model = new SearchStudyAbroadViewModel
            {
                Name = name,
                StudyAbroads = study.ToPagedList(pageNumber, 10),
                Countries = _unitOfWork.CountryRepository.GetQuery(a => a.Active && a.FilterStudy, o => o.OrderBy(a => a.Sort)),
                CountruyId = countruyId,
                CompanyId = companyId
            };
            return View(model);

        }
        [Route("tat-ca-tin-du-hoc")]
        public ActionResult AllStudyAbroad(int? page, string name, int countruyId = 0)
        {
            var pageNumber = page ?? 1;
            var study = _unitOfWork.StudyAbroadRepository.GetQuery(a => a.Active, o => o.OrderByDescending(a => a.Hot));
            if (countruyId > 0)
            {
                study = study.Where(a => a.StudyAbroadCategory.CountryId == countruyId);
            }
            if (!string.IsNullOrEmpty(name))
            {
                study = study.Where(a => a.Name.Contains(name));
            }
            var like = _unitOfWork.LikeRepository.GetQuery(a => a.UserID == User.Id);
            var model = new AllStudyAbroadViewModel
            {
                Name = name,
                StudyAbroads = study.ToPagedList(pageNumber, 10),
                Countries = _unitOfWork.CountryRepository.GetQuery(a => a.Active && a.FilterStudy, o => o.OrderBy(a => a.Sort)),
                CountruyId = countruyId,
                Likes = like
            };
            return View(model);
        }
        [Route("san-hoc-bong")]
        public ActionResult StudyAbroadHot(int? page, string name, int countruyId = 0)
        {
            var pageNumber = page ?? 1;
            var study = _unitOfWork.StudyAbroadRepository.GetQuery(a => a.Active && a.Hot >= DateTime.Now, o => o.OrderByDescending(a => a.Hot));
            if (countruyId > 0)
            {
                study = study.Where(a => a.StudyAbroadCategory.CountryId == countruyId);
            }
            var like = _unitOfWork.LikeRepository.GetQuery(a => a.UserID == User.Id);
            var model = new AllStudyAbroadViewModel
            {
                Name = name,
                StudyAbroads = study.ToPagedList(pageNumber, 10),
                Countries = _unitOfWork.CountryRepository.GetQuery(a => a.Active && a.FilterStudy, o => o.OrderBy(a => a.Sort)),
                CountruyId = countruyId,
                Likes = like
            };
            return View(model);
        }
        [Route("du-hoc/nuoc/{id}")]
        public ActionResult CountruyStudy(int? page, int id , string name = "" , int countruyId = 0)
        {
            var pageNumber = page ?? 1;
            var countruy = _unitOfWork.CountryRepository.GetById(id);
            if (countruy == null)
            {
                return RedirectToAction("NotFound", "Home", new { status = 404 });
            }
            var study = _unitOfWork.StudyAbroadRepository.GetQuery(a => a.Active && (a.StudyAbroadCategory.CountryId == countruy.Id && a.TypeStudyAbroad == TypeStudyAbroad.StudyAbroad), o => o.OrderByDescending(a => a.Hot));
            if (!string.IsNullOrEmpty(name))
            {
                study = study.Where(a => a.Name.Contains(name));
            }
            if (countruyId > 0)
            {
                study = study.Where(a => a.StudyAbroadCategory.CountryId == countruyId);
            }
            var model = new CountruyStudyAbroadViewModel
            {
                StudyAbroads = study.ToPagedList(pageNumber, 10),
                Country = countruy,
                Countries = _unitOfWork.CountryRepository.GetQuery(a => a.Active && a.FilterStudy, o => o.OrderBy(a => a.Sort)),
                Name = name,
                CountruyId = countruyId
            };
            return View(model);
        }
        [Route("du-hoc/hoc-bong/{id}")]
        public ActionResult Scholarship(int? page, int id, string name = "", int countruyId = 0)
        {
            var pageNumber = page ?? 1;
            var countruy = _unitOfWork.CountryRepository.GetById(id);
            if (countruy == null)
            {
                return RedirectToAction("NotFound", "Home", new { status = 404 });
            }
            var study = _unitOfWork.StudyAbroadRepository.GetQuery(a => a.Active && (a.StudyAbroadCategory.CountryId == countruy.Id && a.TypeStudyAbroad == TypeStudyAbroad.Scholarship), o => o.OrderByDescending(a => a.Hot));
            if (!string.IsNullOrEmpty(name))
            {
                study = study.Where(a => a.Name.Contains(name));
            }
            if (countruyId > 0)
            {
                study = study.Where(a => a.StudyAbroadCategory.CountryId == countruyId);
            }
            var model = new CountruyStudyAbroadViewModel
            {
                Country = countruy,
                StudyAbroads = study.ToPagedList(pageNumber, 10),
                Countries = _unitOfWork.CountryRepository.GetQuery(a => a.Active && a.FilterStudy, o => o.OrderBy(a => a.Sort)),
                Name = name,
                CountruyId = countruyId
            };
            return View(model);
        }
        [Route("san-hoc-bong/{url}")]
        public ActionResult Hunting(int? page, string url, string name, int countruyId = 0)
        {
            var pageNumber = page ?? 1;
            var cat = _unitOfWork.StudyAbroadCategoryRepository.GetQuery(a => a.Active && a.Url == url).FirstOrDefault();
            if (cat == null)
            {
                return RedirectToAction("NotFound", "Home", new { status = 404 });
            }
            var hunting = _unitOfWork.StudyAbroadRepository.GetQuery(a => a.Active && a.CategoryId == cat.Id && a.TypeStudyAbroad == TypeStudyAbroad.Scholarship, o => o.OrderByDescending(a => a.Hot));
            if (countruyId > 0)
            {
                hunting = hunting.Where(a => a.StudyAbroadCategory.CountryId == countruyId);
            }
            if (!string.IsNullOrEmpty(name))
            {
                hunting = hunting.Where(a => a.Name.Contains(name));
            }
            var like = _unitOfWork.LikeRepository.GetQuery(a => a.UserID == User.Id);
            var model = new HuntingViewModel
            {
                StudyAbroads = hunting.ToPagedList(pageNumber, 9),
                StudyAbroadCategory = cat,
                Countries = _unitOfWork.CountryRepository.GetQuery(a => a.Active && a.FilterStudy, o => o.OrderBy(a => a.Sort)),
                CountruyId = countruyId,
                Likes = like
            };
            return View(model);
        }

        [Route("du-hoc/nganh/{url}")]
        public ActionResult StudyCareer(int? page, string url , string name = "" , int countruyId = 0)
        {
            var pageNumber = page ?? 1;
            var career = _unitOfWork.CareerRepository.GetQuery(a => a.Active && a.Code == url).FirstOrDefault();
            if (career == null)
            {
                return RedirectToAction("NotFound", "Home", new { status = 404 });
            }
            var study = _unitOfWork.StudyAbroadRepository.GetQuery(a => a.Active && (a.CareerId == career.Id && a.TypeStudyAbroad == TypeStudyAbroad.StudyAbroad), o => o.OrderByDescending(a => a.Hot));
            if (!string.IsNullOrEmpty(name))
            {
                study = study.Where(a => a.Name.Contains(name));
            }
            if (countruyId > 0)
            {
                study = study.Where(a => a.StudyAbroadCategory.CountryId == countruyId);
            }
            var model = new CountruyStudyAbroadViewModel
            {
                StudyAbroads = study.ToPagedList(pageNumber, 10),
                Career = career,
                Countries = _unitOfWork.CountryRepository.GetQuery(a => a.Active && a.FilterStudy, o => o.OrderBy(a => a.Sort)),
                Name = name,
                CountruyId = countruyId
            };
            return View(model);
        }
        [Route("danh-sach-truong/{id}")]
        public ActionResult SchoolCountruy(int? page, int id , string name="")
        {
            var pageNumber = page ?? 1;
            var countruy = _unitOfWork.CountryRepository.GetById(id);
            if (countruy == null)
            {
                return RedirectToAction("NotFound", "Home", new { status = 404 });
            }
            var article = _unitOfWork.ArticleRepository.GetQuery(a => a.Active && a.StudyAbroadCategory.CountryId == countruy.Id, o => o.OrderByDescending(a => a.Hot));
            if (!string.IsNullOrEmpty(name))
            {
                article = article.Where(a => a.Subject.Contains(name));
            }
            var model = new CountruyStudyAbroadViewModel
            {
                Articles = article.ToPagedList(pageNumber, 9),
                Country = countruy
            };
            return View(model);
        }
        #endregion

        [Route("bai-viet/{url}.html")]
        public ActionResult ArticleDetail(string url)
        {
            var article = _unitOfWork.ArticleRepository.GetQuery(a => a.Active && a.Url == url).FirstOrDefault();
            if (article == null)
            {
                return RedirectToAction("NotFound", "Home", new { status = 404 });
            }
            article.View += 1;
            _unitOfWork.Save();
            return View(article);
        }
        public ActionResult About()
        {
            return View();
        }
        [Route("lien-he")]
        public ActionResult Contact()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult Contact(Contact model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { status = false, msg = "Hãy điền đúng định dạng." });
            }
            var IP = HttpContext.Request.UserHostAddress;
            if (IP == null)
            {
                return Json(new { status = false, msg = "Hãy điền đúng định dạng." });
            }
            DateTime currentDate = DateTime.Now.Date;
            var count = _unitOfWork.ContactRepository.GetQuery(a => a.IP == IP && DbFunctions.TruncateTime(a.CreateDate) == currentDate).Count();
            if (count > 5)
            {
                return Json(new { status = false, msg = "Vượt quá số lần cho phép" });
            }
            model.IP = IP;
            _unitOfWork.ContactRepository.Insert(model);
            _unitOfWork.Save();
            var subject = "Email liên hệ từ website: " + Request.Url?.Host;
            var body = $"<p>Tên người liên hệ: {model.FullName},</p>" +
                        $"<p>Email liên hệ: {model.Email},</p>" +
                        $"<p>Số điện thoại: {model.Mobile},</p>" +
                        $"<p>Nội dung:{model.Body}</p>" +
                        $"<p>Đây là hệ thống gửi email tự động, vui lòng không phản hồi lại email này.</p>";
            Task.Run(() => HtmlHelpers.SendEmail("gmail", subject, body, ConfigSite.Email, ConfigSite.EmailConfigs, ConfigSite.EmailConfigs, ConfigSite.PassWordMail, ConfigSite.Title));
            return Json(new { status = true, msg = "Gửi liên hệ thành công.\nChúng tôi sẽ liên lạc lại với bạn sớm nhất có thể." });
        }


        [Route("chi-tiet/{url}.html")]
        public ActionResult UserProfile(string url, int type = 0)
        {
            var detail = _unitOfWork.UserRepository.GetQuery(a => a.Active && a.Url == url).FirstOrDefault();
            if (detail == null)
            {
                return RedirectToAction("NotFound", "Home", new { status = 404 });
            }

            var authCookieUser = Request.Cookies[".ASPXAUTHMEMBER"];
            var authCookieEmployer = Request.Cookies[".ASPXAUTHEMPLOYER"];

            if (authCookieUser != null)
            {
                var ticket = FormsAuthentication.Decrypt(authCookieUser.Value);
                if (ticket != null)
                {
                    var userData = ticket.UserData;
                    var parts = userData.Split('|');
                    if (parts.Length > 1)
                    {
                        var userId = Convert.ToInt32(parts[1]);
                        var user = _unitOfWork.UserRepository.GetById(userId);
                        if (user == null)
                        {
                            return RedirectToAction("NotFound", "Home", new { status = 403 });
                        }
                        else
                        {
                            var isAccount = (user.Id == detail.Id);

                            return PrepareUserProfileView(detail, isAccount, type);
                        }
                    }
                }
            }
            else if (authCookieEmployer != null)
            {
                var ticket = FormsAuthentication.Decrypt(authCookieEmployer.Value);
                if (ticket != null)
                {
                    var userData = ticket.UserData;
                    var parts = userData.Split('|');
                    if (parts.Length > 1)
                    {
                        var employerId = Convert.ToInt32(parts[1]);
                        var apply = _unitOfWork.ApplyJobRepository.GetQuery(a => a.CompanyId == employerId).FirstOrDefault();
                        if (apply == null)
                        {
                            return RedirectToAction("NotFound", "Home", new { status = 403 });
                        }
                        else
                        {
                            return PrepareUserProfileView(detail, false, type);
                        }
                    }
                }
            }

            return RedirectToAction("NotFound", "Home", new { status = 403 });
        }

        private ActionResult PrepareUserProfileView(User detail, bool isAccount, int type)
        {
            var edu = _unitOfWork.EducationRepository.GetQuery(a => a.UserId == detail.Id, o => o.OrderBy(a => a.Id));
            var exp = _unitOfWork.ExperienceRepository.GetQuery(a => a.UserId == detail.Id, o => o.OrderBy(a => a.Id));
            var skill = _unitOfWork.UserSkillRepository.GetQuery(a => a.UserId == detail.Id, o => o.OrderBy(a => a.Id));
            var cer = _unitOfWork.CertificateRepository.GetQuery(a => a.UserId == detail.Id, o => o.OrderBy(a => a.Id));
            var project = _unitOfWork.ProjectRepository.GetQuery(a => a.UserId == detail.Id, o => o.OrderBy(a => a.Id));
            var activity = _unitOfWork.ActivityRepository.GetQuery(a => a.UserId == detail.Id, o => o.OrderBy(a => a.Id));

            var viewLogs = _unitOfWork.UserViewLogRepository.GetQuery(a => a.UserId == detail.Id).ToList();
            var viewsByWeek = viewLogs.GroupBy(log => CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(log.ViewedAt, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday)).Select(group => new { Week = group.Key, ViewCount = group.Count() });
            var viewsByMonth = viewLogs.GroupBy(log => new { log.ViewedAt.Year, log.ViewedAt.Month }).Select(group => new { Year = group.Key.Year, Month = group.Key.Month, ViewCount = group.Count() });
            var viewsByYear = viewLogs.GroupBy(log => log.ViewedAt.Year).Select(group => new { Year = group.Key, ViewCount = group.Count() });

            var model = new UserInfoViewModel
            {
                User = detail,
                Experiences = exp,
                Educations = edu,
                IsAccount = isAccount,
                UserSkills = skill,
                Certificates = cer,
                Projects = project,
                Activitys = activity,
                EduInfo = _unitOfWork.EducationRepository.GetQuery(a => a.UserId == detail.Id).FirstOrDefault(),
                ExpInfo = _unitOfWork.ExperienceRepository.GetQuery(a => a.UserId == detail.Id).FirstOrDefault(),
                ViewsByWeek = viewsByWeek.Sum(item => item.ViewCount),
                ViewsByMonth = viewsByMonth.Sum(item => item.ViewCount),
                ViewsByYear = viewsByYear.Sum(item => item.ViewCount),
            };

            if (type > 0)
            {
                var userViewLog = new UserViewLog { UserId = detail.Id };
                _unitOfWork.UserViewLogRepository.Insert(userViewLog);
                _unitOfWork.Save();
            }

            return View(model);
        }

        public PartialViewResult ApplyForm()
        {
            return PartialView();
        }
        [HttpPost]
        public JsonResult Apply(ApplyViewModel model, FormCollection fc)
        {
            if (ModelState.IsValid)
            {
                if (model.StudyAbroadId > 0)
                {
                    var study = _unitOfWork.StudyAbroadRepository.GetById(model.StudyAbroadId);
                    if (study == null)
                    {
                        return Json(new { success = false, message = "Quá trình thực hiện không thành công" });
                    }
                    var company = _unitOfWork.CompanyRepository.GetQuery(a => a.EmployerId == model.CompanyId).FirstOrDefault();
                    if (company == null)
                    {
                        return Json(new { success = false, message = "Quá trình thực hiện không thành công" });
                    }
                    var file = Request.Files["ApplyJob.FileUpload"];
                    if (file != null && file.ContentLength > 0)
                    {
                        if (!HtmlHelpers.CheckFileExt(file.FileName, "doc|docx|pdf"))
                        {
                            ModelState.AddModelError("", @"Chỉ chấp nhận định dạng doc, docx, pdf");
                            return Json(new { success = false, message = "Quá trình thực hiện không thành công" });
                        }
                        else
                        {
                            if (file.ContentLength > 40000 * 1024)
                            {
                                ModelState.AddModelError("", @"Dung lượng lớn hơn 40MB. Hãy thử lại");
                                return Json(new { success = false, message = "Quá trình thực hiện không thành công" });
                            }
                            else
                            {
                                var imgFileName = HtmlHelpers.ConvertToUnSign(null, Path.GetFileNameWithoutExtension(file.FileName)) + "-" + DateTime.Now.Millisecond + Path.GetExtension(file.FileName);
                                var imgPath = "/images/cv/" + DateTime.Now.ToString("yyyy/MM/dd");
                                var fullPath = Path.Combine(Server.MapPath(imgPath), imgFileName);
                                if (!Directory.Exists(Server.MapPath(imgPath)))
                                {
                                    Directory.CreateDirectory(Server.MapPath(imgPath));
                                }
                                file.SaveAs(fullPath);
                                model.ApplyJob.FileUpload = Path.Combine(imgPath, imgFileName);
                            }
                        }
                    }
                    switch (model.Type)
                    {
                        case 0:
                            model.ApplyJob.TypeApply = TypeApply.Profile;
                            model.ApplyJob.Url = User.Url;
                            break;
                        case 1:
                            model.ApplyJob.TypeApply = TypeApply.File;
                            break;
                    }
                    model.ApplyJob.UserId = User.Id;
                    model.ApplyJob.CompanyId = company.EmployerId;
                    model.ApplyJob.Status = ApplyJobStatus.Waiting;
                    model.ApplyJob.StudyAbroadId = study.Id;
                    _unitOfWork.ApplyJobRepository.Insert(model.ApplyJob);
                    _unitOfWork.Save();

                    var url = Request.Url?.GetLeftPart(UriPartial.Authority) + Url.Action("ListCVStudy", "Employer");
                    var emailTemp = System.IO.File.ReadAllText(Server.MapPath("/EmailTemplates/UserApply.html"));
                    emailTemp = emailTemp.Replace("[USER]", model.ApplyJob.User.FullName)
                        .Replace("[JOB]", model.ApplyJob.StudyAbroad.Name)
                        .Replace("[DATE]", model.ApplyJob.CreateDate.ToString("dd/MM/yyyy")).Replace("[URL]", url);

                    var stud = _unitOfWork.StudyAbroadRepository.GetById(model.ApplyJob.StudyAbroad.Id);
                    var companyEmail = stud.Company.Email ?? stud.Company.Employer.Email;
                    var emailSubject = "Thông báo tuyển dụng " + stud.Name;
                    var emailBody = emailTemp;
                    _unitOfWork.Dispose();
                    Task.Run(() => HtmlHelpers.SendEmail("gmail", emailSubject, emailBody, companyEmail, ConfigSite.EmailConfigs, ConfigSite.EmailConfigs, ConfigSite.PassWordMail, ConfigSite.Title));
                    //Task.Run(() => HtmlHelpers.SendEmail("gmail", "Thông báo tuyển dụng " + model.ApplyJob.StudyAbroad.Name, emailTemp, model.ApplyJob.StudyAbroad.Company.Employer.Email, ConfigSite.EmailConfigs, ConfigSite.EmailConfigs, ConfigSite.PassWordMail, ConfigSite.Title));
                    return Json(new { success = true, message = "Gửi thông tin ứng tuyển thành công!!!" });
                }
                if (model.JobId > 0)
                {
                    var job = _unitOfWork.JobPostRepository.GetById(model.JobId);
                    if (job == null)
                    {
                        return Json(new { success = false, message = "Quá trình thực hiện không thành công" });
                    }
                    var company = _unitOfWork.CompanyRepository.GetQuery(a => a.EmployerId == model.CompanyId).FirstOrDefault();
                    if (company == null)
                    {
                        return Json(new { success = false, message = "Quá trình thực hiện không thành công" });
                    }
                    var file = Request.Files["ApplyJob.FileUpload"];
                    if (file != null && file.ContentLength > 0)
                    {
                        if (!HtmlHelpers.CheckFileExt(file.FileName, "doc|docx|pdf"))
                        {
                            ModelState.AddModelError("", @"Chỉ chấp nhận định dạng doc, docx, pdf");
                            return Json(new { success = false, message = "Quá trình thực hiện không thành công" });
                        }
                        else
                        {
                            if (file.ContentLength > 40000 * 1024)
                            {
                                ModelState.AddModelError("", @"Dung lượng lớn hơn 40MB. Hãy thử lại");
                                return Json(new { success = false, message = "Quá trình thực hiện không thành công" });
                            }
                            else
                            {
                                var imgFileName = HtmlHelpers.ConvertToUnSign(null, Path.GetFileNameWithoutExtension(file.FileName)) + "-" + DateTime.Now.Millisecond + Path.GetExtension(file.FileName);
                                var imgPath = "/images/cv/" + DateTime.Now.ToString("yyyy/MM/dd");
                                var fullPath = Path.Combine(Server.MapPath(imgPath), imgFileName);
                                if (!Directory.Exists(Server.MapPath(imgPath)))
                                {
                                    Directory.CreateDirectory(Server.MapPath(imgPath));
                                }
                                file.SaveAs(fullPath);
                                model.ApplyJob.FileUpload = Path.Combine(imgPath, imgFileName);
                            }
                        }
                    }
                    switch (model.Type)
                    {
                        case 0:
                            model.ApplyJob.TypeApply = TypeApply.Profile;
                            model.ApplyJob.Url = User.Url;
                            break;
                        case 1:
                            model.ApplyJob.TypeApply = TypeApply.File;
                            break;
                    }
                    model.ApplyJob.CompanyId = company.EmployerId;
                    model.ApplyJob.UserId = User.Id;
                    model.ApplyJob.Status = ApplyJobStatus.Waiting;
                    model.ApplyJob.JobPostId = job.Id;
                    _unitOfWork.ApplyJobRepository.Insert(model.ApplyJob);
                    _unitOfWork.Save();
                    var url = Request.Url?.GetLeftPart(UriPartial.Authority) + Url.Action("ListCV", "Employer");
                    var emailTemp = System.IO.File.ReadAllText(Server.MapPath("/EmailTemplates/UserApply.html"));
                    emailTemp = emailTemp.Replace("[USER]", model.ApplyJob.User.FullName)
                        .Replace("[JOB]", model.ApplyJob.JobPost.Name)
                        .Replace("[DATE]", model.ApplyJob.CreateDate.ToString("dd/MM/yyyy")).Replace("[URL]", url);

                    var jobPost = _unitOfWork.JobPostRepository.GetById(model.ApplyJob.JobPost.Id);
                    var companyEmail = jobPost.Company.Email ?? jobPost.Company.Employer.Email;
                    var emailSubject = "Thông báo tuyển dụng " + jobPost.Name;
                    var emailBody = emailTemp;
                    _unitOfWork.Dispose();
                    Task.Run(() => HtmlHelpers.SendEmail("gmail", emailSubject, emailBody, companyEmail, ConfigSite.EmailConfigs, ConfigSite.EmailConfigs, ConfigSite.PassWordMail, ConfigSite.Title));
                    //Task.Run(() => HtmlHelpers.SendEmail("gmail", "Thông báo tuyển dụng " + model.ApplyJob.JobPost.Name, emailTemp, model.ApplyJob.JobPost.Company.Employer.Email, ConfigSite.EmailConfigs, ConfigSite.EmailConfigs, ConfigSite.PassWordMail, ConfigSite.Title));
                    return Json(new { success = true, message = "Gửi thông tin ứng tuyển thành công!!!" });
                }
            }
            return Json(new { success = false, message = "Quá trình thực hiện không thành công" });
        }
        public ActionResult GeneratePdf(string url)
        {
            var user = _unitOfWork.UserRepository.GetQuery(a => a.Url == url).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index");
            }

            var options = new Rotativa.Options.Margins(0, 0, 0, 0);

            return new ActionAsPdf("ViewAndEx", new { url = url })
            {
                FileName = user.FullName + ".pdf",
                PageSize = Rotativa.Options.Size.A4,
                PageMargins = { Left = 10, Right = 10, Top = 20, Bottom = 0 },
                PageOrientation = Rotativa.Options.Orientation.Portrait,
            };

        }
        public ActionResult ViewAndEx(string url)
        {
            var user = _unitOfWork.UserRepository.GetQuery(a => a.Url == url).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index");
            }
            return View(user);
        }
        public PartialViewResult AdviseForm()
        {
            var model = new AdviseViewModel
            {
                User = User
            };
            return PartialView(model);
        }

        [HttpPost]
        public JsonResult AdviseForm(AdviseViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Advise.JobPostId > 0)
                {
                    var job = _unitOfWork.JobPostRepository.GetById(model.Advise.JobPostId);
                    if (job == null)
                    {
                        return Json(new { success = false, message = "Quá trình thực hiện không thành công" });
                    }
                    model.Advise.JobPostId = job.Id;
                    _unitOfWork.AdviseRepository.Insert(model.Advise);
                    _unitOfWork.Save();
                    var jobsubject = "Yêu cầu tư vấn: " + Request.Url?.Host;
                    var jobbody = $"<p>Tên người liên hệ: {model.Advise.CustomerInfo.Fullname},</p>" +
                                $"<p>Email liên hệ: {model.Advise.CustomerInfo.Email},</p>" +
                                $"<p>Số điện thoại: {model.Advise.CustomerInfo.Mobile},</p>" +
                                $"<p>Việc làm cần tư vấn: {model.Advise.Name},</p>" +
                                $"<p>Nội dung:{model.Advise.Body}</p>" +
                                $"<p>Đây là hệ thống gửi email tự động, vui lòng không phản hồi lại email này.</p>";

                    Task.Run(() => HtmlHelpers.SendEmail("gmail", jobsubject, jobbody, job.Company.Email, ConfigSite.EmailConfigs, ConfigSite.EmailConfigs, ConfigSite.PassWordMail, ConfigSite.Title));
                    return Json(new { success = true, message = "Gửi thông yêu cầu tư vấn thành công!!!" });
                }
                if (model.Advise.StudyAbroadId > 0)
                {
                    var study = _unitOfWork.StudyAbroadRepository.GetById(model.Advise.StudyAbroadId);
                    if (study == null)
                    {
                        return Json(new { success = false, message = "Quá trình thực hiện không thành công" });
                    }
                    model.Advise.StudyAbroadId = study.Id;
                    _unitOfWork.AdviseRepository.Insert(model.Advise);
                    _unitOfWork.Save();
                    var subject = "Yêu cầu tư vấn: " + Request.Url?.Host;
                    var body = $"<p>Tên người liên hệ: {model.Advise.CustomerInfo.Fullname},</p>" +
                                $"<p>Email liên hệ: {model.Advise.CustomerInfo.Email},</p>" +
                                $"<p>Số điện thoại: {model.Advise.CustomerInfo.Mobile},</p>" +
                                $"<p>Tin du học cần tư vấn: {model.Advise.Name},</p>" +
                                $"<p>Nội dung:{model.Advise.Body}</p>" +
                                $"<p>Đây là hệ thống gửi email tự động, vui lòng không phản hồi lại email này.</p>";
                    Task.Run(() => HtmlHelpers.SendEmail("gmail", subject, body, study.Company.Email, ConfigSite.EmailConfigs, ConfigSite.EmailConfigs, ConfigSite.PassWordMail, ConfigSite.Title));
                    return Json(new { success = true, message = "Gửi thông yêu cầu tư vấn thành công!!!" });
                }
            }
            return Json(new { success = false, message = "Quá trình thực hiện không thành công" });
        }

        public PartialViewResult QickLogin()
        {
            return PartialView();
        }
        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult Login(UserLoginModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _unitOfWork.UserRepository.Get(a => a.Email == model.Email).SingleOrDefault();
                if (user == null || !HtmlHelpers.VerifyHash(model.Password, "SHA256", user.Password))
                {
                    ModelState.AddModelError("", @"Tên đăng nhập hoặc mật khẩu không chính xác.");
                    return Json(new { success = false, message = "Tên đăng nhập hoặc mật khẩu không chính xác." });
                }
                if (!user.Active)
                {
                    ModelState.AddModelError("", @"Tài khoản tạm thời bị khóa. Vui lòng liên hệ với quản trị viên.");
                    return Json(new { success = false, message = "Tài khoản tạm thời bị khóa. Vui lòng liên hệ với quản trị viên." });
                }

                var userData = user.Avatar + "|" + user.Id + "|" + user.Email + "|" + user.FullName + "|" + user.Url + "|" + user.AvatarSocial;
                var ticket = new FormsAuthenticationTicket(2, user.Email.ToLower(), DateTime.Now, DateTime.Now.AddDays(30), true,
                    userData, FormsAuthentication.FormsCookiePath);
                var encTicket = FormsAuthentication.Encrypt(ticket);
                Response.Cookies.Add(new HttpCookie(".ASPXAUTHMEMBER", encTicket));
                return Json(new { success = true, redirectUrl = Url.Action("Index", "Home") });
            }
            return Json(new { success = false, message = "Vui lòng nhập đủ thông tin" });
        }
        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var checkUser = _unitOfWork.UserRepository.GetQuery(a => a.Email.Equals(model.Email)).SingleOrDefault();
                if (checkUser != null)
                {
                    ModelState.AddModelError("", @"Email đã được sử dụng!! Vui lòng nhập Email khác");
                    return Json(new { success = false, message = "Email đã được sử dụng!! Vui lòng nhập Email khác" });
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
                        AvatarSocial = null,
                        TypeRegister = TypeRegister.Website,
                        Url = HtmlHelpers.ConvertToUnSign(null, model.FullName),
                    };
                    _unitOfWork.UserRepository.Insert(user);
                    _unitOfWork.Save();
                    var count = _unitOfWork.UserRepository.GetQuery(a => a.Url == user.Url).Count();
                    if (count > 1)
                    {
                        user.Url += "-" + user.Id;
                        _unitOfWork.Save();
                    }
                    var userData = user.Avatar + "|" + user.Id + "|" + user.Email + "|" + user.FullName + "|" + user.Url + "|" + user.AvatarSocial;
                    var ticket = new FormsAuthenticationTicket(2, user.Email.ToLower(), DateTime.Now, DateTime.Now.AddDays(30), true,
                        userData, FormsAuthentication.FormsCookiePath);
                    var encTicket = FormsAuthentication.Encrypt(ticket);
                    Response.Cookies.Add(new HttpCookie(".ASPXAUTHMEMBER", encTicket));
                    return Json(new { success = true, message = "Đăng ký thành công" });
                }
            }
            return Json(new { success = false, message = "Vui lòng nhập đủ thông tin" });
        }
        public PartialViewResult QickRegister()
        {
            return PartialView();
        }

        [Route("notfound/{status}")]
        public ActionResult NotFound(int status)
        {
            ViewBag.Status = status;
            ViewBag.Content = "";
            if (status == 404)
            {
                ViewBag.Content = "Không tìm thấy trang này !!!";
            }
            else
            {
                ViewBag.Content = "Bạn không có quyền truy cập trang này !!!";
            }
            return View();
        }
        protected override void Dispose(bool disposing)
        {
            _unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}