using JetBrains.Annotations;

using UnityEngine;
using UnityEngine.UI;

using Zenject;

using Zilon.Core.Tactics;

public class SectorNameHandler : MonoBehaviour
{
    public Text SectorNameText;

    [Inject] [NotNull] private readonly ISectorManager _sectorManager;

    public void FixedUpdate()
    {
        if (_sectorManager.CurrentSector != null)
        {
            SectorNameText.text = _sectorManager.CurrentSector.Scheme.Name.En;

            Destroy(this);
        }
    }
}
