using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

// Mirrors QA-MAP-03 (QASystemsSuite): MapModel.GetRandomRoom is a shuffle bag.
public class MapModelTests
{
    private const string MapModelPath = "Assets/SO/Database/Map/MapModel.asset";

    private readonly List<Object> _created = new List<Object>();
    private Random.State _randomState;

    [SetUp]
    public void SeedRandom()
    {
        _randomState = Random.state;
        Random.InitState(20261112);
    }

    [TearDown]
    public void Cleanup()
    {
        Random.state = _randomState;
        TestSupport.DestroyAll(_created);
    }

    private T Create<T>(string name) where T : ScriptableObject
    {
        T instance = ScriptableObject.CreateInstance<T>();
        instance.name = name;
        _created.Add(instance);
        return instance;
    }

    private MapModel NewMap(int rooms)
    {
        MapModel map = Create<MapModel>("TestMap");
        for (int i = 0; i < rooms; i++) map.fullRoomList.Add(Create<RoomModel>("TestRoom" + i));
        return map;
    }

    [Test]
    public void GetRandomRoom_DealsEveryRoomOncePerCycle()
    {
        MapModel map = NewMap(5);

        var cycle = new List<RoomModel>();
        for (int i = 0; i < 5; i++) cycle.Add(map.GetRandomRoom());

        CollectionAssert.AreEquivalent(map.fullRoomList, cycle);
    }

    [Test]
    public void GetRandomRoom_StartsANewCycleWhenTheBagIsEmpty()
    {
        MapModel map = NewMap(3);
        for (int i = 0; i < 3; i++) map.GetRandomRoom();

        var second = new List<RoomModel>();
        for (int i = 0; i < 3; i++) second.Add(map.GetRandomRoom());

        CollectionAssert.AreEquivalent(map.fullRoomList, second);
    }

    [Test]
    public void GetRandomRoom_SkipsNullEntries()
    {
        MapModel map = NewMap(2);
        map.fullRoomList.Insert(1, null);

        RoomModel first = map.GetRandomRoom();
        RoomModel second = map.GetRandomRoom();

        Assert.IsNotNull(first);
        Assert.IsNotNull(second);
        Assert.AreNotSame(first, second);
    }

    [Test]
    public void GetRandomRoom_WithNoRoom_ReturnsNull()
    {
        Assert.IsNull(NewMap(0).GetRandomRoom());
    }

    [Test]
    public void GetRandomRoom_WithOnlyNullEntries_ReturnsNull()
    {
        MapModel map = NewMap(0);
        map.fullRoomList.Add(null);
        map.fullRoomList.Add(null);

        Assert.IsNull(map.GetRandomRoom());
    }

    // QA-MAP-03 on the committed asset, through an Instantiate copy.
    [Test]
    public void CommittedMapModel_DealsEveryRoomOncePerCycle()
    {
        MapModel map = Object.Instantiate(TestSupport.LoadAsset<MapModel>(MapModelPath));
        _created.Add(map);
        int rooms = map.fullRoomList.Count(r => r != null);
        Assert.Greater(rooms, 0, "MapModel.asset has no room");

        var cycle = new List<RoomModel>();
        for (int i = 0; i < rooms; i++) cycle.Add(map.GetRandomRoom());

        Assert.AreEqual(rooms, cycle.Where(r => r != null).Distinct().Count(), $"{rooms} draws did not give {rooms} distinct rooms");
        Assert.IsNotNull(map.GetRandomRoom(), "GetRandomRoom returned null after a full cycle");
    }
}
