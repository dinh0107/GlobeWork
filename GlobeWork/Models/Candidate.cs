using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GlobeWork.Models
{
    public class Candidate
    {
        [Key]
        [ForeignKey("User")]
        public int UserId { get; set; }
        [Display(Name = "Họ và tên"), StringLength(100, ErrorMessage = "Tối đa 100 ký tự"), Required(ErrorMessage = "Vui lòng nhập họ tên"), UIHint("TextBox")]
        public string Name { get; set; }
        [Display(Name = "Ngày sinh"), UIHint("DateTimePicker"), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", NullDisplayText = "-")]
        public DateTime? DateOfBirth { get; set; }
        [Display(Name = "Hiển thị thông tin")]
        public bool ActiveProfile { get; set; }
        [Display(Name = "CV")]
        public string FileCv { get; set; }
        [Display(Name = "Học Vấn"), UIHint("EditorBox")]
        public string Education { get; set; }
        [Display(Name = "Kinh nghiệm"), UIHint("EditorBox")]
        public string Experience { get; set; }
        [Display(Name = "Email nhận thông tin"), StringLength(100, ErrorMessage = "Tối đa 100 ký tự"), EmailAddress(ErrorMessage = "Email không hợp lệ"), UIHint("TextBox")]
        public string Email { get; set; }
        [Display(Name = "Mức lương tối thiểu"), StringLength(10, ErrorMessage = "Tối đa 10 ký tự"), UIHint("TextBox")]
        public string SalalyMin { get; set; }
        [Display(Name = "Địa chỉ"), StringLength(100, ErrorMessage = "Tối đa 100 ký tự"), UIHint("TextBox")]
        public string Address { get; set; }
        [Display(Name = "Giới thiệu"), StringLength(500, ErrorMessage = "Tối đa 500 ký tự"), UIHint("TextArea")]
        public string Introduce { get; set; }
        [Display(Name = "Chức danh nghề nghiệp"), StringLength(100, ErrorMessage = "Tối đa 100 ký tự"), UIHint("TextBox")]
        public string CareerTitle { get; set; }
        [Display(Name = "Giới tính"), UIHint("TextBox")]
        public Gender Genders { get; set; }
        [Display(Name = "Đường dẫn"), StringLength(500, ErrorMessage = "Tối đa 500 ký tự"), UIHint("TextBox")]
        public string Url { get; set; }
        public virtual ICollection<JobPost> Posts { get; set; }
        public virtual ICollection<City> Citys { get; set; }
        public virtual ICollection<Career> Careers { get; set; }
        public virtual ICollection<Skill> Skills { get; set; }
        public virtual ICollection<JobType> JobTypes { get; set; }
        public virtual ICollection<Rank> Ranks { get; set; }
        //public virtual ICollection<ApplyJob> ApplyJobs { get; set; }
        public virtual ICollection<Company> Companys { get; set; }
        public virtual User User { get; set; }

        public enum Gender
        {
            [Display(Name = "Nam")]
            Male,
            [Display(Name = "Nữ")]
            Female,
            [Display(Name = "Khác")]
            Other,
        }
    }
}