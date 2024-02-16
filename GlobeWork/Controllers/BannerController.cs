using Helpers;
using GlobeWork.DAL;
using GlobeWork.Models;
using GlobeWork.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Drawing;

namespace GlobeWork.Controllers
{
    [Authorize]
    public class BannerController : Controller
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();
        #region Banner
        public ActionResult ListBanner(int groupId = 0, string result = "")
        {
            ViewBag.Banner = result;
            var banners = _unitOfWork.BannerRepository.GetQuery(orderBy: q => q.OrderBy(a => a.GroupId).ThenBy(a => a.Sort));
            if (groupId > 0)
            {
                banners = banners.Where(a => a.GroupId == groupId);
            }
            var model = new ListBannerViewModel
            {
                Banners = banners
            };
            return View(model);
        }
        public ActionResult Banner()
        {
            var model = new BannerViewModel()
            {
                Banner = new Models.Banner() { Sort = 1, Active = true }
            };
            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Banner(BannerViewModel model)
        {
            if (ModelState.IsValid)
            {
                var file = Request.Files["Banner.Image"];
                if (file == null || file.ContentLength <= 0)
                {
                    ModelState.AddModelError("", @"Hãy chọn 1 hình ảnh.");
                    return View(model);
                }

                if (!HtmlHelpers.CheckFileExt(file.FileName, "jpg|jpeg|png|gif|svg"))
                {
                    ModelState.AddModelError("", @"Chỉ chấp nhận định dạng jpg, png, gif, jpeg, svg");
                    return View(model);
                }

                if (file.ContentLength > 4000 * 1024)
                {
                    ModelState.AddModelError("", @"Dung lượng lớn hơn 4MB. Hãy thử lại");
                    return View(model);
                }

                var imgPath = "/images/banners/" + DateTime.Now.ToString("yyyy/MM/dd");
                HtmlHelpers.CreateFolder(Server.MapPath(imgPath));
                //var imgFileName = DateTime.Now.ToFileTimeUtc() + Path.GetExtension(file.FileName);
                var imgFileName = HtmlHelpers.ConvertToUnSign(null, Path.GetFileNameWithoutExtension(file.FileName)) + "-" + DateTime.Now.Millisecond + Path.GetExtension(file.FileName);

                model.Banner.Image = DateTime.Now.ToString("yyyy/MM/dd") + "/" + imgFileName;
                file.SaveAs(Server.MapPath(Path.Combine(imgPath, imgFileName)));

                _unitOfWork.BannerRepository.Insert(model.Banner);
                _unitOfWork.Save();

                return RedirectToAction("ListBanner", new { result = "success" });
            }
            return View(model);
        }
        public ActionResult EditBanner(int bannerId = 0)
        {
            var banner = _unitOfWork.BannerRepository.GetById(bannerId);
            if (banner == null)
            {
                return RedirectToAction("ListBanner");
            }
            var model = new BannerViewModel
            {
                Banner = banner
            };
            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult EditBanner(BannerViewModel model)
        {
            if (ModelState.IsValid)
            {
                var isPost = true;

                var banner = _unitOfWork.BannerRepository.GetById(model.Banner.Id);

                var file = Request.Files["Banner.Image"];
                if (file != null && file.ContentLength > 0)
                {
                    if (!HtmlHelpers.CheckFileExt(file.FileName, "jpg|jpeg|png|gif|svg"))
                    {
                        ModelState.AddModelError("", @"Chỉ chấp nhận định dạng jpg, png, gif, jpeg, svg");
                        isPost = false;
                    }
                    else
                    {
                        if (file.ContentLength > 4000 * 1024)
                        {
                            ModelState.AddModelError("", @"Dung lượng lớn hơn 4MB. Hãy thử lại");
                            isPost = false;
                        }
                        else
                        {
                            var imgPath = "/images/banners/" + DateTime.Now.ToString("yyyy/MM/dd");
                            HtmlHelpers.CreateFolder(Server.MapPath(imgPath));
                            //var imgFileName = DateTime.Now.ToFileTimeUtc() + Path.GetExtension(file.FileName);
                            var imgFileName = HtmlHelpers.ConvertToUnSign(null, Path.GetFileNameWithoutExtension(file.FileName)) + "-" + DateTime.Now.Millisecond + Path.GetExtension(file.FileName);


                            banner.Image = DateTime.Now.ToString("yyyy/MM/dd") + "/" + imgFileName;
                            file.SaveAs(Server.MapPath(Path.Combine(imgPath, imgFileName)));
                        }
                    }
                }

                if (isPost)
                {
                    banner.GroupId = model.Banner.GroupId;
                    banner.BannerName = model.Banner.BannerName;
                    banner.Slogan = model.Banner.Slogan;
                    banner.Sort = model.Banner.Sort;
                    banner.Active = model.Banner.Active;
                    banner.Url = model.Banner.Url;
                    banner.Content = model.Banner.Content;
                    _unitOfWork.BannerRepository.Update(banner);
                    _unitOfWork.Save();

                    return RedirectToAction("ListBanner", new { result = "update" });
                }
            }
            return View(model);
        }
        [HttpPost]
        public bool DeleteBanner(int bannerId = 0)
        {
            var banner = _unitOfWork.BannerRepository.GetById(bannerId);
            if (banner == null)
            {
                return false;
            }
            HtmlHelpers.DeleteFile(Server.MapPath("/images/banners/" + banner.Image));
            _unitOfWork.BannerRepository.Delete(banner);
            _unitOfWork.Save();
            return true;
        }
        public bool UpdateBannerQuick(int sort = 1, bool active = false, int bannerId = 0)
        {
            var banner = _unitOfWork.BannerRepository.GetById(bannerId);
            if (banner == null)
            {
                return false;
            }
            banner.Sort = sort;
            banner.Active = active;

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