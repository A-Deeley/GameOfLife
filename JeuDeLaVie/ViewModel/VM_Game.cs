using JeuDeLaVie.Model;
using RelayCommandLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace JeuDeLaVie.ViewModel
{
    internal class VM_Game : VM_Base
    {
        private ObservableCollection<LifeForm> _formes;
        private int _nbIterations;
        private bool _isPlaying;
        private bool _isPaused;
        private int _canvasWidthTiles;
        private int _canvasHeightTiles;
        private int _canvasWidthPx;
        private int _canvasHeightPx;
        private double _canvasTileSize;

        public ObservableCollection<LifeForm> Formes
        {
            get => _formes;
            set { if (value is not null) { _formes = value; OnPropertyChanged(); } }
        }

        public int NoIterations
        {
            get => _nbIterations;
            set { _nbIterations = value; OnPropertyChanged(); }
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

        public RelayCommand StartGame { get; private set; }

        public VM_Game(int canvasWidth, int canvasHeight, double tileSize)
        {
            _canvasTileSize = tileSize;
            _canvasWidthTiles = canvasWidth;
            _canvasHeightTiles = canvasHeight;
            CanvasWidthPx = canvasWidth * (int)_canvasTileSize;
            CanvasHeightPx = canvasHeight * (int)_canvasTileSize;
            Formes = new();

            for (int height = 0; height < _canvasHeightTiles; height++)
            {
                for (int width = 0; width < _canvasWidthTiles; width++)
                {
                    Formes.Add(new(width, height));
                }
            }

            if ((int)_canvasTileSize > 15)
            {
                foreach (LifeForm form in Formes)
                {
                    form.Coord.SetTileWidth((int)_canvasTileSize);
                }
            }
            StartGame = new(exec => Run());
        }

        private void Run()
        {

        }

        private bool IterateForm(LifeForm f, int index)
        {
            return false;
        }
    }
}