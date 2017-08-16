using System.Data.Common;


namespace BalangaAMS.Core.Interfaces
{
    public interface IDatabaseType
    {
         DbConnection Connectionstring();
    }
}
