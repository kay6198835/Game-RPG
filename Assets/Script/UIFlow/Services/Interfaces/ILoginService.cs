using System;
using System.Collections.Generic;

namespace UIFlow
{
    /// <summary>
    /// Đăng nhập và lấy danh sách server. Dùng callback (Action) để sau này bản thật gọi mạng
    /// bất đồng bộ mà không phải đổi code UI.
    /// </summary>
    public interface ILoginService
    {
        void GetServers(Action<List<ServerInfo>> onLoaded);

        /// <summary>onDone(thànhCông, thôngBáoLỗi).</summary>
        void Login(string userName, string password, ServerInfo server, Action<bool, string> onDone);
    }
}
