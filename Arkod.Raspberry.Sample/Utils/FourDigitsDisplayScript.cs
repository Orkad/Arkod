using Arkod.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Abstractions;

namespace Arkod.Raspberry.Sample.Utils
{
    public sealed class FourDigitsDisplayScript : IScript
    {
        /// <summary>
        /// digits to display
        /// </summary>
        private readonly byte[] digits = new byte[4];

        private readonly byte a = 13;
        private readonly byte b = 5;
        private readonly byte c = 26;
        private readonly byte d = 16;
        private readonly byte e = 20;
        private readonly byte f = 6;
        private readonly byte g = 19;
        private readonly byte dp = 12;
        private readonly byte d1 = 22;
        private readonly byte d2 = 27;
        private readonly byte d3 = 18;
        private readonly byte d4 = 17;

        public void Setup()
        {
            ConfigureGpio(a, b, c, d, e, f, g, dp, d1, d2, d3, d4);
        }

        public void Loop()
        {
            for (byte i = 0; i < 4; i++)
            {
                SelectSegment(byte.MaxValue); // unselect the displayed segment
                WriteDigit(digits[i]); // prepare the digit we want to show
                SelectSegment(i); // select the displayed segment
                Pi.Timing.SleepMilliseconds(1); // wait 1 ms
            }
        }

        public void Cleanup()
        {
            WriteDigit(byte.MaxValue);
            SelectSegment(byte.MaxValue);
        }

        public void Write(int number)
        {
            var currentDigits = number.GetDigits();
            for (int i = 0; i < 4; i++)
            {
                digits[i] = i < currentDigits.Length ? currentDigits[i] : byte.MaxValue;
            }
        }

        /// <summary>
        /// Write digit on the display
        /// </summary>
        /// <param name="digit">digit 0 - 9 to display</param>
        /// <param name="dot">display a dot on the current segment</param>
        private void WriteDigit(byte digit, bool dot = false)
        {
            Pi.Gpio[dp].Write(!dot);
            switch (digit)
            {
                case 0: ShowSegments(a, b, c, d, e, f); HideSegments(g); break;
                case 1: ShowSegments(b, c); HideSegments(a, d, e, f, g); break;
                case 2: ShowSegments(a, b, d, e, g); HideSegments(c, f); break;
                case 3: ShowSegments(a, b, c, d, g); HideSegments(e, f); break;
                case 4: ShowSegments(b, c, g, f); HideSegments(a, d, e); break;
                case 5: ShowSegments(a, c, d, f, g); HideSegments(b, e); break;
                case 6: ShowSegments(a, c, d, e, f, g); HideSegments(b); break;
                case 7: ShowSegments(a, b, c); HideSegments(d, e, f, g); break;
                case 8: ShowSegments(a, b, c, d, e, f, g); break;
                case 9: ShowSegments(a, b, c, d, f, g); HideSegments(e); break;
                default: HideSegments(a, b, c, d, e, f, g); break;
            }
        }

        private void SelectSegment(byte index)
        {
            Pi.Gpio[d1].Write(index == 0);
            Pi.Gpio[d2].Write(index == 1);
            Pi.Gpio[d3].Write(index == 2);
            Pi.Gpio[d4].Write(index == 3);
        }

        private void ShowSegments(params byte[] segments)
        {
            segments.ToList().ForEach(s => Pi.Gpio[s].Write(false));
        }

        private void HideSegments(params byte[] segments)
        {
            segments.ToList().ForEach(s => Pi.Gpio[s].Write(true));
        }

        private void ConfigureGpio(params byte[] gpio)
        {
            gpio.ToList().ForEach(g => Pi.Gpio[g].PinMode = GpioPinDriveMode.Output);
        }
    }
}
