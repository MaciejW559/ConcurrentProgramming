using System.Collections.ObjectModel;
using System.Windows.Input;
using Model;

namespace ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly ModelAbstractAPI _modelLayer;
        private int _ballCount = 5;

        public ObservableCollection<BallModel> Balls => _modelLayer.Balls;
        public ICommand StartCommand { get; }

        public int BallCount
        {
            get => _ballCount;
            set
            {
                _ballCount = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel()
        {
            _modelLayer = ModelAbstractAPI.CreateApi();
            StartCommand = new RelayCommand(StartSimulation);
        }

        private void StartSimulation()
        {
            _modelLayer.StartSimulation(BallCount);
        }
    }
}