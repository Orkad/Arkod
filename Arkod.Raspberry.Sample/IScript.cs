using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Arkod.Raspberry.Sample
{
    interface IScript
    {
        /// <summary>
        /// Initialize your stuff here
        /// </summary>
        void Setup();

        /// <summary>
        /// Main loop of your script
        /// </summary>
        void Loop();

        /// <summary>
        /// Cleanup your stuff here
        /// </summary>
        void Cleanup();
    }

    class ScriptRunner
    {
        bool run = true;
        IScript script;

        /// <summary>
        /// Perform script run 
        /// </summary>
        /// <param name="script">script to run</param>
        public void Run(IScript script)
        {
            run = true;
            this.script = script;
            var currentScriptRun = new Thread(new ThreadStart(InternalRun));
            currentScriptRun.Start();
        }

        public void Stop()
        {
            run = false;
            script.Cleanup();
            Console.WriteLine("script " + script.GetType() + " cleaned up...");
        }

        private void InternalRun()
        {
            Console.WriteLine("script " + script.GetType() + " running...");
            script.Setup();
            while (run)
            {
                script.Loop();
            }
        }
    }
}
