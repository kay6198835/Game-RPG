using System;
using System.Collections.Generic;

namespace UIFlow
{
    /// <summary>
    /// Game hiện là offline, không có server để gọi. Bản Real trả về một server "Offline" duy nhất
    /// và luôn cho vào.
    /// TODO: nối logic thật — gọi API đăng nhập / danh sách server khi game có online.
    /// </summary>
    public class RealLoginService : ILoginService
    {
        public void GetServers(Action<List<ServerInfo>> onLoaded)
        {
            onLoaded?.Invoke(new List<ServerInfo>
            {
                new() { serverName = "Offline", status = ServerStatus.Online, pingMs = 0 },
            });
        }

        public void Login(string userName, string password, ServerInfo server, Action<bool, string> onDone)
        {
            onDone?.Invoke(true, "Chơi offline.");
        }
    }
}
