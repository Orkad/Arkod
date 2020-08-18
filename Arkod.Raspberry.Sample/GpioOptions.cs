using CommandLine;


namespace Arkod.Raspberry.Sample
{
    [Verb("gpio", HelpText = "operation on gpio pin")]
    public class GpioOptions
    {
        [Value(0, Required = true, HelpText = "Gpio pin number")]
        public int GpioPin { get; set; }

        [Value(1, Default = "read", HelpText = "operation on gpio (read, on, off)")]
        public string Operation { get; set; }
    }
}
