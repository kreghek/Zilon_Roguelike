using System;
using Zilon.Core.Persons;
using Zilon.Core.Tactics.Spatial;

namespace Zilon.Core.Tactics
{
    public class Actor : IActor
    {
        public Actor(IPerson person, HexNode node)
        {
            Person = person;
            Node = node;

            Hp = person.Hp;
            Damage = person.Damage;
        }

        /// <summary>
        /// Песонаж, который лежит в основе актёра.
        /// </summary>
        public IPerson Person { get; }

        /// <summary>
        /// Текущий узел карты, в котором находится актёр.
        /// </summary>
        public HexNode Node { get; set; }

        public float Damage { get; set; }

        public float Hp { get; set; }

        public bool IsDead { get; set; }

        public bool CanBeDamaged()
        {
            return !IsDead;
        }

        public void MoveToNode(HexNode targetNode)
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
