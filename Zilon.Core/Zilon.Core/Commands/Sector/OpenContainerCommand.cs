using System;
using System.Diagnostics.CodeAnalysis;

using Zilon.Core.Client;
using Zilon.Core.Tactics;
using Zilon.Core.Tactics.Behaviour;

namespace Zilon.Core.Commands
{
    /// <summary>
    /// Команда открытие контейнера.
    /// </summary>
    public class OpenContainerCommand : ActorCommandBase
    {
        [ExcludeFromCodeCoverage]
        public OpenContainerCommand() :
            base()
        {
        }

        public override bool CanExecute(SectorCommandContext context)
        {
            var map = context.CurrentSector.Map;

            var currentNode = context.ActiveActor.Actor.Node;

            var targetContainerViewModel = GetSelectedNodeViewModel(context);
            if (targetContainerViewModel == null)
            {
                return false;
            }

            var container = targetContainerViewModel.Container;
            var requiredDistance = 1;

            var targetNode = container.Node;

            var distance = map.DistanceBetween(currentNode, targetNode);
            if (distance > requiredDistance)
            {
                return false;
            }

            var containerIsOnLine = map.TargetIsOnLine(currentNode, targetNode);
            if (!containerIsOnLine)
            {
                return false;
            }

            return true;
        }

        protected override void ExecuteTacticCommand(SectorCommandContext context)
        {
            var targetContainerViewModel = GetSelectedNodeViewModel(context);
            if (targetContainerViewModel == null)
            {
                throw new InvalidOperationException("Невозможно выполнить команду. Целевой контейнер не выбран.");
            }

            var container = targetContainerViewModel.Container;
            if (container == null)
            {
                throw new InvalidOperationException("Невозможно выполнить команду. Целевая модель представления не содержит ссылки на контейнер.");
            }

            var intetion = new Intention<OpenContainerTask>(actor => CreateTask(actor, container, context));
            context.TaskSource.Intent(intetion);
        }

        private OpenContainerTask CreateTask(IActor actor, IPropContainer container, SectorCommandContext context)
        {
            var openMethod = new HandOpenContainerMethod();
            return new OpenContainerTask(actor, container, openMethod, context.CurrentSector.Map);
        }

        private IContainerViewModel GetSelectedNodeViewModel(SectorCommandContext context)
        {
            return context.HoverViewModel as IContainerViewModel;
        }
    }
}