using NUnit.Framework;
using UnityEngine;

// Mirrors QA-POOL-01..03 (QASystemsSuite). The manager is built with new GameObject + AddComponent and no
// VContainer resolver: Pool.Spawn() skips injection when the resolver is null.
public class ObjectPoolManagerTests
{
    private GameObject _managerObject;
    private ObjectPoolManager _pool;
    private GameObject _template;

    [SetUp]
    public void CreatePool()
    {
        _managerObject = new GameObject("EditModeTest_PoolManager");
        _pool = _managerObject.AddComponent<ObjectPoolManager>();
        _template = new GameObject("EditModeTest_PoolTemplate");
        _template.transform.position = new Vector3(500f, 500f, 0f);
    }

    // Spawned instances are parented under the manager, so destroying it removes them too.
    [TearDown]
    public void DestroyPool()
    {
        if (_managerObject != null) Object.DestroyImmediate(_managerObject);
        if (_template != null) Object.DestroyImmediate(_template);
    }

    // QA-POOL-01
    [Test]
    public void Spawn_ReturnsActiveInstanceAtRequestedPosition()
    {
        Vector2 at = new Vector2(3f, -2f);

        GameObject spawned = _pool.Spawn(_template, at, Quaternion.identity);

        Assert.IsNotNull(spawned);
        Assert.IsTrue(spawned.activeInHierarchy, "spawned instance is not active");
        Assert.AreNotSame(_template, spawned, "the pool handed out the template itself");
        Assert.Less(Vector2.Distance(spawned.transform.position, at), 0.001f, $"spawned at {(Vector2)spawned.transform.position}, asked {at}");
    }

    // QA-POOL-02
    [Test]
    public void Release_DeactivatesAndNextSpawnReusesTheSameInstance()
    {
        GameObject first = _pool.Spawn(_template, Vector2.zero, Quaternion.identity);

        _pool.Release(first);
        Assert.IsFalse(first.activeSelf, "released instance is still active");

        Vector2 at = new Vector2(1f, 1f);
        GameObject second = _pool.Spawn(_template, at, Quaternion.identity);

        Assert.AreSame(first, second, "a new instance was created instead of reusing the released one");
        Assert.IsTrue(second.activeSelf, "reused instance was not re-activated");
        Assert.Less(Vector2.Distance(second.transform.position, at), 0.001f, "reused instance was not moved to the new position");
    }

    // QA-POOL-03
    [Test]
    public void Spawn_TwiceWithoutRelease_GivesTwoDistinctInstances()
    {
        GameObject a = _pool.Spawn(_template, Vector2.zero, Quaternion.identity);
        GameObject b = _pool.Spawn(_template, Vector2.zero, Quaternion.identity);

        Assert.IsNotNull(a);
        Assert.IsNotNull(b);
        Assert.AreNotSame(a, b);
    }

    [Test]
    public void Release_Twice_DoesNotQueueTheInstanceTwice()
    {
        GameObject first = _pool.Spawn(_template, Vector2.zero, Quaternion.identity);
        _pool.Release(first);
        _pool.Release(first);

        GameObject a = _pool.Spawn(_template, Vector2.zero, Quaternion.identity);
        GameObject b = _pool.Spawn(_template, Vector2.zero, Quaternion.identity);

        Assert.AreSame(first, a);
        Assert.AreNotSame(a, b, "a double release handed the same instance out twice");
    }

    [Test]
    public void Release_ObjectThatNeverCameFromThePool_IsIgnored()
    {
        Assert.DoesNotThrow(() => _pool.Release(_template));
        Assert.IsTrue(_template.activeSelf, "an object the pool does not own was deactivated");
    }
}
