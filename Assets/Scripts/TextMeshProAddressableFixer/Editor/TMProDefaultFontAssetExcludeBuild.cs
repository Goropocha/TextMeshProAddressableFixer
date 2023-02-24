using System.Linq;
using System.Reflection;
using TMPro;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

/// <summary>
/// TextMesh ProのDefault Font Assetをビルド時に除く処理
/// NOTE: Default Font Asset を空にしないと Resources としてビルドインされてしまう対策
/// </summary>
public class TMProDefaultFontAssetExcludeBuild : IPreprocessBuildWithReport, IPostprocessBuildWithReport
{
    public int callbackOrder => 0;

    private FieldInfo _defaultFontAssetFieldInfo = typeof(TMPro.TMP_Settings).GetField("m_defaultFontAsset", BindingFlags.NonPublic | BindingFlags.Instance);
    private object _defaultFontAsset = null;

    /// <summary>
    /// ビルド前の処理
    /// </summary>
    /// <param name="report"></param>
    public void OnPreprocessBuild(BuildReport report)
    {
        var settings = AssetDatabase.LoadAssetAtPath<TMP_Settings>(GetTMPSettingsPath());
        if (settings == null) return;
        // TMP_Settings の Default Font Asset を取得する
        _defaultFontAsset = _defaultFontAssetFieldInfo.GetValue(settings);
        if (_defaultFontAsset == null) return;

        _defaultFontAssetFieldInfo.SetValue(settings, null);
        EditorUtility.SetDirty(settings);
    }

    /// <summary>
    /// ビルド後の処理
    /// </summary>
    /// <param name="report"></param>
    public void OnPostprocessBuild(BuildReport report)
    {
        var settings = AssetDatabase.LoadAssetAtPath<TMP_Settings>(GetTMPSettingsPath());
        if (settings == null || _defaultFontAsset == null) return;

        // NOTE: 開発で問題が発生しないように戻す
        _defaultFontAssetFieldInfo.SetValue(settings, _defaultFontAsset);
        _defaultFontAsset = null;
        EditorUtility.SetDirty(settings);
        AssetDatabase.SaveAssets();
    }

    /// <summary>
    /// TMP_Settingsのアセットパスを取得する
    /// </summary>
    /// <returns>TMP_Settingsのアセットパス</returns>
    private string GetTMPSettingsPath()
        => AssetDatabase.FindAssets("t:TMP_Settings").Select(d => AssetDatabase.GUIDToAssetPath(d)).FirstOrDefault(path => path.Contains("TMP Settings"));
}
