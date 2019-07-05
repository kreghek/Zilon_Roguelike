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

    public DialogModalBody()
    {

    }

    public string Caption { get => "Trader"; }

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
            var viewModel = transitionTransform.GetComponent<DialogTransitionViewModel>();
            RemoveTransitionControl(viewModel);
        }

        var availableTransitions = DialogPlayer.GetAvailableTransitions(_dialog, _currentDialogNode);
        foreach (var transition in availableTransitions)
        {
            CreateTransitionViewModel(transition);
        }
    }

    private void CreateTransitionViewModel(DialogTransition transition)
    {
        var transitionViewModel = Instantiate(DialogTransitionViewModelPrefab);
        transitionViewModel.Init(transition);
        transitionViewModel.transform.SetParent(DialogTransitionParent);
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