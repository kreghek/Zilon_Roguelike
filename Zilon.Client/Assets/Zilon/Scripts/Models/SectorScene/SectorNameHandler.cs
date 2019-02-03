using Assets.Zilon.Scripts.Services;

using JetBrains.Annotations;

using UnityEngine;
using UnityEngine.UI;

using Zenject;
using Zilon.Core.Players;

public class SectorNameHandler : MonoBehaviour
{
    public Text SectorNameText;

    [Inject] [NotNull] private readonly IHumanPersonManager _humanPersonManager;
    [Inject] [NotNull] private readonly HumanPlayer _humanPlayer;

    public void FixedUpdate()
    {
        if (_humanPlayer.GlobeNode == null)
        {
            SectorNameText.text = $"Intro";
        }
        else
        {
            var name = _humanPlayer.GlobeNode.Scheme.Name.En;
            var level = _humanPersonManager.SectorLevel + 1;
            SectorNameText.text = $"{name} lvl{level}";
        }

        Destroy(this);
    }
}
