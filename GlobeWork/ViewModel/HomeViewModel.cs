using GlobeWork.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlobeWork.ViewModel
{
    public class HeaderViewModel
    {
        public IEnumerable<Article> Articles { get; set; }
        public IEnumerable<StudyAbroadCategory> StudyAbroadCategories { get; set; }
        public IEnumerable<Career> Careers { get; set; }
    }
    public class FooterViewModel
    {
        public IEnumerable<Article> Articles { get; set; }
        public IEnumerable<Career> Careers { get; set; }
        public IEnumerable<Country>  Countries { get; set; }
        public IEnumerable<Parter> Parters { get; set; }
    }
    public class HomeViewModel
    {
        public IEnumerable<Item> Items { get; set; }
       public IEnumerable<Career> Careers { get; set; }
       public IEnumerable<Career> SearchCareers { get; set; }
       public IEnumerable<City> Cities { get; set; }
       public IEnumerable<JobType> JobTypes { get; set; }
       public IEnumerable<Rank> Ranks { get; set; }
       public IEnumerable<Banner> Banners { get; set; }
       public IEnumerable<Skill> Skills { get; set; }
       public IEnumerable<Company> Companies { get; set; }
       public IEnumerable<StudyAbroad> StudyAbroads { get; set; }
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
        public IEnumerable<Like> Likes { get; set; }
    }
    public class JobDetailViewModel
    {
        public JobPost Job { get; set; }
        public IEnumerable<JobPost> JobCompanys { get; set; }
        public IEnumerable<JobPost> JobPosts { get; set; }
        public IEnumerable<Company> Companies  { get; set; }
        public IEnumerable<Follow> Follows { get; set; }
        public IEnumerable<Like> Likes { get; set; }
        public IEnumerable<ApplyJob>  ApplyJobs { get; set; }
    }
    public class DetailCompanyViewModel
    {
        public Company Company { get; set; }
        public IEnumerable<JobPost> JobPosts { get; set; }
        public IEnumerable<Like> Likes { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public IEnumerable<Follow> Follows { get; set; }
    }
    public class CompanyRecruitmentViewModel
    {
        public Company Company { get; set; }
        public IPagedList<JobPost> JobPosts { get; set; }
        public IEnumerable<Like> Likes { get; set; }
        public IEnumerable<Follow> Follows { get; set; }
        public IEnumerable<City> Cities { get; set; }
        public IEnumerable<Career> Careers { get; set; }
        public string Name { get; set; }
        public int CareerId { get; set; }
        public int CityId { get; set; }
        public string Url { get; set; }
    }
    public class CompanyStudyViewModel
    {
        public Company Company { get; set; }
        public IPagedList<StudyAbroad> StudyAbroads { get; set; }
        public IEnumerable<Like> Likes { get; set; }
        public IEnumerable<Follow> Follows { get; set; }
        public IEnumerable<City> Cities { get; set; }
        public IEnumerable<Career> Careers { get; set; }
        public string Name { get; set; }
        public int CareerId { get; set; }
        public int CityId { get; set; }
        public string Url { get; set; }
    }
    public class JobHotViewModel
    {
        public IPagedList<JobPost> JobPosts { get; set; }
        public IEnumerable<Like> Likes { get; set; }
        public IEnumerable<Follow> Follows { get; set; }
        public IEnumerable<City> Cities { get; set; }
        public IEnumerable<Career> Careers { get; set; }
        public IEnumerable<Banner> Banners { get; set; }
        public IEnumerable<Career> CareerHot { get; set; }
        public IEnumerable<Company> Companies { get; set; }
        public string Name { get; set; }
        public int CareerId { get; set; }
        public int CityId { get; set; }
        public int Wage { get; set; }
        public IEnumerable<Item> Items { get; set; }
        public IEnumerable<KeyValuePair<int, string>> Wages { get; set; }
        public JobHotViewModel()
        {
            Wages = new Dictionary<int, string>
            {
                { 1, "Dưới 10 triệu" },
                { 2, "Từ 10 - 15 triệu" },
                { 3, "Từ 15 - 20 triệu" },
                { 4, "Từ 20 - 25 triệu" },
                { 5, "Từ 25 - 30 triệu" },
                { 6, "Từ 30 - 50 triệu" },
                { 7, "Trên 50 triệu" },
                { 8, "Thỏa thuận" },
            };
        }
    }
    public class SearchViewModel
    {
        public IPagedList<JobPost> JobPosts { get; set; }
        public IEnumerable<Like> Likes { get; set; }
        public IEnumerable<Follow> Follows { get; set; }
        public IEnumerable<City> Cities { get; set; }
        public IEnumerable<Career> Careers { get; set; }
        public IEnumerable<Banner> Banners { get; set; }
        public IEnumerable<Career> CareerHot { get; set; }
        public string Keyword { get; set; }
        public int CareerId { get; set; }
        public int CompanyId { get; set; }
        public int CityId { get; set; }
        public int Wage { get; set; }
        public IEnumerable<KeyValuePair<int, string>> Wages { get; set; }
        public SearchViewModel()
        {
            Wages = new Dictionary<int, string>
            {
                { 1, "Dưới 10 triệu" },
                { 2, "Từ 10 - 15 triệu" },
                { 3, "Từ 15 - 20 triệu" },
                { 4, "Từ 20 - 25 triệu" },
                { 5, "Từ 25 - 30 triệu" },
                { 6, "Từ 30 - 50 triệu" },
                { 7, "Trên 50 triệu" },
                { 8, "Thỏa thuận" },
            };
        }
    }
    public class JobCategoryViewMpdel
    {
        public Career Career { get; set; }
        public IPagedList<JobPost> JobPosts { get; set; }
        public IEnumerable<City> Cities { get; set; }
        public IEnumerable<Like> Likes { get; set; }
        public IEnumerable<JobType> jobTypes { get; set; }
        public string Name { get; set; }
        public int CityId { get; set; }
        public int Wage { get; set; }
        public int TypeId { get; set; }
        public string Url { get; set; }
        public IEnumerable<KeyValuePair<int, string>> Wages { get; set; }
        public JobCategoryViewMpdel()
        {
            Wages = new Dictionary<int, string>
            {
                { 1, "Dưới 10 triệu" },
                { 2, "Từ 10 - 15 triệu" },
                { 3, "Từ 15 - 20 triệu" },
                { 4, "Từ 20 - 25 triệu" },
                { 5, "Từ 25 - 30 triệu" },
                { 6, "Từ 30 - 50 triệu" },
                { 7, "Trên 50 triệu" },
                { 8, "Thỏa thuận" },
            };
        }
    }
    public class ApplyViewModel
    {
        public ApplyJob ApplyJob { get; set; }
        public int Type { get; set; }
        public int JobId { get; set; }
        public int StudyAbroadId { get; set; }
        public int CompanyId { get; set; }
    }
    public class AdviseViewModel
    {
        public Advise Advise { get; set; }
        public User User { get; set; }
        public int JobId { get; set; }
        public int StudyAbroadId { get; set; }
        public int CompanyId { get; set; }
    }
    public class StudyAbroadViewModel
    {

        public IEnumerable<StudyAbroad> NewStudyAbroad { get; set; }
        public IEnumerable<StudyAbroad> HotStudyAbroad { get; set; }
        public IEnumerable<Article> HotArticle { get; set; }
        public IEnumerable<Country> Countries { get; set; }
        public IEnumerable<Banner> Banners { get; set; }
        public IEnumerable<Skill> Skills { get; set; }
    }
    public class StudyAbroadCategoryViewModel
    {
        public StudyAbroadCategory StudyAbroadCategory { get; set; }
        public IEnumerable<StudyAbroad> NewStudyAbroad { get; set; }
        public IEnumerable<StudyAbroad> HotStudyAbroad { get; set; }
        public IEnumerable<Article> HotArticle { get; set; }
        public IEnumerable<Country> Countries { get; set; }
        public IEnumerable<Banner> Banners { get; set; }
        public IEnumerable<Skill> Skills { get; set; }
        public IEnumerable<Country> FooterCountries { get; set; }
        public IEnumerable<Country> CountriesStudy { get; set; }
        public IEnumerable<Country> CountriesSchool { get; set; }
        public IEnumerable<Career> Careers { get; set; }
    }
    public class StudyAbroadUrlViewModel
    {
        public StudyAbroadCategory StudyAbroadCategory { get; set; }
        public IPagedList<StudyAbroad> StudyAbroads { get; set; }
        public IEnumerable<Country> Countries { get; set; }
        public IEnumerable<Skill> Skills { get; set; }
        public IEnumerable<Like> Likes { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public int CountruyId { get; set; }
    }
    public class SearchStudyAbroadViewModel
    {
        public IPagedList<StudyAbroad> StudyAbroads { get; set; }
        public IEnumerable<Country> Countries { get; set; }
        public string Name { get; set; }
        public int CountruyId { get; set; }
        public int CompanyId { get; set; }
    }
    public class AllStudyAbroadViewModel
    {
        public IPagedList<StudyAbroad> StudyAbroads { get; set; }
        public IEnumerable<Country> Countries { get; set; }
        public string Name { get; set; }
        public int CountruyId { get; set; }
        public IEnumerable<Like> Likes { get; set; }
    }
    public class StudyDetailViewModel
    {
        public StudyAbroad StudyAbroad { get; set; }
        public IEnumerable<StudyAbroad> StudyAbroadCompanys { get; set; }
        public IEnumerable<StudyAbroad> StudyAbroads { get; set; }
        public IEnumerable<Company> Companies { get; set; }
        public IEnumerable<Follow> Follows { get; set; }
        public IEnumerable<Like> Likes { get; set; }
        public IEnumerable<ApplyJob> ApplyJobs { get; set; }
    }
    public class ListCompanyViewModel
    {
        public IEnumerable<Company> HotCompanies { get; set; }
        public IEnumerable<Item> Items { get; set; }
        public IEnumerable<Company> Companies { get; set; }
    }
    public class HotCompanyViewModel
    {
        public IPagedList<Company> Companies { get; set; }
        public IEnumerable<Item> Items { get; set; }
    }
    public class SearchCompanyViewModel
    {
        public IPagedList<Company> Companies { get; set; }
        public string Name { get; set; }
    }

    public class CountruyStudyAbroadViewModel
    {
        public IPagedList<StudyAbroad> StudyAbroads { get; set; }
        public IPagedList<Article> Articles { get; set; }
        public Country Country { get; set; }
        public Career  Career { get; set; }
        public IEnumerable<Like> Likes { get; set; }
        public string Name { get; set; }
    }
}