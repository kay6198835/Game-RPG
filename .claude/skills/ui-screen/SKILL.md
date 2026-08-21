---
name: ui-screen
description: "Build runnable UI Toolkit screens (UXML + USS + controller + wired scene) from a mockup image, a Figma description, or a plain-text flow. Produces structure-only UI with a flat placeholder stylesheet - no theming. Reads the pinned Unity version and existing project UI assets so output drops straight into the project and plays without manual Inspector wiring."
argument-hint: "[screen names, or path to mockup image, or flow description]"
user-invocable: true
allowed-tools: Read, Glob, Grep, Write, Edit, Bash, AskUserQuestion
---

When this skill is invoked:

## Scope contract

This skill delivers **structure and behaviour**, not visual design.

| In scope | Out of scope |
|---|---|
| Screen inventory and hierarchy | Theming, brand colors, custom fonts |
| Element naming and `.uxml` markup | Sprites, icons, 9-slice backgrounds |
| Navigation flow between screens | Animation polish beyond `:hover` / `:active` |
| Button / slider / toggle event wiring | Data binding to gameplay ScriptableObjects |
| A scene that runs on Play with zero Inspector work | Localization, accessibility audit |

The stylesheet emitted is a **flat placeholder**: dark background, rounded buttons, hover state, nothing more. State that explicitly in the final report so the user knows restyling is a separate pass. If the user wants theming, point them at `/ux-design` for the spec and the art bible for the palette.

---

## 1. Read the project before writing anything

Never assume - this project's docs drift from disk. Run these checks:

```bash
cat ProjectSettings/ProjectVersion.txt
grep -n "modules.uielements\|inputsystem" Packages/manifest.json
grep -n "activeInputHandler" ProjectSettings/ProjectSettings.asset
find Assets -name "*.uxml" -o -name "*.uss" -o -name "PanelSettings.asset"
find Assets/Scenes -name "*.unity"
```

Record and carry forward:

- **Unity version** - the YAML shapes in `reference/unity-yaml-ids.md` are verified for 2022.3 LTS. If the project has moved off 2022.x, re-verify importer ids against a package-cache `.meta` before authoring any `.meta` by hand.
- **`com.unity.modules.uielements` present** - required. If absent, stop and tell the user; do not add packages unprompted.
- **`activeInputHandler`** - `0` = legacy only (`Keyboard.current` returns null, so gate Esc handling behind a null check or use legacy input), `1` = Input System only, `2` = Both.
- **Existing `PanelSettings.asset`** - reuse it. A second one is a silent source of double-rendered UI.
- **Real scene paths** - `CLAUDE.md` has listed scenes that do not exist on disk. Only reference a scene name confirmed with `find`.

---

## 2. Ingest the source material

The user supplies one of: a mockup image, a Figma frame description, a flow diagram, or plain text. Read images with the Read tool.

Extract a **screen table** before writing code:

| Field | What to pull out |
|---|---|
| Screen name | One per artboard / frame. PascalCase file name, kebab-case root `name` attribute. |
| Elements | Every interactive control plus every label, top to bottom, in visual order. |
| Element ids | `btn-*`, `slider-*`, `toggle-*`, `label-*`, `field-*`, `list-*`. Kebab-case, unique per screen. |
| Transitions | Which control leads to which screen, and what closes or returns. |
| Entry state | Which screen is visible at Play, and whether the game is paused behind it. |
| Persistent state | Anything a control reads on open and writes on change (volume, fullscreen, difficulty). |

Then resolve ambiguity **once**, in a single `AskUserQuestion` round, only for what the mockup genuinely cannot answer:

- A back button whose destination differs by entry path - confirm whether to track the caller (the `_settingsCameFrom` pattern in the template).
- A control with no visible target - ask what it does, or mark it TODO and say so in the report.
- Whether a screen pauses the game (`Time.timeScale = 0`) or overlays live gameplay.

Do not ask about anything the image already shows. Do not ask about styling - out of scope by contract.

Echo the finished screen table back to the user before generating files.

---

## 3. Choose the file layout

Default, and what the templates assume:

```
Assets/UI/Screens/<ScreenName>.uxml     one per screen
Assets/UI/Styles/<Prefix>UI.uss         one shared stylesheet
Assets/Script/UI/<Prefix>UIController.cs
Assets/Scenes/<Prefix>Sample.unity      demo scene, only when it must run standalone
```

Rules that matter:

- **Never author new assets under a folder whose name contains a space.** Unity's default `Assets/UI Toolkit/` folder holds `PanelSettings.asset` - reuse that asset in place, but put new `.uxml` / `.uss` under `Assets/UI/` so relative `src` paths stay clean.
- Reference the stylesheet from inside each `.uxml` with a **relative** path: `<Style src="../Styles/GameUI.uss" />`. Avoids `project://database/...` URI escaping entirely.
- One `.uss` shared by all screens unless the user asks otherwise. Put the palette in a comment block at the top so the later theming pass has one place to edit.

---

## 4. Generate the screens

Start from `templates/Screen.uxml.template`. Per screen:

- Root `VisualElement` with `name` = kebab-case screen id and `class="screen"`.
- One `.panel` child holding the controls, so the controller only ever toggles the root.
- Controls in the visual order read off the mockup, each with the `name` from the screen table.
- Label + control rows use `class="row"` with a `.row__label`.

Emit the stylesheet from `templates/GameUI.uss`. Extend it only with classes the mockup demands (a new row variant, a wider panel). Do not introduce a color that is not already in the palette comment.

---

## 5. Generate the controller

Use `templates/UIController.cs.template`. One MonoBehaviour clones every screen into a single `UIDocument` root and toggles visibility. Keep that shape - one document, N cloned trees - unless the user asks for separate `UIDocument`s.

Non-obvious details the template already encodes; preserve them:

- **`TemplateContainer` does not fill its parent.** After `asset.Instantiate()`, set `style.position = Position.Absolute` and call `StretchToParentSize()`, or every screen collapses to zero height.
- **Self-wiring `UIDocument`.** `AddComponent<UIDocument>()` runs `OnEnable` before `panelSettings` can be assigned, so the panel builds with no settings. Cycle `enabled = false; panelSettings = ...; enabled = true;` to rebuild. This is what lets the scene contain nothing but the controller component.
- **Build the tree in `Start`, not `OnEnable`** - `rootVisualElement` is only reliably attached after the document's own enable cycle completes.
- **Reset `Time.timeScale = 1f` in `OnDisable`.** A pause screen disabled mid-pause otherwise freezes the game permanently.
- **Null-guard `Keyboard.current`** before reading it. Null when `activeInputHandler` is legacy-only.
- **Editor-only asset fallback.** `ResolveMissingAssets()` re-loads each `VisualTreeAsset` by path under `#if UNITY_EDITOR` when the Inspector slot is empty - the safety net that keeps the scene playable if a GUID fails to resolve. It must stay inside the `#if`; `AssetDatabase` does not exist in a build.

Project rules that apply (from `.claude/rules/ui-code.md`):

- UI never owns or mutates gameplay state. `AudioListener.volume` and `Screen.fullScreen` are engine settings and are fine; `PlayerData` fields are not - route those through `EventManager`.
- Fields are `[SerializeField] private`, never public.
- Live game data: subscribe in `OnEnable`, unsubscribe in `OnDisable`, via `EventManager.Resgister` / `UnResgister` (the typo is the real API - match it).
- No singletons.

---

## 6. Make it run without Inspector work

Only when the user wants a standalone runnable scene. Otherwise hand them the wiring steps and stop.

Author `.meta` files by hand with deterministic GUIDs, then author the scene referencing those GUIDs. Every id needed is in `reference/unity-yaml-ids.md` - read it before writing YAML.

Generate GUIDs deterministically so re-running the skill does not orphan references:

```bash
g(){ printf '%s' "$1" | md5sum | cut -c1-32; }
UIC=$(g "ProjectName/UIController.cs")
```

Build the scene header by slicing it off an existing project scene rather than hand-writing settings blocks:

```bash
awk '/^--- .u.(1|1001) &/{exit} {print}' Assets/Scenes/SampleScene.unity > header.txt
```

(The pattern matches the first `--- !u!1 &` or `--- !u!1001 &` line.) That captures `OcclusionCullingSettings` through `NavMeshSettings` exactly as this project's Unity version serializes them. Append a Main Camera and the UI GameObject after it.

**No EventSystem GameObject is needed.** With no UGUI `EventSystem` in the scene, UI Toolkit falls back to its own internal event system and buttons work. Add one only if the user also needs UGUI in the same scene - guids are in the reference file.

Heredocs with long markdown bodies fail intermittently in this shell. Write `.md`, `.uxml`, `.uss`, and `.cs` files with the Write tool; keep Bash for the short `.meta` and scene-YAML generation loops where shell variables are doing real work.

After writing files, verify: every `guid:` referenced in the scene exists in a `.meta` on disk, and every `name` queried by `Q<T>("...")` in the controller exists in a `.uxml`. Grep for both.

---

## 7. Report

Report in English with Vietnamese in parentheses for key terms on first use, per `.claude/rules/language-reporting.md`.

Cover:

1. Files created, grouped by folder.
2. What the user does to run it - one line if the scene was authored ("open scene X, press Play"), or the numbered UIDocument wiring steps if not.
3. The flow as implemented: entry screen, every transition, what Esc does, what pauses.
4. **Explicitly**: styling is placeholder only, restyle by editing the one `.uss`.
5. Anything left TODO - a control with no defined target, a scene name absent from disk, a screen the mockup showed only partially.

Never claim it runs if the references were not verified. Say what was checked.
