namespace Zilon.Core.Schemes
{
    /// <summary>
    /// The handler of concrete scheme.
    /// </summary>
    /// <typeparam name="TScheme"></typeparam>
    public interface ISchemeServiceHandler<out TScheme> where TScheme : class, IScheme
    {
        TScheme GetItem(string sid);
        TScheme[] GetAll();
        void LoadSchemes();
    }
}