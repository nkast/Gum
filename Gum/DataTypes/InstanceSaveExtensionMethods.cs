﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gum.DataTypes.Variables;
using Gum.Managers;
using Gum.ToolStates;

namespace Gum.DataTypes
{
    public static class InstanceSaveExtensionMethods
    {
        public static void Initialize(this InstanceSave instanceSave)
        {
            // nothing to do currently?

        }

        public static bool IsComponent(this InstanceSave instanceSave)
        {
            ComponentSave baseAsComponentSave = ObjectFinder.Self.GetComponent(instanceSave.BaseType);

            return baseAsComponentSave != null;

        }

        public static VariableSave GetVariableFromThisOrBase(this InstanceSave instance, 
            ElementSave parentContainer, string variable, bool forceDefault = false)
        {
            ElementSave instanceBase = ObjectFinder.Self.GetElementSave(instance.BaseType);

            StateSave stateToPullFrom = parentContainer.DefaultState;
            StateSave defaultState = parentContainer.DefaultState;
            if (parentContainer == SelectedState.Self.SelectedElement && 
                SelectedState.Self.SelectedStateSave != null &&
                !forceDefault)
            {
                stateToPullFrom = SelectedState.Self.SelectedStateSave;
            }

            VariableSave variableSave = stateToPullFrom.GetVariableSave(instance.Name + "." + variable);
            // non-default states can override the default state, so first
            // let's see if the selected state is non-default and has a value
            // for a given variable.  If not, we'll fall back to the default.
            if (variableSave == null && defaultState != stateToPullFrom)
            {
                variableSave = defaultState.GetVariableSave(instance.Name + "." + variable);
            }
            if (variableSave == null)
            {
                // Eventually use the instanceBase's current state value
                variableSave = instanceBase.DefaultState.GetVariableSave(variable);
            }

            if (variableSave != null && variableSave.Value == null)
            {
                // This can happen if there is a tunneled variable that is null
                VariableSave possibleVariable = instanceBase.DefaultState.GetVariableSave(variable);
                if (possibleVariable != null && possibleVariable.Value != null)
                {
                    variableSave = possibleVariable;
                }
            }

            return variableSave;

        }

        public static VariableListSave GetVariableListFromThisOrBase(this InstanceSave instance, ElementSave parentContainer, string variable)
        {
            ElementSave instanceBase = ObjectFinder.Self.GetElementSave(instance.BaseType);

            VariableListSave variableListSave = parentContainer.DefaultState.GetVariableListSave(instance.Name + "." + variable);
            if (variableListSave == null)
            {
                variableListSave = instanceBase.DefaultState.GetVariableListSave(variable);
            }

            if (variableListSave != null && variableListSave.ValueAsIList == null)
            {
                // This can happen if there is a tunneled variable that is null
                VariableListSave possibleVariable = instanceBase.DefaultState.GetVariableListSave(variable);
                if (possibleVariable != null && possibleVariable.ValueAsIList != null)
                {
                    variableListSave = possibleVariable;
                }
            }

            return variableListSave;

        }

        public static object GetValueFromThisOrBase(this InstanceSave instance, ElementSave parentContainer, string variable,
            bool forceDefault = false)
        {
            VariableSave variableSave = instance.GetVariableFromThisOrBase(parentContainer, variable, forceDefault);


            if (variableSave != null)
            {
                return variableSave.Value;
            }
            else
            {
                VariableListSave variableListSave = parentContainer.DefaultState.GetVariableListSave(instance.Name + "." + variable);

                if (variableListSave == null)
                {
                    ElementSave instanceBase = ObjectFinder.Self.GetElementSave(instance.BaseType);

                    variableListSave = instanceBase.DefaultState.GetVariableListSave(variable);
                }

                if (variableListSave != null)
                {
                    return variableListSave.ValueAsIList;
                }
            }

            // If we get ehre that means there isn't any VariableSave or VariableListSave
            return null;

        }

    }


}