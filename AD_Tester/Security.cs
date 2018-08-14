using System;



namespace AD_Tester
{
    public class Security
    {
        /**
         * Used to clean params to survey provider
         */
        public static string CleanInput(string sInput,
                                        string sAllowedCharacters,
                                        int iMaxLength)
        {
            String sClean = "";

            if (sInput == null) return "";

            for (int i = 0; i < sInput.Length; i++)
            {
                char c = Convert.ToChar(sInput.Substring(i, 1));
                if (
                    (c >= 'a' && c <= 'z') ||
                    (c >= 'A' && c <= 'Z') ||
                    (c >= '0' && c <= '9') ||
                    sAllowedCharacters.Contains(c.ToString())
                    )
                    sClean = sClean + c.ToString();
            }

            if (sClean.Length > iMaxLength)
                sClean = sClean.Substring(0, iMaxLength);

            return sClean;
        }


    }
}
