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
		
				// 处理参数拆分
				if (arg.Length > 1)
				{
					// 查找第一个数字的位置
					int index = arg.IndexOfAny("0123456789".ToCharArray());
					
					if (index > 0 && index < arg.Length)
					{
						var key = arg.Substring(0, index); // 字符部分
						var value = arg.Substring(index); // 数字部分
		
						// 存储到 CharacterArgs 中
						if (!CharacterArgs.ContainsKey(key))
						{
							CharacterArgs[key] = value;
							Args.Add(arg);
						}
					}
					else
					{
						// 如果没有数字部分，处理为单字符键
						if (arg.Length == 1 && !int.TryParse(arg[0].ToString(), out _))
						{
							CharacterArgs[arg] = ""; // 单字符键
							Args.Add(arg);
						}
						else
						{
							// 其他情况直接添加为 BasicArgs
							BasicArgs.Add(arg);
							Args.Add(arg);
						}
					}
				}
				else
				{
					// 处理单个字符的参数
					if (arg.Length == 1 && !int.TryParse(arg[0].ToString(), out _))
					{
						CharacterArgs[arg] = ""; // 单字符键
						Args.Add(arg);
					}
					else
					{
						// 其他情况直接添加为 BasicArgs
						BasicArgs.Add(arg);
						Args.Add(arg);
					}
				}
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
