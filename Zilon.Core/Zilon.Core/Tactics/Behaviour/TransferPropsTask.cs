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

        public TransferPropsTask(
            IActor actor,
            IActorTaskContext context,
            IEnumerable<PropTransfer> transfers) :
            base(actor, context)
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
                    // TODO Здесь может быть ошибка, если два персонажа одновременно начнут брать предметы.
                    // Тогда предмет будет в обоих хранилищах.
                    propStore.Add(prop);
                }

                foreach (var prop in transfer.Removed)
                {
                    if (transfer.PropStore.Has(prop))
                    {
                        propStore.Remove(prop);
                    }
                }
            }
        }
    }
}