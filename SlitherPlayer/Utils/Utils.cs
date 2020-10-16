using System;

namespace Slither.Utils
{
    public static class Utils
    {        
        public static int ToEpoch(this DateTime time)
        {
            var t = time - new DateTime(1970, 1, 1);
            return (int)t.TotalSeconds;
        }

    }
}