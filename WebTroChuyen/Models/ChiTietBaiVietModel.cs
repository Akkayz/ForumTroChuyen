using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebTroChuyen.Models
{
    public class ChiTietBaiVietModel
    {
        public BaiViet BaiViet { get; set; }
        public List<BinhLuan> DanhSachBinhLuan { get; set; }
    }
}