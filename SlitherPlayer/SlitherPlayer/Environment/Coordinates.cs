using Microsoft.VisualBasic.CompilerServices;
using Slither.Proto;
using System;
using System.Linq;

namespace SlitherPlayer.Environment
{
    public class Coordinates : ICoordinates
    {
        /// <summary>
        /// Total number of move choices
        /// </summary>
        private readonly int N_slices = Enum.GetNames(typeof(Moves)).Length;


        public Coordinates(int N_slices, int R = 100): this(R)
        {
            this.N_slices = N_slices;            
        }

        public (int, int)[] PositionMapping { get; }

        public Coordinates(int R = 100)
        {
            // Since N_slices includes DoNothing, we don't need to +1 to make it inclusive range
            var angles = Enumerable.Range(0, N_slices).Select(t => (double)t / (N_slices - 1));
            PositionMapping = angles.Select(theta =>
            {
                //var theta = (double)t / N_slices;
                //Console.WriteLine($"{t}/{N_slices}");
                var _x = R * Math.Cos(2d * Math.PI * theta);
                var _y = R * Math.Sin(2d * Math.PI * theta);
                return (
                    Convert.ToInt32(_x),
                    -Convert.ToInt32(_y) // odd parity for y axis in html
                );
            })
            .ToArray();
        }
    }
}