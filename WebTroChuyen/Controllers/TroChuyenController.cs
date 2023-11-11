using System.Data.Entity; // Thêm using này để sử dụng Include
using System.Linq;
using System.Web.Mvc;
using WebTroChuyen.Models;

namespace WebTroChuyen.Controllers
{
    public class TroChuyenController : Controller
    {
        private ForumTroChuyenEntities db = new ForumTroChuyenEntities();

        public ActionResult Index()
        {
            var messages = db.Chats.Include("NguoiDung").OrderByDescending(chat => chat.ThoiGianGui).Take(15).ToList();
            messages.Reverse();
            return View(messages);
        }
    }
}