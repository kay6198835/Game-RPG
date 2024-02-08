using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] GameObject gameoverUI;

    public static GameOverUI instance { get; private set; }

    private void Awake()
    {
        instance = this;
    }

    public void GameOver()
    {
        gameoverUI.SetActive(true);
    }
}
