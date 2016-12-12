using System;
using System.Collections.Generic;

namespace org.activiti.engine.@delegate
{


    using VariableInstance = org.activiti.engine.impl.persistence.entity.VariableInstance;

    public interface VariableScope
    {

        IDictionary<string, object> Variables { get; set; }

        IDictionary<string, VariableInstance> VariableInstances { get; }

        IDictionary<string, object> getVariables(ICollection<string> variableNames);

        IDictionary<string, VariableInstance> getVariableInstances(ICollection<string> variableNames);

        IDictionary<string, object> getVariables(ICollection<string> variableNames, bool fetchAllVariables);

        IDictionary<string, VariableInstance> getVariableInstances(ICollection<string> variableNames, bool fetchAllVariables);

        IDictionary<string, object> VariablesLocal { get; set; }

        IDictionary<string, VariableInstance> VariableInstancesLocal { get; }

        IDictionary<string, object> getVariablesLocal(ICollection<string> variableNames);

        IDictionary<string, VariableInstance> getVariableInstancesLocal(ICollection<string> variableNames);

        IDictionary<string, object> getVariablesLocal(ICollection<string> variableNames, bool fetchAllVariables);

        IDictionary<string, VariableInstance> getVariableInstancesLocal(ICollection<string> variableNames, bool fetchAllVariables);

        object getVariable(string variableName);

        VariableInstance getVariableInstance(string variableName);

        object getVariable(string variableName, bool fetchAllVariables);

        VariableInstance getVariableInstance(string variableName, bool fetchAllVariables);

        object getVariableLocal(string variableName);

        VariableInstance getVariableInstanceLocal(string variableName);

        object getVariableLocal(string variableName, bool fetchAllVariables);

        VariableInstance getVariableInstanceLocal(string variableName, bool fetchAllVariables);

        T getVariable<T>(string variableName, Type variableClass);

        T getVariableLocal<T>(string variableName, Type variableClass);

        List<string> VariableNames { get; }

        List<string> VariableNamesLocal { get; }

        /// <summary>
        /// Sets the variable with the provided name to the provided value.
        /// 
        /// <para>
        /// A variable is set according to the following algorithm:
        /// 
        /// </para>
        /// <para>
        /// <li>If this scope already contains a variable by the provided name as a
        /// <strong>local</strong> variable, its value is overwritten to the provided
        /// value.</li>
        /// <li>If this scope does <strong>not</strong> contain a variable by the
        /// provided name as a local variable, the variable is set to this scope's
        /// parent scope, if there is one. If there is no parent scope (meaning this
        /// scope is the root scope of the hierarchy it belongs to), this scope is
        /// used. This applies recursively up the parent scope chain until, if no scope
        /// contains a local variable by the provided name, ultimately the root scope
        /// is reached and the variable value is set on that scope.</li>
        /// </para>
        /// <para>
        /// In practice for most cases, this algorithm will set variables to the scope
        /// of the execution at the process instance’s root level, if there is no
        /// execution-local variable by the provided name.
        /// 
        /// </para>
        /// </summary>
        /// <param name="variableName">
        ///          the name of the variable to be set </param>
        /// <param name="value">
        ///          the value of the variable to be set </param>
        void setVariable(string variableName, object value);

        void setVariable(string variableName, object value, bool fetchAllVariables);

        object setVariableLocal(string variableName, object value);

        object setVariableLocal(string variableName, object value, bool fetchAllVariables);



        bool hasVariables();

        bool hasVariablesLocal();

        bool hasVariable(string variableName);

        bool hasVariableLocal(string variableName);

        void createVariableLocal(string variableName, object value);

        /// <summary>
        /// Removes the variable and creates a new <seealso cref="HistoricVariableUpdateEntity"/>
        /// .
        /// </summary>
        void removeVariable(string variableName);

        /// <summary>
        /// Removes the local variable and creates a new
        /// <seealso cref="HistoricVariableUpdateEntity"/>.
        /// </summary>
        void removeVariableLocal(string variableName);

        /// <summary>
        /// Removes the variables and creates a new
        /// <seealso cref="HistoricVariableUpdateEntity"/> for each of them.
        /// </summary>
        void removeVariables(ICollection<string> variableNames);

        /// <summary>
        /// Removes the local variables and creates a new
        /// <seealso cref="HistoricVariableUpdateEntity"/> for each of them.
        /// </summary>
        void removeVariablesLocal(ICollection<string> variableNames);

        /// <summary>
        /// Removes the (local) variables and creates a new
        /// <seealso cref="HistoricVariableUpdateEntity"/> for each of them.
        /// </summary>
        void removeVariables();

        /// <summary>
        /// Removes the (local) variables and creates a new
        /// <seealso cref="HistoricVariableUpdateEntity"/> for each of them.
        /// </summary>
        void removeVariablesLocal();

    }
}