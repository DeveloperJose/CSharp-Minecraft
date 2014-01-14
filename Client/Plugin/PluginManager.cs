using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
namespace Client
{
    public static class PluginManager
    {
        public static List<IPlugin> LoadedPlugins { get; private set; }
        public static void Init()
        {
            LoadedPlugins = new List<IPlugin>();
            // Load plugins from this assembly first.
            foreach (Type t in Assembly.GetEntryAssembly().GetTypes())
            {
                if (typeof(IPlugin).IsAssignableFrom(t))
                {
                    if (!t.Equals(typeof(IPlugin)))
                    {
                        IPlugin plugin = (IPlugin)Activator.CreateInstance(t);
                        plugin.Init();
                        if (!LoadedPlugins.Contains(plugin))
                        {
                            LoadedPlugins.Add(plugin);
                        }
                    }
                }
            }
        }
        public static void Stop()
        {
            foreach (IPlugin plugin in LoadedPlugins)
            {
                plugin.Stop();
            }
            LoadedPlugins.Clear();
            Init();
        }
        public static void Reload()
        {
            Stop();
            Init();
        }
    }
}
