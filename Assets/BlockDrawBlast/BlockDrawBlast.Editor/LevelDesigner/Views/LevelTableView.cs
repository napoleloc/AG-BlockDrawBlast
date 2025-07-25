using UnityEngine.UIElements;

namespace BlockDrawBlast.Editor.LevelDesigner.Views
{
    public class LevelTableView : VisualElement
    {
        public static readonly string UssClassName = "level-table";
        public static readonly string HeaderUssClassName = $"{UssClassName}__header";
        public static readonly string HeaderLabelUssClassName = $"{HeaderUssClassName}__label";
        
        private readonly Label _label;
        
        public LevelTableView()
        {
            AddToClassList(UssClassName);
            
            var header = new VisualElement();
            header.AddToClassList(HeaderUssClassName);
            Add(header);
            
            var label = _label = new Label();
            label.AddToClassList(HeaderLabelUssClassName);
            header.Add(label);
        }
        
    }
}

