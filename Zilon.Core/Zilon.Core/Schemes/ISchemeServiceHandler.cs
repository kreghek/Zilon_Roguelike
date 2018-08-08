namespace Zilon.Core.Schemes
{
    public interface ISchemeServiceHandler<TScheme> where TScheme : class, IScheme
    {
        TScheme Get(string sid);
        TScheme[] GetAll();
        void LoadSchemes();
    }
}