using UnityEngine;

namespace Assets.Zilon.Scripts.Models.SectorScene
{
    interface ICanBeHitSectorObject
    {
        void AddHitEffect(HitSfx sfxObject);

        Vector3 Position { get; }
    }
}
