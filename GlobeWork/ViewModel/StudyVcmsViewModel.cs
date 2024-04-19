using GlobeWork.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GlobeWork.ViewModel
{
    public class ListStudyVcmsViewModel
    {
        public IPagedList<StudyAbroad> StudyAbroads { get; set; }
        public string Name { get; set; }
        public string Sort { get; set; }
        public int? Status { get; set; }
        public int? StatusTime { get; set; }
        public int? CountruyId { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string CareerIds { get; set; }
        public string CountryIds { get; set; }
        public SelectList StatuSelectList { get; set; }
        public IEnumerable<Career> Careers { get; set; }
        public IEnumerable<Country> Countries { get; set; }
        public SelectList CitySelectList { get; set; }
        public SelectList SkillSelectList { get; set; }
        public SelectList JobTypeSelectList { get; set; }
        public SelectList RankSelectList { get; set; }

        public ListStudyVcmsViewModel()
        {
            var status = new Dictionary<int, string> { { 1, "Đã đăng" }, { 2, "Chờ đăng" } };
            StatuSelectList = new SelectList(status, "Key", "Value");
        }
    }
    public class CreateStudyVcmsViewModel
    {
        public StudyAbroad StudyAbroad { get; set; }
        public IEnumerable<Career> Careers { get; set; }
        public IEnumerable<StudyAbroadCategory> StudyAbroadCategories { get; set; }
        public IEnumerable<City> Citys { get; set; }
        [Display(Name = "Số ngày hiển thị nổi bật"), RegularExpression(@"\d+", ErrorMessage = "Chỉ nhập số nguyên dương"), UIHint("NumberBox")]
        public int Date { get; set; }
        [Display(Name ="Trừ tiền")]
        public bool TruTien { get; set; }

    }
}