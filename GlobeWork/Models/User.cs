using Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Web;

namespace GlobeWork.Models
{
    public class User
    {
        public int Id { get; set; }
        [Display(Name ="Họ và tên")]
        public string FullName { get; set; }
        [StringLength(50), Display(Name = "Tên đăng nhập")]
        public string Username { get; set; }
        [StringLength(100), Required(ErrorMessage = "Bạn chưa nhập Email"), UIHint("TextBox"), Display(Name = "Email")]
        public string Email { get; set; }
        [StringLength(60), UIHint("Password"), Display(Name = "Mật khẩu")]
        public string Password { get; set; }
        [StringLength(20), Display(Name = "Số điện thoại")]
        public string Phone { get; set; }
        [StringLength(500)]
        public string Avatar { get; set; }
        [StringLength(500)]
        public string Cover { get; set; }
        [Display(Name = "Hoạt động")]
        public bool Active { get; set; }
        public string Url { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Description { get; set; }
        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }
        [Display(Name = "Giới tính")]
        public string  Gender { get; set; }

        [Display(Name = "Chiều cao")]
        public string Height { get; set; }
        [Display(Name = "Cân nặng")]
        public string Weight { get; set; }
        //[Display(Name = "Sức khỏe")]
        //public string Health { get; set; }

        [Display(Name = "Chức vụ")]
        public string Classtify { get; set; }
        [Display(Name = "Tên công ty")]
        public string CompanyName { get; set; }
        //Facebook Info
        public string FaceBookId { get; set; }
        //Google Info
        public string GoogleId { get; set; }
        //Linkedin Info
        public string LinkedinId { get; set; }
        public string Token { get; set; }
        public string AvatarSocial { get; set; }
        public DateTime CreateDate { get; set; }
        public TypeRegister TypeRegister { get; set; }
        public virtual Skill Skill { get; set; }
        public virtual Marriage  Marriage { get; set; }
        public virtual Health Health { get; set; }
        public virtual ICollection<UserLog> UserLog { get; set; }
        public virtual ICollection<Education> Educations { get; set; }
        public virtual ICollection<Experiences> Experiences { get; set; }
        public virtual ICollection<Certificate> Certificates { get; set; }
        public virtual ICollection<Project> Projects { get; set; }
        public virtual ICollection<UserSkill> UserSkill { get; set; }
        public virtual ICollection<Activity> Activities { get; set; }
        public virtual ICollection<ApplyJob> ApplyJobs { get; set; }
        public virtual ICollection<UserViewLog> ViewLogs { get; set; }
        public User()
        {
            CreateDate = DateTime.Now;
            Active = true;
            Token = HtmlHelpers.RandomCode(50);
        }
    }
    public enum TypeRegister
    {
        [Display(Name = "Website")]
        Website,
        [Display(Name = "Facebook")]
        Facebook,
        [Display(Name = "Google")]
        Google,
        [Display(Name = "Linkedin")]
        Linkedin,
    }
    public class UserLog
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; }
        public DateTime CreateDate { get; set; }
        public virtual User User { get; set; }
        public bool Status { get; set; }
        public UserLog()
        {
            CreateDate = DateTime.Now;
        }
    }
    public class Education
    {
        public int Id { get; set; }
        //[Required(ErrorMessage = "Chưa nhập ngành học")]
        public string Majors { get; set; }
        [Required(ErrorMessage = "Chưa nhập tên trường")]
        public string School { get; set; }
        [Display(Name = "Mô tả"), StringLength(500, ErrorMessage = "Tối đa 500 ký tự"), UIHint("TextArea")]
        public string Description { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int UserId { get; set; }
        public bool Active { get; set; }
        public User User { get; set; }
    }
    public class Experiences
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Chưa nhập vị trí")]
        public string Position { get; set; }
        [Required(ErrorMessage = "Chưa nhập tên công ty")]
        public string Company { get; set; }
        [Display(Name = "Mô tả"), StringLength(500, ErrorMessage = "Tối đa 500 ký tự"), UIHint("TextArea")]
        public string Description { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Image { get; set; }
        public int UserId { get; set; }
        public bool Active { get; set; }
        public User User { get; set; }
    }
    public class UserSkill
    {
        public int Id { get; set; }
        [Display(Name ="Tên kỹ năng"), Required(ErrorMessage ="Chưa nhập tên kỹ năng")]
        public string Name { get; set; }
        public int Star { get ; set; }
        [Display(Name = "Mô tả"), StringLength(500, ErrorMessage = "Tối đa 500 ký tự"), UIHint("TextArea")]
        public string Description { get; set; }
        public DateTime CreateDate { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public UserSkill()
        {
            CreateDate = DateTime.Now;
        }
    }
    public class Certificate
    {
        public int Id { get; set; }
        [Display(Name ="Tên chứng chỉ"),Required(ErrorMessage = "Chưa nhập tên chứng chỉ"),]
        public string Name { get; set; }
        [Display(Name = "Tổ chức"), Required(ErrorMessage = "Chưa nhập tên tổ chức"),]
        public string Description { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int UserId { get; set; }
        [StringLength(500)]
        public string Url { get; set; }
        public string Image { get; set; }
        public bool Active { get; set; }
        public User User { get; set; }
    }
    public class Project
    {
        public int Id { get; set; }
        [Display(Name = "Tên dự án"), Required(ErrorMessage = "Chưa nhập tên dự án"),]
        public string Name { get; set; }
        [Display(Name = "Khách hàng"), Required(ErrorMessage = "Chưa nhập tên Khách hàng"),]
        public string Customer { get; set; }
        [Display(Name = "Vị trí"), Required(ErrorMessage = "Chưa nhập vị trí")]
        public string Postion { get; set; }
        [Display(Name = "Số thành viên"), Required(ErrorMessage = "Chưa nhập số thành viên")]
        public string Member { get; set; }
        [Display(Name = "Công nghệ"), Required(ErrorMessage = "Chưa nhập tên công nghệ")]
        public string Tech { get; set; }
        [Display(Name = "Mô tả"), StringLength(500, ErrorMessage = "Tối đa 500 ký tự"), UIHint("TextArea")]
        public string Description { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int UserId { get; set; }
        public string Image { get; set; }
        public bool Active { get; set; }
        [StringLength(500)]
        public string Url { get; set; }
        public User User { get; set; }
    }
    public class Activity
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Tên tổ chức")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Chưa nhập vị trí")]
        public string Position { get; set; }
        [Display(Name = "Mô tả"), StringLength(500, ErrorMessage = "Tối đa 500 ký tự"), UIHint("TextArea")]
        public string Description { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Image { get; set; }
        public int UserId { get; set; }
        public bool Active { get; set; }
        public User User { get; set; }
    }
    public class UserViewLog
    {
        public int Id { get; set; }
        public int UserId { get; set; } 
        public DateTime ViewedAt { get; set; }
        public UserViewLog()
        {
            ViewedAt = DateTime.Now;
        }
    }


    public enum Marriage
    {
        [Display(Name ="Độc thân")]
        Single,
        [Display(Name = "Đã kết hôn")]
        Married,
        [Display(Name = "Đã ly hôn")]
        Divorced
    }
    public enum Health
    {
        [Display(Name = "Tốt")]
        Good,
        [Display(Name = "Không tốt")]
        Bad,
    }
}