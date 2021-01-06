namespace Zilon.Core.PersonGeneration
{
    /// <summary>
    /// Provider for monster indentifier to select monster without graphic clients. Used by tests and the text game.
    /// </summary>
    public interface IMonsterIdentifierGenerator
    {
        /// <summary>
        /// Get new identifier for monster.
        /// </summary>
        /// <returns>New unique identifier.</returns>
        int GetNewId();
    }
}