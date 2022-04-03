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
    internal class VM_CreateCanvas : VM_Base
    {
        private int _inputWidth;
        private int _inputHeight;
        private int _width;
        private int _height;
        private string _errWidth;
        private string _errHeight;
        private Brush _borderBrushWidth;
        private Brush _borderBrushHeight;

        public string Width
        {
            get => _width.ToString();
            set
            {
                if (int.TryParse(value, out int valueInt))
                {
                    ErrWidth = string.Empty;
                    BorderBrushWidth = Brushes.Black;
                    _width = valueInt;
                    OnPropertyChanged();
                }
                else
                {
                    ErrWidth = "Pas un entier.";
                    BorderBrushWidth = Brushes.Red;
                }
            }
        }
        public string Height
        {
            get => _height.ToString();
            set
            {
                if (int.TryParse(value, out int valueInt))
                {
                    ErrHeight = string.Empty;
                    BorderBrushHeight = Brushes.Black;
                    _height = valueInt;
                    OnPropertyChanged();
                }
                else
                {
                    ErrHeight = "Pas un entier.";
                    BorderBrushHeight = Brushes.Red;
                }
            }
        }
        public string ErrWidth
        {
            get => _errWidth;
            set { _errWidth = value; OnPropertyChanged(); }
        }

        public string ErrHeight
        {
            get => _errHeight;
            set { _errHeight = value; OnPropertyChanged(); }
        }

        public Brush BorderBrushWidth
        {
            get => _borderBrushWidth;
            set { _borderBrushWidth = value; OnPropertyChanged(); }
        }

        public Brush BorderBrushHeight
        {
            get => _borderBrushHeight;
            set { _borderBrushHeight = value; OnPropertyChanged(); }
        }

        public Visibility ErrorWidthVisible
        {
            get => string.IsNullOrEmpty(ErrWidth) ? Visibility.Collapsed : Visibility.Visible;
        }

        public Visibility ErrorHeightVisible
        {
            get => string.IsNullOrEmpty(ErrHeight) ? Visibility.Collapsed : Visibility.Visible;
        }

        public RelayCommand CreateCanvas { get; private set; }

        public VM_CreateCanvas()
        {
            CreateCanvas = new(exec => { }, canExec => (_width is <= 30 and >= 10) && (_height is <= 30 and >= 10));
                var tileSize = ((game.Width * 0.6) - 15) / _width;
                if (tileSize < 15)
                    tileSize = 15;
                game.Height = tileSize * _height + 30;
                game.Width = game.Height / 0.6;
                game.DataContext = new VM_Game(_width, _height, tileSize);
            });
            BorderBrushHeight = Brushes.Black;
            BorderBrushWidth = Brushes.Black;
        }
    }
}
