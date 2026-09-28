using UnityEngine;
using UnityEngine.UI;

namespace UIFlow
{
    /// <summary>Lớp cha của một trang trong cửa sổ tab. OnTabShown chạy mỗi lần trang được chọn.</summary>
    public abstract class TabPage : MonoBehaviour
    {
        public abstract string Title { get; }

        public virtual void OnTabShown() { }

        public virtual void OnTabHidden() { }
    }

    /// <summary>
    /// Cửa sổ quản lý nhiều tab (Túi đồ, Nhân vật, Cây kỹ năng, Nhiệm vụ). Q/E chuyển tab (đổi được trong Cài đặt).
    /// Đặt pausesGame = true trong Inspector: game dừng khi cửa sổ mở, nên phím Q/E (cũng là phím gameplay)
    /// không làm nhân vật ra chiêu trong lúc đang xem túi đồ.
    /// </summary>
    public class TabWindow : UIPanel
    {
        [SerializeField] private Button[] tabButtons;
        [SerializeField] private TabPage[] pages;
        [SerializeField] private Button closeButton;
        [SerializeField] private Color activeTabColor = new(0.95f, 0.7f, 0.2f);
        [SerializeField] private Color inactiveTabColor = new(1f, 1f, 1f, 0.15f);

        private int _current;

        public int CurrentTab => _current;

        protected override void Awake()
        {
            base.Awake();
            for (int i = 0; i < tabButtons.Length; i++)
            {
                int index = i;
                tabButtons[i].onClick.AddListener(() => SelectTab(index));
            }
            closeButton.onClick.AddListener(CloseSelf);
        }

        protected override void OnShown() => SelectTab(_current);

        protected override void OnHidden()
        {
            if (_current < pages.Length) pages[_current].OnTabHidden();
        }

        private void Update()
        {
            if (!IsVisible) return;
            if (UIInput.WasPressed(KeyBindings.TabLeft)) SelectTab(_current - 1);
            else if (UIInput.WasPressed(KeyBindings.TabRight)) SelectTab(_current + 1);
        }

        public void SelectTab(int index)
        {
            if (pages.Length == 0) return;
            index = (index % pages.Length + pages.Length) % pages.Length;   // Vòng tròn: tab cuối → E → tab đầu

            if (index != _current) pages[_current].OnTabHidden();
            _current = index;
            for (int i = 0; i < pages.Length; i++)
            {
                pages[i].gameObject.SetActive(i == index);
                tabButtons[i].targetGraphic.color = i == index ? activeTabColor : inactiveTabColor;
            }
            pages[index].OnTabShown();
        }
    }
}
