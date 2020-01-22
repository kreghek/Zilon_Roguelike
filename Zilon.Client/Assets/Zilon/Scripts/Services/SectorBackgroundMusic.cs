using System.Linq;

using UnityEngine;

public class SectorBackgroundMusic : MonoBehaviour
{
    public SectorViewModel SectorViewModel;

    private bool _trackPlaying;

    private readonly string[] _pacificSectorSids = new[] {
        "city",
        "forest"
    };

    public AudioSource AudioSource;

    public AudioClip PacificMusic;

    public AudioClip DungeionMusic;

    public void Update()
    {
        if (!SectorViewModel.IsInitialized)
        {
            return;
        }

        if (_trackPlaying)
        {
            return;
        }

        SelectAndStartPlayingMusic();
    }

    public void SelectAndStartPlayingMusic()
    {
        var selectedClip = SelectMusicClipBySectorSid(SectorViewModel.Sector.Scheme?.Sid);
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
