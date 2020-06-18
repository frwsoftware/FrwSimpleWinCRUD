using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FrwSoftware
{
    public enum NotificationTypeEnum
    {
        error,
        warning,
        task
    }

    [JDisplayName(typeof(FrwUtilsRes), "JNotification")]
    [JEntity]
    public class JNotification
    {
        [JDisplayName(typeof(FrwUtilsRes), "JNotification_JNotificationId")]
        [JPrimaryKey]
        public string JNotificationId { get; set; }

        [JDisplayName(typeof(FrwUtilsRes), "JNotification_Title")]
        [JReadOnly]
        [JNameProperty, JRequired]
        public string Title { get; set; }

        [JDisplayName(typeof(FrwUtilsRes), "JNotification_Description")]
        [JReadOnly]
        public string Description { get; set; }

        [JDisplayName(typeof(FrwUtilsRes), "JNotification_CreatedDate")]
        [JReadOnly]
        [JInitCurrentDate]
        public DateTime CreatedDate { get; set; }

        [JDisplayName(typeof(FrwUtilsRes), "JNotification_NotificationType")]
        [JReadOnly]
        [JDictProp(DictNames.NotificationType, false, DisplyPropertyStyle.TextOnly)]
        public string NotificationType { get; set; }

        [JDisplayName(typeof(FrwUtilsRes), "JNotification_SrcType")]
        [JReadOnly]
        public string SrcType { get; set; }

        [JDisplayName(typeof(FrwUtilsRes), "JNotification_SrcId")]
        [JReadOnly]
        public string SrcId { get; set; }

        [JDisplayName(typeof(FrwUtilsRes), "JNotification_IsArchive")]
        public bool IsArchive { get; set; }

    }
}
