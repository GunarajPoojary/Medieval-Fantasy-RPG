using System;
using System.Diagnostics;
using UnityEngine;

namespace Plugins.Neonalig.SceneToolbar.Attributes {
    /// <summary> Decorates an enum field as a toolbar picker. </summary>
    [AttributeUsage(AttributeTargets.Field)]
    internal sealed class EnumToolbarAttribute : PropertyAttribute { }
}