using Zilon.Core.Diseases;

namespace Zilon.Core.Persons
{
    public interface IDiseaseProcess
    {
        float CurrentPower { get; }
        IDisease Disease { get; }
        float Value { get; }

        void Update();
    }
}