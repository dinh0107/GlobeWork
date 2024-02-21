using GlobeWork.DAL;
using GlobeWork.Models;
using GlobeWork.ViewModel;
using Helpers;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace GlobeWork.Controllers
{
    [Authorize]
    public class ArticleController : Controller
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();
        #region Article
        public ActionResult ListArticle(int? page, string name, int? catId, int? childId, string result = "")
        {
            ViewBag.Result = result;
            var pageNumber = page ?? 1;
            const int pageSize = 15;
            var article = _unitOfWork.ArticleRepository.GetQuery(orderBy: l => l.OrderByDescending(a => a.Id));
            if (!string.IsNullOrEmpty(name))
            {
                article = article.Where(l => l.Subject.Contains(name));
            }
            var model = new ListArticleViewModel
            {
                Articles = article.ToPagedList(pageNumber, pageSize),
                CatId = catId,
                ChildId = childId,
                Name = name
            };
            return View(model);
        }
        public ActionResult Article()
        {
            var model = new InsertArticleViewModel
            {
                Article = new Article { Active = true , IsAdmin = true }
            };
            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Article(InsertArticleViewModel model, FormCollection fc)
        {
            if (ModelState.IsValid)
            {
                for (var i = 0; i < Request.Files.Count; i++)
                {
                    if (Request.Files[i] == null || Request.Files[i].ContentLength <= 0) continue;
                    if (!HtmlHelpers.CheckFileExt(Request.Files[i].FileName, "jpg|jpeg|png|gif")) continue;
                    if (Request.Files[i].ContentLength > 1024 * 1024 * 4) continue;

                    var imgFileName = HtmlHelpers.ConvertToUnSign(null, Path.GetFileNameWithoutExtension(Request.Files[i].FileName)) +
                        "-" + DateTime.Now.Millisecond + Path.GetExtension(Request.Files[i].FileName);
                    var imgPath = "/images/articles/" + DateTime.Now.ToString("yyyy/MM/dd");
                    HtmlHelpers.CreateFolder(Server.MapPath(imgPath));

                    var imgFile = DateTime.Now.ToString("yyyy/MM/dd") + "/" + imgFileName;

                    var newImage = Image.FromStream(Request.Files[i].InputStream);
                    var fixSizeImage = HtmlHelpers.FixedSize(newImage, 1000, 1000, false);
                    HtmlHelpers.SaveJpeg(Server.MapPath(Path.Combine(imgPath, imgFileName)), fixSizeImage, 90);

                    if (Request.Files.Keys[i] == "Article.Image")
                    {
                        model.Article.Image = imgFile;
                    }
                }

                model.Article.Url = HtmlHelpers.ConvertToUnSign(null, model.Article.Url ?? model.Article.Subject);
                var articles = _unitOfWork.ArticleRepository.GetQuery().AsNoTracking();
                if (articles.Any(p => p.Url.Trim() == model.Article.Url.Trim()))
                {
                    model.Article.Url += "-" + DateTime.Now.Millisecond;
                }
                model.Article.IsAdmin = true;
                _unitOfWork.ArticleRepository.Insert(model.Article);
                _unitOfWork.Save();
                return RedirectToAction("ListArticle", new { result = "success" });
            }
            return View(model);
        }
        public ActionResult UpdateArticle(int articleId = 0)
        {
            var article = _unitOfWork.ArticleRepository.GetById(articleId);
            if (article == null)
            {
                return RedirectToAction("ListArticle");
            }
            var model = new InsertArticleViewModel
            {
                Article = article,
            };
            return View(model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult UpdateArticle(InsertArticleViewModel model, FormCollection fc)
        {
            var article = _unitOfWork.ArticleRepository.GetById(model.Article.Id);
            if (article == null)
            {
                return RedirectToAction("ListArticle");
            }
            if (ModelState.IsValid)
            {
                for (var i = 0; i < Request.Files.Count; i++)
                {
                    if (Request.Files[i] == null || Request.Files[i].ContentLength <= 0) continue;
                    if (!HtmlHelpers.CheckFileExt(Request.Files[i].FileName, "jpg|jpeg|png|gif")) continue;
                    if (Request.Files[i].ContentLength > 1024 * 1024 * 4) continue;

                    var imgFileName = HtmlHelpers.ConvertToUnSign(null, Path.GetFileNameWithoutExtension(Request.Files[i].FileName)) +
                        "-" + DateTime.Now.Millisecond + Path.GetExtension(Request.Files[i].FileName);
                    var imgPath = "/images/articles/" + DateTime.Now.ToString("yyyy/MM/dd");
                    HtmlHelpers.CreateFolder(Server.MapPath(imgPath));

                    var imgFile = DateTime.Now.ToString("yyyy/MM/dd") + "/" + imgFileName;

                    var newImage = Image.FromStream(Request.Files[i].InputStream);
                    var fixSizeImage = HtmlHelpers.FixedSize(newImage, 1000, 1000, false);
                    HtmlHelpers.SaveJpeg(Server.MapPath(Path.Combine(imgPath, imgFileName)), fixSizeImage, 90);

                    if (Request.Files.Keys[i] == "Article.Image")
                    {
                        article.Image = imgFile;
                    }
                }

                var file = Request.Files["Article.Image"];

                if (file != null && file.ContentLength == 0)
                {
                    article.Image = fc["CurrentFile"] == "" ? null : fc["CurrentFile"];
                }

                article.Url = HtmlHelpers.ConvertToUnSign(null, model.Article.Url ?? model.Article.Subject);
                article.Subject = model.Article.Subject;
                article.Description = model.Article.Description;
                article.Body = model.Article.Body;
                article.Active = model.Article.Active;
                article.Menu = model.Article.Menu;
                article.Sort = model.Article.Sort;
                article.TitleMeta = model.Article.TitleMeta;
                article.DescriptionMeta = model.Article.DescriptionMeta;

                _unitOfWork.Save();

                var articles = _unitOfWork.ArticleRepository.GetQuery().AsNoTracking();
                if (articles.Any(p => p.Url.ToLower().Trim() == model.Article.Url.ToLower().Trim() && p.Id != model.Article.Id))
                {
                    article.Url += "-" + DateTime.Now.Millisecond;
                }

                _unitOfWork.Save();

                return RedirectToAction("ListArticle", new { result = "update" });
            }
            return View(model);
        }
        [HttpPost]
        public bool DeleteArticle(int articleId = 0)
        {

            var article = _unitOfWork.ArticleRepository.GetById(articleId);
            if (article == null)
            {
                return false;
            }
            _unitOfWork.ArticleRepository.Delete(article);
            _unitOfWork.Save();
            return true;
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            _unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}