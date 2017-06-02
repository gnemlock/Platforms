/* Created by Matthew Francis Keating */

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Platforms
{
    using StringFormats = Utility.PlatformsStringFormats;
    using Labels = Utility.PlatformsLabels;
    using Log = Utility.PlatformsDebug;
    using Tags = Utility.PlatformsTags;

    #if UNITY_EDITOR
    using Tooltips = Utility.LedgeTooltips;
    using Colours = Utility.LedgeColours;
    using Dimensions = Utility.LedgeDimensions;
    #endif

    public class Ledge : MonoBehaviour 
    {
    }
}

namespace Platforms.Utility
{
    [CustomEditor(typeof(Ledge))] public class LedgeEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            Ledge ledge = target as Ledge;
        }
    }

    #if UNITY_EDITOR
    // Strings used to generate tooltips for the editor.
    public static class LedgeTooltips
    {
    }

    // Colours for use in displaying custom editor GUI.
    public static class LedgeColours
    {
    }

    // Dimensions for use in displaying custom editor GUI.
    public static class LedgeDimensions
    {
    }
    #endif

    // Strings containing format for string interface display.
    public static partial class PlatformsStringFormats
    {
    }
    
    // Strings used for general labelling.
    public static partial class PlatformsLabels
    {
    }
    
    // Provides debug functionality, including methods and customised string messages.
    public static partial class PlatformsDebug
    {
    }
    
    // Strings used for tag or name comparison
    public static partial class PlatformsTags
    {
    }
}