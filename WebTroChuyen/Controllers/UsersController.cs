using System;
using System.IO;
using System.Web.Mvc;
using WebTroChuyen.Models;
using System.Linq;

namespace WebTroChuyen.Controllers
{
    public class UsersController : Controller
    {
        private ForumTroChuyenEntities db = new ForumTroChuyenEntities();

        [HttpGet]
        public ActionResult DangKy()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DangKy(FormCollection collection)
        {
            string tenNguoiDung = collection["TenNguoiDung"];
            string userName = collection["UserName"];
            string password = collection["Password"];
            string email = collection["Email"];
            string gioiTinh = collection["GioiTinh"];

            if (string.IsNullOrEmpty(tenNguoiDung) || string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(gioiTinh))
            {
                ModelState.AddModelError("ThongBaoLoi", "Vui lòng điền đầy đủ thông tin.");
                return View();
            }

            byte gioiTinhByte;
            if (!byte.TryParse(gioiTinh, out gioiTinhByte) || (gioiTinhByte != 0 && gioiTinhByte != 1))
            {
                ModelState.AddModelError("GioiTinh", "Giới tính không hợp lệ. Vui lòng chọn 0 nếu là nam hoặc 1 nếu là nữ.");
                return View();
            }

            string avatarFileName = ""; // Khai báo biến avatarFileName và gán giá trị mặc định

            // Kiểm tra và xử lý tệp ảnh
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);

                    // Gán tên tệp ảnh cho avatarFileName
                    avatarFileName = fileName;

                    // Lưu tệp ảnh vào thư mục lưu trữ (ví dụ: "~/Images/UserAvatar/")
                    var path = Path.Combine(Server.MapPath("~/Images/UserAvatar"), fileName);
                    file.SaveAs(path);
                }
            }

            var ngayDangKy = DateTime.Now;
            byte trangThai = 1; // Ép kiểu trangThai thành byte

            // Kiểm tra xem tệp ảnh đã tồn tại trong cơ sở dữ liệu hay chưa
            bool avatarExistsInDB = db.NguoiDungs.Any(nguoiDung => nguoiDung.Avatar == avatarFileName);

            if (avatarExistsInDB)
            {
                ModelState.AddModelError("Avatar", "Tên tệp đã tồn tại trong cơ sở dữ liệu. Vui lòng chọn tên tệp khác hoặc đổi tên tệp.");
                return View();
            }

            // Thêm dữ liệu vào cơ sở dữ liệu mà không sử dụng đối tượng model
            var newUser = new NguoiDung
            {
                TenNguoiDung = tenNguoiDung,
                UserName = userName,
                Password = password,
                Email = email,
                GioiTinh = gioiTinhByte, // Sử dụng giá trị byte
                Avatar = avatarFileName,
                NgayDangKy = ngayDangKy,
                TrangThai = trangThai
            };

            db.NguoiDungs.Add(newUser);
            db.SaveChanges();

            // Điều hướng đến trang đăng nhập hoặc trang chính
            return RedirectToAction("DangNhap", "Users");
        }

        [HttpGet]
        public ActionResult DangNhap()
        {
            return View();
        }

        private bool AvatarFileExists(string fileName)
        {
            var path = Path.Combine(Server.MapPath("~/Images/UserAvatar"), fileName);
            return System.IO.File.Exists(path);
        }
    }
}