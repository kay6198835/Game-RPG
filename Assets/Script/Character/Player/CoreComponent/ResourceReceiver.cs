/// <summary>
/// Player pickup interactor. Items apply themselves to the owning ICharacter (see ItemController);
/// this component only finds and triggers the interaction. Kept as a class because
/// PlayerTest.prefab references it by GUID.
/// </summary>
public class ResourceReceiver : Interact
{
}
