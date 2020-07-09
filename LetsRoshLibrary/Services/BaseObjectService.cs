using LetsRoshLibrary.Core.Repository;
using LetsRoshLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace LetsRoshLibrary.Services
{
    public class BaseObjectService : Service<BaseObject>
    {
        public BaseObjectService(bool enableProxyCreationForContext = true) : base(enableProxyCreationForContext)
        {

        }

        public override void ConvertToPersistent(BaseObject disconnectedEntity, BaseObject persistent = null, Func<BaseObject> populatePersistent = null)
        {
            persistent = persistent ?? populatePersistent();

            if (persistent == null)
            {
                return;
            }

            persistent.Image = new ImageService(false).Get(i => i.Id == persistent.ImageId);

            if (disconnectedEntity.Image != null && persistent.Image != null)
            {
                new ImageService().ConvertToPersistent(disconnectedEntity.Image, persistent.Image);

                disconnectedEntity.ImageId = disconnectedEntity.Image.Id; 
            }

            persistent.Localizations = new LocalizationService(false).Select(l => l.BaseObjectId == persistent.Id);

            if (disconnectedEntity.Localizations.Any() && persistent.Localizations.Any())
            {
                var localizationService = new LocalizationService();

                foreach (var disconnectedEntityLocalization in disconnectedEntity.Localizations)
                {
                    disconnectedEntityLocalization.BaseObject = disconnectedEntity;

                    disconnectedEntityLocalization.BaseObjectId = disconnectedEntity.Id;

                    Func<Localization, bool> predicate = l => l.PropertyName == disconnectedEntityLocalization.PropertyName && l.BaseObjectId == disconnectedEntityLocalization.BaseObjectId;

                    if (persistent.Localizations.Any(predicate))
                    {
                        var persistentLocalization = persistent.Localizations.FirstOrDefault(predicate);

                        disconnectedEntityLocalization.LanguageId = persistentLocalization.LanguageId;

                        localizationService.ConvertToPersistent(disconnectedEntityLocalization, persistentLocalization);
                    }
                }
            }
        }
    }
}
