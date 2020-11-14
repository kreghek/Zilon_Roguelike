using System;
using Zilon.Core.Commands;
using Zilon.Core.Graphs;
using Zilon.Core.Specs.Contexts;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Specs.Steps
{
    [Binding]
    public class MoveSteps : GenericStepsBase<CommonGameActionsContext>
    {
        protected MoveSteps(CommonGameActionsContext context) : base(context)
        {
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

        [Then(@"Актёр находится в ячейке \((.*), (.*)\)")]
        public void ThenАктёрНаходитсяВЯчейке(int expectedOffsetX, int expectedOffsetY)
        {
            IActor actor = Context.GetActiveActor();

            IGraphNode node = actor.Node;

            HexNode hexNode = (HexNode)node;

            OffsetCoords expectedOffsetCoords = new OffsetCoords(expectedOffsetX, expectedOffsetY);
            OffsetCoords factOffsetCoords = hexNode.OffsetCoords;
            factOffsetCoords.Should().Be(expectedOffsetCoords);
        }
    }
}