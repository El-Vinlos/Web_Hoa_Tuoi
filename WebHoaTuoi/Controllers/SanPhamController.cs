using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using WebHoaTuoi.Models;

namespace WebHoaTuoi.Controllers
{
    public class SanPhamController : Controller
    {
        private HoaTuoiDbContext db = new HoaTuoiDbContext();

        public ActionResult Index(string loai, string sort)
        {
            // Lấy danh sách sản phẩm kèm loại
            var sanPhams = db.SanPhams.Include("LoaiSP").AsQueryable();

            // Truyền danh sách loại sang ViewBag
            ViewBag.DanhMucList = db.LoaiSPs.ToList();
            ViewBag.SelectedLoai = loai;

            // Lọc theo loại nếu có
            if (!string.IsNullOrEmpty(loai))
            {
                sanPhams = sanPhams.Where(s => s.MaLoaiSP == loai);
                ViewBag.TenLoai = db.LoaiSPs.Find(loai)?.TenLoaiSP ?? "Danh Sách Sản Phẩm";
            }
            else
            {
                ViewBag.TenLoai = "Danh Sách Sản Phẩm";
            }

            // Sắp xếp theo yêu cầu
            switch (sort)
            {
                case "name":
                    sanPhams = sanPhams.OrderBy(s => s.TenSP);
                    break;
                case "price-asc":
                    sanPhams = sanPhams.OrderBy(s => s.DonGia);
                    break;
                case "price-desc":
                    sanPhams = sanPhams.OrderByDescending(s => s.DonGia);
                    break;
            }

            return View(sanPhams.ToList());
        }

        public ActionResult ChiTietSanPham(string id)
        {
            if (id == null)
                return HttpNotFound();

            var sanPham = db.SanPhams
                .Include("LoaiSP")
                .FirstOrDefault(s => s.MaSP == id);

            if (sanPham == null)
                return HttpNotFound();

            return View(sanPham);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddToCartChitiet(string maSP, int soLuong = 1, string returnUrl = null)
        {
            var sanPham = db.SanPhams.Find(maSP);
            if (sanPham != null && sanPham.SoLuong >= soLuong)
            {
                ShoppingCart.AddToCart(sanPham, soLuong);
                TempData["CartMessage"] = $"Đã thêm {soLuong} x {sanPham.TenSP} vào giỏ hàng!";
            }
            else
            {
                TempData["CartMessage"] = "Sản phẩm không tồn tại hoặc hết hàng!";
            }

            // Redirect back to the page that sent the request
            if (!string.IsNullOrEmpty(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("ChiTietSanPham", new { id = maSP });
        }
        [HttpPost]
        public ActionResult AddToCart(string maSP, int soLuong = 1, string returnUrl = null)
        {
            var sanPham = db.SanPhams.Find(maSP);
            if (sanPham != null && sanPham.SoLuong >= soLuong)
            {
                ShoppingCart.AddToCart(sanPham, soLuong);
                TempData["CartMessage"] = $"Đã thêm {soLuong} x {sanPham.TenSP} vào giỏ hàng!";
            }
            else
            {
                TempData["CartMessage"] = "Sản phẩm không tồn tại hoặc hết hàng!";
            }

            // Redirect back to the page that sent the request
            if (!string.IsNullOrEmpty(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index");
        }

        public ActionResult TheoLoai(string id)
        {
            return RedirectToAction("Index", new { loai = id });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}
