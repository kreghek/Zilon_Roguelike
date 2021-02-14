using System;
using System.Threading.Tasks;

using Assets.Zilon.Scripts;
using Assets.Zilon.Scripts.Models.Modals;

using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

using Zenject;

using Zilon.Core.Commands;

public class QuitModalBody : MonoBehaviour, IModalWindowHandler
{
    private ICommand _targetCommand;

    private QuitModalBehaviour _quitModalBehaviour;

    /// <summary>
    /// Text object to display random quit phrase.
    /// </summary>
    public Text QuitPhraseText;

    /// <summary>
    /// Caption to Close game.
    /// </summary>
    public LocalizedString QuitGameCaption;

    /// <summary>
    /// Caption to end game session.
    /// </summary>
    public LocalizedString QuitToTitleMenuCaption;

    /// <summary>
    /// Multiline string with quit phrases.
    /// </summary>
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

    public async void Start()
    {
        await SetQuitCaption();
        await SetQuitPhrase();
    }

    public void Init(QuitModalBehaviour quitModalBehaviour)
    {
        _quitModalBehaviour = quitModalBehaviour;

        switch (quitModalBehaviour)
        {
            case QuitModalBehaviour.QuitGame:
                _targetCommand = _quitCommand;
                break;

            case QuitModalBehaviour.QuitToTitleMenu:
                _targetCommand = _quitTitleCommand;
                break;

            default:
                if (Debug.isDebugBuild || Application.isEditor)
                {
                    throw new InvalidOperationException($"Invalid behaviour for quit modal: {quitModalBehaviour}.");
                }
                break;
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

    private async Task SetQuitCaption()
    {
        var captionLocalizedString = GetCaptionLocalizedStringByBehaviour(_quitModalBehaviour);

        Caption = await ReadStringFromLocalized(captionLocalizedString);
    }

    private LocalizedString GetCaptionLocalizedStringByBehaviour(QuitModalBehaviour quitModalBehaviour)
    {
        switch (quitModalBehaviour)
        {
            case QuitModalBehaviour.QuitToTitleMenu:
                return QuitToTitleMenuCaption;

            case QuitModalBehaviour.QuitGame:
                return QuitGameCaption;

            default:
                if (Debug.isDebugBuild || Application.isEditor)
                {
                    throw new InvalidOperationException($"Invalid behaviour for quit modal: {quitModalBehaviour}.");
                }
                return QuitGameCaption;
        }
    }

    private async Task SetQuitPhrase()
    {
        var phrasesString = await ReadStringFromLocalized(QuitPhrasesString);

        var quitPhrases = phrasesString.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        var rolledPhraseIndex = UnityEngine.Random.Range(0, quitPhrases.Length);

        QuitPhraseText.text = quitPhrases[rolledPhraseIndex];
    }

    private static async Task<string> ReadStringFromLocalized(LocalizedString localizedString)
    {
        var _textsAsyncHandle = localizedString.GetLocalizedString();
        if (_textsAsyncHandle.IsDone)
        {
            return _textsAsyncHandle.Result;
        }

        return await _textsAsyncHandle.Task;
    }
}
