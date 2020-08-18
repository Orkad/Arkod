using CommandLine;


namespace Arkod.Raspberry.Sample
{
    [Verb("blink", HelpText = "Blink a led")]
    public class BlinkOptions
    {
        [Option('t', "times", Default = 20, HelpText = "how many times the led should blink")]
        public int Times { get; set; }

        [Option('g', "gpio", Default = 17, HelpText = "GPIO Pin where the led is wired")]
        public int GpioPin { get; set; }

        [Option('f', "frequency", Default = 500, HelpText = "Frequency in millisecond (ms) the led should blink")]
        public int Frequency { get; set; }
    }
}
