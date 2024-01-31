using Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GlobeWork.Models
{
    public class Employer
    {
        public int Id { get; set; }
        [Display(Name = "Email"), Required(ErrorMessage = "Vui lòng nhập Email"), StringLength(100, ErrorMessage = "Tối đa 100 ký tự"), EmailAddress(ErrorMessage = "Email không hợp lệ"), UIHint("TextBox"), Remote("CheckEmail", "User")]
        public string Email { get; set; }
        [DisplayName("Mật khẩu"), Required(ErrorMessage = "Bạn chưa nhập mật khẩu"), StringLength(20, MinimumLength = 6, ErrorMessage = "Mật khẩu từ 6 - 20 ký tự")]
        public string Password { get; set; }
        [Display(Name = "Họ và tên"), Required(ErrorMessage = "Hãy nhập Họ và tên"), StringLength(100, ErrorMessage = "Tối đa 100 ký tự"), UIHint("TextBox")]
        public string FullName { get; set; }
        [Display(Name = "Giới tính")]
        public string Gender { get; set; }
        [Display(Name = "Số điện thoại"), RegularExpression(@"^\(?(09|03|07|08|05)\)?[-. ]?([0-9]{8})$", ErrorMessage = "Số điện thoại không đúng định dạng!"),
        Required(ErrorMessage = "Hãy nhập số điện thoại"), StringLength(10, ErrorMessage = "Tối đa 20 ký tự"), UIHint("TextBox")]
        public string PhoneNumber { get; set; }
        [Display(Name = "Tên công ty")]
        public string CompanyName { get; set; }
        [StringLength(500)]
        public string Avatar { get; set; }
        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }
        public bool Active { get; set; }
        public DateTime CreateDate { get; set; }
        [Display(Name = "Vị trí công tác"), Required(ErrorMessage = "Hãy chọn danh mục bài viết")]
        public int RankId { get; set; }
        public string Token { get; set; }
        public virtual Rank Rank { get; set; }
        public Employer()
        {
            CreateDate = DateTime.Now;
            Active = true;
            Token = HtmlHelpers.RandomCode(50);
        }
    }
}