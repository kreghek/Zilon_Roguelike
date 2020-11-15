using System;

using Zilon.Core.Commands;
using Zilon.Core.Specs.Contexts;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Specs.Steps
{
    [Binding]
    public class MoveSteps : GenericStepsBase<CommonGameActionsContext>
    {
        protected MoveSteps(CommonGameActionsContext context) : base(context)
        {
        }

        [Then(@"Актёр находится в ячейке \((.*), (.*)\)")]
        public void ThenАктёрНаходитсяВЯчейке(int expectedOffsetX, int expectedOffsetY)
        {
            var actor = Context.GetActiveActor();

            var node = actor.Node;

            var hexNode = (HexNode)node;

            var expectedOffsetCoords = new OffsetCoords(expectedOffsetX, expectedOffsetY);
            var factOffsetCoords = hexNode.OffsetCoords;
            factOffsetCoords.Should().Be(expectedOffsetCoords);
        }

        [Then(@"Команда на перемещение может выполняться")]
        public void ThenКомандаНаПеремещениеМожетВыполняться()
        {
            var moveCommand = Context.ServiceProvider.GetRequiredService<MoveCommand>();

            moveCommand.CanExecute().Should().BeTrue();
        }

        [Then(@"Команда на перемещение не может выполняться")]
        public void ThenКомандаНаПеремещениеНеМожетВыполняться()
        {
            var moveCommand = Context.ServiceProvider.GetRequiredService<MoveCommand>();

            moveCommand.CanExecute().Should().BeFalse();
        }

        [When(@"Выполняется команда на перемещение")]
        public void WhenВыполняетсяКомандаНаПеремещение()
        {
            var moveCommand = Context.ServiceProvider.GetRequiredService<MoveCommand>();
            moveCommand.Execute();
        }

        [When(@"Выполняется команда на перемещение с ошибкой")]
        public void WhenВыполняетсяКомандаНаПеремещениеСОшибкой()
        {
            try
            {
                var moveCommand = Context.ServiceProvider.GetRequiredService<MoveCommand>();
                moveCommand.Execute();
            }
            catch (InvalidOperationException exception)
            {
                Console.WriteLine(exception.ToString());
            }
        }
    }
}