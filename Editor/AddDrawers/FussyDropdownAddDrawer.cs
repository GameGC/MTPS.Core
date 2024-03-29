using System;
using MTPS.Core.Attributes;
using MTPS.Core.Editor.AddDrawers.Core;
using MTPS.Core.Editor.FuzzySearch;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace MTPS.Core.Editor.AddDrawers
{
    [AddHandlerFor(typeof(FuzzyAddButton))]
    public class FussyDropdownAddDrawer : ListDropdownAddDrawer
    {
        public override void AddDropdown(Rect buttonRect, ReorderableList list, IReferenceAddButton attribute)
        {
            if (Event.current == null) return;

            var property = list.serializedProperty.Copy();
            var listCopy = list;
        
            FuzzyWindow.Show(buttonRect,new Vector2(200, 100), attribute.BaseType, type =>
            {
                var dynamicType = new Tuple<SerializedProperty, Type, int, ReorderableList, string>(
                    property, type, property.arraySize, listCopy, property.propertyPath);
                OnAddItemFromDropdown(dynamicType);
            });
        }
    }
}