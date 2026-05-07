# How to collaborate
## Merge and merge conflitcs
Use the UnityYAMLMerge tool to merge scene and prefab files in a semantically correct way. The tool can be accessed from the command line and is also available to third-party version controlA system for managing file changes.

To set up git with UnityYAMLMerge you must set up the path of the UnityYAMLMerge by creating and exporting the path on an enviroinment variable.
The path is something like:
- On windows:
```
    C:\Program Files\Unity\Hub\Editor\VERSION\Editor\Data\Tools\UnityYAMLMerge.exe
        or
    C:\Program Files\Unity\Hub\Editor\VERSION\Editor\Data\Tools\UnityYAMLMerge.exe
```
- On Linux:
```
```

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