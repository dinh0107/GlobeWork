using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GlobeWork.Models
{
    public class ApplyJob
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int? JobPostId { get; set; }
        public int? StudyAbroadId { get; set; }
        [Display(Name = "Lời nhắn"), StringLength(500, ErrorMessage = "Tối đa 500 ký tự"), DataType(DataType.MultilineText)]
        public string Body { get; set; }
        public int CompanyId { get; set; }
        [Display(Name = "File CV")]
        public string FileUpload { get; set; }
        public string Url { get; set; }
        public ApplyJobStatus Status { get; set; }
        public TypeApply TypeApply { get; set; }

        public DateTime CreateDate { get; set; }
        public virtual JobPost JobPost { get; set; }
        public virtual StudyAbroad StudyAbroad { get; set; }
        public virtual User  User { get; set; }

        public ApplyJob()
        {
            CreateDate = DateTime.Now;
        }
        
    }
    public enum TypeApply
    {
        [Display(Name = "Tải lên Cv")]
        File,
        [Display(Name = "Dùng trang cá nhân")]
        Profile
    }
    public enum ApplyJobStatus
    {
        [Display(Name = "Đang chờ")]
        Waiting,
        [Display(Name = "NTD đã xem CV")]
        View,
        [Display(Name = "Hồ sơ phù hợp")]
        Active,
        [Display(Name = "Hồ sơ chưa phù hợp")]
        NoActive,
    }
}