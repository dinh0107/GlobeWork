﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GlobeWork.Models
{
    public class Banner
    {
        public int Id { get; set; }
        [Display(Name = "Tên banner"), Required(ErrorMessage = "Hãy nhập tên"), StringLength(100, ErrorMessage = "Tối đa 100 ký tự"), UIHint("TextBox")]
        public string BannerName { get; set; }
        [Display(Name = "Tên danh mục"), StringLength(100, ErrorMessage = "Tối đa 100 ký tự"), UIHint("TextBox")]
        public string Category { get; set; }
        [Display(Name = "Slogan"), StringLength(500, ErrorMessage = "Tối đa 500 ký tự"), UIHint("TextArea")]
        public string Slogan { get; set; }
        [Display(Name = "Hình ảnh"), StringLength(500)]
        public string Image { get; set; }
        [Display(Name = "Hoạt động")]
        public bool Active { get; set; }
        [Display(Name = "Vị trí quảng cáo"), Required(ErrorMessage = "Hãy chọn vị trí quảng cáo"), UIHint("GroupId")]
        public int GroupId { get; set; }
        [Display(Name = "Đường dẫn"), StringLength(500, ErrorMessage = "Tối đa 500 ký tự"), UIHint("TextBox")]
        public string Url { get; set; }
        [Display(Name = "Thứ tự"), Required(ErrorMessage = "Hãy nhập thứ tự"), RegularExpression(@"\d+", ErrorMessage = "Chỉ nhập số nguyên"), UIHint("NumberBox")]
        public int Sort { get; set; }
        [Display(Name = "Nội dung giới thiệu"), UIHint("EditorBox")]
        public string Content { get; set; }
    }
    public class Partner
    {
        public int Id { get; set; }
        [Display(Name ="Tên đối tác")] 
        public string Name { get; set; }
        [Display(Name = "Đường dẫn"), StringLength(500, ErrorMessage = "Tối đa 500 ký tự"), UIHint("TextBox")]
        public string Url { get; set; }
        [Display(Name = "Hình ảnh"), StringLength(500)]
        public string Image { get; set; }
        [Display(Name = "Thứ tự"), Required(ErrorMessage = "Hãy nhập thứ tự"), RegularExpression(@"\d+", ErrorMessage = "Chỉ nhập số nguyên"), UIHint("NumberBox")]
        public int Sort { get; set; }
        [Display(Name = "Hiện chân trang")]
        public bool Footer { get; set; }
        [Display(Name = "Hoạt động")]
        public bool Active { get; set; }
    }
}