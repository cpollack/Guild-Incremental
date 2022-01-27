#if UNITY_EDITOR

using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEngine;
using UnityEditor;

public class SaveFileEditorWindow : OdinEditorWindow
{
    
    public GameData gameData = null;

    [MenuItem("Tools/Save File Editor")]
    private static void Open()
    {
        var window = GetWindow<SaveFileEditorWindow>();
        window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 500);        
    }

    private void Awake()
    {

    }

    private void CreateGUI()
    {
        gameData = DataAccessor.Load();
    }

    protected override void OnBeginDrawEditors()
    {
        SirenixEditorGUI.BeginHorizontalToolbar();
        {
            GUILayout.Label("Save File Editor");

            if (SirenixEditorGUI.ToolbarButton(new GUIContent("Save")))
            {
                DataAccessor.Save(gameData);
            }
        }
        SirenixEditorGUI.EndHorizontalToolbar();

    }
}

#endif
