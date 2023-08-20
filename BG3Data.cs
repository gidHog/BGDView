using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Newtonsoft.Json;
using BGEdit.LocalizationStructur;
using System.Windows.Shapes;
using System.Security.Principal;
using System.Windows;

namespace BGEdit
{
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
    public class BG3Data
    {
        public Dictionary<string, Content> localizationDictionary = new Dictionary<string,Content>();
        public Dictionary<string, DialogueStructurRoot> dialogeDictionary = new Dictionary<string, DialogueStructurRoot>();
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

        public BGEdit.ConfigStructur.ConfigFile config = new BGEdit.ConfigStructur.ConfigFile();
        //BGEdit.MergedCharsStructure.Save.Region.Node.Children.Node
        public BG3Data() { }

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
        //Maybe seperate Tag and Flags
        public void loadTags(String[] tagsPaths)
        {

            foreach (String tagPath in tagsPaths) {
                string[] files = Directory.GetFiles(tagPath);
                foreach (string file in files)
                {
                    if (!file.Contains("lsf")) {  
                        XmlDocument doc = new XmlDocument();
                        doc.Load(file);
                        string jsonText = JsonConvert.SerializeXmlNode(doc);
                        BGEdit.TagStructur.TagStructurRoot TagStructurRoot = BGEdit.TagStructur.TagStructurRoot.FromJson(jsonText);
                        foreach (var item in TagStructurRoot.Save.Region.Node.Attribute)
                        {
                            if (TagStructurRoot.attributes.ContainsKey(item.Id))
                            {
                                TagStructurRoot.attributes[item.Id] = item;
                            }
                            else
                            {
                                TagStructurRoot.attributes.Add(item.Id, item);
                            }
                       
                        }
                        if (!tagData.ContainsKey(file.Substring(0, file.IndexOf('.')).Replace(tagPath + "\\", "")))
                        {
                            tagData.Add(file.Substring(0, file.IndexOf('.')).Replace(tagPath + "\\", ""), TagStructurRoot);
                        }
                        else
                        {
                            tagData[file.Substring(0, file.IndexOf('.')).Replace(tagPath + "\\", "")] =  TagStructurRoot;
                        }
                 
                   
                        if (!tagData.ContainsKey(TagStructurRoot.attributes["UUID"].Value))
                        {
                        
              
                            tagData.Add(TagStructurRoot.attributes["UUID"].Value, TagStructurRoot);
                        }
                        else
                        {
                             tagData[TagStructurRoot.attributes["UUID"].Value]= TagStructurRoot;
                        }
                 
                    }
      
                }
            }

        }
        public void loadLocalization(String path)
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
        public void loadVoiceSpeakerGroups(String path)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            string jsonText = JsonConvert.SerializeXmlNode(doc);
            var SpeakerGroupsStructurRoot = BGEdit.SpeakerGroupsStructur.SpeakerGroupsStructurRoot.FromJson(jsonText);
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
        public void loadDialoge(String path)
        {
            using StreamReader reader = new(path);
            var json = reader.ReadToEnd();
            DialogueStructurRoot DialogueStructurRoot = DialogueStructurRoot.FromJson(json);
            dialogeDictionary.Add(DialogueStructurRoot.Save.Regions.Dialog.Speakerlist[0].Speaker[0].SpeakerMappingId.value, DialogueStructurRoot);
            Console.WriteLine(DialogueStructurRoot.Save.Regions.Dialog.Speakerlist[0].Speaker[0].SpeakerMappingId.value);
            Console.WriteLine(DialogueStructurRoot.Save.Regions.Dialog.Nodes[0].RootNodes.Length);
            loadSpeakers(DialogueStructurRoot.Save.Regions.Dialog.Uuid.value,DialogueStructurRoot);
            currentContext.currentDialogeNodeUUID = DialogueStructurRoot.Save.Regions.Dialog.Uuid.value;
        }
        //UUID
        public void loadSpeakers(String uuidnode, DialogueStructurRoot dialoge)
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
                    if (speakerList[uuidnode].ContainsKey(item2.List.value))
                    {
                        speakerList[uuidnode][item2.List.value] =  item2;
                    }
                    else
                    {
                        speakerList[uuidnode].Add(item2.Index.value, item2);
                        Console.WriteLine("Added Speaker: "+item2.List.value);
                    }
                }
            }
        }

        //Mapkey
        public void loadMergedChars(String path)
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
        public void loadOrigins(String path)
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

        public String getSpeakerInfo(String uuid)
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

        public void clearCurrentDialogueData()
        {

        }

        public void loadConfig(String path)
        {
            using StreamReader reader = new(path);
            var json = reader.ReadToEnd();
            //Console.WriteLine(json);
            this.config = JsonConvert.DeserializeObject<BGEdit.ConfigStructur.ConfigFile>(json);
             
        }
        public void load()
        {
            try
            {
                loadConfig("Config.json");

                if (config != null)
                {
                    String rootFolder = config.Config.RootFolder.Path + "\\";
                    loadDialoge(rootFolder+config.Config.RelativeDialoguePath.Path);
                    Console.WriteLine(rootFolder + config.Config.RelativeLocalizationPath.Path);
                    loadLocalization(rootFolder + config.Config.RelativeLocalizationPath.Path);
                    List<String> tagLocations = new List<String>();
                    foreach (var item in config.Config.RelativeflagPaths)
                    {
                        tagLocations.Add(rootFolder+item.Path);
                    }
                    loadTags(tagLocations.ToArray());
                    foreach (var speakerGroup in config.Config.RelativeSpeakerGroupsPath)
                    {
                        loadVoiceSpeakerGroups(rootFolder + speakerGroup.Path);
                    }
                    
                    foreach (var mergedPath in config.Config.RelativeMergedPaths)
                    {
                        loadMergedChars(rootFolder+mergedPath.Path);
                        Console.WriteLine(rootFolder + mergedPath.Path);
                    }
                    loadOrigins(rootFolder + config.Config.RelativeOriginsPath.Path);
                }
                serializeData();


            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
            }
    
            
        }

        //Fucky
        public void searchAndLoadMergedChars(String path)
        {
            Console.WriteLine("Searching for merged...");
            string[] files = Directory.GetFiles(path, "*merged.lsx", SearchOption.AllDirectories);
            foreach (string file in files) {
                Console.WriteLine("Found merged: " + file);
                try
                {
                    loadMergedChars(file);
                }
                catch (Exception e)
                {
                   
                }
                
            }  
        }
        public void loadData(DataType typeOfData, String path)
        {
            switch (typeOfData)
            {
                case DataType.Localization: 
                    loadLocalization(path);
                    break;
                case DataType.Dialog:
                    loadDialoge(path);
                    break;
                case DataType.MergedChars:
                    loadMergedChars(path);
                    break;
                
            }
        }
    
        public void serializeData()
        {
            foreach (var item in dialogeDictionary.Values)
            {
                string json = JsonConvert.SerializeObject(item, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
                Clipboard.SetText(json);
                
            }
        }
    }
}
