using CommandLine;


namespace Arkod.Raspberry.Sample
{
    [Verb("fdd", HelpText = "Afficheur 4 x 7 segments")]
    public class FourDigitsDisplayOptions
    {
        [Value(0, Required = true)]
        public string Operation { get; set; }

        [Value(1)]
        public string Parameter { get; set; }
    }
}
