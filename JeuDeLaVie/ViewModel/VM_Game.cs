using JeuDeLaVie.Model;
using JeuDeLaVie.Properties;
using Microsoft.Win32;
using RelayCommandLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

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
        private double _canvasWidthPx;
        private double _canvasHeightPx;
        private double _canvasTileSizePx;
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

        public double CanvasTileSizePx
        {
            get
            {
                return (_canvasTileSizePx < 15)
                    ? 15
                    : _canvasTileSizePx;
            }
            set
            {
                _canvasTileSizePx = (value < 15)
                    ? 15
                    : value;
            }
        }

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
        public double CanvasWidthPx
        {
            get => _canvasWidthPx;
            set { _canvasWidthPx = value; OnPropertyChanged(); }
        }

        public double CanvasHeightPx
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
        public RelayCommand DrawGunShape { get; private set; }
        public RelayCommand DrawRandomShape { get; private set; }
        public RelayCommand LoadShapeFromFile { get; private set; }
        public RelayCommand SaveShapeToFile { get; private set; }
        #endregion

        #region ICommand Methods
        #region executes
        private void PauseExecute(object s) => IsPlaying = false;
        private void StepExecute(object s) => IsStepSet = true;
        private void ResumeExecute(object s) => IsPlaying = true;
        private void DrawGliderShapeExecute(object s) => SetShape(GetCenterOffset((3, 3)), GetShapeFromApplicationFile(Resources.GliderFile));
        private void DrawBlinkerShapeExecute(object s) => SetShape(GetCenterOffset((3, 3)), GetShapeFromApplicationFile(Resources.BlinkerFile));
        private void DrawGunExecute(object s) => SetShape(GetCenterOffset((33, 21)), GetShapeFromApplicationFile(Resources.GunFile));
        private void DrawRandomShapeExecute(object s)
        {
            Formes = GenerateFilledGridRandom(_canvasWidthTiles, _canvasHeightTiles, CanvasTileSizePx);
            _logicalState = ConvertListTo2DimensionalArray(Formes, _canvasWidthTiles, _canvasHeightTiles);
        }
        private async void LoadShapeFromFileExecute(object s)
        {
            OpenFileDialog loadFileDialog = new();
            loadFileDialog.Filter = "Game of life files (*.gol)|*.gol";
            loadFileDialog.InitialDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), 
                Resources.SaveDirectory,
                Resources.UserSaves);

            if (loadFileDialog.ShowDialog() == true)
            {
                try
                {
                    _logicalState = await ConvertFileToGridAsync(loadFileDialog.FileName);
                    Formes = Convert2DimensionalArrayToList(_logicalState);
                }
                catch(InvalidOperationException e)
                {

                }

            }
        }

        private void SaveShapeToFileExecute(object s)
        {
            SaveFileDialog saveFileDialog = new();
            saveFileDialog.Filter = "Game of life files (*.gol)|*.gol";
            saveFileDialog.InitialDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Resources.SaveDirectory,
                Resources.UserSaves);

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    ConvertGridToFile(_logicalState, _canvasWidthTiles, _canvasHeightTiles, Path.Combine(Environment.CurrentDirectory, Resources.SaveDirectory, Resources.UserSaves, saveFileDialog.FileName));
                }
                catch
                (Exception e)
                { }
            }
        }

        #endregion
        #region canExecutes
        private bool CanPauseExecute(object s) => IsGameStarted && IsPlaying;
        private bool CanStepExecute(object s) => IsGameStarted && !IsPlaying;
        private bool CanResumeExecute(object s) => IsGameStarted && !IsPlaying;
        private bool CanDrawGliderShapeExecute(object s) => !IsGameStarted && _canvasHeightTiles >= 3 && _canvasWidthTiles >= 3;
        private bool CanDrawBlinkerShapeExecute(object s) => !IsGameStarted && _canvasHeightTiles >= 3 && _canvasWidthTiles >= 3;
        private bool CanDrawGunExecute(object s) => !IsGameStarted && _canvasHeightTiles >= 21 && _canvasWidthTiles >= 33;
        private bool CanDrawRandomShapeExecute(object s) => !IsGameStarted;
        private bool CanLoadShapeFromFileExecute(object s) => true;
        private bool CanSaveShapeToFileExecute(object s) => true;
        private async Task<bool[,]> ConvertFileToGridAsync(string fileName)
        {
            GameState? loadedState;
            using (StreamReader reader = new(fileName))
            {
                string json = await reader.ReadToEndAsync();

                loadedState = JsonSerializer.Deserialize<GameState>(json);
            }

            if (loadedState is null)
                throw new InvalidOperationException();

            _canvasWidthTiles = loadedState.CanvasWidth;
            _canvasHeightTiles = loadedState.CanvasHeight;
            return ConvertListTo2DimensionalArray(loadedState.LifeForms, _canvasWidthTiles, _canvasHeightTiles);
        }

        private void ConvertGridToFile(bool[,] grid, int canvasWidth, int canvasHeight, string filePathAndName)
        {
            GameState state = new()
            {
                CanvasWidth = canvasWidth,
                CanvasHeight = canvasHeight,
                LifeForms = Convert2DimensionalArrayToList(grid)
            };

            using (StreamWriter writer = new(filePathAndName, false))
            {
                writer.WriteLine(JsonSerializer.Serialize(state, new() { WriteIndented=true}));
            }
        }
        #endregion
        private void SetShape((int colOffset, int rowOffset) target, int[,] shape)
        {
            if (shape.GetLength(0) > _canvasWidthTiles || shape.GetLength(1) > _canvasHeightTiles)
                throw new ArgumentException(
                $"Shape is bigger than current canvas size (shape: {shape.GetLength(0)} x {shape.GetLength(1)} vs. canvas: {_canvasWidthTiles} x {_canvasHeightTiles}).");

            bool[,] array = new bool[_canvasWidthTiles, _canvasHeightTiles];
            target.rowOffset = (array.GetLength(1) - (shape.GetLength(1) + target.rowOffset) < 0)
                ? 0
                : target.rowOffset;
            target.colOffset = (array.GetLength(0) - (shape.GetLength(0) + target.colOffset) < 0)
                ? 0
                : target.colOffset;

            for (int row = 0; row < shape.GetLength(1); row++)
                for (int col = 0; col < shape.GetLength(0); col++)
                    array[col + target.colOffset, row + target.rowOffset] = shape[col, row] == 1;

            _logicalState = array;
            Formes = Convert2DimensionalArrayToList(_logicalState);
        }
        #endregion

        public void Init()
        {
            Task.Run(InspectSaveDirectory);
            _canvasWidthTiles = (int)(CanvasWidthPx / CanvasTileSizePx);
            _canvasHeightTiles = (int)(CanvasHeightPx / CanvasTileSizePx);
            _logicalState = new bool[_canvasWidthTiles, _canvasHeightTiles];

            PropertyChanged += OnGameStatePropertyChangedUpdateVisibility;
            CurrentIteration = 0;
            IterationSpeed = 50;
            IsGameStarted = false;
            IsInfiniteChecked = false;
            IsStepSet = false;

            Formes = GenerateFilledGrid(_canvasWidthTiles, _canvasHeightTiles, CanvasTileSizePx);

            StartGame = new((s) => _ = Run());
            Pause = new(PauseExecute, CanPauseExecute);
            Step = new(StepExecute, CanStepExecute);
            Resume = new(ResumeExecute, CanResumeExecute);
            DrawGliderShape = new(DrawGliderShapeExecute, CanDrawGliderShapeExecute);
            DrawBlinkerShape = new(DrawBlinkerShapeExecute, CanDrawBlinkerShapeExecute);
            DrawGunShape = new(DrawGunExecute, CanDrawGunExecute);
            DrawRandomShape = new(DrawRandomShapeExecute, CanDrawRandomShapeExecute);
            LoadShapeFromFile = new(LoadShapeFromFileExecute, CanLoadShapeFromFileExecute);
            SaveShapeToFile = new(SaveShapeToFileExecute, CanSaveShapeToFileExecute);
        }

        private void InspectSaveDirectory()
        {
            string saveDirectoryRoot = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Resources.SaveDirectory);

            // Check if save directory exists.
            if (!Directory.Exists(saveDirectoryRoot))
                Directory.CreateDirectory(saveDirectoryRoot);

            // Check if app saves exist and re-create them if not.
            if (!Directory.Exists(Path.Combine(saveDirectoryRoot, Resources.AppSaves)))
            {
                Directory.CreateDirectory(Path.Combine(saveDirectoryRoot, Resources.AppSaves));
                Task.Run(RestoreAppSaveFiles);
            }

            // Check if user save directory exists.
            if (!Directory.Exists(Path.Combine(saveDirectoryRoot, Resources.UserSaves)))
                Directory.CreateDirectory(Path.Combine(saveDirectoryRoot, Resources.UserSaves));
        }

        private void RestoreAppSaveFiles()
        {
            string appSaveDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Resources.SaveDirectory, Resources.AppSaves);
            Dictionary<string, string> files = new()
            {
                { Path.Combine(appSaveDirectory, Resources.GliderFile), Resources.GliderShape },
                { Path.Combine(appSaveDirectory, Resources.BlinkerFile), Resources.BlinkerShape },
                { Path.Combine(appSaveDirectory, Resources.GunFile), Resources.GunShape }
            };

            foreach (KeyValuePair<string, string> filePathAndShape in files)
            {

                using (StreamWriter sw = new(filePathAndShape.Key, false))
                {
                    sw.WriteLine(filePathAndShape.Value);
                }
            }
        }

        private int[,] GetShapeFromApplicationFile(string shapeFileName)
        {
            // "Cette methode est degeulasse." -Andrew Deeley

            if (Path.GetExtension(shapeFileName) != ".gol")
                shapeFileName += ".gol";

            shapeFileName = Path.Combine(Environment.CurrentDirectory, Resources.SaveDirectory, Resources.AppSaves, shapeFileName);
            if (!File.Exists(shapeFileName))
                RestoreAppSaveFiles();

            try
            {
                string shapeString = "";
                using (StreamReader reader = new(shapeFileName))
                {
                    shapeString = reader.ReadToEnd();
                }

                if (string.IsNullOrEmpty(shapeString)) return new int[0, 0];
                string[] shapeStringParams = shapeString.Split(';');
                if (shapeStringParams.Length < 3) return new int[0, 0];
                if (!int.TryParse(shapeStringParams[0], out int width)) return new int[0, 0];
                if (!int.TryParse(shapeStringParams[1], out int height)) return new int[0, 0];

                string[] shapeRows = shapeStringParams[2..];
                if (shapeRows.Length != height) return new int[0, 0];

                List<int> shapeColRowValues = new(width * height);

                foreach (string row in shapeRows)
                {
                    string[] values = row.Split(',');
                    foreach (string value in values)
                    {
                        shapeColRowValues.Add(int.Parse(value));
                    }
                }

                int[,] returnShapeColRowValues = new int[width, height];

                for (int y = 0; y < height; y++)
                    for (int x = 0; x < width; x++)
                        returnShapeColRowValues[x, y] = shapeColRowValues[(width * y) + x];

                return returnShapeColRowValues;
            }
            catch (Exception) { return new int[0, 0]; }
        }

        private (int, int) GetCenterOffset((int width, int height) boundingBox)
        {
            int centerCol = (int)(0.5 * _canvasWidthTiles);
            int centerRow = (int)(0.5 * _canvasHeightTiles);

            centerCol = centerCol - (int)(0.5 * boundingBox.width);
            centerRow = centerRow - (int)(0.5 * boundingBox.height);

            return (centerCol, centerRow);            
        }

        private static List<LifeForm> GenerateFilledGrid(int width, int height, double tileSize)
        {
            List<LifeForm> forms = new(width * height);
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    forms.Add(new(col, row, tileSize));
                }
            }

            return forms;
        }

        private static List<LifeForm> GenerateFilledGridRandom(int width, int height, double tileSize)
        {
            List<LifeForm> filledForms = GenerateFilledGrid(width, height, tileSize);
            Random r = new(Environment.TickCount);

            foreach (LifeForm form in filledForms)
            {
                form.IsAlive = r.Next() % 2 == 0;
            }

            return filledForms;
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
            _logicalState = ConvertListTo2DimensionalArray(Formes, _canvasWidthTiles, _canvasHeightTiles);
            if ((bool)IsInfiniteChecked)
                NoIterations = "0";

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
            CommandManager.InvalidateRequerySuggested();
        }


        /// <summary>
        /// Copies the given list object into the 2-dimensional logical state array.
        /// </summary>
        /// <param name="list">List to copy.</param>
        private bool[,] ConvertListTo2DimensionalArray(List<LifeForm> list, int width, int height)
        {
            if (height * width != list.Count) 
                throw new ArgumentException(
                    $"Invalid array size or size parameters. Array parameters were smaller ({height * width} indexes) than the provided List ({list.Count} indexes)"
                    );


            bool[,] array = new bool[width, height];
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    int index = row * width + col;
                    bool isAlive = list.ElementAt(index).IsAlive;
                    array[col, row] = isAlive;
                }
            }

            return array;
        }

        private List<LifeForm> Convert2DimensionalArrayToList(bool[,] array)
        {
            List<LifeForm> list = new(array.Length);
            for (int row = 0; row < array.GetLength(1); row++)
            {
                for (int col = 0; col < array.GetLength(0); col++)
                {
                    LifeForm form = new(col, row, (int)CanvasTileSizePx);
                    form.IsAlive = array[col, row];
                    list.Add(form);
                }
            }

            return list;
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
            _logicalState = ConvertListTo2DimensionalArray(Formes, _canvasWidthTiles, _canvasHeightTiles);
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