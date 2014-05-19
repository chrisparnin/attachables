using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace ninlabs.attachables.Reminders.Adornments
{
    class DateMatcher
    {
        public DateTime? FromString(string sample, out string friendly)
        {
            sample = sample.Trim().ToLower();

            if (sample.StartsWith("next day"))
            {
                friendly = "next day";
                return DateTime.Today.AddDays(1);
            }
            if (sample.StartsWith("next week"))
            {
                friendly = "next week";
                return DateTime.Today.AddDays(7);
            }
            //if (sample.StartsWith("next month"))
            //{
            //}
            if (sample.StartsWith("tomorrow") || sample.StartsWith("tommorrow") || sample.StartsWith("tommorow"))
            {
                friendly = "tomorrow";
                return DateTime.Today.AddDays(1);
            }
            if (sample.StartsWith("today"))
            {
                friendly = "today";
                return DateTime.Today;
            }

            // Friday, etc.
            var dayOfTheWeek = DayOfTheWeek(sample);
            if (dayOfTheWeek != null)
            {
                friendly = CultureInfo.CurrentUICulture.DateTimeFormat.GetDayName(dayOfTheWeek.Value.DayOfWeek);
                return dayOfTheWeek;
            }
            // Specific date format.
            DateTime dt;

            friendly = null;

            string[] formats = {"M/d/yyyy h:mm:ss tt", "M/d/yyyy h:mm tt", 
                         "MM/dd/yyyy hh:mm:ss", "M/d/yyyy h:mm:ss", 
                         "M/d/yyyy hh:mm tt", "M/d/yyyy hh tt", 
                         "M/d/yyyy h:mm", "M/d/yyyy h:mm", 
                         "MM/dd/yyyy hh:mm", "M/dd/yyyy hh:mm",
                               
                         "M/d/yyyy", "M/d/yy",
                         "M-d-yyyy", "M-d-yy",

                         "d/M/yyyy", "d/M/yy",
                         "d-M-yyyy", "d-M-yy",

                         };

            // whitespace split is default
            // http://stackoverflow.com/questions/6111298/best-way-to-specify-whitespace-in-a-string-split-operation
            string[] dateAtoms = sample.Split(new char[0]);
            if (dateAtoms.Length > 1)
            {
                bool isValid = DateTime.TryParseExact(
                    dateAtoms[0],
                    //"31-5-14",
                    formats,
                    CultureInfo.CurrentUICulture,
                    //new CultureInfo("de-DE"),
                    DateTimeStyles.None,
                    out dt);

                if (isValid)
                    return dt;
            }

            return null;
        }


        public DateTime? DayOfTheWeek(string sample)
        {
            if (sample.StartsWith(CultureInfo.CurrentUICulture.DateTimeFormat.GetDayName(DayOfWeek.Monday).ToLower()))
            {
                return GetNextWeekday(DateTime.Today, DayOfWeek.Monday);
            }

            if (sample.StartsWith(CultureInfo.CurrentUICulture.DateTimeFormat.GetDayName(DayOfWeek.Tuesday).ToLower()))
            {
                return GetNextWeekday(DateTime.Today, DayOfWeek.Tuesday);
            }
            if (sample.StartsWith(CultureInfo.CurrentUICulture.DateTimeFormat.GetDayName(DayOfWeek.Wednesday).ToLower()))
            {
                return GetNextWeekday(DateTime.Today, DayOfWeek.Wednesday);
            }
            if (sample.StartsWith(CultureInfo.CurrentUICulture.DateTimeFormat.GetDayName(DayOfWeek.Thursday).ToLower()))
            {
                return GetNextWeekday(DateTime.Today, DayOfWeek.Thursday);
            }
            if (sample.StartsWith(CultureInfo.CurrentUICulture.DateTimeFormat.GetDayName(DayOfWeek.Friday).ToLower()))
            {
                return GetNextWeekday(DateTime.Today, DayOfWeek.Friday);
            }
            if (sample.StartsWith(CultureInfo.CurrentUICulture.DateTimeFormat.GetDayName(DayOfWeek.Saturday).ToLower()))
            {
                return GetNextWeekday(DateTime.Today, DayOfWeek.Saturday);
            }
            if (sample.StartsWith(CultureInfo.CurrentUICulture.DateTimeFormat.GetDayName(DayOfWeek.Sunday).ToLower()))
            {
                return GetNextWeekday(DateTime.Today, DayOfWeek.Sunday);
            }

            return null;
        }

        // http://stackoverflow.com/questions/6346119/datetime-get-next-tuesday
        public static DateTime GetNextWeekday(DateTime start, DayOfWeek day)
        {
            // The (... + 7) % 7 ensures we end up with a value in the range [0, 6]
            int daysToAdd = ((int)day - (int)start.DayOfWeek + 7) % 7;
            return start.AddDays(daysToAdd);
        }
    }
}
