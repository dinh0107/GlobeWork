using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GlobeWork.Models
{
    public class ApplyJob
    {
        public int Id { get; set; }
        [ForeignKey("Candidate")]
        public int UserId { get; set; }
        [ForeignKey("JobPost")]
        public int JobPostId { get; set; }
        [Display(Name = "Họ và tên"), Required(ErrorMessage = "Vui lòng nhập tên"), StringLength(50, ErrorMessage = "Tối đa 50 ký tự"), UIHint("TextBox")]
        public string FullName { get; set; }
        [Display(Name = "Email"), Required(ErrorMessage = "Vui lòng nhập Email"), StringLength(100, ErrorMessage = "Tối đa 100 ký tự"), EmailAddress(ErrorMessage = "Email không hợp lệ"), UIHint("TextBox")]
        public string Email { get; set; }
        [Display(Name = "Số điện thoại"), RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$",
                   ErrorMessage = "Số điện thoại không đúng !!!."), StringLength(20, ErrorMessage = "Tối đa 20 ký tự"), UIHint("TextBox")]
        public string Phone { get; set; }
        [Display(Name = "Lời nhắn"), StringLength(500, ErrorMessage = "Tối đa 500 ký tự"), DataType(DataType.MultilineText)]
        public string Body { get; set; }
        [Display(Name = "File CV")]
        public string FileUpload { get; set; }

        public ApplyJobStatus Status { get; set; }

        public DateTime CreateDate { get; set; }
        public virtual Candidate Candidate { get; set; }
        public virtual JobPost JobPost { get; set; }

        public ApplyJob()
        {
            CreateDate = DateTime.Now;
        }

        public enum ApplyJobStatus
        {
            [Display(Name = "Đang chờ")]
            Waiting,
            [Display(Name = "Đã duyệt")]
            Active,
            [Display(Name = "Từ chối")]
            NoActive,
        }
    }
}