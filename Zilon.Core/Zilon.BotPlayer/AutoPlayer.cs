namespace Zilon.BotPlayer
{
    public class AutoPlayer
    {
        private readonly IBotEnvironment _botEnvironment;

        private IWorldStrategy _currentStrategy;

        public AutoPlayer(IBotEnvironment botEnvironment)
        {
            _botEnvironment = botEnvironment;
        }

        public void Play()
        {
            // Метод полностью проигрывает партию. Делается это через стратерии разных режимов.
            // У стратерий будет вызываться единственный метод, который будет проигрывать
            // партию до момента смены окружения:
            // 1. Смена режима (вход/выход из сектора).
            // 2. Смена сектора. В этом случае стратерию лучше пересоздавать, чтобы был сброшен внутреннее состояние.
            // Условие окончания работы метода - смерть главного персонажа.
        }
    }
}
