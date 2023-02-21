namespace qs
{
    using System;

    [Serializable]
    public class WeekYear
    {
        #region Constructors

        public WeekYear()
        {
        }

        public WeekYear(int year, int week)
        {
            Year = year;
            Week = week;
        }

        #endregion Constructors

        #region Properties

        public int Week
        {
            get;
            set;
        }

        public int Year
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods

        public override string ToString()
        {
            return string.Format("{0}W{1:00}", Year, Week);
        }

        #endregion Methods
    }
}