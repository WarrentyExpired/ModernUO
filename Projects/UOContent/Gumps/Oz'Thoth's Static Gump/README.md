# Oz'Thoth's Static Gump

Staff tool for placing static decoration. ~135 `[Add*` commands open
category gumps; each gump shows a paginated grid of static tile thumbnails;
clicking one drops that static at the caller. Used by GMs to dress towns,
dungeons, custom regions, and event scenes without leaving the client.

Heavy file count — this is the menu/page surface, not new gameplay logic.

## Command families (all GameMaster)

The commands are partitioned into category pages so each gump fits a
practical number of entries.

| Family | Examples |
|---|---|
| Walls / Roofs / Floors | `[AddWallPt*`, `[AddRoofPt*`, `[AddFloorPt1`..`[AddFloorPt12` |
| Stairs / Fences / Arches / Signposts | `[AddStair`, `[AddFence*`, `[AddArch*`, `[AddSignpost*` |
| Lights / Plants / Food | `[AddLight*`, `[AddPlant*`, `[AddFood*` |
| Furniture / Gear / Misc | `[AddFurnPt1`..`[AddFurnPt6`, `[AddGearPt1`..`[AddGearPt5`, `[AddMiscPt1`..`[AddMiscPt12+` |
| Animation statics | `[AddAnimStPt1`..`[AddAnimStPt6` |
| Custom packs | `[AddCustomPt1`..`[AddCustomPt20` |

For the authoritative list, run `[help Add` in-game or grep the folder for
`CommandSystem.Register`.

## Usage

1. Run the category command, e.g. `[AddWallPt1`.
2. Pick a tile from the paginated gump.
3. The static drops at your feet (or last targeted tile, depending on the
   page's mode).
4. Use the regular `[Move` / `[Remove` commands to fine-tune placement.

## Production notes

- Static placement is permanent across world saves (standard `Static`
  items). Use the standard admin tools (`[Remove`, `[Delete`) to undo.
- For bulk world dressing, prefer the [MapEditor](../../MapEditor/README.md)
  system — it writes to the `.mul` files directly and is the correct path
  for production map changes.
- This UI is best for *event setup*, *one-off decoration*, and *custom
  region polish*, not for canonical map edits.

## Credits

Original "Static Gump" UI by Oz'Thoth (RunUO era); ported and extended for
EvoRealm. Adds the per-category `Pt*` pages so the menu doesn't outgrow a
single gump page.
