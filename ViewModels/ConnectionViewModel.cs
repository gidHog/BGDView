using System.Windows.Media;

namespace BGEdit
{
    public class ConnectionViewModel
    {
        public Brush Stroke { get; set; } = new SolidColorBrush(Colors.PeachPuff);
        public enum ConnectionType
        {
            Default = 0,
            Jump = 1
        }
        public ConnectionViewModel(ConnectorViewModel source, ConnectorViewModel target, ConnectionType connectionType)
        {
            Source = source;
            Target = target;

            Source.IsConnected = true;
            Target.IsConnected = true;
            this.connectionType = connectionType;
        }
        public ConnectorViewModel Source { get; set; }
        public ConnectorViewModel Target { get; set; }

        public ConnectionType connectionType { get; set; }
    }
}
