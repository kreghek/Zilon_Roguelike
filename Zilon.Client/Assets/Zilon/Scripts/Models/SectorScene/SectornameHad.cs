using Assets.Zilon.Scripts.Services;

using JetBrains.Annotations;

using UnityEngine;
using UnityEngine.UI;

using Zenject;

public class SectornameHad : MonoBehaviour
{
    public Text SectorNameText;

    [Inject] [NotNull] private readonly IHumanPersonManager _humanPersonManager;

    public void FixedUpdate()
    {
        if (_humanPersonManager.SectorName != null)
        {
            var name = _humanPersonManager.SectorName;
            var level = _humanPersonManager.SectorLevel + 1;
            SectorNameText.text = $"{name} lvl{level}";

            Destroy(this);
        }
    }
}
