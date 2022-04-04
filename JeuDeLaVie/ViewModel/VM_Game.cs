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
    internal class VM_Game : VM_Base
    {
        private ObservableCollection<LifeForm> _formes;
        internal bool[,] _logicalState;
        private bool _isPlaying;
        private bool _isPaused;
        private int _canvasWidthTiles;
        private int _canvasHeightTiles;
        private int _canvasWidthPx;
        private int _canvasHeightPx;
        private double _canvasTileSize;
        private bool? _infinite;
        private int _iterations;
        private int _currentIteration;
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
            set { if (value != null) { _iterationSpeed = value; OnPropertyChanged(); } }
        }
        public bool IsPlaying
        {
            get => _isPlaying;
            set { _isPlaying = value; OnPropertyChanged(); }
        }
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
                        Formes.Add(new(row, col, (int)_canvasTileSize));
                    }
                }
            else
                for (int row = 0; row < _logicalState.GetLength(1); row++)
                {
                    for (int col = 0; col < _logicalState.GetLength(0); col++)
                    {
                        Formes.Add(new(row, col));
                    }
                }

            StartGame = new(exec => Run());
        }

        private async void Run()
        {
            IsPlaying = true;
            OnPropertyChanged(nameof(CanClickForm));
            CopyFormsToArray2Dimensional(Formes);
            while (CurrentIteration < _iterations)
            {
                await Task.Run(() => Thread.Sleep((int)(1000 * IterationSpeed)));
                // Compute next generation
                List<int> changedForms = await Iterate();
                // Apply it to the view
                await Task.Run(() => { ApplyChanges(changedForms); });
                // Increment iteration
                CurrentIteration++;
            }
        }

        private void CopyFormsToArray2Dimensional(ObservableCollection<LifeForm> list)
        {
            for (int row = 0; row < _logicalState.GetLength(1); row++)
            {
                for (int col = 0; col < _logicalState.GetLength(0); col++)
                {
                    int index = row * _logicalState.GetLength(0) + col;
                    bool isAlive = list.ElementAt(index).IsAlive;
                    _logicalState[row, col] = isAlive;
                }
            }
        }

        private async Task<List<int>> Iterate() 
        {
            List<int> changed = new();
            for (int row = 0; row < _logicalState.GetLength(1); row++)
            {
                for (int col = 0; col < _logicalState.GetLength(0); col++)
                {
                    // If the state is different, add it to a list. Since the list is 1-Dimensional, multiply the row buy the row length and add the column value.
                    if (_logicalState[row, col] != FormNextGenerationIsAlive(row, col)) changed.Add(row * _logicalState.GetLength(0) + col);
                }
            }

            return changed;
        }

        private void ApplyChanges(List<int> changes)
        {
            foreach (int index in changes)
            {
                Formes[index].ToggleState();
            }

            CopyFormsToArray2Dimensional(Formes);
        }


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

            // If we found 3 alive neighbours, and the cell is dead, it becomes aive.
            if (!isCurrentFormAlive && sumAlive >= 3) return true;


            // Check same row
            for (int fCol = leftBound; fCol <= rightBound; fCol++)
            {
                for (int fRow = row; fRow <= row; fRow++)
                {
                    if (fRow == row && fCol == col) continue;
                    if (_logicalState[fCol, fRow]) sumAlive++;

                    // If the form is alive and has more than 3 neighbours, it dies.
                    if (isCurrentFormAlive && sumAlive > 3) return false;

                    // If the form is dead and has at least 3 neighbours, it becomes alive.
                    if (!isCurrentFormAlive && sumAlive >= 3) return true;
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

            // If we found 3 alive neighbours, and the cell is dead, it becomes aive.
            if (!isCurrentFormAlive && sumAlive >= 3) return true;
            // If the form is alive and has more than 3 neighbours, it dies.
            if (isCurrentFormAlive && sumAlive > 3) return false;

            return false;
        }
    }
}