using EggLink.DanhengServer.Util;
using SqlSugar;

namespace EggLink.DanhengServer.Database.Account;

[SugarTable("Account")]
public class AccountData : BaseDatabaseDataHelper
{
    public string? Username { get; set; }

    [SugarColumn(IsNullable = true)] public string? ComboToken { get; set; }

    [SugarColumn(IsNullable = true)] public string? DispatchToken { get; set; }

    [SugarColumn(IsNullable = true)] 
    public string? Permissions { get; set; } // type: permission1,permission2,permission3...
    
    [SugarColumn(DefaultValue = "false")] public bool IsBan { get; set; }

    [SugarColumn(IsNullable = true)] public string? IP { get; set; }

    [SugarColumn(IsNullable = true)] public int? Count { get; set; }

    [SugarColumn(IsNullable = true)] public string? BanMsg { get; set; }

    public static AccountData? GetAccountByUserName(string username)
    {
        AccountData? result = null;
        DatabaseHelper.GetAllInstance<AccountData>()?.ForEach(account =>
        {
            if (account.Username == username) result = account;
        });
        return result;
    }

    public static AccountData? GetAccountByUid(int uid)
    {
        var result = DatabaseHelper.Instance?.GetInstance<AccountData>(uid);
        return result;
    }

    public string GenerateDispatchToken()
    {
        DispatchToken = Crypto.CreateSessionKey(Uid.ToString());
        DatabaseHelper.Instance?.UpdateInstance(this);
        return DispatchToken;
    }

    public string GenerateComboToken()
    {
        ComboToken = Crypto.CreateSessionKey(Uid.ToString());
        DatabaseHelper.Instance?.UpdateInstance(this);
        return ComboToken;
    }

    public void SetIsBan(bool isBan)
    {
        IsBan = isBan;
        DatabaseHelper.Instance?.UpdateInstance(this);
    }

    public void SetIP(string? ip)
    {
        IP = ip;
        DatabaseHelper.Instance?.UpdateInstance(this);
    }

    public void SetCount(int? count)
    {
        Count = count ?? 0;
        DatabaseHelper.Instance?.UpdateInstance(this);
    }

    public void SetBanMessage(string? msg)
    {
        BanMsg = msg;
        DatabaseHelper.Instance?.UpdateInstance(this);
    }
    
}