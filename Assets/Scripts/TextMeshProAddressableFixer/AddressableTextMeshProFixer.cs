using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Process for making TextMeshPro adjust to Addressables
/// </summary>
#if UNITY_EDITOR
[InitializeOnLoad]
#endif
public static class AddressableTextMeshProFixer
{
#if UNITY_EDITOR
    /// <summary>
    /// Constructor
    /// </summary>
    static AddressableTextMeshProFixer()
    {
        EditorApplication.playModeStateChanged += OnChangedPlayMode;
        FixedTextMeshProInEditorMode();
    }

    /// <summary>
    /// Callback form Editor's Play Mode State Changed
    /// </summary>
    /// <param name="state"></param>
    private static void OnChangedPlayMode(PlayModeStateChange state)
    {
        // Stop showing Import TMP Essenstial Resources Popup
        FixedTextMeshProInEditorMode();
    }
#endif

    /// <summary>
    /// Fix TMP_Settings on Runtime
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    static void FixTextMeshPro()
    {
        // Gets TMP_Settings class
        var settingsInstance = typeof(TMP_Settings).GetField("s_Instance", BindingFlags.NonPublic | BindingFlags.Static);
        if (settingsInstance == null) return;
        // TODO: Changes your own TMP Settings Address.
        var tmPro = Addressables.LoadAssetAsync<TMP_Settings>("Assets/TextMesh Pro/Addressables/TMP Settings.asset").WaitForCompletion();
        settingsInstance.SetValue(null, tmPro);

        // NOTE: When do Build, Default Font Asset will be Empty and need setting Font Asset to TMP_Setting on runtime
        // TODO: Changes your own Fonts Asset Address.
        var fontData = Addressables.LoadAssetAsync<TMP_FontAsset>("Assets/TextMesh Pro/Addressables/Fonts & Materials/LiberationSans SDF.asset").WaitForCompletion();
        Debug.Log($"find TMP_FontAsset ? {fontData != null}");
        var fontField = typeof(TMP_Settings).GetField("m_defaultFontAsset", BindingFlags.NonPublic | BindingFlags.Instance);
        fontField.SetValue(TMP_Settings.instance, fontData);
    }

#if UNITY_EDITOR
    /// <summary>
    /// Fix on Editor Mode
    /// </summary>
    public static void FixedTextMeshProInEditorMode()
    {
        var settingsInstance = typeof(TMP_Settings).GetField("s_Instance", BindingFlags.NonPublic | BindingFlags.Static);
        var settingsPath = AssetDatabase.FindAssets("t:TMP_Settings").Select(d => AssetDatabase.GUIDToAssetPath(d)).First(path => path.Contains("TMP Settings"));
        var settings = AssetDatabase.LoadAssetAtPath<TMP_Settings>(settingsPath);
        settingsInstance.SetValue(null, settings);
    }
#endif

#if UNITY_EDITOR
    /// <summary>
    /// Process for making TextMeshPro adjust to Addressables For Edit Mode
    /// </summary>
    class AddressableTextMeshProEditorFixer : AssetPostprocessor
    {
        public override int GetPostprocessOrder() => -10000000;

        /// <summary>
        /// This is called after importing of any number of assets is complete (when the Assets progress bar has reached the end).
        /// </summary>
        static void OnPostProcessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload)
        {
            AddressableTextMeshProFixer.FixedTextMeshProInEditorMode();
        }
    }
#endif
}
