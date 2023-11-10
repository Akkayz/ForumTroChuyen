using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using WebTroChuyen.Models; // Import namespace cho model Chat
using System;
using System.Linq;

namespace WebTroChuyen
{
    public class ChatHub : Hub
    {
        private ForumTroChuyenEntities db = new ForumTroChuyenEntities(); // Khai báo đối tượng DbContext

        public async Task Send(string tenUser, string tinNhan)
        {
            int userId = GetUserIdByName(tenUser);

            var chatMessage = new Chat
            {
                UserID = userId,
                TinNhan = tinNhan,
                ThoiGianGui = DateTime.Now
            };

            db.Chats.Add(chatMessage); // Thêm tin nhắn vào DbSet
            await db.SaveChangesAsync(); // Sử dụng await cho hoạt động IO

            // Gửi tin nhắn đến tất cả các client khác
            Clients.All.addNewMessageToPage(tenUser, tinNhan);
        }

        private int GetUserIdByName(string tenUser)
        {
            var user = db.NguoiDungs.FirstOrDefault(u => u.UserName == tenUser);
            return user != null ? user.UserID : 0;
        }
    }
}