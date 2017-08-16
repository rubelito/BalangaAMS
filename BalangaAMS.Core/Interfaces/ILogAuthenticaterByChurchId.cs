using BalangaAMS.Core.Domain;

namespace BalangaAMS.Core.Interfaces
{
    public interface ILogAuthenticaterByChurchId
    {
        BrethrenBasic Authenticate(string churchId);
    }
}