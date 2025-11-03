using System;
using System.Linq;
using System.Web.Mvc;
using WebHoaTuoi.Models;

namespace WebHoaTuoi.Controllers
{
    public class CartController : Controller
    {
        private HoaTuoiDbContext db = new HoaTuoiDbContext();

        // GET: Cart - Hiển thị giỏ hàng
        public ActionResult Index()
        {
            var cart = ShoppingCart.GetCart();
            ViewBag.TotalAmount = ShoppingCart.GetTotalAmount();
            return View(cart);
        }

        // POST: Cart/UpdateQuantity - Cập nhật số lượng
        [HttpPost]
        public ActionResult UpdateQuantity(string maSP, int soLuong)
        {
            ShoppingCart.UpdateQuantity(maSP, soLuong);
            return Json(new { 
                success = true, 
                cartCount = ShoppingCart.GetCartCount(),
                totalAmount = ShoppingCart.GetTotalAmount()
            });
        }

        // POST: Cart/RemoveFromCart - Xóa sản phẩm khỏi giỏ
        [HttpPost]
        public ActionResult RemoveFromCart(string maSP)
        {
            ShoppingCart.RemoveFromCart(maSP);
            TempData["Message"] = "Đã xóa sản phẩm khỏi giỏ hàng!";
            return RedirectToAction("Index");
        }

        // GET: Cart/Checkout - Trang thanh toán
        public ActionResult Checkout()
        {
            var cart = ShoppingCart.GetCart();
            if (!cart.Any())
            {
                TempData["Error"] = "Giỏ hàng trống!";
                return RedirectToAction("Index");
            }
            
            ViewBag.TotalAmount = ShoppingCart.GetTotalAmount();
            return View(cart);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProcessCheckout(string hoTen, string dienThoai, string diaChi, string ghiChu)
        {
            var cart = ShoppingCart.GetCart();
            if (!cart.Any())
            {
                return RedirectToAction("Index");
            }

            try
            {
                // Tạo đơn hàng mới
                var donHang = new DonHang
                {
                    MaDH = "DH" + DateTime.Now.Ticks,
                    NgayDat = DateTime.Now,
                    HoTen = hoTen,
                    DienThoai = dienThoai,
                    DiaChi = diaChi,
                    GhiChu = ghiChu,
                    TongTien = ShoppingCart.GetTotalAmount(),
                    TrangThai = "Chờ xử lý"
                };

                db.DonHangs.Add(donHang);

                // Thêm chi tiết đơn hàng
                foreach (var item in cart)
                {
                    var chiTiet = new ChiTietDonHang
                    {
                        MaDH = donHang.MaDH,
                        MaSP = item.MaSP,
                        SoLuong = item.SoLuong,
                        DonGia = item.DonGia,
                        ThanhTien = item.ThanhTien
                    };
                    db.ChiTietDonHangs.Add(chiTiet);

                    // Cập nhật số lượng sản phẩm
                    var sanPham = db.SanPhams.Find(item.MaSP);
                    if (sanPham != null)
                    {
                        sanPham.SoLuong -= item.SoLuong;
                    }
                }

                db.SaveChanges();
                ShoppingCart.ClearCart();

                TempData["Success"] = "Đặt hàng thành công! Mã đơn hàng: " + donHang.MaDH;
                return RedirectToAction("OrderComplete", new { maDH = donHang.MaDH });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Có lỗi xảy ra: " + ex.Message;
                return RedirectToAction("Checkout");
            }
        }
        public ActionResult OrderComplete(string maDH)
        {
            var donHang = db.DonHangs.Find(maDH);
            if (donHang == null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(donHang);
        }

        // GET: Cart/GetCartCount - Lấy số lượng sản phẩm trong giỏ (cho AJAX)
        [HttpGet]
        public JsonResult GetCartCount()
        {
            return Json(ShoppingCart.GetCartCount(), JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
