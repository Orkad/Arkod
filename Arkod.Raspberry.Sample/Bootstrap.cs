using Unosquare.RaspberryIO;
using Unosquare.WiringPi;


namespace Arkod.Raspberry.Sample
{
    public static class Bootstrap
    {
        private static bool initialized = false;
        private static object _lock = new object();
        public static void Raspberry()
        {
            if (!initialized)
            {
                lock (_lock)
                {
                    if (!initialized)
                    {
                        Pi.Init<BootstrapWiringPi>();
                    }
                }
            }
            
        }
    }
}
