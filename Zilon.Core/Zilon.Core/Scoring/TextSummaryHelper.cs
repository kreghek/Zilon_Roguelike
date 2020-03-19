using System.Text;

using JetBrains.Annotations;

using Zilon.Core.Tactics;

namespace Zilon.Core.Scoring
{
    /// <summary>
    /// Вспомогательный класс для работы текстовым представлением итогов игры.
    /// </summary>
    public static class TextSummaryHelper
    {
        /// <summary>
        /// Создать текстовое описание итогов игры.
        /// </summary>
        /// <param name="scores"> Объект, содержащий очки игры. </param>
        /// <param name="botName"> Имя бота, который играл. Не указывать, если выводятся очки игрока-человека. </param>
        /// <returns> Возвращает текстовое представление итогов игры в виде строки. </returns>
        //TODO Вместо botName передавать объект BotInfo. Так будет более очевидно.
        public static string CreateTextSummary([NotNull] Scores scores, [CanBeNull] string botName)
        {
            if (scores is null)
            {
                throw new System.ArgumentNullException(nameof(scores));
            }

            var summaryStringBuilder = new StringBuilder();

            if (botName == null)
            {
                summaryStringBuilder.AppendLine("YOU DIED");
            }
            else
            {
                summaryStringBuilder.AppendLine($"YOU (BOT {botName}) DIED");
            }

            summaryStringBuilder.AppendLine($"SCORES: {scores.BaseScores}");

            summaryStringBuilder.AppendLine("=== You survived ===");

            var lifetime = ScoreCalculator.ConvertTurnsToDetailed(scores.Turns);

            summaryStringBuilder.AppendLine($"{lifetime.Days} days {lifetime.Hours} hours");

            if (botName != null)
            {
                summaryStringBuilder.AppendLine($"Turns: {scores.Turns}");
            }

            summaryStringBuilder.AppendLine("=== You visited ===");

            summaryStringBuilder.AppendLine($"{scores.Places.Count} places");

            foreach (var placeType in scores.PlaceTypes)
            {
                summaryStringBuilder.AppendLine($"{placeType.Key.Name?.En ?? placeType.Key.Name?.Ru ?? placeType.Key.ToString()}: {placeType.Value} turns");
            }

            summaryStringBuilder.AppendLine("=== You killed ===");
            foreach (var frag in scores.Frags)
            {
                summaryStringBuilder.AppendLine($"{frag.Key.Name?.En ?? frag.Key.Name?.Ru ?? frag.Key.ToString()}: {frag.Value}");
            }

            return summaryStringBuilder.ToString();
        }

        /// <summary>
        /// Создать текстовое описание итогов игры.
        /// </summary>
        /// <param name="scores"> Объект, содержащий очки игры. </param>
        /// <param name="botName"> Имя бота, который играл. Не указывать, если выводятся очки игрока-человека. </param>
        /// <returns> Возвращает текстовое представление итогов игры в виде строки. </returns>
        public static string CreateTextSummary([NotNull] Scores scores)
        {
            return CreateTextSummary(scores, botName: null);
        }
    }
}
