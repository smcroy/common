namespace qs.Extensions.DateTimeExtensions
{
    using qs.Extensions.StringExtensions;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Xml;

    public static class DateTimeExtensions
    {
        #region Public Methods

        /// <summary>
        /// Returns a new System.DateTime that adds the specified number of business days to the
        /// value of this instance. A business day is considered any day Monday - Friday. Saturday
        /// and Sunday are not considered business days. For example, if this instance is a Monday
        /// date, then the sum after adding 4 business days is a Friday date. If this instance is a
        /// Friday date, then the sum after adding 4 business days is a Thursday date. If this
        /// instance is a Saturday or Sunday date, then the sum after adding 4 business days is a
        /// Friday date. Where this instance is a Saturday or Sunday, then the first business day is
        /// considered Monday.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="days"></param>
        /// <returns></returns>
        public static DateTime AddBusinessDays(this DateTime date, int days)
        {
            if (days < 0)
                throw new ArgumentException("days cannot be negative", "days");
            if (days == 0)
                return date;

            const int SUNDAY = 0;
            const int SATURDAY = 6;

            int businessDays = days;
            int dayOfWeek = (int)date.DayOfWeek;
            int calendarDays = 0;
            while (businessDays > 0)
            {
                if (dayOfWeek == SUNDAY)
                {
                    calendarDays++;
                    dayOfWeek++;
                }
                else if (dayOfWeek == SATURDAY)
                {
                    dayOfWeek = SUNDAY;
                    calendarDays++;
                }
                else
                {
                    dayOfWeek++;
                    calendarDays++;
                    businessDays--;
                }
            }
            if (dayOfWeek == SUNDAY)
                calendarDays++;
            else if (dayOfWeek == SATURDAY)
                calendarDays += 2;
            return date.AddDays(calendarDays);
        }

        /// <summary>
        /// Returns a new System.DateTime that adds the specified number of work days to the value of
        /// this instance. A work day is considered any day Monday - Friday. Saturday and Sunday are
        /// not considered work days. For example, if this instance is a Monday date, then the sum
        /// after adding 4 work days is a Friday date. If this instance is a Friday date, then the
        /// sum after adding 4 work days is a Thursday date. If this instance is a Saturday or Sunday
        /// date, then the sum after adding 4 work days is a Friday date. Where this instance is a
        /// Saturday or Sunday, then the first work day is considered Monday.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="value"></param>
        /// <returns>
        /// An object whose value is the sum of the date and time represented by this instance and
        /// the number of work days represented by value.
        /// </returns>
        [Obsolete("Use AddBusinessDays")]
        public static DateTime AddWorkDays(this DateTime a, int value)
        {
            return a.AddWorkDays(value, new List<DateTime>());
        }

        /// <summary>
        /// Returns a new System.DateTime that adds the specified number of work days to the value of
        /// this instance. A work day is considered any day Monday - Friday. Saturday and Sunday are
        /// not considered work days. If specified, holidays are not considered work days. For
        /// example, if this instance is a Monday date, then the sum after adding 4 work days is a
        /// Friday date. If this instance is a Friday date, then the sum after adding 4 work days is
        /// a Thursday date. If this instance is a Saturday or Sunday date, then the sum after adding
        /// 4 work days is a Friday date. Where this instance is a Saturday or Sunday, then the first
        /// work day is considered Monday.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="value"></param>
        /// <param name="holidays"></param>
        /// <returns>
        /// An object whose value is the sum of the date and time represented by this instance and
        /// the number of work days represented by value.
        /// </returns>
        [Obsolete("Use AddBusinessDays")]
        public static DateTime AddWorkDays(this DateTime a, int value, IEnumerable<DateTime> holidays)
        {
            List<DateTime> h = holidays.ToList();
            int x = value;
            int y = 0;
            DateTime endDate = a.Date;
            while (x > y)
            {
                if (h.Contains(endDate))
                {
                    while (h.Contains(endDate))
                        endDate = endDate.AddDays(1);
                }
                if (endDate.DayOfWeek == DayOfWeek.Saturday)
                    endDate = endDate.AddDays(2);
                else if (endDate.DayOfWeek == DayOfWeek.Sunday)
                    endDate = endDate.AddDays(1);
                else
                {
                    endDate = endDate.AddDays(1);
                    if (h.Contains(endDate))
                        endDate = endDate.AddDays(1);
                    if (endDate.DayOfWeek == DayOfWeek.Saturday)
                        endDate = endDate.AddDays(2);
                    else if (endDate.DayOfWeek == DayOfWeek.Sunday)
                        endDate = endDate.AddDays(1);
                    y++;
                }
            }
            return endDate;
        }

        /// <summary>
        /// Compares this instance with another instance of nullable System.DateTime.
        /// </summary>
        /// <param path="a">Instance of type nullable System.DateTime to use in the comparison.</param>
        /// <param path="a">A comparand of type nullable System.DateTime.</param>
        /// <returns>
        /// A 32-bit signed integer indicating the lexical relationship between the two comparands.
        /// Value Condition Less than zero This instance is less than a -or- this instance is null.
        /// Zero This instance is equal to a -or- this instance and a are both null. Greater than
        /// zero This instance is greater than a -or- a is null.
        /// </returns>
        public static int CompareTo(this DateTime? a, DateTime? b)
        {
            if (a == null && b == null)
                return 0;
            if (a == null)
                return -1;
            if (b == null)
                return 1;
            if (a == b)
                return 0;
            if (a < b)
                return -1;
            if (a > b)
                return 1;
            return 0;
        }

        public static bool EqualsDate(this DateTime a, DateTime value)
        {
            return (a.Year == value.Year && a.Month == value.Month && a.Day == value.Day);
        }

        public static DateTime GetAbsoluteEndOfYear(this DateTime date)
        {
            DateTime a = new DateTime(date.Year + 1, 1, 1, 0, 0, 0, 0);
            return a.SubtractMilliseconds(1);
        }

        /// <summary>
        /// Returns the number of business days between the value of this instance and the specified
        /// to date. A business day is considered any day Monday - Friday. Saturday and Sunday are
        /// not considered business days.
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public static int GetBusinessDays(this DateTime fromDate, DateTime toDate)
        {
            if ((fromDate.Date.CompareTo(toDate.Date)) > 0)
            {
                DateTime s = toDate;
                DateTime e = fromDate;
                fromDate = s;
                toDate = e;
            }
            int r = (int)toDate.Date.Subtract(fromDate.Date).TotalDays;
            if (fromDate.DayOfWeek == DayOfWeek.Sunday)
                r--;
            else if (fromDate.DayOfWeek == DayOfWeek.Saturday)
                r -= 2;
            if (toDate.DayOfWeek == DayOfWeek.Sunday)
                r--;
            else if (toDate.DayOfWeek == DayOfWeek.Saturday)
                r -= 2;
            if (r <= 6 && r > 0 && !fromDate.IsWeekend() && !toDate.IsWeekend() && fromDate.DayOfWeek > toDate.DayOfWeek)
                r -= 2;
            else
                r = (r / 7) * 5 + r % 7;
            return r;
        }

        /// <summary>
        /// Get the current age in years as an integer.
        /// </summary>
        /// <param name="baseDate">Base date.</param>
        /// <returns>
        /// If the base date is less than the current date, returns the current age in years as an
        /// integer; otherwise returns 0.
        /// </returns>
        public static int GetCurrentAgeInYears(this DateTime baseDate)
        {
            return GetFutureAgeInYears(baseDate, DateTime.Now.Date);
        }

        public static int GetDaysInQuarter(this DateTime a)
        {
            int quarter = a.GetQuarter();
            return a.GetDaysInQuarter(quarter);
        }

        public static int GetDaysInQuarter(this DateTime a, int quarter)
        {
            DateTime sd;
            DateTime ed;
            int y = a.Year;
            int q = quarter > 0 && quarter <= 4 ? quarter : quarter < 0 ? 1 : 4;
            if (q < 4)
            {
                sd = new DateTime(y, (3 * q - 2), 1);
                ed = new DateTime(y, (3 * q - 2) + 3, 1);
            }
            else
            {
                sd = new DateTime(y, (3 * q - 2), 1);
                ed = new DateTime(y + 1, 1, 1);
            }
            TimeSpan ts = new TimeSpan(ed.Subtract(sd).Ticks);
            return ts.Days;
        }

        /// <summary>
        /// Returns an instance of System.DateTime that represents the end of the specified day, i.e.
        /// 23 hours, 59 minutes, 59 seconds, 999 milliseconds.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static DateTime GetEndOfDay(this DateTime a)
        {
            return new DateTime(a.Year, a.Month, a.Day, 23, 59, 59, 999);
        }

        /// <summary>
        /// Get Sunday's date of the first week in the specified month.
        /// <code>
        /// // example:
        /// DateTime n = new DateTime( 2019, 10, 15 ); DateTime fd =
        /// n.GetFirstDayOfCalendar( ); DateTime ed = n.GetLastDayOfCalendar( );
        ///
        /// // fd = Sunday, 9/29/2019
        /// // ed = Saturday, 11/2/2019
        /// </code>
        /// </summary>
        /// <param name="date"></param>
        /// <returns>Returns the Sunday's date of the first week of the specified month.</returns>
        public static DateTime GetFirstDayOfCalendarMonth(this DateTime date)
        {
            DateTime d = new DateTime(date.Year, date.Month, 1);
            int i = (int)d.DayOfWeek;
            int p = 7 - (7 - i);
            if (p > 0)
            {
                d = d.Subtract(new TimeSpan(p, 0, 0, 0));
            }
            return d;
        }

        /// <summary>
        /// Get the age in years as an integer from a specified future date.
        /// </summary>
        /// <param name="baseDate">Specifies the base date.</param>
        /// <param name="futureDate">Specifies the future date.</param>
        /// <returns>
        /// If the base date is less than the future date, returns the future age in years as an
        /// integer; otherwise returns 0.
        /// </returns>
        /// <remarks>This adjusts any February 29 date to February 28 date for calculations</remarks>
        public static int GetFutureAgeInYears(this DateTime baseDate, DateTime futureDate)
        {
            DateTime a = baseDate;
            DateTime b = futureDate;
            if (a.Month == 2 && a.Day >= 28)
                a = new DateTime(a.Year, a.Month, 28);
            if (b.Month == 2 && b.Day >= 28)
                b = new DateTime(b.Year, b.Month, 28);
            int aa = string.Format("{0}{1}", a.Year, a.DayOfYear.ToString().PadLeft(3, '0')).ToInt32();
            int bb = string.Format("{0}{1}", b.Year, b.DayOfYear.ToString().PadLeft(3, '0')).ToInt32();
            int i;
            if (aa >= bb)
                i = 0;
            else
            {
                int y = b.Year - a.Year;
                // adjust if leap year and specified date is February 29 or later '.DayOfYear > 59'
                // (31 days in January + 28 days in February)
                int aDayOfYear = a.IsLeapYear() && a.DayOfYear > 59 ? a.DayOfYear - 1 : a.DayOfYear;
                // adjust if leap year and specified date is February 29 or later
                //'.DayOfYear > 59' (31 days in January + 28 days in February)
                int bDayOfYear = b.IsLeapYear() && b.DayOfYear > 59 ? b.DayOfYear - 1 : b.DayOfYear;
                int d = bDayOfYear - aDayOfYear;
                i = d < 0 ? y - 1 : y;
            }
            return i;
        }

        public static WeekYear GetISOWeekOfYearWeekAndYear(this DateTime a)
        {
            int i = 4 - (int)a.DayOfWeek;
            DateTime b = a.AddDays(i > 0 ? i : 0);
            DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
            Calendar calendar = dfi.Calendar;
            int w = calendar.GetWeekOfYear(b, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            int y = b.Month == 12 && w > 1 ? b.Year : b.AddDays(6 - (int)b.DayOfWeek).Year;
            return new WeekYear(y, w);
        }

        /// <summary>
        /// Get Saturday's date of the last week in the specified month.
        /// <code>
        /// // example:
        /// DateTime n = new DateTime( 2019, 10, 15 );
        /// DateTime fd = n.GetFirstDayOfCalendar( );
        /// DateTime ed = n.GetLastDayOfCalendar( );
        ///
        /// // fd = Sunday, 9/29/2019
        /// // ed = Saturday, 11/2/2019
        /// </code>
        /// </summary>
        /// <param name="date"></param>
        /// <returns>Returns the Saturday's date of the last week of the specified month.</returns>
        public static DateTime GetLastDayOfCalendarMonth(this DateTime date)
        {
            DateTime a = date.AddMonths(1);
            DateTime d = new DateTime(a.Year, a.Month, 1);
            int i = (int)d.DayOfWeek;
            int p = 7 - (i + 1);
            if (p > 0)
            {
                d = d.Add(new TimeSpan(p, 0, 0, 0));
            }
            return d;
        }

        /// <summary>
        /// Returns a new System.DateTime that is the next business day from the value of this
        /// instance. A business day is considered any day Monday - Friday. Saturday and Sunday are
        /// not considered business days. For example, if this instance is a Friday date, then the
        /// next business day is a Monday date. If this instance is a Saturday or Sunday date, then
        /// the next business day is a Monday date.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime GetNextBusinessDay(this DateTime date)
        {
            int calendarDays = 0;
            if (date.DayOfWeek == DayOfWeek.Friday)
                calendarDays += 3;
            else if (date.DayOfWeek == DayOfWeek.Saturday)
                calendarDays += 2;
            else if (date.DayOfWeek == DayOfWeek.Sunday)
                calendarDays++;
            else
                calendarDays++;
            return date.AddDays(calendarDays);
        }

        /// <summary>
        /// Returns a new System.DateTime that is the prior business day from the value of this
        /// instance. A business day is considered any day Monday - Friday. Saturday and Sunday are
        /// not considered business days. For example, if this instance is a Monday date, then the
        /// prior business day is a Friday date. If this instance is a Saturday or Sunday date, then
        /// the prior business day is a Friday date.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime GetPriorBusinessDay(this DateTime date)
        {
            int calendarDays = 0;
            if (date.DayOfWeek == DayOfWeek.Monday)
                calendarDays += 3;
            else if (date.DayOfWeek == DayOfWeek.Sunday)
                calendarDays += 2;
            else if (date.DayOfWeek == DayOfWeek.Saturday)
                calendarDays++;
            else
                calendarDays++;
            return date.SubtractDays(calendarDays);
        }

        public static int GetQuarter(this DateTime a)
        {
            DateTime d = a;
            int m = d.Month;
            return m % 3 == 0 ? m / 3 : m / 3 + 1;
        }

        public static DateTime GetQuarterEndDate(this DateTime a)
        {
            int quarter = a.GetQuarter();
            return a.GetQuarterEndDate(quarter);
        }

        public static DateTime GetQuarterEndDate(this DateTime a, int quarter)
        {
            int y = a.Year;
            int q = quarter > 0 && quarter <= 4 ? quarter : quarter < 0 ? 1 : 4;
            int m = (q * 3) + 1;
            if (m > 12)
            {
                m = 1;
                y++;
            }
            return new DateTime(y, m, 1).AddDays(-1);
        }

        public static List<string> GetQuarterMonthNames(this DateTime a)
        {
            int quarter = a.GetQuarter();
            List<string> l = new List<string>();
            for (int i = quarter * 3 - 2; i <= quarter * 3; i++)
                l.Add(new DateTime(a.Year, i, 1).ToString("MMMM"));
            return l;
        }

        public static DateTime GetQuarterStartDate(this DateTime a)
        {
            int quarter = a.GetQuarter();
            return a.GetQuarterStartDate(quarter);
        }

        public static DateTime GetQuarterStartDate(this DateTime a, int quarter)
        {
            int y = a.Year;
            int q = quarter > 0 && quarter <= 4 ? quarter : quarter < 0 ? 1 : 4;
            return new DateTime(y, q * 3 - 2, 1);
        }

        /// <summary>
        /// Returns an instance of System.DateTime that represents the start of the specified day,
        /// i.e. 0 hour, 0 minute, 0 second, 0 millisecond.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static DateTime GetStartOfDay(this DateTime a)
        {
            return new DateTime(a.Year, a.Month, a.Day, 0, 0, 0, 0);
        }

        /// <summary>
        /// Get unique identifier based upon the specified date time. This is a repeatable identifier
        /// generator, i.e. given the same date time the same unique identifer is returned.
        /// </summary>
        /// <param name="a">Specified date time from which a unique identifier is generated.</param>
        /// <returns>Retrns a unique identifier based upon the specified date time.</returns>
        public static string GetUniqueID(this DateTime a)
        {
            return a.GetHashCode().ToString("x").PadLeft(8, '0');
        }

        /// <summary>
        /// Returns the Coordinated Universal Time (UTC) offset for the specified local time.
        /// </summary>
        /// <param name="a"></param>
        /// <returns>Coordinated Universal Time (UTC) offset as a TimeSpan.</returns>
        public static TimeSpan GetUtcOffset(this DateTime a)
        {
            return TimeZoneInfo.Local.GetUtcOffset(a);
        }

        /// <summary>
        /// Returns the Coordinated Universal Time (UTC) offset for the specified local time adjusted
        /// for daylight saving time periods.
        /// </summary>
        /// <param name="a"></param>
        /// <returns>Coordinated Universal Time (UTC) offset as a TimeSpan.</returns>
        public static TimeSpan GetUtcOffsetAdjusted(this DateTime a)
        {
            TimeSpan t = TimeZoneInfo.Local.GetUtcOffset(a);
            if (TimeZoneInfo.Local.IsDaylightSavingTime(a))
                t = t.Subtract(new TimeSpan(1, 0, 0));
            return t;
        }

        /// <summary>
        /// Returns the week of the year that includes the date in the specified System.DateTime.
        /// Both the starting day of week, and the rule determining the first week of the year are
        /// based upon the current culture.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static int GetWeekOfYear(this DateTime a)
        {
            DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
            Calendar calendar = dfi.Calendar;
            return calendar.GetWeekOfYear(a, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
        }

        /// <summary>
        /// Returns the week of the year that includes the date in the specified System.DateTime. The
        /// ISO-8601 week date starts on Monday and ends on Sunday and the calendar week rule is the
        /// first four day week. The US week date starts on Sunday and ends on Saturday.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="firstDayOfWeek">
        /// Specifies the first day of the week to calculate the week of year.
        /// </param>
        /// <param name="calendarWeekRule">
        /// Specifies the rule to determine the first week of the year.
        /// </param>
        /// <returns></returns>
        public static int GetWeekOfYear(this DateTime a, DayOfWeek firstDayOfWeek, CalendarWeekRule calendarWeekRule)
        {
            DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
            Calendar calendar = dfi.Calendar;
            return calendar.GetWeekOfYear(a, calendarWeekRule, firstDayOfWeek);
        }

        /// <summary>
        /// Returns the week and week year of the year that includes the date in the specified
        /// System.DateTime. The ISO-8601 week date starts on Monday and ends on Sunday and the
        /// calendar week rule is the first four day week. The US week date starts on Sunday and ends
        /// on Saturday.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="firstDayOfWeek">
        /// Specifies the first day of the week to calculate the week of year.
        /// </param>
        /// <param name="calendarWeekRule">
        /// Specifies the rule to determine the first week of the year.
        /// </param>
        /// <returns></returns>
        public static WeekYear GetWeekOfYearWeekAndYear(this DateTime a, DayOfWeek firstDayOfWeek, CalendarWeekRule calendarWeekRule)
        {
            int w = a.GetWeekOfYear(firstDayOfWeek, calendarWeekRule);
            int y = a.Month == 12 && w > 1 ? a.Year : a.AddDays(6 - (int)a.DayOfWeek).Year;
            return new WeekYear(y, w);
        }

        /// <summary>
        /// Returns the week and week year of the year that includes the date in the specified
        /// System.DateTime. Both the starting day of week, and the rule determining the first week
        /// of the year are based upon the current culture.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static WeekYear GetWeekOfYearWeekAndYear(this DateTime a)
        {
            int w = a.GetWeekOfYear();
            int y = a.Month == 12 && w > 1 ? a.Year : a.AddDays(6 - (int)a.DayOfWeek).Year;
            return new WeekYear(y, w);
        }

        /// <summary>
        /// Returns the year containing the week of the year that includes the date in the specified
        /// System.DateTime. The ISO-8601 week date starts on Monday and ends on Sunday and the
        /// calendar week rule is the first four day week. The US week date starts on Sunday and ends
        /// on Saturday.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="firstDayOfWeek">
        /// Specifies the first day of the week to calculate the week of year.
        /// </param>
        /// <param name="calendarWeekRule">
        /// Specifies the rule to determine the first week of the year.
        /// </param>
        /// <returns></returns>
        public static int GetWeekOfYearYear(this DateTime a, DayOfWeek firstDayOfWeek, CalendarWeekRule calendarWeekRule)
        {
            int w = a.GetWeekOfYear(firstDayOfWeek, calendarWeekRule);
            return a.Month == 12 && w > 1 ? a.Year : a.AddDays(6 - (int)a.DayOfWeek).Year;
        }

        /// <summary>
        /// Returns the year containing the week of the year that includes the date in the specified
        /// System.DateTime. Both the starting day of week, and the rule determining the first week
        /// of the year are based upon the current culture.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static int GetWeekOfYearYear(this DateTime a)
        {
            int w = a.GetWeekOfYear();
            return a.Month == 12 && w > 1 ? a.Year : a.AddDays(6 - (int)a.DayOfWeek).Year;
        }

        /// <summary>
        /// Returns a value indicating whether the specified date and time is within a daylight
        /// saving time period.
        /// </summary>
        /// <param name="a"></param>
        /// <returns>
        /// true if the specified date and time is within a daylight saving time period; otherwise false.
        /// </returns>
        public static bool IsDaylightSavingTime(this DateTime a) => TimeZoneInfo.Local.IsDaylightSavingTime(a);

        /// <summary>
        /// Determines if the specified date is contained within a leap year (i.e. 366 days in the year).
        /// </summary>
        /// <param name="a">
        /// Date from which to determine if the year is a leap year (i.e. 366 days in the year).
        /// </param>
        /// <returns>Returns true if a leap year; otherwise returns false.</returns>
        public static bool IsLeapYear(this DateTime a)
        {
            return new DateTime(a.Year, 12, 31).DayOfYear > 365;
        }

        /// <summary>
        /// Determines if the specified date is recognized as a part of the week end (i.e. Saturday
        /// or Sunday).
        /// </summary>
        /// <param name="a">Date from which to determine if part of a week end.</param>
        /// <returns>Returns true if the date is a Saturday or Sunday; otherwise false.</returns>
        public static bool IsWeekend(this DateTime a)
        {
            return (a.DayOfWeek == DayOfWeek.Saturday || a.DayOfWeek == DayOfWeek.Sunday) ? true : false;
        }

        /// <summary>
        /// Determines if the specified date is within the stated date range.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns>
        /// Returns true if the specified date is within the date range specified; otherwise returns false.
        /// </returns>
        public static bool IsWithinRange(this DateTime value, DateTime fromDate, DateTime toDate)
        {
            return value.CompareTo(fromDate) >= 0 && value.CompareTo(toDate) <= 0 ? true : false;
        }

        /// <summary>
        /// Returns a new System.DateTime that subtracts the specified number of business days from
        /// the value of this instance. A business day is considered any day Monday - Friday.
        /// Saturday and Sunday are not considered business days. For example, if this instance is a
        /// Monday date, then the value after subtracting 4 business days is a Tuesday date. If this
        /// instance is a Friday date, then the value after subtracting 4 business days is a Monday
        /// date. If this instance is a Saturday or Sunday date, then the value after subtracting 4
        /// business days is a Tuesday date. Where this instance is a Saturday or Sunday, then the
        /// first prior business day is considered Friday.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="days"></param>
        /// <returns></returns>
        public static DateTime SubtractBusinessDays(this DateTime date, int days)
        {
            if (days < 0)
                throw new ArgumentException("days cannot be negative", "days");
            if (days == 0)
                return date;

            const int SUNDAY = 0;
            const int SATURDAY = 6;
            const int FRIDAY = 5;
            int businessDays = days;
            int dayOfWeek = (int)date.DayOfWeek;
            int calendarDays = 0;
            if (dayOfWeek == SUNDAY)
            {
                dayOfWeek = FRIDAY;
                calendarDays += 2;
            }
            else if (dayOfWeek == SATURDAY)
            {
                dayOfWeek = FRIDAY;
                calendarDays = calendarDays++;
            }
            while (businessDays > 0)
            {
                if (dayOfWeek == SUNDAY)
                {
                    dayOfWeek = SATURDAY;
                    calendarDays++;
                }
                else if (dayOfWeek == SATURDAY)
                {
                    dayOfWeek--;
                    calendarDays++;
                }
                else
                {
                    dayOfWeek--;
                    calendarDays++;
                    businessDays--;
                }
            }
            if (dayOfWeek == SUNDAY)
                calendarDays += 2;
            else if (dayOfWeek == SATURDAY)
                calendarDays++;
            return date.SubtractDays(calendarDays);
        }

        public static DateTime SubtractDays(this DateTime a, double value)
        {
            return a.AddDays(0 - value);
        }

        public static DateTime SubtractHours(this DateTime a, double value)
        {
            return a.AddHours(0 - value);
        }

        public static DateTime SubtractMilliseconds(this DateTime a, double value)
        {
            return a.AddMilliseconds(0 - value);
        }

        public static DateTime SubtractMinutes(this DateTime a, double value)
        {
            return a.AddMinutes(0 - value);
        }

        public static DateTime SubtractMonths(this DateTime a, int months)
        {
            return a.AddMonths(0 - months);
        }

        public static DateTime SubtractSeconds(this DateTime a, double value)
        {
            return a.AddSeconds(0 - value);
        }

        public static DateTime SubtractTicks(this DateTime a, long value)
        {
            return a.AddTicks(0 - value);
        }

        /// <summary>
        /// Returns a new System.DateTime that subtracts the specified number of work days from the
        /// value of this instance. A work day is considered any day Monday - Friday. Saturday and
        /// Sunday are not considered work days. For example, if this instance is a Friday date, then
        /// the difference after subtracting 4 work days is a Monday date. If this instance is a
        /// Thursday date, then the difference after subtracting 4 work days is a Friday date. If
        /// this instance is a Saturday or Sunday date, then the difference after subtracting 4 work
        /// days is a Monday date. Where this instance is a Saturday or Sunday, then the last work
        /// day is considered Friday.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="value"></param>
        /// <returns>
        /// An object whose value is the difference of the date and time represented by this instance
        /// and the number of work days represented by value.
        /// </returns>
        public static DateTime SubtractWorkDays(this DateTime a, int value)
        {
            return a.SubtractWorkDays(value, new List<DateTime>());
        }

        /// <summary>
        /// Returns a new System.DateTime that subtracts the specified number of work days from the
        /// value of this instance. A work day is considered any day Monday - Friday. Saturday and
        /// Sunday are not considered work days. If specified, holidays are not considered work days.
        /// For example, if this instance is a Friday date, then the difference after subtracting 4
        /// work days is a Monday date. If this instance is a Thursday date, then the difference
        /// after subtracting 4 work days is a Friday date. If this instance is a Saturday or Sunday
        /// date, then the difference after subtracting 4 work days is a Monday date. Where this
        /// instance is a Saturday or Sunday, then the last work day is considered Friday.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="value"></param>
        /// <param name="holidays"></param>
        /// <returns>
        /// An object whose value is the difference of the date and time represented by this instance
        /// and the number of work days represented by value.
        /// </returns>
        public static DateTime SubtractWorkDays(this DateTime a, int value, IEnumerable<DateTime> holidays)
        {
            List<DateTime> h = holidays.ToList();
            int x = value;
            int y = 0;
            DateTime startDate = a.Date;
            while (x > y)
            {
                if (h.Contains(startDate))
                {
                    while (h.Contains(startDate))
                        startDate = startDate.AddDays(-1);
                }
                if (startDate.DayOfWeek == DayOfWeek.Saturday)
                    startDate = startDate.AddDays(-1);
                else if (startDate.DayOfWeek == DayOfWeek.Sunday)
                    startDate = startDate.AddDays(-2);
                else
                {
                    startDate = startDate.AddDays(-1);
                    if (h.Contains(startDate))
                    {
                        while (h.Contains(startDate))
                            startDate = startDate.AddDays(-1);
                    }
                    if (startDate.DayOfWeek == DayOfWeek.Saturday)
                        startDate = startDate.AddDays(-1);
                    else if (startDate.DayOfWeek == DayOfWeek.Sunday)
                        startDate = startDate.AddDays(-2);
                    y++;
                }
            }
            return startDate;
        }

        public static DateTime SubtractYears(this DateTime a, int value)
        {
            return a.AddYears(0 - value);
        }

        public static string ToISO8601Modified(this DateTime a)
        {
            return a.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// Returns the local date and time equivalent of the specified date and time.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static DateTime ToLocalDateTime(this DateTimeOffset a)
        {
            if (a.DateTime.Equals(DateTime.MinValue))
                return a.Date;
            return a.UtcDateTime.AddHours(TimeZoneInfo.Local.BaseUtcOffset.Hours + (TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now) ? 1 : 0));
        }

        public static string ToLongDateShortTimeString(this DateTime a)
        {
            return string.Format("{0} {1}", a.ToLongDateString(), a.ToShortTimeString());
        }

        public static string ToLongDateTimeString(this DateTime a)
        {
            return string.Format("{0} {1}", a.ToLongDateString(), a.ToLongTimeString());
        }

        /// <summary>
        /// Convert the DateTime Year to the equivalent Roman Numeral.
        /// </summary>
        /// <param name="a">Instance of type System.DateTime of which the Year is converted.</param>
        /// <returns>Returns an instance of RomanNumeral.</returns>
        public static RomanNumeral ToRomanNumeral(this DateTime a)
        {
            return new RomanNumeral(a);
        }

        public static string ToShortDateLongTimeString(this DateTime a)
        {
            return string.Format("{0} {1}", a.ToShortDateString(), a.ToLongTimeString());
        }

        /// <summary>
        /// Converts the System.DateTime to its equivalent short date and time string representation.
        /// </summary>
        /// <param name="a">Instance of type System.DateTime to be converted.</param>
        /// <returns>String representation of the specified date and time.</returns>
        public static string ToShortDateTimeString(this DateTime a)
        {
            return string.Format("{0} {1}", a.ToShortDateString(), a.ToShortTimeString());
        }

        /// <summary>
        /// Converts the System.DateTime to a System.String treating the date and time value as a
        /// local time. Returns as 2012-09-21T16:45:13Z.
        /// </summary>
        /// <param path="a">Instance of type System.DateTime to be converted.</param>
        /// <returns>String representation of the specified date and time.</returns>
        public static string ToShortXml(this DateTime a)
        {
            return XmlConvert.ToString(a, "yyyy-MM-ddTHH:mm:ssZ");
        }

        /// <summary>
        /// Converts the System.DateTime to a System.String treating the date and time value as a
        /// local time and converted to a Utc.
        /// </summary>
        /// <param path="value">Instance of type System.DateTime to be converted.</param>
        /// <returns>String representation of the specified date and time as Utc.</returns>
        public static string ToUtcXml(this DateTime a)
        {
            return XmlConvert.ToString(a, XmlDateTimeSerializationMode.Utc);
        }

        /// <summary>
        /// "2012-09-24T17:01:29.3554687Z" Converts the System.DateTime to a System.String treating
        /// the date and time value as a local time. Returns as 2012-09-21T16:45:13.3554687Z
        /// </summary>
        /// <param path="a">Instance of type System.DateTime to be converted.</param>
        /// <returns>String representation of the specified date and time.</returns>
        public static string ToXml(this DateTime a)
        {
            return XmlConvert.ToString(a, "yyyy-MM-ddTHH:mm:ssZzzzzz");
        }

        #endregion Public Methods
    }
}