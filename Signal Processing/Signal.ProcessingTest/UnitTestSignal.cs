using Data.Annex.MathExtended.ComplexNumbers;
using Data.Annex.Signal.Processing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SignalProcessingTest
{
    [TestClass]
    public class UnitTestSignal
    {
        [TestMethod]
        public void SignalResizeNextPowerOfTwo()
        {
            var a = new Signal(100);
            a.ResizeToNextPowerOfTwo();

            Assert.AreEqual(128, a.Count, "Signal length not correct!");
        }

        [TestMethod]
        public void SignalLengthIsPowerOfTwo()
        {
            var a = new Signal(100);
            var b = new Signal(128);
            var c = new Signal(150);
            var d = new Signal(1024);

            Assert.IsFalse(a.IsLengthPowerOfTwo, "Signal length not power of two!");
            Assert.IsTrue(b.IsLengthPowerOfTwo, "Signal length is power of two!");
            Assert.IsFalse(c.IsLengthPowerOfTwo, "Signal length not power of two!");
            Assert.IsTrue(d.IsLengthPowerOfTwo, "Signal length is power of two!");
        }

        [TestMethod]
        public void SignalResizeNearestPowerOfTwo()
        {
            var a = new Signal(70);
            var b = new Signal(256);
            var c = new Signal(100);

            a.ResizeToNearestPowerOfTwo();
            b.ResizeToNearestPowerOfTwo();
            c.ResizeToNearestPowerOfTwo();

            Assert.AreEqual(64, a.Count, "Signal A length not correct!");
            Assert.AreEqual(256, b.Count, "Signal B length not correct!");
            Assert.AreEqual(128, c.Count, "Signal C length not correct!");
        }

        [TestMethod]
        public void SignalDiscreteFourier()
        {
            var _signal = new Signal(4);
            _signal[0] = 1.0;
            _signal[1] = 2.0;
            _signal[2] = 2.0;
            _signal[3] = 1.0;
            var _transformedSignal = _signal.DiscreteFourierTransform();
            var _trueValues = new Complex[4] {
            new Complex(6, 0),
            new Complex(-1, -1),
            new Complex(0, 0),
            new Complex(-1, 1)};

            CollectionAssert.AreEqual(_trueValues, _transformedSignal, "Discrete Fourier Transform of Input signal Invalid!");
        }

        [TestMethod]
        public void SignalFastFourier()
        {
            var _signal = new Signal(4);
            _signal[0] = 1.0;
            _signal[1] = 2.0;
            _signal[2] = 2.0;
            _signal[3] = 1.0;
            var _transformedSignal = _signal.FastFourierTransform();
            var _trueValues = new Complex[4] {
            new Complex(6, 0),
            new Complex(-1, -1),
            new Complex(0, 0),
            new Complex(-1, 1)};

            CollectionAssert.AreEqual(_trueValues, _transformedSignal, "Fast Fourier Transform of Input signal Invalid!");
        }
    }
}
