using System;
using UnityEngine.UIElements;

namespace BlockDrawBlast.Editor.LevelDesigner.Views;

public class LevelBlockTableView : VisualElement
{
    public static readonly string UssClassName = "level-block-table";
    public static readonly string HeaderUssClassName = $"{UssClassName}__header";
    public static readonly string HeaderLabelUssClassName = $"{HeaderUssClassName}__label";
    
    public static readonly string ButtonLoadUssClassName = $"{UssClassName}__button-load";
    public static readonly string ButtonDuplicateUssClassName = $"{UssClassName}__button-duplicate";
    public static readonly string ButtonDeleteUssClassName = $"{UssClassName}__button-delete";
    
    private Action<int> OnButtonLoadClickAction;
    private Action<int> OnButtonDuplicateClickAction;
    private Action<int> OnButtonDeleteClickAction;
    
    public LevelBlockTableView()
    {
        AddToClassList(UssClassName);
        
        var header = new VisualElement();
        header.AddToClassList(HeaderUssClassName);
        Add(header);
        
        var label = new Label();
        label.AddToClassList(HeaderLabelUssClassName);
        header.Add(label);
        
        var buttonLoad = new Button();
        buttonLoad.AddToClassList(ButtonLoadUssClassName);
        header.Add(buttonLoad);
        
        var buttonDuplicate = new Button();
        buttonDuplicate.AddToClassList(ButtonDuplicateUssClassName);
        header.Add(buttonDuplicate);
        
        var buttonDelete = new Button();
        buttonDelete.AddToClassList(ButtonDeleteUssClassName);
    }

    private void OnButtonLoadClick()
    {
        
    }

    private void OnButtonDuplicateClick()
    {
        
    }
    
    private void OnButtonDeleteClick()
    {
        
    }
}