using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEngine.UI;

public class FontChangerWindow : EditorWindow
{
    Font uiFont;
    TMP_FontAsset tmpFont;

    [MenuItem("Tools/Font Changer")]
    static void Open() => GetWindow<FontChangerWindow>("Font Changer");

    void OnGUI()
    {
        uiFont = (Font)EditorGUILayout.ObjectField("UI Font", uiFont, typeof(Font), false);
        tmpFont = (TMP_FontAsset)EditorGUILayout.ObjectField("TMP Font", tmpFont, typeof(TMP_FontAsset), false);

        GUI.enabled = uiFont || tmpFont;

        if (GUILayout.Button("Change Fonts In Scene"))
        {
            foreach (var t in FindObjectsByType<Text>(FindObjectsSortMode.None))
            {
                if (uiFont)
                {
                    Undo.RecordObject(t, "Change UI Font");
                    t.font = uiFont;
                    EditorUtility.SetDirty(t);
                }
            }

            foreach (var t in FindObjectsByType<TextMeshProUGUI>(FindObjectsSortMode.None))
            {
                if (tmpFont)
                {
                    Undo.RecordObject(t, "Change TMP Font");
                    t.font = tmpFont;
                    EditorUtility.SetDirty(t);
                }
            }

            AssetDatabase.SaveAssets();
        }

        GUI.enabled = true;
    }
}