using System.Linq;

using Zilon.Core.Props;

namespace Zilon.Core.StaticObjectModules
{
    public sealed class DepositContainer : ChestBase
    {
        public DepositContainer() : base(new ChestStore())
        {
            Content.Removed += Content_Removed;

            IsActive = false;
        }

        private void Content_Removed(object sender, PropStoreEventArgs e)
        {
            if (!Content.CalcActualItems().Any())
            {
                IsActive = false;
            }
        }

        public override bool IsMapBlock { get => true; }
    }
}