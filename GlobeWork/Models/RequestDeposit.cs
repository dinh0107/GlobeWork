using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GlobeWork.Models
{
    public class RequestDeposit
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public DateTime CreateTime { get; set; }
        public decimal Amount { get; set; }
        public virtual Company Company { get; set; }
        public StatusRequest StatusRequest { get; set; }
    }
    public enum StatusRequest
    {
        [Display(Name ="Chờ duyệt")]
        NoActive,
        [Display(Name ="Đã duyệt")]
        Active,
    }
}