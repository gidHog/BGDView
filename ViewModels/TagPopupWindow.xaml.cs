using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace BGEdit
{
    /// <summary>
    /// Interaction logic for TagPopupWindow.xaml
    /// </summary>
    public partial class TagPopupWindow : Window
    {
        public TagPopupWindow()
        {
            InitializeComponent();
        }


    }

    public class TagDisplayMember : INotifyPropertyChanged
    {
        public String Name { get; set; } = "not found";
        public String Description { get; set; } = "not found";
        public String UUID { get; set; } = "not found";
        public String CheckOrSet { get; set; } = "not found";
        public String Type { get; set; } = "not found";
        public int Value { get; set; } = 0;
        public bool Condition { get; set; } = true;
        private int _selectedComboSetCheck { get; set; } = 0;
        public int SelectedComboSetCheck
        {
            get
            {
                return _selectedComboSetCheck;
            }

            set
            {
                if (_selectedComboSetCheck == value)
                {
                    return;
                }
                _selectedComboSetCheck = value;

                BGData.Instance.AddOrUpdateFlag(BGData.Instance.dialogeNodes[EditorViewModel.Instance.CurrentNodeViewModel.UUID], UUID, "Local", SelectedComboSetCheck != 1, 0, Condition);

            }
        }
        public TagDisplayMember(String name, String description, String uuid,String checkOrSet = "",String type ="Local", int value = 0, bool condition = false)
        {
            this.Name = name;
            this.Description = description;
            this.UUID = uuid;
            this.CheckOrSet = checkOrSet;
            this.Type = type;
            this.Value = value;
            this.Condition = condition;
            if (checkOrSet == "Set") this.SelectedComboSetCheck = 1;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
    public class TagPopupWindowViewModel : INotifyPropertyChanged
    {
        public TagPopupWindowViewModel()
        {
            
            AddFlagsToList();
            PropertyChanged += TagPopWindow_PropertyChangedType;
            Instance = this;
            AddFlagsFromCurrentNodeView(EditorViewModel.Instance.CurrentNodeViewModel);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<String> FlagTypes { get; set; } = new ObservableCollection<String>();
        public ObservableCollection<TagDisplayMember> DisplayedData { get; set; } = new ObservableCollection<TagDisplayMember>();
        public ObservableCollection<TagDisplayMember> DisplayAddedTags { get; set; } = new ObservableCollection<TagDisplayMember>();

        private static TagPopupWindowViewModel? _instance;
        public static TagPopupWindowViewModel Instance
        {
            set
            {
                _instance = value;
            }
            get
            {
                if (_instance == null) _instance = new TagPopupWindowViewModel();
                return _instance;
            }
        }

        public ICommand DownArrow { get; set; } = new DelegateCommand(() =>
        {
            Instance.AddFlagFromSelection();
        });
        public ICommand UpArrow { get; set; } = new DelegateCommand(() =>
        {
            Instance.RemoveFlagFromSelection();
        });
        public ICommand OK { get; set; } = new DelegateCommand(() =>
        {
        });
        public ICommand Chancel { get; set; } = new DelegateCommand(() =>
        {
            Console.WriteLine("Chancel");
        });

        private int _selectedIndexFlagType = 0;
        public int SelectedIndexFlagType
        {
            get
            {
                return _selectedIndexFlagType;
            }

            set
            {
                if (_selectedIndexFlagType == value)
                {
                    return;
                }
                _selectedIndexFlagType = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedIndexFlagType)));
                //Console.WriteLine(_selectedIndexFlagType + ": " + FlagTypes[_selectedIndexFlagType]);
            }
        }

        private int _selectedIndexFlag = 0;
        public int SelectedIndexFlag
        {
            get
            {
                return _selectedIndexFlag;
            }

            set
            {
                if (_selectedIndexFlag == value)
                {
                    return;
                }
                _selectedIndexFlag = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedIndexFlag)));
                //Console.WriteLine(_selectedIndexFlagType + ": " + FlagTypes[_selectedIndexFlagType]);
            }
        }
        private int _selectedIndexAddedFlag = 0;
        public int SelectedIndexAddedFlag
        {
            get
            {
                return _selectedIndexAddedFlag;
            }

            set
            {
                if (_selectedIndexAddedFlag == value)
                {
                    return;
                }
                _selectedIndexAddedFlag = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedIndexAddedFlag)));
                //Console.WriteLine(_selectedIndexFlagType + ": " + FlagTypes[_selectedIndexFlagType]);
            }
        }
        // Category , List of Flags
        private Dictionary<String,ObservableCollection<TagDisplayMember>> FlagsAndTagsMapping { get; set; } = new();
        private Dictionary<String, bool> addedUUIDS = new();
        
        private void AddFlagsFromCurrentNodeView(NodeViewModel currentNodeViewModel)
        {
            NodeNode selectedNode = BGData.Instance.dialogeNodes[currentNodeViewModel.UUID];
            foreach (var item in selectedNode.Checkflags)
            {
                AddFlagsAndTagsFromNode(item.flaggroup,"Check");
            }
            foreach (var item in selectedNode.Setflags)
            {
                AddFlagsAndTagsFromNode(item.flaggroup,"Set");
            }

        }

        private void AddFlagsAndTagsFromNode(List<Flaggroup> fgroups,String setCheck)
        {
            if (fgroups == null) return;
            foreach (var item2 in fgroups)
            {
                foreach (var item3 in item2.flag)
                {
                    if (BGData.Instance.tagData.ContainsKey(item3.UUID.Value))
                    {
                        var flag = BGData.Instance.tagData[item3.UUID.Value];
                        if (flag.save.regions.RegTags == null)
                        {
                            DisplayAddedTags.Add(new TagDisplayMember(flag.save.regions.RegFlags.Name.Value, flag.save.regions.RegFlags.Description.Value, flag.save.regions.RegFlags.UUID.Value, setCheck, item2.type.Value, 0, item3.value.Value));
                        }
                        else
                        {
                            DisplayAddedTags.Add(new TagDisplayMember(flag.save.regions.RegTags.Name.Value, flag.save.regions.RegTags.Description.Value, flag.save.regions.RegTags.UUID.Value, setCheck, item2.type.Value, 0, item3.value.Value));
                        }
                    }

                }
            }
            
        }

        private void AddFlagFromSelection()
        {
            //Console.WriteLine($"SelectedType{SelectedIndexFlagType} and index {SelectedIndexFlag}");
            String selectedType = FlagTypes[_selectedIndexFlagType];
            DisplayAddedTags.Add(FlagsAndTagsMapping[selectedType][SelectedIndexFlag]);
            BGData.Instance.AddOrUpdateFlag(BGData.Instance.dialogeNodes[EditorViewModel.Instance.CurrentNodeViewModel.UUID], FlagsAndTagsMapping[selectedType][SelectedIndexFlag].UUID,"Local",true,0, FlagsAndTagsMapping[selectedType][SelectedIndexFlag].Condition);
        }

        private void RemoveFlagFromSelection() {

            if (DisplayAddedTags.Count != 0)
            {
                if(SelectedIndexAddedFlag > DisplayAddedTags.Count)
                {
                    SelectedIndexAddedFlag = 0;
                }
                bool isCheck = true;
                if (DisplayAddedTags[SelectedIndexAddedFlag].CheckOrSet == "Check")
                {
                    isCheck = false;
                };
                BGData.Instance.RemoveFlag(BGData.Instance.dialogeNodes[EditorViewModel.Instance.CurrentNodeViewModel.UUID], DisplayAddedTags[SelectedIndexAddedFlag].UUID, isCheck);
                DisplayAddedTags.Remove(DisplayAddedTags[SelectedIndexAddedFlag]);
            }
           
        }
        private void AddFlagsToList()
        {

            //Console.WriteLine(BGData.Instance.tagData.Values.Count);
            //FlagTypes.Add("Flags");
            foreach (var flag in BGData.Instance.tagData.Values)
            {
                //
                if (flag.save.regions.RegTags != null && flag.save.regions.RegTags.Categories != null)
                {
         
                    foreach (var item in flag.save.regions.RegTags.Categories)
                    {
                        if (item != null && item.category != null)
                        {
                            foreach (var item2 in item.category)
                            {
                                if (!FlagTypes.Contains(item2.Name.Value))
                                {
                                    FlagTypes.Add(item2.Name.Value);
                                   
                                }
                                if (!addedUUIDS.ContainsKey(flag.save.regions.RegTags.UUID.Value))
                                {

                                    AddFlagToCategory(item2.Name.Value, new TagDisplayMember(flag.save.regions.RegTags.Name.Value, flag.save.regions.RegTags.Description.Value, flag.save.regions.RegTags.UUID.Value));
                                }
                            }
                            addedUUIDS.TryAdd(flag.save.regions.RegTags.UUID.Value, true);
                        }
                    }
                }
                else if (flag.save.regions.RegFlags != null)
                {
                    if (!addedUUIDS.ContainsKey(flag.save.regions.RegFlags.UUID.Value))
                    {
                      
                        AddFlagToCategory("Flags", new TagDisplayMember(flag.save.regions.RegFlags.Name.Value, flag.save.regions.RegFlags.Description.Value, flag.save.regions.RegFlags.UUID.Value));
                        addedUUIDS.Add(flag.save.regions.RegFlags.UUID.Value, true);
                    }
                }
            }
     
        }

        private void AddFlagToCategory(String category, TagDisplayMember tdMember)
        {
            if (!FlagsAndTagsMapping.ContainsKey(category))
            {
                FlagsAndTagsMapping.Add(category, new ObservableCollection<TagDisplayMember>());
                //Console.WriteLine(category);
               
            }
            FlagsAndTagsMapping.TryGetValue(category, out var flags);
            flags.Add(tdMember);
         
        }
        public void TagPopWindow_PropertyChangedType(object? sender, PropertyChangedEventArgs e)
        {
            Console.WriteLine("Called "+e.PropertyName);
            switch (e.PropertyName)
            {
                case "SelectedIndexFlagType":
                    var senderView = sender as TagPopupWindowViewModel;
                    String selectedType = FlagTypes[_selectedIndexFlagType];
                    //Console.WriteLine("Selected: "+ selectedType);
                    DisplayedData.Clear();
                    foreach (var flag in FlagsAndTagsMapping[selectedType])
                    {
                        DisplayedData.Add(flag);
                    }
                    break;
                case "SelectedIndexFlag":
                    Console.WriteLine("Selected Flag");
                    break;
                case "SelectedComboSetCheck":
                    Console.WriteLine("ComboChecked");
                    break;
                default:
                    Console.WriteLine(e.PropertyName);
                    break;
            }
        }
    }
}
