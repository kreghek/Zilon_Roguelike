namespace Zilon.Core.Scoring
{
    /// <summary>
    ///     Вспомогательный класс для объединения всех расчётов очков.
    /// </summary>
    public static class ScoreCalculator
    {
        /// <summary>
        ///     Разбирает ходы на отдельные значения дней, часов и т.д.
        /// </summary>
        /// <param name="turns"> Количество игровых ходов. </param>
        /// <returns> Возвращает отдельный объект, хранящий ходы в отдельных компонентах. </returns>
        public static DetailedLifetime ConvertTurnsToDetailed(int turns)
        {
            var minutesTotal = turns * 2;
            var hoursTotal = minutesTotal / 60f;
            var daysTotal = hoursTotal / 24f;

            var days = (int)daysTotal;

            // Замечание от lgtm
            // https://lgtm.com/rules/1506096756023/
            // Оба числа в операции daysCuttedFloat - float. Не видно причин, почему это плохо.
            // Зато анализатор не беспокоится.
            // Здесь не может быть переполнения, потому что все эти числа получены из turns,
            // который является int. И дни, и часы являются результатом деления целочисленного turns.

            var daysCuttedFloat = (float)days * 24;
            var hours = (int)(hoursTotal - daysCuttedFloat);

            return new DetailedLifetime(days, hours);
        }
    }
}