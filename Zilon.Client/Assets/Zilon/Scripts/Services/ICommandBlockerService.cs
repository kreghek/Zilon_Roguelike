namespace Assets.Zilon.Scripts.Services
{
    interface ICommandBlockerService
    {
        void AddBlocker(ICommandBlocker commandBlocker);
        bool HasBlockers { get; }

        void DropBlockers();
    }
}
