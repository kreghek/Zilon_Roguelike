using System;

using Zilon.Core.Persons;

namespace Zilon.Core.Tactics
{
    //TODO Пересмотреть необходимость этого типа. Вероятно его можно объединить с ISurvivalData (который станет PersonState).
    /// <summary>
    /// Базовая реализация состояния актёра.
    /// </summary>
    public class ActorState : IActorState
    {
        private readonly IPerson _person;

        public ActorState(IPerson person, int initialHp)
        {
            _person = person;

            Hp = initialHp;
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

        public void RestoreHp(int value)
        {
            Hp += value;

            if (Hp > _person.Hp)
            {
                Hp = _person.Hp;
            }
        }
    }
}
