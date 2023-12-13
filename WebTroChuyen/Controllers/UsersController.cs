using System;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using WebTroChuyen.Models;

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
        public ActionResult DangKy(NguoiDung model, HttpPostedFileBase AvatarFile, string ConfirmPassword)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(model.TenNguoiDung))
                {
                    ModelState.AddModelError("TenNguoiDung", "Vui lòng nhập tên người dùng.");
                    return View(model);
                }
                // Kiểm tra ô "Tên người dùng" (UserName) có được nhập hay không
                if (string.IsNullOrEmpty(model.UserName))
                {
                    ModelState.AddModelError("UserName", "Vui lòng nhập tên tài khoản");
                    return View(model);
                }

                // Kiểm tra ô "Email" có được nhập và có đúng định dạng email không
                if (string.IsNullOrEmpty(model.Email) || !IsValidEmail(model.Email))
                {
                    ModelState.AddModelError("Email", "Vui lòng nhập một địa chỉ email hợp lệ.");
                    return View(model);
                }

                // Kiểm tra ô "Mật khẩu" (Password) có được nhập hay không
                if (string.IsNullOrEmpty(model.Password))
                {
                    ModelState.AddModelError("Password", "Vui lòng nhập mật khẩu.");
                    return View(model);
                }

                // Kiểm tra ô "Xác nhận mật khẩu" (ConfirmPassword) có trùng khớp với mật khẩu hay không
                if (model.Password != ConfirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", "Xác nhận mật khẩu không khớp với mật khẩu.");
                    return View(model);
                }

                // Kiểm tra ô "Ảnh đại diện" (AvatarFile) có được tải lên hay không
                if (AvatarFile == null || AvatarFile.ContentLength == 0)
                {
                    ModelState.AddModelError("Avatar", "Vui lòng tải lên ảnh đại diện.");
                    return View(model);
                }

                // Kiểm tra xem UserName đã tồn tại trong cơ sở dữ liệu hay chưa
                bool userNameExists = db.NguoiDungs.Any(nguoiDung => nguoiDung.UserName == model.UserName);

                if (userNameExists)
                {
                    ModelState.AddModelError("UserName", "Tên người dùng đã được sử dụng. Vui lòng chọn tên người dùng khác.");
                    return View(model);
                }

                // Kiểm tra xem Email đã tồn tại trong cơ sở dữ liệu hay chưa
                bool emailExists = db.NguoiDungs.Any(nguoiDung => nguoiDung.Email == model.Email);

                if (emailExists)
                {
                    ModelState.AddModelError("Email", "Địa chỉ email đã được sử dụng. Vui lòng chọn địa chỉ email khác.");
                    return View(model);
                }

                int gioiTinh = model.GioiTinh;

                if (gioiTinh != 0 && gioiTinh != 1)
                {
                    ModelState.AddModelError("GioiTinh", "Giới tính không hợp lệ. Vui lòng chọn 0 nếu là nam hoặc 1 nếu là nữ.");
                    return View(model);
                }

                string avatarFileName = model.Avatar; // Lấy tên tệp ảnh từ model

                // Kiểm tra và xử lý tệp ảnh
                if (AvatarFile != null && AvatarFile.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(AvatarFile.FileName);

                    // Gán tên tệp ảnh cho avatarFileName
                    avatarFileName = fileName;

                    // Lưu tệp ảnh vào thư mục lưu trữ (ví dụ: "~/Images/UserAvatar/")
                    var path = Path.Combine(Server.MapPath("~/Images/UserAvatar"), fileName);
                    AvatarFile.SaveAs(path);
                }

                var ngayDangKy = DateTime.Now;
                byte trangThai = 1; // Ép kiểu trangThai thành byte
                int vaiTro = 1;   // Đặt giá trị mặc định cho VaiTro là 1
                int capDo = 1;     // Đặt giá trị mặc định cho CapDo là 1

                // Kiểm tra xem tệp ảnh đã tồn tại trong cơ sở dữ liệu hay chưa
                bool avatarExistsInDB = db.NguoiDungs.Any(nguoiDung => nguoiDung.Avatar == avatarFileName);

                if (avatarExistsInDB)
                {
                    ModelState.AddModelError("Avatar", "Tên tệp đã tồn tại trong cơ sở dữ liệu. Vui lòng chọn tên tệp khác hoặc đổi tên tệp.");
                    return View(model);
                }

                // Cập nhật model với thông tin mới
                model.Avatar = avatarFileName;
                model.NgayDangKy = ngayDangKy;
                model.TrangThai = trangThai;
                model.VaitroID = vaiTro;
                model.CapDo = capDo;
                model.GioiTinh = gioiTinh; // Chuyển giới tính thành kiểu st

                // Thêm dữ liệu vào cơ sở dữ liệu
                db.NguoiDungs.Add(model);
                db.SaveChanges();

                // Điều hướng đến trang đăng nhập hoặc trang chính
                return RedirectToAction("DangNhap", "Users");
            }

            return View(model);
        }

        [HttpGet]
        public ActionResult DangNhap()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DangNhap(NguoiDung model)
        {
            if (ModelState.IsValid)
            {
                var user = db.NguoiDungs.FirstOrDefault(u => u.UserName == model.UserName && u.Password == model.Password);

                if (user != null)
                {
                    Session["UserID"] = user.UserID;
                    Session["UserName"] = user.UserName;
                    Session["TenNguoiDung"] = user.TenNguoiDung;
                    Session["Avatar"] = user.Avatar;
                    Session["CapDo"] = user.CapDo;

                    // Lưu thông tin người dùng vào Context.User.Identity.Name
                    System.Web.HttpContext.Current.User = new GenericPrincipal(new GenericIdentity(user.UserName), null);

                    return RedirectToAction("Index", "BaiViet");
                }
                else
                {
                    ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
                }
            }

            return View(model);
        }

        public ActionResult Logout()
        {
            // Xóa thông tin đăng nhập khỏi Session
            Session.RemoveAll();

            // Điều hướng người dùng đến trang chính hoặc bất kỳ trang nào bạn muốn
            return RedirectToAction("DangBai", "DangBai"); // Ví dụ: điều hướng đến trang chính của ứng dụng
        }

        private bool IsValidEmail(string email)
        {
            string pattern = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
            Match match = Regex.Match(email, pattern);
            return match.Success;
        }
    }
}