# Unity YAML ids for hand-authored UI assets

Verified against Unity 2022.3.62f3 in this project (2026-08-21). Re-verify before use on
a different major version: every value below can be read back off disk from an existing
`.meta` in `Library/PackageCache/` or `Assets/`.

## Importer script ids (`.meta` files)

All use `guid: 0000000000000000e000000000000000` (Unity built-in) with these `fileID`s:

| Asset | Importer | `script:` fileID |
|---|---|---|
| `.uxml` | UIElementsViewImporter | `13804` |
| `.uss` | StyleSheetImporter | `12385` |
| `.tss` | ThemeStyleSheetImporter | `12388` |
| `PanelSettings.asset` | NativeFormatImporter (`mainObjectFileID: 11400000`) | n/a |

How to re-verify:

```bash
find Library/PackageCache -name "*.uxml.meta" | head -1 | xargs cat
find Library/PackageCache -name "*.uss.meta"  | head -1 | xargs cat
```

`.uss` metas carry `disableValidation: 0`; `.uxml` metas may omit it.

## Main-object fileIDs (for references from a `.unity` / `.prefab`)

| Referenced asset | fileID | `type:` |
|---|---|---|
| `VisualTreeAsset` (a `.uxml`) | `9197481963319205126` | `3` |
| `PanelSettings.asset` | `11400000` | `2` |
| A MonoBehaviour script (`.cs`) | `11500000` | `3` |

## Known component GUIDs

| Component | GUID | Source |
|---|---|---|
| `EventSystem` | `76c392e42b5098c458856cdf6ecaaaa1` | com.unity.ugui |
| `InputSystemUIInputModule` | `01614664b831546d2ae94a42149d80ac` | com.unity.inputsystem 1.14.0 |

Only needed when the scene must also host UGUI. UI Toolkit alone does not require an
EventSystem - it falls back to its own internal event system.

`UIDocument` is a built-in engine MonoBehaviour, so its `m_Script` fileID lives in the
built-in table and is **not** documented here. Do not guess it. Instead let the controller
add the component itself at runtime (`gameObject.AddComponent<UIDocument>()`), which is
why the template does the enabled off/on cycle - see SKILL.md section 5.

## `.meta` templates

`.cs`:

```yaml
fileFormatVersion: 2
guid: <32 hex>
MonoImporter:
  externalObjects: {}
  serializedVersion: 2
  defaultReferences: []
  executionOrder: 0
  icon: {instanceID: 0}
  userData: 
  assetBundleName: 
  assetBundleVariant: 
```

`.uxml` (swap `13804` for `12385` and add `disableValidation: 0` for `.uss`):

```yaml
fileFormatVersion: 2
guid: <32 hex>
ScriptedImporter:
  internalIDToNameTable: []
  externalObjects: {}
  serializedVersion: 2
  userData: 
  assetBundleName: 
  assetBundleVariant: 
  script: {fileID: 13804, guid: 0000000000000000e000000000000000, type: 0}
```

Folder:

```yaml
fileFormatVersion: 2
guid: <32 hex>
folderAsset: yes
DefaultImporter:
  externalObjects: {}
  userData: 
  assetBundleName: 
  assetBundleVariant: 
```

Scene (`.unity`) uses the same `DefaultImporter` block without `folderAsset`.

The trailing spaces after `userData:`, `assetBundleName:`, `assetBundleVariant:` match what
Unity writes. They are not required for parsing but keep diffs clean when Unity rewrites
the file.

## Scene YAML skeleton

Take the settings header from an existing scene rather than hand-writing it:

```bash
awk '/^--- .u.(1|1001) &/{exit} {print}' Assets/Scenes/SampleScene.unity > header.txt
```

Then append GameObjects. Minimum viable UI scene is a Main Camera
(`!u!1` GameObject + `!u!4` Transform + `!u!20` Camera + `!u!81` AudioListener) and a UI
GameObject (`!u!1` + `!u!4` + `!u!114` MonoBehaviour pointing at the controller script).

Omitted serialized fields fall back to defaults on load, so a minimal Camera block is safe.
Set `orthographic: 1` and `m_ClearFlags: 2` (solid color) for a 2D project.

Serialized field names in the `!u!114` block must match the C# field names exactly,
including the leading underscore.

## Adding the scene to the build

Editor Play Mode runs whatever scene is open - build settings are irrelevant for that.
A scene only needs an entry in `ProjectSettings/EditorBuildSettings.asset` when
`SceneManager.LoadScene` targets it, or when it ships. Appending an entry changes build
indices, so ask before editing that file.
