using System;

namespace Data.Annex.Signal.Processing
{
    public partial class Signal
    {
        private double[] _samples;
        private double[] _convolutionKernel;

        private double _samplingFrequency = 0.0;

        private double[] Convolute(int kernelCenterIndex)
        {
            int _signalLength = _samples.Length;
            int _kernelLength = _convolutionKernel.Length;
            int _kernelOffsetLeft = kernelCenterIndex;
            int _kernelOffsetRight = _kernelLength - _kernelOffsetLeft - 1;
            var result = new double[_signalLength];
            for (int n = 0; n < _signalLength; n++)
            {
                double sum = 0.0;
                for (int k = 0; k < _kernelLength; k++)
                {
                    int index = n + k - _kernelOffsetLeft;
                    if (index >= 0 && index < _signalLength)
                        sum += (_samples[index] * _convolutionKernel[k]);
                }
                result[n] = sum;
            }
            return result;
        }

        private double GetMaxValue()
        {
            int _signalLength = _samples.Length;
            double _result = _samples[0];
            for (int n = 0; n < _signalLength; n++)
            {
                if (_samples[n] > _result) _result = _samples[n];
            }
            return _result;
        }

        private double GetMinValue()
        {
            int _signalLength = _samples.Length;
            double _result = _samples[0];
            for (int n = 0; n < _signalLength; n++)
            {
                if (_samples[n] < _result) _result = _samples[n];
            }
            return _result;
        }

        private double GetRootMeanSquare()
        {
            int _signalLength = _samples.Length;
            double _result = 0.0;
            for (int n = 0; n < _signalLength; n++)
            {
                _result += Math.Pow(_samples[n], 2);
            }
            return Math.Sqrt(_result / _signalLength);
        }

        public double this[int n]
        {
            get
            {
                return _samples[n];
            }

            set
            {
                _samples[n] = value;
            }
        }

        public bool IsLengthPowerOfTwo
        {
            get
            {
                uint length = (uint)_samples.Length;
                return (length == 1) || ((length & (length - 1)) == 0);
            }
        }

        public int Count { get { return _samples.GetLength(0); } }
        public double MaxValue { get { return GetMaxValue(); } }
        public double MinValue { get { return GetMinValue(); } }
        public double Amplitude { get { return GetMaxValue() - GetMinValue(); } }
        public double RMS { get { return GetRootMeanSquare(); } }

        public double SamplingFrequency
        {
            get { return _samplingFrequency; }
            set { _samplingFrequency = value; }
        }

        public Signal(int length)
        {
            _samples = new double[length];
        }

        public void ResizeToNextPowerOfTwo()
        {
            int _oldLen = _samples.Length;
            double _lenLog = Math.Ceiling(Math.Log(_oldLen, 2));
            int _newLen = Convert.ToInt32(Math.Pow(2, _lenLog));
            Array.Resize<double>(ref _samples, _newLen);
        }

        public void ResizeToNearestPowerOfTwo()
        {
            int _oldLen = _samples.Length;
            int _lenDown = Convert.ToInt32(Math.Pow(2, Math.Floor(Math.Log(_oldLen, 2))));
            int _lenUp = Convert.ToInt32(Math.Pow(2, Math.Ceiling(Math.Log(_oldLen, 2))));
            int _newLen = (_oldLen - _lenDown) > (_lenUp - _oldLen) ? _lenUp : _lenDown;
            Array.Resize<double>(ref _samples, _newLen);
        }

        public void FirstDerivative()
        {
            _convolutionKernel = new double[3] { -0.5, 0.0, 0.5 };
            var _convolutionResult = Convolute(1);
            _samples = _convolutionResult;
        }

        public void SecondDerivative()
        {
            _convolutionKernel = new double[3] { 1.0, -2.0, 1.0 };
            var _convolutionResult = Convolute(1);
            _samples = _convolutionResult;
        }

        public void Offset(double offset)
        {
            for (int n = 0; n < Count; n++) _samples[n] += offset;
        }

        public void Apmlify(double factor)
        {
            for (int n = 0; n < Count; n++) _samples[n] *= factor;
        }

        public void CrossCorrelation(Signal signal)
        {

        }

        public void Resize(int newCount)
        {
            if (newCount <= 0)
                throw new ArgumentOutOfRangeException("Sample Count must be larger than 0.");
            Array.Resize(ref _samples, newCount);
        }

        public void Clear()
        {
            Array.Resize(ref _samples, 0);
        }

        public void Add(double value)
        {
            Array.Resize(ref _samples, _samples.Length + 1);
            _samples[_samples.Length - 1] = value;
        }

        public Signal Duplicate()
        {
            int _length = Count;
            var _result = new Signal(_length);
            for (int n = 0; n < _length; n++) _result[n] = _samples[n];
            return _result;
        }

        #region Overloaded operators
        /// <summary>
        /// Sum signals together. Resulting length is the greatest of input signals. Shorter signal
        /// is repeated
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Signal operator +(Signal first, Signal second)
        {
            int _firstLength = first.Count;
            int _secondLength = second.Count;
            var _result = new Signal(_firstLength > _secondLength ? _firstLength : _secondLength);
            int _resultLength = _result.Count;
            for (int n = 0; n < _resultLength; n++)
            {
                _result[n] = first[n % _firstLength] + second[n % _secondLength];
            }
            return first;
        }

        /// <summary>
        /// Signal convolution
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Signal operator *(Signal first, Signal second)
        {
            var _result = first.Duplicate();
            return _result;
        }
        #endregion
    }
}
