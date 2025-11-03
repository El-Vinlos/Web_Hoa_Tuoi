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
