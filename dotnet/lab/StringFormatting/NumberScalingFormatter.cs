using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace StringFormatting
{
    public class ScalingFactor
    {
        private readonly int _scalingFactor;
        private readonly string _symbol;

        public ScalingFactor(int scalingFactor, string symbol)
        {
            if (scalingFactor < 0)
            {

                throw new ArgumentOutOfRangeException("numberScaling");
            }
            _scalingFactor = scalingFactor;
            _symbol = symbol;
        }

        public int Scaling
        {
            get
            {
                return _scalingFactor;
            }
        }

        public string Symbol
        {
            get
            {
                return _symbol;
            }
        }

        public static ScalingFactor None = new ScalingFactor(0, "");
        public static ScalingFactor Million = new ScalingFactor(2, "M");
        public static ScalingFactor Billion = new ScalingFactor(3, "Bn");
        
    }

    public class NumberScalingFormatter : IFormatProvider, ICustomFormatter
    {
        private readonly CultureInfo _underlyingCulture;
        private readonly ScalingFactor _scalingFactor;


        public NumberScalingFormatter(ScalingFactor scalingFactor, CultureInfo underlyingCulture)
        {
            if (scalingFactor == null || underlyingCulture == null)
            {
                throw new ArgumentNullException("underlyingCulture");
            }
            _scalingFactor = scalingFactor;
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
            else if (_scalingFactor.Scaling == 0)
            {
                scaledValue = arg;
            }
            else
            {
                try
                {
                    double convertedValue = Convert.ToDouble(arg);
                    scaledValue = Math.Pow(10, _scalingFactor.Scaling * -3) * convertedValue;
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
            formattableString.Append(_scalingFactor.Symbol);

            return string.Format(_underlyingCulture, formattableString.ToString(), Scale(arg));
        }

        #endregion
    }
}
