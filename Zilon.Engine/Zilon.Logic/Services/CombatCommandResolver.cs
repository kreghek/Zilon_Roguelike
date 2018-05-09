using System;
using System.Collections.Generic;
using System.Linq;
using Zilon.Logic.PathFinding;
using Zilon.Logic.Tactics;
using Zilon.Logic.Tactics.Events;
using Zilon.Logic.Tactics.Map;

namespace Zilon.Logic.Services
{
    public class CombatCommandResolver : ICombatCommandResolver
    {
        public const int MoveCost = 1;  //TODO Задавать в схеме персонажа

        public CommandResult MoveSquad(Combat combat, ActorSquad actorSquad, MapNode targetNode)
        {
            var groupIndex = 0;

            var moveEvents = GetMoveToPointEvents(combat, actorSquad, targetNode, actorSquad.MP, ref groupIndex);

            // Путь не найден
            if (moveEvents == null)
            {
                return new CommandResult
                {
                    Type = CommandResultType.PathNotFound,
                    Errors = new[] { "Путь не найден." }
                };
            }

            // Изменяем состояние хода взвода
            actorSquad.TurnState = SquadTurnState.Acted;

            return new CommandResult
            {
                Type = CommandResultType.Complete,
                Events = moveEvents
            };
        }

        private ICommandEvent[] GetMoveToPointEvents(Combat combat, ActorSquad actorSquad, MapNode targetNode, int availableMP, ref int groupIndex)
        {

            var pathFindingContext = new PathFindingContext();

            var path = FindPath(pathFindingContext, actorSquad.Node, targetNode, availableMP);
            //TODO Вероятно, тут нужно возвращать события Путь не найден
            if (path == null)
                return null;

            var moveEvents = new List<ICommandEvent>();

            var pathList = path.ToArray();
            var i = 1;
            while (actorSquad.MP > 0 && i < pathList.Length && availableMP > 0)
            {
                var oneMoveNode = pathList[i];

                var moveResult = MoveOne(combat, actorSquad, oneMoveNode);
                availableMP--;

                if (moveResult.Type != CommandResultType.Complete)
                    break;

                var groupName = (string)null;
                var groupTriggers = (TargetTriggerGroup[])null;

                if (i == 1 && groupIndex == 0)
                {
                    groupTriggers = new[] {
                        new TargetTriggerGroup {
                            Triggers = new[] { $"group-{groupIndex + 1}" }
                        }
                    };
                }
                else
                {
                    groupName = $"group-{groupIndex}";
                    groupTriggers = new[] {
                        new TargetTriggerGroup {
                            Triggers = new[] { $"group-{groupIndex + 1}" }
                        }
                    };
                }
                var moveGroup = new EventGroup(groupName, groupTriggers, moveResult.Events);
                groupIndex++;
                i++;

                moveEvents.Add(moveGroup);
            }


            return moveEvents.ToArray();
        }

        private CommandResult MoveOne(Combat combat, ActorSquad actorSquad, MapNode targetNode)
        {
            // Проверяем наличие MP - 1MP за клетку
            if (actorSquad.MP < MoveCost)
            {
                return new CommandResult
                {
                    Type = CommandResultType.NotEnoughMP
                };
            }

            // Перемещение

            // Проверяем доступность клетки
            if (!combat.Map.IsPositionAvailableFor(targetNode, actorSquad))
            {
                return new CommandResult
                {
                    Type = CommandResultType.NoFreeSpace
                };
            }

            // Разворачиваем лицом к точке перемещения

            var oldLookAt = actorSquad.LookAt;
            actorSquad.LookAt = null;

            // Выполняем перемещение
            combat.Map.ReleaseNode(actorSquad.Node, actorSquad);
            var squadMovedEvents = ResolveSquadMovement(targetNode, actorSquad);
            
            combat.Map.HoldNode(actorSquad.Node, actorSquad);
            actorSquad.MP--;

            var isHumanPlayerSquad = true;

            if (isHumanPlayerSquad)
            {
                return new CommandResult
                {
                    Type = CommandResultType.Complete,
                    Events = squadMovedEvents
                };
            }
            else
            {
                return new CommandResult
                {
                    Type = CommandResultType.Complete,
                    Events = squadMovedEvents
                };
            }
        }

        private static ICommandEvent[] ResolveSquadMovement(MapNode targetNode, ActorSquad actorSquad)
        {
            var oldNode = actorSquad.Node;
            actorSquad.SetCurrentNode(targetNode);

            // Реализовать перемещние актёров
            var moveSquadEvent = new SquadMovedEvent(null, null, actorSquad.Id, oldNode.Id, targetNode.Id);
            return new ICommandEvent[] { moveSquadEvent };
        }

        private MapNode[] FindPath(PathFindingContext pathFindingContext, MapNode node, MapNode targetNode, int availableMP)
        {
            return new [] { node, targetNode };
        }
    }
}
