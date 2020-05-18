using LetsRoshLibrary.Model;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetsRoshLibrary.Core.Repository
{
    public enum IncludeType
    {
        Normal,
        Then,
        All
    }

    [Obsolete("This class has been created to test retrieving includes of an entity")]
    public class Includes
    {
        public static string[] Get<T>(IncludeType includeType = IncludeType.Normal) where T : BaseObject
        {
            //var getVirtualProperties = new Func<string>(()=> 
            //{
            //    var virtuals = new List<string>();

            //    var type = typeof(T);

            //    var properties = type.GetProperties();

            //    foreach (var property in properties)
            //    {
            //        var isVirtual = property.GetGetMethod().IsVirtual;

            //        if (isVirtual && properties.FirstOrDefault(c => c.Name == property.Name + "Id") != null)
            //        {
            //            virtuals.Add(property.Name);
            //        }
            //    }

            //    return string.Join(",", virtuals);
            //});
            

            string[] list = null;

            var typeName = typeof(T).Name;

            switch (includeType)
            {
                case IncludeType.Normal:

                    if (typeName == typeof(BaseObject).Name)
                    {
                        list = new[] { "Image", "Localizations" };
                    }
                    else if (typeName == typeof(Item).Name)
                    {
                        list = Get<BaseObject>(IncludeType.Normal);
                    }
                    else if (typeName == typeof(Localization).Name)
                    {

                    }

                    break;

                case IncludeType.Then:

                    if (typeName == typeof(BaseObject).Name)
                    {
                        list = new string[] { "Localizations.BaseObject,Localizations.Language" };
                    }
                    else if (typeName == typeof(Item).Name)
                    {
                        list = Get<BaseObject>(IncludeType.Normal);
                    }
                    else if (typeName == typeof(Localization).Name)
                    {

                    }

                    break;

                case IncludeType.All:

                    if (typeName == typeof(BaseObject).Name)
                    {
                        list = Get<BaseObject>(IncludeType.All).Union(Get<BaseObject>(IncludeType.Then)).ToArray();
                    }
                    else if (typeName == typeof(Item).Name)
                    {
                        list = Get<BaseObject>(IncludeType.Normal);
                    }
                    else if (typeName == typeof(Localization).Name)
                    {

                    }

                    break;
            }

            return list;
        }
    }
}
