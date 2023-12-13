using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebTroChuyen.Models;
using System.Data.Entity;
using System.Data.Entity.Validation;

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

            var chiTietBaiViet = db.BaiViets
                .Include(bv => bv.NguoiDung)
                .Include(bv => bv.DanhMuc)
                .FirstOrDefault(bv => bv.BaiVietID == baiVietID);

            if (chiTietBaiViet == null)
            {
                return HttpNotFound();
            }

            // Lấy danh sách bình luận
            var danhSachBinhLuan = db.BinhLuans
                .Where(bl => bl.BaiVietID == baiVietID)
                .Include(bl => bl.NguoiDung)
                .OrderByDescending(bl => bl.NgayBinhLuan)
                .ToList();

            // Truyền danh sách bình luận vào ViewBag
            ViewBag.ChiTietBaiViet = new ChiTietBaiVietModel
            {
                BaiViet = chiTietBaiViet,
                DanhSachBinhLuan = danhSachBinhLuan
            };

            ViewBag.UserId = userId;
            return View(chiTietBaiViet);
        }

        [HttpPost]
        public ActionResult ThemBinhLuan(int baiVietID, string noiDungBinhLuan)
        {
            // Kiểm tra đăng nhập
            var userId = Session["UserID"] as int? ?? 0;
            if (userId == 0)
            {
                // Chưa đăng nhập, xử lý tùy ý (chẳng hạn chuyển hướng đến trang đăng nhập)
                return RedirectToAction("DangNhap", "Users");
            }

            // Tạo một đối tượng BinhLuan mới
            var binhLuan = new BinhLuan
            {
                BaiVietID = baiVietID,
                UserID = userId,
                NoiDung = noiDungBinhLuan,
                NgayBinhLuan = DateTime.Now
            };

            // Kiểm tra validation trước khi lưu vào cơ sở dữ liệu
            if (!ModelState.IsValid)
            {
                // Có lỗi validation
                // Có thể xử lý lỗi hoặc trả về view với thông báo lỗi
                return View("ErrorView");
            }

            try
            {
                // Thêm vào cơ sở dữ liệu
                db.BinhLuans.Add(binhLuan);
                db.SaveChanges();

                // Chuyển hướng lại trang chi tiết bài viết
                return RedirectToAction("XemChiTietBaiViet", new { baiVietID = baiVietID });
            }
            catch (DbEntityValidationException ex)
            {
                // Có lỗi khi validate entity
                // In ra lỗi để debug
                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Console.WriteLine($"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                    }
                }

                // Có thể xử lý lỗi hoặc trả về view với thông báo lỗi
                return View("ErrorView");
            }
            catch (Exception ex)
            {
                // Có lỗi khác khi lưu vào cơ sở dữ liệu
                // In ra lỗi để debug
                Console.WriteLine(ex.Message);

                // Có thể xử lý lỗi hoặc trả về view với thông báo lỗi
                return View("ErrorView");
            }
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