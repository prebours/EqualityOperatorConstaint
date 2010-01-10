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


            DisplayFormatting("{0:N}", valueToFormat, invariantCulture);

            DisplayFormatting("{0:#,#,,}", valueToFormat, invariantCulture);
            DisplayFormatting("{0:#,#,,.00}", valueToFormat, invariantCulture);
            DisplayFormatting("{0:#,#,,.00}", valueToFormat, usCulture);
            DisplayFormatting("{0:#,#,,.00}", valueToFormat, frenchCulture);


            DisplayFormatting("{0:N}", valueToFormat, invariantCulture);
            DisplayFormatting("{0:N}", valueToFormat, new NumberScalingFormatter(ScalingFactor.Million, invariantCulture));
            DisplayFormatting("{0:N}", valueToFormat, new NumberScalingFormatter(ScalingFactor.Million, usCulture));
            DisplayFormatting("{0:N}", valueToFormat, new NumberScalingFormatter(ScalingFactor.Million, frenchCulture));
        }


        // We write in the Debug Output widow of Visual Studio since Console.Write cannot render properly UNICODE characters
        // (like €)
        private static void DisplayFormatting(string format, decimal valueToFormat, IFormatProvider formatProvider)
        {
            StringBuilder code = new StringBuilder("// string.Format(");
            if (formatProvider is CultureInfo)
            {
                code.Append(GenerateCode((CultureInfo)formatProvider));
            }
            else
            {
                NumberScalingFormatter formatter = (NumberScalingFormatter)formatProvider;
                code.AppendFormat("new NumberScalingFormatter({0}.{1}, {2})",
                    typeof(ScalingFactor).Name, formatter.Factor,
                    GenerateCode(formatter.Culture));
            }
            code.AppendFormat(",{0}, {1}): ", format, valueToFormat);
            Debug.WriteLine(code.ToString());
            Debug.WriteLine("\t" + string.Format(formatProvider, format, valueToFormat));
            

        }

        private static string GenerateCode(CultureInfo culture)
        {
            if (culture == CultureInfo.InvariantCulture)
            {
                return "CultureInfo.InvariantCulture";
            }
            else
            {
                return string.Format("new CultureInfo(\"{0}\")", culture.Name);
            }
        }










    }
}
