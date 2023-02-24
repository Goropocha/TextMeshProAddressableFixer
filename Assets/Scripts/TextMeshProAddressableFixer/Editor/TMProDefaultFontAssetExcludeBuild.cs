using System.Linq;
using System.Reflection;
using TMPro;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

/// <summary>
/// Process to remove Default Font Asset in TextMesh Pro at build
/// </summary>
public class TMProDefaultFontAssetExcludeBuild : IPreprocessBuildWithReport, IPostprocessBuildWithReport
{
    public int callbackOrder => 0;

    private FieldInfo _defaultFontAssetFieldInfo = typeof(TMPro.TMP_Settings).GetField("m_defaultFontAsset", BindingFlags.NonPublic | BindingFlags.Instance);
    private object _defaultFontAsset = null;

    /// <summary>
    /// OnPreprocessBuild
    /// </summary>
    /// <param name="report"></param>
    public void OnPreprocessBuild(BuildReport report)
    {
        var settings = AssetDatabase.LoadAssetAtPath<TMP_Settings>(GetTMPSettingsPath());
        if (settings == null) return;
        // Get Default Font Asset from TMP_Settings
        _defaultFontAsset = _defaultFontAssetFieldInfo.GetValue(settings);
        if (_defaultFontAsset == null) return;

        _defaultFontAssetFieldInfo.SetValue(settings, null);
        EditorUtility.SetDirty(settings);
    }

    /// <summary>
    /// OnPostprocessBuild
    /// </summary>
    /// <param name="report"></param>
    public void OnPostprocessBuild(BuildReport report)
    {
        var settings = AssetDatabase.LoadAssetAtPath<TMP_Settings>(GetTMPSettingsPath());
        if (settings == null || _defaultFontAsset == null) return;

        // NOTE: Revert asset to avoid problems in development
        _defaultFontAssetFieldInfo.SetValue(settings, _defaultFontAsset);
        _defaultFontAsset = null;
        EditorUtility.SetDirty(settings);
        AssetDatabase.SaveAssets();
    }

    /// <summary>
    /// Get Asset Path of TMP_Settings
    /// </summary>
    /// <returns></returns>
    private string GetTMPSettingsPath()
        => AssetDatabase.FindAssets("t:TMP_Settings").Select(d => AssetDatabase.GUIDToAssetPath(d)).FirstOrDefault(path => path.Contains("TMP Settings"));
}
