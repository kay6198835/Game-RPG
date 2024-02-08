using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, Interactable
{
    [SerializeField] Dialog dialog;

    Merchant merchant;

    private void Awake()
    {
        merchant = GetComponent<Merchant>();
    }

    public IEnumerator Interact()
    {
        if (merchant != null)
        {
            yield return merchant.Trade();
        }
        else
        {
            yield return DialogManager.Instance.ShowDialog(dialog);
            //StartCoroutine(DialogManager.Instance.ShowDialog(dialog));
        } 
    }
}
