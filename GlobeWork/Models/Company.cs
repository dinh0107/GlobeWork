using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GlobeWork.Models
{
    public class Company
    {
        [Key]
        [ForeignKey("User")]
        public int UserId { get; set; }
        [Display(Name = "Tên công ty"), Required(ErrorMessage = "Hãy nhập tên công ty"), StringLength(100, ErrorMessage = "Tối đa 100 ký tự"), UIHint("TextBox")]
        public string Name { get; set; }
        [Display(Name = "WebSite"), StringLength(100, ErrorMessage = "Tối đa 100 ký tự"), UIHint("TextBox")]
        public string WebsiteUrl { get; set; }
        [Display(Name = "Mô tả"), UIHint("EditorBox")]
        public string Body { get; set; }
        [Display(Name = "Ngày thành lập"), UIHint("DateTimePicker"), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? EstablishmentDate { get; set; }
        [Display(Name = "Hiện trang chủ")]
        public bool ShowHome { get; set; }
        [Display(Name = "Email nhận tin ứng tuyển"), StringLength(100, ErrorMessage = "Tối đa 100 ký tự"), EmailAddress(ErrorMessage = "Email không hợp lệ"), UIHint("TextBox")]
        public string Email { get; set; }
        [Display(Name = "Địa chỉ"), UIHint("TextBox")]
        public string Address { get; set; }
        [Display(Name = "Quy mô công ty"), StringLength(10, ErrorMessage = "Tối đa 10 ký tự"), UIHint("NumberBox")]
        public string CompanySize { get; set; }
        [Display(Name = "Đường dẫn"), StringLength(500, ErrorMessage = "Tối đa 500 ký tự"), UIHint("TextBox")]
        public string Url { get; set; }
        public DateTime Vipdate { get; set; }
        [ForeignKey("City")]
        public int? CityId { get; set; }
        public virtual City City { get; set; }
        public virtual ICollection<Career> Careers { get; set; }
        public virtual User User { get; set; }
        public virtual ICollection<JobPost> JobPosts { get; set; }
        public virtual ICollection<Candidate> Candidates { get; set; }
        public Company()
        {
            ShowHome = false;
            Vipdate = DateTime.Now;
        }
    }

}