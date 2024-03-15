using GlobeWork.DAL;
using GlobeWork.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Configuration;
using System.Web.Mvc;

namespace GlobeWork.Controllers
{
    public class BaseController : Controller
    {
        public readonly UnitOfWork _unitOfWork = new UnitOfWork();
        public static string Email => WebConfigurationManager.AppSettings["email"];
        public static string PasswordEmail => WebConfigurationManager.AppSettings["password"];
        public decimal fiveM = 5000000;
        public decimal tenM = 10000000;
        public decimal fifteenM = 15000000;

        public SelectList CitySelectList => new SelectList(_unitOfWork.CityRepository.Get(orderBy: a => a.OrderBy(o => o.Name)), "Id", "Name");
        public SelectList RankSelectList => new SelectList(_unitOfWork.RankRepository.Get(orderBy: a => a.OrderBy(o => o.Name)), "Id", "Name");
        public SelectList JobTypeSelectList => new SelectList(_unitOfWork.JobTypeRepository.Get(orderBy: a => a.OrderBy(o => o.Name)), "Id", "Name");
        public SelectList CareerSelectList => new SelectList(_unitOfWork.CareerRepository.Get(orderBy: a => a.OrderBy(o => o.Name)), "Id", "Name");
        public SelectList SkillSelectList => new SelectList(_unitOfWork.SkillRepository.Get(orderBy: a => a.OrderBy(o => o.SkillName)), "Id", "SkillName");

        public IEnumerable<Career> Careers => _unitOfWork.CareerRepository.GetQuery(p => p.Active);
        public IEnumerable<City> Cities => _unitOfWork.CityRepository.GetQuery(p => p.Active);

        public JsonResult GetCities(string city = "")
        {
            var cities = _unitOfWork.CityRepository
                .GetQuery(orderBy: a => a.OrderBy(o => o.Name));
            return Json(cities, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            _unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}