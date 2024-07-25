using EggLink.DanhengServer.Server;
using EggLink.DanhengServer.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EggLink.DanhengServer.Command
{
    public class CommandArg
    {
        public string Raw { get; }
        public List<string> Args { get; } = [];
        public List<string> BasicArgs { get; } = [];
        public Dictionary<string, string> CharacterArgs { get; } = [];
        public Connection? Target { get; set; }
        public ICommandSender Sender { get; }

        public CommandArg(string raw, ICommandSender sender, Connection? con = null)
        {
            Raw = raw;
            Sender = sender;
            var args = raw.Split(' ');
            foreach (var arg in args)
            {
                if (string.IsNullOrEmpty(arg))
                {
                    continue;
                }
                // 处理长度大于1的参数，支持两个字符的键
                if (arg.Length > 1 && !int.TryParse(arg[0].ToString(), out var _) && arg[0] != '-')
                {
                    if (arg.Length > 2)
                    {
                        var key = arg.Substring(0, 2); // 取前两个字符作为键
                        var value = arg.Substring(2); // 剩下的部分作为值
                        if (!CharacterArgs.ContainsKey(key))
                        {
                            CharacterArgs[key] = value;
                            Args.Add(arg);
                            continue;
                        }
                    }
                }
                BasicArgs.Add(arg);
                Args.Add(arg);
            }
            if (con != null)
            {
                Target = con;
            }

            CharacterArgs.TryGetValue("@", out var target);
            if (target != null)
            {
                var connection = Listener.Connections.Values.ToList().Find(item => item.Player?.Uid.ToString() == target);
                if (connection != null)
                {
                    Target = connection;
                }
            }
        }
        public int GetInt(int index)
        {
            if (BasicArgs.Count <= index)
            {
                return 0;
            }
            _ = int.TryParse(BasicArgs[index], out int res);
            return res;
        }

        public void SendMsg(string msg)
        {
            Sender.SendMsg(msg);
        }

        public override string ToString()
        {
            return $"BasicArg: {BasicArgs.ToArrayString()}. CharacterArg: {CharacterArgs.ToJsonString()}.";
        }
    }
}
