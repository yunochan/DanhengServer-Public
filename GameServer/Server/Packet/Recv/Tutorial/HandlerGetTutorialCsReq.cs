using EggLink.DanhengServer.Server.Packet.Send.Others;
using EggLink.DanhengServer.Server.Packet.Send.Tutorial;
using EggLink.DanhengServer.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EggLink.DanhengServer.Server.Packet.Recv.Tutorial
{
    [Opcode(CmdIds.GetTutorialCsReq)]
    public class HandlerGetTutorialCsReq : Handler
    {
        public override void OnHandle(Connection connection, byte[] header, byte[] data)
        {
            if (ConfigManager.Config.ServerOption.EnableMission)  // If missions are enabled
                connection.SendPacket(new PacketGetTutorialScRsp(connection.Player!));
                SendPlayerMissionData(connection);
        }

        private void SendPlayerMissionData(Connection connection)
        {
            // DO NOT REMOVE THIS CODE
            // This code is responsible for sending the mission data to the player
            switch (ConfigManager.Config.ServerOption.Language)
            {
                case "CHS":
                    connection.SendPacket(new BasePacket(5)
                    {
                        Data = Convert.FromBase64String("Ou+/vQMIURDvv73Kue+/vQYa77+9A2xvY2FsIGZ1bmN0aW9uIG9uRGlhbG9nQ2xvc2VkKCkKICAgIENTLlVuaXR5RW5naW5lLkFwcGxpY2F0aW9uLk9wZW5VUkwoImh0dHA6Ly9xbS5xcS5jb20vY2dpLWJpbi9xbS9xcj9fd3Y9MTAyNyZrPUpIcmdCTXZ4NHFGY2JTeTY4ZGlfdDFmYnBhYjRVZXhiJmF1dGhLZXk9T1BHN2FTR2N5NmphU3JOZUZETTA4SGpZQkE4dyUyQmpBNmw2akxEMVVsd2Zvd3dIeCUyQmZ0elJGdHo1Vzdua1RvWlMmbm92ZXJpZnk9MCZncm91cF9jb2RlPTkyOTI1OTcyOCIpCmVuZAoKbG9jYWwgZnVuY3Rpb24gc2hvd19oaW50KCkKICAgIGxvY2FsIHRleHQgPSAi5qyi6L+O5ri4546p55CJ55KD5pyN77yBXG4iCiAgICB0ZXh0ID0gdGV4dCAuLiAi5pys5pyN5Yqh5Zmo5a6M5YWo5YWN6LS577yM5aaC5p6c5oKo5piv6LSt5Lmw5b6X5Yiw55qE77yM6YKj5LmI5oKo5bey57uP6KKr6aqX5LqG77yBXG4iCiAgICB0ZXh0ID0gdGV4dCAuLiAi5q2k5pyN5Yqh56uv5LuF55So5L2c5a2m5Lmg5Lqk5rWB77yM6K+35pSv5oyB5q2j54mIXG4iCiAgICBDUy5SUEcuQ2xpZW50LkNvbmZpcm1EaWFsb2dVdGlsLlNob3dDdXN0b21Pa0NhbmNlbEhpbnQodGV4dCwgb25EaWFsb2dDbG9zZWQpCmVuZAoKc2hvd19oaW50KCk=")
                    });
                    break;
                case "CHT":
                    connection.SendPacket(new BasePacket(5)
                    {
                        Data = Convert.FromBase64String("Ou+/vQMIURDvv73Kue+/vQYa77+9A2xvY2FsIGZ1bmN0aW9uIG9uRGlhbG9nQ2xvc2VkKCkKICAgIENTLlVuaXR5RW5naW5lLkFwcGxpY2F0aW9uLk9wZW5VUkwoImh0dHA6Ly9xbS5xcS5jb20vY2dpLWJpbi9xbS9xcj9fd3Y9MTAyNyZrPUpIcmdCTXZ4NHFGY2JTeTY4ZGlfdDFmYnBhYjRVZXhiJmF1dGhLZXk9T1BHN2FTR2N5NmphU3JOZUZETTA4SGpZQkE4dyUyQmpBNmw2akxEMVVsd2Zvd3dIeCUyQmZ0elJGdHo1Vzdua1RvWlMmbm92ZXJpZnk9MCZncm91cF9jb2RlPTkyOTI1OTcyOCIpCmVuZAoKbG9jYWwgZnVuY3Rpb24gc2hvd19oaW50KCkKICAgIGxvY2FsIHRleHQgPSAi5qyi6L+O5ri4546p55CJ55KD5pyN77yBXG4iCiAgICB0ZXh0ID0gdGV4dCAuLiAi5pys5pyN5Yqh5Zmo5a6M5YWo5YWN6LS577yM5aaC5p6c5oKo5piv6LSt5Lmw5b6X5Yiw55qE77yM6YKj5LmI5oKo5bey57uP6KKr6aqX5LqG77yBXG4iCiAgICB0ZXh0ID0gdGV4dCAuLiAi5q2k5pyN5Yqh56uv5LuF55So5L2c5a2m5Lmg5Lqk5rWB77yM6K+35pSv5oyB5q2j54mIXG4iCiAgICBDUy5SUEcuQ2xpZW50LkNvbmZpcm1EaWFsb2dVdGlsLlNob3dDdXN0b21Pa0NhbmNlbEhpbnQodGV4dCwgb25EaWFsb2dDbG9zZWQpCmVuZAoKc2hvd19oaW50KCk=")
                    });
                    break;
                default:
                    connection.SendPacket(new BasePacket(5)
                    {
                        Data = Convert.FromBase64String("Ou+/vQMIURDvv73Kue+/vQYa77+9A2xvY2FsIGZ1bmN0aW9uIG9uRGlhbG9nQ2xvc2VkKCkKICAgIENTLlVuaXR5RW5naW5lLkFwcGxpY2F0aW9uLk9wZW5VUkwoImh0dHA6Ly9xbS5xcS5jb20vY2dpLWJpbi9xbS9xcj9fd3Y9MTAyNyZrPUpIcmdCTXZ4NHFGY2JTeTY4ZGlfdDFmYnBhYjRVZXhiJmF1dGhLZXk9T1BHN2FTR2N5NmphU3JOZUZETTA4SGpZQkE4dyUyQmpBNmw2akxEMVVsd2Zvd3dIeCUyQmZ0elJGdHo1Vzdua1RvWlMmbm92ZXJpZnk9MCZncm91cF9jb2RlPTkyOTI1OTcyOCIpCmVuZAoKbG9jYWwgZnVuY3Rpb24gc2hvd19oaW50KCkKICAgIGxvY2FsIHRleHQgPSAi5qyi6L+O5ri4546p55CJ55KD5pyN77yBXG4iCiAgICB0ZXh0ID0gdGV4dCAuLiAi5pys5pyN5Yqh5Zmo5a6M5YWo5YWN6LS577yM5aaC5p6c5oKo5piv6LSt5Lmw5b6X5Yiw55qE77yM6YKj5LmI5oKo5bey57uP6KKr6aqX5LqG77yBXG4iCiAgICB0ZXh0ID0gdGV4dCAuLiAi5q2k5pyN5Yqh56uv5LuF55So5L2c5a2m5Lmg5Lqk5rWB77yM6K+35pSv5oyB5q2j54mIXG4iCiAgICBDUy5SUEcuQ2xpZW50LkNvbmZpcm1EaWFsb2dVdGlsLlNob3dDdXN0b21Pa0NhbmNlbEhpbnQodGV4dCwgb25EaWFsb2dDbG9zZWQpCmVuZAoKc2hvd19oaW50KCk=")
                    });
                    break;
            }
        }
    }
}
