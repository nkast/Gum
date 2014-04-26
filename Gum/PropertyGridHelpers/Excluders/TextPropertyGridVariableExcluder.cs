﻿using Gum.Plugins.BaseClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gum.PropertyGridHelpers.Excluders
{
    [Export(typeof(PluginBase))]
    public class TextPropertyGridVariableExcluder : InternalPlugin
    {
        public override void StartUp()
        {
            this.VariableExcluded += HandleVariableExcluded;
        }

        private bool HandleVariableExcluded(DataTypes.Variables.VariableSave variable, DataTypes.RecursiveVariableFinder rvf)
        {
            string rootName = variable.GetRootName();

            bool shouldExclude = false;

            if(rootName == "Font" || rootName == "FontSize" || rootName == "OutlineThickness")
            {
                bool useCustomFont = rvf.GetValue<bool>("UseCustomFont");

                shouldExclude = useCustomFont;
            }
            else if (rootName == "CustomFontFile")
            {
                bool useCustomFont = rvf.GetValue<bool>("UseCustomFont");

                shouldExclude = useCustomFont == false;
            }

            return shouldExclude;
        }

        public override string FriendlyName
        {
            get { return "Text Variable Excluder"; }
        }

        public override Version Version
        {
            get { return new Version(); }
        }

        public override bool ShutDown(Plugins.PluginShutDownReason shutDownReason)
        {
            return true;
        }
    }
}
