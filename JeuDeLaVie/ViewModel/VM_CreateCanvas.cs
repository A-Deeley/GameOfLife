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
            get => _inputWidth.ToString();
            set
            {
                if (!string.IsNullOrEmpty(value) && int.TryParse(value, out int valueInt))
                {
                    switch (valueInt)
                    {
                        case > 30:
                            {
                                ErrWidth = $"Trop grand (> 30)";
                                BorderBrushWidth = Brushes.Red;
                                break;
                            }
                        case < 10:
                            {
                                ErrWidth = $"Trop petit (< 10)";
                                BorderBrushWidth = Brushes.Red;
                                break;
                            }
                        default:
                            {
                                BorderBrushWidth = Brushes.Black;
                                ErrWidth = string.Empty;
                                _width = valueInt;
                                break;
                            }
                    }
                    _inputWidth = valueInt;
                    OnPropertyChanged();
                }
                else
                {
                    ErrWidth = $"Valeur pas un entier.";
                    BorderBrushWidth = Brushes.Red;
                }
            }
        }

        public string Height
        {
            get => _inputHeight.ToString();
            set
            {
                if (!string.IsNullOrEmpty(value) && int.TryParse(value, out int valueInt))
                {
                    switch (valueInt)
                    {
                        case > 30:
                            {
                                ErrHeight = $"Trop grand (> 30)";
                                BorderBrushHeight = Brushes.Red;
                                break;
                            }
                        case < 10:
                            {
                                ErrHeight = $"Trop petit (< 10)";
                                BorderBrushHeight = Brushes.Red;
                                break;
                            }
                        default:
                            {
                                BorderBrushHeight = Brushes.Black;
                                ErrHeight = string.Empty;
                                _height = valueInt;
                                break;
                            }
                    }
                    _inputHeight = valueInt;
                    OnPropertyChanged();
                }
                else
                {
                    ErrHeight = $"Valeur pas un entier.";
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
            get => string.IsNullOrEmpty(ErrWidth) ? Visibility.Hidden : Visibility.Visible;
        }

        public Visibility ErrorHeightVisible
        {
            get => string.IsNullOrEmpty(ErrHeight) ? Visibility.Hidden : Visibility.Visible;
        }

        public RelayCommand CreateCanvas { get; private set; }

        public VM_CreateCanvas()
        {
            CreateCanvas = new(exec => { }, canExec => (_width is <= 30 and >= 10) && (_height is <= 30 and >= 10));
            BorderBrushHeight = Brushes.Black;
            BorderBrushWidth = Brushes.Black;
        }
    }
}
