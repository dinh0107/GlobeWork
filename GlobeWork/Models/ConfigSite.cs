using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection.Emit;
using System.Web;

namespace GlobeWork.Models
{
    public class ConfigSite
    {
        public int Id { get; set; }

        [StringLength(500, ErrorMessage = "Tối đa 500 ký tự"), Display(Name = "Đường dẫn Facebook"),
         Url(ErrorMessage = "Đường dẫn không chính xác"), UIHint("TextBox")]
        public string Facebook { get; set; }
        [StringLength(500, ErrorMessage = "Tối đa 500 ký tự"), Display(Name = "Đường dẫn Linkedin"),
         Url(ErrorMessage = "Đường dẫn không chính xác"), UIHint("TextBox")]
        public string Linkedin { get; set; }
        [StringLength(500, ErrorMessage = "Tối đa 500 ký tự"), Display(Name = "Đường dẫn Instagram"),
         Url(ErrorMessage = "Đường dẫn không chính xác"), UIHint("TextBox")]
        public string Instagram { get; set; }
        [StringLength(500, ErrorMessage = "Tối đa 500 ký tự"), Display(Name = "Đường dẫn Youtube"),
        Url(ErrorMessage = "Đường dẫn không chính xác"), UIHint("TextBox")]
        public string Youtube { get; set; }
        [StringLength(500, ErrorMessage = "Tối đa 500 ký tự"), Display(Name = "Đường dẫn Twitter"),
         Url(ErrorMessage = "Đường dẫn không chính xác"), UIHint("TextBox")]
        public string Twitter { get; set; }
        [StringLength(50, ErrorMessage = "Tối đa 50 ký tự"), Display(Name = "Tài khoản Zalo"), UIHint("TextBox")]
        public string Zalo { get; set; }
        [StringLength(4000, ErrorMessage = "Tối đa 4000 ký tự"), Display(Name = "Mã nhúng Live chat"),
        UIHint("TextArea")]
        public string LiveChat { get; set; }
        [Display(Name = "Logo"), UIHint("ImageConfig")]
        public string Image { get; set; }

        [Display(Name = "Ảnh share"), UIHint("ImageConfig")]
        public string ImageShare { get; set; }
        [Display(Name = "Favicon"), UIHint("ImageConfig")]
        public string Favicon { get; set; }

        [StringLength(50, ErrorMessage = "Tối đa 50 ký tự"), Display(Name = "Email tự động"), UIHint("TextBox")]
        public string EmailConfigs { get; set; }
        [Display(Name = "Mật khẩu ứng dụng") , UIHint("TextBox")]
        public string PassWordMail { get; set; }

        [Display(Name = "Ảnh giới thiệu"), UIHint("ImageConfig")]
        public string AbountImage { get; set; }
        [StringLength(4000, ErrorMessage = "Tối đa 4000 ký tự"), Display(Name = "Mã Google Map"), UIHint("TextArea")]
        public string GoogleMap { get; set; }
        [StringLength(4000, ErrorMessage = "Tối đa 4000 ký tự"), Display(Name = "Mã Google Analytics"), UIHint("TextArea")]
        public string GoogleAnalytics { get; set; }
        [Display(Name = "Địa chỉ"), UIHint("TextBox")]
        public string Place { get; set; }
        [Display(Name = "Thẻ title"), StringLength(200, ErrorMessage = "Tối đa 200 ký tự"), UIHint("TextBox")]
        public string Title { get; set; }
        [Display(Name = "Thẻ description"), StringLength(500, ErrorMessage = "Tối đa 500 ký tự"), UIHint("TextArea")]
        public string Description { get; set; }
        [Display(Name = "Hotline"), StringLength(50, ErrorMessage = "Tối đa 50 ký tự"), UIHint("TextBox")]
        public string Hotline { get; set; }

        [StringLength(50, ErrorMessage = "Tối đa 50 ký tự"), Display(Name = "Email"), UIHint("TextBox")]
        public string Email { get; set; }
        [Display(Name = "Slogan giới thiệu"), StringLength(200, ErrorMessage = "Tối đa 200 ký tự"), UIHint("TextArea")]
        public string AboutText { get; set; }

        [Display(Name = "Thông tin chân trang"), UIHint("EditorBox")]
        public string InfoFooter { get; set; }
        [Display(Name = "Thông tin liên hệ"), UIHint("EditorBox")]
        public string AboutContact { get; set; }
        [Display(Name = "Thời gian làm việc")]
        public string WorkingTime { get; set; }
    }
}