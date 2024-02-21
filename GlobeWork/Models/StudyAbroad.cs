using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace GlobeWork.Models
{
    public class StudyAbroad
    {
        public int Id { get; set; }
        [Display(Name = "Tiêu đề tuyển sinh", Description = "Tên vị trí dài tối đa 150 ký tự"),
         Required(ErrorMessage = "Hãy nhập tiêu đề"), StringLength(150, ErrorMessage = "Tối đa 150 ký tự"),
         UIHint("TextBox")]
        public string Name { get; set; }
        public string Image { get; set; }
        [Display(Name = "Giới tính")]
        public string Gender { get; set; }
        [Display(Name = "Địa điểm làm việc")]
        public string Address { get; set; }
        [Display(Name = "Trình độ học vấn, bằng cấp")]
        public string LearningProblems { get; set; }
        [Display(Name = "Sức khỏe")]
        public string Health { get; set; }
        [Display(Name = "Ngày hết hạn"), UIHint("DateTimePicker"), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? EndTime { get; set; }
        [ForeignKey("Company")]
        [Display(Name = "Nhà tuyển dụng"), UIHint("TextBox")]
        public int CompanyId { get; set; }
        [Display(Name = "Đường dẫn"), StringLength(500, ErrorMessage = "Tối đa 500 ký tự"), UIHint("TextBox")]
        public string Url { get; set; }
        [Display(Name = "Đăng bài")]
        public bool Active { get; set; }
        [Display(Name = "Ngày đăng"), UIHint("DateTimePicker"), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime CreateDate { get; set; }
        [Display(Name = "Số lượng"), Required(ErrorMessage = "Hãy nhập thứ tự"), RegularExpression(@"\d+", ErrorMessage = "Chỉ nhập số nguyên")]
        public int Quantity { get; set; }
        [Display(Name = "Mô tả"), UIHint("EditorBox")]
        public string Body { get; set; }
        [Display(Name = "Yêu cầu học viên"), UIHint("EditorBox")]
        public string Requirements { get; set; }
        [Display(Name = "Yêu đãi được hưởng"), UIHint("EditorBox")]
        public string Incentives { get; set; }
        [Display(Name = "Ngày hết hạn"), UIHint("DateTimePicker"), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? ExpirationDate { get; set; }
        public virtual Company Company { get; set; }
        public int View { get; set; }
        public int CategoryId { get; set; }
        public decimal? Min { get; set; }
        public decimal? Max { get; set; }
        public string Code { get; set; }
        [Display(Name="Cấp bậc")]
        public string Level { get; set; }
        public DateTime? Hot { get; set; }
        public string ListImage { get; set; }
        public virtual StudyAbroadCategory  StudyAbroadCategory { get; set; }    
        public virtual ICollection<Career> Careers { get; set; }
        public virtual ICollection<City> Cities { get; set; }
        public virtual ICollection<Like> Likes { get; set; }

        public Wage Wages { get; set; }

        public StudyAbroad()
        {
            CreateDate = DateTime.Now;
            View = 1;
            Active = true;
        }
    }

}