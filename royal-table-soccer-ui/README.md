# Royal Table Soccer — Modern UI

Redesign of three game scenes for **Unity 6** using **UI Toolkit** (UXML + USS + C#).
Resolution base: **1920x1080 landscape** (Android / iOS).
Two visual directions are provided so you can A/B compare:

| Option | Style | Folder | Browser preview |
|--------|-------|--------|-----------------|
| **1** | **Stadium Night** — royal blue cards, neon purple/cyan/magenta glow, rounded pills | `royal-table-soccer-ui/` (root) | `preview.html` |
| **2** | **FUT Premium** — black + gold + crimson, angular FUT-style trading cards, dense info | `royal-table-soccer-ui/opcao2/` | `preview-opcao2.html` |

Both use **Lilita One** as the only font, both target the same `1920x1080` reference resolution, and both controllers expose the same public API (`SetTeams`, `SetData`, `ShowOpponent`, etc.) so you can swap between options without touching your data layer.

## Scenes included

1. **TeamSelect** — pick the club for the championship (formation preview on the right)
2. **Profile** — player avatar, level, stats, badge slot, photo controls
3. **Matchmaking** — searching for a real opponent, VS layout, cancel button

## Folder layout

```
royal-table-soccer-ui/
├── Theme/
│   └── Theme.uss                  # Shared tokens (colors, spacing, buttons, typography)
├── TeamSelect/
│   ├── TeamSelect.uxml
│   ├── TeamSelect.uss
│   └── TeamSelectController.cs
├── Profile/
│   ├── Profile.uxml
│   ├── Profile.uss
│   └── ProfileController.cs
└── Matchmaking/
    ├── Matchmaking.uxml
    ├── Matchmaking.uss
    └── MatchmakingController.cs
```

## How to install in your Unity 6 project

1. **Copy the folder** `royal-table-soccer-ui/` into `Assets/UI/` in your Unity project.

2. **Import Lilita One as a TMP/SDF font asset:**
   - Place the `.ttf` in `Assets/Fonts/`.
   - `Window → TextMeshPro → Font Asset Creator` → generate `LilitaOne-Regular SDF.asset`.
   - The USS already references `Assets/Fonts/LilitaOne-Regular SDF.asset`.

3. **Stadium background sprite:**
   - Save your stadium photo as `Assets/Sprites/UI/stadium_night.png`.
   - You can reuse the same one from the main menu.

4. **Per scene setup:**
   - Create a new scene (e.g. `TeamSelect.unity`).
   - Add a `UI Document` GameObject (`GameObject → UI Toolkit → UI Document`).
   - In the inspector, drag the matching `.uxml` into **Source Asset**.
   - Create a `PanelSettings` asset if you don't have one (`Assets → Create → UI Toolkit → Panel Settings Asset`); set:
     - **Scale Mode:** `Scale With Screen Size`
     - **Reference Resolution:** `1920 x 1080`
     - **Match:** `0.5`
   - Attach the matching controller `.cs` to the same GameObject.

5. **Wire data:** each controller exposes public fields/methods (e.g. `TeamSelectController.SetTeams(Team[] teams)`). Hook them to your existing game data wherever you currently load teams / profile / matchmaking state.

## Design tokens (quick reference)

| Token              | Value                          | Used for                  |
|--------------------|--------------------------------|---------------------------|
| `--rts-bg-deep`    | `#040816`                      | Outer background          |
| `--rts-bg-card`    | `rgba(20,36,96,0.85)`          | Card body                 |
| `--rts-royal-blue` | `#1C38B4`                      | Primary buttons           |
| `--rts-neon-purple`| `#AA5AFF`                      | Border glow (default)     |
| `--rts-neon-cyan`  | `#00E6FF`                      | Hover glow / accents      |
| `--rts-neon-magenta`| `#FF3CC8`                     | Highlights, "VS"          |
| `--rts-text-gold`  | `#FFC83C`                      | Player name / star        |

## Notes

- All sizes use the `--rts-fs-*` scale so changing the reference resolution only requires editing `Theme.uss`.
- Buttons have built-in hover (`scale 1.04` + cyan border) and active (`scale 0.97`) states — no extra code needed.
- The matchmaking spinner uses a USS rotation animation; the timer is updated from C#.
