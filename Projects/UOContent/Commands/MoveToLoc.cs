using System;
using Server;
using Server.Commands;

namespace Server.Custom.Commands
{
    public static class MoveToLoc
    {
        public static void Initialize()
        {
            // Registering the command for GMs and above
            CommandSystem.Register("MoveToLoc", AccessLevel.GameMaster, new CommandEventHandler(MoveToLoc_OnCommand));
        }

        [Usage("MoveToLoc <x offset> <y offset> [z offset]")]
        [Description("Moves your character relative to your current coordinates.")]
        private static void MoveToLoc_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            if (e.Arguments.Length < 2)
            {
                from.SendMessage(38, "Format: [MoveToLoc <x offset> <y offset> [z offset]");
                return;
            }

            // Attempt to parse the first two arguments as integers
            if (int.TryParse(e.Arguments[0], out int xOffset) && int.TryParse(e.Arguments[1], out int yOffset))
            {
                int newX = from.X + xOffset;
                int newY = from.Y + yOffset;
                int newZ = from.Z;

                // If they provided a 3rd argument, use it as a Z offset
                if (e.Arguments.Length >= 3 && int.TryParse(e.Arguments[2], out int zOffset))
                {
                    newZ = from.Z + zOffset;
                }
                else if (from.Map != null && from.Map != Map.Internal)
                {
                    // If no Z offset is provided, automatically snap to the ground height of the new tile
                    newZ = from.Map.GetAverageZ(newX, newY);
                }

                Point3D newLoc = new Point3D(newX, newY, newZ);

                // Move the GM
                from.MoveToWorld(newLoc, from.Map);
                from.SendMessage(68, $"You have moved to {newX}, {newY}, {newZ}.");
            }
            else
            {
                from.SendMessage(38, "Invalid offsets. Please use whole numbers (e.g., +50, -25).");
            }
        }
    }
}
