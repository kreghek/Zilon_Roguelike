namespace Assets.Zilon.Scripts
{
    /// <summary>
    /// Operation invoked then modal closed via cross.
    /// </summary>
    public enum CloseBehaviourOperation
    {
        /// <summary>
        /// Just close discarding changes.
        /// </summary>
        DoNothing,

        /// <summary>
        /// Apply changes (call <see cref="IModalWindowHandler.ApplyChanges"/> method).
        /// </summary>
        ApplyChanges
    }
}
