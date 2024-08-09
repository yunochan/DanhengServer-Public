using EggLink.DanhengServer.Database;
using EggLink.DanhengServer.Database.Inventory;
using EggLink.DanhengServer.Database.Mail;
using EggLink.DanhengServer.GameServer.Game.Player;
using EggLink.DanhengServer.GameServer.Server.Packet.Send.Mail;
using EggLink.DanhengServer.Proto;
using EggLink.DanhengServer.Util;

namespace EggLink.DanhengServer.GameServer.Game.Mail;

public class MailManager(PlayerInstance player) : BasePlayerManager(player)
{
    public MailData MailData { get; } = DatabaseHelper.Instance!.GetInstanceOrCreateNew<MailData>(player.Uid);

    public List<MailInfo> GetMailList()
    {
        return MailData.MailList;
    }

    public MailInfo? GetMail(int mailId)
    {
        return MailData.MailList.Find(x => x.MailID == mailId);
    }

    public void DeleteMail(int mailId)
    {
        var index = MailData.MailList.FindIndex(x => x.MailID == mailId);
        MailData.MailList.RemoveAt(index);
    }

    public async ValueTask SendMail(string sender, string title, string content, int templateId, int expiredDay = 30)
    {
        var mail = new MailInfo
        {
            MailID = MailData.NextMailId++,
            SenderName = sender,
            Content = content,
            Title = title,
            TemplateID = templateId,
            SendTime = DateTime.Now.ToUnixSec(),
            ExpireTime = DateTime.Now.AddDays(expiredDay).ToUnixSec()
        };

        MailData.MailList.Add(mail);

        await Player.SendPacket(new PacketNewMailScNotify(mail.MailID));
    }

    public async ValueTask SendMail(string sender, string title, string content, int templateId,
        List<ItemData> attachments, int expiredDay = 30)
    {
        var mail = new MailInfo
        {
            MailID = MailData.NextMailId++,
            SenderName = sender,
            Content = content,
            Title = title,
            TemplateID = templateId,
            SendTime = DateTime.Now.ToUnixSec(),
            ExpireTime = DateTime.Now.AddDays(expiredDay).ToUnixSec(),
            Attachment = new MailAttachmentInfo
            {
                Items = attachments
            }
        };

        MailData.MailList.Add(mail);

        await Player.SendPacket(new PacketNewMailScNotify(mail.MailID));
    }

    //Send Welcome mail to newly login player
    public async ValueTask SendWelcomeMail()
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

        await Player.SendPacket(new PacketNewMailScNotify(mail.MailID));
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

        foreach (var mail in MailData.MailList) list.Add(mail.ToProto());

        return list;
    }

    public List<ClientMail> ToNoticeMailProto()
    {
        var list = new List<ClientMail>();

        foreach (var mail in MailData.NoticeMailList) list.Add(mail.ToProto());

        return list;
    }
}