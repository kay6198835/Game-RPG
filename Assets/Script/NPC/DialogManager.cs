using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    [SerializeField] GameObject dialogBox;
    [SerializeField] ChoiceBox choiceBox;
    [SerializeField] Text dialogText;
    [SerializeField] int lettersPerSecond;

    public event Action OnShowDialog, OnHideDialog;

    public static DialogManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public bool IsShowing { get; private set; }

    //Dialog dialog;
    //int currentLine = 0;

    public IEnumerator ShowDialogText(string text, bool waitForInput = true, bool autoClose = true, List<string> choices = null, Action<int> onChoiceSelected = null)
    {
        OnShowDialog?.Invoke();
        IsShowing = true;
        dialogBox.SetActive(true);

        yield return TypeDialog(text);

        if (waitForInput)
        {
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
        }

        if (choices != null && choices.Count > 1)
        {
            yield return choiceBox.ShowChoices(choices, onChoiceSelected);
        }

        if (autoClose)
        {
            CloseDiaLog();
        }

        OnHideDialog?.Invoke();
    }

    public IEnumerator ShowDialog(Dialog dialog, List<string> choices = null, Action<int> onChoiceSelected = null)
    {
        yield return new WaitForEndOfFrame();

        OnShowDialog?.Invoke();
        IsShowing = true;
        //this.dialog = dialog;
        dialogBox.SetActive(true);

        foreach (var line in dialog.Lines)
        {
            yield return TypeDialog(line);
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
        }

        //StartCoroutine(TypeDialog(dialog.Lines[0]));

        if(choices != null && choices.Count >= 1) 
        {
            choiceBox.ShowChoices(choices, onChoiceSelected);
        }

        dialogBox.SetActive(false);
        IsShowing = false;
        OnHideDialog?.Invoke();
    }

    public void CloseDiaLog()
    {
        dialogBox.SetActive(false);
        IsShowing = false;
    }

    public void HandleUpdate()
    {
        //if (Input.GetKeyDown(KeyCode.E))
        //{
        //    ++currentLine;
        //    if (currentLine < dialog.Lines.Count)
        //    {
        //        StartCoroutine(TypeDialog(dialog.Lines[currentLine]));
        //    }
        //    else
        //    {
        //        currentLine = 0;
        //        dialogBox.SetActive(false);
        //        OnHideDialog?.Invoke();
        //    }
        //}
    }

    public IEnumerator TypeDialog(string line)
    {
        dialogText.text = "";

        foreach (var letter in line.ToCharArray()) 
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }
    }
}
