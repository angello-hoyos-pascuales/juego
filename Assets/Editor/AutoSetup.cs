using UnityEngine;
using UnityEditor;

namespace SuperJoshua.Editor
{
    /// <summary>
    /// Configuración automática que se ejecuta al abrir el proyecto por primera vez
    /// </summary>
    [InitializeOnLoad]
    public static class AutoSetup
    {
        private const string SETUP_COMPLETED_KEY = "SuperJoshua_SetupCompleted";

        static AutoSetup()
        {
            // Verificar si ya se ejecutó la configuración
            if (!EditorPrefs.GetBool(SETUP_COMPLETED_KEY, false))
            {
                // Ejecutar configuración automáticamente
                EditorApplication.delayCall += () =>
                {
                    if (EditorUtility.DisplayDialog(
                        "Super Joshua - Configuración Automática",
                        "¡Bienvenido a Super Joshua!\n\n" +
                        "¿Deseas configurar automáticamente el proyecto para que esté listo para jugar?\n\n" +
                        "Esto incluye:\n" +
                        "• Sprites placeholders\n" +
                        "• Prefabs configurados\n" +
                        "• Escena demo jugable\n" +
                        "• Configuración de física",
                        "SÍ, configurar automáticamente",
                        "No, configurar manualmente"))
                    {
                        ProjectSetupTool.SetupCompleteProject();

                        // Marcar como completado
                        EditorPrefs.SetBool(SETUP_COMPLETED_KEY, true);

                        EditorUtility.DisplayDialog(
                            "¡Configuración Completada!",
                            "Super Joshua está listo para jugar.\n\n" +
                            "Controles:\n" +
                            "• A/D: Mover\n" +
                            "• Espacio: Saltar\n" +
                            "• Ctrl: Spin Dash (como Sonic)\n\n" +
                            "¡Presiona PLAY para empezar!",
                            "¡Genial!");
                    }
                    else
                    {
                        EditorUtility.DisplayDialog(
                            "Configuración Manual",
                            "Puedes configurar el proyecto en cualquier momento usando:\n\n" +
                            "Menú: Super Joshua > Setup Complete Project",
                            "Entendido");

                        // Marcar como completado para no preguntar de nuevo
                        EditorPrefs.SetBool(SETUP_COMPLETED_KEY, true);
                    }
                };
            }
        }

        [MenuItem("Super Joshua/Reset Auto Setup")]
        public static void ResetAutoSetup()
        {
            EditorPrefs.DeleteKey(SETUP_COMPLETED_KEY);
            Debug.Log("Auto setup reseteado. Se ejecutará la próxima vez que se abra el proyecto.");
        }
    }
}