using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace StringFormatting
{
    public class NumberScalingFormatter : IFormatProvider, ICustomFormatter
    {
        private readonly CultureInfo _underlyingCulture;
        private readonly int _numberScaling;

      
        public NumberScalingFormatter(int numberScaling, CultureInfo underlyingCulture)
        {
            if (numberScaling <= 0)
            {
                throw new ArgumentOutOfRangeException("numberScaling");
            }
            if (underlyingCulture == null)
            {
                throw new ArgumentNullException("underlyingCulture");
            }
            _numberScaling = numberScaling;
            _underlyingCulture = underlyingCulture;
        }

        #region IFormatProvider Members

        public object GetFormat(Type formatType)
        {
            object formatter = null;
            if (formatType == typeof(ICustomFormatter))
            {
                formatter = this;
            }
            else
            {
                formatter = _underlyingCulture.GetFormat(formatType);
            }
            return formatter;
        }

        #endregion

        #region ICustomFormatter Members

        private object Scale(object arg)
        {
            object scaledValue = null;

            if (arg == null)
            {
                scaledValue = null;
            }
            else if (_numberScaling == 0)
            {
                scaledValue = arg;
            }
            else
            {
                try
                {
                    double convertedValue = Convert.ToDouble(arg);
                    scaledValue = Math.Pow(10, _numberScaling * -3) * convertedValue;
                }
                catch (InvalidCastException)
                {

                }
            }
            return scaledValue;
        }

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            StringBuilder formattableString = new StringBuilder("{0");
            if (!string.IsNullOrEmpty(format))
            {
                formattableString.Append(":");
                formattableString.Append(format);
            }
            formattableString.Append("}");

            return string.Format(_underlyingCulture, formattableString.ToString(), Scale(arg));
        }

        #endregion
    }
}
