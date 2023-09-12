using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;


namespace BGEdit
{
    public class NodeViewModel : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler? PropertyChanged;
        public ObservableCollection<ConnectorViewModel> Input { get; set; } = new ObservableCollection<ConnectorViewModel>();
        public ObservableCollection<ConnectorViewModel> Output { get; set; } = new ObservableCollection<ConnectorViewModel>();
        public ObservableCollection<string> RootList { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> TagList { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> SpeakerList { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> GroupList { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> TagTextList { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<string> TagSetList { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<StackPanel> SetFlagEditable { get; set; } = new ObservableCollection<StackPanel>();
        public ObservableCollection<StackPanel> CheckFlagEditable { get; set; } = new ObservableCollection<StackPanel>();
        public ObservableCollection<StackPanel> EditorDataEditable { get; set; } = new ObservableCollection<StackPanel>();
        public ObservableCollection<StackPanel> TaggedTextDataEditable { get; set; } = new ObservableCollection<StackPanel>();
        public Brush HeaderBrushColor { get; set; } = new SolidColorBrush(Colors.DimGray);
        public String AddChildVisible { get; set; } = "True";
        public String RemoveVisible { get; set; } = "True";
        public ICommand AddChildren { get; set; } = new DelegateCommand(() =>
        {
            Console.WriteLine("Default");
        });
        public ICommand AddEditorData { get; set; } = new DelegateCommand(() =>
        {
            Console.WriteLine("AddEditorData default");
        });
        public ICommand RemoveNode { get; set; } = new DelegateCommand(() =>
        {
            Console.WriteLine("Remove node default");
        });
       
        public string RootsFound { get; set; } = "Hidden";
        public string GroupsFound { get; set; } = "Hidden";
        public string TagsFound { get; set; } = "Hidden";
        public string SpeakersFound { get; set; } = "Hidden";
        public string TagTextFound { get; set; } = "Hidden";
        public string TagsToSetFound { get; set; } = "Hidden";
        private Point _location;
        public Point Location
        {
            set
            {
                _location = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Location)));
            }
            get => _location;
        }
        private bool _RootCheckbox = false;
        public bool RootCheckbox
        {
            set
            {
                _RootCheckbox = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RootCheckbox)));
            }
            get => _RootCheckbox;
        }
        private bool _EndCheckbox = false;
        public bool EndCheckbox
        {
            set
            {
                _EndCheckbox = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EndCheckbox)));
            }
            get => _EndCheckbox;
        }
        private bool _OptionalCheckbox = false;
        public bool OptionalCheckbox
        {
            set
            {
                _OptionalCheckbox = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(OptionalCheckbox)));
            }
            get => _OptionalCheckbox;
        }
        private string? _type;
        public string Type
        {
            set
            {
                _type = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Type)));
            }
            get => _type;
        }
        private string _UUID;
        public string UUID
        {
            set
            {
                _UUID = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UUID)));
            }
            get => _UUID;
        }
        private string? _Title;
        public string Title
        {
            set
            {
                _Title = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Title)));
            }
            get => _Title;
        }
        private string _GroupID;
        public string GroupID
        {
            set
            {
                _GroupID = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(GroupID)));
            }
            get => _GroupID;
        }
        private string _GroupIndex;
        public string GroupIndex
        {
            set
            {
                _GroupIndex = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(GroupIndex)));
            }
            get => _GroupIndex;
        }

        private int _PopLevel = 0;
        public int PopLevel
        {
            set
            {
                _PopLevel = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PopLevel)));
            }
            get => _PopLevel;
        }
        private string _TaggedTexts;
        public string TaggedTexts
        {
            set
            {
                _TaggedTexts = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TaggedTexts)));
            }
            get => _TaggedTexts;
        }
        private string _Constructor;
        public string Constructor
        {
            set
            {
                _Constructor = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Constructor)));
            }
            get => _Constructor;
        }
        private string _Speaker;
        public string Speaker
        {
            set
            {
                _Speaker = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Speaker)));
            }
            get => _Speaker;
        }

        private bool _hasTagRule = false;
        public bool HasTagRule
        {
            set
            {
                _hasTagRule = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasTagRule)));
            }
            get => _hasTagRule;
        }

        private string _tagCombineType = "uint8";
        public string TagCombineType
        {
            set
            {
                _tagCombineType = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TagCombineType)));
            }
            get => _tagCombineType;
        }
        private byte _tagCombineValue = 0;

        public byte TagCombineValue
        {
            set
            {
                _tagCombineValue = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TagCombineValue)));
            }
            get => _tagCombineValue;
        }
        private string _lineUUIDNumber = "uuidnumber";
        public string LineUUIDNumber
        {
            set
            {
                _lineUUIDNumber = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LineUUIDNumber)));
            }
            get => _lineUUIDNumber;
        }
        private string _localisationUUID = "LocaUUID";

        public string LocalisationUUID
        {
            set
            {
                _localisationUUID = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LocalisationUUID)));
            }
            get => _localisationUUID;
        }
        private bool _isStub = false;
        public bool IsStub
        {
            set
            {
                _isStub = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsStub)));
            }
            get => _isStub;
        }

        public ICommand AddSetFlag { get; set; } = new DelegateCommand(() =>
        {
            Console.WriteLine("AddSetFlag default");
        });
        public ICommand AddCheckFlag { get; set; } = new DelegateCommand(() =>
        {
            Console.WriteLine("AddCheckFlag default");
        });
        //todo
        public void items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems)
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(item)));
            }
            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems)
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(item)));
            }
        }
        public NodeViewModel()
        {
            EditorDataEditable.CollectionChanged += items_CollectionChanged;
            
        }

    }
}
