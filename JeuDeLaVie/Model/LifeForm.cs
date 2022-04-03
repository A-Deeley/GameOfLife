using RelayCommandLibrary;
using System;
using System.Collections.Generic;
using System.Windows.Shapes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace JeuDeLaVie.Model
{
    internal class LifeForm : INotifyPropertyChanged
    {
        #region Events
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName]string caller = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(caller));
        #endregion

        private bool _isAlive;
        private Brush _lifeFormColour;
        #region Bindings
        public bool IsAlive
        {
            get => _isAlive;
            set
            {
                _isAlive = value;
                LifeFormColour = (value) ? Brushes.Black : Brushes.White;
            }
        }
        public Brush LifeFormColour {
            get => _lifeFormColour;
            set
            {
                _lifeFormColour = value;
                OnPropertyChanged();
            }
        }
        public RelayCommand LifeFormClickedEvent { get; private set; }
        #endregion

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
            // Convert the click event into a toggling of the IsAlive state.
            LifeFormClickedEvent = new((caller) => IsAlive = !IsAlive);
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
