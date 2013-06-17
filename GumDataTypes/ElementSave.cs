﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gum.DataTypes.Variables;
using System.Xml.Serialization;
using ToolsUtilities;

namespace Gum.DataTypes
{

    public abstract class ElementSave
    {
        #region Fields

        public string Name
        {
            get;
            set;
        }

        public string BaseType
        {
            get;
            set;
        }

        [XmlElement("State")]
        public List<StateSave> States
        {
            get;
            set;
        }

        [XmlElement("Instance")]
        public List<InstanceSave> Instances
        {
            get;
            set;
        }

        public abstract string Subfolder
        {
            get;
        }

        // If adding anything here, 

        #endregion

        #region Properties

        public abstract string FileExtension
        {
            get;
        }

        [XmlIgnore]
        public StateSave DefaultState
        {
            get
            {
                if (States == null)
                {
                    return null;
                }
                else
                {
                    // This may change if the user can redefine the default state as Justin asked.
                    return States[0];
                }
            }
        }

        #endregion

        public ElementSave()
        {
            States = new List<StateSave>();
            Instances = new List<InstanceSave>();
        }

        public InstanceSave GetInstance(string name)
        {
            foreach (InstanceSave instance in this.Instances)
            {
                if (instance.Name == name)
                {
                    return instance;
                }
            }
            return null;
        }

        public void Save(string fileName)
        {
            FileManager.XmlSerialize(this.GetType(), this, fileName);
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(BaseType))
            {
                return Name;
            }
            else
            {
                return BaseType + " " + Name;
            }
        }
    }
}