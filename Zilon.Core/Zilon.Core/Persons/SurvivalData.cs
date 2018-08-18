namespace Zilon.Core.Persons
{
    /// <summary>
    /// Базовая реализация данных о выживании.
    /// </summary>
    public class SurvivalData : ISurvivalData
    {
        public SurvivalData()
        {
            Satiety = 50;
            Thirst = 50;
        }

        public int Satiety { get; private set; }

        public int Thirst { get; private set; }

        public void ReplenishSatiety(int value)
        {
            Satiety += value;
            if (Satiety >= 100)
            {
                Satiety = 100;
            }
        }

        public void ReplenishThirst(int value)
        {
            Thirst += value;
            if (Thirst >= 100)
            {
                Thirst = 100;
            }
        }

        public void Update()
        {
            Satiety--;
            Thirst--;
        }
    }
}
