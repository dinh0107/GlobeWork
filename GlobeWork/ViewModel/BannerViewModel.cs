using GlobeWork.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace GlobeWork.ViewModel
{
    public class BannerViewModel
    {
        public Banner Banner { get; set; }
        public SelectList SelectGroup { get; set; }
        public BannerViewModel()
        {
            var listgroup = new Dictionary<int, string>
            {
                { 1, "Banner Slide" },
                { 2, "Du học" },
                { 3, "Việc làm" },
            };
            SelectGroup = new SelectList(listgroup, "Key", "Value");
        }
    }
    public class ListBannerViewModel
    {
        public IEnumerable<Banner> Banners { get; set; }
        public int? GroupId { get; set; }
        public SelectList SelectGroup { get; set; }
        public ListBannerViewModel()
        {
            var listgroup = new Dictionary<int, string>
            {
                { 1, "Banner Slide" },
                { 2, "Du học" },
                { 3, "Việc làm" },
            };
            SelectGroup = new SelectList(listgroup, "Key", "Value");
        }
    }
}