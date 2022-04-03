using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JeuDeLaVie.Model
{
    internal class LifeForm
    {
        public bool IsAlive { get; set; }
        public Coordinate Coord { get; private set; }

        /// <summary>
        /// Create a LifeForm object with default tile width.
        /// </summary>
        /// <param name="x">x position of the form</param>
        /// <param name="y">y position of the form</param>
        public LifeForm(int x, int y)
        {
            IsAlive = false;
            Coord = new Coordinate(x, y);
        }

        /// <summary>
        /// Create a LifeForm object with a specific tile width.
        /// </summary>
        /// <param name="x">x position of the form.</param>
        /// <param name="y">y position of the form.</param>
        /// <param name="tileWidth">Tile width to set. Must be greater than 15.</param>
        public LifeForm(int x, int y, int tileWidth)
            :this(x, y)
        {
            Coord.SetTileWidth(tileWidth);
        }
    }
}
