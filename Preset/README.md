# Rou Configuration Guide

This is a guide for Rou configuration. Currently you can only change options/configs by manually editing the json files. The options UI will be implement later.

## File structure
```
/ - Root directory which Rou located
  /configs - All configure files should be in this folder
    /default.json - The default actions and some options
    /<process_name>.json - The actions for specific process. 
                           The `<process_name>` should be replaced (eq. "chrome.json").
```

## Configure file

The `json` file may contains the following fields

Field | Type | Required | Note
--- | --- | --- | ---
actions | [ActionsCollection](#actionscollection) | Yes 
configs | [ConfigObject](#configobject) | No | Should only be present in `default.json` 

### ActionsCollection

`ActionsCollection` is an array of `Actions` which defined below.

**Actions**

Field | Type | Required | Note
--- | --- | --- | ---
type | String | Yes | "keyboard" is the only accepted value for now
text | String | Yes | The text label of this action
icon | String | Yes | Rou uses [Material Icons](https://material.io/icons/), you can find the icon id there.
keys | String/[String] | Required with type:`keyboard` | See [Keys](#keys)

#### Keys

~ | Example
--- | ---
For single key press | "MediaNextTrack"
For combined keys |  ["D LWin", "P Tab", "U LWin"]

- "D" for key down
- "P" for key press (down, then up)
- "U" for key up