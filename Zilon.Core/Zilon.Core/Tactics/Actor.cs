using System;

using Zilon.Core.Persons;
using Zilon.Core.Players;
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
            Damage = person.Damage;

            WeaponDistance = 1;
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

        public int WeaponDistance { get; }

        public IPlayer Owner { get; }

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

        public event EventHandler OnMoved;
        public event EventHandler OnDead;
    }
}
