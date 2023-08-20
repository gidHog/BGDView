# How to
1. Extract all data with [BG3-Modders-Multitool](https://github.com/ShinyHobo/BG3-Modders-Multitool/releases)
2. The following folder structure is needed ("UnpackedData" name and path can be changed in the config):
```
BGEdit.exe
Config.json
UnpackedData/
├─ Gustav/
│  ├─ Public/
│  │  ├─ GustavDev/
│  │  │  ├─ Origins/
│  │  │  │  ├─ Origins.lsx
├─ English/
│  ├─ Localization/
│  │  ├─ English/
│  │  │  ├─ english.xml (from .loca)
├─ *\\Dialogs\\*/
│  ├─ *Filename*.lsj
├─ *\\Characters/
│  ├─ _merged.lsx
├─ ..../

```

3. Edit the Config.json to fit your needs
	1. Change to .lsj-Dialogue you want to view
	2. If speakernames are missing edit/add merged.lsx 
		1. Name is searched first in origins
		2. After that in merged
		3. If nothing is found SpeakerGroups.lsx is used
		4. If nothing is found the UUID is shown
	3. "RelativeLocalizationPath" is the path to the english.xml (convert the .loca to xml before!)
	4. "RelativeflagPaths" are the paths to tags and flags (currently treated almost the same)
4. Run the .exe, or compile the code yourself
5. Controls: Zoom with the mousewheel, drag with mouse
---
# <span style="color:red">Disclaimer</span>

1. This is a early and incomplete version, things may not work or be displayed incorrect (some .lsj loop)
2. Loading takes longer with Windows-Defender enabled
3. The Code is currently horrendous 
4. The RootNode is currently at x = 0 y = 0


## Examples (fragments)


<details>
  <summary>[Spoiler warning](Astarion_InParty2.lsj))</summary>
  Change in the Config: "RelativeDialoguePath" : {"path" :"Gustav\\Mods\\GustavDev\\Story\\Dialogs\\Companions\\Astarion_InParty2.lsj"},<br> 
  Some fragments of "Astarion_InParty2.lsj":<br> 
 (https://github.com/gidHog/BGDView/assets/64482285/72e309c0-f394-4863-9ee6-490808d1c70a)<br> 
 (https://github.com/gidHog/BGDView/assets/64482285/7fc11908-c121-4816-8d3d-a433a75344c4)<br> 

</details>

<details>
  <summary>[Spoiler warning](DEN_HagTrader.lsj)))</summary>
  Change in the Config: "RelativeDialoguePath" : {"path" :"Gustav\\Mods\\Gustav\\Story\\Dialogs\\Act1\\DEN\\DEN_HagTrader.lsj"},<br> 
  (https://github.com/gidHog/BGDView/assets/64482285/2c2bcd44-ae40-4b5a-a4a7-d4582cd42461)<br> 
</details>


