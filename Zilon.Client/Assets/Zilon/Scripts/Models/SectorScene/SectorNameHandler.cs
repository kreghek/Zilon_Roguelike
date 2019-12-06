using System.Linq;

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
            var locationName = _humanPlayer.GlobeNode.Scheme.Name.En;
            string sectorName = null;
            if (_humanPlayer.GlobeNode.Scheme.SectorLevels != null)
            {
                if (_humanPlayer.SectorSid == null)
                {
                    var sector = _humanPlayer
                        .GlobeNode
                        .Scheme
                        .SectorLevels
                        .SingleOrDefault(x => x.IsStart);
                    sectorName = sector.Name?.En;
                }
                else
                {
                    var sector = _humanPlayer
                        .GlobeNode
                        .Scheme
                        .SectorLevels
                        .SingleOrDefault(x => x.Sid == _humanPlayer.SectorSid);
                    sectorName = sector.Name?.En;
                }
            }

            var name = sectorName ?? locationName;
            SectorNameText.text = $"{name}";
        }

        Destroy(this);
    }
}
