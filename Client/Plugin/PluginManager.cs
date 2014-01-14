﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
namespace Client
{
    public static class PluginManager
    {
        public static List<string> LoadedPlugins { get; private set; }
        public static void Init()
        {
            LoadedPlugins = new List<string>();
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
                        LoadedPlugins.Add(t.Name);
                    }
                }
            }
        }
    }
}
