﻿using System;

using Assets.Zilon.Scripts;

using JetBrains.Annotations;

using UnityEngine;
using UnityEngine.UI;

using Zenject;

using Zilon.Core.Commands;

public class QuitModalBody : MonoBehaviour, IModalWindowHandler
{
    public Text Text;

    private string[] _texts = new[] {
        "Give up and go out?",
        "Is this fate too cruel to continue?",
        "Easier to run than fight for your destiny?",
        "Are you sure you want to quit this great game?",
        "You're trying to say you like work better than me, right?",
        "Just leave. When you come back, I'll be waiting with a bat."
    };

    [Inject(Id = "quit-command")] private ICommand _command;

    [NotNull] [Inject] private readonly ICommandManager _clientCommandExecutor;

    public string Caption => "Quit";

    public event EventHandler Closed;

    public void Init()
    {
        var textIndex = UnityEngine.Random.Range(0, _texts.Length - 1);
        Text.text = _texts[textIndex];
    }

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
