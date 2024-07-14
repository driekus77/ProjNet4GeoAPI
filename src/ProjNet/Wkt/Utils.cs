using System;

namespace ProjNet.Wkt
{
    /// <summary>
    /// Helper functions for WKT Parser(s).
    /// </summary>
    public class Utils
    {
        internal static double CalcAsFractionOf(uint i, uint f)
        {
            // Convert f to string to count the digits
            string fstr = f.ToString();
            int fractionDigits = fstr.Length;

            double d = i;

            // Calculate the fractional part from f based on the number of fractional digits
            double divisor = Math.Pow(10, fractionDigits);
            double fractionPart = f / divisor;

            // Sum i and the fractional part
            return d + fractionPart;
        }
    }
}
