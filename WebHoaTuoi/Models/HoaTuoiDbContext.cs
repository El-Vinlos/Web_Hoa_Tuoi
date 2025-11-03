using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace WebHoaTuoi.Models
{
    public class HoaTuoiDbContext : DbContext
    {
        // Constructor - kết nối đến database
        public HoaTuoiDbContext() : base("name=HoaTuoiConnection")
        {
            // Cấu hình
            this.Configuration.LazyLoadingEnabled = true;
            this.Configuration.ProxyCreationEnabled = true;
        }

        // DbSet cho các bảng
        public DbSet<LoaiSP> LoaiSPs { get; set; }
        public DbSet<SanPham> SanPhams { get; set; }
        public DbSet<DonHang> DonHangs { get; set; }
        public DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; }
        

        // Cấu hình model
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Xóa quy ước đặt tên bảng số nhiều
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            // Cấu hình mối quan hệ
            modelBuilder.Entity<SanPham>()
                .HasRequired(s => s.LoaiSP)
                .WithMany(l => l.SanPhams)
                .HasForeignKey(s => s.MaLoaiSP)
                .WillCascadeOnDelete(false);

            base.OnModelCreating(modelBuilder);
        }
    }
}