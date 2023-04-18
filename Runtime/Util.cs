using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace StringSDK
{
    // Usage: await Util.WaitUntil(() => condition == true);
    public static class Util
    {
        public static async Task WaitUntil(Func<bool> condition, int frequency = 25, int timeout = -1)
        {
            var waitTask = Task.Run(async () =>
            {
                while (!condition()) await Task.Delay(frequency);
            });

            if (waitTask != await Task.WhenAny(waitTask, 
                    Task.Delay(timeout))) 
                throw new TimeoutException();
        }
    }
}