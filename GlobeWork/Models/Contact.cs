﻿using System;
using System.ComponentModel.DataAnnotations;

namespace GlobeWork.Models
{
    public class Contact
    {
        public int Id { get; set; }
        [Display(Name = "Họ và tên"), Required(ErrorMessage = "Hãy nhập họ tên"), UIHint("TextBox"), StringLength(100, ErrorMessage = "Tối đa 100 ký tự")]
        public string FullName { get; set; }
        [Display(Name = "Số điện thoại"), RegularExpression(@"^\(?(09|03|07|08|05)\)?[-. ]?([0-9]{8})$", ErrorMessage = "Số điện thoại không đúng định dạng!"),
         Required(ErrorMessage = "Hãy nhập số điện thoại"), StringLength(10, ErrorMessage = "Tối đa 20 ký tự"), UIHint("TextBox")]
        public string Mobile { get; set; }
        [Display(Name = "Email"), Required(ErrorMessage = "Hãy nhập Email"), StringLength(100, ErrorMessage = "Tối đa 100 ký tự"), EmailAddress(ErrorMessage = "Email không hợp lệ"), UIHint("TextBox")]
        public string Email { get; set; }
        [Display(Name = "Lời nhắn"), DataType(DataType.MultilineText), StringLength(4000)]
        public string Body { get; set; }
        public DateTime CreateDate { get; set; }
        public string IP { get; set; }
        public string CourseName { get; set; }
        public TypeContact TypeContact { get; set; }    
        public Contact()
        {
            CreateDate = DateTime.Now;
        }
    }
    public enum TypeContact
    {
        [Display(Name ="Ứng viên")]
        User,
        [Display(Name ="Nhà tuyển dụng")]
        Employer,
        [Display(Name ="Đối tác")]
        Partner
    }
}