# How to
1. Extract all data with [BG3-Modders-Multitool](https://github.com/ShinyHobo/BG3-Modders-Multitool/releases)(unpack and decompress). If you are using a newer version, you might need to change the file extension (e.g., from .lsf to .lsx).
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
6. Limited edits should be possible<br> (For selecting the current node in the UI, you need to move it)
7. The popup window currently doesn't allow editing certain values.

| BGDView  | NP++ |
| ------------- | ------------- |
|![UI](https://github.com/gidHog/BGDView/assets/64482285/e16e58ec-84f0-41f9-b072-5989e18bbd12) | ![NodeExample2](https://github.com/gidHog/BGDView/assets/64482285/e1b38f18-22f5-4aaa-b143-d7eb4e4a9b34)  |
![TagWindow](https://github.com/gidHog/BGDView/assets/64482285/d4403cc0-e4d1-4bce-ae61-771e6a5b4d91)
---

# <span style="color:red">Disclaimer</span>

1. This is a early and incomplete version, things may not work or be displayed incorrect
   - Not everything is displayed
   - Not everything is editable
   - Currently you need to move the node to display it in the UI
3. Loading takes longer with Windows-Defender enabled (M.2 SSD: ~10 vs ~100+ seconds) -> The option for merged flags/tags should be used.
5. The code is kinda messy



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

