using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebTroChuyen.Models;
using System.Data.Entity;

namespace WebTroChuyen.Controllers
{
    public class BaiVietController : Controller
    {
        private readonly ForumTroChuyenEntities db = new ForumTroChuyenEntities();

        public ActionResult Index()
        {
            var topThanhVien = db.NguoiDungs
                .OrderByDescending(u => u.CapDo)
                .Take(5)
                .ToList();

            // Lấy UserID từ Session
            var userId = Session["UserID"] as int? ?? 0;

            ViewBag.TopThanhVien = topThanhVien;
            ViewBag.UserId = userId;

            return View();
        }

        public ActionResult BaiVietPartial()
        {
            var userId = Session["UserID"] as int? ?? 0;
            var danhSachBaiViet = db.BaiViets
                .Where(bv => bv.TrangThai)
                .ToList();
            ViewBag.UserId = userId;
            ViewBag.DanhSachBaiViet = danhSachBaiViet;
            return PartialView("");
        }

        // Add this action to your BaiVietController
        public ActionResult XemChiTietBaiViet(int baiVietID)
        {
            var userId = Session["UserID"] as int? ?? 0;

            // Retrieve the specific post for the given baiVietID
            var chiTietBaiViet = db.BaiViets
                .Include(bv => bv.NguoiDung)
                .Include(bv => bv.DanhMuc)
                .FirstOrDefault(bv => bv.BaiVietID == baiVietID);

            if (chiTietBaiViet == null)
            {
                return HttpNotFound(); // Or handle the case where the post is not found
            }

            ViewBag.UserId = userId;
            return View(chiTietBaiViet); // Truyền dữ liệu chiTietBaiViet đến view
        }

        [HttpPost]
        public ActionResult ToggleReaction(int baiVietID, bool isLike)
        {
            var userId = Session["UserID"] as int? ?? 0;

            if (userId == 0)
            {
                return Json(new { success = false, message = "Bạn cần đăng nhập để thích bài viết." });
            }

            var existingLike = db.Likes
                .FirstOrDefault(l => l.BaiVietID == baiVietID && l.UserID == userId);

            var existingDislike = db.Dislikes
                .FirstOrDefault(d => d.BaiVietID == baiVietID && d.UserID == userId);

            var baiViet = db.BaiViets.Find(baiVietID);

            if (isLike)
            {
                if (existingLike == null && existingDislike == null)
                {
                    var newLike = new Like
                    {
                        BaiVietID = baiVietID,
                        UserID = userId
                    };

                    db.Likes.Add(newLike);
                    baiViet.LuotThich = (baiViet.LuotThich ?? 0) + 1;
                }
                else if (existingLike != null)
                {
                    db.Likes.Remove(existingLike);
                    baiViet.LuotThich = (baiViet.LuotThich ?? 0) - 1;
                }
                else if (existingDislike != null)
                {
                    db.Dislikes.Remove(existingDislike);
                    baiViet.LuotKhongThich = (baiViet.LuotKhongThich ?? 0) - 1;

                    var newLike = new Like
                    {
                        BaiVietID = baiVietID,
                        UserID = userId
                    };

                    db.Likes.Add(newLike);
                    baiViet.LuotThich = (baiViet.LuotThich ?? 0) + 1;
                }
            }
            else // Dislike
            {
                if (existingDislike == null && existingLike == null)
                {
                    var newDislike = new Dislike
                    {
                        BaiVietID = baiVietID,
                        UserID = userId
                    };

                    db.Dislikes.Add(newDislike);
                    baiViet.LuotKhongThich = (baiViet.LuotKhongThich ?? 0) + 1;
                }
                else if (existingDislike != null)
                {
                    db.Dislikes.Remove(existingDislike);
                    baiViet.LuotKhongThich = (baiViet.LuotKhongThich ?? 0) - 1;
                }
                else if (existingLike != null)
                {
                    db.Likes.Remove(existingLike);
                    baiViet.LuotThich = (baiViet.LuotThich ?? 0) - 1;

                    var newDislike = new Dislike
                    {
                        BaiVietID = baiVietID,
                        UserID = userId
                    };

                    db.Dislikes.Add(newDislike);
                    baiViet.LuotKhongThich = (baiViet.LuotKhongThich ?? 0) + 1;
                }
            }

            db.SaveChanges();

            var result = new
            {
                success = true,
                luotThich = baiViet.LuotThich,
                luotKhongThich = baiViet.LuotKhongThich,
                daLike = existingLike != null,
                daDislike = existingDislike != null
            };

            return Json(result);
        }
    }
}