using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GlobeWork.Models
{
    public class Company
    {
        [Key]
        [ForeignKey("Employer")]
        public int EmployerId { get; set; }
        [Display(Name = "Tên công ty"), Required(ErrorMessage = "Hãy nhập tên công ty"), StringLength(100, ErrorMessage = "Tối đa 100 ký tự"), UIHint("TextBox")]
        public string Name { get; set; }
        [Display(Name = "WebSite"), StringLength(100, ErrorMessage = "Tối đa 100 ký tự"), UIHint("TextBox")]
        public string WebsiteUrl { get; set; }
        [Display(Name = "Mô tả"), UIHint("EditorBox")]
        public string Body { get; set; }
        [Display(Name = "Ngày thành lập")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime EstablishmentDate { get; set; }
        [Display(Name = "Hiện trang chủ")]
        public bool ShowHome { get; set; }
        [Display(Name = "Độ tuổi trung bình") , Required(ErrorMessage ="Chưa nhập độ tuổi trung bình")]
        public string Age { get; set; }
        [Display(Name = "Email nhận tin ứng tuyển"), StringLength(100, ErrorMessage = "Tối đa 100 ký tự"), EmailAddress(ErrorMessage = "Email không hợp lệ"), UIHint("TextBox")]
        public string Email { get; set; }
        [Display(Name = "Địa chỉ"), UIHint("TextBox")]
        public string Address { get; set; }
        [Display(Name = "Quy mô công ty"), StringLength(50, ErrorMessage = "Tối đa 50 ký tự"), UIHint("NumberBox")]
        public string CompanySize { get; set; }
        [Display(Name = "Đường dẫn"), StringLength(500, ErrorMessage = "Tối đa 500 ký tự"), UIHint("TextBox")]
        public string Url { get; set; }
        public DateTime Vipdate { get; set; }
        public string Cover { get; set; }
        public string Avatar { get; set; }
        // Info
        [Display(Name = "Giới thiệu"), UIHint("EditorBox")]
        public string Introduct { get; set; }
        [Display(Name = "Mã nhúng Youtube")]
        public string VideoYoutube { get; set; }
        [Display(Name = "Mã nhúng Google Map")]
        public string GoogleMap { get; set; }
        [Display(Name = "Sản phẩm dịch vụ"), UIHint("EditorBox")]
        public string Product { get; set; }
        [Display(Name = "Số điện thoại"), RegularExpression(@"^\(?(09|03|07|08|05)\)?[-. ]?([0-9]{8})$", ErrorMessage = "Số điện thoại không đúng định dạng!"),
         Required(ErrorMessage = "Hãy nhập số điện thoại"), StringLength(10, ErrorMessage = "Tối đa 20 ký tự"), UIHint("TextBox")]
        public string Phone { get; set; }

        [ForeignKey("City")]
        public int? CityId { get; set; }
        public virtual City City { get; set; }
        public virtual ICollection<Career> Careers { get; set; }
        public virtual Employer Employer { get; set; }
        public virtual ICollection<JobPost> JobPosts { get; set; }
        public virtual ICollection<Candidate> Candidates { get; set; }
        public virtual ICollection<Follow> Followers { get; set; }
        public Company()
        {
            ShowHome = false;
            Vipdate = DateTime.Now;
        }
    }

}