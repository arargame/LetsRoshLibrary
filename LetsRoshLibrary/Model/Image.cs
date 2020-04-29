using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace LetsRoshLibrary.Model
{
    public class Image : BaseObject
    {
        public string Path { get; set; }

        public byte[] Data { get; set; }

        public Image(string path,byte[] data,string name)
        {
            Path = path;

            Data = data;
        }

        public static Image Load(string path,string name)
        {
            byte[] imageAsByteArray = null;

            try
            {
                using (var webClient = new WebClient())
                {
                    imageAsByteArray = webClient.DownloadData(path);
                }
            }
            catch (Exception ex)
            {

            }

            return new Image(path, imageAsByteArray,name);
        }

        public async static Task<Image> LoadAsync(string path,string name)
        {
            return await Task.Run(() => Load(path,name));
        }
    }
}
