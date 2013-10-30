using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
namespace HexaClassicClient
{
    public static class PluginManager
    {
        public static void Init()
        {
            // Load plugins from this assembly first.
            foreach (Type t in Assembly.GetEntryAssembly().GetTypes())
            {
                if (typeof(IListener).IsAssignableFrom(t))
                {
                    //dynamic plugin = Activator.CreateInstance(t);
                    //plugin.Init();
                    if (!t.Equals(typeof(IListener)))
                    {
                        IListener plugin = (IListener)Activator.CreateInstance(t);
                        plugin.Init();
                    }
                }
            }
        }
    }
}
