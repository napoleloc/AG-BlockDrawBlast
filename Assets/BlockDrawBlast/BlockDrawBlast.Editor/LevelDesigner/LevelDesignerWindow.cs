using UnityEditor;
using UnityEngine;

namespace BlockDrawBlast.Editor.LevelDesigner.Views
{
    public class LevelDesignerWindow : EditorWindow
    {
        private LevelDesignerView _view;
        
        [MenuItem("Tools/Level Designer")]
        public static void OpenWindow()
        {
            var window = GetWindow<LevelDesignerWindow>();
            window.titleContent = new GUIContent("Level Designer");
            window.Show();
        }
        
        private void CreateGUI()
        {
            _view = LevelDesignerAPI.CreateView(rootVisualElement);
        }

        private void OnDestroy()
        {
            _view.Dispose();
        }
    }
}
