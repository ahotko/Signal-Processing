using Data.Annex.MathExtended.ComplexNumbers;
using System;

namespace Data.Annex.Signal.Processing
{
    public partial class Signal
    {
        public enum FourierTransformDirection
        {
            Forward,
            Inverse
        }

        public Complex[] DiscreteFourierTransform()
        {
            int _len = _samples.Length;
            var _result = new Complex[_len];
            double _f = 0.0;
            double _real = 0.0;
            double _imaginary = 0.0;

            for (int k = 0; k < _len; k++)
            {
                _real = 0.0;
                _imaginary = 0.0;
                for (int n = 0; n < _len; n++)
                {
                    _f = 2.0 * Math.PI * k * n / _len;
                    _real += _samples[n] * Math.Cos(_f);
                    _imaginary += _samples[n] * Math.Sin(_f);
                }
                _result[k] = new Complex(_real, -_imaginary);
            }
            return _result;
        }

        public Complex[] FastFourierTransform()
        {
            if (!IsLengthPowerOfTwo)
            {
                throw new ArgumentOutOfRangeException("FFT Length must be Power of Two!");
            }
            var _result = new Complex[_samples.Length];
            int _length = _samples.Length;
            //copy input to output 
            for (int n = 0; n < _samples.Length; n++) _result[n] = new Complex(_samples[n]);
            //rearrange input sequence - fft butterfly (bit reverse)
            //Example (8 points = 3 bits -> 2^3 = 8)
            //input order  => 0 1 2 3 4 5 6 7
            //output order => 0 4 2 6 1 5 3 7
            //Index   Index-binary (input)   Index-binary (output)
            //----------------------------------------------------
            //  0         000                    000
            //  1         001                    100
            //  2         010                    010
            //  3         011                    110
            //  4         100                    001
            //  5         101                    101
            //  6         110                    011
            //  7         111                    111
            int i = 1;
            for (int n = 1; n < _length - 1; n++)
            {
                if (n < i)
                {
                    var _tmp = _result[i - 1];
                    _result[i - 1] = _result[n - 1];
                    _result[n - 1] = _tmp;
                }
                int _half = _length >> 1;
                while (_half < i)
                {
                    i -= _half;
                    _half >>= 1;
                }
                i += _half;
            }
            //step count; while-loop is faster than log2(n)
            int _stepCount = 0;
            int _step = _length;
            while (_step > 1)
            {
                _step >>= 1;
                _stepCount++;
            }
            //FFT
            for (int n = 0; n < _stepCount; n++)
            {
                int _s = 1 << n;
                double _sr = 1.0;
                double _si = 0.0;
                double _wr = Math.Cos(Math.PI / _s);
                double _wi = -Math.Sin(Math.PI / _s);
                for (int m = 0; m < _s; m++)
                {
                    int _k = m;
                    while (_k < _length)
                    {
                        Complex _tmp = Complex.Zero;
                        _tmp.Real = _result[_k + _s].Real * _sr - _result[_k + _s].Imaginary * _si;
                        _tmp.Imaginary = _result[_k + _s].Imaginary * _sr + _result[_k + _s].Real * _si;
                        _result[_k + _s].Real = _result[_k].Real - _tmp.Real;
                        _result[_k + _s].Imaginary = _result[_k].Imaginary - _tmp.Imaginary;
                        _result[_k].Real = _result[_k].Real + _tmp.Real;
                        _result[_k].Imaginary = _result[_k].Imaginary + _tmp.Imaginary;
                        _k += (_s << 1);
                    }
                    double _tmpd = _sr * _wr - _si * _wi;
                    _si = _si * _wr + _sr * _wi;
                    _sr = _tmpd;
                }
            }
            return _result;
        }

        public Complex[] FourierTransform()
        {
            if (IsLengthPowerOfTwo)
            {
                return FastFourierTransform();
            }
            else
            {
                return DiscreteFourierTransform();
            }
        }

        public double[] InverseDFT()
        {
            return null;
        }

        public double[] InverseFFT()
        {
            return null;
        }
    }
}
