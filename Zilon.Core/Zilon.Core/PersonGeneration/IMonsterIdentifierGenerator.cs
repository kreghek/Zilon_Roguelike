namespace Zilon.Core.PersonGeneration
{
    public interface IMonsterIdentifierGenerator
    {
        int GetNewId();
    }

    public sealed class MonsterIdentifierGenerator : IMonsterIdentifierGenerator
    {
        private int _currentIdCounter;

        public int GetNewId()
        {
            _currentIdCounter++;

            return _currentIdCounter;
        }
    }
}