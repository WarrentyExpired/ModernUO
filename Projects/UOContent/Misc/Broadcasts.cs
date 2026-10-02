namespace Server.Misc
{
    public static class Broadcasts
    {
        public static void Initialize()
        {
            EventSink.ServerCrashed += EventSink_Crashed;
            EventSink.Shutdown += EventSink_Shutdown;
            EventSink.Disconnected += EventSink_Disconnected;
        }

        public static void EventSink_Crashed(ServerCrashedEventArgs e)
        {
            try
            {
                World.Broadcast(0x35, true, "The server has crashed.");
            }
            catch
            {
                // ignored
            }
        }

        public static void EventSink_Shutdown()
        {
            try
            {
                World.Broadcast(0x35, true, "The server has shut down.");
            }
            catch
            {
                // ignored
            }
        }

        public static void EventSink_Disconnected(Mobile m)
        {
            if ( Settings.S_SaveOnCharacterLogout )
            {
                World.Save();
            }
        }
    }
}
