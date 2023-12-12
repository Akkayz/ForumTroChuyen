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
            var danhSachBaiViet = db.BaiViets
                .Where(bv => bv.TrangThai)
                .ToList();
            var topThanhVien = db.NguoiDungs
                .OrderByDescending(u => u.CapDo)
                .Take(5)
                .ToList();

            // Lấy UserID từ Session
            var userId = Session["UserID"] as int? ?? 0;

            ViewBag.DanhSachBaiViet = danhSachBaiViet;
            ViewBag.TopThanhVien = topThanhVien;
            ViewBag.UserId = userId;

            return View();
        }

        [HttpPost]
        public ActionResult LikeBaiViet(int baiVietID, int userID)
        {
            var existingLike = db.Likes
                .FirstOrDefault(l => l.BaiVietID == baiVietID && l.UserID == userID);

            var baiViet = db.BaiViets.Find(baiVietID);

            if (existingLike == null)
            {
                // Thêm like
                var newLike = new Like
                {
                    BaiVietID = baiVietID,
                    UserID = userID
                };

                db.Likes.Add(newLike);
                baiViet.LuotThich = (baiViet.LuotThich ?? 0) + 1;
            }
            else
            {
                // Xoá like
                db.Likes.Remove(existingLike);
                baiViet.LuotThich = (baiViet.LuotThich ?? 0) - 1;
            }

            db.SaveChanges();

            var result = new
            {
                success = true,
                luotThich = baiViet.LuotThich,
                daLike = existingLike != null
            };

            return Json(result);
        }
    }
}