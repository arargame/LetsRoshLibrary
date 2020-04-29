using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetsRoshLibrary.Model
{
    public abstract class BaseObject
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime AddedDate { get; set; }

        public DateTime ModifiedDate { get; set; }

        public Image Image { get; set; }

        public List<Localization> Localizations = new List<Localization>();

        public BaseObject()
        {
            Id = Guid.NewGuid();

            AddedDate = ModifiedDate = DateTime.Now;
        }

        public void LoadImage(string path,string name)
        {
            Image = Image.Load(path,name);
        }

        public async Task LoadImageAsync(string path,string name)
        {
            Image = await Image.LoadAsync(path,name);
        }

        public virtual void SetLocalization(Language language)
        {

        }
    }
}
