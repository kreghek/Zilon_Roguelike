namespace Zilon.BotPlayer
{
    public class SectorStrategy : IWorldStrategy
    {
        public void Execute()
        {
            // Проигрывает стратегию работы в секторе до выхода из сектора.
            // Далее либо сменяется GlobeStrategy либо новым экземпляром SectorStrategy.
        }
    }
}
