using System;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using Server.Items;

namespace Server.Gumps;

public class SoulAtlasGump : Gump
{
    private readonly PlayerMobile _player;
    private readonly SoulAtlas _atlas;

    // Define the list of your 8 progression dungeons
    private static readonly string[] DungeonNames = new string[8]
    {
        "SHAME (TIER 1)",
        "DECEIT (TIER 2)",
        "COVETOUS (TIER 3)",
        "HYTHLOTH (TIER 4)",
        "DESPISE (TIER 5)",
        "DESTARD (TIER 6)",
        "WRONG (TIER 7)",
        "THE VOID FINALE (TIER 8)"
    };

    // Define coordinates just outside or inside each dungeon entrance
    // NOTE: Replace these with your exact map coordinates later!
    private static readonly Point3D[] DungeonCoords = new Point3D[8]
    {
        new Point3D(3515, 173, 0),  // Shame
        new Point3D(4111, 434, 5),   // Deceit
        new Point3D(2498, 921, 0),   // Covetous
        new Point3D(4721, 3816, 0),  // Hythloth
        new Point3D(1301, 1080, 0),  // Despise
        new Point3D(1176, 2640, 0),  // Destard
        new Point3D(2043, 224, 0),   // Wrong
        new Point3D(1361, 895, 0)    // Finale (e.g., Wind/Doom Ruins)
    };

    public SoulAtlasGump(PlayerMobile pm, SoulAtlas atlas) : base(150, 150)
    {
        _player = pm;
        _atlas = atlas;

        Closable = true;
        Disposable = true;

        AddPage(0);
        // The Parchment Book Frame look
        AddBackground(0, 0, 420, 460, 9270);
        AddAlphaRegion(10, 10, 400, 440);

        AddLabel(140, 25, 1152, "SOUL ATLAS ARCHIVE");
        AddImageTiled(20, 50, 380, 2, 0x2424);

        int yOffset = 70;

        for (int i = 0; i < 8; i++)
        {
            bool isUnlocked = _atlas.IsDungeonUnlocked(i);

            if (isUnlocked)
            {
                // Unlocked rows get a blue recall button (ID 10 + i) and golden text
                AddButton(30, yOffset + 2, 4005, 4007, 10 + i, GumpButtonType.Reply, 0);
                AddLabel(75, yOffset, 0x480, DungeonNames[i]);
                AddLabel(300, yOffset, 0x3F, "[READY]");
            }
            else
            {
                // Locked rows are dead un-clickable lines with red markers
                AddImage(30, yOffset + 4, 4017);
                AddLabel(75, yOffset, 0x384, DungeonNames[i]);
                AddLabel(300, yOffset, 0x22, "[LOCKED]");
            }

            yOffset += 45;
        }
    }

    public override void OnResponse(NetState sender, in RelayInfo info)
    {
        if (info.ButtonID >= 10 && info.ButtonID < 18)
        {
            int index = info.ButtonID - 10;

            if (_atlas == null || _atlas.Deleted || !_player.InRange(_atlas.GetWorldLocation(), 2))
            {
                _player.SendMessage("The archive link has broken.");
                return;
            }

            if (_atlas.IsDungeonUnlocked(index))
            {
                Point3D targetLoc = DungeonCoords[index];
                Map targetMap = Map.Trammel; // Enforces your world map selection context

                // Play classic recall departure sparks and sound
                _player.FixedEffect(0x3709, 10, 30);
                _player.PlaySound(0x1FC);

                // Instantly shift locations safely
                _player.MoveToWorld(targetLoc, targetMap);

                // Play arrival sparks at destination
                _player.FixedEffect(0x3709, 10, 30);
                _player.PlaySound(0x1FC);

                _player.SendMessage(0x3F, $"The Soul Atlas safely delivers you to {DungeonNames[index]}.");
            }
        }
    }
}
