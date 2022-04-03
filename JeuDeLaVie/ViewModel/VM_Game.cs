using JeuDeLaVie.Model;
using RelayCommandLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JeuDeLaVie.ViewModel
{
    internal class VM_Game : VM_Base
    {
        private ObservableCollection<LifeForm> _formes;
        private int _nbIterations;
        private bool _isPlaying;
        private bool _isPaused;
        private int _canvasWidth;
        private int _canvasHeight;

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

        public RelayCommand StartGame { get; private set; }

        public VM_Game(int canvasWidth, int canvasHeight)
        {
            _canvasWidth = canvasWidth;
            _canvasHeight = canvasHeight;
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
