using System;

using Assets.Zilon.Scripts;

using UnityEngine;
using UnityEngine.UI;

using Zilon.Core.PersonDialogs;
using Zilon.Core.Persons;

public class DialogModalBody : MonoBehaviour, IModalWindowHandler
{
    public Text DialogNodeText;
    public Transform DialogTransitionParent;
    public DialogTransitionViewModel DialogTransitionViewModelPrefab;

    private CitizenPerson _questGiver;

    private Dialog _dialog;
    private DialogNode _currentDialogNode;

    public event EventHandler Closed;


    public string Caption { get => "Dialog"; }

    public void Init(CitizenPerson questGiver)
    {
        _questGiver = questGiver ?? throw new ArgumentNullException(nameof(questGiver));
        _dialog = _questGiver.Dialog ?? throw new ArgumentNullException(nameof(questGiver),
            "Не указан диалог для выбранного мирного жителя.");

        _currentDialogNode = _dialog.RootNode;

        UpdateCurrentNode();
    }

    private void SelectTransition(DialogTransition dialogTransition)
    {
        _currentDialogNode = DialogPlayer.SelectNode(_dialog, _currentDialogNode, dialogTransition);
        UpdateCurrentNode();
    }

    private void UpdateCurrentNode()
    {
        DialogNodeText.text = _currentDialogNode.Text;

        foreach (Transform transitionTransform in DialogTransitionParent)
        {
            var transitionViewModel = transitionTransform.GetComponent<DialogTransitionViewModel>();
            transitionViewModel.Clicked -= TransitionViewModel_Clicked;
            RemoveTransitionControl(transitionViewModel);
        }

        var availableTransitions = DialogPlayer.GetAvailableTransitions(_dialog, _currentDialogNode);
        foreach (var transition in availableTransitions)
        {
            CreateTransitionViewModel(transition, DialogTransitionParent);
        }
    }

    private void CreateTransitionViewModel(DialogTransition transition, Transform parentPosition)
    {
        var transitionViewModel = Instantiate(DialogTransitionViewModelPrefab, parentPosition);
        transitionViewModel.Init(transition);
        transitionViewModel.transform.SetParent(DialogTransitionParent);
        transitionViewModel.Clicked += TransitionViewModel_Clicked;
    }

    private void TransitionViewModel_Clicked(object sender, EventArgs e)
    {
        var transitionViewModel = sender as DialogTransitionViewModel;
        SelectTransition(transitionViewModel.DialogTransition);
    }

    private void RemoveTransitionControl(DialogTransitionViewModel transitionViewModel)
    {
        Destroy(transitionViewModel.gameObject);
    }

    public void ApplyChanges()
    {

    }

    public void CancelChanges()
    {

    }
}