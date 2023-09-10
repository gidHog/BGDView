using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Data;

namespace BGEdit
{

    public partial class Colorization
    {
        public static readonly Brush defaultBrush = new SolidColorBrush(Colors.MediumAquamarine);
        public static readonly Brush DefaultBrush = new SolidColorBrush(Colors.DimGray);
        public static readonly Brush EndnodeBrush = new SolidColorBrush(Colors.OrangeRed);
        public static readonly Brush RootBrush = new SolidColorBrush(Colors.OliveDrab);
        public static readonly Brush LogicNodeBrush = new SolidColorBrush(Colors.PowderBlue);
        public static readonly Dictionary<string, Brush> NodeValueToColor = new Dictionary<string, Brush>
        {
            { "TagAnswer", new SolidColorBrush(Colors.Orange) },
            { "RollResult", new SolidColorBrush(Colors.Goldenrod) },
            { "TagQuestion", new SolidColorBrush(Colors.DarkCyan) },
            { "Jump", new SolidColorBrush(Colors.PaleVioletRed) },
            { "ActiveRoll", new SolidColorBrush(Colors.DarkKhaki) }
        };
        public Colorization() { }
    }

    public static partial class GenericNodeActions
    {
        private static readonly float PosY = 1.3f, PosX = 1.8f;
        public static void AddTextToNode(ObservableCollection<String> list, String text)
        {
            list ??= new ObservableCollection<String>();
            list.Add(text);
        }
        public static Border AddBorderToStack(UIElement element)
        {
            Border border = new Border()
            {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                Background = new SolidColorBrush(Colors.White),
                Margin = new Thickness(2),
                Child = element
            };
            return border;
        }
        public static void CreateBinding(Control element, NodeViewModel bindTo, String bindName, DependencyProperty property)
        {
            Binding binding = new Binding(bindName);
            binding.Source = bindTo;
            binding.Mode = BindingMode.TwoWay;
            element.SetBinding(property, binding);
        }
        public static void AddInfoToNodeView(NodeNode node, NodeViewModel nView)
        {
            nView.Constructor = node.Constructor?.Value;
            nView.GroupID = node.GroupID?.Value;
            nView.GroupIndex = node.GroupIndex?.Value + "";
            if (node.TaggedTexts != null)
            { }
            nView.Speaker = node.Speaker?.Value + "";
            nView.UUID = node.Uuid?.Value;

        }
        public static Brush ColorNode(NodeNode node, BGData bgData)
        {
            if (node.Endnode?.Value == true)
                return Colorization.EndnodeBrush;

            if (node.Root?.Value == true)
                return Colorization.RootBrush;

            if (bgData.editorData[node.Uuid.Value]["logicalname"] == "LOGIC NODE")
                return Colorization.LogicNodeBrush;

            if (node.Constructor.Value != null && Colorization.NodeValueToColor.TryGetValue(node.Constructor.Value, out Brush color))
                return color;

            return Colorization.DefaultBrush;
        }

        public static NodeViewModel AddSpeakerRootNode(string MapKey)
        {
            var SpeakerRootNode = new NodeViewModel
            {
                Title = "Dialog Root UUID\n" + MapKey,
                Output = new ObservableCollection<ConnectorViewModel>
            {
                new ConnectorViewModel
                {
                    Title = "Block"
                }
            }
            };
            return SpeakerRootNode;
        }

        public static Point LocationStringToPoint(String UUID)
        {
            string[] coordinates = BGData.Instance.editorData[UUID]["position"].Split(';');

            if (coordinates.Length < 2)
            {
                return new Point(0,0);
            }

            double x = int.Parse(coordinates[0]) * PosX;
            double y = int.Parse(coordinates[1]) * PosY;

            return new Point(x, y);
        }
    }
}
