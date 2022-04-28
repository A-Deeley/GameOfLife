using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace JeuDeLaVie.Model
{
    [Serializable]
    public class GameState
    {
        public int CanvasWidth { get; set; }
        public int CanvasHeight { get; set; }
        public List<LifeForm> LifeForms { get; set; }
    }
}