
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;


namespace BGEdit
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml + adding Nodes 
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {

            InitializeComponent();
        }
    }

    public class NodeViewModel : INotifyPropertyChanged
    {
        public string Title { get; set; }
        private Point _location;
        public event PropertyChangedEventHandler PropertyChanged;
        public List<String> RootList { get; set; } = new List<String>();
        public List<String> TagList { get; set; } = new List<String>();
        public List<String> SpeakerList { get; set; } = new List<String>();
        public List<String> GroupList { get; set; } = new List<String>();

        public List<String> TagTextList { get; set; } = new List<String>();
        public Brush HeaderBrushColor { get; set; } = new SolidColorBrush(Colors.DimGray);


        public String RootsFound { get; set; } = "Hidden";
        public String GroupsFound { get; set; } = "Hidden";
        public String TagsFound { get; set; } = "Hidden";
        public String SpeakersFound { get; set; } = "Hidden";

        public String TagTextFound { get; set; } = "Hidden";
        public Point Location
        {
            set
            {
                _location = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Location)));
            }
            get => _location;
        }

        public ObservableCollection<ConnectorViewModel> Input { get; set; } = new ObservableCollection<ConnectorViewModel>();
        public ObservableCollection<ConnectorViewModel> Output { get; set; } = new ObservableCollection<ConnectorViewModel>();
    }


    public class ConnectorViewModel : INotifyPropertyChanged
    {
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
        public bool IsConnected
        {
            set
            {
                _isConnected = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsConnected)));
            }
            get => _isConnected;
        }

        public string Title { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
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


    public interface INodifyCommand : ICommand
    {
        void RaiseCanExecuteChanged();
    }

    public class DelegateCommand : INodifyCommand
    {
        private readonly Action _action;
        private readonly Func<bool>? _condition;

        public event EventHandler? CanExecuteChanged;

        public DelegateCommand(Action action, Func<bool>? executeCondition = default)
        {
            _action = action ?? throw new ArgumentNullException(nameof(action));
            _condition = executeCondition;
        }

        public bool CanExecute(object parameter)
            => _condition?.Invoke() ?? true;

        public void Execute(object parameter)
            => _action();

        public void RaiseCanExecuteChanged()
            => CanExecuteChanged?.Invoke(this, new EventArgs());
    }

    public class DelegateCommand<T> : INodifyCommand
    {
        private readonly Action<T> _action;
        private readonly Func<T, bool>? _condition;

        public event EventHandler? CanExecuteChanged;

        public DelegateCommand(Action<T> action, Func<T, bool>? executeCondition = default)
        {
            _action = action ?? throw new ArgumentNullException(nameof(action));
            _condition = executeCondition;
        }

        public bool CanExecute(object parameter)
        {
            if (parameter is T value)
            {
                return _condition?.Invoke(value) ?? true;
            }

            return _condition?.Invoke(default!) ?? true;
        }

        public void Execute(object parameter)
        {
            if (parameter is T value)
            {
                _action(value);
            }
            else
            {
                _action(default!);
            }
        }

        public void RaiseCanExecuteChanged()
            => CanExecuteChanged?.Invoke(this, new EventArgs());
    }
    public class PendingConnectionViewModel
    {
        private readonly EditorViewModel _editor;
        private ConnectorViewModel _source;

        public PendingConnectionViewModel(EditorViewModel editor)
        {
            _editor = editor;
            StartCommand = new DelegateCommand<ConnectorViewModel>(source => _source = source);
            FinishCommand = new DelegateCommand<ConnectorViewModel>(target =>
            {
                if (target != null)
                    _editor.Connect(_source, target);
            });
        }

        public ICommand StartCommand { get; }
        public ICommand FinishCommand { get; }
    }

 

    public class EditorViewModel
    {
        public PendingConnectionViewModel PendingConnection { get; }
        public ObservableCollection<NodeViewModel> Nodes { get; set; } = new ObservableCollection<NodeViewModel>();
        public ObservableCollection<ConnectionViewModel> Connections { get; } = new ObservableCollection<ConnectionViewModel>();
        public Dictionary<string, NodeViewModel> addedNodes = new Dictionary<string, NodeViewModel>();
        public ICommand DisconnectConnectorCommand { get; }
 
        public ICommand AddEditorCommand { get; }
        BG3Data bgData = new BG3Data();



        public void loadNodes()
        {
            bgData.load();
            foreach (string key in bgData.dialogeDictionary.Keys)
            {
                AddRootNodes(bgData, key, AddSpeakerRootNode("todo"));

            }
        }
        public EditorViewModel()
        {
           
           
            Console.WriteLine("BGEdit started");
            PendingConnection = new PendingConnectionViewModel(this);
            

            DisconnectConnectorCommand = new DelegateCommand<ConnectorViewModel>(connector =>
            {
                var connection = Connections.First(x => x.Source == connector || x.Target == connector);
                connection.Source.IsConnected = false;  // This is not correct if there are multiple connections to the same connector
                connection.Target.IsConnected = false;
               
                Connections.Remove(connection);
            });
            loadNodes();



            //Connections are a bit much to view
            //addSpeakerNodes(bgData, bgData.currentContext.currentDialogeNodeUUID);


        }

        Brush defaultBrush = new SolidColorBrush(Colors.MediumAquamarine);
        public void addSpeakerNodes(BG3Data bgData, string uuidOfSpeakerNode)
        {
            if (bgData.speakerList.ContainsKey(uuidOfSpeakerNode))
            {
                Console.WriteLine("Found UUID of Dialoge: " + uuidOfSpeakerNode);
                foreach (Speaker item in bgData.speakerList[uuidOfSpeakerNode].Values)
                {
                    if (!addedNodes.ContainsKey(item.List.Value))
                    {
                        addedNodes.Add(item.List.Value, new NodeViewModel
                        {
                            Title = "Speaker"+'\n'+item.List.Value,
                            Output = new ObservableCollection<ConnectorViewModel>
                        {
                            new ConnectorViewModel
                            {
                                Title = "Out"
                            }
                         }
                        });
                        Console.WriteLine("Added Speaker"+ item.List.Value);
                        Nodes.Add(addedNodes[item.List.Value]);
                        
                    }
                  
                    
                }
            }
        }
        public NodeViewModel AddSpeakerRootNode(string MapKey)
        {
            var SpeakerRootNode = new NodeViewModel
            {
                Title = "MapKey\n"+MapKey,
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
        
        public void addTextToNode(List<String> list, String text)
        {
            list.Add(text);
        }

        public void AddRootNodes(BG3Data bgData,String key, NodeViewModel SpeakerRootNode)
        {
            int x = 1, y = 1;
            Nodes.Add(SpeakerRootNode);

            foreach (RootNode item in bgData.dialogeDictionary[key].Save.Regions.Dialog.Nodes[0].RootNodes)
            {
               
                var tmpNode = new NodeViewModel
                {
                    Title = "RootNode\n"+ item.RootNodes.Value,
                    Input = new ObservableCollection<ConnectorViewModel>
                {
                new ConnectorViewModel
                {
                    Title = "In"
                     }
                     },
                    Output = new ObservableCollection<ConnectorViewModel>
                {
                new ConnectorViewModel
                {
                    Title = "Out"
                }
                     }
                };
                Nodes.Add(tmpNode);
                Connect(SpeakerRootNode.Output[0], tmpNode.Input[0]);
               
                addTextToNode(tmpNode.RootList,item.RootNodes.Value);
                tmpNode.RootsFound = "Visible";
                y++;
                foreach (NodeNode node in bgData.dialogeDictionary[key].Save.Regions.Dialog.Nodes[0].Node)
                {
                    bgData.dialogeNodes[node.Uuid.Value] = node;
                    foreach (var dat in node.EditorData[0].Data)
                    {
                        
                        if (!bgData.editorData.ContainsKey(node.Uuid.Value))
                        {
                            bgData.editorData.Add(node.Uuid.Value, new Dictionary<string, string>());
                        }
                        else
                        {
                            //Console.WriteLine(dat.Key.Value);
                            if (!bgData.editorData[node.Uuid.Value].ContainsKey(dat.Key.Value))
                            {
                                bgData.editorData[node.Uuid.Value].Add(dat.Key.Value, dat.Val.Value);
                            }
                            
                        }
                    }
                }
               
                foreach (NodeNode node in bgData.dialogeDictionary[key].Save.Regions.Dialog.Nodes[0].Node)
                {
                    if (item.RootNodes.Value == node.Uuid.Value)
                    {
                        //Console.WriteLine(node.Uuid.Value);
                        AddNode(tmpNode.Output[0],node, bgData,++x,y++);
                         
                        if (bgData.editorData[node.Uuid.Value].ContainsKey("position"))
                        {
                            string[] words = bgData.editorData[node.Uuid.Value]["position"].Split(';');
                            int tmpx = 0, tmpy = 0;
                            foreach (string word in words)
                            {


                                //System.Console.WriteLine(int.Parse(word));
                                if (tmpx != 0)
                                {
                                    tmpy = (int)(int.Parse(word) * 1.3);
                                }
                                else
                                {
                                    tmpx = (int)(int.Parse(word) * 1.8);
                                }



                            }
                            if (tmpx != 0 && tmpy != 0)
                            {
                                tmpNode.Location = new Point(tmpx-256, tmpy);
                                //SpeakerRootNode.Location = new Point(tmpx - 512, tmpy-128);
                            }
                        }
                    }
                    
                }
                //Add Jumptargets after every node is added
                foreach (NodeNode node in bgData.dialogeDictionary[key].Save.Regions.Dialog.Nodes[0].Node)
                {
                   if(node.Jumptarget != null && addedNodes.ContainsKey(node.Uuid.Value) && addedNodes.ContainsKey(node.Jumptarget.value))
                    {
                        Connect(addedNodes[node.Uuid.Value].Output[0], addedNodes[node.Jumptarget.value].Input[0], new SolidColorBrush(Colors.PeachPuff));
                    }
                }
            }
           
        }

        public void addTaggedTexts(BG3Data bgData, NodeNode node, List<String> tagTextToAdd)
        {
            if (node.TaggedTexts != null)
            {

                foreach (var item in node.TaggedTexts)
                {
                    if (item.TaggedText == null) continue;

                    foreach (var item2 in item.TaggedText)
                    {
                        if (item2.TagTexts == null) continue;
                        if (item2.HasTagRule.Value)
                        {

                            foreach (var rule in item2.RuleGroup)
                            {
                                foreach (var rule2 in rule.Rules)
                                {
                                    if (rule2 == null || rule2.Rule == null) continue;
                                    foreach (var rule3 in rule2.Rule)
                                    {
                                        if (rule2 == null || rule3.Tags == null) continue;
                                        tagTextToAdd.Add("Tag Rule:");
                                        foreach (var rule4 in rule3.Tags)
                                        {
                                            if (rule2 == null || rule4.Tag == null) continue;
                                            foreach (var rule5 in rule4.Tag)
                                            {
                                                tagTextToAdd.Add(rule5.Object.Value + " " + bgData.getTagData(rule5.Object.Value + "") + "\n");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        foreach (var item3 in item2.TagTexts)
                        {
                            if (item3.TagTextArray == null) continue;

                            foreach (var item4 in item3.TagTextArray)
                            {
                                if (bgData.localizationDictionary.ContainsKey(item4.TagText.Handle))
                                {
                                    tagTextToAdd.Add(bgData.localizationDictionary[item4.TagText.Handle].text + "\n __________");

                                }

                            }
                        }
                    }
                }
            }
            bgData.tagedTextAdd.Add(node.Uuid.Value, "True");
        }

        public void addSpeakersToNode(BG3Data bgData, NodeNode node)
        {
            if (node.Speaker != null)
            {
                if (!bgData.speakerAdded.ContainsKey(node.Uuid.Value))
                {
                    var speakerInfo = "";
                    if (bgData.speakerList[bgData.currentContext.currentDialogeNodeUUID].ContainsKey(node.Speaker.Value + ""))
                    {
                        speakerInfo += bgData.getSpeakerInfo(bgData.speakerList[bgData.currentContext.currentDialogeNodeUUID][node.Speaker.Value + ""].List.Value);
                    }

                    //var speakerConnector = new ConnectorViewModel { Title = "Speaker:" + node.Speaker.Value +" "+ speakerInfo };
                    //addedNodes[node.Uuid.Value].Input.Add(speakerConnector);
                    addTextToNode(addedNodes[node.Uuid.Value].SpeakerList, "Speaker: " + node.Speaker.Value);
                    addTextToNode(addedNodes[node.Uuid.Value].SpeakerList, "Speaker Info: " + speakerInfo);
                    addedNodes[node.Uuid.Value].SpeakersFound = "Visible";
                    bgData.speakerAdded.Add(node.Uuid.Value, "True");

                    //Console.WriteLine("Current Searched Speaker"+node.Speaker.Value);

                    foreach (Speaker item in bgData.speakerList[bgData.currentContext.currentDialogeNodeUUID].Values)
                    {
                        if (item.Index.Value == (node.Speaker.Value + ""))
                        {
                            //makes it very messy todo : combine Lines?
                            //Connect(addedNodes[item.List.Value].Output[0], speakerConnector);
                        }
                    }
                }

            }
        }
        public void addCheckFlags(BG3Data bgData, NodeNode node)
        {
            if (node.Checkflags != null)
            {
                foreach (var item in node.Checkflags)
                {
                    if (item.flaggroup != null)
                    {
                        foreach (var item2 in item.flaggroup)
                        {
                            foreach (var item3 in item2.flag)
                            {
                                //Console.WriteLine("Found Flag: " + item3.UUID.value);
                                if (bgData.tagData.ContainsKey(item3.UUID.value) && !(bgData.beenTagged.ContainsKey(node.Uuid.Value)))
                                {
                                    //Console.WriteLine("With Text: " + bgData.tagData[item3.UUID.value].attributes["Description"].Value);
                                    //var conc = new ConnectorViewModel { Title = "Tag:"+ bgData.tagData[item3.UUID.value].attributes["Description"].Value };
                                    //addedNodes[node.Uuid.Value].Input.Add(conc);

                                    addTextToNode(addedNodes[node.Uuid.Value].TagList, "Tag: " + bgData.tagData[item3.UUID.value].attributes["Description"].Value + ": " + item3.value.value);
                                    addedNodes[node.Uuid.Value].TagsFound = "Visible";


                                }
                            }
                        }
                    }
                }
            }
            if (!bgData.beenTagged.ContainsKey(node.Uuid.Value))
            {
                bgData.beenTagged.Add(node.Uuid.Value, "true");
            }
        }
        public void AddNode(ConnectorViewModel source, NodeNode node, BG3Data bgData, int x, int y)
        {
            List<String> groupInfostoAdd = new List<String>();
            List<String> tagTextToAdd = new List<String>();
            Brush Color = new SolidColorBrush(Colors.DimGray);
            if (!addedNodes.ContainsKey(node.Uuid.Value))
            {
                string title = "TemplateNodeUUID\n" + node.Uuid.Value+"\n";
                if (bgData.editorData[node.Uuid.Value].ContainsKey("logicalname"))
                {
                    title += bgData.editorData[node.Uuid.Value]["logicalname"]+"\n";
                    Color = colorNode(node,bgData);
  

                }
                if(node.GroupID != null){

                    groupInfostoAdd.Add("Group ID" +node.GroupID.Value+ "\nGroupIndex: "+ node.GroupIndex.Value+"\n");
                }
                addTaggedTexts(bgData,node, tagTextToAdd);
                addedNodes[node.Uuid.Value] = new NodeViewModel
                {
                    Title = title,
                    Input = new ObservableCollection<ConnectorViewModel>
                {
                new ConnectorViewModel
                {
                    Title = "In"
                     }
                     },
                    Output = new ObservableCollection<ConnectorViewModel>
                {
                new ConnectorViewModel
                {
                    Title = "Out"
                }
                     }
                };
                addedNodes[node.Uuid.Value].Location = new Point(x*256, y*128);
                addedNodes[node.Uuid.Value].HeaderBrushColor = Color;
                Nodes.Add(addedNodes[node.Uuid.Value]);
                foreach (var item in groupInfostoAdd)
                {
                    addTextToNode(addedNodes[node.Uuid.Value].GroupList, item);
                    addedNodes[node.Uuid.Value].GroupsFound = "Visible";
                }
                foreach (var item in tagTextToAdd)
                {
                    addTextToNode(addedNodes[node.Uuid.Value].TagTextList, item);
                    addedNodes[node.Uuid.Value].TagTextFound = "Visible";
                }
                if (bgData.editorData[node.Uuid.Value].ContainsKey("position"))
                {
                    string[] words = bgData.editorData[node.Uuid.Value]["position"].Split(';');
                    int tmpx = 0, tmpy = 0;
                    foreach (string word in words)
                    {
                        
                       
                        //System.Console.WriteLine(int.Parse(word));
                        if (tmpx != 0)
                        {
                            tmpy = (int)(int.Parse(word) * 1.3);
                        }
                        else
                        {
                            tmpx = (int)(int.Parse(word)*1.8);
                        }
                        


                    }
                    if (tmpx != 0 && tmpy != 0)
                    {
                        addedNodes[node.Uuid.Value].Location = new Point(tmpx, tmpy);
                    }
                }
       
        
            }
            //Connect speakers to Nodes
            addSpeakersToNode(bgData,node);
            addCheckFlags(bgData, node);


            Connect(source, addedNodes[node.Uuid.Value].Input[0]);
            foreach (var child in node.children)
            {
                if(child == null || child.children == null) continue; //Trashy stop
                foreach (var child2 in child.children)
                {
                   
                    if (bgData.dialogeNodes.ContainsKey(child2.UUID.value))
                    {
                        AddNode(addedNodes[node.Uuid.Value].Output[0], bgData.dialogeNodes[child2.UUID.value], bgData,++x,y++);
                    }
                    
                }
              
   
            }

        }
        

        public enum NodeType
        {
            Root,
            Logic,
            Jump,
            Question,
            Answer,
            AnswerAlias,
            Group,
            Unknown
        }
        public NodeViewModel createNode(NodeType type)
        {
            switch (type)
            {
                case NodeType.Root:
                    break;
                case NodeType.Logic:
                    break;
                case NodeType.Jump:
                    break;
                case NodeType.Question:
                    break;
                case NodeType.Answer:
                    break;
                case NodeType.AnswerAlias:
                    break;
                case NodeType.Group:
                    break;
                case NodeType.Unknown:
                    break;
                default:
                    break;
            }
            return null;
        }

        public Brush colorNode(NodeNode node, BG3Data bgData)
        {
            Brush color = new SolidColorBrush(Colors.DimGray);
            //Console.WriteLine(node.Constructor.Value);
            if (node.Constructor.Value != null)
            {
                switch (node.Constructor.Value) {
                    case "TagAnswer":
                        color = new SolidColorBrush(Colors.Orange);
                        break;
                    case "RollResult":
                        color = new SolidColorBrush(Colors.Goldenrod);
                        break;
                    case "TagQuestion":
                        color = new SolidColorBrush(Colors.DarkCyan);
                        break;
                    case "Jump":
                        color = new SolidColorBrush(Colors.PaleVioletRed);
                        break;
                    case "ActiveRoll":
                        color = new SolidColorBrush(Colors.DarkKhaki);
                        break;
                    default:
                        color = new SolidColorBrush(Colors.DimGray);
                        break;
                }
            }
            if (node.Endnode != null && node.Endnode.Value)
            {
                color = new SolidColorBrush(Colors.OrangeRed);
                
            }
            else if (node.Root != null && node.Root.Value)
            {
                color = new SolidColorBrush(Colors.OliveDrab);
            
            }
            else if (bgData.editorData[node.Uuid.Value]["logicalname"] == "LOGIC NODE")
            {
                color = new SolidColorBrush(Colors.PowderBlue);
            }

            return color;
        }
        public void Connect(ConnectorViewModel source, ConnectorViewModel target, Brush brush = null)
        {
            var connection = new ConnectionViewModel(source, target);
           
            if(brush == null)
            {
                connection.Stroke = defaultBrush;
            }
            else
            {
                connection.Stroke = brush;
            }
            Connections.Add(connection);
        }


        public void clearCurrentNodes(BG3Data bgData)
        {
            bgData.clearCurrentDialogueData();
        }

    }
}
