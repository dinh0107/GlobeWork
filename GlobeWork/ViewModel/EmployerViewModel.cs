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
    public class EmployerViewModel
    {
        public Employer Employer { get; set; }
        [DisplayFormat(DataFormatString = "{0:N0} VNĐ")]
        public decimal Amount { get ; set; }
        public int JobPost {  get; set; }
        public int Cv {  get; set; }
    }
    public class InsertEmployerViewModel
    {
        public JobPost JobPost { get; set; }
        public IEnumerable<Career> Careers { get; set; }
        public IEnumerable<Skill> Skills { get; set; }
        public IEnumerable<JobType> JobTypes { get; set; }
        public IEnumerable<Rank>  Ranks { get; set; }
        [Display(Name = "Mức lương tối thiểu"),  DisplayFormat(DataFormatString = "{0:N0}đ")]
        public string SalalyMin { get; set; }
        [Display(Name = "Mức lương tối đa"),  DisplayFormat(DataFormatString = "{0:N0}đ")]
        public string SalalyMax { get; set; }
        //public SelectList CitySelectList { get; set; }
        [Display(Name = "Thành phố")]
        public IEnumerable<City>  Citys { get; set; }
        //public int CityId { get; set; }
        [Display(Name = "Số ngày"), Required(ErrorMessage = "Hãy nhập thứ tự"), RegularExpression(@"\d+", ErrorMessage = "Chỉ nhập số nguyên")]
        public int DateHot { get; set; }
    }
    public class InserCompanyEmployerViewModel
    {
        public Company Company { get; set; }
        public IEnumerable<Career> Careers { get; set; }
        public IEnumerable<JobType> JobTypes { get; set; }
        public IEnumerable<Rank> Ranks { get; set; }
        public IEnumerable<City> Cities { get; set; }
        public DateTime DateTime { get; set; }
        [Display(Name = "Số ngày"), Required(ErrorMessage = "Hãy nhập thứ tự"), RegularExpression(@"\d+", ErrorMessage = "Chỉ nhập số nguyên")]
        public int DateHot { get; set; }
    }
    public class ChangeInfoViewModel
    {
        public Employer Employer { get; set;}
        public IEnumerable<Rank> Ranks { get; set; }
    }
    public class ChangePasswordUserViewModel
    {
        [Display(Name = "Mật khẩu mới"), StringLength(20, MinimumLength = 6, ErrorMessage = "Mật khẩu từ 4 đến 20 ký tự"), Required(ErrorMessage = "Bạn chưa nhập mật khẩu mới"), UIHint("Password")]
        public string Password { get; set; }
        [Display(Name = "Nhập lại mật khẩu mới"), Required(ErrorMessage = "Bạn chưa nhập lại mật khẩu"), System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Nhập lại mật khẩu không chính xác"), UIHint("Password")]
        public string ConfirmPassword { get; set; }
        [Display(Name = "Mật khẩu hiện tại"), Required(ErrorMessage = "Bạn chưa nhập mật khẩu hiện tại"), UIHint("Password")]
        public string OldPassword { get; set; }
    }
    public class ListMyJobPost
    {
        public IPagedList<JobPost> JobPosts { get; set; }
        public string Name { get; set; }
        public int Type {  get; set; }
    }
    public class ListMyJobStudyAbroadViewModel
    {
        public IPagedList<StudyAbroad> StudyAbroads { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
    }
    public class InsertStudyAbroadViewModel
    {
        public StudyAbroad StudyAbroad { get; set; }
        public IEnumerable<Career> Careers { get; set; }
        public IEnumerable<StudyAbroadCategory> StudyAbroadCategories { get; set; }
        public IEnumerable<City> Citys { get; set; }
        [Display(Name = "Số ngày"), Required(ErrorMessage = "Hãy nhập thứ tự"), RegularExpression(@"\d+", ErrorMessage = "Chỉ nhập số nguyên")]
        public int DateHot { get; set; }
    }
    public class ListCvViewModel
    {
        public IPagedList<ApplyJob> ApplyJobs { get; set; }    
        public string Name { get; set; }
        public int? Status { get; set; }
        public int JobId { get; set; }
        public int Type { get; set; }
    }
    public class NaptienViewModel
    {
        public Employer Employer { get; set; }
        [DisplayFormat(DataFormatString = "{0:N0} VNĐ")]
        public decimal TongNap { get; set; }
    }
}