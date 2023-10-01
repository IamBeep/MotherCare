using mothercare.Data;
using mothercare.Helper;
using mothercare.Models;
using mothercare.Repository;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace mothercare.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        
        public GenericUnitOfWork _unitOfWork = new GenericUnitOfWork();
        public List<SelectListItem> GetCategory()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            var cat = _unitOfWork.GetRepositoryInstance<Tbl_Category>().GetAllRecords();
            foreach (var item in cat)
            {
                list.Add(new SelectListItem { Value = item.CategoryId.ToString(), Text = item.CategoryName });
            }
            return list;
        }
        public List<SelectListItem> GetCartStatus()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            var cat = _unitOfWork.GetRepositoryInstance<Tbl_CartStatus>().GetAllRecords();
            foreach (var item in cat)
            {
                list.Add(new SelectListItem { Value = item.CartStatusId.ToString(), Text = item.CartStatus });
            }
            return list;
        }
        [AuthorizationFilter]
        public ActionResult Dashboard()
        {
            return View();
        }
        [AuthorizationFilter]
        public ActionResult Categories()
        {
            return View(_unitOfWork.GetRepositoryInstance<Tbl_Category>().GetAllRecords());
        }
        [AuthorizationFilter]
        public ActionResult Slider()
        {
            return View(_unitOfWork.GetRepositoryInstance<Tbl_Slider>().GetAllRecords());
        }
        [AuthorizationFilter]
        public ActionResult CategoryEdit(int catId)
        {
            return View(_unitOfWork.GetRepositoryInstance<Tbl_Category>().GetFirstorDefault(catId));
        }
        [AuthorizationFilter]
        [HttpPost]
        public ActionResult CategoryEdit(Tbl_Category tbl)
        {
            _unitOfWork.GetRepositoryInstance<Tbl_Category>().Update(tbl);
            return RedirectToAction("Categories");
        }
        [AuthorizationFilter]
        public ActionResult Product()
        {
            return View(_unitOfWork.GetRepositoryInstance<Tbl_Product>().GetProduct());
        }
        [AuthorizationFilter]
        public ActionResult ProductEdit(int productId)
        {
            ViewBag.CategoryList = GetCategory();
            return View(_unitOfWork.GetRepositoryInstance<Tbl_Product>().GetFirstorDefault(productId));
        }
        [AuthorizationFilter]
        [HttpPost]
        public ActionResult ProductEdit(Tbl_Product tbl, HttpPostedFileBase file)
        {
            string pic = null;
            if (file != null)
            {
                pic = System.IO.Path.GetFileName(file.FileName);
                string path = System.IO.Path.Combine(Server.MapPath("~/ProductImg/"), pic);
                // file is uploaded
                file.SaveAs(path);
            }
            dbmothercareEntities context = new dbmothercareEntities();
            var data3 = context.Tbl_Product.Where(x => x.ProductId == tbl.ProductId).FirstOrDefault();
            tbl.ProductImage = file != null ? pic : data3.ProductImage;
            tbl.ModifiedDate = DateTime.Now;
            _unitOfWork.GetRepositoryInstance<Tbl_Product>().Update(tbl);
            return RedirectToAction("Product");
        }
        [AuthorizationFilter]
        public ActionResult ProductAdd()
        {
            ViewBag.CategoryList = GetCategory();
            return View();
        }
        [AuthorizationFilter]
        [HttpPost]
        public ActionResult ProductAdd(Tbl_Product tbl, HttpPostedFileBase file)
        {
            string pic = null;
            if (file != null)
            {
                pic = System.IO.Path.GetFileName(file.FileName);
                string path = System.IO.Path.Combine(Server.MapPath("~/ProductImg/"), pic);
                // file is uploaded
                file.SaveAs(path);
            }
            tbl.ProductImage = pic;
            tbl.CreatedDate = DateTime.Now;
            tbl.IsDelete = false;
            _unitOfWork.GetRepositoryInstance<Tbl_Product>().Add(tbl);
            return RedirectToAction("Product");
        }

        [AuthorizationFilter]
        public ActionResult CategoryAdd()
        {
            return View();
        }
        [AuthorizationFilter]
        public ActionResult SliderAdd()
        {
            return View();
        }
        [AuthorizationFilter]
        [HttpPost]
        public ActionResult SliderAdd(Tbl_Slider tbl, HttpPostedFileBase file)
        {
            string pic = null;
            if (file != null)
            {
                pic = System.IO.Path.GetFileName(file.FileName);
                string path = System.IO.Path.Combine(Server.MapPath("~/ProductImg/"), pic);
                // file is uploaded
                file.SaveAs(path);
            }
            tbl.SliderPath = pic;
            tbl.CreatedOn = DateTime.Now;
            _unitOfWork.GetRepositoryInstance<Tbl_Slider>().Add(tbl);
            return RedirectToAction("Slider");
        }
        [AuthorizationFilter]
        public ActionResult SliderEdit(int sliderId)
        {
            return View(_unitOfWork.GetRepositoryInstance<Tbl_Slider>().GetFirstorDefault(sliderId));
        }
        [AuthorizationFilter]
        [HttpPost]
        public ActionResult SliderEdit(Tbl_Slider tbl, HttpPostedFileBase file)
        {
            Console.WriteLine(tbl.SliderPath);
            string pic = null;
            if (file != null)
            {
                pic = System.IO.Path.GetFileName(file.FileName);
                string path = System.IO.Path.Combine(Server.MapPath("~/ProductImg/"), pic);
                // file is uploaded
                file.SaveAs(path);
            }
            dbmothercareEntities context = new dbmothercareEntities();
            var data3 = context.Tbl_Slider.Where(x => x.SliderId==tbl.SliderId).FirstOrDefault();
            tbl.SliderPath = file != null ? pic : data3.SliderPath;
            tbl.ModifiedOn = DateTime.Now;
            _unitOfWork.GetRepositoryInstance<Tbl_Slider>().Update(tbl);
            return RedirectToAction("Slider");
        }
        [AuthorizationFilter]
        [HttpPost]
        public ActionResult CategoryAdd(Tbl_Category tbl)
        {
            _unitOfWork.GetRepositoryInstance<Tbl_Category>().Add(tbl);
            return RedirectToAction("Categories");
        }
        [AuthorizationFilter]
        public ActionResult Users()
        {
            return View(_unitOfWork.GetRepositoryInstance<Tbl_Members>().GetProduct());
        }
        [AuthorizationFilter]
        public ActionResult Orders()
        {
            return View(_unitOfWork.GetRepositoryInstance<Tbl_Cart>().GetProduct().OrderByDescending(x=>x.CartId));
        }
        [AuthorizationFilter]
        public ActionResult OrdersEdit(int cartId)
        {
            ViewBag.CartStatus = GetCartStatus();
            return View(_unitOfWork.GetRepositoryInstance<Tbl_Cart>().GetFirstorDefault(cartId));
        }
        [AuthorizationFilter]
        [HttpPost]
        public ActionResult OrdersEdit(Tbl_Cart tbl)
        {
            _unitOfWork.GetRepositoryInstance<Tbl_Cart>().Update(tbl);
            return RedirectToAction("Orders");
        }
        [AuthorizationFilter]
        public ActionResult UserEdit(int userId)
        {
            return View(_unitOfWork.GetRepositoryInstance<Tbl_Members>().GetFirstorDefault(userId));
        }
        [AuthorizationFilter]
        [HttpPost]
        public ActionResult UserEdit(Tbl_Members tbl)
        {
            _unitOfWork.GetRepositoryInstance<Tbl_Members>().Update(tbl);
            return RedirectToAction("Users");
        }
        public ActionResult Logout()
        {
            Session["Username"] = null;
            Session["Role"] = null;
            return RedirectToAction("Login");
        }
        [AuthorizationFilter]
        public ActionResult Notice()
        {
            return View(_unitOfWork.GetRepositoryInstance<Tbl_Notice>().GetProduct());
        }
        [AuthorizationFilter]
        public ActionResult NoticeEdit(int noticeId)
        {
            return View(_unitOfWork.GetRepositoryInstance<Tbl_Notice>().GetFirstorDefault(noticeId));
        }
        [AuthorizationFilter]
        [HttpPost]
        public ActionResult NoticeEdit(Tbl_Notice tbl)
        {
            _unitOfWork.GetRepositoryInstance<Tbl_Notice>().Update(tbl);
            return RedirectToAction("Notice");
        }
        [AuthorizationFilter]
        public ActionResult NoticeAdd()
        {
            return View();
        }
        [AuthorizationFilter]
        [HttpPost]
        public ActionResult NoticeAdd(Tbl_Notice tbl)
        {
            _unitOfWork.GetRepositoryInstance<Tbl_Notice>().Add(tbl);
            return RedirectToAction("Notice");
        }
    }
}