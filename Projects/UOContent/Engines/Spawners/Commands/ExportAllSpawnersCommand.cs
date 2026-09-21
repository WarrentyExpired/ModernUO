// Exports all spawners on the map.
using System;
using Server;
using Server.Commands;

namespace Server.Custom
{
    public class ExportSpawnersAlias
    {
        public static void Initialize()
        {
            CommandSystem.Register("EAS", AccessLevel.Administrator, new CommandEventHandler(OnCommand));
        }

        [Usage("EAS [filename]")]
        [Description("A quick shortcut for [global ExportSpawners")]
        public static void OnCommand(CommandEventArgs e)
        {
            Mobile m = e.Mobile;

            string commandToRun = $"{CommandSystem.Prefix}global exportspawners";
            string args = e.ArgString.Trim();
            if (!string.IsNullOrEmpty(args))
            {
                commandToRun += " " + args;
            }

            m.SendMessage(68, $"Running shortcut: {commandToRun}");
            CommandSystem.Handle(m, commandToRun);
        }
    }
}
