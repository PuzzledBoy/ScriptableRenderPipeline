using System;
using UnityEngine;

namespace UnityEditor.Rendering.Universal.DY
{
    public partial class DYSGMasterNodeGUI : UnityEditor.ShaderGUI
    {
        private static readonly Color titleColor = new Color(246f/255,249f/255,228f/255); 
        
        
        protected void DrawTitle(string label)
        {   
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            var oriFontStyle = EditorStyles.label.fontStyle;
            var oriFontSize = EditorStyles.label.fontSize;
            EditorStyles.label.fontStyle = FontStyle.Bold;
            EditorStyles.label.fontSize = 13;
            
            var oriColor = GUI.color;
            GUI.color = titleColor;
            EditorGUILayout.LabelField(label);
            GUI.color = oriColor;
            EditorGUILayout.Space();

            EditorStyles.label.fontSize = oriFontSize;
            EditorStyles.label.fontStyle = oriFontStyle;

        }
        
        private void DoEnumPopUp<T>(MaterialProperty property, GUIContent label, bool showModify, params GUILayoutOption[] options) where T : struct, Enum
        {
            var modify = showModify && !EditorGUI.showMixedValue;
            if (modify)
            {
                EditorStyles.label.fontStyle = FontStyle.Bold;
            }

            EditorGUI.showMixedValue = property.hasMixedValue;
            
            T mode = (T) Enum.ToObject(typeof(T), (int) property.floatValue);
            EditorGUI.BeginChangeCheck();
            mode = (T) EditorGUILayout.EnumPopup(label, mode, options);
            if (EditorGUI.EndChangeCheck())
            {
                materialEditor.RegisterPropertyChangeUndo(label.text);
                property.floatValue = Convert.ToSingle(mode);
            }
            EditorGUI.showMixedValue = false;
            
            EditorStyles.label.fontStyle = FontStyle.Normal;
        }

        protected void DoEnumPopUp<T>(MaterialProperty property, GUIContent label, params GUILayoutOption[] options) where T : struct, Enum
        {
            DoEnumPopUp<T>(property, label, false);
        }

        protected void DoEnumPopUp<T>(MaterialProperty property, GUIContent label, float defaultValue, params GUILayoutOption[] options) where T : struct, Enum
        {
            DoEnumPopUp<T>(property,label,property.floatValue != defaultValue);
        }
        
        protected void DoToggle(MaterialProperty property, GUIContent label, params GUILayoutOption[] options)
        {
            EditorGUI.showMixedValue = property.hasMixedValue;

            EditorGUI.BeginChangeCheck();
            var b = property.floatValue == 0.0 ? false : true;
            b = EditorGUILayout.Toggle(label, b, options);
            if (EditorGUI.EndChangeCheck())
            {
                materialEditor.RegisterPropertyChangeUndo(label.text);
                property.floatValue = b ? 1f : 0f;
            }

            EditorGUI.showMixedValue = false;
        }
    }
}