using Helpers;
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

namespace GlobeWork.Controllers
{
    [MemberLoginFilter]
    public class HomeController : Controller
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();
        public ConfigSite ConfigSite => (ConfigSite)HttpContext.Application["ConfigSite"];
        private int Id => Convert.ToInt32(RouteData.Values["Id"]);
        private new User User => _unitOfWork.UserRepository.GetById(Id);

        public PartialViewResult Header()
        {
            return PartialView();
        }
        public PartialViewResult Footer()
        {
            return PartialView();
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
            var model = new HomeViewModel
            {
                Careers = _unitOfWork.CareerRepository.GetQuery(a => a.Hot && a.Active, o => o.OrderBy(a => a.Sort), 16),
                SearchCareers = _unitOfWork.CareerRepository.GetQuery(a => a.Active),
                Cities = _unitOfWork.CityRepository.Get(a => a.Active),
                JobTypes = _unitOfWork.JobTypeRepository.Get(),
                Ranks = _unitOfWork.RankRepository.Get(),
                Items = items,
                Banners = banner,
                Skills = _unitOfWork.SkillRepository.GetQuery(a => a.ShowHome, o => o.OrderBy(a => a.SkillName), 10),
            };
            return View(model);
        }
        public PartialViewResult GetFilter()
        {
            var model = new GetFilterViewModel
            {
                Cities = _unitOfWork.CityRepository.GetQuery(a => a.Active && a.Home, o => o.OrderBy(a => a.Sort)),
                Careers = _unitOfWork.CareerRepository.GetQuery(a => a.Active && a.ShowHome, o => o.OrderByDescending(a => a.CreateDate))
            };
            return PartialView(model);
        }

        public PartialViewResult GetJob(int cityId = 0, int type = 0)
        {
            var job = _unitOfWork.JobPostRepository.GetQuery(a => a.Active && a.Hot != null && a.Hot > DateTime.Now);
            switch (type)
            {
                case 1:
                    job = job.OrderBy(a => a.CityId);
                    break;
                case 2:
                    job = job.OrderByDescending(a => a.Wages);
                    break;
                case 3:
                    job = job.OrderBy(a => a.Experiences);
                    break;
                case 4:
                    job = job.OrderBy(a => a.CareerId);
                    break;
            }
            if (cityId > 0)
            {
                job = job.Where(a => a.CityId == cityId);
            }
            var model = new GetJobHotViewModel
            {
                JobPosts = job
            };
            return PartialView(model);
        }

        public ActionResult StudyAbroad()
        {
            return View();
        }
        public ActionResult StudyAbroadCategory()
        {
            return View();
        }
        public ActionResult StudyAbroadDetail()
        {
            return View();
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
                job = job.Where(a => a.Cities.Any(l => l.Id == cityId));
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
                Banners = _unitOfWork.BannerRepository.GetQuery(a => a.Active && a.GroupId == 3, o => o.OrderBy(a => a.Sort)),
                Careers = _unitOfWork.CareerRepository.GetQuery(a => a.Active, o => o.OrderByDescending(a => a.CreateDate)),
                Cities = _unitOfWork.CityRepository.Get(orderBy: a => a.OrderBy(l => l.Id)),
                CareerHot = _unitOfWork.CareerRepository.GetQuery(a => a.Hot && a.Active, o => o.OrderBy(a => a.Sort), 16)
            };
            return View(model);
        }
        [Route("job/chi-tiet/{url}.html")]
        public ActionResult JobDetail(string url)
        {
            var job = _unitOfWork.JobPostRepository.GetQuery(a => a.Active && a.Url == url).FirstOrDefault();
            if (job == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var jobCompany = _unitOfWork.JobPostRepository.GetQuery(a => a.Active && (a.CompanyId == job.CompanyId && a.Id != job.Id), o => o.OrderByDescending(a => a.CreateDate), 3);
            var jobRelated = _unitOfWork.JobPostRepository.GetQuery(a => a.Active && (a.CareerId == job.CareerId && a.Id != job.Id), o => o.OrderByDescending(a => a.CreateDate), 5);
            var jobCareers = job.Company.Careers.Select(c => c.Id).ToList();
            var companyRelate = _unitOfWork.CompanyRepository.GetQuery(a => a.EmployerId != job.Company.EmployerId && a.Careers.Any(c => jobCareers.Contains(c.Id))).Take(5);
            var follow = _unitOfWork.FollowRepository.GetQuery(a => a.UserId == User.Id);
            var like = _unitOfWork.LikeRepository.GetQuery(a => a.UserID == User.Id);
            var model = new JobDetailViewModel
            {
                Job = job,
                JobCompanys = jobCompany,
                JobPosts = jobRelated,
                Companies = companyRelate,
                Follows = follow,
                Likes = like,
                ApplyJobs = _unitOfWork.ApplyJobRepository.GetQuery(a => a.UserId == User.Id && a.JobPostId == job.Id)
            };
            return View(model);
        }

        [Route("viec-lam-tot-nhat")]
        public ActionResult JobHot(int? page, string name = "", int cityId = 0, int careerId = 0, int wage = 0)
        {
            var pageNumber = page ?? 1;
            var job = _unitOfWork.JobPostRepository.GetQuery(a => a.Active && (a.Hot != null && a.Hot > DateTime.Now), o => o.OrderByDescending(a => a.CreateDate));
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
                job = job.Where(a => a.Cities.Any(l => l.Id == cityId));
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
                Name = name,
                CityId = cityId,
                Wage = wage,
                CareerId = careerId,
                Likes = like,
                Careers = _unitOfWork.CareerRepository.GetQuery(a => a.Active, o => o.OrderByDescending(a => a.CreateDate)),
                Cities = _unitOfWork.CityRepository.Get(orderBy: a => a.OrderBy(l => l.Id))
            };
            return View(model);
        }
        [Route("tim-kiem")]
        public ActionResult Search(int? page,  int jobTypeId = 0 , int rankId = 0, string keyword = "", int cityId = 0, int careerId = 0, int wage = 0, int companyId = 0)
        {
            var pageNumber = page ?? 1;
            var job = _unitOfWork.JobPostRepository.GetQuery(a => a.Active && a.Name.ToLower().Contains(keyword.ToLower()), o => o.OrderByDescending(a => a.CreateDate));
            if (companyId > 0)
            {
                job = job.Where(a => a.CompanyId == companyId);
            }
            if (careerId > 0)
            {
                job = job.Where(a => a.CareerId == careerId);
            }
            if (cityId > 0)
            {
                job = job.Where(a => a.Cities.Any(l => l.Id == cityId));
            }
            if(jobTypeId > 0)
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
                CityId = cityId,
                Wage = wage,
                CareerId = careerId,
                CompanyId = companyId,
                Likes = like,
                Careers = _unitOfWork.CareerRepository.GetQuery(a => a.Active, o => o.OrderByDescending(a => a.CreateDate)),
                Cities = _unitOfWork.CityRepository.Get(orderBy: a => a.OrderBy(l => l.Id))
            };
            return View(model);
        }
        [Route("tim-viec-lam-tu-van/{url}")]
        public ActionResult JobCategory(int? page, string url, int wage = 0, int cityId = 0, string name = "", int type = 0)
        {
            var pageNumber = page ?? 1;
            var category = _unitOfWork.CareerRepository.GetQuery(a => a.Active && a.Code == url).FirstOrDefault();
            if (category == null)
            {
                return RedirectToAction("Index", "Home");
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
                job = job.Where(a => a.Cities.Any(l => l.Id == cityId));
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
                Cities = _unitOfWork.CityRepository.Get(orderBy: a => a.OrderBy(l => l.Id)),
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
        #endregion

        [Route("cong-ty/{url}")]
        public ActionResult Employer(string url, string name)
        {
            var company = _unitOfWork.CompanyRepository.GetQuery(a => a.Url == url).FirstOrDefault();
            if (company == null)
            {
                return RedirectToAction("Index");
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
                return RedirectToAction("Index");
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
                job = job.Where(a => a.Cities.Any(l => l.Id == cityId));
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
                CityId = cityId,
                Url = url,
                Careers = _unitOfWork.CareerRepository.GetQuery(a => a.Active, o => o.OrderByDescending(a => a.CreateDate)),
                Cities = _unitOfWork.CityRepository.Get(orderBy: a => a.OrderBy(l => l.Id))
            };
            return View(model);
        }
        public ActionResult ListCompany()
        {
            return View();
        }
        public ActionResult CompanyTop()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        [Route("chi-tiet/{url}.html")]
        public ActionResult UserProfile(string url)
        {
            var detail = _unitOfWork.UserRepository.GetQuery(a => a.Active && a.Url == url).FirstOrDefault();
            if (detail == null)
            {
                return RedirectToAction("Index", "Home");
            }
            //var id = Convert.ToInt32(RouteData.Values["Id"]);
            //var user = _unitOfWork.UserRepository.GetQuery(a => a.Id == id).FirstOrDefault();
            //if (user == null)
            //{
            //    return RedirectToAction("Index", "Home");
            //}
            var edu = _unitOfWork.EducationRepository.GetQuery(a => a.UserId == detail.Id, o => o.OrderBy(a => a.Id));
            var exp = _unitOfWork.ExperienceRepository.GetQuery(a => a.UserId == detail.Id, o => o.OrderBy(a => a.Id));
            var model = new UserInfoViewModel
            {
                User = detail,
                Experiences = exp,
                Educations = edu
            };
            return View(model);
        }
        public PartialViewResult ApplyForm()
        {
            return PartialView();
        }
        [HttpPost]
        public ActionResult Apply(ApplyViewModel model, FormCollection fc)
        {
            if (ModelState.IsValid)
            {
                var job = _unitOfWork.JobPostRepository.GetById(model.JobId);
                if (job == null)
                {
                    return Json(new { success = false, message = "Quá trình thực hiện không thành công" });
                }
                var file = Request.Files["ApplyJob.FileUpload"];
                if (file != null && file.ContentLength > 0)
                {
                    if (!HtmlHelpers.CheckFileExt(file.FileName, "doc|docx|pdf"))
                    {
                        ModelState.AddModelError("", @"Chỉ chấp nhận định dạng doc, docx, pdf");
                        return View(model);
                    }
                    else
                    {
                        if (file.ContentLength > 40000 * 1024)
                        {
                            ModelState.AddModelError("", @"Dung lượng lớn hơn 40MB. Hãy thử lại");
                            return View(model);
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
                model.ApplyJob.Status = ApplyJobStatus.Waiting;
                model.ApplyJob.JobPostId = job.Id;
                _unitOfWork.ApplyJobRepository.Insert(model.ApplyJob);
                _unitOfWork.Save();
                return RedirectToAction("JobDetail", new { url = job.Url });
            }
            return View(model);
        }
        protected override void Dispose(bool disposing)
        {
            _unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}