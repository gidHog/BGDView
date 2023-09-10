using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using BGEdit.GenericStructures;
// <partial auto-generated/>
namespace BGEdit
{

    public partial class DialogueStructurRoot
    {
        [JsonProperty("save")]
        public Save Save { get; set; }
    }

    public partial class Save
    {
        [JsonProperty("header")]
        public Header Header { get; set; }

        [JsonProperty("regions")]
        public Regions Regions { get; set; }
    }

    public partial class Header
    {
        [JsonProperty("version")]
        public string Version { get; set; }
    }

    public partial class Regions
    {
        [JsonProperty("dialog")]
        public Dialog Dialog { get; set; }

        [JsonProperty("editorData")]
        public EditorData EditorData { get; set; }
    }

    public partial class Dialog
    {
        [JsonProperty("AllowDeadSpeakers")]
        public TypeValPairBool AllowDeadSpeakers { get; set; }
        [JsonProperty("DefaultAddressedSpeakers")]
        public List<DefaultAddressedSpeaker> DefaultAddressedSpeakers { get; set; }
      
        [JsonProperty("DefaultSpeakerIndex")]
        public TypeValPairInt32 DefaultSpeakerIndex { get; set; }
        [JsonProperty("IsAllowingJoinCombat")]
        public TypeValPairBool IsAllowingJoinCombat { get; set; }

        [JsonProperty("IsBehaviour")]
        public TypeValPairBool IsBehaviour { get; set; }
        [JsonProperty("IsPrivateDialog")]
        public TypeValPairBool IsPrivateDialog { get; set; }
        [JsonProperty("IsSubbedDialog")]
        public TypeValPairBool IsSubbedDialog { get; set; }
        [JsonProperty("IsWorld")]
        public TypeValPairBool IsWorld { get; set; }
        [JsonProperty("TimelineId")]
        public TypeValPairStr TimelineId { get; set; }

        [JsonProperty("UUID")]
        public TypeValPairStr Uuid { get; set; }

        [JsonProperty("automated")]
        public TypeValPairBool Automated { get; set; }

        [JsonProperty("category")]
        public TypeValPairStr Category { get; set; }

        [JsonProperty("issfxdialog")]
        public TypeValPairBool issfxdialog { get; set; }

        [JsonProperty("nodes")]
        public List<DialogNode> Nodes { get; set; }

        [JsonProperty("speakerlist")]
        public List<Speakerlist> Speakerlist { get; set; }
    }

    public partial class IsAllowingJoinCombat
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("value")]
        public bool Value { get; set; }
    }

   

    public partial class DefaultAddressedSpeakerObject
    {
        [JsonProperty("MapKey")]
        public TypeValPairInt32 MapKey { get; set; }
        [JsonProperty("MapValue")]
        public TypeValPairInt32 MapValue { get; set; }
    }
    public partial class DefaultAddressedSpeaker
    {
        [JsonProperty("Object")]
        public List<DefaultAddressedSpeakerObject> Object { get; set; }
    }

    public partial class DialogNode
    {
        [JsonProperty("RootNodes")]
        public List<RootNode> RootNodes { get; set; }

        [JsonProperty("node")]
        public List<NodeNode> Node { get; set; }
    }

    public class UUID
    {
        [JsonProperty("type")]
        public string type { get; set; }

        [JsonProperty("value")]
        public string value { get; set; }

        public UUID() { }
        public UUID(string type, string value)
        {
            this.type = type;
            this.value = value;
        }
    }
    public partial class Child
    {

        [JsonProperty("UUID")]
        public UUID UUID { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as Child);
        }

        public bool Equals(Child obj)
        {
            return obj != null && obj.UUID.value != null && this.UUID !=null && obj.UUID.value == this.UUID.value;
   
        }

    }

    public partial class Children
    {
        [JsonProperty("child")]
        public List<Child> children { get; set; }

    }


    public class Jumptarget
    {
        [JsonProperty("type")]
        public string type { get; set; }

        [JsonProperty("value")]
        public string value { get; set; }
    }


    public partial class Checkflag
    {
        [JsonProperty("flaggroup")]
        public List<Flaggroup> flaggroup { get; set; }
    }
    public partial class Flaggroup
    {
        [JsonProperty("flag")]
        public List<Flag> flag { get; set; }

        [JsonProperty("type")]
        public TypeValPairStr type { get; set; }
    }
    public partial class Flag
    {
        [JsonProperty("UUID")]
        public TypeValPairStr UUID { get; set; }
        [JsonProperty("paramval")]
        public TypeValPairInt32 paramval { get; set; }
        [JsonProperty("value")]
        public TypeValPairBool value { get; set; }
    }


    public partial class Value
    {
        [JsonProperty("type")]
        public string type { get; set; }

        [JsonProperty("value")]
        public bool value { get; set; }
    }

    public partial class EditorDataNodeData
    {
        [JsonProperty("key")]
        public TypeValPairStr Key { get; set; }

        [JsonProperty("val")]
        public TypeValPairStr Val { get; set; }
        public EditorDataNodeData(){}
        public EditorDataNodeData(TypeValPairStr Key, TypeValPairStr Val)
        {
            this.Val = Val;
            this.Key = Key;
        }

    }
    public partial class EditorDataNode
    {
        [JsonProperty("data")]
        public List<EditorDataNodeData> Data { get; set; }
    }

    public partial class SpeakerLinkingEntries
    {
        [JsonProperty("SpeakerLinkingEntry")]
        public List<SpeakerLinkingEntry> SpeakerLinkingEntry { get; set; }
    }

    public partial class SpeakerLinkingEntry
    {
        [JsonProperty("Key")]
        public TypeValPairInt32 key { get; set; }
        [JsonProperty("Value")]
        public TypeValPairInt32 value { get; set; }
    }

    public partial class ValidatedFlags
    {
        [JsonProperty("ValidatedHasValue")]
        public TypeValPairStr ValidatedHasValue { get; set; }
        [JsonProperty("flaggroup")]
        public List<Flaggroup> Flaggroup { get; set; }
    }

    public partial class NodeNode
    {
        [JsonIgnore]
        public List<String> Parents { get; set; } = new List<String> ();
        public TypeValPairStr NestedDialogNodeUUID { get; set; }
        [JsonProperty("Ability")]
        public TypeValPairStr Ability { get; set; }

        [JsonProperty("AllowGrouping")]
        public TypeValPairBool AllowGrouping { get; set; }
        [JsonProperty("AllowNodeGrouping")]
        public TypeValPairBool AllowNodeGrouping { get; set; }
        [JsonProperty("Advantage")]
        public TypeValPairUInt8 Advantage { get; set; }
        [JsonProperty("addressedspeaker")]
        public TypeValPairInt32 addressedspeaker { get; set; }
        [JsonProperty("DifficultyClassID")]
        public TypeValPairStr DifficultyClassID { get; set; }

        [JsonProperty("ExcludeCompanionsOptionalBonuses")]
        public TypeValPairBool ExcludeCompanionsOptionalBonuses { get; set; }
        [JsonProperty("ExcludeSpeakerOptionalBonuses")]
        public TypeValPairBool ExcludeSpeakerOptionalBonuses { get; set; }
        [JsonProperty("exclusive")]
        public TypeValPairBool exclusive { get; set; }
        [JsonProperty("gameplaynode")]
        public TypeValPairBool gameplaynode { get; set; }
        [JsonProperty("stub")]
        public TypeValPairBool stub { get; set; }
        [JsonProperty("suppresssubtitle")]
        public TypeValPairBool suppresssubtitle { get; set; }
 
        [JsonProperty("waittime")]
        public TypeValPairFloat waittime { get; set; }
        [JsonProperty("RollTargetSpeaker")]
        public TypeValPairInt32 RollTargetSpeaker { get; set; }
        [JsonProperty("RollType")]
        public TypeValPairStr RollType { get; set; }
        [JsonProperty("Success")]
        public TypeValPairStr Success { get; set; }
        [JsonProperty("ApprovalRatingID")]
        public TypeValPairStr ApprovalRatingID { get; set; }
        [JsonProperty("SpeakerLinking")]
        public List<SpeakerLinkingEntries> SpeakerLinking { get; set; }
        [JsonProperty("GameData")]
        public List<GameData> GameData { get; set; }
        [JsonProperty("Greeting")]
        public TypeValPairBool Greeting { get; set; }
        [JsonProperty("ValidatedFlags")]
        public List<ValidatedFlags> ValidatedFlags { get; set; }
        [JsonProperty("Result")]
        public TypeValPairBool Result { get; set; }
        [JsonProperty("GroupID")]
        public TypeValPairStr GroupID { get; set; }
        [JsonProperty("GroupIndex")]
        public TypeValPairInt32 GroupIndex { get; set; }
        [JsonProperty("ShowOnce")]
        public TypeValPairBool showOnce { get; set; }
        [JsonProperty("Skill")]
        public TypeValPairStr Skill { get; set; }
  
        [JsonProperty("SourceNode")]
        public TypeValPairStr SourceNode { get; set; }
        [JsonProperty("TaggedTexts")]
        public List<NodeTaggedText> TaggedTexts { get; set; }
        [JsonProperty("Tags")]
        public List<TagTag> Tags { get; set; }
        [JsonProperty("UUID")]
        public TypeValPairStr Uuid { get; set; }
       
        [JsonProperty("checkflags")]
        public List<Checkflag> Checkflags { get; set; }
        [JsonProperty("PopLevel")]
        public TypeValPairInt32 PopLevel { get; set; }

        [JsonProperty("children")]
        //public Children[] children { get; set; }
        public List<Children> children { get; set; }

        [JsonProperty("constructor")]
        public TypeValPairStr Constructor { get; set; }

        [JsonProperty("editorData")]
        public List<EditorDataNode> EditorData { get; set; }

        [JsonProperty("optional")]
        public TypeValPairBool optional { get; set; }

        [JsonProperty("speaker")]
        public NextNodeId Speaker { get; set; }
        [JsonProperty("transitionmode")]
        public TypeValPairUInt8 Transitionmode { get; set; }
        [JsonProperty("jumptarget")]
        public Jumptarget Jumptarget { get; set; }
        [JsonProperty("jumptargetpoint")]
        public TypeValPairUInt8 jumptargetpoint { get; set; }
        [JsonProperty("Root")]
        public TypeValPairBool Root { get; set; }

        [JsonProperty("endnode")]
        public TypeValPairBool Endnode { get; set; }

        [JsonProperty("setflags")]
        public List<Flags> Setflags { get; set; }


    }

    public partial class DefaultAttitude
    {
        [JsonProperty("data")]
        public List<Datum> Data { get; set; }
    }

    public partial class Datum
    {
        [JsonProperty("key")]
        public TypeValPairStr Key { get; set; }

        [JsonProperty("val")]
        public TypeValPairStr Val { get; set; }
    }

    public partial class LookAts
    {
        [JsonProperty("Speaker")]
        public TypeValPairUInt16 Speaker { get; set; }
        [JsonProperty("Target")]
        public TypeValPairUInt16 Target { get; set; }
    }

    public partial class AiPersonal
    {
        [JsonProperty("AiPersonality")]
        TypeValPairStr AiPersonality { get; set; }
}
    public partial class AiPersonalities
    {
        [JsonProperty("AiPersonality")]
        List<AiPersonal> AiPersonality { get; set; }
    }
    public partial class MusicInstrumentSounds
    {

    }

    public partial class OriginSound
    {

    }
    public partial class GameData
    {
        [JsonProperty("AiPersonalities")]
        public List<AiPersonalities> AiPersonalities { get; set; }

        [JsonProperty("LookAts")]
        public List<LookAts> LookAts { get; set; }
        [JsonProperty("CameraTarget")]
        public TypeValPairInt32 CameraTarget { get; set; }
        [JsonProperty("CustomMovie")]
        public TypeValPairStr CustomMovie { get; set; }
        [JsonProperty("ExtraWaitTime")]
        public TypeValPairInt32 ExtraWaitTime { get; set; }
        [JsonProperty("MusicInstrumentSounds")]
        public List<MusicInstrumentSounds> MusicInstrumentSounds { get; set; }

        [JsonProperty("OriginSound")]
        public List<OriginSound> OriginSound { get; set; }
        [JsonProperty("SoundEvent")]
        public TypeValPairStr SoundEvent { get; set; }
        
    }

    public partial class NextNodeId
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("value")]
        public long Value { get; set; }
    }

    public partial class NodeTaggedText
    {
        [JsonProperty("TaggedText")]
        public List<TaggedTextTaggedText> TaggedText { get; set; }
    }
    public partial class RuleGroup
    {
        [JsonProperty("Rules")]
        public List<RuleGroupRule> Rules { get; set; }

        [JsonProperty("TagCombineOp")]
        public TypeValPairUInt8 TagCombineOp { get; set; }
    }

    public partial class RuleGroupRule
    {
        [JsonProperty("Rule")]
        public List<RuleRule> Rule { get; set; }
    }

    public partial class RuleRule
    {
        [JsonProperty("HasChildRules")]
        public HasChildRules HasChildRules { get; set; }

        [JsonProperty("TagCombineOp")]
        public TagCombineOp TagCombineOp { get; set; }

        [JsonProperty("Tags")]
        public List<RuleTag> Tags { get; set; }

        [JsonProperty("speaker")]
        public TagCombineOp Speaker { get; set; }
    }

    public partial class HasChildRules
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("value")]
        public bool Value { get; set; }
    }

    public partial class TagCombineOp
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("value")]
        public long Value { get; set; }
    }

    public partial class RuleTag
    {
        [JsonProperty("Tag")]
        public List<TagTag> Tag { get; set; }
    }

    public partial class NestedTag
    {
        [JsonProperty("Tag")]
        public TypeValPairStr nestedTag { get; set; }
    }
    public partial class TagTag
    {
        [JsonProperty("Object")]
        public Object Object { get; set; }
        [JsonProperty("Tag")]
        public List<NestedTag> tag { get; set; }

    }

    public partial class Object
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("value")]
        public Guid Value { get; set; }
    }
    public partial class TaggedTextTaggedText
    {
        [JsonProperty("HasTagRule")]
        public IsAllowingJoinCombat HasTagRule { get; set; }

        [JsonProperty("RuleGroup")]
        public List<RuleGroup> RuleGroup { get; set; }

        [JsonProperty("TagTexts")]
        public List<TaggedTextTagText> TagTexts { get; set; }
    }

    public partial class TaggedTextTagText
    {
        [JsonProperty("TagText")]
        public List<TagTextTagText> TagTextArray { get; set; }
    }

    public partial class DialogVariables
    {
        [JsonProperty("DialogVariables")]
        public TypeValPairStr DialogVariable { get; set; }
    }
    public partial class TagTextTagText
    {
        [JsonProperty("DialogVariables")]
        public List<DialogVariables> DialogVariables { get; set; }
        [JsonProperty("LineId")]
        public TypeValPairStr LineId { get; set; }

        [JsonProperty("CustomSequenceId")]
        public TypeValPairStr CustomSequenceId { get; set; }

        [JsonProperty("OldText")]
        public Text OldText { get; set; }

        [JsonProperty("TagText")]
        public Text TagText { get; set; }

        [JsonProperty("stub")]
        public TypeValPairBool Stub { get; set; }
    }

    public partial class Text
    {
        [JsonProperty("handle")]
        public string Handle { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("version")]
        public long Version { get; set; }
    }

    public partial class RootNode
    {
        [JsonProperty("RootNodes")]
        public TypeValPairStr RootNodes { get; set; }
    }

    public partial class Speakerlist
    {
        [JsonProperty("speaker")]
        public List<Speaker> Speaker { get; set; }
    }

    public partial class Speaker
    {
        [JsonProperty("IsPeanutSpeaker")]
        public TypeValPairBool IsPeanutSpeaker { get; set; }

        [JsonProperty("SpeakerMappingId")]
        public TypeValPairStr SpeakerMappingId { get; set; }
        [JsonProperty("SpeakerTagsIndex")]
        public TypeValPairStr SpeakerTagsIndex { get; set; }
        [JsonProperty("index")]
        public TypeValPairStr Index { get; set; }
  

        [JsonProperty("list")]
        public TypeValPairStr List { get; set; }
    }


    public partial class FlagCombinationData
    {
        [JsonProperty("key")]
        public TypeValPairStr key { get; set; }    
        [JsonProperty("val")]
        public TypeValPairStr val { get; set; }
    }
    public partial class FlagCombinationFlags
    {
        [JsonProperty("flaggroup")]
        public List<Flaggroup> flaggroup { get; set; }
    }

    public partial class Flags
    {
        [JsonProperty("flaggroup")]
        public List<Flaggroup> flaggroup { get; set; }
    }
    public partial class FlagCombination
    {
        [JsonProperty("data")]
        public List<FlagCombinationData> data { get; set; }

        [JsonProperty("flags")]
        public List<Flags> flags { get; set; }
    }
    public partial class FlagCombinations
    {
        [JsonProperty("flagCombination")]
        public List<FlagCombination> flagCombinations { get; set; }
    }
    
    public partial class IsPeanuts
    {
        [JsonProperty("data")]
        public List<EditorDataNodeData> HowToTrigger { get; set; }

    }


    public partial class isImportantForStagings
    {
        [JsonProperty("data")]
        public List<EditorDataNodeData> IsImportantForStagings { get; set; }
    }
    public partial class EditorData
    {
        [JsonProperty("HowToTrigger")]
        public TypeValPairStr HowToTrigger { get; set; }

        [JsonProperty("defaultAttitudes")]
        public List<DefaultAttitude> DefaultAttitudes { get; set; }

        [JsonProperty("defaultEmotions")]
        public List<DefaultAttitude> DefaultEmotions { get; set; }

        [JsonProperty("ignoreInvalidFlagCombinations")]
        public List<FlagCombinations> FlagCombination { get; set; }

        [JsonProperty("isImportantForStagings")]
        public List<isImportantForStagings> IsImportantForStagings { get; set; }
    
        [JsonProperty("isPeanuts")]
        public List<IsPeanuts> IsPeanuts { get; set; }

        [JsonProperty("needLayout")]
        public TypeValPairBool NeedLayout { get; set; }

        [JsonProperty("nextNodeId")]
        public TypeValPairUInt32 NextNodeId { get; set; }

        [JsonProperty("speakerSlotDescription")]
        public List<DefaultAttitude> SpeakerSlotDescription { get; set; }

        [JsonProperty("synopsis")]
        public TypeValPairStr Synopsis { get; set; }

        [JsonProperty("templateInstances")]

        public List<TemplateInstances> templateInstances { get; set; }
    }
    public partial class SpeakerLinkingTemplate
    {
        [JsonProperty("data")]
        public List<EditorDataNodeData> data { get; set; }
    }
    public partial class TemplateInstance
    {
        [JsonProperty("data")]
        public List<EditorDataNodeData> data { get; set; }
        //todo prob. wring
        [JsonProperty("flagParameterMappings")]
        public List<EditorDataNodeData> flagParameterMappings { get; set; }
        [JsonProperty("SpeakerLinking")]
        public List<SpeakerLinkingTemplate> SpeakerLinking { get; set; }
        [JsonProperty("speakerlist")]
        public List<Speakerlist> Speakerlist { get; set; }
    }
    public partial class TemplateInstances
    {
        [JsonProperty("templateInstance")]
        public List<TemplateInstance> templateInstance { get; set; }
    }

}
