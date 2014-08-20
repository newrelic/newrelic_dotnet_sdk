namespace NewRelic.Platform.Sdk.Utils
{
    public static class StringExtensions
    {
        public static bool IsValidString(this string str)
        {
            bool result = !string.IsNullOrEmpty(str) && str.Trim().Length != 0;
            return result;
        }
    }
}
