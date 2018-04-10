using System;
using System.Linq;

namespace Data.Annex.Signal.Processing
{
    public partial class Signal
    {
        public void Zero()
        {
            Array.Clear(_samples, 0, _samples.Length);
        }

        public void DiracDelta(int offset = 0)
        {
            Array.Clear(_samples, 0, _samples.Length);
            _samples[offset] = 1.0;
        }

        public void GenerateDCSignal(double amplitude)
        {
            for (int n = 0; n < _samples.Length; n++)
            {
                _samples[n] = amplitude;
            }
        }

        public void GenerateSine(double[] frequencies, double[] amplitudes, double[] phases)
        {
            if ((frequencies.Length != amplitudes.Length)
                || (frequencies.Length != phases.Length)
                || (frequencies.Length < 1)
                || (amplitudes.Length < 1)
                || (phases.Length < 1))
            {
                throw new ArgumentException("Invalid parameters!");
            }
            for (int cnt = 0; cnt < _samples.Length; cnt++)
            {
                double _value = 0.0;
                for (int n = 0; n < frequencies.Length; n++)
                {
                    _value += amplitudes[n] * Math.Sin(phases[n] + 2.0 * Math.PI * frequencies[n] * cnt / this._samplingFrequency);
                }
                _samples[cnt] = _value;
            }
        }

        public void GenerateSine(double[] frequencies, double[] amplitudes)
        {
            this.GenerateSine(frequencies, amplitudes, Enumerable.Repeat(0.0, frequencies.Length).ToArray());
        }

        public void GenerateSine(double frequency, double amplitude)
        {
            this.GenerateSine(new double[] { frequency }, new double[] { amplitude }, new double[] { 0.0 });
        }

        public void GenerateSine(double frequency, double amplitude, double phase)
        {
            this.GenerateSine(new double[] { frequency }, new double[] { amplitude }, new double[] { phase });
        }
    }
}
