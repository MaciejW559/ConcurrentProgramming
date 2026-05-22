using Data;
using Logic;
using Model;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IModel _modelLayer;
        private readonly Logger _logger;
        public static double DEFAULT_WIDTH => IModel.DEFAULT_WIDTH;
        public static double DEFAULT_HEIGHT => IModel.DEFAULT_HEIGHT;
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
            _logger = new Logger("BallsLogs.txt");

            Task.Run(() => _logger.LoggingThread(CancellationToken.None));

            _modelLayer = new ModelLayer(
                new LogicLayer(
                    new Data.DataLayer(),
                    _logger
                )
            );
            StartCommand = new RelayCommand(StartSimulation);
        }

        public MainViewModel(IModel modelLayer)
        {
            _modelLayer = modelLayer;
            StartCommand = new RelayCommand(StartSimulation);
        }

        private async void StartSimulation()
        {
            await _modelLayer.StartSimulation(BallCount);
        }
    }
}