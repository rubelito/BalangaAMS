using System.IO;

namespace BalangaAMS.ApplicationLayer.HelperClass
{
    public class FileNameChecker
    {
        public static bool IsNotValidPathOrFileName(string fileName)
        {
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            return fileNameWithoutExtension != null && (!Path.IsPathRooted(fileName) || fileNameWithoutExtension.Length == 0);
        }
    }
}
