using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GlobeWork.Models;
using PagedList;

namespace GlobeWork.ViewModel
{

    public class DashboardHomeViewModel
    {
        public User User { get; set; }
        public Company Company { get; set; }
        public Candidate Candidate { get; set; }
        public IEnumerable<ApplyJob> ApplyJobs { get; set; }
        public IEnumerable<JobPost> JobPosts { get; set; }
        public int ToTalAppLyJob { get; set; }
        public int View { get; set; }
    }
    public class ProfileUserViewModel
    {
        [Display(Name = "Ảnh đại diện")]
        public string Avatar { get; set; }
        [Display(Name = "Số điện thoại"), RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$",
                      ErrorMessage = "Số điện thoại không đúng !!!."), StringLength(20, ErrorMessage = "Tối đa 20 ký tự"), UIHint("TextBox")]
        public string Phone { get; set; }
        public string Type { get; set; }
        [Display(Name = "Họ và tên"), StringLength(100, ErrorMessage = "Tối đa 100 ký tự"), UIHint("TextBox")]
        public string Name { get; set; }

        [Display(Name = "Ngày sinh"), UIHint("DateTimePicker"), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public string DateOfBirth { get; set; }

        [Display(Name = "Hiển thị thông tin"), UIHint("TextBox")]
        public bool ActiveProfile { get; set; }

        [Display(Name = "CV")]
        public string FileCv { get; set; }

        [Display(Name = "Học Vấn"), UIHint("EditorBox")]
        public string Education { get; set; }

        [Display(Name = "Kinh nghiệm"), UIHint("EditorBox")]
        public string Experience { get; set; }

        [Display(Name = "Email nhận thông tin"), StringLength(100, ErrorMessage = "Tối đa 100 ký tự"), EmailAddress(ErrorMessage = "Email không hợp lệ"), UIHint("TextBox")]
        public string Email { get; set; }

        [Display(Name = "Mức lương tối thiểu"), StringLength(10, ErrorMessage = "Tối đa 10 ký tự"), UIHint("TextBox")]
        public string SalalyMin { get; set; }

        [Display(Name = "Địa chỉ"), StringLength(100, ErrorMessage = "Tối đa 100 ký tự"), UIHint("TextBox")]
        public string Address { get; set; }

        [Display(Name = "Giới thiệu"), StringLength(500, ErrorMessage = "Tối đa 500 ký tự"), UIHint("TextArea")]
        public string Introduce { get; set; }

        [Display(Name = "Chức danh nghề nghiệp"), StringLength(100, ErrorMessage = "Tối đa 200 ký tự"), UIHint("TextBox")]
        public string CareerTitle { get; set; }

        [Display(Name = "Giới tính"), UIHint("TextBox")]
        public Gender Genders { get; set; }

        public virtual ICollection<City> Citys { get; set; }
        public virtual ICollection<Career> Careers { get; set; }
        public virtual ICollection<Skill> Skills { get; set; }
        public virtual ICollection<JobType> JobTypes { get; set; }
        public virtual ICollection<Rank> Ranks { get; set; }


        [Display(Name = "Tên công ty"), StringLength(100, ErrorMessage = "Tối đa 100 ký tự"), UIHint("TextBox")]
        public string NameCompany { get; set; }
        [Display(Name = "WebSite"), StringLength(100, ErrorMessage = "Tối đa 100 ký tự"), UIHint("TextBox")]
        public string WebsiteUrl { get; set; }
        [Display(Name = "Mô tả"), UIHint("EditorBox")]
        public string BodyCompany { get; set; }
        [Display(Name = "Quy mô công ty"), StringLength(10, ErrorMessage = "Tối đa 10 ký tự"), UIHint("NumberBox")]
        public string CompanySize { get; set; }
        [Display(Name = "Đường dẫn"), StringLength(500, ErrorMessage = "Tối đa 500 ký tự"), UIHint("TextBox")]
        public string Url { get; set; }
        [Display(Name = "Ngày thành lập"), UIHint("DateTimePicker"), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public string EstablishmentDate { get; set; }
        public int? CityId { get; set; }
        public virtual ICollection<Career> CareersCompany { get; set; }




        [StringLength(500, ErrorMessage = "Tối đa 500 ký tự"), Display(Name = "Đường dẫn Facebook"),
         Url(ErrorMessage = "Đường dẫn không chính xác"), UIHint("TextBox")]
        public string Facebook { get; set; }
        [StringLength(500, ErrorMessage = "Tối đa 500 ký tự"), Display(Name = "Đường dẫn Linkedin"),
         Url(ErrorMessage = "Đường dẫn không chính xác"), UIHint("TextBox")]
        public string Linkedin { get; set; }
        [StringLength(500, ErrorMessage = "Tối đa 500 ký tự"), Display(Name = "Đường dẫn Instagram"),
         Url(ErrorMessage = "Đường dẫn không chính xác"), UIHint("TextBox")]
        public string Instagram { get; set; }
        [StringLength(500, ErrorMessage = "Tối đa 500 ký tự"), Display(Name = "Đường dẫn Youtube"),
        Url(ErrorMessage = "Đường dẫn không chính xác"), UIHint("TextBox")]
        public string Youtube { get; set; }
        [StringLength(500, ErrorMessage = "Tối đa 500 ký tự"), Display(Name = "Đường dẫn Twitter"),
         Url(ErrorMessage = "Đường dẫn không chính xác"), UIHint("TextBox")]
        public string Twitter { get; set; }
        [StringLength(50, ErrorMessage = "Tối đa 50 ký tự"), Display(Name = "Tài khoản Zalo"), UIHint("TextBox")]
        public string Zalo { get; set; }




        public SelectList CareerSelectList { get; set; }
        public SelectList RankSelectList { get; set; }
        public SelectList JobTypeSelectList { get; set; }
        public SelectList CitySelectList { get; set; }
        public enum Gender
        {
            [Display(Name = "Nam")]
            Male,
            [Display(Name = "Nữ")]
            Female,
            [Display(Name = "Khác")]
            Other,
        }
    }

    public class CreatePostViewModel
    {
        public JobPost JobPost { get; set; }
        public Company Company { get; set; }
        public string MinSalary { get; set; }
        public string MaxSalary { get; set; }
        public IEnumerable<Career> Careers { get; set; }
        public IEnumerable<Country> Countries { get; set; }
        public SelectList SkillSelectList { get; set; }
        public SelectList RankSelectList { get; set; }
        public SelectList JobTypeSelectList { get; set; }
        public SelectList CitySelectList { get; set; }
        [Display(Name = "Số ngày hiển thị nổi bật"), RegularExpression(@"\d+", ErrorMessage = "Chỉ nhập số nguyên dương"), UIHint("NumberBox")]
        public int Date { get; set;}
        [Display(Name = "Trừ tiền")]
        public bool TruTien { get; set; }
    }

    public class ListJobPostViewModel
    {
        public IPagedList<JobPost> JobPosts { get; set; }
        public string Name { get; set; }
        public string Sort { get; set; }
        public int? Status { get; set; }
        public int? StatusTime { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public int? CareerIds { get; set; }
        public int? CountruyId { get; set; }
        public int? CityIds { get; set; }
        public string JobTypeIds { get; set; }
        public string RankIds { get; set; }
        public string SkillIds { get; set; }
        public SelectList StatuSelectList { get; set; }
        public IEnumerable<Career> Careers { get; set; }
        public IEnumerable<City> Cities { get; set; }
        public IEnumerable<Country> Countries { get; set; }
        public SelectList CitySelectList { get; set; }
        public SelectList SkillSelectList { get; set; }
        public SelectList JobTypeSelectList { get; set; }
        public SelectList RankSelectList { get; set; }

        public ListJobPostViewModel()
        {
            var status = new Dictionary<int, string> { { 1, "Đã đăng" }, { 2, "Chờ đăng" } };
            StatuSelectList = new SelectList(status, "Key", "Value");
        }
    }

    public class ListApplyJobViewModel
    {
        public IPagedList<ApplyJob> ApplyJobs { get; set; }
        public string Sort { get; set; }
        public string Keyword { get; set; }
        public int jobPostId { get; set; }
        public int? Status { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public SelectList StatusSelectList { get; set; }
        public ListApplyJobViewModel()
        {
            var status = new Dictionary<int, string> { { 0, "Đang chờ" }, { 1, "Đã duyệt" }, { 2, "Đã từ chối" } };
            StatusSelectList = new SelectList(status, "Key", "Value");

        }
    }

    public class ListCompanyFollow
    {
        public IPagedList<Company> Companies { get; set; }
        public string Keyword { get; set; }
    }

    public class ListJobPostFollowViewModel
    {
        public IPagedList<JobPost> JobPosts { get; set; }
        public string Keyword { get; set; }
    }
}