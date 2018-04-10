using Data.Annex.MathExtended.Matrices;
using System;

namespace Data.Annex.Signal.Processing
{
    public partial class Signal
    {
        private double[] CreateSavitzkyGolayKernel(int kernelLeft, int kernelRight, int polynomialOrder, int derivativeOrder)
        {
            int kernelSize = kernelLeft + kernelRight + 1;
            var result = new double[kernelSize];
            var _matrix = new Matrix(kernelSize, polynomialOrder + 1);
            for (int row = 0; row < kernelSize; row++)
            {
                for (int col = 0; col <= polynomialOrder; col++)
                    _matrix[row + 1, col + 1] = Math.Pow(row - kernelLeft, col);
            }
            var _m2 = _matrix.Duplicate();
            _matrix.Transpose();
            _matrix.Multiply(_m2);
            _matrix.Inverse();
            _m2.Transpose();
            _matrix.Multiply(_m2);
            for (int n = 0; n < kernelSize; n++)
                result[n] = _matrix[derivativeOrder + 1, n + 1];
            return result;
        }

        public void MovingAverage(int kernelSize = 5)
        {
            if (kernelSize % 2 != 1)
                throw new ArgumentException("Kernel size must be odd number.");
            _convolutionKernel = new double[kernelSize];
            for (int n = 0; n < kernelSize; n++) _convolutionKernel[n] = 1.0 / kernelSize;
            var _convolutionResult = Convolute((kernelSize - 1) / 2);
            _samples = _convolutionResult;
        }

        /// <summary>
        /// Symetric Savitzky-Golay Smoothing
        /// </summary>
        /// <param name="kernelSize">Length of kernel array, must be odd, left and right parts are of the same length</param>
        /// <param name="derivativeOrder"></param>
        /// <param name="polynomialOrder"></param>
        public void SavitzkyGolay(int kernelSize = 5, int polynomialOrder = 4, int derivativeOrder = 0)
        {
            if (kernelSize % 2 != 1)
                throw new ArgumentException("Kernel size must be odd number.");
            int _kernelOffset = (kernelSize - 1) / 2;
            _convolutionKernel = CreateSavitzkyGolayKernel(_kernelOffset, _kernelOffset, polynomialOrder, derivativeOrder);

            var _convolutionResult = Convolute((kernelSize - 1) / 2);
            _samples = _convolutionResult;
        }

        /// <summary>
        /// Asymetric Savitzky-Golay Smoothing
        /// </summary>
        /// <param name="kernelLeft"></param>
        /// <param name="kernelRight"></param>
        /// <param name="derivativeOrder"></param>
        /// <param name="polynomialOrder"></param>
        public void SavitzkyGolay(int kernelLeft, int kernelRight, int polynomialOrder, int derivativeOrder = 0)
        {
            if ((kernelLeft + kernelRight) % 2 == 1)
                throw new ArgumentException("Kernel size must be odd number.");
            _convolutionKernel = CreateSavitzkyGolayKernel(kernelLeft, kernelRight, polynomialOrder, derivativeOrder);

            var _convolutionResult = Convolute(kernelLeft);
            _samples = _convolutionResult;
        }

        public void SimpleExponentialSmoothing(double smoothingFactor)
        {
            if (smoothingFactor < 0 || smoothingFactor > 1.0)
                throw new ArgumentException("Smoothing factor must be between 0.0 and 1.0");
            int _signalLength = _samples.Length;
            var result = new double[_signalLength];
            double _previousValue = 0.0;
            double _coeff = 1.0 - smoothingFactor;

            for (int n = 0; n < _signalLength; n++)
            {
                result[n] = smoothingFactor * _samples[n] + _coeff * _previousValue;
                _previousValue = result[n];
            }
            _samples = result;
        }

        public void LinearExponentialSmoothing(double levelSmoothing, double trendSmoothing)
        {
            if (levelSmoothing < 0 || trendSmoothing < 0 || levelSmoothing > 1.0 || trendSmoothing > 1.0)
                throw new ArgumentException("Smoothing factors must be between 0.0 and 1.0");
            int _signalLength = _samples.Length;
            var result = new double[_signalLength];
            double _previousLevelValue = 0.0;
            double _previousTrendValue = 0.0;

            for (int n = 0; n < _signalLength; n++)
            {
                result[n] = levelSmoothing * _samples[n] + (1.0 - levelSmoothing) * (_previousLevelValue + _previousTrendValue);
                _previousTrendValue = trendSmoothing * (result[n] - _previousLevelValue) + (1.0 - trendSmoothing) * _previousTrendValue;
                _previousLevelValue = result[n];
            }
            _samples = result;
        }
    }
}
