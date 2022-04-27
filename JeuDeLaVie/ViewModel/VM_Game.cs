using JeuDeLaVie.Model;
using RelayCommandLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        private List<LifeForm> _formes;
        /// <summary>
        /// The 2-dimensional bool array stores the logical state of the game. Iterations are performed on this array and then copied to the display list.
        /// </summary>
        internal bool[,] _logicalState;
        /// <summary>
        /// Represents the state of the game, and while true prevents the user from interacting with the UI (ignores MouseDown events).
        /// </summary>
        private bool _isPlaying;
        private int _canvasWidthTiles;
        private int _canvasHeightTiles;
        private int _canvasWidthPx;
        private int _canvasHeightPx;
        /// <summary>
        /// Width of the a single tile in px squared.
        /// </summary>
        private double _canvasTileSize;
        /// <summary>
        /// Amount of iterations (generations) to run the program for.
        /// </summary>
        private int _iterations;
        private int _currentIteration;
        /// <summary>
        /// Iteration speed in ms.
        /// </summary>
        private double _iterationSpeed;
        private bool _step;
        private bool _isGameStarted;
        private bool? _isInfiniteChecked;
        private bool _allowIterationInput;
        private Visibility _pauseVisible;
        private Visibility _stepVisible;
        private Visibility _startVisible;
        private Visibility _resumeVisible;

        public bool? IsInfiniteChecked
        {
            get => _isInfiniteChecked;
            set
            {
                _isInfiniteChecked = value;
                OnPropertyChanged();
                AllowIterationInput = !value ?? true;
            }
        }

        public bool AllowIterationInput
        {
            get => _allowIterationInput;
            set
            {
                _allowIterationInput = value;
                OnPropertyChanged();
            }
        }
        public Visibility PauseVisible
        {
            get => _pauseVisible;
            set
            {
                _pauseVisible = value;
                OnPropertyChanged();
            }
        }
        public Visibility StepVisible
        {
            get => _stepVisible;
            set
            {
                _stepVisible = value;
                OnPropertyChanged();
            }

        }
        public Visibility StartVisible
        {
            get => _startVisible;
            set
            {
                _startVisible = value;
                OnPropertyChanged();
            }
        }
        public Visibility ResumeVisible
        {
            get => _resumeVisible;
            set
            {
                _resumeVisible = value;
                OnPropertyChanged();
            }

        }

        public List<LifeForm> Formes
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

        private bool IsGameStarted
        {
            get => _isGameStarted;
            set
            {
                _isGameStarted = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Blocks MouseDown events from occuring if the game is currently iterating.
        /// </summary>
        public bool CanClickForm
        {
            get => !IsPlaying;
        }

        public bool IsStepSet
        { get => _step; set
            {
                _step = value;
                OnPropertyChanged();
            }
        }
        #region ICommands
        public RelayCommand StartGame { get; private set; }
        public RelayCommand Pause { get; private set; }
        public RelayCommand Step { get; private set; }
        public RelayCommand Resume { get; private set; }
        public RelayCommand DrawGliderShape { get; private set; }
        public RelayCommand DrawBlinkerShape { get; private set; }
        public RelayCommand DrawBoatShape { get; private set; }
        public RelayCommand DrawRandomShape { get; private set; }
        public RelayCommand LoadShapeFromFile { get; private set; }
        public RelayCommand SaveShapeToFile { get; private set; }
        #endregion

        #region ICommand Methods
        #region executes
        private void PauseExecute(object s) => IsPlaying = false;
        private void StepExecute(object s) => IsStepSet = true;
        private void ResumeExecute(object s) => IsPlaying = true;
        private void DrawGliderShapeExecute(object s) => SetShape(3, 2, 1, 5, 6, 7, 8);
        private void DrawBlinkerShapeExecute(object s) { }
        private void DrawBoatShapeExecute(object s) { }
        private void DrawRandomShapeExecute(object s) { }
        private void LoadShapeFromFileExecute(object s) { }
        private void SaveShapeToFileExecute(object s) { }
        #endregion
        #region canExecutes
        private bool CanPauseExecute(object s) => IsGameStarted && IsPlaying;
        private bool CanStepExecute(object s) => IsGameStarted && !IsPlaying;
        private bool CanResumeExecute(object s) => IsGameStarted && !IsPlaying;
        private bool CanDrawGliderShapeExecute(object s) => !IsGameStarted && _canvasHeightTiles >= 3 && _canvasWidthTiles >= 3;
        private bool CanDrawBlinkerShapeExecute(object s) => !IsGameStarted && _canvasHeightTiles >= 3 && _canvasWidthTiles >= 3;
        private bool CanDrawBoatShapeExecute(object s) => !IsGameStarted && _canvasHeightTiles >= 3 && _canvasWidthTiles >= 3;
        private bool CanDrawRandomShapeExecute(object s) => !IsGameStarted;
        private bool CanLoadShapeFromFileExecute(object s) => false;
        private bool CanSaveShapeToFileExecute(object s) => false;
        #endregion
        private void SetShape(int boundingBoxSize, int targetOffset, params int[] indexes)
        {
            Formes = GenerateFilledGrid(_canvasWidthTiles, _canvasHeightTiles, (int)_canvasTileSize);

            int rowOffset = (_canvasWidthTiles * targetOffset) + 2 < Formes.Count
                ? _canvasWidthTiles * targetOffset
                : 0;
            int colOffset = targetOffset + 2< Formes.Count
                ? targetOffset
                : 0;

            for (int i = 0; i < indexes.Length; i++)
            {
                //                (col, row) = index in that grid (1D)
                // 3x3 grid     : ( 2 ,  1 ) = index 5
                // ?x? grid     : (   , ?/index]) = index (col*row)+col
                // ex: 5x5 grid : ( 2 , 
                int rowNumber = boundingBoxSize / indexes[i];
                Formes[indexes[i]+rowOffset+colOffset].IsAlive = true;
            }
        }
        #endregion

        public VM_Game(int canvasWidth, int canvasHeight, double tileSize)
        {
            _canvasTileSize = tileSize;
            _canvasWidthTiles = canvasWidth;
            _canvasHeightTiles = canvasHeight;
            CanvasWidthPx = canvasWidth * (int)_canvasTileSize;
            CanvasHeightPx = canvasHeight * (int)_canvasTileSize;
            _logicalState = new bool[_canvasWidthTiles, _canvasHeightTiles];

            PropertyChanged += OnGameStatePropertyChangedUpdateVisibility;
            CurrentIteration = 0;
            IterationSpeed = 1000;
            IsGameStarted = false;
            IsInfiniteChecked = false;
            IsStepSet = false;

            // Generate initial view state. Resize tiles if window size permits it.
            if ((int)_canvasTileSize < 15)
                _canvasTileSize = 15;
            Formes = GenerateFilledGrid(_canvasWidthTiles, _canvasHeightTiles, (int)_canvasTileSize);
            OnPropertyChanged(nameof(Formes));

            DrawGliderShape = new(DrawGliderShapeExecute, CanDrawGliderShapeExecute);

            StartGame = new(exec => _ = Run());
        }

        private static List<LifeForm> GenerateFilledGrid(int width, int height, int tileSize)
        {
            List<LifeForm> forms = new(width * height);
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    forms.Add(new(col, row, (int)tileSize));
                }
            }

            return forms;
        }

        private void OnGameStatePropertyChangedUpdateVisibility(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IsGameStarted) || e.PropertyName == nameof(IsPlaying))
            {
                PauseVisible = (IsGameStarted && IsPlaying)
                    ? Visibility.Visible
                    : Visibility.Hidden;

                StepVisible = (IsGameStarted && !IsPlaying)
                    ? Visibility.Visible
                    : Visibility.Hidden;

                ResumeVisible = (IsGameStarted && !IsPlaying)
                    ? Visibility.Visible
                    : Visibility.Hidden;

                StartVisible = (!IsPlaying)
                    ? Visibility.Visible
                    : Visibility.Hidden;

                OnPropertyChanged(nameof(CanClickForm));
            }
        }

        /// <summary>
        /// Main game loop.
        /// </summary>
        private async Task Run()
        {
            IsPlaying = true;
            IsGameStarted = true;
            // Copy the initial view state from user input into the logical state.
            CopyFormsToArray2Dimensional(Formes);

            while (((bool)IsInfiniteChecked && IsGameStarted) || CurrentIteration < _iterations)
            {
                if (!IsStepSet) await Task.Delay((int)IterationSpeed);
                // Compute next generation
                List<int> changedForms = Iterate();
                // Apply it to the view
                ApplyChanges(changedForms);
                // Increment iteration
                CurrentIteration++;

                // if pause is pressed, idle
                while (!IsPlaying)
                {
                    IsStepSet = false;
                    await Task.Delay(25);
                    // Allow player to step 1 iteration at a time.
                    if (IsStepSet)
                        break;
                }

            }
            IsGameStarted = false;
            IsPlaying = false;
            IsStepSet = false;
            CurrentIteration = 0;
        }


        /// <summary>
        /// Copies the given list object into the 2-dimensional logical state array.
        /// </summary>
        /// <param name="list">List to copy.</param>
        private void CopyFormsToArray2Dimensional(List<LifeForm> list)
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

        private void ApplyChangesTest(in List<LifeForm> changes, Dictionary<LifeForm, ReadOnlyCollection<LifeForm>> forms)
        {
            // Apply changes.

            foreach (LifeForm form in changes)
            {
                form.ToggleState();
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