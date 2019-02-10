using Assets.Zilon.Scripts.Services;

using JetBrains.Annotations;

using UnityEngine;
using UnityEngine.UI;

using Zenject;
using Zilon.Core.Players;
using Zilon.Core.Tactics;

public class SectorNameHandler : MonoBehaviour
{
    public Text SectorNameText;

    [Inject] [NotNull] private readonly HumanPlayer _humanPlayer;
    [Inject] [NotNull] private readonly ISectorManager _sectorManager;

    public void FixedUpdate()
    {
        if (_humanPlayer.GlobeNode == null)
        {
            SectorNameText.text = $"Intro";
        }
        else
        {
            var name = _humanPlayer.GlobeNode.Scheme.Name.En;
            var level = _sectorManager.SectorLevel + 1;
            SectorNameText.text = $"{name} lvl{level}";
        }

        Destroy(this);
    }
}
