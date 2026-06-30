using System;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Mobiles;

namespace Server.Custom.Spawners
{
    public class AdvancedSpawnerGump : Gump
    {
        private AdvancedSpawner _spawner;
        private int _page;

        // Added _page parameter which defaults to 0
        public AdvancedSpawnerGump(AdvancedSpawner spawner, int page = 0) : base(50, 50)
        {
            _spawner = spawner;
            _page = page;

            Closable = true;
            Disposable = true;
            Resizable = false;

            AddPage(0);

            // Increased the height from 420 to 450 to make room for our new stats
            AddBackground(0, 0, 380, 450, 5054);
            AddAlphaRegion(10, 10, 360, 430);

            // --- HEADER ---
            AddHtml(10, 15, 360, 20, "<CENTER><BASEFONT COLOR=#FFD700>Advanced Spawner</BASEFONT></CENTER>", false, false);

            string status = _spawner.Running ? "<BASEFONT COLOR=#00FF00>Running</BASEFONT>" : "<BASEFONT COLOR=#FF0000>Stopped</BASEFONT>";
            string radar = _spawner.IsPlayerPresent ? "<BASEFONT COLOR=#00FF00>Player Detected</BASEFONT>" : "<BASEFONT COLOR=#808080>Sleeping (Empty)</BASEFONT>";

            AddHtml(20, 40, 150, 20, $"Status: {status}", false, false);
            AddHtml(180, 40, 180, 20, $"Radar: {radar}", false, false);

            // --- NEW: TOTAL ACTIVE TRACKER ---
            int totalActive = 0;
            foreach (var e in _spawner.Entries)
            {
                totalActive += e.CurrentSpawnCount; // Pulls from the spawner's built-in tracking
            }

            string countColor = totalActive >= _spawner.GlobalMaxCount ? "#FF0000" : "#F4F4F4";
            AddHtml(20, 65, 200, 20, $"<BASEFONT COLOR=#FFD700>Total Spawned:</BASEFONT> <BASEFONT COLOR={countColor}>{totalActive} / {_spawner.GlobalMaxCount}</BASEFONT>", false, false);

            // --- PAGINATION CONTROLS ---
            if (_page > 0)
            {
                AddButton(290, 65, 0x15E3, 0x15E7, 5, GumpButtonType.Reply, 0); // Previous Page
            }

            // Show 'Next' if they have enough entries to fill the current page
            if (_spawner.Entries.Count >= (_page + 1) * 10)
            {
                AddButton(315, 65, 0x15E1, 0x15E5, 6, GumpButtonType.Reply, 0); // Next Page
            }

            // --- COLUMN LABELS ---
            AddLabel(20, 95, 1152, "Mob Name");
            AddLabel(200, 95, 1152, "Max");
            AddLabel(250, 95, 1152, "Prob");
            AddLabel(305, 95, 1152, "Active");

            // --- SPAWN ENTRIES (10 Rows Per Page) ---
            int y = 120;
            for (int i = 0; i < 10; i++)
            {
                // Calculate the actual entry index based on the current page
                int entryIndex = (_page * 10) + i;

                string mobName = "";
                string maxCount = "1";
                string prob = "100";
                string active = "0";

                if (entryIndex < _spawner.Entries.Count)
                {
                    AdvancedSpawnerEntry entry = _spawner.Entries[entryIndex];
                    mobName = entry.MobName;
                    maxCount = entry.MaxCount.ToString();
                    prob = entry.Probability.ToString();
                    active = entry.CurrentSpawnCount.ToString();
                }

                // Text Backgrounds
                AddImageTiled(18, y - 2, 164, 22, 0xA40);
                AddImageTiled(198, y - 2, 44, 22, 0xA40);
                AddImageTiled(248, y - 2, 44, 22, 0xA40);

                // Text Entries
                AddTextEntry(20, y, 160, 20, 1152, i * 3, mobName);
                AddTextEntry(200, y, 40, 20, 1152, (i * 3) + 1, maxCount);
                AddTextEntry(250, y, 40, 20, 1152, (i * 3) + 2, prob);

                // Active Count Label
                AddLabel(315, y, 2100, active);

                y += 25;
            }

            // --- CONTROLS ---
            // Shifted buttons down slightly to accommodate the larger gump
            AddButton(20, 390, 0xFB7, 0xFB9, 1, GumpButtonType.Reply, 0);
            AddLabel(55, 390, 1152, "Apply & Save");

            AddButton(20, 415, 0xFA8, 0xFAA, 2, GumpButtonType.Reply, 0);
            AddLabel(55, 415, 1152, "Global Properties");

            AddButton(160, 390, 0xFA2, 0xFA4, 3, GumpButtonType.Reply, 0);
            AddLabel(195, 390, 1152, "Clear Mobs");

            AddButton(160, 415, _spawner.Running ? 0x2A62 : 0x2A4E, _spawner.Running ? 0x2A62 : 0x2A4E, 4, GumpButtonType.Reply, 0);
            AddLabel(195, 415, 1152, _spawner.Running ? "Turn Off" : "Turn On");
        }

        public override void OnResponse(NetState sender, in RelayInfo info)
        {
            Mobile from = sender.Mobile;

            if (_spawner == null || _spawner.Deleted) return;

            int buttonId = info.ButtonID;

            if (buttonId == 1 || buttonId == 4 || buttonId == 5 || buttonId == 6)
            {
                SaveEntries(info);
            }

            switch (buttonId)
            {
                case 0: // Right Click / Close
                    break;

                case 1: // Apply & Save
                    from.SendMessage(68, "Spawner updated successfully.");
                    if (_spawner.IsPlayerPresent)
                    {
                        _spawner.ForceBurstSpawn();
                    }
                    from.SendGump(new AdvancedSpawnerGump(_spawner, _page)); // Maintain current page[cite: 7]
                    break;

                case 2: // Global Properties
                    from.SendGump(new AdvancedSpawnerGump(_spawner, _page));
                    from.SendGump(new PropertiesGump(from, _spawner));
                    break;

                case 3: // Clear Active Mobs
                    from.SendMessage(38, "Clearing all active mobs from this spawner.");
                    ClearAllMobs();
                    from.SendGump(new AdvancedSpawnerGump(_spawner, _page));
                    break;

                case 4: // Toggle On/Off
                    _spawner.Running = !_spawner.Running;
                    from.SendMessage(68, _spawner.Running ? "Spawner is now ON." : "Spawner is now OFF.");
                    if (!_spawner.Running) ClearAllMobs();
                    from.SendGump(new AdvancedSpawnerGump(_spawner, _page));
                    break;

                case 5: // Previous Page
                    if (_page > 0) _page--;
                    from.SendGump(new AdvancedSpawnerGump(_spawner, _page));
                    break;

                case 6: // Next Page
                    if (_spawner.Entries.Count >= (_page + 1) * 10) _page++;
                    from.SendGump(new AdvancedSpawnerGump(_spawner, _page));
                    break;
            }
        }

        private void SaveEntries(RelayInfo info)
        {
            // Create a completely new list, inserting items in the correct page order
            List<AdvancedSpawnerEntry> updatedList = new List<AdvancedSpawnerEntry>();

            // 1. Preserve all items BEFORE the current page
            for (int i = 0; i < _page * 10 && i < _spawner.Entries.Count; i++)
            {
                updatedList.Add(_spawner.Entries[i]);
            }

            // 2. Add/Update the items FROM the current page based on the Gump's text boxes
            for (int i = 0; i < 10; i++)
            {
                int originalIndex = (_page * 10) + i;

                string nameText = info.GetTextEntry(i * 3);
                string maxText = info.GetTextEntry((i * 3) + 1);
                string probText = info.GetTextEntry((i * 3) + 2);

                if (!string.IsNullOrWhiteSpace(nameText))
                {
                    string mobName = nameText.Trim();
                    int max = 1;
                    int prob = 100;

                    if (!string.IsNullOrWhiteSpace(maxText) && int.TryParse(maxText, out int parsedMax)) max = parsedMax;
                    if (!string.IsNullOrWhiteSpace(probText) && int.TryParse(probText, out int parsedProb)) prob = parsedProb;

                    if (originalIndex < _spawner.Entries.Count)
                    {
                        AdvancedSpawnerEntry existing = _spawner.Entries[originalIndex];

                        // If the name hasn't changed, keep the tracked mobs so they don't despawn!
                        if (existing.MobName.Equals(mobName, StringComparison.OrdinalIgnoreCase))
                        {
                            existing.MaxCount = max;
                            existing.Probability = prob;
                            updatedList.Add(existing);
                        }
                        else
                        {
                            // If they typed a completely different mob name, wipe the old active mobs
                            foreach (var m in existing.TrackedMobs) m?.Delete();
                            updatedList.Add(new AdvancedSpawnerEntry { MobName = mobName, MaxCount = max, Probability = prob });
                        }
                    }
                    else
                    {
                        // Brand new entry on an empty row
                        updatedList.Add(new AdvancedSpawnerEntry { MobName = mobName, MaxCount = max, Probability = prob });
                    }
                }
                else
                {
                    // If a text box was intentionally cleared out by the GM, delete the active mobs associated with it
                    if (originalIndex < _spawner.Entries.Count)
                    {
                        AdvancedSpawnerEntry existing = _spawner.Entries[originalIndex];
                        foreach (var m in existing.TrackedMobs) m?.Delete();
                    }
                }
            }

            // 3. Preserve all items AFTER the current page
            for (int i = (_page * 10) + 10; i < _spawner.Entries.Count; i++)
            {
                updatedList.Add(_spawner.Entries[i]);
            }

            _spawner.Entries = updatedList; // Push the perfectly ordered list back to the spawner[cite: 7]
        }

        private void ClearAllMobs()
        {
            foreach (var entry in _spawner.Entries)
            {
                for (int i = entry.TrackedMobs.Count - 1; i >= 0; i--)
                {
                    Mobile m = entry.TrackedMobs[i];
                    if (m != null && !m.Deleted)
                    {
                        m.Delete();
                    }
                }
                entry.TrackedMobs.Clear();
            }
        }
    }
}
