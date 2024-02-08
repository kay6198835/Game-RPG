using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    [SerializeField] CoreGameObject CoreGameObject;

    public void Restart()
    {
        CoreGameObject.ResetGame();
        Application.LoadLevel("SampleScene");
    }

    public void TitleReturn()
    {
        CoreGameObject.ResetGame();
        Application.LoadLevel("TitleScreen");
    }
}
