using System.Collections;
using UnityEngine;

public class SimpleCharacterMotor : MonoBehaviour
{
    private Coroutine _moveRoutine;

    public void Lunge(Vector3 direction, float distance, float duration)
    {
        if (_moveRoutine != null)
        {
            StopCoroutine(_moveRoutine);
        }

        _moveRoutine = StartCoroutine(LungeRoutine(direction, distance, duration));
    }

    private IEnumerator LungeRoutine(Vector3 direction, float distance, float duration)
    {
        direction.y = 0f;
        direction.Normalize();

        Vector3 start = transform.position;
        Vector3 end = start + direction * distance;

        float time = 0f;
        duration = Mathf.Max(0.01f, duration);

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            transform.position = Vector3.Lerp(start, end, t);
            yield return null;
        }

        transform.position = end;
        _moveRoutine = null;
    }
}
