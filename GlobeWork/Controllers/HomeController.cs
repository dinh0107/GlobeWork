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

namespace GlobeWork.Controllers
{
    [MemberLoginFilter]
    public class HomeController : Controller
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();
        public ConfigSite ConfigSite => (ConfigSite)HttpContext.Application["ConfigSite"];
        private string MemberEmail => RouteData.Values["Email"].ToString();
        private new User User => _unitOfWork.UserRepository.GetQuery(a => a.Active && a.Email == MemberEmail).SingleOrDefault();

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
            var model = new HomeViewModel
            {
                Careers = _unitOfWork.CareerRepository.GetQuery(a => a.Hot && a.Active, o => o.OrderBy(a => a.Sort), 16)
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
        public ActionResult Job()
        {
            return View();
        }
        [Route("job/chi-tiet/{url}")]
        public ActionResult JobDetail(string url)
        {
            var job = _unitOfWork.JobPostRepository.GetQuery(a => a.Active && a.Url == url).FirstOrDefault();
            if (job == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var jobCompany = _unitOfWork.JobPostRepository.GetQuery(a => a.Active && a.CompanyId == job.CompanyId, o => o.OrderByDescending(a => a.CreateDate), 3);
            var jobRelated = _unitOfWork.JobPostRepository.GetQuery(a => a.Active && (a.CareerId == job.CareerId && a.Id != job.Id), o => o.OrderByDescending(a => a.CreateDate), 5);
            var jobCareers = job.Company.Careers.Select(c => c.Id).ToList();
            var companyRelate = _unitOfWork.CompanyRepository.GetQuery(a => a.UserId != job.Company.UserId && a.Careers.Any(c => jobCareers.Contains(c.Id))).Take(5);
            var follow = _unitOfWork.FollowRepository.GetQuery(a => a.UserId == User.Id);
            var like = _unitOfWork.LikeRepository.GetQuery(a => a.UserID == User.Id && a.JobId == job.Id);
            var model = new JobDetailViewModel
            {
                Job = job,
                JobCompanys = jobCompany,
                JobPosts = jobRelated,
                Companies = companyRelate,
                Follows = follow,
                Likes = like,
            };
            return View(model);
        }

        #region Action Job
        [HttpPost]
        public JsonResult Follow(int id)
        {
            var company = _unitOfWork.CompanyRepository.GetQuery(a => a.UserId == id).FirstOrDefault();
            if (company == null)
            {
                return Json(new { success = false, message = "Quá trình thực hiện không thành công" });
            }
            var follow = new Follow()
            {
                CompanyId = company.UserId,
                UserId = User.Id
            };
            var follows = _unitOfWork.FollowRepository.GetQuery(a => a.UserId == User.Id && a.CompanyId == company.UserId).Count();
            if (follows >= 1)
            {
                return Json(new { success = true, message = "Theo dõi công ty thành công !!!" });
            }
            _unitOfWork.FollowRepository.Insert(follow);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Theo dõi công ty thành công !!!" });
        }

        [HttpPost]
        public JsonResult UnFollow(int id)
        {
            var company = _unitOfWork.CompanyRepository.GetQuery(a => a.UserId == id).FirstOrDefault();
            if (company == null)
            {
                return Json(new { success = false, message = "Quá trình thực hiện không thành công" });
            }
            var follow = _unitOfWork.FollowRepository.GetQuery(a => a.UserId == User.Id && a.CompanyId == company.UserId).FirstOrDefault();
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
            var like = new Like()
            {
                JobId = job.Id,
                UserID = User.Id
            };
            var likes = _unitOfWork.LikeRepository.GetQuery(a => a.UserID == User.Id && a.JobId == job.Id).Count();
            if (likes >= 1)
            {
                return Json(new { success = true, message = "Lưu tin thành công !!!" });
            }
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
        public ActionResult Employer(string url)
        {
            var company = _unitOfWork.CompanyRepository.GetQuery(a => a.Url == url).FirstOrDefault();
            if(company == null)
            {
                return RedirectToAction("Index");
            }
            var job = _unitOfWork.JobPostRepository.GetQuery(a => a.CompanyId == company.UserId , l => l.OrderByDescending(a => a.Hot), 6);
            var follow = _unitOfWork.FollowRepository.GetQuery(a => a.UserId == User.Id && a.CompanyId == company.UserId);
            var model = new DetailCompanyViewModel
            {
                JobPosts = job,
                Company = company,
                Follows = follow,
            };
            return View(model);
        }
        public ActionResult Recruitment()
        {
            return View();
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
    }
}