using Zilon.Core.Players;

namespace Zilon.Core.ProgressStoring
{
    public sealed class HumanPlayerStorageData
    {
        public static HumanPlayerStorageData Create(HumanPlayer humanPlayer)
        {
            if (humanPlayer is null)
            {
                throw new System.ArgumentNullException(nameof(humanPlayer));
            }

            var storageData = new HumanPlayerStorageData();
            return storageData;
        }

#pragma warning disable CA1801 // Review unused parameters
        public static void Restore(HumanPlayer humanPlayer)
#pragma warning restore CA1801 // Review unused parameters
        {
            // Здесь нужно восстанавливать позиуию игрока в графе узлов биома.
            // Еще не успел это сделать. Вернуться, когда будет готово сохранение биомов.
        }
    }
}