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
using Google.Protobuf.Collections;

namespace EggLink.DanhengServer.GameServer.Game.Mail
{
    public class MailManager(PlayerInstance player) : BasePlayerManager(player)
    {
        private readonly static Logger logger = new("Mail");
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
        public void SendWelcomeMail()
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

            // Debug
            logger.Debug($"发送邮件 ID: {mail.MailID}");
            
            foreach (var item in mail.Attachment.Items)
            {
                logger.Debug($"物品 ID: {item.ItemId}, 数量: {item.Count}");
            }

            MailData.MailList.Add(mail);

            // 打印邮件列表数量以调试
            logger.Debug($"邮件列表总数: {MailData.MailList.Count}");

            Player.SendPacket(new PacketNewMailScNotify(mail.MailID));
        }
        
        public List<MailInfo> TakeMailAttachments(RepeatedField<uint> mailIdList)
        {
            List<MailInfo> attachments = new List<MailInfo>();

            List<int> idList = mailIdList.Select(id => (int)id).ToList();

            if (idList == null || idList.Count == 0)
            {
                idList = MailData.MailList.Select(mail => mail.MailID).ToList();
            }

            foreach (int id in idList)
            {
                var mail = MailData.MailList.FirstOrDefault(x => x.MailID == id);

                if (mail == null || mail.IsRead || mail.Attachment == null || mail.Attachment.Items.Count == 0)
                {
                    continue;
                }

                foreach (var item in mail.Attachment.Items)
                {
                    Player.InventoryManager!.AddItem(item.ItemId, item.Count, true, item.Rank, item.Level, sync:true);
                }

                mail.IsRead = true;
                attachments.Add(mail);
            }

            DatabaseHelper.Instance?.UpdateInstance(MailData);

            return attachments;
        }

		public List<int> DeleteMail(RepeatedField<uint> mailIdList)
		{
			List<int> deleteList = new List<int>();
		
			List<int> idList = mailIdList.Select(id => (int)id).ToList();
		
			if (idList.Count == 0)
			{
				idList = MailData.MailList.Select(mail => mail.MailID).ToList();
			}
		
			foreach (int id in idList)
			{
				var mail = MailData.MailList.FirstOrDefault(x => x.MailID == id);
				if (mail == null || !mail.IsRead)
				{
					continue;
				}
		
				MailData.MailList.Remove(mail);
				deleteList.Add(mail.MailID);
			}
		
			DatabaseHelper.Instance?.UpdateInstance(MailData);
		
			return deleteList;
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
