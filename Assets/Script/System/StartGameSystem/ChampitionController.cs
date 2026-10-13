
using UIFlow;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(CircleCollider2D))]
public class ChampitionController : InteractiveObjects
{
    [SerializeField] private PlayerData dataChampition;
    [SerializeField] private Sprite avatarChampition;
    [SerializeField] private CircleCollider2D circleCollider;
    [SerializeField] private SpriteRenderer spriteRenderer;
    ICharacter character;

    public override bool Interact(Interact interactor)
    {
        character = interactor.GetComponentInParent<ICharacter>();
        if (character != null)
        {
            DialogueLine dialogueLine = new DialogueLine
            {
                speaker = dataChampition.name,
                text = "Chào mừng bạn đến với thế giới của chúng tôi! Ta là " +
                dataChampition.name +
                ", người sẽ hướng dẫn bạn trong hành trình này. Hãy chuẩn bị cho những thử thách và phiêu lưu phía trước!"
            };
            //UIEvents.RequestDialogue(new DialogueLine[] { dialogueLine }, ConfirmSetCharacterData);
            UIEvents.OnOpenConfirmPanel(SetCharacterData, new ConfirnData
            {
                confirmText = "Xác nhận",
                cancelText = "Hủy"
            });
            return true;
        }
        return false;
    }

    void SetCharacterData()
    {
        character.SetCharacterData(dataChampition);
        EventManager.Emit(EventID.ON_OPEN_DOOR);
    }

    protected override void Awake()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = avatarChampition;
    }

    void Start()
    {
        transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
    }


}