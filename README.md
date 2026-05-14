# How to configure

## Requirements

- Unity Hub: 3.18.0 or above

- Unity 6.3 LTS (6000.3.13f1) or at least Unity 6

    - Android Build Support

        - OpenJDK

        - Android SDK & NDK Tools

- Visual Studio Code with the Unity extension 1.2.2 or above, Visual Studio in alternative

## Clone and set up

Clone the repo, add it into Unity Hub and start it using the proper Unity Editor version. Once opened, go to `Build Profiles` and switch to Android. Packages and dependencies should be already imported.

In order to use the rig and the interactions inside the Editor, go to `Project Settings > XR Plug-in Management > XR Interaction Toolkit` and enbale `Use XR Interaction Simulator in scenes`.

## Merge and merge conflitcs

Use the [UnityYAMLMerge](https://docs.unity3d.com/6000.4/Documentation/Manual/SmartMerge.html) tool to merge scene and prefab files in a semantically correct way. The tool can be accessed from the command line and is also available to third-party version controlA system for managing file changes.

To set up git with UnityYAMLMerge you must set up the path of the UnityYAMLMerge by creating and exporting the path on an enviroinment variable.
The path is something like:

- On Windows:
```
    C:\Program Files\Unity\Editor\Data\Tools\UnityYAMLMerge.exe

        or

    C:\Program Files (x86)\Unity\Editor\Data\Tools\UnityYAMLMerge.exe
```

<!-- - On Linux:
```
``` -->

To create the variable run:

- On Windows:
```
    setx UNITY_MERGE <PATH>
```

- On Linux:
```
    export UNITY_MERGE=<PATH>
```

Finally you must need to run the following command on the root of the repo:
```
    git config --local include.path ../.gitconfig
```