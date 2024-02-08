using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceText : MonoBehaviour
{
    Text text;

    private void Awake()
    {
        text = GetComponent<Text>();
    }

    public void SetSelected(bool selected)
    {
        if (selected)
        {
            text.color = Color.blue;
        }
        else { text.color = Color.black; }
    }

    public Text TextField => text;
}
