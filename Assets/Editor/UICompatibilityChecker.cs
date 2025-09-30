using UnityEngine;
using UnityEditor;

namespace SuperJoshua.Editor
{
    /// <summary>
    /// Herramienta para verificar y corregir problemas de compatibilidad de UI
    /// </summary>
    public static class UICompatibilityChecker
    {
        [MenuItem("Super Joshua/Check UI Compatibility")]
        public static void CheckUICompatibility()
        {
            Debug.Log("🔍 Verificando compatibilidad de UI...");
            
            string unityVersion = Application.unityVersion;
            Debug.Log($"📋 Unity Version: {unityVersion}");
            
            // Verificar si Unity UI está disponible
            bool hasUIModule = System.Type.GetType("UnityEngine.UI.Text, UnityEngine.UI") != null;
            bool hasSliderModule = System.Type.GetType("UnityEngine.UI.Slider, UnityEngine.UI") != null;
            
            Debug.Log($"✅ UI Text disponible: {hasUIModule}");
            Debug.Log($"✅ UI Slider disponible: {hasSliderModule}");
            
            if (!hasUIModule || !hasSliderModule)
            {
                EditorUtility.DisplayDialog(
                    "⚠️ Problema de UI Detectado",
                    "Los componentes de UI no están disponibles.\n\n" +
                    "Soluciones posibles:\n" +
                    "1. Verificar que el módulo UI esté instalado\n" +
                    "2. Reimportar el proyecto\n" +
                    "3. Usar Unity 2022.3 LTS para máxima compatibilidad\n\n" +
                    "¿Quieres que se abra la ventana de Package Manager?",
                    "Sí, abrir Package Manager",
                    "Cancelar");
                    
                EditorApplication.ExecuteMenuItem("Window/Package Manager");
            }
            else
            {
                EditorUtility.DisplayDialog(
                    "✅ Compatibilidad UI Verificada",
                    "Todos los componentes de UI están disponibles.\n" +
                    "El proyecto está listo para compilar correctamente.",
                    "Perfecto!");
            }
        }
        
        [MenuItem("Super Joshua/Force Refresh UI References")]
        public static void ForceRefreshUI()
        {
            Debug.Log("🔄 Forzando actualización de referencias UI...");
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog(
                "🔄 Referencias Actualizadas",
                "Las referencias de UI han sido actualizadas.\n" +
                "Intenta compilar el proyecto nuevamente.",
                "OK");
        }
    }
}