using GlobeWork.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlobeWork.ViewModel
{
    public class HomeViewModel
    {
       public IEnumerable<Career> Careers { get; set; }
    }
    public class GetFilterViewModel
    {
        public IEnumerable<City> Cities { get; set; }
        public IEnumerable<Career> Careers { get; set; }
        public IEnumerable<KeyValuePair<int, string>> Wage { get; set; }
        public IEnumerable<KeyValuePair<int, string>> Experience { get; set; }
        public GetFilterViewModel()
        {
            Wage = new Dictionary<int, string>
            {
                { 0, "Dưới 10 triệu" },
                { 1, "Từ 10 - 15 triệu" },
                { 2, "Từ 15 - 20 triệu" },
                { 3, "Từ 20 - 25 triệu" },
                { 4, "Từ 25 - 30 triệu" },
                { 5, "Từ 30 - 50 triệu" },
                { 6, "Trên 50 triệu" },
                { 7, "Thỏa thuận" },
            };
            Experience = new Dictionary<int, string>
            {
                { 0, "Chưa có kinh nghiệm" },
                { 1, "1 năm trở xuống" },
                { 2, "2 năm" },
                { 3, "3 năm" },
                { 4, "Từ 4 - 5 năm" },
                { 5, "Trên 5 năm" },
            };
        }
    }
    public class GetJobHotViewModel
    {
        public IEnumerable<JobPost> JobPosts { get; set; }
    }
    public class JobDetailViewModel
    {
        public JobPost Job { get; set; }
        public IEnumerable<JobPost> JobCompanys { get; set; }
        public IEnumerable<JobPost> JobPosts { get; set; }
        public IEnumerable<Company> Companies  { get; set; }
        public IEnumerable<Follow> Follows { get; set; }
        public IEnumerable<Like> Likes { get; set; }
    }
    public class DetailCompanyViewModel
    {
        public Company Company { get; set; }
        public IEnumerable<JobPost> JobPosts { get; set; }
        public IEnumerable<Follow> Follows { get; set; }
    }
}