using System.Reflection;
using BalangaAMS.Core.Interfaces;
using System;
using System.Drawing;
using System.IO;

namespace BalangaAMS.ApplicationLayer.Service
{
    public class ImageService : IImageService
    {
        private readonly string _imagelocation;
        private readonly string _defaultImageFileName;
        private Image _photo;
        private bool _hasPicture;

        public ImageService(string imagelocation, string defaultImageFileName)
        {
            _imagelocation = imagelocation;
            _defaultImageFileName = defaultImageFileName;
        }

        public void SaveImage(string sourcepath, long brethrenId)
        {
            string destinationfile = GetFullDirectory(_imagelocation) + Convert.ToString(brethrenId) + ".jpg";
            File.Copy(sourcepath, destinationfile, true);
        }

        private string GetFullDirectory(string imageLocation)
        {
            string fullDirectory;
            if (Path.GetPathRoot(imageLocation) == null)
            {
                fullDirectory = imageLocation;
            }
            else
            {
                string appPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                fullDirectory = appPath + imageLocation;
            }
            return fullDirectory;
        }

        public Image GetDefaultPicture()
        {
            Usedefaultimage();
            return _photo;
        }

        public void RemovePicture(long brethrenId)
        {
            if (IsBrethrenIdAndImageExist(brethrenId))
            {
                string fullPath = GetFullDirectory(_imagelocation) + Convert.ToString(brethrenId) + ".jpg";
                File.Delete(fullPath);
            }
        }

        public Image GetPicture(long brethrenId)
        {
            if (IsdefaultImageExist())
            {
                SetImage(brethrenId);
            }
            else
            {
                throw new FileNotFoundException("Defaut Image does not exist");
            }

            return _photo;
        }

            private bool IsdefaultImageExist()
            {
                return (File.Exists(GetFullDirectory(_imagelocation) + _defaultImageFileName));
            }

            private void SetImage(long brethrenId)
            {
                if (IsBrethrenIdAndImageExist(brethrenId))
                {
                    var filestream =
                        new FileStream(GetFullDirectory(_imagelocation) + Convert.ToString(brethrenId) + ".jpg",
                            FileMode.Open, FileAccess.Read);
                    var memorystream = new MemoryStream();
                    var bytes = new byte[filestream.Length];
                    filestream.Read(bytes, 0, (int) filestream.Length);
                    memorystream.Write(bytes, 0, (int)filestream.Length);
                    _photo = Image.FromStream(memorystream);
                    filestream.Close();
                    _hasPicture = true;
                }
                else
                {
                    Usedefaultimage();
                    _hasPicture = false;
                }
            }

                private bool IsBrethrenIdAndImageExist(long brethrenId)
                {
                    return brethrenId != 0 &&
                           File.Exists(GetFullDirectory(_imagelocation) + Convert.ToString(brethrenId) + ".jpg");
                }

                private void Usedefaultimage()
                {
                    var filestream = new FileStream(GetFullDirectory(_imagelocation) + _defaultImageFileName,
                        FileMode.Open,
                        FileAccess.Read);
                    var memorystream = new MemoryStream();
                    var bytes = new byte[filestream.Length];
                    filestream.Read(bytes, 0, (int) filestream.Length);
                    memorystream.Write(bytes, 0 , (int)filestream.Length);
                    _photo = Image.FromStream(memorystream);
                    filestream.Close();
                }  

        public bool HasPicture()
        {
            return _hasPicture;
        }

        public bool HasPicture(long brethrenId)
        {
            return IsBrethrenIdAndImageExist(brethrenId);
        }
    }
}
