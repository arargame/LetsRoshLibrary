using LetsRoshLibrary.Core.Repository;
using LetsRoshLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetsRoshLibrary.Services
{
    public class BaseObjectService : Service<BaseObject>
    {

        public override void ConvertToPersistent(BaseObject disconnectedEntity, object persistent = null, Func<object> populatePersistent = null)
        {
            persistent = persistent ?? populatePersistent();

            if (persistent == null)
            {
                return;
            }

            var persistentImage = persistent.GetType().GetProperty("Image").GetValue(persistent, null);

            if (disconnectedEntity.Image != null && persistentImage != null)
            {
                new ImageService().ConvertToPersistent(disconnectedEntity.Image, persistentImage);

                disconnectedEntity.ImageId = disconnectedEntity.Image.Id; 
            }

            if (disconnectedEntity.Localizations.Any())
            {
                var list = persistent.GetType().GetProperty("Localizations").GetValue(persistent, null) as IEnumerable<object>;

                if (list == null)
                    return;

                var persistentLocalizations = list.Select(pl => new LocalizationService().AnonymousTypeToT(pl)).ToList();

                foreach (var disconnectedEntityLocalization in disconnectedEntity.Localizations)
                {
                    var localizationRepository = new LocalizationRepository();

                    new LocalizationService().ConvertToPersistent(disconnectedEntityLocalization, persistentLocalizations.FirstOrDefault(localizationRepository.UniqueFilter(disconnectedEntityLocalization).Compile()));

                    if (persistentLocalizations.Any(localizationRepository.UniqueFilter(disconnectedEntityLocalization).Compile()))
                    {
                        disconnectedEntityLocalization.ChangeEntityState(System.Data.Entity.EntityState.Added);
                    }
                }
            }
        }
    }
}
