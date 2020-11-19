using System.Linq;

using UnityEngine;

using Zenject;

using Zilon.Core.Players;

public class SectorBackgroundMusic : MonoBehaviour
{
    private bool _trackPlaying;

    private readonly string[] _pacificSectorSids = new[] {
        "city",
        "forest"
    };

    [Inject]
    private readonly IPlayer _player;

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
        var currentSectorSid = _player.SectorNode.Sector.Scheme?.Sid;

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
