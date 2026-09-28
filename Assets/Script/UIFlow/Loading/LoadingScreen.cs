using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UIFlow
{
    /// <summary>
    /// Màn loading dùng chung cho mọi lần chuyển scene. Đọc scene đích từ SceneFlow.TargetScene.
    ///
    /// Thanh tiến trình là THẬT: lấy từ AsyncOperation.progress. Lưu ý của Unity:
    /// khi allowSceneActivation = false, progress dừng ở 0.9 (phần 0.9 → 1 là bước kích hoạt scene).
    /// Vì vậy chia cho 0.9 để thanh chạy đủ 0 → 100%.
    /// </summary>
    public class LoadingScreen : MonoBehaviour
    {
        [SerializeField] private Image progressFill;          // Image kiểu Filled, Fill Method = Horizontal
        [SerializeField] private TMP_Text percentText;
        [SerializeField] private TMP_Text tipText;
        [SerializeField] private TMP_Text targetText;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private GameObject errorGroup;
        [SerializeField] private TMP_Text errorText;
        [SerializeField] private Button backToMenuButton;

        [Header("Nội dung ngẫu nhiên")]
        [Tooltip("Ảnh nền, chọn ngẫu nhiên mỗi lần load. Để trống thì dùng màu nền ngẫu nhiên bên dưới.")]
        [SerializeField] private Sprite[] backgrounds;
        [SerializeField] private Color[] fallbackBackgroundColors =
        {
            new(0.09f, 0.1f, 0.16f), new(0.14f, 0.08f, 0.1f), new(0.07f, 0.13f, 0.11f), new(0.12f, 0.1f, 0.06f),
        };
        [Tooltip("Mẹo hiện khi load. Để trống thì dùng mẹo mẫu trong MockCatalog.")]
        [SerializeField, TextArea] private string[] tips;
        [SerializeField] private float tipInterval = 3f;

        private float _displayedProgress;

        private void Start()
        {
            errorGroup.SetActive(false);
            backToMenuButton.onClick.AddListener(SceneFlow.ReturnToMainMenu);
            PickRandomBackground();
            StartCoroutine(RotateTips());
            StartCoroutine(LoadTarget(SceneFlow.TargetScene));
        }

        private IEnumerator LoadTarget(string sceneName)
        {
            targetText.text = "Đang tải: " + FriendlyName(sceneName);
            SetProgress(0f);

            // Scene chưa có trong Build Settings thì LoadSceneAsync trả về null → báo lỗi dễ hiểu thay vì treo.
            if (!Application.CanStreamedLevelBeLoaded(sceneName))
            {
                ShowError($"Không tìm thấy scene \"{sceneName}\" trong File > Build Settings.");
                yield break;
            }

            float startTime = Time.unscaledTime;
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            operation.allowSceneActivation = false;   // Giữ màn loading lại cho tới khi mình cho phép

            while (operation.progress < 0.9f)
            {
                SetProgress(operation.progress / 0.9f);
                yield return null;
            }

            // Load xong rất nhanh thì vẫn giữ màn loading tối thiểu minLoadingTime để kịp đọc mẹo.
            while (Time.unscaledTime - startTime < UIServices.Config.minLoadingTime || _displayedProgress < 0.999f)
            {
                SetProgress(1f);
                yield return null;
            }

            operation.allowSceneActivation = true;   // Chuyển scene; scene Loading tự bị hủy (LoadSceneMode.Single)
        }

        private void SetProgress(float target)
        {
            // Thanh chạy mượt về giá trị thật, không nhảy cóc.
            _displayedProgress = Mathf.MoveTowards(_displayedProgress, target, Time.unscaledDeltaTime * 1.5f);
            progressFill.fillAmount = _displayedProgress;
            percentText.text = Mathf.RoundToInt(_displayedProgress * 100f) + "%";
        }

        private IEnumerator RotateTips()
        {
            string[] source = tips != null && tips.Length > 0 ? tips : MockCatalog.LoadingTips;
            int last = -1;
            while (true)
            {
                int index = Random.Range(0, source.Length);
                if (source.Length > 1 && index == last) index = (index + 1) % source.Length;   // Không lặp lại mẹo vừa hiện
                last = index;
                tipText.text = source[index];
                yield return new WaitForSecondsRealtime(tipInterval);
            }
        }

        private void PickRandomBackground()
        {
            if (backgrounds != null && backgrounds.Length > 0)
            {
                backgroundImage.sprite = backgrounds[Random.Range(0, backgrounds.Length)];
                backgroundImage.color = Color.white;
                return;
            }
            backgroundImage.sprite = null;
            backgroundImage.color = fallbackBackgroundColors[Random.Range(0, fallbackBackgroundColors.Length)];
        }

        private void ShowError(string message)
        {
            errorGroup.SetActive(true);
            errorText.text = message;
            Debug.LogError("[LoadingScreen] " + message);
        }

        private static string FriendlyName(string sceneName) => sceneName switch
        {
            SceneNames.MainGamePlay => "Menu chính",
            SceneNames.GameplayReal => "Hầm ngục",
            SceneNames.GameplayMock => "Hầm ngục (bản giả để test UI)",
            _ => sceneName,
        };
    }
}
