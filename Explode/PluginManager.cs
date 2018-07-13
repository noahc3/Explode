﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ExplodePluginBase;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
    This plugin system is based on code written by Christoph Gattnar, 
    licensed under the Apache License, Version 2.0 (the "License");
    you may not use this file except in compliance with the License.
    You may obtain a copy of the License at

	   http://www.apache.org/licenses/LICENSE-2.0

    Unless required by applicable law or agreed to in writing, software
    distributed under the License is distributed on an "AS IS" BASIS,
    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    See the License for the specific language governing permissions and
    limitations under the License.
 */

namespace Explode
{
    public class PluginManager
    {
        // contains instances of plugins
        private List<IPluginBase> _plugins = new List<IPluginBase>();

        public PluginManager(string directory)
        {
            // plugin file names in here
            string[] pluginFileNames = null;
            if (Directory.Exists(directory))
            {
               // kind of speaks for itself here
               pluginFileNames = Directory.GetFiles(directory, "*.dll");
            }

            // contains all the assemblies
            ICollection<Assembly> assemblies = new List<Assembly>(pluginFileNames.Length);
            foreach (string dllFile in pluginFileNames)
            {
                // loads all of the assemblies
                AssemblyName an = AssemblyName.GetAssemblyName(dllFile);
                Assembly assembly = Assembly.Load(an);
                assemblies.Add(assembly);
            }
           
            Type pluginType = typeof(IPluginBase);
            // contains Type objects of all the plugins
            ICollection<Type> pluginTypes = new List<Type>();
            foreach (Assembly assembly in assemblies)
            {
                if (assembly != null)
                {
                    Type[] types = assembly.GetTypes();
                    foreach (Type type in types)
                    {
                        if (type.IsInterface || type.IsAbstract)
                        {
                            continue;
                        }
                        else
                        {
                            if (type.GetInterface(pluginType.FullName) != null)
                            {
                                pluginTypes.Add(type);
                            }
                        }
                    }
                }
            }
            // creates an instance of each type
            foreach (Type type in pluginTypes)
            {
                IPluginBase plugin = (IPluginBase)Activator.CreateInstance(type);
                _plugins.Add(plugin);
            }
        }

        public List<IPluginBase> Plugins
        {
            get { return _plugins; }
        }
    }
}