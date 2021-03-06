using JeuDeLaVie.View;
using RelayCommandLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace JeuDeLaVie.ViewModel
{
    /// <summary>
    /// Viewmodel serving the canvas creation view. Handles verification and initialisation of the game view & logic.
    /// </summary>
    internal class VM_CreateCanvas : VM_Base
    {
        private int? _width;
        private int? _height;
        private string _errWidth;
        private string _errHeight;
        private Brush _borderBrushWidth;
        private Brush _borderBrushHeight;

        /// <summary>
        /// Input for the width in tiles of the game board.
        /// </summary>
        public string Width
        {
            get => _width.ToString();
            set
            {
                if (int.TryParse(value, out int valueInt) && valueInt > 0)
                {
                    ErrWidth = string.Empty;
                    BorderBrushWidth = Brushes.Green;
                    _width = valueInt;
                    OnPropertyChanged();
                }
                else
                {
                    _width = null;
                    BorderBrushWidth = Brushes.Red;
                }
            }
        }
        /// <summary>
        /// Input for the height in tiles of the game board.
        /// </summary>
        public string Height
        {
            get => _height.ToString();
            set
            {
                if (int.TryParse(value, out int valueInt) && valueInt > 0)
                {
                    ErrHeight = string.Empty;
                    BorderBrushHeight = Brushes.Green;
                    _height = valueInt;
                    OnPropertyChanged();
                }
                else
                {
                    _height = null;
                    BorderBrushHeight = Brushes.Red;
                }
            }
        }
        /// <summary>
        /// Error message to display depending on validation parameters.
        /// </summary>
        public string ErrWidth
        {
            get => _errWidth;
            set { _errWidth = value; OnPropertyChanged(); }
        }
        /// <summary>
        /// Error message to display depending on validation parameters.
        /// </summary>
        public string ErrHeight
        {
            get => _errHeight;
            set { _errHeight = value; OnPropertyChanged(); }
        }
        /// <summary>
        /// Colour brush for the width input textbox (red for error, black normally).
        /// </summary>
        public Brush BorderBrushWidth
        {
            get => _borderBrushWidth;
            set { _borderBrushWidth = value; OnPropertyChanged(); }
        }
        /// <summary>
        /// Colour brush for the height input textbox (red for error, black normally).
        /// </summary>
        public Brush BorderBrushHeight
        {
            get => _borderBrushHeight;
            set { _borderBrushHeight = value; OnPropertyChanged(); }
        }

        public RelayCommand CreateCanvas { get; private set; }
        /// <summary>
        /// Creates an instance of the viewmodel for the canvas creation.
        /// </summary>
        /// <param name="window"></param>
        public VM_CreateCanvas(Window window)
        {
            CreateCanvas = new(exec =>
            {
                Window game = new GameOfLife();
                double tileSize = game.Width * 0.6 / (int)_width;
                if (tileSize < 15)
                    tileSize = 15;
                game.Height = tileSize * (int)_height;
                game.Width = game.Height / 0.6;
                var dt = new VM_Game
                {
                    CanvasWidthPx = (int)_width * tileSize,
                    CanvasHeightPx = (int)_height * tileSize,
                    CanvasTileSizePx = tileSize
                };
                dt.Init();
                game.DataContext = dt;
                game.Show();
                window.Close();
            },
            canExec => 
            {
                return !string.IsNullOrEmpty(Width) && !string.IsNullOrEmpty(Height) && _width > 0 && _height > 0;
            });
            BorderBrushHeight = Brushes.Black;
            BorderBrushWidth = Brushes.Black;
        }
    }
}
