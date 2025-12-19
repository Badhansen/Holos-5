namespace H.Core.Providers.Feed
{
    public interface IDietProvider
    {
        List<Diet> GetDiets();
        Diet GetNoDiet();
    }
}