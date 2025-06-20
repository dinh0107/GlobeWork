﻿using GlobeWork.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace GlobeWork.ViewModel
{
    //public class ListUserViewModel
    //{
    //    public IPagedList<User> Users { get; set; }
    //    [Display(Name = "Email")]
    //    public string Email { get; set; }
    //    public string Mobile { get; set; }
    //    public string Name { get; set; }
    //}

    //public class EditUserViewModel
    //{
    //    public int Id { get; set; }
    //    [Display(Name = "Họ và tên"), Required(ErrorMessage = "Hãy nhập họ và tên"), StringLength(50, ErrorMessage = "Tối đa 50 ký tự")]
    //    public string Username { get; set; }
    //    [DisplayName("Mật khẩu"), StringLength(20, MinimumLength = 6, ErrorMessage = "Mật khẩu từ 6 - 20 ký tự")]
    //    public string Password { get; set; }
    //    [Display(Name = "Số điện thoại"), Required(ErrorMessage = "Hãy nhập số điện thoại"), Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    //    public string PhoneNumber { get; set; }
    //    [StringLength(50, ErrorMessage = "Tối đa 50 ký tự"), Display(Name = "Địa chỉ email"), EmailAddress(ErrorMessage = "Email không chính xác"), Required(ErrorMessage = "Bạn vui lòng nhập email")]
    //    public string Email { get; set; }
    //    [Display(Name = "Địa chỉ"), Required(ErrorMessage = "Hãy nhập địa chỉ"), StringLength(200, ErrorMessage = "Tối đa 200 ký tự")]
    //    public string Address { get; set; }
    //    public string Avatar { get; set; }
    //    [Display(Name = "Ngày sinh")]
    //    public string Birthday { get; set; }
    //    public DateTime CreateDate { get; set; }
    //}

    public class UserLoginModel
    {
        [Display(Name = "Tên đăng nhập hoặc Email"), Required(ErrorMessage = "Hãy nhập email hoặc tên đăng nhập")]
        public string Email { get; set; }
        [Display(Name = "Mật khẩu"), Required(ErrorMessage = "Hãy nhập mật khẩu")]
        public string Password { get; set; }
    }

    public class RegisterViewModel
    {
        //[Display(Name = "Số điện thoại"), RegularExpression(@"^\(?(09|03|07|08|05)\)?[-. ]?([0-9]{8})$", ErrorMessage = "Số điện thoại không đúng định dạng!"),
        //Required(ErrorMessage = "Hãy nhập số điện thoại"), StringLength(10, ErrorMessage = "Tối đa 20 ký tự"), UIHint("TextBox")]
        //public string PhoneNumber { get; set; }
        [Display(Name = "Họ và tên"), Required(ErrorMessage = "Hãy nhập Họ và tên"), StringLength(100, ErrorMessage = "Tối đa 100 ký tự"), UIHint("TextBox")]
        public string FullName { get; set; }
        //[Display(Name = "Tên đăng nhập"), Required(ErrorMessage = "Hãy nhập tên đăng nhập"), RegularExpression(@"[a-zA-Z0-9._]{6,25}", ErrorMessage = "Tên đăng nhập từ 6 đến 25 ký tự. Chỉ ký tự không dấu, số nguyên, dấu . và _"), Remote("CheckUsername", "User")]
        //public string UserName { get; set; }
        [DisplayName("Mật khẩu"), Required(ErrorMessage = "Bạn chưa nhập mật khẩu"), StringLength(20, MinimumLength = 6, ErrorMessage = "Mật khẩu từ 6 - 20 ký tự")]
        public string Password { get; set; }
        [DisplayName("Nhập lại mật khẩu"), Required(ErrorMessage = "Bạn chưa nhập lại mật khẩu")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Mật khẩu không trùng khớp")]
        public string ConfirmPassword { get; set; }
        //[Display(Name = "Tên công ty"), StringLength(100, ErrorMessage = "Tối đa 100 ký tự"), UIHint("TextBox")]
        //public string NameCompany { get; set; }
        [Display(Name = "Email"), Required(ErrorMessage = "Vui lòng nhập Email"), StringLength(100, ErrorMessage = "Tối đa 100 ký tự"), EmailAddress(ErrorMessage = "Email không hợp lệ"), UIHint("TextBox"), Remote("CheckEmail", "User")]
        public string Email { get; set; }
    }
    public class ForgotPasswordViewModel
    {
        [Display(Name = "Email"), EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ"), Required(ErrorMessage = "Hãy nhập email của bạn")]
        public string Email { get; set; }
    }
    public class EmployerRegisterViewModel
    {
        [Display(Name = "Họ và tên"), Required(ErrorMessage = "Hãy nhập Họ và tên"), StringLength(100, ErrorMessage = "Tối đa 100 ký tự"), UIHint("TextBox")]
        public string FullName { get; set; }
        [DisplayName("Mật khẩu"), Required(ErrorMessage = "Bạn chưa nhập mật khẩu"), StringLength(20, MinimumLength = 6, ErrorMessage = "Mật khẩu từ 6 - 20 ký tự")]
        public string Password { get; set; }
        [DisplayName("Nhập lại mật khẩu"), Required(ErrorMessage = "Bạn chưa nhập lại mật khẩu")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Mật khẩu không trùng khớp")]
        public string ConfirmPassword { get; set; }
        [Display(Name = "Email"), Required(ErrorMessage = "Vui lòng nhập Email"), StringLength(100, ErrorMessage = "Tối đa 100 ký tự"), EmailAddress(ErrorMessage = "Email không hợp lệ"), UIHint("TextBox"), Remote("CheckEmail", "User")]
        public string Email { get; set; }
        [Display(Name = "Tên công ty"), Required(ErrorMessage = "Chưa nhập tên công ty")]
        public string CompanyName { get; set; }
        [Display(Name = "Giới tính")]
        public string Gender { get; set; }
        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }
        public SelectList CitySelectList { get; set; }
        public SelectList DistrictSelectList { get; set; }
        [Display(Name = "Thành phố")/*, Required(ErrorMessage = "Bạn hãy chọn thành phố")*/]
        public int? CityId { get; set; }
        [Display(Name = "Vị trí công tác"), Required(ErrorMessage = "Bạn hãy chọn Vị trí công tác")]
        public int? RankId { get; set; }
        [Display(Name = "Quận huyện")/*, Required(ErrorMessage = "Bạn hãy chọn quận huyện")*/]
        public int? DistrictId { get; set; }
        [Display(Name = "Số điện thoại"), RegularExpression(@"^\(?(09|03|07|08|05)\)?[-. ]?([0-9]{8})$", ErrorMessage = "Số điện thoại không đúng định dạng!"),
        Required(ErrorMessage = "Hãy nhập số điện thoại"), StringLength(10, ErrorMessage = "Tối đa 20 ký tự"), UIHint("TextBox")]
        public string PhoneNumber { get; set; }
        public IEnumerable<Rank> Ranks;
    }
    public class UserDashboardViewModel
    {
        public User User { get; set; }
    }


    public class ChangePasswordViewModel
    {
        [DisplayName("Mật khẩu hiện tại"), Required(ErrorMessage = "Bạn vui lòng nhập mật khẩu hiện tại"), StringLength(20, MinimumLength = 6, ErrorMessage = "Mật khẩu từ 6 - 20 ký tự")]
        public string OldPassword { get; set; }
        [DisplayName("Mật khẩu"), StringLength(20, MinimumLength = 6, ErrorMessage = "Mật khẩu từ 6 - 20 ký tự"), Required(ErrorMessage = "Bạn vui lòng nhập mật mới")]
        public string Password { get; set; }
        [DisplayName("Nhập lại mật khẩu"), System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Nhập lại mật khẩu không chính xác"), StringLength(20, MinimumLength = 6, ErrorMessage = "Mật khẩu từ 6 - 20 ký tự"), Required(ErrorMessage = "Bạn vui lòng nhập lại mật khẩu mới"),]
        public string ConfirmPassword { get; set; }
    }

    public class UpdateInfoViewModel
    {
        [Display(Name = "Họ và tên"), Required(ErrorMessage = "Hãy nhập họ và tên"), StringLength(50, ErrorMessage = "Tối đa 50 ký tự")]
        public string Fullname { get; set; }
        [Display(Name = "Số điện thoại"), Required(ErrorMessage = "Hãy nhập số điện thoại"), Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string PhoneNumber { get; set; }
        [StringLength(50, ErrorMessage = "Tối đa 50 ký tự"), Display(Name = "Email"), EmailAddress(ErrorMessage = "Email không chính xác"), Required(ErrorMessage = "Bạn vui lòng nhập email")]
        public string Email { get; set; }
        [Display(Name = "Địa chỉ"), Required(ErrorMessage = "Hãy nhập địa chỉ"), StringLength(200, ErrorMessage = "Tối đa 200 ký tự")]
        public string Address { get; set; }
        public string Avatar { get; set; }
        [Display(Name = "Ngày sinh")]
        public string Birthday { get; set; }
        [Display(Name = "Địa điểm làm việc"), Required(ErrorMessage = "Bạn hãy chọn thành phố")]
        public int? CityId { get; set; }

        public SelectList CitySelectList { get; set; }
        public SelectList DistrictSelectList { get; set; }
        public SelectList WardSelectList { get; set; }

        public UpdateInfoViewModel()
        {
            CitySelectList = new SelectList(new List<City>(), "Id", "Name");
        }
    }

    public class SetNewPasswordViewModel
    {
        public string Token { get; set; }
        [DisplayName("Mật khẩu"), Required(ErrorMessage = "Nhập mật khẩu"), StringLength(20, MinimumLength = 6, ErrorMessage = "Mật khẩu từ 6 - 20 ký tự")]
        public string Password { get; set; }
        [DisplayName("Nhập lại mật khẩu"), StringLength(20, MinimumLength = 6, ErrorMessage = "Mật khẩu không trùng khớp")]
        [System.ComponentModel.DataAnnotations.Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
    public class SetPasswordSocial
    {
        public int Id { get; set; } 
        [DisplayName("Mật khẩu"), Required(ErrorMessage = "Nhập mật khẩu"), StringLength(20, MinimumLength = 6, ErrorMessage = "Mật khẩu từ 6 - 20 ký tự")]
        public string Password { get; set; }
    }
    public class AdminListCompanyViewModel
    {
        public IPagedList<Company> Companies { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public int? Status { get; set; }
        public List<int> CareerIds { get; set; }
        public List<int> CityIds { get; set; }
        public SelectList CareerSelectList { get; set; }
        public SelectList CitySelectList { get; set; }
    }
    public class AdminListCandidateViewModel
    {
        public IPagedList<Candidate> Candidates { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int? Status { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public List<int> CareerIds { get; set; }
        public List<int> CityIds { get; set; }
        public List<int> JobTypeIds { get; set; }
        public List<int> RankIds { get; set; }
        public List<int> SkillIds { get; set; }
        public SelectList CareerSelectList { get; set; }
        public SelectList CitySelectList { get; set; }
        public SelectList SkillSelectList { get; set; }
        public SelectList JobTypeSelectList { get; set; }
        public SelectList RankSelectList { get; set; }
        public SelectList StatuSelectList { get; set; }

        public AdminListCandidateViewModel()
        {
            var status = new Dictionary<int, string> { { 1, "Công khai TT" }, { 2, "Không công khai TT" } };
            StatuSelectList = new SelectList(status, "Key", "Value");
        }

    }
    public class UploadJobViewModel
    {
        public SelectList CompaniesSelectList { get; set; }
        public int CompanyId { get; set; }
    }

    public class AdminEditCompanyViewModel
    {
        public Company Company { get; set; }
        public IEnumerable<Career> Careers { get; set; }
        public IEnumerable<JobType> JobTypes { get; set; }
        public IEnumerable<Rank> Ranks { get; set; }
        public IEnumerable<City> Cities { get; set; }

        [Display(Name = "Số ngày hiển thị nổi bật"), RegularExpression(@"\d+", ErrorMessage = "Chỉ nhập số nguyên dương"), UIHint("NumberBox")]
        public int Date { get; set; }
        [Display(Name = "Trừ tiền")]
        public bool TruTien { get; set; }
    }
    public class AdminEditCandidateViewModel
    {
        [Display(Name = "Mật khẩu"), UIHint("Password"), StringLength(20, MinimumLength = 6, ErrorMessage = "Mật khẩu từ 6 - 20 ký tự")]
        public string Password { get; set; }
        public Candidate Candidate { get; set; }
        public SelectList CareerSelectList { get; set; }
        public SelectList RankSelectList { get; set; }
        public SelectList JobTypeSelectList { get; set; }
        public SelectList CitySelectList { get; set; }
    }
    public class UserInfoViewModel
    {
        public UserViewLog UserViewLog { get; set; }
        public User User { get; set; }
        public bool IsAccount { get; set; }
        public IEnumerable<Education> Educations { get; set; }
        public Education EduInfo { get; set; }
        public IEnumerable<Experiences> Experiences { get; set; }
        public Experiences ExpInfo { get; set; }
        public IEnumerable<UserSkill> UserSkills { get; set; }
        public IEnumerable<Certificate> Certificates { get; set; }
        public IEnumerable<Project> Projects { get; set; }
        public IEnumerable<Activity> Activitys { get; set; }
        public int ViewsByWeek { get; set; }
        public int ViewsByMonth { get; set; }
        public int ViewsByYear { get; set; }
    }
    public class ChangeInfoUserViewModel
    {
        //public User User { get; set; }
        //public EducationViewModel Education { get; set; }
        //public ExperiencesViewModel Experience { get; set; }
        //public IEnumerable<Education> ListEducations { get; set; }
        //public IEnumerable<Experiences> ListExperiences { get; set; }
        //public List<EducationViewModel> Educations { get; set; }
        //public List<ExperiencesViewModel> Experiences { get; set; }

        //user
        [Display(Name = "Họ và tên"), Required(ErrorMessage = "Họ và tên không được để trống")]
        public string FullName { get; set; }
        [Display(Name = "Chức vụ")]
        public string Classtify { get; set; }
        [Display(Name = "Giới tính")]
        public string Gender { get; set; }
        [Display(Name = "Chiều cao")]
        public string Height { get; set; }
        [Display(Name = "Cân nặng")]
        public string Weight { get; set; }
        public int Date { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }
        [StringLength(100), Required(ErrorMessage = "Bạn chưa nhập Email"), UIHint("TextBox"), Display(Name = "Email")]
        public string Email { get; set; }
        public string Url { get; set; }
        [Display(Name = "Giới thiệu bản thân")]
        public string Description { get; set; }
        [StringLength(20), Display(Name = "Số điện thoại")]
        public string Phone { get; set; }
        [Display(Name = "Tình trạng hôn nhân")]
        public Marriage Marriage { get; set; }
        [Display(Name = "Tình trạng sức khỏe")]
        public Health Health { get; set; }

    }
    public class EducationViewModel
    {
        public int? Id { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Majors { get; set; }
        public string School { get; set; }
        public string Description { get; set; }
    }
    public class ExperiencesViewModel
    {
        public int? Id { get; set; }
        public string Position { get; set; }
        public string Company { get; set; }
        public string Description { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
    public class UserCompanyViewModel
    {
        public User User { get; set; }
        public IPagedList<Follow> Follows { get; set; }
    }
    public class UserLikeViewModel
    {
        public IPagedList<Like> Likes { get; set; }
        public User User { get; set; }
    }
    public class UserApplyViewModel
    {
        public IPagedList<ApplyJob> ApplyJobs { get; set; }
        public User User { get; set; }
        public int? Status { get; set; }
    }
    public class InsertEduViewModel
    {
        public Education Education { get; set; }
        public int StarMonth { get; set; }
        public int StarYear { get; set; }
        public int? EndMonth { get; set; }
        public int? EndYear { get; set; }
    }
    public class InsertExperienceViewModel
    {
        public Experiences  Experiences { get; set; }
        public int StarMonth { get; set; }
        public int StarYear { get; set; }
        public int? EndMonth { get; set; }
        public int? EndYear { get; set; }
    }
    public class InsertCertificateViewModel
    {
        public Certificate Certificate { get; set; }
        public int StarMonth { get; set; }
        public int StarYear { get; set; }
        public int? EndMonth { get; set; }
        public int? EndYear { get; set; }
    }
    public class InsertProjectViewModel
    {
        public Project Project { get; set; }
        public int StarMonth { get; set; }
        public int StarYear { get; set; }
        public int EndMonth { get; set; }
        public int EndYear { get; set; }
    }
    public class InsertActivityViewModel
    {
        public Activity Activity { get; set; }
        public int StarMonth { get; set; }
        public int StarYear { get; set; }
        public int? EndMonth { get; set; }
        public int? EndYear { get; set; }
    }
}