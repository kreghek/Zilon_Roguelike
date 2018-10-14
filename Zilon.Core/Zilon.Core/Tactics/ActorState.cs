using System;

namespace Zilon.Core.Tactics
{
    /// <summary>
    /// Базовая реализация состояния актёра.
    /// </summary>
    public class ActorState : IActorState
    {
        public ActorState(int hp)
        {
            Hp = hp;
        }

        public int Hp { get; private set; }

        public bool IsDead { get; private set; }

        public event EventHandler Dead;

        public void SetHpForce(int hp)
        {
            if (hp == 0)
            {
                throw new ArgumentException(nameof(hp));
            }

            Hp = hp;
        }

        public void TakeDamage(int value)
        {
            Hp -= value;

            if (Hp <= 0)
            {
                IsDead = true;
                Dead?.Invoke(this, new EventArgs());
            }
        }

        public void RestoreHp(int value, int max)
        {
            Hp += value;

            if (Hp > max)
            {
                Hp = max;
            }
        }
    }
}
