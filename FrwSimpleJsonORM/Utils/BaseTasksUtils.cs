using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrwSoftware
{
    public class BaseTasksUtils
    {
        public static ExpiredTypeEnum Expired(DateTime start, string period)
        {
            if (string.IsNullOrEmpty(period)) return ExpiredTypeEnum.GREEN;

            string[] p = period.Split('_');
            int count = int.Parse(p[1]);
            DateTime checkRedMin;
            DateTime checkYellowMin;
            if (p[0].Equals("DAY"))
            {
                checkRedMin = start.AddDays(count);
                if (count > 1) checkYellowMin = checkRedMin.AddDays(-2);
                else checkYellowMin = checkRedMin.AddDays(-1);
            }
            else if (p[0].Equals("WEEK"))
            {
                checkRedMin = start.AddDays(count * 7);
                checkYellowMin = checkRedMin.AddDays(-2);
            }
            else if (p[0].Equals("MONTH"))
            {
                checkRedMin = start.AddMonths(count);
                checkYellowMin = checkRedMin.AddDays(-2);
            }
            else if (p[0].Equals("YEAR"))
            {
                checkRedMin = start.AddYears(count);
                checkYellowMin = checkRedMin.AddDays(-2);
            }
            else
            {
                throw new InvalidOperationException("Wrong period type: " + p[0]);
            }

            if (checkRedMin < DateTimeOffset.Now)
            {
                return ExpiredTypeEnum.RED;
            }
            else if (checkYellowMin < DateTimeOffset.Now)
            {
                return ExpiredTypeEnum.YELLOW;
            }
            return ExpiredTypeEnum.GREEN;
        }

        public static Color ExpiredToColor(ExpiredTypeEnum e, Color defautlColor)
        {
            Color color = defautlColor;
            if (e == ExpiredTypeEnum.YELLOW) color = Color.Yellow;
            else if (e == ExpiredTypeEnum.RED) color = Color.Red;
            return color;
        }
        public static ExpiredTypeEnum Expired(DateTimeOffset start, string period)
        {
            return Expired(start.UtcDateTime, period);
        }


    }
}
