using System;
using System.Collections.Generic;

namespace UIFlow
{
    /// <summary>
    /// Đăng nhập giả: luôn thành công nếu có nhập tên và server đang mở.
    /// TODO: nối logic thật — bản Real gọi server thật.
    /// </summary>
    public class MockLoginService : ILoginService
    {
        public void GetServers(Action<List<ServerInfo>> onLoaded)
        {
            onLoaded?.Invoke(MockCatalog.CreateServers());
        }

        public void Login(string userName, string password, ServerInfo server, Action<bool, string> onDone)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                onDone?.Invoke(false, "Hãy nhập tên đăng nhập.");
                return;
            }
            if (server == null)
            {
                onDone?.Invoke(false, "Hãy chọn một server.");
                return;
            }
            if (server.status == ServerStatus.Maintenance)
            {
                onDone?.Invoke(false, "Server " + server.serverName + " đang bảo trì.");
                return;
            }
            if (server.status == ServerStatus.Full)
            {
                onDone?.Invoke(false, "Server " + server.serverName + " đã đầy.");
                return;
            }

            // Bản giả không kiểm tra mật khẩu.
            onDone?.Invoke(true, "Đã vào " + server.serverName + ".");
        }
    }
}
