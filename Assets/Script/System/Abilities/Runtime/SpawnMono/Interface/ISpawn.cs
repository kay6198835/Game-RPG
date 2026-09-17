public interface ISpawn
{
    void Launch(Vector2 target, float speed, float lifetime,
                AbilityContext context, Action<object[]> execute);
    void DespawnOneSelf();
    IEnumerator DespawnOneselfAffterDuration();
}