public class UpgradeController : InteractiveObjects
{
    ICharacter<ICore> character;
    IPlayerStatService playerStatService;
    public override bool Interact(Interact interactor)
    {
        character = interactor.GetComponentInParent<ICharacter<ICore>>();
        if (character != null)
        {
            if (character.Core.TryGetCapability(out playerStatService))
            {
                playerStatService.TryLevelup();
            }
        }
        return true;
    }


}