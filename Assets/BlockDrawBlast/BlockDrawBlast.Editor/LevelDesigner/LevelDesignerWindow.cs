using EncosyTower.Editor.UIElements;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace BlockDrawBlast.Editor.LevelDesigner.Views
{
    public class LevelDesignerWindow : EditorWindow
    {
        [SerializeField] internal ThemeStyleSheet _themeStyleSheet;
        
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
            var root = rootVisualElement;

            if (Application.isPlaying == false)
            {
                root.styleSheets.Add(_themeStyleSheet);
            }
            else
            {
                root.ApplyEditorStyleSheet(Constants.THEME_STYLE_SHEET);
            }
            
            _view = LevelDesignerAPI.CreateView(root);
        }

        private void OnDestroy()
        {
            _view.Dispose();
        }
    }
}
