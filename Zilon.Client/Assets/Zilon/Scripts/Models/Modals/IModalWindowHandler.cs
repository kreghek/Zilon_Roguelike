namespace Assets.Zilon.Scripts
{
    public interface IModalWindowHandler
    {
        string Caption { get; }
        void ApplyChanges();
        void CancelChanges();
    }
}
