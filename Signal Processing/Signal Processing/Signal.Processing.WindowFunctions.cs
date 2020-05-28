using System;
using System.Collections.Generic;

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

        private Dictionary<WindowFunction, Func<double, int, double, double>> _windowFunctions = new Dictionary<WindowFunction, Func<double, int, double, double>>()
        {
            { WindowFunction.BlackmanNuttall,
                (double value, int windowWidth, double additionalParameter) => 0.3635819 -
                            0.4891775 * Math.Cos((2.0 * Math.PI * value) / (windowWidth - 1)) +
                            0.1365995 * Math.Cos((4.0 * Math.PI * value) / (windowWidth - 1)) -
                            0.0106411 * Math.Cos((6.0 * Math.PI * value) / (windowWidth - 1)) },
            { WindowFunction.Rectangular,
                (double value, int windowWidth, double additionalParameter) => 1.0 },
            { WindowFunction.Gauss,
                (double value, int windowWidth, double additionalParameter) =>
                {
                    double _sigma = (additionalParameter == 0.0) ? 0.4 : additionalParameter;
                    double _coeff = (value - ((windowWidth - 1) / 2.0)) / (_sigma * (windowWidth - 1) / 2.0);
                    return Math.Exp(-0.5 * Math.Pow(_coeff, 2));
                } },
            { WindowFunction.Tukey,
                (double value, int windowWidth, double additionalParameter) => throw new NotImplementedException("Tukey") },
            { WindowFunction.Hamming,
                (double value, int windowWidth, double additionalParameter) => 0.53836 -
                            0.46164 * Math.Cos((2.0 * Math.PI * value) / (windowWidth - 1)) },
            { WindowFunction.Hann,
                (double value, int windowWidth, double additionalParameter) => 0.5 * (1.0 - Math.Cos((2 * Math.PI * value) / (windowWidth - 1))) },
            { WindowFunction.Bartlett,
                (double value, int windowWidth, double additionalParameter) => (2.0 / (windowWidth - 1)) * (((windowWidth - 1) / 2.0) - Math.Abs(value - ((windowWidth - 1) / 2.0))) },
            { WindowFunction.Triangular,
                (double value, int windowWidth, double additionalParameter) => (2.0 / windowWidth) * ((windowWidth / 2.0) - Math.Abs(value - ((windowWidth - 1) / 2.0))) },
            { WindowFunction.BartlettHann,
                (double value, int windowWidth, double additionalParameter) => 0.62 -
                            0.48 * Math.Abs(1.0 * value / (windowWidth - 1) - 0.5) -
                            0.38 * Math.Cos((2.0 * Math.PI * value) / (windowWidth - 1)) },
            { WindowFunction.Blackman,
                (double value, int windowWidth, double additionalParameter) => 0.42 -
                            0.5 * Math.Cos((2.0 * Math.PI * value) / (windowWidth - 1)) +
                            0.08 * Math.Cos((4.0 * Math.PI * value) / (windowWidth - 1)) },
            { WindowFunction.Kaiser,
                (double value, int windowWidth, double additionalParameter) =>
                {
                        double _alpha = (additionalParameter == 0.0) ? 3.0 : additionalParameter;
                        double _piAlpha = Math.PI * _alpha;
                        double _bCoeff = Math.Sqrt(1.0 - Math.Pow((2.0 * value / (windowWidth - 1.0)) - 1.0, 2));
                        return AdditionalMathFunctions.BesselI0(_piAlpha * _bCoeff) / AdditionalMathFunctions.BesselI0(_piAlpha);
                } },
            { WindowFunction.Nuttall,
                (double value, int windowWidth, double additionalParameter) => 0.355768 -
                            0.487396 * Math.Cos((2.0 * Math.PI * value) / (windowWidth - 1)) +
                            0.144232 * Math.Cos((4.0 * Math.PI * value) / (windowWidth - 1)) -
                            0.012604 * Math.Cos((6.0 * Math.PI * value) / (windowWidth - 1)) },
            { WindowFunction.BlackmanHarris,
                (double value, int windowWidth, double additionalParameter) => 0.35875 -
                            0.48829 * Math.Cos((2.0 * Math.PI * value) / (windowWidth - 1)) +
                            0.14128 * Math.Cos((4.0 * Math.PI * value) / (windowWidth - 1)) -
                            0.01168 * Math.Cos((6.0 * Math.PI * value) / (windowWidth - 1)) },
            { WindowFunction.FlatTop,
                (double value, int windowWidth, double additionalParameter) => 1.0 -
                            1.93 * Math.Cos((2.0 * Math.PI * value) / (windowWidth - 1)) +
                            1.29 * Math.Cos((4.0 * Math.PI * value) / (windowWidth - 1)) -
                            0.388 * Math.Cos((6.0 * Math.PI * value) / (windowWidth - 1)) +
                            0.032 * Math.Cos((8.0 * Math.PI * value) / (windowWidth - 1)) },
            { WindowFunction.Poisson,
                (double value, int windowWidth, double additionalParameter) =>
            {
                //default time constant is to decay 60dB over half the Window width
                        double _tau = (additionalParameter <= 0.0) ? (windowWidth / 2.0 * 8.69 / 60.0) : additionalParameter;
                        double _coeffP = Math.Abs(value - (windowWidth - 1) / 2.0) / _tau;
                        return Math.Exp(-_coeffP);
            } },
            { WindowFunction.Exponential,
                (double value, int windowWidth, double additionalParameter) => throw new NotImplementedException("Exponential") },
            { WindowFunction.Lanczos,
                (double value, int windowWidth, double additionalParameter) => AdditionalMathFunctions.Sinc(2.0 * value / (windowWidth - 1) - 1.0) },

        };


        public void GenerateWindowFunction(WindowFunction windowFunction, double parameter = 0.0)
        {
            int windowWidth = _samples.Length;

            if (windowWidth < 2)
            {
                throw new ArgumentOutOfRangeException(nameof(_samples), "Width of Window function should be at least 2 samples.");
            }

            Func<double, int, double, double> selectedWindowFunction = _windowFunctions[windowFunction];

            for (int n = 0; n < windowWidth; n++)
            {
                _samples[n] = selectedWindowFunction(n, windowWidth, parameter);
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
