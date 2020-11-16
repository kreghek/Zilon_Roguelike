namespace Zilon.Core.Schemes
{
    /// <summary>
    /// The handler of concrete scheme.
    /// </summary>
    /// <typeparam name="TScheme"></typeparam>
    public interface ISchemeServiceHandler<out TScheme> where TScheme : class, IScheme
    {
        TScheme[] GetAll();

        TScheme GetItem(string sid);

        void LoadSchemes();
    }
}