using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Core;

namespace RemoteKeycard
{
    public class RemoteKeycard
    {
        
        public static RemoteKeycard Instance { get; private set; }

        [PluginConfig("config.yml")] public Config Config;

        [PluginPriority(LoadPriority.Highest)]
        [PluginEntryPoint("RemoteKeycard", "1.0.0", "Remote Keycard", "sssssssthedev")]
        public void LoadPlugin()
        {
            Instance = this;
            Log.Info("(1/2) Registering events...");
            PluginAPI.Events.EventManager.RegisterEvents<EventHandlers>(this);
            Log.Info("(2/3) Registering config...");
            var handler = PluginHandler.Get(this);
            handler.SaveConfig(this, nameof(Config));
        }
    }
}