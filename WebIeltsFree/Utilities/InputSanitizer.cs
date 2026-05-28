using System.Net;

namespace WebIeltsFree.Utilities
{
    public static class InputSanitizer
    {
        public static string Sanitize(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;
            
            return WebUtility.HtmlEncode(input);
        }
        
        // Aliased for compatibility with existing code
        public static string StripHtmlTags(string input)
        {
            return Sanitize(input);
        }
    }
}
