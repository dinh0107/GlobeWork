using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GlobeWork.Models;

namespace GlobeWork.ViewModel
{
    public class ListArticleViewModel
    {
        public PagedList.IPagedList<Article> Articles { get; set; }
        public SelectList SelectCategories { get; set; }
        public SelectList ChildCategoryList { get; set; }
        public int? CatId { get; set; }
        public int? ChildId { get; set; }
        public string Name { get; set; }
    }
    public class InsertArticleViewModel
    {
        public Article Article { get; set; }
        public SelectList SelectCategories { get; set; }
        public int? CategoryId { get; set; }
        public IEnumerable<StudyAbroadCategory> StudyAbroadCategories { get; set; }
        [Display(Name = "Số ngày"), Required(ErrorMessage = "Hãy nhập thứ tự"), RegularExpression(@"\d+", ErrorMessage = "Chỉ nhập số nguyên")]
        public int DateHot { get; set; }
        public bool TruTien { get; set; }
    }
}