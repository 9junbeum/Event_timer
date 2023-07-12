using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Event_timer
{
    public class TimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Time_ time)
            {
                string amPm = time.AM_PM == 'a' ? "오전" : "오후";
                string hour = time.Hour.ToString("D2");
                string minute = time.Minute.ToString("D2");
                string second = time.Second.ToString("D2");

                return $"{amPm} {hour}:{minute}:{second}";
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DayOfWeekConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool[] dotw)
            {
                string[] days = new string[] { "일", "월", "화", "수", "목", "금", "토" };
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < dotw.Length; i++)
                {
                    if (dotw[i])
                    {
                        sb.Append(days[i]).Append(" ");
                    }
                }

                return sb.ToString().Trim();
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class EventConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool[] eventNum)
            {
                List<string> eventNames = new List<string>();

                for (int i = 0; i < eventNum.Length; i++)
                {
                    if (eventNum[i])
                    {
                        eventNames.Add((i + 1).ToString());
                    }
                }

                return string.Join(", ", eventNames);
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
