# How to
1. Extract all data with [BG3-Modders-Multitool](https://github.com/ShinyHobo/BG3-Modders-Multitool/releases) (unpack,decompress if your using a newer version i might be nedded to change the file ending (lsf.lsx -> .lsx ...))
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
5. Controls: Zoom with the mousewheel, drag with mouse, "copy to clipboard"/"save"-Button
6. Limited edits should be possible<br>

| BGDView  | NP++ |
| ------------- | ------------- |
|![NodeExample](https://github.com/gidHog/BGDView/assets/64482285/db1e1528-a9f2-4321-a9ff-50d61086f909)  | ![NodeExample2](https://github.com/gidHog/BGDView/assets/64482285/e1b38f18-22f5-4aaa-b143-d7eb4e4a9b34)  |

---
# <span style="color:red">Disclaimer</span>

1. This is a early and incomplete version, things may not work or be displayed incorrect
   - Not everything is displayed
   - Not everything is editable
   - Currently you can only add nodes and connections and not remove them
3. Loading takes longer with Windows-Defender enabled (M.2 SSD: ~10 vs ~100+ seconds)
4. The code is kinda messy



## Examples (fragments)

<details>
	<summary>[Spoiler warning](Astarion_InParty2.lsj))</summary>
	Change in "Config.json": "RelativeDialoguePath" : {"path" :"Gustav\\Mods\\GustavDev\\Story\\Dialogs\\Companions\\Astarion_InParty2.lsj"},<br> 
	Some fragments of "Astarion_InParty2.lsj":<br> 
	<img src="https://github.com/gidHog/BGDView/assets/64482285/9cc794a1-5a2b-4aa0-b46a-6caa6aa14f32"><br>
	

</details>

<details>
	<summary>[Spoiler warning](DEN_HagTrader.lsj)))</summary>
	Change in "Config.json": "RelativeDialoguePath" : {"path" :"Gustav\\Mods\\Gustav\\Story\\Dialogs\\Act1\\DEN\\DEN_HagTrader.lsj"},<br> 
	<img src="https://github.com/gidHog/BGDView/assets/64482285/c229a6d9-f0f4-4c20-8141-0de74bb1ae3e">
</details>

