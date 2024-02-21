using GlobeWork.Models;
using Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GlobeWork.Models
{
    public class Employer
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; }
        public string PhoneNumber { get; set; }
        public string CompanyName { get; set; }
        [StringLength(500)]
        public string Avatar { get; set; }
        //[Display(Name = "Địa chỉ")]
        //public string Address { get; set; }
        public bool Active { get; set; }
        public DateTime CreateDate { get; set; }
        public int RankId { get; set; }
        public string Token { get; set; }
        public virtual Rank Rank { get; set; }
        public int? CityId { get; set; }
        public int? DistrictId { get; set; }
        [Display(Name = "Tài khoản"), DisplayFormat(DataFormatString = "{0:N0}đ")]
        public decimal Amount { get; set; }
        public virtual City City { get; set; }
        public virtual District District { get; set; }
        public virtual ICollection<EmployerLog> EmployerLogs { get; set; }
        public virtual ICollection<Article> Articles { get; set; }
        public Employer()
        {
            CreateDate = DateTime.Now;
            Active = true;
            Token = HtmlHelpers.RandomCode(50);
        }
    }
    public class EmployerLog
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        [DisplayFormat(DataFormatString = "{0:N0}đ")]
        public decimal Amount { get; set; }
        public string Content { get; set; }
        public DateTime CreateDate { get; set; }
        public EmployerLogType EmployerLogType { get; set; }
        public virtual Employer Employer { get; set; }
        public EmployerLog()
        {
            CreateDate = DateTime.Now;
        }
    }
    public enum EmployerLogType
    {
        [Display(Name = "Cộng tiền")]
        PublicMoney,
        [Display(Name = "Trừ tiền")]
        Deduction,
        [Display(Name = "Đổi mật khẩu")]
        ChangePassword,
    }
}