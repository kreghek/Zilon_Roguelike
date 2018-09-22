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
        }

        public float Hp { get; private set; }

        public bool IsDead { get; private set; }

        public event EventHandler Dead;

        public void SetHpForce(float hp)
        {
            if (hp == 0)
            {
                throw new ArgumentException(nameof(hp));
            }

            Hp = hp;
        }

        public void TakeDamage(float value)
        {
            Hp -= value;

            if (Hp <= 0)
            {
                IsDead = true;
                Dead?.Invoke(this, new EventArgs());
            }
        }

        public void RestoreHp(float value, float max)
        {
            Hp += value;

            if (Hp > max)
            {
                Hp = max;
            }
        }
    }
}
