//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebTroChuyen.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class BaiViet
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public BaiViet()
        {
            this.Dislikes = new HashSet<Dislike>();
            this.Likes = new HashSet<Like>();
            this.BaiVietBaoCaos = new HashSet<BaiVietBaoCao>();
            this.BinhLuans = new HashSet<BinhLuan>();
        }
    
        public int BaiVietID { get; set; }
        public string TieuDe { get; set; }
        public string NoiDung { get; set; }
        public System.DateTime NgayDang { get; set; }
        public int UserID { get; set; }
        public int DanhMucID { get; set; }
        public string HinhAnh { get; set; }
        public bool TrangThai { get; set; }
        public Nullable<int> LuotThich { get; set; }
        public Nullable<int> LuotKhongThich { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Dislike> Dislikes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Like> Likes { get; set; }
        public virtual DanhMuc DanhMuc { get; set; }
        public virtual NguoiDung NguoiDung { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BaiVietBaoCao> BaiVietBaoCaos { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BinhLuan> BinhLuans { get; set; }
    }
}
