namespace LMS.Common.Extensions
{
    public static class StringExtension
    {
        public static string Key(this string value)
        {
            var result = value.Split('.');
            return result.Length <= 1
                ? value
                : string.Join('.', result.Take(result.Length - 1));
        }

        public static string SubKey(this string value)
        {
            var result = value.Split('.');
            return result.Length == 0 ? string.Empty : result[^1];
        }

    }
}
