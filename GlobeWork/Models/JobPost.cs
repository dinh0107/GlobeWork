using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.NetworkInformation;

namespace GlobeWork.Models
{
    public class JobPost
    {
        public int Id { get; set; }
        [Display(Name = "Tên vị trí*", Description = "Tên vị trí dài tối đa 150 ký tự"),
         Required(ErrorMessage = "Hãy nhập tiêu đề"), StringLength(150, ErrorMessage = "Tối đa 150 ký tự"),
         UIHint("TextBox")]
        public string Name { get; set; }
        
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
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime? Hot { get; set; }
        [Display(Name = "Mức lương tối thiểu")]
        public string SalalyMin { get; set; }
        [Display(Name = "Mức lương tối đa")]
        public string SalalyMax { get; set; }
        public int View { get; set; }
        [Display(Name = "Ngày đăng"), UIHint("DateTimePicker"), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime CreateDate { get; set; }
        [Display(Name = "Ngày sửa gần nhất"), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime LastEditDate { get; set; }
        [Display(Name = "Ngày hết hạn"), UIHint("DateTimePicker"), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? ExpirationDate { get; set; }
        //Yêu cầu 

        [Display(Name="Yêu cầu kinh nghiệm")]
        public string Experience { get; set; }
        [Display(Name = "Giới tính")]
        public string Gender { get; set; }
        [Display(Name = "Địa điểm làm việc") , Required(ErrorMessage ="Chưa nhập địa chỉ cụ thể")]
        public string Address { get; set; }
        [Display(Name = "Mô tả công việc"), UIHint("EditorBox")]
        public string Body { get; set; }
        [Display(Name = "Yêu cầu ứng viên"), UIHint("EditorBox")]
        public string Requirement { get; set; }
        [Display(Name = "Quyền lợi"), UIHint("EditorBox")]
        public string Benefits { get; set; }
        [Display(Name = "Địa điểm làm việc"), UIHint("EditorBox")]
        public string WorkLocation { get; set; }
        [Display(Name = "Số lượng"), Required(ErrorMessage = "Hãy nhập thứ tự"), RegularExpression(@"\d+", ErrorMessage = "Chỉ nhập số nguyên")]
        public int Quantity { get; set; }
        [Display(Name = "Yêu cầu")]
        public string Request { get; set; }
        [Display(Name = "Mã")]
        public string Code { get; set; }
        //Hết yêu cầu
        public int? CityId { get; set; }
        [ForeignKey("Rank")]
        public int RankId { get; set; }
        [ForeignKey("JobType")]
        public int JobTypeId { get; set; }
        [ForeignKey("Career")]
        public int CareerId { get; set; }
        public virtual Company Company { get; set; }
        public virtual ICollection<City> Cities { get; set; }
        public virtual JobType JobType { get; set; }
        public virtual Rank Rank { get; set; }
        public virtual Career  Career { get; set; }
        public virtual ICollection<Candidate> Candidates { get; set; }
        public virtual ICollection<Skill> Skills { get; set; }
        public virtual ICollection<ApplyJob> ApplyJobs { get; set; }
        public ICollection<Like> Likes { get; set; }
        [Display(Name = "Mức lương")]
        public Wage Wages { get; set; }
        public Experience Experiences { get; set; }
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
    public enum Wage
    {
        [Display(Name = "Dưới 10 triệu")]
        Duoi10,
        [Display(Name = "Từ 10 - 15 triệu")]
        Tu10den15,
        [Display(Name = "Từ 15 - 20 triệu")]
        Tu15den20,
        [Display(Name = "Từ 20 - 25 triệu")]
        Tu20den25,
        [Display(Name = "Từ 25 - 30 triệu")]
        Tu25den30,
        [Display(Name = "Từ 30 - 50 triệu")]
        Tu30den50,
        [Display(Name = "Trên 50 triệu")]
        Tren50,
        [Display(Name = "Thỏa thuận")]
        ThoaThuan,
    }
    public enum Experience
    {
        [Display(Name = "Chưa có kinh nghiệm")]
        Chuaco,
        [Display(Name = "1 năm trở xuống")]
        Motnamtroxuong,
        [Display(Name = "2 năm")]
        Hainam,
        [Display(Name = "3 năm")]
        Banam,
        [Display(Name = "Từ 4 - 5 năm")]
        tu4den5,
        [Display(Name = "Trên 5 năm")]
        tren5,
    }
}