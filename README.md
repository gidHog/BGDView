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
3. The code is currently horrendous 
4. The "RootNode" is currently at x = 0 y = 0


## Examples (fragments)

<details>
	<summary>[Spoiler warning](Astarion_InParty2.lsj))</summary>
	Change in "Config.json": "RelativeDialoguePath" : {"path" :"Gustav\\Mods\\GustavDev\\Story\\Dialogs\\Companions\\Astarion_InParty2.lsj"},<br> 
	Some fragments of "Astarion_InParty2.lsj":<br> 
	<img src="https://github.com/gidHog/BGDView/assets/64482285/25c3c8a7-2c7a-499e-9fb4-aa66762f020a"><br>
	<img src="https://github.com/gidHog/BGDView/assets/64482285/bf1fa8a8-aea6-4d34-8855-d1f31f02ff44"><br>


</details>

<details>
	<summary>[Spoiler warning](DEN_HagTrader.lsj)))</summary>
	Change in "Config.json": "RelativeDialoguePath" : {"path" :"Gustav\\Mods\\Gustav\\Story\\Dialogs\\Act1\\DEN\\DEN_HagTrader.lsj"},<br> 
	<img src="https://github.com/gidHog/BGDView/assets/64482285/c37d508d-5a59-41ae-91b4-77383d53ccc0">
</details>

