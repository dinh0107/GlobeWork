﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GlobeWork.Models
{
    public class City 
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Hãy nhập tên thành phố"), Display(Name = "Tên thành phố"), StringLength(50, ErrorMessage = "Tối đa 50 ký tự")]
        public string Name { get; set; }
        [Display(Name = "Thứ tự"), Required(ErrorMessage = "Hãy nhập thứ tự"), RegularExpression(@"\d+", ErrorMessage = "Chỉ nhập số nguyên")]
        public int Sort { get; set; }
        [Display(Name = "Hoạt động")]
        public bool Active { get; set; }
        [StringLength(20)]
        public string Prefix { get; set; }
        [Display(Name = "Phí Ship"), UIHint("MoneyBox")]
        public int? ShipFee { get; set; }
        [Display(Name ="Hiện trang chủ")]
        public bool Home { get; set; }
        [Display(Name = "Quốc gia")]
        public int CountruyId { get; set; }
        public virtual Country Country { get; set; }
        public virtual ICollection<District> Districts { get; set; }
        public virtual ICollection<JobPost> JobPosts { get; set; }
    }
}