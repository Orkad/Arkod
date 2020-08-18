using Arkod.Raspberry.Sample.Utils;
using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using System.Threading;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Abstractions;


namespace Arkod.Raspberry.Sample
{

    public class Program
    {
        static List<ScriptRunner> ScriptRunners = new List<ScriptRunner>();
        public static int Main(string[] args)
        {
#if DEBUG
            args = new[] { "blink", "--help" };
            args = new[] { "gpio", "17" };
#endif
            Console.CancelKeyPress += Console_CancelKeyPress;
            Parser.Default.ParseArguments<BlinkOptions, GpioOptions, GpioInfoOptions, FourDigitsDisplayOptions>(args)
                .WithParsed<BlinkOptions>(Blink)
                .WithParsed<GpioOptions>(Gpio)
                .WithParsed<FourDigitsDisplayOptions>(RunFourDigitsDisplay)
                .WithParsed<GpioInfoOptions>(GpioInfo);
            return 1;
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            // empty
            Console.WriteLine("canceled");
            ScriptRunners.ForEach(sr => sr.Stop());
        }

        private static void RunFourDigitsDisplay(FourDigitsDisplayOptions options)
        {
            Console.WriteLine("Four digits display (3461BS-1) ...");
            Console.WriteLine($"operation: {options.Operation}");
            Console.WriteLine($"parameter: {options.Parameter}");
            Bootstrap.Raspberry();
            var scriptRunner = new ScriptRunner();
            ScriptRunners.Add(scriptRunner);
            var script = new FourDigitsDisplayScript();
            scriptRunner.Run(script);
            switch (options.Operation)
            {
                case "number":
                    script.Write(int.Parse(options.Parameter));
                    Console.ReadKey();
                    break;
                case "counter":
                    int i = 0;
                    while (true)
                    {
                        script.Write(++i);
                        Console.ReadKey();
                    }
                case "clock":
                    while (true)
                    {
                        var now = DateTime.Now;
                        int minute = now.Minute;
                        int hour = now.Hour;
                        int number = hour * 100 + minute;
                        script.Write(number);
                        Thread.Sleep(100);
                    }
                    break;
                default:
                    break;
            }
        }

        public static void Blink(BlinkOptions opts)
        {
            Console.WriteLine("Blinking...");
            Bootstrap.Raspberry();
            var pin = Pi.Gpio[opts.GpioPin];
            pin.PinMode = GpioPinDriveMode.Output;
            var on = false;
            for (var i = 0; i < opts.Times * 2; i++)
            {
                on = !on;
                pin.Write(on);
                Thread.Sleep(opts.Frequency);
            }
        }

        public static void Gpio(GpioOptions options)
        {
            Console.WriteLine("Gpio operation...");
            Console.WriteLine($"pin number: {options.GpioPin}");
            Console.WriteLine($"operation: {options.Operation}");
            Bootstrap.Raspberry();
            var pin = Pi.Gpio[options.GpioPin];
            switch (options.Operation)
            {
                case "read":
                    pin.PinMode = GpioPinDriveMode.Input;
                    Console.WriteLine(pin.Read());
                    break;
                case "on":
                    pin.PinMode = GpioPinDriveMode.Output;
                    pin.Write(true);
                    break;
                case "off":
                    pin.PinMode = GpioPinDriveMode.Output;
                    pin.Write(false);
                    break;
                default:
                    break;
            }
        }

        public static void GpioInfo(GpioInfoOptions opt)
        {
            Console.WriteLine("Gpio informations display...");

        }
    }
}
