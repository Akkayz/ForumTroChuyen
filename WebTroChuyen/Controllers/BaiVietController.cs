using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebTroChuyen.Models;

namespace WebTroChuyen.Controllers
{
    public class BaiVietController : Controller
    {
        private readonly ForumTroChuyenEntities db = new ForumTroChuyenEntities();

        public ActionResult BaiViet()
        {
            var danhSachBaiViet = db.BaiViets.ToList();
            var topThanhVien = db.NguoiDungs
                .OrderByDescending(u => u.CapDo)
                .Take(5)
                .ToList();

            ViewBag.DanhSachBaiViet = danhSachBaiViet;
            ViewBag.TopThanhVien = topThanhVien;

            return View();
        }
    }
}