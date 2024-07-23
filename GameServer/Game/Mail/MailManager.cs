using EggLink.DanhengServer.Database;
using EggLink.DanhengServer.Database.Inventory;
using EggLink.DanhengServer.Database.Mail;
using EggLink.DanhengServer.Game;
using EggLink.DanhengServer.Game.Player;
using EggLink.DanhengServer.GameServer.Server.Packet.Send.Mail;
using EggLink.DanhengServer.Proto;
using EggLink.DanhengServer.Util;
using EggLink.DanhengServer.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace EggLink.DanhengServer.GameServer.Game.Mail
{
    public class MailManager(PlayerInstance player) : BasePlayerManager(player)
    {
        public MailData MailData { get; private set; } = DatabaseHelper.Instance!.GetInstanceOrCreateNew<MailData>(player.Uid);

        public List<MailInfo> GetMailList()
        {
            return MailData.MailList;
        }

        public MailInfo? GetMail(int mailId)
        {
            return MailData.MailList.Find(x => x.MailID == mailId);
        }

        public void SendMail(string sender, string title, string content, int templateId, int expiredDay = 30)
        {
            var mail = new MailInfo()
            {
                MailID = MailData.NextMailId++,
                SenderName = sender,
                Content = content,
                Title = title,
                TemplateID = templateId,
                SendTime = DateTime.Now.ToUnixSec(),
                ExpireTime = DateTime.Now.AddDays(expiredDay).ToUnixSec(),
            };

            MailData.MailList.Add(mail);

            Player.SendPacket(new PacketNewMailScNotify(mail.MailID));
        }

        public void SendMail(string sender, string title, string content, int templateId, List<ItemData> attachments, int expiredDay = 30)
        {
            var mail = new MailInfo()
            {
                MailID = MailData.NextMailId++,
                SenderName = sender,
                Content = content,
                Title = title,
                TemplateID = templateId,
                SendTime = DateTime.Now.ToUnixSec(),
                ExpireTime = DateTime.Now.AddDays(expiredDay).ToUnixSec(),
                Attachment = new()
                {
                    Items = attachments
                }
            };

            MailData.MailList.Add(mail);

            Player.SendPacket(new PacketNewMailScNotify(mail.MailID));
        }
        
        //Send Welcome mail to newly login player
        public void sendWelcomeMail()
        {
            var welcomeMail = ConfigManager.Config.ServerOption.WelcomeMail;
            var mail = new MailInfo()
            {
                MailID = MailData.NextMailId++,
                SenderName = welcomeMail.SenderName,
                Content = welcomeMail.Content,
                Title = welcomeMail.Title,
                TemplateID = 1,
                SendTime = DateTime.Now.ToUnixSec(),
                ExpireTime = DateTime.Now.AddDays(welcomeMail.ExpiredDay).ToUnixSec(),
                Attachment = new()
                {
                    Items = welcomeMail.Attachment
                }
            };

            MailData.MailList.Add(mail);

            Player.SendPacket(new PacketNewMailScNotify(mail.MailID));
        }

        public List<ClientMail> ToMailProto()
        {
            var list = new List<ClientMail>();

            foreach (var mail in MailData.MailList)
            {
                list.Add(mail.ToProto());
            }

            return list;
        }

        public List<ClientMail> ToNoticeMailProto()
        {
            var list = new List<ClientMail>();

            foreach (var mail in MailData.NoticeMailList)
            {
                list.Add(mail.ToProto());
            }

            return list;
        }
    }
}
