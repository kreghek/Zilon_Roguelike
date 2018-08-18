namespace Zilon.Core.Schemes
{
    public interface ISchemeServiceHandler<out TScheme> where TScheme : class, IScheme
    {
        TScheme Get(string sid);
        TScheme[] GetAll();
        void LoadSchemes();
    }
}