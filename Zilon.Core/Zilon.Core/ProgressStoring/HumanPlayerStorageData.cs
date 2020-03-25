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

        public void Restore(HumanPlayer humanPlayer)
        {
            
        }
    }
}
