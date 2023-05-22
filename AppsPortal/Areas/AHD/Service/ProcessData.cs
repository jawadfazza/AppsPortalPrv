namespace AppsPortal.Areas.AHD.Service
{
    public class ProcessData
    {
        public static string GetMonthName(int? MonthNumber)
        {
            if (MonthNumber == 1)
            {
                return "January";
            }
            if (MonthNumber == 2)
            {
                return "February";

            }
            if (MonthNumber == 3)
            {
                return "March";
            }
            if (MonthNumber == 4)
            {
                return "April";
            }
            if (MonthNumber == 5)
            {
                return "May";
            }
            if (MonthNumber == 6)
            {
                return "June";
            }
            if (MonthNumber == 7)
            {
                return "July";
            }
            if (MonthNumber == 8)
            {
                return "August";

            }
            if (MonthNumber == 9)
            {
                return "September";

            }
            if (MonthNumber == 10)
            {
                return "October";

            }
            if (MonthNumber == 11)
            {
                return "November";

            }
            if (MonthNumber == 12)
            {
                return "December";

            }
            return "";

        }

    }


}