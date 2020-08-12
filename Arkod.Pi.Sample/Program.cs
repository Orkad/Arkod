using System;
using Unosquare.RaspberryIO;

namespace Arkod
{
    class Program
    {
        static void Main(string[] args)
        {
            Pi.Init<BootstrapWiringPi>();
        }
    }
}
