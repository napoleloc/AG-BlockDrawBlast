using System;
using UnityEngine.UIElements;

namespace BlockDrawBlast.Editor.LevelDesigner.Views
{
    public class LevelDesignerView : VisualElement, IDisposable
    {
        public static readonly string UssClassName = "level-designer-view";
        
        public LevelDesignerView(VisualElement root)
        {
            AddToClassList(UssClassName);
            
            root.Add(this);
        }
        
        public void Dispose()
        {
            
        }
    }
}

