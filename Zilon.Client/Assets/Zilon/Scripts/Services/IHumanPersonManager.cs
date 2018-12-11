using Zilon.Core.Persons;

namespace Assets.Zilon.Scripts.Services
{
    interface IHumanPersonManager
    {
        HumanPerson Person { get; set; }

        int SectorLevel { get; set; }

        string SectorName { get; set; }
    }
}
