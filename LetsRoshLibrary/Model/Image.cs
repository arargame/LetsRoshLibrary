using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        //public BaseObject BaseObject { get; set; }

        public string Path { get; set; }

        public byte[] Data { get; set; }

        public Image() { }
        public Image(string path,byte[] data,string name)
        {
            Path = path;

            Data = data;

            Name = name;
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
                Log.Save(new Log(ex.Message + string.Format(" (path : {0},name : {1})", path, name), LogType.Error));
            }

            return new Image(path, imageAsByteArray,name);
        }

        public async static Task<Image> LoadAsync(string path,string name)
        {
            return await Task.Run(() => Load(path,name));
        }

        public override bool Equals(object obj)
        {
            var image = obj as Image;

            if (image is null)
                return false;

            return Name == image.Name && Path == image.Path && Data == image.Data;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
