using System;
using UnityEditor;
using UnityEngine;

namespace MTPS.Core.Editor.FuzzySearch
{
    public class FuzzyHeaderWithBackButtonPanel
    {
        public event Action BackClicked;
        private float _headerWidth;

        #region Styles

        private static GUIStyle _header;
        private static GUIStyle _leftArrow;
        private static bool _stylesCached;

        private static void LoadStyles()
        {
            _header = new GUIStyle("In BigTitle")
            {
                font = EditorStyles.boldLabel.font,
                normal =
                {
                    textColor = EditorGUIUtility.isProSkin? new Color(0.81f,0.81f,0.81f,1): new Color(0.8f,0.8f,0.8f,1)
                },
                alignment = TextAnchor.MiddleCenter,
                padding = new RectOffset(0, 0, 0, 0)
            };
            
            _leftArrow = new GUIStyle("AC LeftArrow");
        }
        #endregion

        public void OnGUI(IOptionTree parent, Rect headerPosition, in bool isRepaint)
        {
            if (!_stylesCached)
            {
                LoadStyles();
                _stylesCached = true;
            }
            Vector2 iconSize = EditorGUIUtility.GetIconSize();
            EditorGUIUtility.SetIconSize(Vector2.one * 16);
            
            

            var grandParent = parent?.Parent;
            var headerContent = parent?.Content;

            _headerWidth = _header.CalcSize(headerContent).x;

            DrawLabel(headerPosition, headerContent ?? GUIContent.none,in isRepaint);

            EditorGUIUtility.SetIconSize(iconSize);
        
            if (grandParent != null)
            {
                if (isRepaint)
                {
                    var leftArrowPosition = new Rect(headerPosition.x + 4, headerPosition.y + 7, 13, 13);
                    _leftArrow.Draw(leftArrowPosition, GUIContent.none, false, true, true, true);
                }

                if (Event.current.clickCount > 0 && headerPosition.Contains(Event.current.mousePosition))
                {
                    Event.current.Use();
                    BackClicked?.Invoke();
                }
            }
        }

        private void DrawLabel(Rect headerPosition,GUIContent headerContent,in bool isRepaint)
        {
            if (isRepaint)
                _header.Draw(headerPosition, headerContent, true, false, false, false);
        }

        public float GetWidth() => _headerWidth;
    }
}