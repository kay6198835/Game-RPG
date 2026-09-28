using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIFlow
{
    /// <summary>
    /// Đăng nhập + chọn server. Chỉ nói chuyện với ILoginService, không biết bản Mock hay Real.
    /// </summary>
    public class LoginPanel : UIPanel
    {
        [SerializeField] private TMP_InputField userNameInput;
        [SerializeField] private TMP_InputField passwordInput;
        [SerializeField] private Transform serverListContainer;
        [Tooltip("Một dòng server mẫu (đang tắt). Script nhân bản nó cho mỗi server.")]
        [SerializeField] private Button serverRowTemplate;
        [SerializeField] private Button loginButton;
        [SerializeField] private TMP_Text statusText;

        private readonly List<Button> _rows = new();
        private List<ServerInfo> _servers = new();
        private ServerInfo _selected;

        public event Action LoggedIn;

        protected override void Awake()
        {
            base.Awake();
            serverRowTemplate.gameObject.SetActive(false);
            loginButton.onClick.AddListener(OnLoginClicked);
            passwordInput.contentType = TMP_InputField.ContentType.Password;
        }

        protected override void OnShown()
        {
            statusText.text = "Chọn server rồi đăng nhập.";
            UIServices.Login.GetServers(BuildServerList);
        }

        private void BuildServerList(List<ServerInfo> servers)
        {
            _servers = servers ?? new List<ServerInfo>();
            foreach (Button row in _rows) Destroy(row.gameObject);
            _rows.Clear();

            foreach (ServerInfo server in _servers)
            {
                Button row = Instantiate(serverRowTemplate, serverListContainer);
                row.gameObject.SetActive(true);
                row.GetComponentInChildren<TMP_Text>().text =
                    $"{server.serverName}   <color=#{ColorUtility.ToHtmlStringRGB(StatusColor(server.status))}>{StatusText(server.status)}</color>   {server.pingMs} ms";
                ServerInfo captured = server;   // Bắt biến riêng cho mỗi vòng lặp để lambda không dùng nhầm server cuối
                row.onClick.AddListener(() => Select(captured));
                _rows.Add(row);
            }

            // Tự chọn server Online đầu tiên cho người chơi đỡ phải bấm.
            Select(_servers.Find(s => s.status == ServerStatus.Online) ?? (_servers.Count > 0 ? _servers[0] : null));
        }

        private void Select(ServerInfo server)
        {
            _selected = server;
            for (int i = 0; i < _rows.Count; i++)
            {
                Image background = _rows[i].GetComponent<Image>();
                background.color = _servers[i] == server ? new Color(0.35f, 0.55f, 0.9f) : new Color(1f, 1f, 1f, 0.12f);
            }
        }

        private void OnLoginClicked()
        {
            loginButton.interactable = false;
            statusText.text = "Đang đăng nhập…";
            UIServices.Login.Login(userNameInput.text, passwordInput.text, _selected, (success, message) =>
            {
                loginButton.interactable = true;
                statusText.text = message;
                if (success) LoggedIn?.Invoke();
            });
        }

        private static string StatusText(ServerStatus status) => status switch
        {
            ServerStatus.Online => "Tốt",
            ServerStatus.Busy => "Đông",
            ServerStatus.Full => "Đầy",
            _ => "Bảo trì",
        };

        private static Color StatusColor(ServerStatus status) => status switch
        {
            ServerStatus.Online => new Color(0.4f, 1f, 0.4f),
            ServerStatus.Busy => new Color(1f, 0.85f, 0.3f),
            ServerStatus.Full => new Color(1f, 0.4f, 0.3f),
            _ => new Color(0.6f, 0.6f, 0.6f),
        };
    }
}
