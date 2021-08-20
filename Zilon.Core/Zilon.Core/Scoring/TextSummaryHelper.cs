using System;
using System.Linq;
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
        /// <param name="botName"> Имя бота, который играл. Не указывать, если выводятся очки игрока-человека. </param>
        /// <returns> Возвращает текстовое представление итогов игры в виде строки. </returns>
        //TODO Вместо botName передавать объект BotInfo. Так будет более очевидно.
        public static string CreateTextSummary(Scores scores, string? botName, string lang)
        {
            var summaryStringBuilder = new StringBuilder();

            if (botName is null)
            {
                if (string.Equals(lang, "en", StringComparison.InvariantCultureIgnoreCase))
                {
                    summaryStringBuilder.AppendLine("YOU DIED");
                }
                else if (string.Equals(lang, "ru", StringComparison.InvariantCultureIgnoreCase))
                {
                    summaryStringBuilder.AppendLine("ВЫ МЕРТВЫ");
                }
            }
            else
            {
                if (string.Equals(lang, "en", StringComparison.InvariantCultureIgnoreCase))
                {
                    summaryStringBuilder.AppendLine($"YOU (BOT {botName}) DIED");
                }
                else if (string.Equals(lang, "ru", StringComparison.InvariantCultureIgnoreCase))
                {
                    summaryStringBuilder.AppendLine($"ВЫ (БОТ {botName}) МЕРТВЫ");
                }
            }

            if (string.Equals(lang, "en", StringComparison.InvariantCultureIgnoreCase))
            {
                summaryStringBuilder.AppendLine($"SCORES: {scores.BaseScores}");

                summaryStringBuilder.AppendLine("=== You survived ===");
            }
            else if (string.Equals(lang, "ru", StringComparison.InvariantCultureIgnoreCase))
            {
                summaryStringBuilder.AppendLine($"ОЧКИ: {scores.BaseScores}");

                summaryStringBuilder.AppendLine("=== Вы прожили ===");
            }

            var lifetime = ScoreCalculator.ConvertTurnsToDetailed(scores.Turns);

            if (string.Equals(lang, "en", StringComparison.InvariantCultureIgnoreCase))
            {
                summaryStringBuilder.AppendLine($"{lifetime.Days} days {lifetime.Hours} hours");
            }
            else if (string.Equals(lang, "ru", StringComparison.InvariantCultureIgnoreCase))
            {
                summaryStringBuilder.AppendLine($"{lifetime.Days} дней {lifetime.Hours} часов");
            }

            if (botName != null)
            {
                summaryStringBuilder.AppendLine($"Turns: {scores.Turns}");
            }

            //summaryStringBuilder.AppendLine("=== You visited ===");

            //foreach (var placeType in scores.PlaceTypes)
            //{
            //    summaryStringBuilder.AppendLine(
            //        $"{placeType.Key.Name?.En ?? placeType.Key.Name?.Ru ?? placeType.Key.ToString()}: {placeType.Value} turns");
            //}

            if (scores.Diseases.Any())
            {
                if (string.Equals(lang, "en", StringComparison.InvariantCultureIgnoreCase))
                {
                    summaryStringBuilder.AppendLine("=== Infections ===");

                    foreach (var disease in scores.Diseases)
                    {
                        var name =
                            $"{disease.Name.Secondary?.En} {disease.Name.PrimaryPrefix?.En}{disease.Name.Primary?.En} {disease.Name.Subject?.En}";
                        summaryStringBuilder.AppendLine(name);
                    }
                }
                else if (string.Equals(lang, "ru", StringComparison.InvariantCultureIgnoreCase))
                {
                    summaryStringBuilder.AppendLine("=== Инфекции ===");

                    foreach (var disease in scores.Diseases)
                    {
                        var name =
                            $"{disease.Name.Secondary?.Ru} {disease.Name.PrimaryPrefix?.Ru}{disease.Name.Primary?.Ru} {disease.Name.Subject?.Ru}";
                        summaryStringBuilder.AppendLine(name);
                    }
                }
            }

            if (string.Equals(lang, "en", StringComparison.InvariantCultureIgnoreCase))
            {
                summaryStringBuilder.AppendLine("=== You killed ===");
                foreach (var frag in scores.Frags)
                {
                    summaryStringBuilder.AppendLine(
                        $"{frag.Key.Name?.En ?? frag.Key.ToString()}: {frag.Value}");
                }
            }
            else if (string.Equals(lang, "ru", StringComparison.InvariantCultureIgnoreCase))
            {
                summaryStringBuilder.AppendLine("=== Вы убили ===");
                foreach (var frag in scores.Frags)
                {
                    summaryStringBuilder.AppendLine(
                        $"{frag.Key.Name?.Ru ?? frag.Key.ToString()}: {frag.Value}");
                }
            }

            return summaryStringBuilder.ToString();
        }

        /// <summary>
        /// Создать текстовое описание итогов игры.
        /// </summary>
        /// <param name="scores"> Объект, содержащий очки игры. </param>
        /// <param name="botName"> Имя бота, который играл. Не указывать, если выводятся очки игрока-человека. </param>
        /// <returns> Возвращает текстовое представление итогов игры в виде строки. </returns>
        public static string CreateTextSummary(Scores scores, string lang)
        {
            return CreateTextSummary(scores, null, lang);
        }
    }
}