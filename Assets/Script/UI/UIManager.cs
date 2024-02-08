using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject gameOverScreen;

    public void GameOverUI()
    {
        Debug.Log("game over");
        gameOverScreen.SetActive(true);
    }
}
