using System.Text;

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
        /// <returns> Возвращает текстовое представление итогов игры в виде строки. </returns>
        public static string CreateTextSummary(Scores scores)
        {
            var summaryStringBuilder = new StringBuilder();

            summaryStringBuilder.AppendLine("YOU (BOT) DIED");

            summaryStringBuilder.AppendLine($"SCORES: {scores.BaseScores}");

            summaryStringBuilder.AppendLine("=== You survived ===");
            var minutesTotal = scores.Turns * 2;
            var hoursTotal = minutesTotal / 60f;
            var daysTotal = hoursTotal / 24f;
            var days = (int)daysTotal;
            var hours = (int)(hoursTotal - days * 24);

            summaryStringBuilder.AppendLine($"{days} days {hours} hours");
            summaryStringBuilder.AppendLine($"Turns: {scores.Turns}");

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
    }
}
