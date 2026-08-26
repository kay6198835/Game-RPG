public class ItemController : InteractiveObjects
{
    [SerializeField] private Sprite ItemSprite { get; protected set; }
    [SerializeField] private string NameItem { get; protected set; }
    [SerializeField] private int Id { get; protected set; }
    [SerializeField] private ItemOS Data { get; protected set; }

    void Awake()
    {
        ItemSprite = GetComponent<Sprite>();
    }

    void SetDataItem(ItemOS data)
    {
        Data = data;
    }

    public override bool Interact(Interact interactor)
    {
        // Do something apply effect 
        Data.Apply((ResourceReceiver)interactor);
    }


}