using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebTroChuyen.Models;

namespace WebTroChuyen.Controllers
{
    public class DangBaiController : Controller
    {
        private readonly ForumTroChuyenEntities db = new ForumTroChuyenEntities();

        // GET: BaiViet/DangBai
        public ActionResult DangBai()
        {
            // Lấy danh sách danh mục để hiển thị trong dropdown
            var danhMucList = db.DanhMucs.ToList();

            ViewBag.DanhMucList = new SelectList(danhMucList, "DanhMucID", "TenDanhMuc");

            return View();
        }

        [HttpPost]
        public ActionResult DangBai(BaiViet model, HttpPostedFileBase file)
        {
            // Lấy danh sách danh mục để hiển thị trong dropdown
            var danhMucList = db.DanhMucs.ToList();
            ViewBag.DanhMucList = new SelectList(danhMucList, "DanhMucID", "TenDanhMuc");

            // Kiểm tra nếu ModelState không hợp lệ
            if (string.IsNullOrEmpty(model.TieuDe))
            {
                ModelState.AddModelError("TieuDe", "Vui lòng nhập tiêu đề.");
            }

            if (model.DanhMucID == 0)
            {
                ModelState.AddModelError("DanhMucID", "Vui lòng chọn danh mục.");
            }

            if (file == null || file.ContentLength == 0)
            {
                ModelState.AddModelError("HinhAnh", "Vui lòng tải lên hình ảnh.");
            }
            else
            {
                // Kiểm tra định dạng hình ảnh
                var allowedFileExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var fileExtension = System.IO.Path.GetExtension(file.FileName).ToLower();

                if (!allowedFileExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("HinhAnh", "Định dạng hình ảnh không hợp lệ. Chỉ chấp nhận .jpg, .jpeg, .png, .gif.");
                    return View(model);
                }
            }

            if (string.IsNullOrEmpty(model.NoiDung))
            {
                ModelState.AddModelError("NoiDung", "Vui lòng nhập nội dung.");
            }

            // Kiểm tra nếu ModelState không hợp lệ
            if (!ModelState.IsValid)
            {
                return View(model); // Trả về View với thông tin hiện tại và thông báo lỗi
            }

            // Lưu hình ảnh vào thư mục và lấy đường dẫn
            if (file != null && file.ContentLength > 0)
            {
                var fileName = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(file.FileName);
                var path = System.IO.Path.Combine(Server.MapPath("~/Images/UserImagePost"), fileName);
                file.SaveAs(path);

                model.HinhAnh = fileName;
            }

            // Tiếp tục xử lý và lưu vào cơ sở dữ liệu
            // Lưu bài viết vào cơ sở dữ liệu
            model.NgayDang = DateTime.Now;
            int? userID = Session["UserID"] as int?;

            // Kiểm tra xem UserID có tồn tại hay không
            if (userID.HasValue)
            {
                // Lưu UserID vào bài viết
                model.UserID = userID.Value;

                db.BaiViets.Add(model);
                db.SaveChanges();

                return RedirectToAction("Index", "BaiViet");
            }
            else
            {
                return RedirectToAction("DangNhap", "Users");
            }
        }
    }
}