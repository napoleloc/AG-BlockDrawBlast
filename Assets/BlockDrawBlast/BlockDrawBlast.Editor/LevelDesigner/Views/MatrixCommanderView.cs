using System;
using UnityEngine.UIElements;

namespace BlockDrawBlast.Editor.LevelDesigner.Views;

public class MatrixCommanderView : VisualElement, IDisposable
{
    public static readonly string UssClassName = "matrix-commander-view";
    public static readonly string HeaderUssClassName = $"{UssClassName}__header";
    public static readonly string HeaderLabelUssClassName = $"{HeaderUssClassName}__label";
    public static readonly string ContentUssClassName = $"{UssClassName}__content";

    private readonly Label _label;
    private readonly VisualElement _container;
    
    public MatrixCommanderView(VisualElement root)
    {
        AddToClassList(UssClassName);
        
        var header = new VisualElement();
        header.AddToClassList(HeaderUssClassName);
        Add(header);
        
        var label = _label = new Label();
        label.AddToClassList(HeaderLabelUssClassName);
        header.Add(label);
        
        _container = new VisualElement();
        _container.AddToClassList(ContentUssClassName);
        Add(_container);
        
        
        root.Add(this);
    }

    public void SetLabel(string value)
    {
        _label.text = value;
    }
    
    public void Dispose()
    {
        
    }
}