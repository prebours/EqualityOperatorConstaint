using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text;
using System.Globalization;

namespace StringFormatting
{
    class Program
    {
        static void Main(string[] args)
        {
            CultureInfo invariantCulture = CultureInfo.InvariantCulture;
            CultureInfo usCulture = new CultureInfo("en-US");
            CultureInfo frenchCulture = new CultureInfo("fr-FR");

            // 1billion and 345 millions
            decimal valueToFormat = 1345278000.346M;

            DisplayFormatting("{0:#,#,,.00}", valueToFormat, invariantCulture);
            DisplayFormatting("{0:#,#,,.00}", valueToFormat, usCulture);
            DisplayFormatting("{0:#,#,,.00}", valueToFormat, frenchCulture);


            DisplayFormatting("{0:C}", valueToFormat, invariantCulture);
            DisplayFormatting("{0:C}", valueToFormat, new NumberScalingFormatter(2, invariantCulture));
            DisplayFormatting("{0:C}", valueToFormat, new NumberScalingFormatter(2, usCulture));
            DisplayFormatting("{0:C}", valueToFormat, new NumberScalingFormatter(2, frenchCulture));
        }

      
        // We write in the Debug Output widow of Visual Studio since Console.Write cannot render properly UNICODE characters
        // (like €)
        private static void DisplayFormatting(string format, decimal valueToFormat, IFormatProvider formatProvider)
        {
            //culture.NumberFormat.NumberGroupSeparator
            Console.WriteLine(string.Format(formatProvider, format, valueToFormat));
            Debug.WriteLine(string.Format(formatProvider, format, valueToFormat));
        }

        


    }
}
