using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Newtonsoft.Json;
using BGEdit.LocalizationStructur;
using System.Windows;
using BGEdit.GenericStructures;
using System.Windows.Input;
using System.Text;
using Microsoft.Win32;
using System.Text.RegularExpressions;
using BGEdit.SpeakerGroupsStructur;
using System.Linq;

namespace BGEdit
{
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
    //In what context is the data currently?: Later dictionary with every context and switchable if multiple instances are used.

    public class BGDataContext
    {
        public String currentDialogeNodeUUID = "";
        public Dictionary<string,Speaker> currentSpeakerInDialogeContext = new Dictionary<string,Speaker>();
        public String rootNodeId = "";

        public void reset()
        {
            currentDialogeNodeUUID = "";
            rootNodeId = "";
            currentSpeakerInDialogeContext.Clear(); 
        }
    }

    public class BGData
    {
     
        public Dictionary<string, Content> localizationDictionary = new Dictionary<string,Content>();
        public Dictionary<string, DialogueStructurRoot> dialogeDictionary = new Dictionary<string, DialogueStructurRoot>();
        //Nodes that are found, add new node here
        public Dictionary<string, NodeNode> dialogeNodes = new Dictionary<string, NodeNode>();
        public Dictionary<string, Dictionary<string, string>> editorData = new Dictionary<string, Dictionary<string, string>>();
        public Dictionary<string, BGEdit.TagStructur.TagStructurRoot> tagData = new Dictionary<string, BGEdit.TagStructur.TagStructurRoot>();

        public Dictionary<string, Dictionary<string, Speaker>> speakerList = new Dictionary<string, Dictionary<string, Speaker>>();
        public Dictionary<string, Dictionary<string, string>> speakerGroups = new Dictionary<string, Dictionary<string, string>>();
        public Dictionary<string, BGEdit.MergedCharsStructure.Node> mergedCharData= new Dictionary<string, BGEdit.MergedCharsStructure.Node>();
        public Dictionary<string, BGEdit.OriginStructur.Node> originsData = new Dictionary<string, OriginStructur.Node>();
        
        //For keeping oversight off the current context (for loading multiple)
        public BGDataContext currentContext = new BGDataContext();
        public Dictionary<string, string> beenTagged = new Dictionary<string, string>();
        public Dictionary<string, string> speakerAdded = new Dictionary<string, string>();
        public Dictionary<string, string> tagedTextAdd = new Dictionary<string, string>();
        public Dictionary<string, string> editorDataAdded = new Dictionary<string, string>();
        public Dictionary<string, string> nodePopulated = new Dictionary<string, string>();

        public BGEdit.ConfigStructur.ConfigFile config = new BGEdit.ConfigStructur.ConfigFile();
        //BGEdit.MergedCharsStructure.Save.Region.Node.Children.Node

        public BGData() { }

        private static BGData? _instance;
        public static BGData Instance
        {
            set
            {
                _instance = value;   
            }
            get
            {
                if(_instance == null) _instance = new BGData();
                return _instance;
            }
        }

        public void InitNode(NodeNode node, NodeType nodeType)
        {
            node.NestedDialogNodeUUID = new TypeValPairStr();
            node.SpeakerLinking = new List<SpeakerLinkingEntries>();
            node.GameData = new List<GameData>();
            node.GroupID = new TypeValPairStr();
            node.GroupIndex = new TypeValPairInt32();
            node.TaggedTexts = new List<NodeTaggedText>();
            node.Tags = new List<TagTag>();
            node.Uuid = new TypeValPairStr();
            node.Checkflags = new List<Checkflag>();
            node.children = new List<Children>();
            node.Constructor = new TypeValPairStr();
            node.EditorData = new List<EditorDataNode>();
            node.Speaker = new NextNodeId();
            node.Jumptarget = new Jumptarget();
            node.Root = new TypeValPairBool("bool", false);
            node.Endnode = new TypeValPairBool("bool", false);
            node.Setflags = new List<Flags>();
            NodeTypeSpecificActions(node, nodeType);
            AddDefaultEditorData(node);
        }
        private void NodeTypeSpecificActions(NodeNode node, NodeType nodeType)
        {
            switch (nodeType)
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
                    Console.WriteLine("Answer");
                    node.Constructor = new TypeValPairStr();
                    node.Constructor.Type = "FixedString";
                    node.Constructor.Value = "TagAnswer";
                    break;
                case NodeType.AnswerAlias:
                    break;
                case NodeType.Group:
                    break;
                case NodeType.Unknown:
                    break;
                default:
                    node.Constructor = new TypeValPairStr();
                    node.Constructor.Type = "FixedString";
                    node.Constructor.Value = "TagAnswer";
                    break;
            }
        }
        public void AddDefaultEditorData(NodeNode node)
        {
            node.EditorData = new List<EditorDataNode>()
            {
                new EditorDataNode
                {
                    Data = new List<EditorDataNodeData>()
                    {
                        new EditorDataNodeData(new TypeValPairStr("FixedString","AnimationTags"),new TypeValPairStr("LSString","")),
                        new EditorDataNodeData(new TypeValPairStr("FixedString","Attitude"),new TypeValPairStr("LSString","Default")),
                        new EditorDataNodeData(new TypeValPairStr("FixedString", "CinematicNodeContext"), new TypeValPairStr("LSString", "")),
                        new EditorDataNodeData(new TypeValPairStr("FixedString", "CinematicObjects"), new TypeValPairStr("LSString", "")),
                        new EditorDataNodeData(new TypeValPairStr("FixedString", "CustomCineArtKeysPresent"), new TypeValPairStr("LSString", "False")),
                        new EditorDataNodeData(new TypeValPairStr("FixedString", "CustomLightingPresent"), new TypeValPairStr("LSString", "False")),
                        new EditorDataNodeData(new TypeValPairStr("FixedString", "CustomSoundPresent"), new TypeValPairStr("LSString", "False")),
                        new EditorDataNodeData(new TypeValPairStr("FixedString", "Emotion"), new TypeValPairStr("LSString", "Default")),
                        new EditorDataNodeData(new TypeValPairStr("FixedString", "ForceDisableIsCustomNode"), new TypeValPairStr("LSString", "False")),
                        new EditorDataNodeData(new TypeValPairStr("FixedString", "ID"), new TypeValPairStr("LSString", "N884")),
                        new EditorDataNodeData(new TypeValPairStr("FixedString", "InternalNodeContext"), new TypeValPairStr("LSString", "")),
                        new EditorDataNodeData(new TypeValPairStr("FixedString", "IsCustomNode"), new TypeValPairStr("LSString", "False")),
                        new EditorDataNodeData(new TypeValPairStr("FixedString", "NodeContext"), new TypeValPairStr("LSString", "")),
                        new EditorDataNodeData(new TypeValPairStr("FixedString", "Quality"), new TypeValPairStr("LSString", "Bronze")),
                        new EditorDataNodeData(new TypeValPairStr("FixedString", "SFXTags"), new TypeValPairStr("LSString", "")),
                        new EditorDataNodeData(new TypeValPairStr("FixedString", "TemplateNodeUUID"), new TypeValPairStr("LSString", "00000000-0000-0000-0000-000000000000")),
                        new EditorDataNodeData(new TypeValPairStr("FixedString", "TemplateVersion"), new TypeValPairStr("LSString", "0")),
                        new EditorDataNodeData(new TypeValPairStr("FixedString", "VFXTags"), new TypeValPairStr("LSString", "")),
                        new EditorDataNodeData(new TypeValPairStr("FixedString", "logicalname"), new TypeValPairStr("LSString", "Answer")),
                        new EditorDataNodeData(new TypeValPairStr("FixedString", "position"), new TypeValPairStr("LSString", "0;0")),
                        new EditorDataNodeData(new TypeValPairStr("FixedString", "sourcetemplate"), new TypeValPairStr("LSString", ""))

                    }
                }
            };
          


        }
        public bool AddOrUpdateNode(NodeNode node)
        {
            try
            {
                if (node != null )
                {
                    var uuid = node.Uuid.Value;
                    if (!dialogeNodes.ContainsKey(uuid))
                    {
                        dialogeNodes.Add(uuid, node);
                        return true;
                    }
                    dialogeNodes[uuid] = node;
                    return true;
                }
               
            }catch (Exception ex) { 
                Console.WriteLine(ex.Message);
                return false;
            }
            return false;
        }

        public void RemoveConnection(String UUIDFrom, String UUIDTo)
        {
            if (dialogeNodes.ContainsKey(UUIDFrom) && dialogeNodes.ContainsKey(UUIDTo))
            {
                foreach (var children in dialogeNodes[UUIDFrom].children)
                {
                    foreach(var child in children.children)
                    {
                        //found Connection
                        if (child.UUID.value == UUIDTo)
                        {
                            NodeNode node = dialogeNodes[UUIDFrom];
                            node.children[0].children.Remove(child);
                            AddOrUpdateNode(node);
                        }
                    }
                }
            }
        }

        public void RemoveNode(NodeNode node)
        {
            String uuid = node.Uuid.Value;
            foreach (String ParentUUID in node.Parents)
            {
                RemoveConnection(ParentUUID, uuid);
            }
            if (dialogeNodes.ContainsKey(uuid))
            {
                dialogeNodes.Remove(uuid);
            }
        }

        private bool IsValidNode(NodeNode node)
        {
            return node != null && node.TaggedTexts != null && node.TaggedTexts.Count > 0;
        }

        private bool IsValidTaggedText(NodeNode node, int index)
        {
            return IsValidNode(node) && index >= 0 && index < node.TaggedTexts[0].TaggedText.Count && node.TaggedTexts[0].TaggedText[index] != null;
        }

        private bool IsValidRuleGroup(NodeNode node, int index, int rgIndex)
        {
            return IsValidTaggedText(node, index) && node.TaggedTexts[0].TaggedText[index].RuleGroup != null && rgIndex >= 0 && rgIndex < node.TaggedTexts[0].TaggedText[index].RuleGroup.Count;
        }

        private bool IsValidTagText(NodeNode node, int index, int tgIndex)
        {
            return IsValidTaggedText(node, index) && node.TaggedTexts[0].TaggedText[index].TagTexts != null && tgIndex >= 0 && tgIndex < node.TaggedTexts[0].TaggedText[index].TagTexts.Count;
        }

        private bool IsValidTagTextArray(NodeNode node, int index, int tgIndex, int tgArrayIndex)
        {
            return IsValidTagText(node, index, tgIndex) && node.TaggedTexts[0].TaggedText[index].TagTexts[tgIndex].TagTextArray != null && tgArrayIndex >= 0 && tgArrayIndex < node.TaggedTexts[0].TaggedText[index].TagTexts[tgIndex].TagTextArray.Count;
        }

        public void UpdateHasTagRule(NodeNode node, bool value, int index = 0)
        {
            if (IsValidTaggedText(node, index) && node.TaggedTexts[0].TaggedText[index].HasTagRule != null)
            {
                node.TaggedTexts[0].TaggedText[index].HasTagRule.Value = value;
                AddOrUpdateNode(node);
            }
        }

        public void UpdateTagCombineType(NodeNode node, string value, int index = 0, int rgIndex = 0)
        {
            if (IsValidRuleGroup(node, index, rgIndex) && node.TaggedTexts[0].TaggedText[index].RuleGroup[rgIndex].TagCombineOp != null)
            {
                node.TaggedTexts[0].TaggedText[index].RuleGroup[rgIndex].TagCombineOp.Type = value;
                AddOrUpdateNode(node);
            }
        }

        public void UpdateTagCombineValue(NodeNode node, byte value, int index = 0, int rgIndex = 0)
        {
            if (IsValidRuleGroup(node, index, rgIndex) && node.TaggedTexts[0].TaggedText[index].RuleGroup[rgIndex].TagCombineOp != null)
            {
                node.TaggedTexts[0].TaggedText[index].RuleGroup[rgIndex].TagCombineOp.Value = value;
                AddOrUpdateNode(node);
            }
        }

        public void UpdateLineUUIDNumber(NodeNode node, string value, int index = 0, int tgIndex = 0, int tgArrayIndex = 0)
        {
            if (IsValidTagTextArray(node, index, tgIndex, tgArrayIndex) && node.TaggedTexts[0].TaggedText[index].TagTexts[tgIndex].TagTextArray[tgArrayIndex].LineId != null)
            {
                node.TaggedTexts[0].TaggedText[index].TagTexts[tgIndex].TagTextArray[tgArrayIndex].LineId.Value = value;
                AddOrUpdateNode(node);
            }
        }

        public void UpdateLocalisationUUID(NodeNode node, string value, int index = 0, int tgIndex = 0, int tgArrayIndex = 0)
        {
            if (IsValidTagTextArray(node, index, tgIndex, tgArrayIndex) && node.TaggedTexts[0].TaggedText[index].TagTexts[tgIndex].TagTextArray[tgArrayIndex].TagText.Handle != null)
            {
                node.TaggedTexts[0].TaggedText[index].TagTexts[tgIndex].TagTextArray[tgArrayIndex].TagText.Handle = value;
                AddOrUpdateNode(node);
            }
        }

        public void UpdateIsStub(NodeNode node, bool value, int index = 0, int tgIndex = 0, int tgArrayIndex = 0)
        {
            if (IsValidTagTextArray(node, index, tgIndex, tgArrayIndex) && node.TaggedTexts[0].TaggedText[index].TagTexts[tgIndex].TagTextArray[tgArrayIndex].Stub != null)
            {
                node.TaggedTexts[0].TaggedText[index].TagTexts[tgIndex].TagTextArray[tgArrayIndex].Stub.Value = value;
                AddOrUpdateNode(node);
            }
        }

        public void UpdatePopLevel(NodeNode node, Int32 value)
        {
            node.PopLevel ??= new TypeValPairInt32();
            node.PopLevel.Value = value;
            AddOrUpdateNode(node);
        }

        public void UpdateGroupID(NodeNode node, String groupID)
        {
            node.GroupID.Value = groupID;
            AddOrUpdateNode(node);
        }
        public void UpdateGroupIndex(NodeNode node, Int32 groupIndex)
        {
            node.GroupIndex.Value = groupIndex;
            AddOrUpdateNode(node);
        }
        public void UpdateUUID(NodeNode node, String UUID)
        {
            node.Uuid.Value = UUID;
            AddOrUpdateNode(node);
        }
        public void UpdateConstructor(NodeNode node, String constructor)
        {
            node.Constructor.Value = constructor;
            AddOrUpdateNode(node);
        }

        public void UpdateLocation(NodeNode node, int x , int y)
        {
            EditorDataNodeData data = new EditorDataNodeData();
            data.Key = new TypeValPairStr();
            data.Val = new TypeValPairStr();
            data.Key.Type = "FixedString";
            data.Key.Value = "position";
            data.Val.Type = "LSString";
            data.Val.Value = x+";"+y;
            UpdateOrAddEditordata(node, data);
        }

        public void UpdateSpeaker(NodeNode node, Int32 speaker)
        {
            node.Speaker.Value = speaker;
            AddOrUpdateNode(node);
        }

        public void UpdateOptionalCheckbox(NodeNode node, bool optional)
        {
            node.optional.Value = optional;
            AddOrUpdateNode(node);
        }

        public void UpdateEndCheckBox(NodeNode node, bool endNode)
        {
            node.Endnode.Value = endNode;
            AddOrUpdateNode(node);
        }

        public void UpdateRootCheckBox(NodeNode node, bool rootNode)
        {
            if (node.Root == null)
            {
                node.Root = new TypeValPairBool("bool",rootNode);
            }
            else
            {
                node.Root.Value = rootNode;
            }
           
            AddOrUpdateNode(node);
        }
        
        public void UpdateEditorAttribute(NodeNode node,String keyVal, String valValue, String oldKey)
        {

            //Console.WriteLine("Got Update request:"+ node.Uuid + " | "+keyVal+" | "+valValue);
            //Keyname has changed
            if (keyVal != oldKey)
            {
                RemoveEditorData(node,oldKey);

            }
            EditorDataNodeData nData = new EditorDataNodeData();
            nData.Key = new TypeValPairStr("FixedString", keyVal);
            nData.Val = new TypeValPairStr("LSString", valValue);
            UpdateOrAddEditordata(node, nData);


        }

        public void RemoveEditorData(NodeNode node, String keyVal)
        {
            editorData[node.Uuid.Value].Remove(keyVal);
            foreach (var item in node.EditorData)
            {
                foreach (var editorDataNodeData in item.Data)
                {
                    if (editorDataNodeData.Key.Value == keyVal)
                    {

                        item.Data.Remove(editorDataNodeData);
                        break;
                    }
                }
            }
            AddOrUpdateNode(node);
        }
        public void UpdateOrAddEditordata(NodeNode node, EditorDataNodeData value)
        {
            //Remove from lookup dic
            if (editorData[node.Uuid.Value].ContainsKey(value.Key.Value))
            {
                editorData[node.Uuid.Value][value.Key.Value] = value.Val.Value;
            }
            else
            {
                editorData[node.Uuid.Value].Add(value.Key.Value, value.Val.Value);
            }
            bool foundAttribute = false;
            foreach (var item in node.EditorData)
            {
                foreach (var editorDataNodeData in item.Data)
                {
                    if (editorDataNodeData.Key.Value == value.Key.Value)
                    {
                        editorDataNodeData.Val.Value = value.Val.Value;
                        foundAttribute = true;
                        break;
                    }
                }
            }
            if (!foundAttribute)
            {
                node.EditorData[0].Data.Add(value);
            }
            AddOrUpdateNode(node);
        }

        public void RemoveEditorData(NodeNode node, EditorDataNodeData value)
        {
            if (editorData[node.Uuid.Value].ContainsKey(value.Key.Value))
            {
                editorData[node.Uuid.Value].Remove(value.Key.Value);
            }
        }
        public void RemoveFlag(NodeNode node,String flagUUID,bool isSet= false)
        {
          
            List<Flaggroup> flaggroup;

            if (isSet)
            {
                flaggroup = node.Setflags[0].flaggroup ?? new List<Flaggroup>();
                node.Setflags[0].flaggroup = flaggroup;
            }
            else
            {
                flaggroup = node.Checkflags[0].flaggroup ?? new List<Flaggroup>();
                node.Checkflags[0].flaggroup = flaggroup;
            }
            if (node.Checkflags != null && flaggroup != null)
            {
                foreach (var item in flaggroup)
                {
                    if (item.flag.Count <= 1 && item.flag[0].UUID.Value == flagUUID)
                    {
                        flaggroup.Remove(item);
                        if (isSet)
                        {
                            node.Setflags = new List<Flags>
                            {
                                new Flags()
                            };
                        }
                        else
                        {
                            node.Checkflags = new List<Checkflag>
                            {
                                new Checkflag()
                            };
                            
                        }
                        break;
                    }
                    else
                    {
                        foreach (var item2 in item.flag)
                        {
                            if (item2.UUID.Value == flagUUID)
                            {   
                                item.flag.Remove(item2);
                                break;
                            }
                               
                        }
                    }
                }
            }
            
            AddOrUpdateNode(node);
        }  
        public void AddOrUpdateFlag(NodeNode node, String flagUUID,String type,bool value, Int32 paramval, bool isSet = false)
        {
            List<Flaggroup> flaggroup;
            Flaggroup newFlag = new Flaggroup();

            if (isSet)
            {
                flaggroup = node.Setflags[0].flaggroup ?? new List<Flaggroup>();
                node.Setflags[0].flaggroup = flaggroup;
            }
            else
            {
                flaggroup = node.Checkflags[0].flaggroup ?? new List<Flaggroup>();
                node.Checkflags[0].flaggroup = flaggroup;
            }

            bool foundFlag = false;
         
            foreach (var item in flaggroup)
            {
                foreach (var item2 in item.flag)
                {
                    if (item2.UUID.Value == flagUUID)
                    {
                        Console.WriteLine("Found flag!");
                        item2.paramval.Value = paramval;
                        item2.value.Value = value;
                        foundFlag = true;
                        break;
                    }
                }
                if (foundFlag)
                {
                    item.type.Value = type;
                    break;
                }
            }
            
         

            if (!foundFlag)
            {
                newFlag.type = new TypeValPairStr("FixedString", type);
                newFlag.flag = new List<Flag>();
                Flag nflag = new Flag();
                nflag.UUID = new TypeValPairStr("FixedString", flagUUID);
                nflag.paramval = new TypeValPairInt32("int32", paramval);
                nflag.value = new TypeValPairBool("bool", value);
                newFlag.flag.Add(nflag);
                flaggroup.Add(newFlag);
                Console.WriteLine("Added new Flag");
            }
            AddOrUpdateNode(node);
        }
        public String getTagData(String value)
        {
            String res = "\n";
                                    
            if (tagData.ContainsKey(value))
            {
                if(tagData[value].attributes["Description"].Value!= "")
                {
                    res += tagData[value].attributes["Description"].Value;
                   
                }else if (tagData[value].attributes["Name"].Value != "")
                {
                    res += tagData[value].attributes["Name"].Value;
                  
                }
                else if (tagData[value].attributes["DisplayName"].Handle != "")
                {
                    res += tagData[value].attributes["DisplayName"].Handle;
                }
                else
                {
                    res = "Not found but key in dict";
                }

            }
            else
            {
                res = "not found";
            }
            return res;
        }
       
        private const string splitTagOn = "#SplitSeperator#";
        private bool merge = false;
        public void LoadTags(string[] tagsPaths)
        {
            Console.WriteLine("Loading Flags");
            foreach (string tagPath in tagsPaths)
            {
                string[] files = Directory.GetFiles(tagPath).Where(file => !file.Contains("lsf")).ToArray();

                foreach (string file in files)
                {
                    string jsonText = ConvertXmlToJson(file);
                    if (merge)
                    {
                        AppendTextToFile("merged.json", jsonText + splitTagOn);
                    }

                    BGEdit.TagStructur.TagStructurRoot tagStructurRoot = BGEdit.TagStructur.TagStructurRoot.FromJson(jsonText);
                    UpdateAttributes(tagStructurRoot);

                    string trimmedFilePath = file.Substring(0, file.LastIndexOf('.')).Replace(tagPath + "\\", "");
                    tagData[trimmedFilePath] = tagStructurRoot;
                    tagData[tagStructurRoot.attributes["UUID"].Value] = tagStructurRoot;
                }
            }
        }

        private string ConvertXmlToJson(string filePath)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);
            return JsonConvert.SerializeXmlNode(doc);
        }

        private void AppendTextToFile(string fileName, string text)
        {
            try
            {
                using (StreamWriter writer = File.AppendText(fileName))
                {
                    writer.Write(text);
                }
            }
            catch (Exception exp)
            {
                Console.Write(exp.Message);
            }
        }

        private void UpdateAttributes(BGEdit.TagStructur.TagStructurRoot tagStructurRoot)
        {
            foreach (var item in tagStructurRoot.Save.Region.Node.Attribute)
            {
                tagStructurRoot.attributes[item.Id] = item;
            }
        }
        public void LoadLocalization(String path)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            string jsonText = JsonConvert.SerializeXmlNode(doc);
            if (jsonText != null)
            {
                Root deserializedLocalization = JsonConvert.DeserializeObject<Root>(jsonText);
                foreach (Content cnt in deserializedLocalization.contentList.content)
                {
                    localizationDictionary.Add(cnt.contentuid, cnt);
                }
            }
         
           

        }
        public void LoadVoiceSpeakerGroups(String path)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            string jsonText = JsonConvert.SerializeXmlNode(doc);
            var SpeakerGroupsStructurRoot = JsonConvert.DeserializeObject<BGEdit.SpeakerGroupsStructur.SpeakerGroupsStructurRoot>(jsonText); 
            foreach (var item in SpeakerGroupsStructurRoot.Save.Region.Node.Children.Node)
            {
                //item.Attribute[0].Value == UUID of attribute
                if (!speakerGroups.ContainsKey(item.Attribute[0].Value))
                {
                    speakerGroups.Add(item.Attribute[0].Value, new Dictionary<string, string>());
                }
                foreach (var item2 in item.Attribute)
                {
                    //Console.WriteLine(item.Attribute[0].Value+"||"+ item2.Id + "||"+item2.Value);
                    speakerGroups[item.Attribute[0].Value].Add(item2.Id, item2.Value);
                }
            }
        }
        public void LoadDialoge(String path)
        {

            String json = "";
            using StreamReader reader = new(path);
            json = reader.ReadToEnd();
            DialogueStructurRoot DialogueStructurRoot = JsonConvert.DeserializeObject <DialogueStructurRoot>(json);
            /*try if something is not impl. throw error
            {
                DialogueStructurRoot DialogueStructurRoot = JsonConvert.DeserializeObject<DialogueStructurRoot>(json, new JsonSerializerSettings
                {
                    MissingMemberHandling = MissingMemberHandling.Error
                });*/
                dialogeDictionary.Add(DialogueStructurRoot.Save.Regions.Dialog.Uuid.Value, DialogueStructurRoot);
                Console.WriteLine(DialogueStructurRoot.Save.Regions.Dialog.Nodes[0].RootNodes.Count);
                LoadSpeakers(DialogueStructurRoot.Save.Regions.Dialog.Uuid.Value, DialogueStructurRoot);
                currentContext.currentDialogeNodeUUID = DialogueStructurRoot.Save.Regions.Dialog.Uuid.Value;
            /*}
            catch (JsonSerializationException ex)
            {
                Console.WriteLine(ex.Message);
                // Could not find member 'DeletedDate' on object of type 'Account'. Path 'DeletedDate', line 4, position 23.
            }*/
          
        }
        //UUID
        public void LoadSpeakers(String uuidnode, DialogueStructurRoot dialoge)
        {
            //Add DialogeUUID if not present
            Console.WriteLine(uuidnode);
            if (!speakerList.ContainsKey(uuidnode))
            {
                speakerList.Add(uuidnode, new Dictionary<string, Speaker>());
            }
            foreach(Speakerlist item in dialoge.Save.Regions.Dialog.Speakerlist)
            {
                foreach (Speaker item2 in item.Speaker)
                {
                    if (speakerList[uuidnode].ContainsKey(item2.List.Value))
                    {
                        speakerList[uuidnode][item2.List.Value] =  item2;
                    }
                    else
                    {
                        speakerList[uuidnode].Add(item2.Index.Value, item2);
                        Console.WriteLine("Added Speaker: "+item2.List.Value);
                    }
                }
            }
        }
        //Mapkey
        public void LoadMergedChars(String path)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            string jsonText = JsonConvert.SerializeXmlNode(doc);
           
            var myDeserializedClass = JsonConvert.DeserializeObject<BGEdit.MergedCharsStructure.Root>(jsonText);
            // Console.WriteLine("Found Amount of merged nodes in: "+ myDeserializedClass.Save.Region.Node.Children.Node.Length);
            foreach (var item in myDeserializedClass.save.region.node.children.node)
            {
                foreach (var item2 in item.attribute)
                {
                    //Console.WriteLine(item2.value);
                    if (item2.id == "MapKey")
                    {
                        //Console.WriteLine("Found MapKey");
                        if (mergedCharData.ContainsKey(item2.value))
                        {
                            mergedCharData[item2.value] = item;
                        }
                        else
                        {
                            mergedCharData.Add(item2.value, item);
                        }
                    }
                }
            }
        }
        //GlobalTemplate Attribute
        public void LoadOrigins(String path)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            string jsonText = JsonConvert.SerializeXmlNode(doc);
            var myDeserializedClass = JsonConvert.DeserializeObject<BGEdit.OriginStructur.Root>(jsonText);
            foreach (var item in myDeserializedClass.save.region.node.children.node)
            {
                foreach (var item2 in item.attribute)
                {
                    if (item2.id == "GlobalTemplate")
                    {
                       // Console.WriteLine("Added Line");
                        if (originsData.ContainsKey(item2.value))
                        {
                            originsData[item2.value] = item;
                        }
                        else
                        {
                            originsData[item2.value] = item;
                        }
                    }
                }
            }
        }
        public String GetSpeakerInfo(String uuid)
        {
            String res = "";
            //Console.WriteLine("Got Request for UUIDSPEAKER:"+ uuid);

            if (originsData.ContainsKey(uuid))
            {
                foreach (var item in originsData[uuid].attribute)
                {
                    if (item.id =="Name")
                    {
                        return (res += item.value);
                        
                    }
                }
            }
            else if (mergedCharData.ContainsKey(uuid))
            {
                foreach (var item in mergedCharData[uuid].attribute)
                {
                    if (item.id == "Name")
                    {
                        return (res += item.value);

                    }
                }
            }
            else if (speakerGroups.ContainsKey(uuid))
            {
                return (res += speakerGroups[uuid]["Description"]);
            }
            else
            {
                res += uuid + " :Not found";
            }
            return res;
        }
        public void LoadConfig(String path)
        {
            using StreamReader reader = new(path);
            var json = reader.ReadToEnd();
            //Console.WriteLine(json);
            this.config = JsonConvert.DeserializeObject<BGEdit.ConfigStructur.ConfigFile>(json);
             
        }
        public void Load()
        {
            try
            {
                LoadConfig("Config.json");

                if (config != null)
                {
                    String rootFolder = config.Config.RootFolder.Path + "\\";
                    LoadDialoge(rootFolder+config.Config.RelativeDialoguePath.Path);
                    Console.WriteLine(rootFolder + config.Config.RelativeLocalizationPath.Path);
                    LoadLocalization(rootFolder + config.Config.RelativeLocalizationPath.Path);
                    List<String> tagLocations = new List<String>();
                    foreach (var item in config.Config.RelativeflagPaths)
                    {
                        tagLocations.Add(rootFolder+item.Path);
                    }
                    LoadTags(tagLocations.ToArray());
                    foreach (var speakerGroup in config.Config.RelativeSpeakerGroupsPath)
                    {
                        LoadVoiceSpeakerGroups(rootFolder + speakerGroup.Path);
                    }
                    
                    foreach (var mergedPath in config.Config.RelativeMergedPaths)
                    {
                        LoadMergedChars(rootFolder+mergedPath.Path);
                        Console.WriteLine(rootFolder + mergedPath.Path);
                    }
                    LoadOrigins(rootFolder + config.Config.RelativeOriginsPath.Path);
                    
                    foreach (string key in dialogeDictionary.Keys)
                    {
                        foreach (NodeNode node in dialogeDictionary[key].Save.Regions.Dialog.Nodes[0].Node)
                        {
                            AddDialogueToBgData(node);
                        }

                    }
                   
                }
                //serializeData();


            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
            }
    
            
        }
        public void AddNodeToDialogueNode(NodeNode node)
        {
            if (dialogeDictionary.ContainsKey(currentContext.currentDialogeNodeUUID))
            {
                List<NodeNode> nodes = new List<NodeNode>(dialogeDictionary[currentContext.currentDialogeNodeUUID].Save.Regions.Dialog.Nodes[0].Node);
                nodes.Add(node);
                dialogeDictionary[currentContext.currentDialogeNodeUUID].Save.Regions.Dialog.Nodes[0].Node = nodes;
                Console.WriteLine("Added node to dialogue" + currentContext.currentDialogeNodeUUID);

            }
            else
            {
                Console.WriteLine("Current dialoge uuid not found"+ currentContext.currentDialogeNodeUUID);
            }
             
        }
        public String SerializeData()
        {

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            String json ="";
            String pattern = @"(""\w+""\s*:\s*\{)\s*(""\w+""\s*:\s*""?[\w\-]+""?,)\s*(""\w+""\s*:\s*""?.+?""?)\s*(\},?)";


            String replacement = "$1 $2 $3$4";
            foreach (var item in dialogeDictionary.Values)
            {

                json = JsonConvert.SerializeObject(item, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                }); 
              
                
            }
            String res = Regex.Replace(json, pattern, replacement);
            //todo: should be handled in writer and not like that
            return res.Replace("  ", "   ").Replace(":", " :").Replace("{ \"", "{\"");
        }

        public void AddConnectionInData(NodeNode parentNode, NodeNode childNode)
        {
            List<Child> parentChilds = new List<Child>();
            if (parentNode.children != null && parentNode.children.Count > 0 && parentNode.children[0].children != null) {
                parentChilds.AddRange(parentNode.children[0].children);
            }
            var child = new Child();
           
            child.UUID = new UUID(childNode.Uuid.Type, childNode.Uuid.Value);

            if (!parentChilds.Contains(child))
            {
                parentChilds.Add(child);
                Console.WriteLine("Added child");
            }
            if (parentNode.children == null)
            {
                parentNode.children = new List<Children>();
                parentNode.children.Add(new Children());
            }
            if (parentNode.children.Count == 0)
            {
                parentNode.children.Add(new Children());

            }
            parentNode.children[0].children = parentChilds;
            if (dialogeNodes.ContainsKey(parentNode.Uuid.Value))
            {
                dialogeNodes[parentNode.Uuid.Value] = parentNode;
                Console.WriteLine("Added into data child");
            }
            //bgData.SerializeData();

        }
        //todo
        public void RemoveConnectionInData()
        {

        }
        //todo
        public void RemoveNodeInData()
        {

        }

        public void AddDialogueToBgData(NodeNode node)
        {
            dialogeNodes[node.Uuid.Value] = node;
            foreach (var dat in node.EditorData[0].Data)
            {

                if (!editorData.ContainsKey(node.Uuid.Value))
                {
                    editorData.Add(node.Uuid.Value, new Dictionary<string, string>());
                }
                else
                {
                    //Console.WriteLine(dat.Key.Value);
                    if (!this.editorData[node.Uuid.Value].ContainsKey(dat.Key.Value))
                    {
                        editorData[node.Uuid.Value].Add(dat.Key.Value, dat.Val.Value);
                    }

                }
            }
        }
        public ICommand Save { get; set; } = new DelegateCommand(() =>
        {
            Console.WriteLine("Save");
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true)
                File.WriteAllText(saveFileDialog.FileName, BGData.Instance.SerializeData());
        });
        public ICommand CopyToClip { get; set; } = new DelegateCommand(() =>
        {
            Console.WriteLine("Copy to Clip");
            Clipboard.SetText(BGData.Instance.SerializeData());
        });

    }
}
