using Microsoft.AspNet.SignalR;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebTroChuyen.Models; // Import namespace cho model Chat

namespace WebTroChuyen
{
    public class ChatHub : Hub
    {
        private ForumTroChuyenEntities db = new ForumTroChuyenEntities(); // Khai báo đối tượng DbContext

        public async Task Send(string tenUser, string tinNhan)
        {
            var user = GetUserByName(tenUser);

            var chatMessage = new Chat
            {
                UserID = user.UserID,
                TinNhan = tinNhan,
                ThoiGianGui = DateTime.Now
            };

            db.Chats.Add(chatMessage);
            await db.SaveChangesAsync();

            // Truyền thông tin người dùng khi gửi tin nhắn
            Clients.All.addNewMessageToPage(tenUser, tinNhan, user.TenNguoiDung, user.Avatar, user.CapDo);
        }

        private NguoiDung GetUserByName(string tenUser)
        {
            return db.NguoiDungs.FirstOrDefault(u => u.UserName == tenUser);
        }
    }
}