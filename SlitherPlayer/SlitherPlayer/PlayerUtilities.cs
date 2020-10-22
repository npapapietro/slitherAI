using System;
using System.Collections.Generic;
using System.Text;

namespace SlitherPlayer
{
    public static class PlayerUtilities
    {
     
        public static int ToEpoch(this DateTime time)
        {
            var t = time - new DateTime(1970, 1, 1);
            return (int)t.TotalSeconds;
        }

        
    }
}
