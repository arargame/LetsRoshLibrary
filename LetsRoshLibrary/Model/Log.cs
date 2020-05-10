using LetsRoshLibrary.Core.ADO;
using LetsRoshLibrary.Core.Connection;
using LetsRoshLibrary.Core.UnitofWork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LetsRoshLibrary.Model
{
    public enum LogType
    {
        None,
        Error,
        Info,
        Warning,
        Debug
    }

    public class Log : BaseObject
    {
        public LogType LogType { get; set; }

        public string Category { get; set; }

        [StringLength(36)]
        [Index()]
        public string EntityId { get; set; }

        public Log()
        {

        }

        /// <summary>
        /// Log constructor
        /// </summary>
        /// <param name="category">Kategori(Örnek : sınıf adı)</param>
        /// <param name="name">Ad(Örnek : fonksiyon adı)</param>
        /// <param name="description">Hata ile ilgili açıklama veya durum bilgilendirilmesi(Örnek : ex.ToString();)</param>
        /// <param name="logType">Log kaydının tipi(Örnek : Hata kaydı ise LogType.Error,bilgilendirme ise LogType.Info)</param>
        /// <param name="entityId">Log kaydının sahibi nesne</param>
        public Log(string category,
            string name,
            string description,
            LogType logType,
            string entityId = null)
        {
            Initialize();

            Category = category;
            Name = name;
            Description = description;
            LogType = logType;
            EntityId = entityId;
        }

        public Log(string description,
            LogType logType = LogType.Error,
            string entityId = null,
            int frameIndex = 1)
        {
            Initialize();

            var methodBase = new StackTrace().GetFrame(frameIndex).GetMethod();

            Category = methodBase.DeclaringType.Name;

            Name = methodBase.Name;

            Description = description;

            LogType = logType;

            EntityId = entityId;
        }

        void Initialize()
        {
            Id = Guid.NewGuid();

            AddedDate = DateTime.Now;

            ModifiedDate = DateTime.Now;
        }

        public override string DisplayName()
        {
            return string.Format("{0}/{1}", Category, Name);
        }

        public static void Save(Log log)
        {
            Task<bool> t = new Task<bool>(new Func<bool>(() =>
            {
                Thread.Sleep(2000 - DateTime.Now.Millisecond);

                //MsSqlConnection connection = new MsSqlConnection();

                //return SqlManager.ExecuteNonQuery("insert into Log (LogType,Category,Name,Description,EntityId,AddedDate,ModifiedDate,IsActive)"
                //                            + " values(@LogType,@Category,@Name,@Description,@EntityId,@AddedDate,@ModifiedDate,1)",
                //    new Dictionary<string, object>()
                //    {
                //        { "@LogType", log.LogType },
                //        { "@Category",log.Category },
                //        { "@Name", log.Name },
                //        { "@Description", log.Description.Replace("'"," ") },
                //        { "@EntityId", log.EntityId ?? (object)DBNull.Value },
                //        { "@AddedDate", DateTime.Now },
                //        { "@ModifiedDate", DateTime.Now }
                //    }) == 1;

                using (var uow = new Dota2UnitofWork())
                {
                    uow.Load<Log>().Create(log);

                    return uow.Commit();
                }
            }));

            t.Start();
        }
    }
}
