using System;
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
            var messages = db.Chats.OrderBy(chat => chat.ThoiGianGui).ToList();

            return View(messages);
        }
    }
}