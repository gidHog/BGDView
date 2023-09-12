using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

namespace BGEdit
{
    public class ConnectorViewModel : INotifyPropertyChanged
    {
        public List<ConnectionViewModel> connections = new List<ConnectionViewModel>();
        private Point _anchor;
        public Point Anchor
        {
            set
            {
                _anchor = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Anchor)));
            }
            get => _anchor;
        }

        private bool _isConnected;

        //todo
        public bool IsConnected
        {
            set
            {
                _isConnected = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsConnected)));
            }
            get => _isConnected;
        }

        public string Title { get; set; } = "Title not set";
        public string parentUUID { get; set; } = "UUID not set";
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
