using Assets.Zilon.Scripts;

using UnityEngine;
using UnityEngine.UI;

public class InstructionModalBody : MonoBehaviour, IModalWindowHandler
{
    private int _pageCounter;

    public GameObject[] Pages;
    public Button NextButton;
    public Button PrevButton;

    public void Start()
    {
        _pageCounter = 0;
        ShowPage();
    }

    public void NextButton_Handler()
    {
        _pageCounter++;

        if (_pageCounter >= Pages.Length)
        {
            _pageCounter = Pages.Length - 1;
        }

        ShowPage();
    }

    public void PrevButton_Handler()
    {
        _pageCounter--;

        if (_pageCounter < 0)
        {
            _pageCounter = 0;
        }

        ShowPage();
    }

    public void ApplyChanges()
    {

    }

    public void CancelChanges()
    {
        throw new System.NotImplementedException();
    }

    private void ShowPage()
    {
        for (int i = 0; i < Pages.Length; i++)
        {
            var page = Pages[i];

            page.SetActive(i == _pageCounter);
        }
    }
}
