using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using GlobeWork.Models;
using PagedList;

namespace GlobeWork.ViewModel
{
    public class ChangePasswordModel
    {
        [Display(Name = "Mật khẩu hiện tại"), Required(ErrorMessage = "Hãy nhập mật khẩu hiện tại"), UIHint("Password")]
        public string OldPassword { get; set; }
        [Display(Name = "Mật khẩu mới"), Required(ErrorMessage = "Hãy nhập mật khẩu mới"),
         StringLength(16, MinimumLength = 4, ErrorMessage = "Mật khẩu từ 4, 16 ký tự"), UIHint("Password")]
        public string Password { get; set; }
        [Display(Name = "Nhập lại mật khẩu"), System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Nhập lại mật khẩu không chính xác"),
         UIHint("Password")]
        public string ConfirmPassword { get; set; }
    }

    public class AdminLoginModel
    {
        [Display(Name = "Tên đăng nhập"), Required(ErrorMessage = "Hãy nhập tên đăng nhập")]
        public string Username { get; set; }
        [Display(Name = "Mật khẩu"), Required(ErrorMessage = "Hãy nhập mật khẩu")]
        public string Password { get; set; }
    }
    public class InfoAdminViewModel
    {
        public int Articles { get; set; }
        public int Contacts { get; set; }
        public int Admins { get; set; }
        public int Banners { get; set; }
        public int Job { get; set; }
        public int Study { get; set; }
        public int ApplyJob { get; set; }
        public int ApplyStudy { get; set; }
        public int Employer { get; set; }
        public int User { get; set; }
        //public int Showrooms { get; set; }
    }
    public class EditAdminViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Hoạt động")]
        public bool Active { get; set; }
        [Display(Name = "Tên đăng nhập"), Required(ErrorMessage = "Bạn chưa điền tên đăng nhập"), StringLength(20, MinimumLength = 4, ErrorMessage = "Tên đăng nhập từ 4 - 20 ký tự"), RegularExpression(@"[a-zA-Z0-9]+$", ErrorMessage = "Chấp nhận chữ cái không dấu, số, viết liền không khoảng trống"), UIHint("TextBox")]
        public string Username { get; set; }
        [Display(Name = "Mật khẩu"), UIHint("Password"), StringLength(20, MinimumLength = 6, ErrorMessage = "Mật khẩu từ 6 - 20 ký tự")]
        public string Password { get; set; }
    }

    public class ListUserViewModel
    {
        public IPagedList<User> Users { get; set; }
        public string Name { get; set; }
    }

    public class EditUserViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Hoạt động")]
        public bool Active { get; set; }
        [Display(Name = "Tên đăng nhập"), Required(ErrorMessage = "Bạn chưa điền tên đăng nhập"), StringLength(20, MinimumLength = 4, ErrorMessage = "Tên đăng nhập từ 4 - 20 ký tự"), RegularExpression(@"[a-zA-Z0-9]+$", ErrorMessage = "Chấp nhận chữ cái không dấu, số, viết liền không khoảng trống"), UIHint("TextBox")]
        public string Username { get; set; }
        [Display(Name = "Mật khẩu"), UIHint("Password"), StringLength(20, MinimumLength = 6, ErrorMessage = "Mật khẩu từ 6 - 20 ký tự")]
        public string Password { get; set; }
    }

    public class AdminEditUserViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Tên đăng nhập"), StringLength(50, ErrorMessage = "Tối đa 50 ký tự")]
        public string Username { get; set; }
        [DisplayName("Mật khẩu"), StringLength(60, MinimumLength = 6, ErrorMessage = "Mật khẩu từ 6 - 60 ký tự")]
        public string Password { get; set; }
        [Display(Name = "Số điện thoại"), Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string Phone { get; set; }
        [Display(Name = "Hoạt động")]
        public bool Active { get; set; }
        [StringLength(50, ErrorMessage = "Tối đa 50 ký tự"), Display(Name = "Địa chỉ email"), EmailAddress(ErrorMessage = "Email không chính xác"), Required(ErrorMessage = "Bạn vui lòng nhập email")]
        public string EmailRegister { get; set; }
        [StringLength(500)]
        public string Avatar { get; set; }
        public DateTime CreateDate { get; set; }
    }
    public class EmployerEditUserViewModel
    {
        public int Id { get; set; }
        [DisplayName("Mật khẩu"), StringLength(60, MinimumLength = 6, ErrorMessage = "Mật khẩu từ 6 - 60 ký tự")]
        public string Password { get; set; }
        [Display(Name = "Số điện thoại"), Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string Phone { get; set; }
        [StringLength(50, ErrorMessage = "Tối đa 50 ký tự"), Display(Name = "Địa chỉ email"), EmailAddress(ErrorMessage = "Email không chính xác"), Required(ErrorMessage = "Bạn vui lòng nhập email")]
        public string EmailRegister { get; set; }
        [StringLength(500)]
        public string Avatar { get; set; }
        public DateTime CreateDate { get; set; }
        public bool Active { get; set; }
    }

    public class ListMemberViewModel
    {
        public IPagedList<User> Users { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int? Status { get; set; }
        public int? TypeUser { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public SelectList StatuSelectList { get; }
        public SelectList TypeUserSelectList { get; }

        public ListMemberViewModel()
        {
            var status = new Dictionary<int, string> { { 1, "Kích hoạt" }, { 2, "Chưa kích hoạt" } };
            StatuSelectList = new SelectList(status, "Key", "Value");
            var typeUser = new Dictionary<int, string> { { 1, "Ứng viên" }, { 2, "Nhà tuyển dụng" } };
            TypeUserSelectList = new SelectList(typeUser, "Key", "Value");
        }

    }
    public class ListEmployerViewModel
    {
        public IPagedList<Employer> Employers { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int? Status { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public SelectList StatuSelectList { get; }

        public ListEmployerViewModel()
        {
            var status = new Dictionary<int, string> { { 1, "Kích hoạt" }, { 2, "Chưa kích hoạt" } };
            StatuSelectList = new SelectList(status, "Key", "Value");
        }

    }
    public class PublicMoneyViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Số tiền"), UIHint("MoneyBox"), DisplayFormat(DataFormatString = "{0:N0}đ")]
        public string Price { get; set; }
    }
    public class ListCountruyViewModel
    {
        public IPagedList<Country> Countries { get; set; }
        public string Name { get; set; }
    }
    public class InsertCityViewModel
    {
        public City City { get; set; }
        public IEnumerable<Country> Countries { get; set; }
    }
    public class InsertStudyAbroadCategoryViewModel
    {
        public StudyAbroadCategory StudyAbroadCategory { get; set; }
        public IEnumerable<Country> Countries { get; set; }
    }
    public class ConfigViewModel
    {
        public ConfigSite ConfigSite { get; set; }
        [Display(Name = "Giá hiển thị tin tuyển dụng nổi bật"), UIHint("MoneyBox"), DisplayFormat(DataFormatString = "{0:N0}đ")]
        public string PriceJob { get; set; }
        [Display(Name = "Giá hiển thị tin du học nổi bật"), UIHint("MoneyBox"), DisplayFormat(DataFormatString = "{0:N0}đ")]
        public string PriceStudyAbroad { get; set; }
        [Display(Name = "Giá hiển thị công ty nổi bật"), UIHint("MoneyBox"), DisplayFormat(DataFormatString = "{0:N0}đ")]
        public string PriceCompany { get; set; }
        [Display(Name = "Giá hiển thị bài viết nổi bật"), UIHint("MoneyBox"), DisplayFormat(DataFormatString = "{0:N0}đ")]
        public string PriceArticle { get; set; }
    }
    public class ListCityViewModel
    {
        public IEnumerable<Country> Countries { get; set; }
        public IEnumerable<City> Citys { get; set; }
    }
}