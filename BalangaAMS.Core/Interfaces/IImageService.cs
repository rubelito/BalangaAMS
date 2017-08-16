using System.Drawing;

namespace BalangaAMS.Core.Interfaces
{
    public interface IImageService
    {
        void SaveImage(string sourcepath, long brethrenId);
        Image GetPicture(long brethrenId);
        void RemovePicture(long brethrenId);
        Image GetDefaultPicture();
        bool HasPicture();
        bool HasPicture(long brethrenId);
    }
}
