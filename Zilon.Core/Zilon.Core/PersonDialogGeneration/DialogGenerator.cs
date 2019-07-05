using Zilon.Core.CommonServices.Dices;

namespace Zilon.Core.PersonDialogs
{
    public sealed class DialogGenerator
    {
        private readonly IDice _dice;

        public DialogGenerator(IDice dice)
        {
            _dice = dice;
        }

        public Dialog Generate()
        {

        }
    }
}
