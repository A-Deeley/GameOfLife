using JeuDeLaVie.Model;
using RelayCommandLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace JeuDeLaVie.ViewModel
{
    /// <summary>
    /// ViewModel hosting the bindings for the views as well as the game logic.
    /// </summary>
    internal class VM_Game : VM_Base
    {
        /// <summary>
        ///  The ObservableCollection stores the view-representation of the game state.
        /// </summary>
        private ObservableCollection<LifeForm> _formes;
        /// <summary>
        /// The 2-dimensional bool array stores the logical state of the game. Iterations are performed on this array and then copied to the display list.
        /// </summary>
        internal bool[,] _logicalState;
        /// <summary>
        /// Represents the state of the game, and while true prevents the user from interacting with the UI (ignores MouseDown events).
        /// </summary>
        private bool _isPlaying;
        private bool _isPaused;
        private int _canvasWidthTiles;
        private int _canvasHeightTiles;
        private int _canvasWidthPx;
        private int _canvasHeightPx;
        /// <summary>
        /// Width of the a single tile in px squared.
        /// </summary>
        private double _canvasTileSize;
        /// <summary>
        /// View-binding to enable infinite iterations.
        /// </summary>
        private bool? _infinite;
        /// <summary>
        /// Amount of iterations (generations) to run the program for.
        /// </summary>
        private int _iterations;
        private int _currentIteration;
        /// <summary>
        /// Iteration speed in ms.
        /// </summary>
        private double _iterationSpeed;

        public ObservableCollection<LifeForm> Formes
        {
            get => _formes;
            set { if (value is not null) { _formes = value; OnPropertyChanged(); } }
        }
        public int CanvasWidthPx
        {
            get => _canvasWidthPx;
            set { _canvasWidthPx = value; OnPropertyChanged(); }
        }

        public int CanvasHeightPx
        {
            get => _canvasHeightPx;
            set { _canvasHeightPx = value; OnPropertyChanged(); }
        }

        public bool? Infinite
        {
            get => _infinite;
            set { _infinite = value; OnPropertyChanged(); }
        }

        public string NoIterations
        {
            get => _iterations.ToString();
            set
            {
                if (int.TryParse(value, out int valueInt))
                {
                    _iterations = valueInt;
                    OnPropertyChanged();
                }
            }
        }
        public int CurrentIteration
        {
            get => _currentIteration;
            set { _currentIteration = value; OnPropertyChanged(); }
        }
        public double IterationSpeed
        {
            get => _iterationSpeed;
            set { _iterationSpeed = value; OnPropertyChanged(); }
        }
        public bool IsPlaying
        {
            get => _isPlaying;
            set { _isPlaying = value; OnPropertyChanged(); }
        }
        /// <summary>
        /// Blocks MouseDown events from occuring if the game is currently iterating.
        /// </summary>
        public bool CanClickForm
        {
            get => !IsPlaying;
        }

        public RelayCommand StartGame { get; private set; }

        public VM_Game(int canvasWidth, int canvasHeight, double tileSize)
        {
            _canvasTileSize = tileSize;
            _canvasWidthTiles = canvasWidth;
            _canvasHeightTiles = canvasHeight;
            CanvasWidthPx = canvasWidth * (int)_canvasTileSize;
            CanvasHeightPx = canvasHeight * (int)_canvasTileSize;
            Formes = new();
            _logicalState = new bool[_canvasWidthTiles, _canvasHeightTiles];
            Infinite = false;
            CurrentIteration = 0;
            IterationSpeed = 1;

            // Generate initial view state. Resize tiles if window size permits it.
            if ((int)_canvasTileSize > 15)
                for (int row = 0; row < _logicalState.GetLength(1); row++)
                {
                    for (int col = 0; col < _logicalState.GetLength(0); col++)
                    {
                        Formes.Add(new(col, row, (int)_canvasTileSize));
                    }
                }
            else
                for (int row = 0; row < _logicalState.GetLength(1); row++)
                {
                    for (int col = 0; col < _logicalState.GetLength(0); col++)
                    {
                        Formes.Add(new(col, row));
                    }
                }

            StartGame = new(exec => Run());
        }
        /// <summary>
        /// Main game loop.
        /// </summary>
        private async void Run()
        {
            IsPlaying = true;
            // Make sure the property value disabling MouseDown is correctly updated to the view.
            OnPropertyChanged(nameof(CanClickForm));
            // Copy the initial view state from user input into the logical state.
            CopyFormsToArray2Dimensional(Formes);

            while (CurrentIteration < _iterations)
            {
                //TODO: Implement pause feature.

                await Task.Run(() => Thread.Sleep((int)(1000 * IterationSpeed))); //TODO: Change iteration speed to be a linear ms-based slider instead of a multiplier.
                // Compute next generation
                List<int> changedForms = Iterate();
                // Apply it to the view
                ApplyChanges(changedForms);
                // Increment iteration
                CurrentIteration++;
            }
        }
        /// <summary>
        /// Copies the given list object into the 2-dimensional logical state array.
        /// </summary>
        /// <param name="list">List to copy.</param>
        private void CopyFormsToArray2Dimensional(ObservableCollection<LifeForm> list)
        {
            for (int row = 0; row < _logicalState.GetLength(1); row++)
            {
                for (int col = 0; col < _logicalState.GetLength(0); col++)
                {
                    int index = row * _logicalState.GetLength(0) + col;
                    bool isAlive = list.ElementAt(index).IsAlive;
                    _logicalState[col, row] = isAlive;
                }
            }
        }
        /// <summary>
        /// Computes 1 generation of the elements in the logical state array, storing the changed indexes in a list.
        /// </summary>
        /// <returns>List of indexes of changed states.</returns>
        private List<int> Iterate() 
        {
            List<int> changed = new();
            for (int row = 0; row < _logicalState.GetLength(1); row++)
            {
                for (int col = 0; col < _logicalState.GetLength(0); col++)
                {
                    // If the state is different, add it to a list. Since the list is 1-Dimensional, multiply the row buy the row length and add the column value.
                    if (_logicalState[col, row] != FormNextGenerationIsAlive(row, col)) changed.Add(row * _logicalState.GetLength(0) + col);
                }
            }

            return changed;
        }
        /// <summary>
        /// Applies the changes to the indexes given to the display list (view), then updates the logical state.
        /// </summary>
        /// <param name="changes"></param>
        private void ApplyChanges(List<int> changes)
        {
            // Apply changes.
            foreach (int index in changes)
            {
                Formes[index].ToggleState();
            }
            // Update logical state with changes.
            CopyFormsToArray2Dimensional(Formes);
        }

        /// <summary>
        /// Computes wether a LifeForm on a specified (x, y) is alive or dead in the next generation.
        /// </summary>
        /// <param name="row">The row of the LifeForm.</param>
        /// <param name="col">The column of the LifeForm.</param>
        /// <returns>True if the specified LifeForm is alive, false if it's dead.</returns>
        private bool FormNextGenerationIsAlive(int row, int col)
        {
            int sumAlive = 0;

            int leftBound = 0;
            if (col - 1 < 0)
                leftBound = 0;
            else
                leftBound = col - 1;

            int rightBound = 0;
            if (col + 1 >= _logicalState.GetLength(0))
                rightBound = col;
            else
                rightBound = col + 1;

            bool isCurrentFormAlive = _logicalState[col, row];
            // Check top row, if it exists
            if (row - 1 >= 0)
                for (int fCol = leftBound; fCol <= rightBound; fCol++)
                {
                    for (int fRow = row - 1; fRow < row; fRow++)
                    {
                        if (_logicalState[fCol, fRow]) sumAlive++;
                    }
                }

            // Check same row
            for (int fCol = leftBound; fCol <= rightBound; fCol++)
            {
                for (int fRow = row; fRow <= row; fRow++)
                {
                    if (fRow == row && fCol == col) continue;
                    if (_logicalState[fCol, fRow]) sumAlive++;

                    // If the form is alive and has more than 3 neighbours, it dies.
                    if (isCurrentFormAlive && sumAlive > 3) return false;

                }
            }

            // Check bottom row, if it exists
            if (row + 1 < _logicalState.GetLength(1))
                for (int fCol = leftBound; fCol <= rightBound; fCol++)
                {
                    for (int fRow = row + 1; fRow <= row + 1; fRow++)
                    {
                        if (_logicalState[fCol, fRow]) sumAlive++;
                    }
                }

            // If we found 3 alive neighbours, and the cell is dead, it becomes alive.
            if (!isCurrentFormAlive && sumAlive == 3) return true;
            // If the form is alive and has more than 3 neighbours, it dies.
            if (isCurrentFormAlive && sumAlive > 3) return false; 
            // If the form is alive and has 2 or 3 neighbours, it stays alives
            if (isCurrentFormAlive && (sumAlive is >= 2 and <= 3)) return true;

            return false;
        }
    }
}