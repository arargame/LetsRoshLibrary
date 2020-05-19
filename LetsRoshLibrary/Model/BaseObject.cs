using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetsRoshLibrary.Model
{

    public abstract class BaseObject
    {
        [NotMapped]
        public EntityState EntityState { get; private set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Index(IsClustered = false, IsUnique = true)]
        public Guid Id { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        public bool IsActive { get; set; }

        public string AddedInfo { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d MMMM yyyy}")]
        [DataType(DataType.Date)]
        [ReadOnly(true)]
        public DateTime AddedDate { get; set; }

        public string ModifiedInfo { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d MMMM yyyy}")]
        [DataType(DataType.Date)]
        [ReadOnly(true)]
        public DateTime? ModifiedDate { get; set; }

        [Timestamp]
        public Byte[] Timestamp { get; set; }


        public Guid? ImageId { get; set; }

        [ForeignKey("ImageId")]
        public virtual Image Image { get; set; }


        public Guid? Class1Id { get; set; }

        [ForeignKey("Class1Id")]
        public virtual Class1 Class1 { get; set; }
        public virtual string UniqueValue { get; }

        public virtual ICollection<Localization> Localizations { get; set; }

        public BaseObject()
        {
            ChangeEntityState(EntityState.Unchanged);

            Id = Guid.NewGuid();

            AddedDate = DateTime.Now;
            ModifiedDate = DateTime.Now;

            Localizations = new List<Localization>();
        }

        public void LoadImage(string path,string name)
        {
            Image = Image.Load(path,name);
        }

        public async Task LoadImageAsync(string path,string name)
        {
            Image = await Image.LoadAsync(path,name);
        }

        public virtual void SetLocalization(Language language) { }

        public virtual string DisplayName()
        {
            return Name;
        }

        public void AddLocalization(Localization localization)
        {

            if (localization == null)
                throw new Exception("localization parameter is null");

            Localizations.Add(localization);
        }

        public void ChangeEntityState(EntityState entityState)
        {
            EntityState = entityState;
        }
    }
}
