using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Abstractions;
using Unosquare.WiringPi;

namespace Arkod.Raspberry.Sample
{
    public class Program
    {
        [Verb("blink", HelpText = "Blink a led")]
        public class BlinkOptions
        {
            
        }

        public static int Main(string[] args)
        {
#if DEBUG
            args = new[] { " --help" };
#endif

            try
            {
                Pi.Init<BootstrapWiringPi>();
            }
            catch (DllNotFoundException)
            {
                Console.WriteLine("L'environnement n'est pas compatible");
                return 1;
            }
            var result = Parser.Default.ParseArguments<BlinkOptions>(args);
            result.WithParsed<BlinkOptions>(Blink);
            return 1;
        }

        public static void Blink(BlinkOptions opts)
        {
            var pin = Pi.Gpio[17];
            pin.PinMode = GpioPinDriveMode.Output;

            // perform writes to the pin by toggling the isOn variable
            var on = false;
            for (var i = 0; i < 20; i++)
            {
                on = !on;
                pin.Write(on);
                System.Threading.Thread.Sleep(500);
            }
        }
    }
}
