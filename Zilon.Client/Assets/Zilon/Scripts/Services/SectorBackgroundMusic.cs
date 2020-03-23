using System.Linq;

using UnityEngine;

using Zenject;

using Zilon.Core.Tactics;

public class SectorBackgroundMusic : MonoBehaviour
{
    private bool _trackPlaying;

    private readonly string[] _pacificSectorSids = new[] {
        "city",
        "forest"
    };

    [Inject]
    private readonly ISectorManager _sectorManager;

    public AudioSource AudioSource;

    public AudioClip PacificMusic;

    public AudioClip DungeionMusic;

    public void Update()
    {
        if (_trackPlaying)
        {
            return;
        }

        SelectAndStartPlayingMusic();
    }

    public void SelectAndStartPlayingMusic()
    {
        var currentSectorSid = _sectorManager.CurrentSector.Scheme?.Sid;

        var selectedClip = SelectMusicClipBySectorSid(currentSectorSid);
        AudioSource.clip = selectedClip;

        AudioSource.Play();

        _trackPlaying = true;
    }

    private AudioClip SelectMusicClipBySectorSid(string currentSectorSid)
    {
        if (_pacificSectorSids.Contains(currentSectorSid))
        {
            return PacificMusic;
        }
        else
        {
            return DungeionMusic;
        }
    }
}
