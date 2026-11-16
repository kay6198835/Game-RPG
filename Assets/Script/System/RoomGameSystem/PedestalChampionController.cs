
using UIFlow;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PedestalChampionController : InteractiveObjectsRoom
{
    [SerializeField] private PlayerData dataChampition;
    protected override void Awake()
    {
        base.Awake();
        interactData = new InteractData
        {
            textMessage = "Press " + KeyCode.E.ToString() + " to conversation class",
            keyCode = KeyCode.E
        };
    }

    public override bool Interact(Interact interactor)
    {
        if (base.Interact(interactor))
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

    void Start()
    {
        transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
    }


}