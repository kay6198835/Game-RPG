#if UNITY_EDITOR
using System;
using System.Collections;
using UnityEngine;

namespace QA.Automation
{
    // Base of every QA suite. A suite lists its cases in Run(); each case is a coroutine that asserts
    // numbers or state through Check / Fail / Skip / Observe. Cleanup() removes what the suite created.
    public abstract class QASuite : MonoBehaviour
    {
        protected QAHarness H { get; private set; }

        public void Bind(QAHarness harness) => H = harness;

        public abstract IEnumerator Run();

        public virtual void Cleanup() { }

        protected IEnumerator Case(string id, string system, QAPriority priority, string title, IEnumerator body)
            => H.Case(id, system, priority, title, body);

        protected void Check(bool condition, string pass, string fail) => H.Check(condition, pass, fail);
        protected void Fail(string message) => H.Fail(message);
        protected void Skip(string message) => H.Skip(message);
        protected void Observe(string message) => H.Observe(message);
        protected void Note(string message) => H.Note(message);

        protected static bool Near(float a, float b, float tolerance = 0.01f) => Mathf.Abs(a - b) <= tolerance;

        protected static IEnumerator Frames(int count)
        {
            for (int i = 0; i < count; i++) yield return null;
        }

        // Game time; the harness fixes it to 1/60 s per frame, so a wait is a fixed number of frames.
        protected static IEnumerator Wait(float seconds)
        {
            float end = Time.time + seconds;
            while (Time.time < end) yield return null;
        }

        protected static IEnumerator WaitUntil(Func<bool> condition, float timeout, Action<bool> result)
        {
            float end = Time.time + timeout;
            while (!condition())
            {
                if (Time.time >= end)
                {
                    result(false);
                    yield break;
                }
                yield return null;
            }
            result(true);
        }

        // Polls for the whole window and reports whether the condition was ever true.
        protected static IEnumerator WatchFor(float seconds, Func<bool> condition, Action<bool> seen)
        {
            float end = Time.time + seconds;
            bool any = false;
            while (Time.time < end)
            {
                any |= condition();
                yield return null;
            }
            seen(any);
        }
    }
}
#endif
