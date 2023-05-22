namespace AppsPortal.Library
{
    public class Numbers
    {
        public static string ArabicToEnglish(string input)
        {
            char[] Numbers = "٠١٢٣٤٥٦٧٨٩".ToCharArray();

            if (input.IndexOfAny(Numbers) >= 0)
            {
                var Result = input.Replace("٠", "0")
                                  .Replace("١", "1")
                                  .Replace("٢", "2")
                                  .Replace("٣", "3")
                                  .Replace("٤", "4")
                                  .Replace("٥", "5")
                                  .Replace("٦", "6")
                                  .Replace("٧", "7")
                                  .Replace("٨", "8")
                                  .Replace("٩", "9")
                                  .Replace((char)160, (char)32);
                return Result;
            }
            return input;


        }

        public static string EnglishToArabic(string input)
        {
            char[] Numbers = "0123456789".ToCharArray();

            if (input.IndexOfAny(Numbers) >= 0)
            {
                var Result = input.Replace("0", "٠")
                                  .Replace("1", "١")
                                  .Replace("2", "٢")
                                  .Replace("3", "٣")
                                  .Replace("4", "٤")
                                  .Replace("5", "٥")
                                  .Replace("6", "٦")
                                  .Replace("7", "٧")
                                  .Replace("8", "٨")
                                  .Replace("9", "٩")
                                  .Replace((char)160, (char)32);
                return Result;
            }
            return input;
        }
    }
}