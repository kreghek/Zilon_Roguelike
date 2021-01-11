using Assets.Zilon.Scripts.Services;

using UnityEngine;

using Zenject;

using Zilon.Core.Client.Windows;

public class InstructionLauncherHandler : MonoBehaviour
{
    [Inject] private readonly ISectorModalManager _sectorModalManager;

    [Inject] private readonly UiSettingService _uiSettingService;

    public void Start()
    {
        if (_uiSettingService.ShowTutorialOnStart)
        {
            _sectorModalManager.ShowInstructionModal();
            _uiSettingService.ShowTutorialOnStart = false;
        }

        Destroy(gameObject);
    }
}
