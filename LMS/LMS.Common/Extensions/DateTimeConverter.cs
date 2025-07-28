namespace LMS.Application.Abstractions
{
    public static class DateTimeConverter
    {
        public static string? ConvertToSyrianTime(this DateTime? utcDateTime)
        {
            if (utcDateTime is null)
                return null;

            try
            {
                var tz = TimeZoneInfo.FindSystemTimeZoneById("Syria Standard Time");
                var syrianTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime.Value, tz);
                return syrianTime.ToString("yyyy-MM-dd");
            }
            catch (TimeZoneNotFoundException)
            {
                try
                {
                    var tz = TimeZoneInfo.FindSystemTimeZoneById("Asia/Damascus");
                    var syrianTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime.Value, tz);
                    return syrianTime.ToString("yyyy-MM-dd");
                }
                catch (Exception ex)
                {
                    return utcDateTime.Value.ToString("yyyy-MM-dd");
                }
            }
        }

        public static string ConvertToSyrianTime(this DateTime utcDateTime)
        {
            try
            {
                var tz = TimeZoneInfo.FindSystemTimeZoneById("Syria Standard Time");
                var syrianTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, tz);
                return syrianTime.ToString("yyyy-MM-dd");
            }
            catch (TimeZoneNotFoundException)
            {
                try
                {
                    var tz = TimeZoneInfo.FindSystemTimeZoneById("Asia/Damascus");
                    var syrianTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, tz);
                    return syrianTime.ToString("yyyy-MM-dd");
                }
                catch (Exception ex)
                {
                    return utcDateTime.ToString("yyyy-MM-dd");
                }
            }
        }


        public static string ConvertToSyrianClock(this TimeSpan utcTimeSpan, DateTime baseDate)
        {
            var utcDateTime = baseDate.Add(utcTimeSpan);

            TimeZoneInfo tz;
            try
            {
                tz = TimeZoneInfo.FindSystemTimeZoneById("Syria Standard Time"); 
            }
            catch (TimeZoneNotFoundException)
            {
                tz = TimeZoneInfo.FindSystemTimeZoneById("Asia/Damascus");
            }

            var syrianTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, tz);
            return syrianTime.ToString("HH:mm");
        }

        public static string? ConvertToSyrianClock(this TimeSpan? utcTimeSpan, DateTime baseDate)
        {
            if (utcTimeSpan is null)
            {
                return null;
            }

            var utcDateTime = baseDate.Add(utcTimeSpan.Value);

            TimeZoneInfo tz;
            try
            {
                tz = TimeZoneInfo.FindSystemTimeZoneById("Syria Standard Time");
            }
            catch (TimeZoneNotFoundException)
            {
                tz = TimeZoneInfo.FindSystemTimeZoneById("Asia/Damascus");
            }

            var syrianTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, tz);
            return syrianTime.ToString("HH:mm");
        }

    }
}
