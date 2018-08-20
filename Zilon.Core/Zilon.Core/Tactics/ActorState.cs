using System;

namespace Zilon.Core.Tactics
{
    /// <summary>
    /// Базовая реализация состояния актёра.
    /// </summary>
    public class ActorState : IActorState
    {
        public ActorState(float hp)
        {
            Hp = hp;

            Initiative = 1;
        }

        public float Hp { get; private set; }

        public bool IsDead { get; private set; }

        public float Initiative { get; }

        public event EventHandler Dead;

        public void TakeDamage(float value)
        {
            Hp -= value;

            if (Hp <= 0)
            {
                IsDead = true;
                Dead?.Invoke(this, new EventArgs());
            }
        }
    }
}
