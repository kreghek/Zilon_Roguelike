using System;

using Zilon.Core.Persons;
using Zilon.Core.Players;
using Zilon.Core.Tactics.Behaviour;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics
{
    public class Actor : IActor
    {
        public Actor(IPerson person, IPlayer owner, IMapNode node)
        {
            Person = person;
            Owner = owner;
            Node = node;

            Hp = person.Hp;
            Initiative = 1;
        }

        /// <inheritdoc />
        /// <summary>
        /// Песонаж, который лежит в основе актёра.
        /// </summary>
        public IPerson Person { get; }

        /// <inheritdoc />
        /// <summary>
        /// Текущий узел карты, в котором находится актёр.
        /// </summary>
        public IMapNode Node { get; set; }

        public float Damage { get; set; }

        public float Hp { get; set; }

        public bool IsDead { get; set; }

        public IPlayer Owner { get; }

        public float Initiative { get; }

        public bool CanBeDamaged()
        {
            return !IsDead;
        }

        public void MoveToNode(IMapNode targetNode)
        {
            Node = targetNode;
            OnMoved?.Invoke(this, new EventArgs());
        }

        public void TakeDamage(float value)
        {
            Hp -= value;

            if (Hp <= 0)
            {
                IsDead = true;
                OnDead?.Invoke(this, new EventArgs());
            }
        }

        public void OpenContainer(IPropContainer container, IOpenContainerMethod method)
        {
            var openResult = method.TryOpen(container);

            DoOpenContainer(openResult);
        }

        private void DoOpenContainer(IOpenContainerResult openResult)
        {
            var e = new OpenContainerEventArgs(openResult);
            OpenedContainer?.Invoke(this, e);
        }

        public ITacticalAct Acts { get; }

        public event EventHandler OnMoved;
        public event EventHandler OnDead;
        public event EventHandler<OpenContainerEventArgs> OpenedContainer;

        public override string ToString()
        {
            return $"{Person}";
        }
    }
}
