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
            var moveCommand = _context.Container.GetInstance<ICommand>("move");

            moveCommand.CanExecute().Should().BeTrue();
        }


        [Then(@"Команда на перемещение не может выполняться")]
        public void ThenКомандаНаПеремещениеНеМожетВыполняться()
        {
            var moveCommand = _context.Container.GetInstance<ICommand>("move");

            moveCommand.CanExecute().Should().BeFalse();
        }

        [When(@"Выполняется команда на перемещение")]
        public void WhenВыполняетсяКомандаНаПеремещение()
        {
            var moveCommand = _context.Container.GetInstance<ICommand>("move");
            moveCommand.Execute();
        }

        [Then(@"Актёр находится в ячейке \((.*), (.*)\)")]
        public void ThenАктёрНаходитсяВЯчейке(int expectedOffsetX, int expectedOffsetY)
        {
            var actor = _context.GetActiveActor();

            var node = actor.Node;

            var hexNode = (HexNode)node;

            hexNode.OffsetX.Should().Be(expectedOffsetX);
            hexNode.OffsetY.Should().Be(expectedOffsetY);
        }

    }
}
