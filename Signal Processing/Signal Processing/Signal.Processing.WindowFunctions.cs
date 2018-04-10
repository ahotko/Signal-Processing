using Data.Annex.MathExtended.Functions;
using System;

namespace Data.Annex.Signal.Processing
{
    public partial class Signal
    {
        public enum WindowFunction
        {
            Rectangular,
            Gauss,
            Tukey,
            Hamming,
            Hann,
            Bartlett,
            Triangular,
            BartlettHann,
            Blackman,
            Kaiser,
            Nuttall,
            BlackmanHarris,
            BlackmanNuttall,
            FlatTop,
            Poisson,
            Exponential,
            Lanczos
        };

        public void GenerateWindowFunction(WindowFunction windowFunction, double parameter = 0.0)
        {
            int _width = _samples.Length;

            if (_width < 2)
            {
                throw new ArgumentOutOfRangeException("Width of Window function should be at least 2 samples.");
            }

            var _functions = new Functions();

            for (int n = 0; n < _width; n++)
            {
                double _value = 0.0;
                switch (windowFunction)
                {
                    case WindowFunction.BlackmanNuttall:
                        _value = 0.3635819 -
                            0.4891775 * Math.Cos((2.0 * Math.PI * n) / (_width - 1)) +
                            0.1365995 * Math.Cos((4.0 * Math.PI * n) / (_width - 1)) -
                            0.0106411 * Math.Cos((6.0 * Math.PI * n) / (_width - 1));
                        break;
                    case WindowFunction.Rectangular:
                        _value = 1.0;
                        break;
                    case WindowFunction.Gauss:
                        double _sigma = (parameter == 0.0) ? 0.4 : parameter;
                        double _coeff = (n - ((_width - 1) / 2.0)) / (_sigma * (_width - 1) / 2.0);
                        _value = Math.Exp(-0.5 * Math.Pow(_coeff, 2));
                        break;
                    case WindowFunction.Tukey:
                        throw new NotImplementedException("Tukey Window Function not yet implemented.");
                    //break;
                    case WindowFunction.Hamming:
                        _value = 0.53836 -
                            0.46164 * Math.Cos((2.0 * Math.PI * n) / (_width - 1));
                        break;
                    case WindowFunction.Hann:
                        _value = 0.5 * (1.0 - Math.Cos((2 * Math.PI * n) / (_width - 1)));
                        break;
                    case WindowFunction.Bartlett:
                        _value = (2.0 / (_width - 1)) * (((_width - 1) / 2.0) - Math.Abs(n - ((_width - 1) / 2.0)));
                        break;
                    case WindowFunction.Triangular:
                        _value = (2.0 / _width) * ((_width / 2.0) - Math.Abs(n - ((_width - 1) / 2.0)));
                        break;
                    case WindowFunction.BartlettHann:
                        _value = 0.62 -
                            0.48 * Math.Abs(1.0 * n / (_width - 1) - 0.5) -
                            0.38 * Math.Cos((2.0 * Math.PI * n) / (_width - 1));
                        break;
                    case WindowFunction.Blackman:
                        _value = 0.42 -
                            0.5 * Math.Cos((2.0 * Math.PI * n) / (_width - 1)) +
                            0.08 * Math.Cos((4.0 * Math.PI * n) / (_width - 1));
                        break;
                    case WindowFunction.Kaiser:
                        double _alpha = (parameter == 0.0) ? 3.0 : parameter;
                        double _piAlpha = Math.PI * _alpha;
                        double _bCoeff = Math.Sqrt(1.0 - Math.Pow((2.0 * n / (_width - 1.0)) - 1.0, 2));
                        _value = _functions.I0(_piAlpha * _bCoeff) / _functions.I0(_piAlpha);
                        break;
                    case WindowFunction.Nuttall:
                        _value = 0.355768 -
                            0.487396 * Math.Cos((2.0 * Math.PI * n) / (_width - 1)) +
                            0.144232 * Math.Cos((4.0 * Math.PI * n) / (_width - 1)) -
                            0.012604 * Math.Cos((6.0 * Math.PI * n) / (_width - 1));
                        break;
                    case WindowFunction.BlackmanHarris:
                        _value = 0.35875 -
                            0.48829 * Math.Cos((2.0 * Math.PI * n) / (_width - 1)) +
                            0.14128 * Math.Cos((4.0 * Math.PI * n) / (_width - 1)) -
                            0.01168 * Math.Cos((6.0 * Math.PI * n) / (_width - 1));
                        break;
                    case WindowFunction.FlatTop:
                        _value = 1.0 -
                            1.93 * Math.Cos((2.0 * Math.PI * n) / (_width - 1)) +
                            1.29 * Math.Cos((4.0 * Math.PI * n) / (_width - 1)) -
                            0.388 * Math.Cos((6.0 * Math.PI * n) / (_width - 1)) +
                            0.032 * Math.Cos((8.0 * Math.PI * n) / (_width - 1));
                        break;
                    case WindowFunction.Exponential:
                    case WindowFunction.Poisson:
                        //default time constant is to decay 60dB over half the Window width
                        double _tau = (parameter <= 0.0) ? (_width / 2.0 * 8.69 / 60.0) : parameter;
                        double _coeffP = Math.Abs(n - (_width - 1) / 2.0) / _tau;
                        _value = Math.Exp(-_coeffP);
                        break;
                    case WindowFunction.Lanczos:
                        _value = _functions.Sinc(2.0 * n / (_width - 1) - 1.0);
                        break;
                    default:
                        _value = 0.0;
                        break;
                }
                _samples[n] = _value;
            }
        }

        #region Wrappers for GenerateWindowFunction
        public void GenerateRectangularWindow()
        {
            GenerateWindowFunction(WindowFunction.Rectangular);
        }

        public void GenerateGaussWindow(double sigma)
        {
            GenerateWindowFunction(WindowFunction.Gauss, sigma);
        }

        public void GenerateTukeyWindow()
        {
            GenerateWindowFunction(WindowFunction.Tukey);
        }

        public void GenerateHammingWindow()
        {
            GenerateWindowFunction(WindowFunction.Hamming);
        }

        public void GenerateHannWindow()
        {
            GenerateWindowFunction(WindowFunction.Hann);
        }

        public void GenerateBartlettWindow()
        {
            GenerateWindowFunction(WindowFunction.Bartlett);
        }

        public void GenerateTriangularWindow()
        {
            GenerateWindowFunction(WindowFunction.Triangular);
        }

        public void GenerateBartlettHannWindow()
        {
            GenerateWindowFunction(WindowFunction.BartlettHann);
        }

        public void GenerateBlackmanWindow()
        {
            GenerateWindowFunction(WindowFunction.Blackman);
        }

        public void GenerateKaiserWindow(double alpha)
        {
            GenerateWindowFunction(WindowFunction.Kaiser, alpha);
        }

        public void GenerateNuttallWindow()
        {
            GenerateWindowFunction(WindowFunction.Nuttall);
        }

        public void GenerateBlackmanHarrisWindow()
        {
            GenerateWindowFunction(WindowFunction.BlackmanHarris);
        }

        public void GenerateBlackmanNuttallWindow()
        {
            GenerateWindowFunction(WindowFunction.BlackmanNuttall);
        }

        public void GenerateFlatTopWindow()
        {
            GenerateWindowFunction(WindowFunction.FlatTop);
        }

        public void GenerateExponentialWindow(double tau)
        {
            GenerateWindowFunction(WindowFunction.Exponential, tau);
        }

        public void GeneratePoissonWindow(double tau)
        {
            GenerateWindowFunction(WindowFunction.Poisson, tau);
        }

        public void GenerateLanczosWindow()
        {
            GenerateWindowFunction(WindowFunction.Lanczos);
        }
        #endregion
    }
}
