using FluentAssertions;

using LightInject;

using TechTalk.SpecFlow;

using Zilon.Core.Commands;
using Zilon.Core.Spec.Contexts;

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

    }
}
