using System;
using UnityEngine;

namespace MTPS.Core.Editor.FuzzySearch
{
    public class FuzzyListElementPanel
    {
        public IOptionTree Tree;
        public event Action<IOptionTree> NextClicked;

        #region Styles
    
        private static GUIStyle _optionWithIcon;
        private static GUIStyle _optionWithoutIcon;
        private static GUIStyle _rightArrow;

        private static bool _stylesCached;

        private void LoadStyles()
        {
            _optionWithIcon = new GUIStyle("PR Label")
            {
                richText = true,
                alignment = TextAnchor.MiddleLeft
            };
            _optionWithIcon.padding.left -= 15;
            _optionWithIcon.fixedHeight = 20f;
        
            _optionWithoutIcon = new GUIStyle(_optionWithIcon);
            _optionWithoutIcon.padding.left += 17;
        
            _rightArrow = new GUIStyle("AC RightArrow");
        
        }
        #endregion


        public float Width;
        public void OnGUI(Rect optionPosition,GUIContent option, bool isSelected,in bool isRepaint)
        {
            if (!_stylesCached)
            {
                LoadStyles();
                _stylesCached = true;
            }

            bool isNextClicked = GUI.Button(optionPosition,option,option.image ? _optionWithIcon : _optionWithoutIcon) && isSelected; 
            if (isRepaint)
            {
                var optionStyle =option.image ? _optionWithIcon : _optionWithoutIcon;
                Width = optionStyle.CalcSize(option).x;
            }

            var right = optionPosition.xMax;

            //has next (childs)
            if (Tree is CategoryTree)
            {
                right -= 13;
                var rightArrowPosition = new Rect(right, optionPosition.y + 4, 13, 13);

                if (isRepaint)
                    _rightArrow.Draw(rightArrowPosition, isSelected, true, true, false);
            }
            
            if (isNextClicked)
            {
                NextClicked?.Invoke(Tree);
            }
        }
    }
}