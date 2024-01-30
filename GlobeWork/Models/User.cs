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
        [Display(Name = "Hoạt động")]
        public bool Active { get; set; }

        //Facebook Info
        public string FaceBookId { get; set; }
        //Google Info
        public string GoogleId { get; set; }
        public string Token { get; set; }
        public DateTime CreateDate { get; set; }
        public TypeUser TypeUser { get; set; }
        public TypeRegister TypeRegister { get; set; }
        public User()
        {
            CreateDate = DateTime.Now;
            Active = true;
            Token = HtmlHelpers.RandomCode(50);
        }
    }
    public enum TypeUser
    {
        [Display(Name ="Nhà tuyển dụng")]
        Employer,
        [Display(Name = "Người dùng")]
        User,
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
}