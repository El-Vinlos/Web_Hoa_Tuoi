using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebHoaTuoi.Models
{
    public class ShoppingCart
    {
        private const string CartSessionKey = "Cart";

        // Lấy giỏ hàng từ Session
        public static List<CartItem> GetCart()
        {
            var cart = HttpContext.Current.Session[CartSessionKey] as List<CartItem>;
            if (cart == null)
            {
                cart = new List<CartItem>();
                HttpContext.Current.Session[CartSessionKey] = cart;
            }
            return cart;
        }

        // Thêm sản phẩm vào giỏ hàng
        public static void AddToCart(SanPham sp, int soLuong = 1)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(x => x.MaSP == sp.MaSP);
            
            if (item != null)
            {
                // Nếu sản phẩm đã có trong giỏ, tăng số lượng
                item.SoLuong += soLuong;
            }
            else
            {
                // Nếu chưa có, thêm mới
                cart.Add(new CartItem
                {
                    MaSP = sp.MaSP,
                    TenSP = sp.TenSP,
                    Hinh = sp.Hinh,
                    DonGia = sp.DonGia,
                    SoLuong = soLuong
                });
            }
        }

        // Cập nhật số lượng sản phẩm
        public static void UpdateQuantity(string maSP, int soLuong)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(x => x.MaSP == maSP);
            if (item != null)
            {
                if (soLuong <= 0)
                {
                    // Nếu số lượng <= 0, xóa sản phẩm
                    cart.Remove(item);
                }
                else
                {
                    item.SoLuong = soLuong;
                }
            }
        }

        // Xóa sản phẩm khỏi giỏ hàng
        public static void RemoveFromCart(string maSP)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(x => x.MaSP == maSP);
            if (item != null)
            {
                cart.Remove(item);
            }
        }

        // Xóa toàn bộ giỏ hàng
        public static void ClearCart()
        {
            HttpContext.Current.Session[CartSessionKey] = null;
        }

        // Đếm tổng số sản phẩm trong giỏ
        public static int GetCartCount()
        {
            var cart = GetCart();
            return cart.Sum(x => x.SoLuong);
        }

        // Tính tổng tiền
        public static decimal GetTotalAmount()
        {
            var cart = GetCart();
            return cart.Sum(x => x.ThanhTien);
        }
    }
}
