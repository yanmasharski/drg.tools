# IronSource Ads Integration

This package provides integration with IronSource ads through the Unity LevelPlay SDK.

## Dependencies
- DRG.Tools ("https://github.com/yanmasharski/drg.tools.git")
- Unity LevelPlay SDK

## Features
- Interstitial ads
- Rewarded video ads
- Automatic ad loading and retry logic

## Usage
- Copy the files from this folder into any folder under your Assets/ directory in your Unity project.
- Open the copied files and remove any scripting define symbols related to "DRG_IRONSOURCE".

This ensures the files work independently of the IronSource/LevelPlay integration.

⚠️ Why this is needed:

Due to Unity's assembly definition system limitations, it's not possible to reference assemblies from IronSource/LevelPlay tools directly.
As a workaround, the necessary files must be copied into the main project and stripped of unsupported defines.