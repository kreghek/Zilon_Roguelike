using System;
using FluentAssertions;

using LightInject;

using TechTalk.SpecFlow;

using Zilon.Core.Commands;
using Zilon.Core.Spec.Contexts;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Spec.Steps
{
    [Binding]
    public class MoveSteps: GenericStepsBase<CommonGameActionsContext>
    {
        protected MoveSteps(CommonGameActionsContext context) : base(context)
        {
        }

        [Then(@"Команда на перемещение может выполняться")]
        public void ThenКомандаНаПеремещениеМожетВыполняться()
        {
            var moveCommand = Context.Container.GetInstance<ICommand>("move");

            moveCommand.CanExecute().Should().BeTrue();
        }


        [Then(@"Команда на перемещение не может выполняться")]
        public void ThenКомандаНаПеремещениеНеМожетВыполняться()
        {
            var moveCommand = Context.Container.GetInstance<ICommand>("move");

            moveCommand.CanExecute().Should().BeFalse();
        }

        [When(@"Выполняется команда на перемещение")]
        public void WhenВыполняетсяКомандаНаПеремещение()
        {
            var moveCommand = Context.Container.GetInstance<ICommand>("move");
            moveCommand.Execute();
        }

        [When(@"Выполняется команда на перемещение с ошибкой")]
        public void WhenВыполняетсяКомандаНаПеремещениеСОшибкой()
        {
            try
            {
                var moveCommand = Context.Container.GetInstance<ICommand>("move");
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
            var actor = Context.GetActiveActor();

            var node = actor.Node;

            var hexNode = (HexNode)node;

            var expectedOffsetCoords = new OffsetCoords(expectedOffsetX, expectedOffsetY);
            var factOffsetCoords = new OffsetCoords(hexNode.OffsetX, hexNode.OffsetY);
            factOffsetCoords.Should().Be(expectedOffsetCoords);
        }

    }
}
