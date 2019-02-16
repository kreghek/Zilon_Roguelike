using System;

using Assets.Zilon.Scripts;

using JetBrains.Annotations;

using UnityEngine;

using Zenject;

using Zilon.Core.Commands;

public class QuitModalBody : MonoBehaviour, IModalWindowHandler
{
    [Inject(Id = "quit-command")] private ICommand _command;

    [NotNull] [Inject] private readonly ICommandManager _clientCommandExecutor;

    public string Caption => "Quit";

    public event EventHandler Closed;

    public void ApplyChanges()
    {
        _clientCommandExecutor.Push(_command);
    }

    public void CancelChanges()
    {
        // ничего не делаем
        // просто закрываем окно
        Closed?.Invoke(this, new EventArgs());
    }
}
