using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Slither.Environment
{
    public class Coorindates
    {
        public const int N_slices = 6;
        public readonly int X;
        public readonly int Y;
        public readonly int R;

        /// <summary>
        /// The coordinates of the mouse position around a unit circle centered on the slither.
        /// </summary>
        public readonly (int, int)[] PositionMapping;

        public Coorindates(int X, int Y, int R=10)
        {
            this.X = X;
            this.Y = Y;
            this.R = R;

            PositionMapping = Enumerable.Range(0, N_slices)
            .Select(x => 2 * Math.PI * (double)x / (double)N_slices)
            .Select(x =>
            {
                var _x = (double)R * Math.Cos(x);
                var _y = (double)R * Math.Sin(x);
                return (
                    (int)(R * _x),
                    (int)(R * _y) // odd parity for y axis in html
                );
            })
            .ToArray();
        }   
    }
}