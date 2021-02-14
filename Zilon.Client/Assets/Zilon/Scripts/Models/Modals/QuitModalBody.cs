using System;
using System.Threading.Tasks;

using Assets.Zilon.Scripts;

using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

using Zenject;

using Zilon.Core.Commands;

public class QuitModalBody : MonoBehaviour, IModalWindowHandler
{
    private ICommand _targetCommand;

    public Text QuitPhraseText;

    public LocalizedString QuitPhrasesString;

    [Inject(Id = "quit-command")]
    private readonly ICommand _quitCommand;

    [Inject(Id = "quit-title-command")]
    private readonly ICommand _quitTitleCommand;

    [Inject]
    private readonly ICommandManager _clientCommandExecutor;

    public string Caption { get; private set; }
    public CloseBehaviourOperation CloseBehaviour => CloseBehaviourOperation.DoNothing;

    public event EventHandler Closed;

    public async Task Start()
    {
        var _textsAsyncHandler = QuitPhrasesString.GetLocalizedString();
        string stringRaw;
        if (_textsAsyncHandler.IsDone)
        {
            stringRaw = _textsAsyncHandler.Result;
        }
        else
        {
            stringRaw = await _textsAsyncHandler.Task;
        }

        string[] _texts = stringRaw.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

        var textIndex = UnityEngine.Random.Range(0, _texts.Length - 1);
        QuitPhraseText.text = _texts[textIndex];
    }

    public void Init(string caption, bool closeGame)
    {
        Caption = caption ?? throw new ArgumentNullException(nameof(caption));

        if (closeGame)
        {
            _targetCommand = _quitCommand;
        }
        else
        {
            _targetCommand = _quitTitleCommand;
        }
    }

    public void ApplyChanges()
    {
        _clientCommandExecutor.Push(_targetCommand);
    }

    public void CancelChanges()
    {
        // ничего не делаем
        // просто закрываем окно
        Closed?.Invoke(this, new EventArgs());
    }
}
