using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GlobeWork.Models
{
    public class JobPost
    {
        public int Id { get; set; }
        [Display(Name = "Tên vị trí*", Description = "Tên vị trí dài tối đa 150 ký tự"),
         Required(ErrorMessage = "Hãy nhập tiêu đề"), StringLength(150, ErrorMessage = "Tối đa 150 ký tự"),
         UIHint("TextBox")]
        public string Name { get; set; }
        [Display(Name = "Mô tả công việc"), UIHint("EditorBox")]
        public string Body { get; set; }
        [Display(Name = "Yêu cầu công việc"), UIHint("EditorBox")]
        public string Requirement { get; set; }
        [Display(Name = "Đường dẫn"), StringLength(500, ErrorMessage = "Tối đa 500 ký tự"), UIHint("TextBox")]
        public string Url { get; set; }
        [ForeignKey("Company")]
        [Display(Name = "Nhà tuyển dụng"), UIHint("TextBox")]
        public int CompanyId { get; set; }
        [Display(Name = "Email nhận hồ sơ ứng tuyển"), StringLength(100, ErrorMessage = "Tối đa 100 ký tự"), EmailAddress(ErrorMessage = "Email không hợp lệ"), UIHint("TextBox")]
        public string Email { get; set; }
        [Display(Name = "Hình ảnh đại diện"), StringLength(500)]
        public string Image { get; set; }
        [Display(Name = "Đăng bài")]
        public bool Active { get; set; }
        [Display(Name = "WebSite nhận thông tin"), StringLength(100, ErrorMessage = "Tối đa 100 ký tự"), UIHint("TextBox")]
        public string WebsiteUrl { get; set; }
        [Display(Name = "Nổi bật")]
        public bool Special { get; set; }
        [Display(Name = "Hiện trang chủ")]
        public bool ShowHome { get; set; }
        [Display(Name = "Mức lương tối thiểu"), DisplayFormat(DataFormatString = "{0:N0}đ")]
        public decimal? SalalyMin { get; set; }
        [Display(Name = "Mức lương tối đa"), DisplayFormat(DataFormatString = "{0:N0}đ")]
        public decimal? SalalyMax { get; set; }
        public int View { get; set; }
        [Display(Name = "Ngày đăng"), UIHint("DateTimePicker"), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime CreateDate { get; set; }
        [Display(Name = "Ngày sửa gần nhất"), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime LastEditDate { get; set; }
        [Display(Name = "Ngày hết hạn"), UIHint("DateTimePicker"), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? ExpirationDate { get; set; }
        public int? CityId { get; set; }
        [ForeignKey("Rank")]
        public int RankId { get; set; }
        [ForeignKey("JobType")]
        public int JobTypeId { get; set; }
        public virtual Company Company { get; set; }
        public virtual ICollection<City> Cities { get; set; }
        public virtual JobType JobType { get; set; }
        public virtual Rank Rank { get; set; }
        public virtual ICollection<Candidate> Candidates { get; set; }
        //public virtual ICollection<Tag> Tags { get; set; }
        public virtual ICollection<Skill> Skills { get; set; }
        public virtual ICollection<Career> Careers { get; set; }
        public virtual ICollection<ApplyJob> ApplyJobs { get; set; }
        public JobPost()
        {
            CreateDate = DateTime.Now;
            LastEditDate = DateTime.Now;
            View = 1;
            Active = true;
        }

        public enum Currency
        {
            USD,
            VND
        }
    }
}