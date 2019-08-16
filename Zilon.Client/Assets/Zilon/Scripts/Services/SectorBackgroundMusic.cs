using System.Linq;
using UnityEngine;

using Zenject;

using Zilon.Core.Tactics;

public class SectorBackgroundMusic : MonoBehaviour
{
    private readonly string[] _pacificSectorSids = new[] {
        "city",
        "forest"
    };

    [Inject]
    private readonly ISectorManager _sectorManager;

    public AudioSource AudioSource;

    public AudioClip PacificMusic;

    public AudioClip DungeionMusic;

    public void Awake()
    {
        var currentSectorSid = _sectorManager.CurrentSector.Scheme.Sid;
        if (_pacificSectorSids.Contains(currentSectorSid))
        {
            AudioSource.clip = PacificMusic;
        }
        else
        {
            AudioSource.clip = DungeionMusic;
        }

        AudioSource.Play();
    }
}
