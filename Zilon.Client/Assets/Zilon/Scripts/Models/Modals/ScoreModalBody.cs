using System;

using Assets.Zilon.Scripts;
using Assets.Zilon.Scripts.Common;
using Assets.Zilon.Scripts.Services;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using Zenject;

using Zilon.Core.Players;
using Zilon.Core.ScoreResultGenerating;
using Zilon.Core.Scoring;

public class ScoreModalBody : MonoBehaviour, IModalWindowHandler
{
    public Text TotalScoreText;

    public Text DetailsText;

    public InputField NameInput;

    [Inject]
    private readonly IScoreManager _scoreManager;

    [Inject]
    private readonly ScoreStorage _scoreStorage;

    [Inject]
    private readonly DeathReasonService _deathReasonService;

    [Inject]
    private readonly IPlayerEventLogService _playerEventLogService;

    [Inject]
    private readonly IPlayer _player;

    [Inject]
    private readonly GlobeStorage _globeStorage;

    public string Caption => "Scores";

    public event EventHandler Closed;

    public void Init()
    {
        NameInput.text = "Безымянный бродяга";

        // TODO Сделать анимацию - плавное накручивание очков через Lerp от инта
        TotalScoreText.text = _scoreManager.BaseScores.ToString();

        var lastPlayerEvent = _playerEventLogService.GetPlayerEvent();
        var deathReason = _deathReasonService.GetDeathReasonSummary(lastPlayerEvent, Zilon.Core.Localization.Language.Ru);

        DetailsText.text = "Причина смерти:" + deathReason + "\n" + TextSummaryHelper.CreateTextSummary(_scoreManager.Scores);
    }

    public void ApplyChanges()
    {
        var name = NameInput.text;

        var scores = _scoreManager.Scores;
        try
        {
            var lastPlayerEvent = _playerEventLogService.GetPlayerEvent();
            var deathReason = _deathReasonService.GetDeathReasonSummary(lastPlayerEvent, Zilon.Core.Localization.Language.Ru);

            _scoreStorage.AppendScores(name, scores, deathReason);

            GameCleanupHelper.ResetState(_player, _globeStorage);
        }
        catch (Exception exception)
        {
            Debug.LogError("Не удалось выполнить запись результатов в БД\n" + exception.ToString());
        }

        SceneManager.LoadScene("scores");
    }

    public void CancelChanges()
    {
        throw new NotSupportedException();
    }
}
