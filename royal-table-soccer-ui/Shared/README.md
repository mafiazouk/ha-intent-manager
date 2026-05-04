# Shared components

Reusable bits you can plug into any scene without depending on a specific Theme or controller.

## Spinner ring (rotating outline around an avatar/icon)

Same effect used in the Matchmaking scene of option 1.

### Files
- `Spinner.uss` — styles (`.rts-spinner` + color variants)
- `UISpinner.cs` — tiny `MonoBehaviour` that rotates every element marked with the class

### How to use in any scene

**1. Link the stylesheet in your UXML** (top of the file, alongside other `<Style src=...>`):

```xml
<Style src="project://database/Assets/UI/royal-table-soccer-ui/Shared/Spinner.uss" />
```

**2. Drop the spinner structure where you want the rotating ring:**

```xml
<ui:VisualElement class="rts-spinner-wrap" style="width:300px; height:300px;">
    <ui:VisualElement class="rts-spinner rts-spinner-cyan" />

    <!-- The thing being framed (avatar, crest, icon, button, anything) -->
    <ui:VisualElement class="rts-spinner-content"
                      style="width:260px; height:260px; border-radius:999px;
                             background-image: url(...);" />
</ui:VisualElement>
```

The wrap is the **size of the ring**. Anything inside `rts-spinner-content` stays still while the `rts-spinner` element rotates around it.

**3. Attach the script** to the same GameObject as the `UIDocument` of that scene:
- Click the GameObject with the `UIDocument`
- **Add Component → UISpinner**
- Configure speed (default 220 deg/sec) if you want

That's it. **All** elements in that scene with the class `rts-spinner` start rotating automatically.

### Color variants

Add one of these classes alongside `rts-spinner`:

| Class | Color |
|---|---|
| `rts-spinner-cyan` | Ciano (matchmaking-left default) |
| `rts-spinner-magenta` | Magenta neon |
| `rts-spinner-purple` | Roxo neon |
| `rts-spinner-gold` | Dourado |
| `rts-spinner-green` | Verde neon |
| `rts-spinner-red` | Vermelho (matchmaking-right default) |

### Reverse direction

Replace `rts-spinner` with `rts-spinner-reverse` (use both classes together to rotate counter-clockwise). Useful when you have two spinners next to each other and want them spinning opposite directions, like the matchmaking VS.

### Multiple spinners on the same scene

You can have any number — they all rotate at the same speed using a single `UISpinner` component. If you want **different speeds** for different rings, just add a second `UISpinner` component with a different `Spin Class` (e.g. `rts-spinner-fast`) and reference that class in the UXML elements you want to spin faster.
