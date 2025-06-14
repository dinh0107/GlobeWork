﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public bool Active { get; set; } = true;

        [Display(Name = "Ngày đăng"), UIHint("DateTimePicker"), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        [Display(Name = "Số lượng"), Required(ErrorMessage = "Hãy nhập thứ tự"), RegularExpression(@"\d+", ErrorMessage = "Chỉ nhập số nguyên")]
        public int Quantity { get; set; }
        [Display(Name = "Mô tả"), UIHint("EditorBox")]
        public string Body { get; set; }
        [Display(Name = "Yêu cầu học viên"), UIHint("EditorBox")]
        public string Requirements { get; set; }
        [Display(Name = "Ưu đãi được hưởng"), UIHint("EditorBox")]
        public string Incentives { get; set; }
        [Display(Name = "Ngày hết hạn"), UIHint("DateTimePicker"), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? ExpirationDate { get; set; }
        public virtual Company Company { get; set; }
        public int View { get; set; } = 1;
        public int CategoryId { get; set; }
        public decimal? Min { get; set; }
        public decimal? Max { get; set; }
        public string Code { get; set; }
        [Display(Name="Cấp bậc")]
        public string Level { get; set; }
        [Display(Name = "Học bổng")]
        public bool Scholarship { get; set; }
        public DateTime? Hot { get; set; }
        public string ListImage { get; set; }
        public int? ServiceId { get; set; }
        public int? CareerId { get; set; }
        public virtual StudyAbroadCategory  StudyAbroadCategory { get; set; }    
        public virtual Career Careers { get; set; }
        //public virtual ICollection<City> Cities { get; set; }
        public virtual ICollection<Like> Likes { get; set; }
        public virtual ICollection<ApplyJob> ApplyJobs { get; set; }
        public TypeStudyAbroad  TypeStudyAbroad { get; set; }
        public Scholarship  WageScholarship { get; set; }
        public virtual Service Service { get; set; }
    }
    public enum TypeStudyAbroad
    {
        [Display(Name ="Du học")]
        StudyAbroad,
        [Display(Name = "Học bổng")]
        Scholarship,
    }
    public enum Scholarship
    {
        [Display(Name = "Từ 100 - 300 triệu")]
        Tu100den300,
        [Display(Name = "Từ 300 - 500 triệu")]
        Tu300den500,
        [Display(Name = "Trên 500 triệu")]
        Tren500,
        [Display(Name = "Học bổng toàn phần")]
        Toanphan,
    }
}