using BGEdit.LocalizationStructur;
using BGEdit.SpeakerGroupsStructur;
using Nodify;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using static Nodify.EditorGestures;

namespace BGEdit
{
    public class EditorViewModel
    {
        public PendingConnectionViewModel PendingConnection { get; }
        public ObservableCollection<NodeViewModel> Nodes { get; set; } = new ObservableCollection<NodeViewModel>();
        public ObservableCollection<ConnectionViewModel> Connections { get; } = new ObservableCollection<ConnectionViewModel>();
        public Dictionary<string, NodeViewModel> AddedNodes = new Dictionary<string, NodeViewModel>();
        private Dictionary<ConnectorViewModel, List<ConnectorViewModel>> ToConnect = new Dictionary<ConnectorViewModel, List<ConnectorViewModel>>();
        public ICommand DisconnectConnectorCommand { get; }
        public Point ViewportLocation { get; set; } = new Point(0d,0d);

        private BGData bgData = BGData.Instance;
        private bool startUp = true;
        public EditorViewModel()
        {
            PendingConnection = new PendingConnectionViewModel(this);
            DisconnectConnectorCommand = new DelegateCommand<ConnectorViewModel>(connector =>
            {
                var connection = Connections.First(x => x.Source == connector || x.Target == connector);
                connection.Source.IsConnected = false;  // This is not correct if there are multiple connections to the same connector
                connection.Target.IsConnected = false;
               
                Connections.Remove(connection);
             
            });
           
           
            LoadNodes();
            //Connections are a bit much to view
            //addSpeakerNodes(bgData, bgData.currentContext.currentDialogeNodeUUID);
            startUp = false;
        }
        public void ConnectNodes()
        {
            foreach (var key in ToConnect.Keys)
            {
                foreach (var item in ToConnect[key])
                {
                    Connect(key, item);
                }
            }
        }
        public void LoadNodes()
        {
            bgData.Load();

            foreach (string key in bgData.dialogeDictionary.Keys)
            {
                AddRootNodes(key, GenericNodeActions.AddSpeakerRootNode(key));

            }
            ConnectNodes();
        }
        public void AddSpeakerNodes(String uuidOfSpeakerNode)
        {
            if (bgData.speakerList.ContainsKey(uuidOfSpeakerNode))
            {
                foreach (Speaker item in bgData.speakerList[uuidOfSpeakerNode].Values)
                {
                    var value = item.List.Value;
                    if (!AddedNodes.ContainsKey(value))
                    {
                        AddedNodes.Add(value, new NodeViewModel
                        {
                            Title = $"Speaker\n{value}",
                            Output = new ObservableCollection<ConnectorViewModel>
                            {
                                new ConnectorViewModel
                                {
                                    Title = "Out",
                                }
                            }
                        });
                        Nodes.Add(AddedNodes[value]);
                    }
                }
            }
        }
        public void AddRootNodes(String key, NodeViewModel SpeakerRootNode)
        {
           
            Nodes.Add(SpeakerRootNode);
            foreach (RootNode item in bgData.dialogeDictionary[key].Save.Regions.Dialog.Nodes[0].RootNodes)
            {

                var tmpNode = new NodeViewModel
                {
                    AddChildVisible = "False",
                    RemoveVisible = "False",
                    Title = "RootNode\n" + item.RootNodes.Value,
                    Input = new ObservableCollection<ConnectorViewModel>
                    {
                        new ConnectorViewModel
                        {
                            Title = "In",
                        }
                    },
                    Output = new ObservableCollection<ConnectorViewModel>
                    {
                        new ConnectorViewModel
                        {
                            Title = "Out"
                        }
                    },
                    RootsFound = "Visible",
                    RootCheckbox = true
                };

                Nodes.Add(tmpNode);
                Connect(SpeakerRootNode.Output[0], tmpNode.Input[0]);
                GenericNodeActions.AddTextToNode(tmpNode.RootList, item.RootNodes.Value);

                ForEachNodeInDDict(key, SpeakerRootNode, item, tmpNode);
                //Add Jumptargets after every node is added
                ConnectJumptargets(key);
            }

        }

        private void ConnectJumptargets(string key)
        {
            foreach (NodeNode node in bgData.dialogeDictionary[key].Save.Regions.Dialog.Nodes[0].Node)
            {
                if (node.Jumptarget != null && AddedNodes.ContainsKey(node.Uuid.Value) && AddedNodes.ContainsKey(node.Jumptarget.value))
                {
                    Connect(AddedNodes[node.Uuid.Value].Output[0], AddedNodes[node.Jumptarget.value].Input[0], new SolidColorBrush(Colors.PeachPuff));
                }
            }
        }

        private void ForEachNodeInDDict(string key, NodeViewModel SpeakerRootNode, RootNode item, NodeViewModel tmpNode)
        {
            foreach (NodeNode node in bgData.dialogeDictionary[key].Save.Regions.Dialog.Nodes[0].Node)
            {
                if (item.RootNodes.Value == node.Uuid.Value)
                {
                    //Console.WriteLine(node.Uuid.Value);
                    AddNode(tmpNode.Output[0], node, bgData);

                    if (bgData.editorData[node.Uuid.Value].ContainsKey("position"))
                    {
                        Point nodePoint = GenericNodeActions.LocationStringToPoint(node.Uuid.Value);
                        if (nodePoint.X != 0 && nodePoint.Y != 0)
                        {
                            tmpNode.Location = new Point(nodePoint.X - 256, nodePoint.Y);
                            SpeakerRootNode.Location = new Point(nodePoint.X - 512, nodePoint.Y - 128);
                            ViewportLocation = new Point(nodePoint.X - 512, nodePoint.Y - 128);
                        }
                    }
                }

            }
        }
        public void AddTaggedTexts(NodeNode node, List<string> tagTextToAdd)
        {
            if (node.TaggedTexts == null) return;

            bool hasTagRule = false;
            string tagCombineType = "uint8";
            byte tagCombineValue = 0;
            string lineUUIDNumber = "uuidnumber";
            string localisationUUID = "LocaUUID";
            bool isStub = false;

            foreach (var taggedText in node.TaggedTexts)
            {
                if (taggedText?.TaggedText == null) continue;

                foreach (var tag in taggedText.TaggedText)
                {
                    if (tag.TagTexts == null) continue;

                    hasTagRule = tag.HasTagRule.Value;
                    ProcessRules(tag, tagTextToAdd);
                    ProcessTagTexts(tag, tagTextToAdd, ref lineUUIDNumber, ref localisationUUID, ref isStub);
                }
            }

            AddTaggedText(node, bgData, hasTagRule, tagCombineType, tagCombineValue, lineUUIDNumber, localisationUUID, isStub);
            bgData.tagedTextAdd.TryAdd(node.Uuid.Value, "True");
        }

        private void ProcessRules(TaggedTextTaggedText tag, List<string> tagTextToAdd)
        {
            if (!tag.HasTagRule.Value) return;

            foreach (var ruleGroup in tag.RuleGroup)
            {
                byte tagCombineValue = ruleGroup.TagCombineOp.Value;
                string tagCombineType = ruleGroup.TagCombineOp.Type;

                foreach (var rule2 in ruleGroup.Rules)
                {
                    if (rule2?.Rule == null) continue;

                    foreach (var ruleItem in rule2.Rule)
                    {
                        if (ruleItem?.Tags == null) continue;

                        tagTextToAdd.Add("Tag Rule:");
                        foreach (var ruleTag in ruleItem.Tags)
                        {
                            if (ruleTag?.Tag == null) continue;

                            foreach (var tagItem in ruleTag.Tag)
                            {
                                tagTextToAdd.Add(tagItem.Object.Value + " " + bgData.getTagData(tagItem.Object.Value.ToString()) + "\n");
                            }
                        }
                    }
                }
            }
        }

        private void ProcessTagTexts(TaggedTextTaggedText tag, List<string> tagTextToAdd, ref string lineUUIDNumber, ref string localisationUUID, ref bool isStub)
        {
            foreach (var tagText in tag.TagTexts)
            {
                if (tagText.TagTextArray == null) continue;

                int count = tagText.TagTextArray.Count;
                foreach (var tagTextItem in tagText.TagTextArray)
                {
                    lineUUIDNumber = tagTextItem.LineId.Value;
                    localisationUUID = tagTextItem.TagText.Handle;
                    isStub = tagTextItem.Stub.Value;

                    if (bgData.localizationDictionary.TryGetValue(tagTextItem.TagText.Handle, out var localizedText))
                    {
                        string separator = (--count > 0) ? "\n____" : "";
                        tagTextToAdd.Add(localizedText.text + separator);
                    }
                }
            }
        }
       
        public void AddSpeakersToNode(NodeNode node)
        {
            String nodeUUID = node.Uuid.Value;
            if (node.Speaker != null && !bgData.speakerAdded.ContainsKey(nodeUUID))
            {
                String speakerInfo = "";
                String currentDialogeNodeUUID = bgData.currentContext.currentDialogeNodeUUID;
                NodeViewModel? addedNodes = AddedNodes[nodeUUID];

                if (bgData.speakerList[currentDialogeNodeUUID].ContainsKey(node.Speaker.Value + ""))
                {
                    speakerInfo += bgData.GetSpeakerInfo(bgData.speakerList[currentDialogeNodeUUID][node.Speaker.Value + ""].List.Value);
                }

                GenericNodeActions.AddTextToNode(addedNodes.SpeakerList, "Speaker: " + node.Speaker.Value);
                GenericNodeActions.AddTextToNode(addedNodes.SpeakerList, "Speaker Info: " + speakerInfo);
                addedNodes.SpeakersFound = "Visible";
                bgData.speakerAdded.Add(nodeUUID, "True");

                /*
                //makes it very messy todo : combine Lines?
                //Console.WriteLine("Current Searched Speaker"+node.Speaker.Value);
                //var speakerConnector = new ConnectorViewModel { Title = "Speaker:" + node.Speaker.Value +" "+ speakerInfo };
                //AddedNodes[node.Uuid.Value].Input.Add(speakerConnector);
                foreach (Speaker item in bgData.speakerList[bgData.currentContext.currentDialogeNodeUUID].Values)
                {
                    if (item.Index.value == (node.Speaker.Value + ""))
                    {
                        //Connect(AddedNodes[item.List.Value].Output[0], speakerConnector);
                    }
                }*/
            }
        }

        public void AddCheckFlags(NodeNode node)
        {
            ProcessCheckFlags(node);
            ProcessSetFlag(node);
            bgData.beenTagged.TryAdd(node.Uuid.Value, "true");   
        }

        private void ProcessSetFlag(NodeNode node)
        {
            if (node?.Setflags == null) return;
            foreach (var item in node.Setflags)
            {
                if (item != null && item.flaggroup != null)
                {
                    foreach (var item2 in item.flaggroup)
                    {
                        foreach (var item3 in item2.flag)
                        {
                            String item3UUIDValue = item3.UUID.Value;
                            String nodeUUID = node.Uuid.Value;
                            if (bgData.tagData.ContainsKey(item3UUIDValue) && !(bgData.beenTagged.ContainsKey(nodeUUID)))
                            {
                                GenericNodeActions.AddTextToNode(AddedNodes[nodeUUID].TagSetList, "Set Flag: " 
                                    + bgData.tagData[item3UUIDValue].attributes["Description"].Value + ": " + item3.value.Value);
                                AddedNodes[nodeUUID].TagsToSetFound = "Visible";
                            }
                            if (!(bgData.beenTagged.ContainsKey(nodeUUID)))
                            {
                                Int32 param = item3.paramval?.Value ?? 0;
                                AddSetFlagsEditable(node, true, false, param, item2.type.Value, item3UUIDValue, item3.value.Value);
                            }

                        }
                    }
                }
            }
        }

        private void ProcessCheckFlags(NodeNode node)
        {
            if (node?.Checkflags == null) return;
            foreach (var item in node.Checkflags)
            {
                if (item != null && item.flaggroup != null)
                {
                    foreach (var item2 in item.flaggroup)
                    {
                        foreach (var item3 in item2.flag)
                        {
                            String item3UUIDValue = item3.UUID.Value;
                            String nodeUUID = node.Uuid.Value;
                            if (bgData.tagData.ContainsKey(item3UUIDValue) && !(bgData.beenTagged.ContainsKey(nodeUUID)))
                            {
                                GenericNodeActions.AddTextToNode(AddedNodes[nodeUUID].TagList, "Check Flag: "
                                    + bgData.tagData[item3UUIDValue].attributes["Description"].Value + ": " + item3.value.Value);
                                AddedNodes[nodeUUID].TagsFound = "Visible";

                            }
                            if (!(bgData.beenTagged.ContainsKey(nodeUUID)))
                            {
                                Int32 param = item3.paramval?.Value ?? 0;
                                AddSetFlagsEditable(node, false, false, param, item2.type.Value, item3UUIDValue, item3.value.Value);
                            }

                        }
                    }
                }
            }
        }

        public void AddSetFlagsEditable(NodeNode node, bool checkOrSet = true, bool newFlag = true,Int32 Paramval = 0, String FlagTypeStr = "",String UUID ="", bool Condiction = false)
        {
            //Console.WriteLine("Add Flag called");

            var flagUUID = new TextBox
            {
                Text = newFlag ? Guid.NewGuid().ToString() : UUID,
                
            };
            
            var flagType = new TextBox
            {
                Text = newFlag ? "Local" : FlagTypeStr
            };
            var paramval = new TextBox
            {
                Text = newFlag ? 0+"" : Paramval+""
            };
            var flagValue = new CheckBox
            {
                IsChecked = newFlag || Condiction
            };

            var removeTag = new Button
            {
                Content = "Remove"
            };

            flagUUID.TextChanged += (s, e) =>
             {
                 if (!startUp)
                 {
                     TextBox change = s as TextBox;
                     //todo handle changes separtly in BGData!
                     if (change != null)
                     {
                         int parVal = 0;

                         Int32.TryParse(paramval.Text, out parVal);
                         bgData.AddOrUpdateFlag(node,change.Text, flagType.Text,flagValue.IsChecked.Value, parVal, checkOrSet);
                     }

                 }

             };
            flagType.TextChanged += (s, e) =>
            {
                if (!startUp)
                {
                    TextBox change = s as TextBox;
                    //todo handle changes separtly in BGData!
                    if (change != null)
                    {
                        int parVal = 0;

                        Int32.TryParse(paramval.Text, out parVal);
                        bgData.AddOrUpdateFlag(node, flagUUID.Text, change.Text, flagValue.IsChecked.Value, parVal, checkOrSet);
                    }

                }

            };
            paramval.TextChanged += (s, e) =>
            {
                if (!startUp)
                {
                    TextBox change = s as TextBox;
                    //todo handle changes separtly in BGData!
                    if (change != null)
                    {
                        int parVal = 0;

                        Int32.TryParse(change.Text, out parVal);
                        bgData.AddOrUpdateFlag(node, flagUUID.Text, flagType.Text, flagValue.IsChecked.Value, parVal, checkOrSet);
                    }

                }

            };

            flagValue.Checked += (s, e) =>
            {
                if (!startUp)
                {
                    CheckBox change = s as CheckBox;
                    if (change != null)
                    {
                        int parVal = 0;

                        Int32.TryParse(paramval.Text, out parVal);
                        bgData.AddOrUpdateFlag(node, flagUUID.Text, flagType.Text, change.IsChecked.Value, parVal, checkOrSet);
                    }

                }

            };
            flagValue.Unchecked += (s, e) =>
            {
                if (!startUp)
                {
                    CheckBox change = s as CheckBox;
                    if (change != null)
                    {
                        int parVal = 0;

                        Int32.TryParse(paramval.Text, out parVal);
                        bgData.AddOrUpdateFlag(node, flagUUID.Text, flagType.Text, change.IsChecked.Value, parVal, checkOrSet);
                    }

                }

            };
            var panel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Children = { flagUUID, flagValue, flagType, paramval,removeTag }
            };
           
            // Check if the node exists in AddedNodes dictionary
            if (AddedNodes.TryGetValue(node.Uuid.Value, out var foundNode))
            {
                int parVal = 0;

                Int32.TryParse(paramval.Text, out parVal);
                if (checkOrSet)
                {
                    removeTag.Command = new DelegateCommand(() => {
                        foundNode.SetFlagEditable.Remove(panel);
                        bgData.RemoveFlag(node, flagUUID.Text,true);
                    });
                    foundNode.SetFlagEditable.Add(panel);
                    if(!startUp) bgData.AddOrUpdateFlag(node, flagUUID.Text, flagType.Text, flagValue.IsChecked.Value, parVal,true);
                }
                else
                {
                    removeTag.Command = new DelegateCommand(() => {
                        foundNode.CheckFlagEditable.Remove(panel);
                        bgData.RemoveFlag(node, flagUUID.Text);
                        });
                    foundNode.CheckFlagEditable.Add(panel);
                    if (!startUp)
                    {
                        bgData.AddOrUpdateFlag(node, flagUUID.Text, flagType.Text, flagValue.IsChecked.Value, parVal, false);
                    }
                }
            }
        }

        private Dictionary<String, String> keyValOld = new Dictionary<String, String>();
        public void AddEditorData(NodeNode node, bool newFlag = true, String keyValText="",String valTypeText="", String valValueText="")
        {
            //EditorDataEditable
            StackPanel dockPanel = new StackPanel { Orientation = Orientation.Horizontal };
            TextBox keyVal = new TextBox();
            TextBox valValue = new TextBox();
            Button removeEditorData = new Button
            {
                Content = "Remove"
              
            };
            removeEditorData.Command = new DelegateCommand(() => {
                bgData.RemoveEditorData(node, keyVal.Text);
                if (AddedNodes.TryGetValue(node.Uuid.Value, out var foundNode))
                {

                    foundNode.EditorDataEditable.Remove(dockPanel);

                }
            });
            keyVal.Uid = Guid.NewGuid().ToString();
           
            keyVal.TextChanged += (s, e) =>
            {
                TextBox change = s as TextBox;
                //todo handle changes in BGData!
                if (!startUp)
                {
                    if (change != null)
                    {
                        if (keyValOld.TryGetValue(keyVal.Uid, out string oldValue))
                        {
                            bgData.UpdateEditorAttribute(node, change.Text, valValue.Text, oldValue);
                        }
                        else
                        {
                            bgData.UpdateEditorAttribute(node, change.Text, valValue.Text, change.Text);
                        }
                        
                        keyValOld[keyVal.Uid] = change.Text;
                    }
                }
            };
            valValue.Uid = Guid.NewGuid().ToString();
            valValue.TextChanged += (s, e) =>
            {
                if (!startUp)
                {
                    TextBox change = s as TextBox;
                    //todo handle changes separtly in BGData!
                    if(change != null)
                    {
                        if (keyValOld.TryGetValue(keyVal.Uid, out string oldValue))
                        {
                            bgData.UpdateEditorAttribute(node, keyVal.Text, change.Text, keyValOld[keyVal.Uid]);
                        }
                        else
                        {
                            bgData.UpdateEditorAttribute(node, keyVal.Text, change.Text, keyVal.Text);
                        }
                        
                    }
                   
                }
           
            };
            if (newFlag)
            {
                keyVal.Text = "logicalname";
                valValue.Text = "Pop";
            }
            else
            {
                keyVal.Text = keyValText;
                valValue.Text = valValueText;
            }
            keyValOld.TryAdd(keyVal.Uid, keyVal.Text);
            dockPanel.Children.Add(keyVal);
            dockPanel.Children.Add(valValue);
            dockPanel.Children.Add(removeEditorData);
            if (AddedNodes.TryGetValue(node.Uuid.Value, out var foundNode))
            {

                foundNode.EditorDataEditable.Add(dockPanel);

            }
        }
        public void AddChildrenNode(NodeNode parentNode)
        {
            NodeNode newNode = new NodeNode();
            bgData.InitNode(newNode, NodeType.Answer);
            newNode.Uuid.Value = Guid.NewGuid().ToString();
            newNode.Uuid.Type = "FixedString";
            bgData.AddDialogueToBgData(newNode);
            AddNode(AddedNodes[parentNode.Uuid.Value].Output[0], newNode,bgData);
            AddedNodes[newNode.Uuid.Value].Location = AddedNodes[parentNode.Uuid.Value].Location;
            bgData.AddNodeToDialogueNode(newNode);
            Connect(AddedNodes[parentNode.Uuid.Value].Output[0], AddedNodes[newNode.Uuid.Value].Input[0]);
            //Connection in data is added in connect()
            //AddConnectionInData(parentNode, newNode);
            
        }
        public void AddTaggedText(NodeNode node,BGData bgData, bool hasTagRule = false, String tagcombineType = "uint8", Byte tagCombineValue = 0, String lineUUIDNumber="uuidnumber", String LocalisationUUID = "LocaUUID", bool isStub = false)
        {
            StackPanel mainStackPanel = new StackPanel();
            NodeViewModel tmp = AddedNodes[node.Uuid.Value];
            tmp.HasTagRule = hasTagRule;
            tmp.TagCombineType = tagcombineType;
            tmp.TagCombineValue = tagCombineValue;
            tmp.LineUUIDNumber = lineUUIDNumber;
            tmp.LocalisationUUID = LocalisationUUID;
            tmp.IsStub = isStub;

            // Rules Section AddedNodes[node.Uuid.value].IsStub 
            CheckBox hasTagRuleCheckBox = new CheckBox() { Content = "HasTagRule" };
            GenericNodeActions.CreateBinding(hasTagRuleCheckBox, tmp, "HasTagRule", CheckBox.IsCheckedProperty);
            
            TextBox TagCombineType = new TextBox(){ Text = AddedNodes[node.Uuid.Value].TagCombineType };
            GenericNodeActions.CreateBinding(TagCombineType, tmp, "TagCombineType", TextBox.TextProperty);

            TextBox TagCombineValue =  new TextBox() { Text = AddedNodes[node.Uuid.Value].TagCombineValue + "" };
            GenericNodeActions.CreateBinding(TagCombineValue, tmp, "TagCombineValue", TextBox.TextProperty);

            StackPanel ruleGroup = new StackPanel();
            StackPanel tagCombineStackPanel = new StackPanel() { Orientation = Orientation.Horizontal };

            tagCombineStackPanel.Children.Add(new TextBlock() { Text = "TagCombineOp"});
            tagCombineStackPanel.Children.Add(TagCombineType);
            tagCombineStackPanel.Children.Add(TagCombineValue);

            //todo add rules
            ruleGroup.Children.Add(new StackPanel()); 
            ruleGroup.Children.Add(tagCombineStackPanel);

            // Tagged Text Section
            StackPanel taggedTextStackPanel = new StackPanel();
            StackPanel linePanel = new StackPanel() { Orientation = Orientation.Horizontal };
            TextBox LineUUIDNumber = new TextBox() { Text = AddedNodes[node.Uuid.Value].LineUUIDNumber };
            GenericNodeActions.CreateBinding(LineUUIDNumber, tmp, "LineUUIDNumber", TextBox.TextProperty);
            linePanel.Children.Add(new TextBlock() { Text = "LineID"});
            linePanel.Children.Add(LineUUIDNumber);

            taggedTextStackPanel.Children.Add(linePanel);


            TextBox LocalisationUUIDTB = new TextBox() { Text = AddedNodes[node.Uuid.Value].LocalisationUUID };
            GenericNodeActions.CreateBinding(LocalisationUUIDTB, tmp, "LocalisationUUID", TextBox.TextProperty);
            StackPanel tagTextStackPanel = new StackPanel() { Orientation = Orientation.Horizontal };
            tagTextStackPanel.Children.Add(new TextBox() { Text = "handle", IsReadOnly = true });
            tagTextStackPanel.Children.Add(LocalisationUUIDTB);

            // Stub Section
            CheckBox isStubCheckBox = new CheckBox() { IsChecked = AddedNodes[node.Uuid.Value].IsStub, Content = "Stub" };
            GenericNodeActions.CreateBinding(isStubCheckBox, tmp, "IsStub", CheckBox.IsCheckedProperty);
            // Add all sections to mainStackPanel

            mainStackPanel.Children.Add(GenericNodeActions.AddBorderToStack(hasTagRuleCheckBox));
            mainStackPanel.Children.Add(GenericNodeActions.AddBorderToStack(ruleGroup));
            mainStackPanel.Children.Add(GenericNodeActions.AddBorderToStack(taggedTextStackPanel));
            mainStackPanel.Children.Add(GenericNodeActions.AddBorderToStack(tagTextStackPanel));
            mainStackPanel.Children.Add(GenericNodeActions.AddBorderToStack(isStubCheckBox));
            if (AddedNodes.TryGetValue(node.Uuid.Value, out var foundNode))
            {
                foundNode.TaggedTextDataEditable.Add(mainStackPanel);
            }
            else
            {
                Console.WriteLine("Didnt find node");
            }
            //TaggedTextDataEditable.Add(mainStackPanel);
        }
        public void AddNode(ConnectorViewModel source, NodeNode node, BGData bgData)
        {
            List<String> groupInfostoAdd = new List<String>();
            List<String> tagTextToAdd = new List<String>();
            Brush Color = new SolidColorBrush(Colors.DimGray);
            string title = "TemplateNodeUUID";
            string nodeType = "";
            String nUUID = node.Uuid.Value;
            if (!AddedNodes.ContainsKey(nUUID))
            {

                if (bgData.editorData.TryGetValue(nUUID, out var nodeData) && nodeData.ContainsKey("logicalname"))
                {
                    nodeType = nodeData["logicalname"];
                    Color = GenericNodeActions.ColorNode(node, bgData);
                }
                if (node.GroupID != null)
                {

                    groupInfostoAdd.Add("Group ID" + node.GroupID.Value + "\nGroupIndex: " + node.GroupIndex.Value + "\n");
                }

                CreateDefaultNode(node, Color, title, nodeType);
                //Connect to Events
                AddedNodes[nUUID].PropertyChanged += EditorViewModel_PropertyChangedType;
                Nodes.Add(AddedNodes[nUUID]);
                AddTaggedTexts(node, tagTextToAdd);


            }
          
            PopulateNodeWithData(source, node, bgData, groupInfostoAdd, tagTextToAdd);
            AddNodeRecursion(node, bgData);
            GenericNodeActions.AddInfoToNodeView(node, AddedNodes[nUUID]);
            
        }

        private void AddNodeRecursion(NodeNode node, BGData bgData)
        {
            foreach (var child in node.children)
            {
                if (child == null || child.children == null) continue; //Trashy stop
                foreach (var child2 in child.children)
                {

                    if (bgData.dialogeNodes.ContainsKey(child2.UUID.value))
                    {
                        bgData.dialogeNodes[child2.UUID.value].Parents.Add(node.Uuid.Value);
                        AddNode(AddedNodes[node.Uuid.Value].Output[0], bgData.dialogeNodes[child2.UUID.value], bgData);

                    }
                }
            }
        }

        private void PopulateNodeWithData(ConnectorViewModel source, NodeNode node, BGData bgData, List<string> groupInfostoAdd, List<string> tagTextToAdd)
        {
            foreach (var item in groupInfostoAdd)
            {
                GenericNodeActions.AddTextToNode(AddedNodes[node.Uuid.Value].GroupList, item);
                AddedNodes[node.Uuid.Value].GroupsFound = "Visible";
            }
           foreach (var item in tagTextToAdd)
            {
                GenericNodeActions.AddTextToNode(AddedNodes[node.Uuid.Value].TagTextList, item);
                AddedNodes[node.Uuid.Value].TagTextFound = "Visible";
            }
            //addEditorData(BG3Data bgData, NodeNode node, bool newFlag = true, String keyValText="",String valTypeText="", String valValueText="")
            long count = 0;
            if (!bgData.editorDataAdded.ContainsKey(node.Uuid.Value))
            {
                foreach (var key in bgData.editorData[node.Uuid.Value].Keys)
                {
                    AddEditorData(node, false, key, "LSString", bgData.editorData[node.Uuid.Value][key]);
                }
                bgData.editorDataAdded.TryAdd(node.Uuid.Value, "true");
            }
       
          
            if (bgData.editorData.ContainsKey(node.Uuid.Value) && bgData.editorData[node.Uuid.Value].ContainsKey("position"))
            {

                AddedNodes[node.Uuid.Value].Location = GenericNodeActions.LocationStringToPoint(node.Uuid.Value);

            }

            //Connect speakers to Nodes
            AddSpeakersToNode(node);
            AddCheckFlags(node);
           if (!ToConnect.ContainsKey(source))
            {

                ToConnect.Add(source, new List<ConnectorViewModel>());
            }
            if (!ToConnect[source].Contains(AddedNodes[node.Uuid.Value].Input[0]))
            {
                ToConnect[source].Add(AddedNodes[node.Uuid.Value].Input[0]);
            }
        }

        private void CreateDefaultNode(NodeNode node, Brush Color, string title, string nodeType)
        {
            AddedNodes[node.Uuid.Value] = new NodeViewModel
            {
                Title = title,
                Type = nodeType,
                UUID = node.Uuid.Value,
                RootCheckbox = node.Root != null ? node.Root.Value : false,
                EndCheckbox = node.Endnode != null ? node.Endnode.Value : false,
                OptionalCheckbox = node.optional != null ? node.optional.Value : false,
                PopLevel = node.PopLevel != null ? node.PopLevel.Value : 0,
                AddChildren = new DelegateCommand(() =>
                {
                    AddChildrenNode(node);
                }),
                AddSetFlag = new DelegateCommand(() =>
                {
                    AddSetFlagsEditable(node);
                }),
                AddCheckFlag = new DelegateCommand(() =>
                {
                    AddSetFlagsEditable(node, false);
                }),
                AddEditorData = new DelegateCommand(() =>
                {
                    AddEditorData(node);
                }),
                RemoveNode = new DelegateCommand(() =>
                {
                    RemoveVisualNode(node);

                }),
                Input = new ObservableCollection<ConnectorViewModel>
                {
                new ConnectorViewModel
                {
                    Title = "In",
                    parentUUID = node.Uuid.Value,

                     }
                     },
                Output = new ObservableCollection<ConnectorViewModel>
                {
                new ConnectorViewModel
                {
                    Title = "Out",
                    parentUUID = node.Uuid.Value,
                }
                     }
            };
            AddedNodes[node.Uuid.Value].HeaderBrushColor = Color;
        }

        private void RemoveVisualNode(NodeNode node)
        {
            if (node?.Uuid?.Value == null || !AddedNodes.ContainsKey(node.Uuid.Value))
                return;

            bgData.RemoveNode(node);

            NodeViewModel nodeViewModel = AddedNodes[node.Uuid.Value];
            RemoveConnectionsFromNode(nodeViewModel.Input);
            RemoveConnectionsFromNode(nodeViewModel.Output);

            Nodes.Remove(nodeViewModel);
            AddedNodes.Remove(node.Uuid.Value);
        }

        private void RemoveConnectionsFromNode(ObservableCollection<ConnectorViewModel> connections)
        {
            foreach (var connection in connections)
            {
                foreach (var conn in connection.connections)
                {
                    Connections.Remove(conn);
                }
            }
        }

        private void EditorViewModel_PropertyChangedType(object? sender, PropertyChangedEventArgs e)
        {
            if (!startUp) {
                //Console.WriteLine("PropertyChangedType: " + e.PropertyName);
                NodeViewModel? nvSender = sender as NodeViewModel;
                if (!(nvSender != null && bgData.dialogeNodes.ContainsKey(nvSender.UUID))) return;
                switch (e.PropertyName){
                    case "Location":
                        bgData.UpdateLocation(bgData.dialogeNodes[nvSender.UUID],(int)nvSender.Location.X,(int)nvSender.Location.Y);
                        break;
                    case "UUID":
                        /* todo -> change all refs to new uuid.
                        if (nvSender != null)
                        {
                            string theText = nvSender.Type;
                            Console.WriteLine(theText + " # " + nvSender.UUID);
                            if (AddedNodes.ContainsKey(nvSender.UUID))
                            {
                                AddedNodes[nvSender.UUID].HeaderBrushColor = new SolidColorBrush(Colors.Orange);
                            }
                            bgData.UpdateUUID(bgData.dialogeNodes[nvSender.UUID],nvSender.UUID);
                        }*/
                        break;
                    case "PopLevel":
                        bgData.UpdatePopLevel(bgData.dialogeNodes[nvSender.UUID], nvSender.PopLevel);
                        //Console.WriteLine("New Poplevel: " + nvSender.PopLevel);
                        break;
                    case "RootCheckbox":
                        bgData.UpdateRootCheckBox(bgData.dialogeNodes[nvSender.UUID],nvSender.RootCheckbox);
                        //Console.WriteLine("Is checked: "+ nvSender.RootCheckbox);
                        break;
                    case "OptionalCheckbox":
                        bgData.UpdateOptionalCheckbox(bgData.dialogeNodes[nvSender.UUID], nvSender.OptionalCheckbox);
                        //Console.WriteLine("Is checked: " + nvSender.OptionalCheckbox);
                        break;
                    case "EndCheckbox":
                        bgData.UpdateEndCheckBox(bgData.dialogeNodes[nvSender.UUID], nvSender.EndCheckbox);
                        //Console.WriteLine("Is checked: " + nvSender.EndCheckbox);
                        break;
                    case "Type":
                        break;
                    case "Title":
                        break;
                    case "GroupID":
                        bgData.UpdateGroupID(bgData.dialogeNodes[nvSender.UUID],nvSender.GroupID);
                        break;
                    case "GroupIndex":
                        
                        int tmp;
                        if (Int32.TryParse(nvSender.GroupIndex, out tmp))
                        {
                            bgData.UpdateGroupIndex(bgData.dialogeNodes[nvSender.UUID], tmp);
                        }
                        
                        break;
                    case "TaggedTexts":
                        //Changed directly in code.
                        break;
                    case "Constructor":
                          bgData.UpdateConstructor(bgData.dialogeNodes[nvSender.UUID],nvSender.Constructor);
                          break;
                    case "Speaker":
                        int speaker = -1;

                        if (Int32.TryParse(nvSender.Speaker, out speaker))
                        {
                            bgData.UpdateSpeaker(bgData.dialogeNodes[nvSender.UUID], speaker);
                        }
                       
                        break;
                    case "HasTagRule":
                        bgData.UpdateHasTagRule(bgData.dialogeNodes[nvSender.UUID], nvSender.HasTagRule);
                        //Console.WriteLine(nvSender.HasTagRule);
                        break;
                    case "TagCombineType":
                        bgData.UpdateTagCombineType(bgData.dialogeNodes[nvSender.UUID], nvSender.TagCombineType);
                        //Console.WriteLine(nvSender.TagCombineType);
                        break;
                    case "TagCombineValue":
                      
                        bgData.UpdateTagCombineValue(bgData.dialogeNodes[nvSender.UUID], nvSender.TagCombineValue);
                        //Console.WriteLine(nvSender.TagCombineValue);
                       
                        break;
                    case "LineUUIDNumber":
                       
                        bgData.UpdateLineUUIDNumber(bgData.dialogeNodes[nvSender.UUID], nvSender.LineUUIDNumber);
                        //Console.WriteLine(nvSender.LineUUIDNumber);
                        break;
                    case "LocalisationUUID":
                        bgData.UpdateLocalisationUUID(bgData.dialogeNodes[nvSender.UUID], nvSender.LocalisationUUID);
                        //Console.WriteLine(nvSender.LocalisationUUID);
                        break;
                    case "IsStub":
                        bgData.UpdateIsStub(bgData.dialogeNodes[nvSender.UUID],nvSender.IsStub);
                        //Console.WriteLine(nvSender.IsStub);
                        break;
                    default:
                        break;
                }
                

            }
        }
        public void Connect(ConnectorViewModel source, ConnectorViewModel target, Brush? brush = null)
        {
            var connection = new ConnectionViewModel(source, target)
            {
                Stroke = brush ?? Colorization.defaultBrush
            };
            source.connections.Add(connection);
            target.connections.Add(connection);
            if (!startUp)
            {
                Console.WriteLine("Connected not in startup source UUID: "+source.parentUUID+" || target UUID:" +target.parentUUID);
              
                if (source.parentUUID == target.parentUUID)
                {
                    return;
                }else if (bgData.dialogeNodes.ContainsKey(target.parentUUID) && bgData.dialogeNodes.ContainsKey(source.parentUUID))
                {
                    Console.WriteLine("Connected not in startup source UUID in data: " + source.parentUUID + " || target UUID:" + target.parentUUID);
                    bgData.AddConnectionInData(bgData.dialogeNodes[source.parentUUID], bgData.dialogeNodes[target.parentUUID]);
                }
            }
            Connections.Add(connection);
        }
        
    }
}
