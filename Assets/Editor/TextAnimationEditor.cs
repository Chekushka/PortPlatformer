using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(TextAnimation))] 
    public class TextAnimationEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            // 1. Малюємо стандартний інспектор (щоб бачити змінні toolName тощо)
            DrawDefaultInspector();

            // Отримуємо посилання на цільовий скрипт з правильним типом
            TextAnimation textAnimation = (TextAnimation)target;

            EditorGUILayout.Space(10); // Додаємо трохи відступу для краси
        
            // 2. Створюємо кнопку
            // GUILayout.Button повертає true в момент натискання
            if (GUILayout.Button("Play Test Animation", GUILayout.Height(30)))
            {
                // 3. Викликаємо метод
                textAnimation.PlayTestAnimation();
            
                // Якщо метод змінює змінні в скрипті, позначаємо об'єкт як "брудний", 
                // щоб Unity запропонувала зберегти сцену (Ctrl+S)
                EditorUtility.SetDirty(textAnimation);
            }
            
            if (GUILayout.Button("Stop Animation", GUILayout.Height(30)))
            {
                textAnimation.StopAnimation();
                EditorUtility.SetDirty(textAnimation);
            }
        }
    }
}