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
        public ActionResult LikeBaiViet(int baiVietID)
        {
            var userId = Session["UserID"] as int?;

            if (!userId.HasValue)
            {
                return Json(new { success = false, message = "Bạn cần đăng nhập để thích bài viết." });
            }

            try
            {
                var existingLike = db.Likes
                    .FirstOrDefault(l => l.BaiVietID == baiVietID && l.UserID == userId);

                var baiViet = db.BaiViets.Find(baiVietID);

                if (existingLike == null)
                {
                    var newLike = new Like
                    {
                        BaiVietID = baiVietID,
                        UserID = userId.Value
                    };

                    db.Likes.Add(newLike);
                    baiViet.LuotThich = (baiViet.LuotThich ?? 0) + 1;
                }
                else
                {
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
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Đã xảy ra lỗi: {ex.Message}" });
            }
        }

        [HttpPost]
        public ActionResult DislikeBaiViet(int baiVietID)
        {
            var userId = Session["UserID"] as int? ?? 0;

            if (userId == 0)
            {
                return Json(new { success = false, message = "Bạn cần đăng nhập để không thích bài viết." });
            }

            var existingDislike = db.Dislikes
                .FirstOrDefault(d => d.BaiVietID == baiVietID && d.UserID == userId);

            var baiViet = db.BaiViets.Find(baiVietID);

            if (existingDislike == null)
            {
                var newDislike = new Dislike
                {
                    BaiVietID = baiVietID,
                    UserID = userId
                };

                db.Dislikes.Add(newDislike);
                baiViet.LuotKhongThich = (baiViet.LuotKhongThich ?? 0) + 1;
            }
            else
            {
                db.Dislikes.Remove(existingDislike);
                baiViet.LuotKhongThich = (baiViet.LuotKhongThich ?? 0) - 1;
            }

            db.SaveChanges();

            var result = new
            {
                success = true,
                luotKhongThich = baiViet.LuotKhongThich,
                daDislike = existingDislike != null
            };

            return Json(result);
        }
    }
}