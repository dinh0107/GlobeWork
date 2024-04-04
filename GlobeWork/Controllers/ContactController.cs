using GlobeWork.DAL;
using GlobeWork.Models;
using GlobeWork.ViewModel;
using Helpers;
using PagedList;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace GlobeWork.Controllers
{
    public class ContactController : Controller
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();
        #region Contact
        public ActionResult ListContact(int? page, string name)
        {
            var pageNumber = page ?? 1;
            const int pageSize = 15;
            var contact = _unitOfWork.ContactRepository.GetQuery(orderBy: l => l.OrderByDescending(a => a.Id));

            if (!string.IsNullOrEmpty(name))
            {
                contact = contact.Where(l => l.FullName.Contains(name));
            }
            var model = new ListContactViewModel
            {
                Contacts = contact.ToPagedList(pageNumber, pageSize),
                Name = name
            };
            return View(model);
        }
        [HttpPost]
        public bool DeleteContact(int contactId = 0)
        {
            var contact = _unitOfWork.ContactRepository.GetById(contactId);
            if (contact == null)
            {
                return false;
            }
            _unitOfWork.ContactRepository.Delete(contact);
            _unitOfWork.Save();
            return true;
        }
        public PartialViewResult LoadContact(int contactId = 0)
        {
            var contact = _unitOfWork.ContactRepository.GetById(contactId);

            var model = new LoadContactViewModel
            {
                Contact = contact
            };
            return PartialView(model);
        }
        #endregion

        #region 
        public ActionResult ListAdvise(int? page, string name , int type = 0)
        {
            var pageNumber = page ?? 1;
            const int pageSize = 15;
            var contact = _unitOfWork.AdviseRepository.GetQuery(orderBy: l => l.OrderByDescending(a => a.Id));

            if (!string.IsNullOrEmpty(name))
            {
                contact = contact.Where(l => l.CustomerInfo.Fullname.Contains(name));
            }
            switch (type)
            {
                case 1:
                    contact = contact.Where(a => a.JobPostId != null);
                    break;
                case 2:
                    contact = contact.Where(a => a.StudyAbroadId != null);
                    break;
            }
            var model = new ListAdviseViewModel
            {
                Advises = contact.ToPagedList(pageNumber, pageSize),
                Name = name,
                Type = type
            };
            return View(model);
        }
        [HttpPost]
        public bool DeleteAdvise(int Id = 0)
        {
            var contact = _unitOfWork.AdviseRepository.GetById(Id);
            if (contact == null)
            {
                return false;
            }
            _unitOfWork.AdviseRepository.Delete(contact);
            _unitOfWork.Save();
            return true;
        }
        public PartialViewResult LoadAdvise(int id = 0)
        {
            var contact = _unitOfWork.AdviseRepository.GetById(id);
            return PartialView(contact);
        }
        #endregion
        protected override void Dispose(bool disposing)
        {
            _unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}