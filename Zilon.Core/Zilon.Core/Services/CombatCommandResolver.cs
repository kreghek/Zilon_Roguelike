namespace Zilon.Core.Services
{
    using System.Collections.Generic;
    using System.Linq;

    using Zilon.Core.PathFinding;
    using Zilon.Core.Tactics;
    using Zilon.Core.Tactics.Events;
    using Zilon.Core.Tactics.Map;

    //TODO Этот класс разделить тоже по паттерну команды
    public class CombatCommandResolver : ICombatCommandResolver
    {
        public const int MOVE_COST = 1;  //TODO Задавать в схеме персонажа

        public CommandResult MoveSquad(Combat combat, Actor actor, MapNode targetNode)
        {
            var groupIndex = 0;

            var moveEvents = GetMoveToPointEvents(combat, actor, targetNode, ref groupIndex);

            // Путь не найден
            if (moveEvents == null)
            {
                return new CommandResult
                {
                    Type = CommandResultType.PathNotFound,
                    Errors = new[] { "Путь не найден." }
                };
            }

            return new CommandResult
            {
                Type = CommandResultType.Complete,
                Events = moveEvents
            };
        }

        private ITacticEvent[] GetMoveToPointEvents(Combat combat, Actor actorSquad, MapNode targetNode, ref int groupIndex)
        {

            var pathFindingContext = new PathFindingContext();

            var path = FindPath(pathFindingContext, actorSquad.Node, targetNode);
            //TODO Вероятно, тут нужно возвращать события Путь не найден
            if (path == null)
                return null;

            var moveEvents = new List<ITacticEvent>();

            var pathList = path.ToArray();
            var i = 1;
            if (i < pathList.Length)
            {
                var oneMoveNode = pathList[i];

                var moveResult = MoveOne(combat, actorSquad, oneMoveNode);

                if (moveResult.Type != CommandResultType.Complete)
                    return moveEvents.ToArray();

                var groupName = (string)null;
                TargetTriggerGroup[] groupTriggers;

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

        private CommandResult MoveOne(Combat combat, Actor actor, MapNode targetNode)
        {
            // Перемещение

            // Проверяем доступность клетки
            if (!combat.Map.IsPositionAvailableFor(targetNode, actor))
            {
                return new CommandResult
                {
                    Type = CommandResultType.NoFreeSpace
                };
            }

            // Выполняем перемещение
            combat.Map.ReleaseNode(actor.Node, actor);
            var squadMovedEvents = ResolveSquadMovement(targetNode, actor);
            
            combat.Map.HoldNode(actor.Node, actor);

            //TODO Доработать определение, ясвляется ли данный актёр управляемым игроком.
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

        private static ITacticEvent[] ResolveSquadMovement(MapNode targetNode, Actor actor)
        {
            var oldNode = actor.Node;
            actor.Node = targetNode;

            // Реализовать перемещние актёров
            var moveSquadEvent = new SquadMovedEvent(null, null, actor.Person.Id, oldNode.Id, targetNode.Id);
            return new ITacticEvent[] { moveSquadEvent };
        }

        private MapNode[] FindPath(PathFindingContext pathFindingContext, MapNode node, MapNode targetNode)
        {
            return new [] { node, targetNode };
        }
    }
}
