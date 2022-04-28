using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JeuDeLaVie.Model
{
    /// <summary>
    /// Represents a position on the game board. Stores values as discrete values, but provides dynamic positioning attributes for variable display sizes.
    /// </summary>
    /// 
    [Serializable]
    public class Coordinate
    {

        /// <summary>
        /// Discrete column number.
        /// </summary>
        public int Col { get; private set; }
        /// <summary>
        /// Discrete row number.
        /// </summary>
        public int Row { get; private set; }
        /// <summary>
        /// Dynamic position (row) of the coordinate based on size of the bound object (tile).
        /// </summary>
        public int PosX { get => Col * TileWidth; }
        /// <summary>
        /// Dynamic position (column) of the coordinate based on the size of the bound object (tile).
        /// </summary>
        public int PosY { get => Row * TileWidth; }
        /// <summary>
        /// Variable width of the bound object (tile). Default: 15. Minimum value: 15.
        /// </summary>
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

        public Coordinate() { }

        /// <summary>
        /// Set the tile width modifier.
        /// </summary>
        /// <param name="width">New value for the tile width. Must be at least 15 (pixels).</param>
        public void SetTileWidth(int width)
        {
            if (width >= 15)
                TileWidth = width;
        }

        public override string ToString()
        {
            return $"({Col}, {Row})";
        }
    }
}
