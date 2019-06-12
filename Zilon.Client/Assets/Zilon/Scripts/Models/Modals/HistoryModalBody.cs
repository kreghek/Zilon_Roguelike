using System;
using System.Linq;
using Assets.Zilon.Scripts;

using UnityEngine;
using UnityEngine.UI;
using Zenject;

using Zilon.Core.World;
using Zilon.Core.WorldGeneration;

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

            var details = WriteWorldDetails(_worldManager.Globe);
            WorldDetailsText.text = details;
        }
    }

    private static string WriteWorldDetails(Globe globe)
    {
        var detailsText = string.Empty;

        // Выводим информацию по всем государствам.
        // Выводим:
        // 1. Название государства.
        // 2. Первого попавшенося самого влиятельного агента.
        // Потенциально, это будут главы государств.
        // 3. Выводим наименование персого самого населённого города.
        // Потенциально это будут столицы.


        var mostPowerfullAgents = globe.Agents.OrderBy(x => x.Skills.Sum(s => s.Value)).ToArray();
        var localities = globe.Localities.OrderBy(x => x.Population).ToArray();
        foreach (var realm in globe.Realms)
        {
            // Наименование.
            detailsText += $"=== {realm.Name} ===" + Environment.NewLine;

            // Агент-представитель.
            var mostPowerfullAgent = mostPowerfullAgents.FirstOrDefault(x=>x.Realm == realm);
            if (mostPowerfullAgent != null)
            {
                detailsText += $"The most influential citizen: {mostPowerfullAgent.Name}" + Environment.NewLine;
            }

            // Столица.
            var biggestCity = localities.FirstOrDefault(x => x.Owner == realm);
            if (biggestCity != null)
            {
                detailsText += $"The capital: {biggestCity.Name}" + Environment.NewLine;
            }

            detailsText += Environment.NewLine;
        }

        return detailsText;
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
