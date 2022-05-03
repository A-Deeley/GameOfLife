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
using System.Text.Json.Serialization;

namespace JeuDeLaVie.Model
{
    /// <summary>
    /// Model representing one square in the game board.
    /// </summary>
    public class LifeForm : INotifyPropertyChanged
    {
        #region Events
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName]string caller = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(caller));
        #endregion
        private bool _isAlive;
        private Brush _lifeFormColour;
        #region Bindings
        /// <summary>
        /// Represents the state of the object (alive or dead).
        /// </summary>
        public bool IsAlive
        {
            get => _isAlive;
            set
            {
                _isAlive = value;
                OnPropertyChanged();
                // Set the colour of the square depending on the state.
                LifeFormColour = (value) ? Brushes.Black : Brushes.White;
            }
        }
        /// <summary>
        /// Depending on the state, the square is either black (alive) or white (dead).
        /// </summary>
        /// 
        [JsonIgnore]
        public Brush LifeFormColour {
            get => _lifeFormColour;
            set
            {
                _lifeFormColour = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Command binding to the conversion from an event to a command in the xaml.
        /// </summary>
        public RelayCommand LifeFormClickedEvent { get; private set; }
        #endregion

        /// <summary>
        /// Represents the position (x, y) of the form in the game grid.
        /// </summary>
        public Coordinate Coord { get; set; }

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
        public LifeForm(int x, int y, double tileWidth)
            :this(x, y)
        {
            Coord.SetTileWidth(tileWidth);
        }
        public LifeForm() { }

        public void ToggleState()
        {
            IsAlive = !IsAlive;
        }
    }
}
