using System;

using Assets.Zilon.Scripts;

using UnityEngine;
using UnityEngine.UI;
using Zenject;

using Zilon.Core.World;

public class HistoryModalBody : MonoBehaviour, IModalWindowHandler
{
    public GlobeMinimap Minimap;
    public Text WorldDetailsText;

    [Inject] readonly IWorldManager _worldManager;

    private MinimapState _minimapState;

    public string Caption => "Globe History";

    public event EventHandler Closed;

    public void Init()
    {
        if (_worldManager.Globe != null)
        {
            Minimap.InitMapTextures();
            Minimap.ShowRealms();

            _minimapState = MinimapState.Realms;
        }
    }

    public void SwitchMapButton_Handler()
    {
        switch (_minimapState)
        {
            case MinimapState.Realms:
                _minimapState = MinimapState.Branches;
                Minimap.ShowBranches();
                break;

            case MinimapState.Branches:
                _minimapState = MinimapState.Realms;
                Minimap.ShowRealms();
                break;

            default:
                // В случае сбоя показываем политическую карту мира.
                _minimapState = MinimapState.Realms;
                Minimap.ShowRealms();
                break;
        }
    }

    public void ApplyChanges()
    {

    }

    public void CancelChanges()
    {

    }

    private enum MinimapState
    {
        Unknown = 0,
        Realms,
        Branches
    }
}
