using Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public DateTime CreateDate { get; set; }
        public TypeRegister TypeRegister { get; set; }
        public virtual Skill Skill { get; set; }
        public virtual ICollection<UserLog> UserLog { get; set; }
        public virtual ICollection<Education> Educations { get; set; }
        public virtual ICollection<Experiences> Experiences { get; set; }
        public virtual ICollection<Certificate> Certificates { get; set; }
        public virtual ICollection<Project> Projects { get; set; }
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
        [Required(ErrorMessage = "Chưa nhập ngành học")]
        public string Majors { get; set; }
        [Required(ErrorMessage = "Chưa nhập tên trường")]
        public string School { get; set; }
        [Display(Name = "Mô tả"), StringLength(500, ErrorMessage = "Tối đa 500 ký tự"), UIHint("TextArea")]
        public string Description { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int UserId { get; set; }
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

        public int UserId { get; set; }
        public User User { get; set; }
    }
    public class Certificate
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Tên chứng chỉ")]
        public string Name { get; set; }
        [Display(Name = "Mô tả"), StringLength(500, ErrorMessage = "Tối đa 500 ký tự"), UIHint("TextArea")]
        public string Description { get; set; }
        public DateTime? Date { get; set; }
        public int UserId { get; set; }
        [StringLength(500)]
        public string Url { get; set; }
        public User User { get; set; }
    }
    public class Project
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Tên dự án")]
        public string Name { get; set; }
        [Display(Name = "Mô tả"), StringLength(500, ErrorMessage = "Tối đa 500 ký tự"), UIHint("TextArea")]
        public string Description { get; set; }
        public DateTime? StarDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int UserId { get; set; }
        [StringLength(500)]
        public string Url { get; set; }
        public User User { get; set; }
    }
}