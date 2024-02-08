using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { FreeRoam, Talking, Shopping}
public class GameController : MonoBehaviour
{
    //
    [SerializeField] CoreGameObject coreGameObject;
    //
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] PlayerController playerController;
    [SerializeField] Character character;

    GameState state;

    private void Awake()
    {
        coreGameObject = GetComponentInParent<CoreGameObject>();
        playerMovement = coreGameObject.Player.gameObject.GetComponent<PlayerMovement>();
        playerController = coreGameObject.Player.gameObject.GetComponent<PlayerController>();
        character = coreGameObject.Player.gameObject.GetComponent<Character>();
    }

    private void Start()
    {
        DialogManager.Instance.OnShowDialog += () => 
        {
            state = GameState.Talking;
        };

        DialogManager.Instance.OnHideDialog += () =>
        {
            state = GameState.FreeRoam;
        };

        ShopController.i.OnStartShopping += () => state = GameState.Shopping;
        ShopController.i.OnFinishShopping += () => state = GameState.FreeRoam;
    }

    private void Update()
    {
        if (state == GameState.FreeRoam)
        {
            playerMovement.enabled = true;
            playerController.HandleUpdate();
        }

        else if (state == GameState.Talking)
        {
            playerMovement.enabled = false;
            DialogManager.Instance.HandleUpdate();  
        }

        else if (state == GameState.Shopping)
        {
            playerMovement.enabled = false;
            ShopController.i.HandleUpdate();
        }

        if (character.isDeath == true)
        {
            GameOverUI.instance.GameOver();
        }
    }
}
