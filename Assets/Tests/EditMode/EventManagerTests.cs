using System;
using System.Collections.Generic;
using NUnit.Framework;

// Mirrors QA-EVT-01..03 (QASystemsSuite). EventManager is static: every test uses ON_TEST only and removes
// its own listeners in TearDown.
public class EventManagerTests
{
    private readonly List<Action<object>> _registered = new List<Action<object>>();

    private void Listen(Action<object> listener)
    {
        _registered.Add(listener);
        EventManager.Resgister(EventID.ON_TEST, listener);
    }

    [TearDown]
    public void RemoveListeners()
    {
        foreach (Action<object> listener in _registered) EventManager.UnResgister(EventID.ON_TEST, listener);
        _registered.Clear();
    }

    // QA-EVT-01
    [Test]
    public void Emit_DeliversPayloadOnceToRegisteredListener()
    {
        object received = null;
        int calls = 0;
        Listen(o => { received = o; calls++; });
        object payload = new object();

        EventManager.Emit(EventID.ON_TEST, payload);

        Assert.AreEqual(1, calls);
        Assert.AreSame(payload, received);
    }

    [Test]
    public void Emit_WithoutPayload_DeliversNull()
    {
        object received = new object();
        Listen(o => received = o);

        EventManager.Emit(EventID.ON_TEST);

        Assert.IsNull(received);
    }

    // QA-EVT-02
    [Test]
    public void UnResgister_StopsDelivery()
    {
        int calls = 0;
        Action<object> listener = _ => calls++;
        Listen(listener);

        EventManager.UnResgister(EventID.ON_TEST, listener);
        EventManager.Emit(EventID.ON_TEST);

        Assert.AreEqual(0, calls);
    }

    // QA-EVT-03, first half
    [Test]
    public void Emit_CallsEveryListenerOfTheEventOnce()
    {
        int a = 0, b = 0;
        Listen(_ => a++);
        Listen(_ => b++);

        EventManager.Emit(EventID.ON_TEST);

        Assert.AreEqual(1, a);
        Assert.AreEqual(1, b);
    }

    // QA-EVT-03, second half
    [Test]
    public void Emit_AfterEveryListenerRemoved_DoesNotThrow()
    {
        Action<object> listener = _ => { };
        Listen(listener);
        EventManager.UnResgister(EventID.ON_TEST, listener);

        Assert.DoesNotThrow(() => EventManager.Emit(EventID.ON_TEST));
    }

    [Test]
    public void UnResgister_OneListener_KeepsTheOther()
    {
        int a = 0, b = 0;
        Action<object> first = _ => a++;
        Listen(first);
        Listen(_ => b++);

        EventManager.UnResgister(EventID.ON_TEST, first);
        EventManager.Emit(EventID.ON_TEST);

        Assert.AreEqual(0, a);
        Assert.AreEqual(1, b);
    }

    [Test]
    public void Emit_DoesNotReachListenersOfAnotherEvent()
    {
        int calls = 0;
        Listen(_ => calls++);

        EventManager.Emit(EventID.ON_ROOM_CLEAR);

        Assert.AreEqual(0, calls);
    }
}
