using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JeuDeLaVie.Model
{
    internal class Coordinate
    {

        public int Col { get; private set; }
        public int Row { get; private set; }
        public int PosX { get => Col * TileWidth; }
        public int PosY { get => Row * TileWidth; }
        public int TileWidth { get; private set; }

        /// <summary>
        /// Creates a Coordinate object that represents a location on a canvas control. Default tile width of 15px.
        /// </summary>
        /// <param name="x">x position</param>
        /// <param name="y">y position</param>
        public Coordinate(int x, int y)
        {
            Col = x;
            Row = y;
            TileWidth = 15;
        }

        /// <summary>
        /// Set the tile width modifier.
        /// </summary>
        /// <param name="width">New value for the tile width. Must be greater than 15 (pixels).</param>
        public void SetTileWidth(int width)
        {
            if (width > 15)
                TileWidth = width;
        }

        public override string ToString()
        {
            return $"({Col}, {Row})";
        }
    }
}
