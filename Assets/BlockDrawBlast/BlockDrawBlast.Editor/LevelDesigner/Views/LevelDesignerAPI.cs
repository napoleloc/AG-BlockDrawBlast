using System.Diagnostics.CodeAnalysis;
using UnityEngine.UIElements;

namespace BlockDrawBlast.Editor.LevelDesigner.Views
{
    public static class LevelDesignerAPI
    {
        public static LevelDesignerView CreateView([NotNull] VisualElement root)
        {
            return new LevelDesignerView(root);
        }
    }
}

