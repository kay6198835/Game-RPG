using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UIFlow
{
    /// <summary>
    /// Thông báo tự ẩn: mỗi dòng hiện displaySeconds giây rồi mờ dần và biến mất.
    /// Tối đa maxVisible dòng; dòng cũ nhất bị đẩy ra khi có dòng mới.
    /// Các dòng được tái sử dụng (pool nhỏ) thay vì Instantiate/Destroy liên tục.
    /// </summary>
    public class NotificationFeed : MonoBehaviour
    {
        [SerializeField] private Transform container;
        [SerializeField] private CanvasGroup entryTemplate;   // Có một TMP_Text con
        [SerializeField] private int maxVisible = 4;
        [SerializeField] private float displaySeconds = 3f;
        [SerializeField] private float fadeSeconds = 0.5f;

        private readonly Queue<CanvasGroup> _pool = new();
        private readonly List<CanvasGroup> _active = new();
        // Mỗi lần một dòng được dùng lại thì "thế hệ" tăng lên. Coroutine của lần dùng trước thấy
        // thế hệ đã đổi sẽ tự dừng, không ẩn nhầm thông báo mới.
        private readonly Dictionary<CanvasGroup, int> _generation = new();

        private void Awake()
        {
            entryTemplate.gameObject.SetActive(false);
        }

        public void Show(string message)
        {
            if (_active.Count >= maxVisible) Recycle(_active[0]);

            CanvasGroup entry = _pool.Count > 0 ? _pool.Dequeue() : Instantiate(entryTemplate, container);
            entry.gameObject.SetActive(true);
            entry.transform.SetAsLastSibling();
            entry.alpha = 1f;
            entry.GetComponentInChildren<TMP_Text>().text = message;
            _active.Add(entry);

            _generation.TryGetValue(entry, out int generation);
            _generation[entry] = ++generation;
            StartCoroutine(Expire(entry, generation));
        }

        private IEnumerator Expire(CanvasGroup entry, int generation)
        {
            // Realtime: thông báo vẫn tự ẩn cả khi đang mở menu tạm dừng.
            yield return new WaitForSecondsRealtime(displaySeconds);
            float time = 0f;
            while (time < fadeSeconds)
            {
                if (_generation[entry] != generation) yield break;
                time += Time.unscaledDeltaTime;
                entry.alpha = 1f - time / fadeSeconds;
                yield return null;
            }
            if (_generation[entry] == generation && _active.Contains(entry)) Recycle(entry);
        }

        private void Recycle(CanvasGroup entry)
        {
            _active.Remove(entry);
            _generation[entry]++;
            entry.gameObject.SetActive(false);
            _pool.Enqueue(entry);
        }
    }
}
