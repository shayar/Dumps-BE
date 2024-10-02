namespace Dumps.Domain.Common.Utilities
{
    public static class DateHelper
    {
        public static DateTime ConvertToLocalDateTime(this DateTime dateTime)
        {
            return dateTime.AddHours(5).AddMinutes(45);
        }
        public static DateTime ConvertLocalToUTCDateTime(this DateTime dateTime)
        {
            return dateTime.AddHours(-5).AddMinutes(-45);
        }
        public static DateTime GetCurrentLocalDateTime()
        {
            return DateTime.UtcNow.AddHours(5).AddMinutes(45);
        }
        public static bool BeAValidDate(DateTime date)
        {
            return !date.Equals(default(DateTime));
        }
    }
}
