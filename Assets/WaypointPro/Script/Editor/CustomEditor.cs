using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace Mr1
{
    public class CustomGUI : Editor
    {
        static List<string> HeaderList = new List<string>();

        public static string Header_LeftStyle = "flow overlay header lower left";
        public static string Header_RightStyle = "flow overlay header lower right";
        public static string PopupBackground_DefaultStyle = "PopupCurveSwatchBackground";

        public static GUIContent iconToolbarMinus = IconContent("Toolbar Minus", "Remove");
        public static GUIContent iconToolbarPlus = IconContent("Toolbar Plus", "Add new");

        public static GUIContent IconContent(string name, string tooltip)
        {
            var t = typeof(EditorGUIUtility);
            var m = t.GetMethod("IconContent", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static,
                                System.Type.DefaultBinder, new[] { typeof(string) }, null);
            var content = (GUIContent)m.Invoke(t, new[] { name });
            content.tooltip = tooltip;
            return content;
        }

        public static void BeginVertical()
        {
            GUILayout.BeginVertical(PopupBackground_DefaultStyle, GUILayout.ExpandWidth(true));
        }

        public static void EndVertical()
        {
            GUI.color = Color.white;
            GUILayout.EndVertical();
        }

        public static void Space()
        {
            EditorGUILayout.Space();
        }

        public static bool HeaderButton(string buttonName, Action OnPlusBtnClick = null, Action OnMinusBtnClick = null)
        {
            GUI.color = Color.cyan;

            HeaderList.Add(buttonName);

            bool foldOut = EditorPrefs.GetBool(buttonName, false);

            Rect rect = GUILayoutUtility.GetRect(GUIContent.none, Header_LeftStyle, GUILayout.ExpandWidth(true));

            Rect plusBtnRect = new Rect(rect.width - 35, rect.y + 2, 25, 25);
            Rect minusBtnRect = new Rect(rect.width - 10, rect.y + 2, 25, 25);

            if (foldOut && OnPlusBtnClick != null)
            {
                // Plus Button Down
                if (GUI.Button(plusBtnRect, "", "label"))
                {
                    OnPlusBtnClick();
                }
            }

            if (foldOut && OnMinusBtnClick != null)
            {
                // Minus Button Down
                if (GUI.Button(minusBtnRect, "", "label"))
                {
                    OnMinusBtnClick();
                    EditorPrefs.DeleteKey(buttonName);
                    foldOut = false;
                }
            }

            // Header Button Down
            if (GUI.Button(rect, buttonName, Header_LeftStyle))
            {
                // Button Up
                if (Event.current.button == 0)
                {
                    EditorPrefs.SetBool(buttonName, !foldOut);
                }

                if (!foldOut)
                {
                    foreach (var header in HeaderList)
                    {
                        if (header != buttonName)
                            EditorPrefs.SetBool(header, false);
                    }
                }
            }

            if (foldOut)
            {
                if (OnPlusBtnClick != null) GUI.Label(plusBtnRect, iconToolbarPlus);
                if (OnMinusBtnClick != null) GUI.Label(minusBtnRect, iconToolbarMinus);
            }

            GUI.color = Color.white;

            return foldOut;
        }

        public static void HeaderLabel(string labelName, Color color, Action OnPlusBtnClick = null, Action OnMinusBtnClick = null)
        {
            GUI.color = color;

            Rect labelRect = GUILayoutUtility.GetRect(GUIContent.none, Header_LeftStyle, GUILayout.ExpandWidth(true));
            labelRect.x += 10;
            Rect plusBtnRect = new Rect(labelRect.width - 25, labelRect.y, 25, 25);
            Rect minusBtnRect = new Rect(labelRect.width, labelRect.y, 25, 25);

            GUI.Label(labelRect, labelName, Header_LeftStyle);

            if (OnPlusBtnClick != null)
            {
                if (GUI.Button(plusBtnRect, "", "label"))
                    OnPlusBtnClick();
            }

            if (OnMinusBtnClick != null)
            {
                if (GUI.Button(minusBtnRect, "", "label"))
                    OnMinusBtnClick();
            }

            if (OnPlusBtnClick != null) GUI.Label(plusBtnRect, iconToolbarPlus);
            if (OnMinusBtnClick != null) GUI.Label(minusBtnRect, iconToolbarMinus);

            GUI.color = Color.white;
        }

        public static void DrawSeparator(Color color, float size = 1.0f)
        {
            EditorGUILayout.Space();
            Texture2D tex = new Texture2D(1, 1);

            GUI.color = color;
            float y = GUILayoutUtility.GetLastRect().yMax;
            GUI.DrawTexture(new Rect(0.0f, y, Screen.width, size), tex);
            tex.hideFlags = HideFlags.DontSave;
            GUI.color = Color.white;

            EditorGUILayout.Space();
        }
    }
}