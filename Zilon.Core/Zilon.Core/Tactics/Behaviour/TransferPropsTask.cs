using System.Collections.Generic;
using System.Linq;

namespace Zilon.Core.Tactics.Behaviour
{
    /// <summary>
    /// Задача на перемещение предметов относительно указанного хранилища.
    /// </summary>
    public class TransferPropsTask : OneTurnActorTaskBase
    {
        private readonly PropTransfer[] _transfers;

        public TransferPropsTask(IActor actor,
            IEnumerable<PropTransfer> transfers) :
            base(actor)
        {
            _transfers = transfers.ToArray();
        }

        protected override void ExecuteTask()
        {
            foreach (var transfer in _transfers)
            {
                var propStore = transfer.PropStore;

                foreach (var prop in transfer.Added)
                {
                    propStore.Add(prop);
                }

                foreach (var prop in transfer.Removed)
                {
                    propStore.Remove(prop);
                }
            }
        }
    }
}
