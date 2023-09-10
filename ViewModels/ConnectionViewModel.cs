using System.Windows.Media;

namespace BGEdit
{
    public class ConnectionViewModel
    {
        public Brush Stroke { get; set; } = new SolidColorBrush(Colors.PeachPuff);
        public ConnectionViewModel(ConnectorViewModel source, ConnectorViewModel target)
        {
            Source = source;
            Target = target;

            Source.IsConnected = true;
            Target.IsConnected = true;
        }
        public ConnectorViewModel Source { get; set; }
        public ConnectorViewModel Target { get; set; }
    }
}
