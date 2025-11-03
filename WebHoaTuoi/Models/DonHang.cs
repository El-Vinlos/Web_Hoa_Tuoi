using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebHoaTuoi.Models;

namespace WebHoaTuoi.Models
{
    // Model Đơn Hàng
    public class DonHang
    {
        [Key]
        [StringLength(50)]
        public string MaDH { get; set; }

        [Required]
        public DateTime NgayDat { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Họ tên")]
        public string HoTen { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Điện thoại")]
        public string DienThoai { get; set; }

        [Required]
        [StringLength(500)]
        [Display(Name = "Địa chỉ")]
        public string DiaChi { get; set; }

        [StringLength(500)]
        [Display(Name = "Ghi chú")]
        public string GhiChu { get; set; }

        [Display(Name = "Tổng tiền")]
        public decimal TongTien { get; set; }

        [StringLength(50)]
        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; }

        public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; }
    }

    // Model Chi Tiết Đơn Hàng
    public class ChiTietDonHang
    {
        [Key, Column(Order = 0)]
        [StringLength(50)]
        public string MaDH { get; set; }

        [Key, Column(Order = 1)]
        [StringLength(50)]
        public string MaSP { get; set; }

        [Display(Name = "Số lượng")]
        public int SoLuong { get; set; }

        [Display(Name = "Đơn giá")]
        public decimal DonGia { get; set; }

        [Display(Name = "Thành tiền")]
        public decimal ThanhTien { get; set; }

        [ForeignKey("MaDH")]
        public virtual DonHang DonHang { get; set; }

        [ForeignKey("MaSP")]
        public virtual SanPham SanPham { get; set; }
    }
}
