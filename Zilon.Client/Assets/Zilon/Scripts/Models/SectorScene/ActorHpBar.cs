using System.Linq;

using UnityEngine;

using Zilon.Core.Persons;
using Zilon.Core.Tactics;

public class ActorHpBar : MonoBehaviour
{
    public GameObject Bar;
    public GameObject[] Objects;

    public IActor Actor { get; set; }

    public void FixedUpdate()
    {
        if (Actor == null)
        {
            return;
        }


        var hpStat = Actor.Person.Survival?.Stats.SingleOrDefault(x => x.Type == SurvivalStatType.Health);
        if (hpStat == null)
        {
            return;
        }

        if (hpStat.Value == hpStat.Range.Max || hpStat.Value <= 0)
        {
            foreach (var obj in Objects)
            {
                obj.SetActive(false);
            }
        }
        else
        {
            foreach (var obj in Objects)
            {
                obj.SetActive(true);
            }

            var hpPercent = (float)hpStat.Value / hpStat.Range.Max;

            Bar.transform.localScale = new Vector3(hpPercent, 0.1f, 1);
        }
    }
}
