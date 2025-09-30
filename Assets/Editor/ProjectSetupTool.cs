using UnityEngine;
using UnityEditor;
using System.IO;

namespace SuperJoshua.Editor
{
    /// <summary>
    /// Herramienta del editor para generar sprites placeholders y configurar el proyecto automáticamente
    /// </summary>
    public class ProjectSetupTool : EditorWindow
    {
        [MenuItem("Super Joshua/Setup Complete Project")]
        public static void ShowWindow()
        {
            GetWindow<ProjectSetupTool>("Super Joshua Setup");
        }

        private void OnGUI()
        {
            GUILayout.Label("Super Joshua - Configuración Automática", EditorStyles.boldLabel);
            GUILayout.Space(10);

            GUILayout.Label("Esta herramienta configurará automáticamente:", EditorStyles.helpBox);
            GUILayout.Label("• Sprites placeholders");
            GUILayout.Label("• Prefabs listos para usar");
            GUILayout.Label("• Capas de física");
            GUILayout.Label("• Tags necesarios");
            GUILayout.Label("• Escena demo jugable");

            GUILayout.Space(20);

            if (GUILayout.Button("🚀 CONFIGURAR TODO AUTOMÁTICAMENTE", GUILayout.Height(40)))
            {
                SetupCompleteProject();
            }

            GUILayout.Space(10);

            if (GUILayout.Button("🎨 Solo crear sprites placeholders"))
            {
                CreatePlaceholderSprites();
            }

            if (GUILayout.Button("⚙️ Solo configurar capas y tags"))
            {
                SetupLayersAndTags();
            }

            if (GUILayout.Button("📦 Solo crear prefabs"))
            {
                CreatePrefabs();
            }
        }

        /// <summary>
        /// Configuración completa del proyecto
        /// </summary>
        public static void SetupCompleteProject()
        {
            Debug.Log("🚀 Iniciando configuración completa del proyecto Super Joshua...");

            // 1. Configurar capas y tags
            SetupLayersAndTags();

            // 2. Crear sprites placeholders
            CreatePlaceholderSprites();

            // 3. Crear materials de física
            CreatePhysicsMaterials();

            // 4. Crear prefabs
            CreatePrefabs();

            // 5. Configurar escena demo
            SetupDemoScene();

            AssetDatabase.Refresh();
            Debug.Log("✅ ¡Proyecto Super Joshua configurado completamente! Presiona PLAY para jugar.");
        }

        /// <summary>
        /// Configura las capas de física y tags necesarios
        /// </summary>
        private static void SetupLayersAndTags()
        {
            Debug.Log("⚙️ Configurando capas y tags...");

            // Configurar Tags
            string[] requiredTags = { "Player", "Enemy", "PowerUp", "Ground" };
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty tagsProp = tagManager.FindProperty("tags");

            foreach (string tag in requiredTags)
            {
                bool tagExists = false;
                for (int i = 0; i < tagsProp.arraySize; i++)
                {
                    if (tagsProp.GetArrayElementAtIndex(i).stringValue == tag)
                    {
                        tagExists = true;
                        break;
                    }
                }

                if (!tagExists)
                {
                    tagsProp.InsertArrayElementAtIndex(tagsProp.arraySize);
                    tagsProp.GetArrayElementAtIndex(tagsProp.arraySize - 1).stringValue = tag;
                }
            }

            // Configurar Layers
            SerializedProperty layersProp = tagManager.FindProperty("layers");
            string[] requiredLayers = { "", "", "", "", "", "", "", "", "Ground", "Player", "PowerUps", "Enemies" };

            for (int i = 8; i < requiredLayers.Length && i < 32; i++)
            {
                if (!string.IsNullOrEmpty(requiredLayers[i]))
                {
                    layersProp.GetArrayElementAtIndex(i).stringValue = requiredLayers[i];
                }
            }

            tagManager.ApplyModifiedProperties();
            Debug.Log("✅ Tags y capas configurados");
        }

        /// <summary>
        /// Crea sprites placeholders para todos los elementos del juego
        /// </summary>
        private static void CreatePlaceholderSprites()
        {
            Debug.Log("🎨 Creando sprites placeholders...");

            // Crear texturas placeholder
            CreateSpriteTexture("Assets/Sprites/Player/joshua_idle.png", Color.blue, 32, 32);
            CreateSpriteTexture("Assets/Sprites/Player/joshua_walk.png", Color.cyan, 32, 32);
            CreateSpriteTexture("Assets/Sprites/Player/joshua_jump.png", Color.green, 32, 32);

            CreateSpriteTexture("Assets/Sprites/Player/sonic_idle.png", new Color(0, 0.5f, 1f), 32, 32);
            CreateSpriteTexture("Assets/Sprites/Player/sonic_run.png", new Color(0, 0.7f, 1f), 32, 32);
            CreateSpriteTexture("Assets/Sprites/Player/sonic_spindash.png", new Color(0, 0.3f, 0.8f), 32, 32);

            CreateSpriteTexture("Assets/Sprites/PowerUps/star.png", Color.yellow, 16, 16);
            CreateSpriteTexture("Assets/Sprites/PowerUps/coin.png", new Color(1f, 0.8f, 0f), 16, 16);
            CreateSpriteTexture("Assets/Sprites/PowerUps/ring.png", new Color(1f, 1f, 0.3f), 16, 16);

            CreateSpriteTexture("Assets/Sprites/Environment/platform.png", new Color(0.6f, 0.4f, 0.2f), 64, 16);
            CreateSpriteTexture("Assets/Sprites/Environment/background.png", new Color(0.5f, 0.8f, 1f), 32, 32);

            Debug.Log("✅ Sprites placeholders creados");
        }

        /// <summary>
        /// Crea una textura placeholder y la configura como sprite
        /// </summary>
        private static void CreateSpriteTexture(string path, Color color, int width, int height)
        {
            // Crear directorio si no existe
            string directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Crear textura
            Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            texture.filterMode = FilterMode.Point; // Pixel perfect

            // Llenar con color
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    texture.SetPixel(x, y, color);
                }
            }

            // Añadir borde más oscuro
            Color borderColor = color * 0.7f;
            for (int x = 0; x < width; x++)
            {
                texture.SetPixel(x, 0, borderColor);
                texture.SetPixel(x, height - 1, borderColor);
            }
            for (int y = 0; y < height; y++)
            {
                texture.SetPixel(0, y, borderColor);
                texture.SetPixel(width - 1, y, borderColor);
            }

            texture.Apply();

            // Guardar como PNG
            byte[] pngData = texture.EncodeToPNG();
            File.WriteAllBytes(path, pngData);

            AssetDatabase.ImportAsset(path);

            // Configurar como sprite
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.spritePixelsPerUnit = 32;
                importer.filterMode = FilterMode.Point;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.SaveAndReimport();
            }
        }

        /// <summary>
        /// Crea los materials de física necesarios
        /// </summary>
        private static void CreatePhysicsMaterials()
        {
            Debug.Log("🔧 Creando materials de física...");

            // Material para el jugador
            PhysicsMaterial2D playerMaterial = new PhysicsMaterial2D("PlayerMaterial");
            playerMaterial.friction = 0.4f;
            playerMaterial.bounciness = 0f;

            if (!Directory.Exists("Assets/Materials"))
            {
                Directory.CreateDirectory("Assets/Materials");
            }

            AssetDatabase.CreateAsset(playerMaterial, "Assets/Materials/PlayerMaterial.physicsMaterial2D");

            Debug.Log("✅ Materials de física creados");
        }

        /// <summary>
        /// Crea todos los prefabs necesarios
        /// </summary>
        private static void CreatePrefabs()
        {
            Debug.Log("📦 Creando prefabs...");

            CreatePlayerPrefab();
            CreatePowerUpPrefabs();
            CreateEnvironmentPrefabs();

            Debug.Log("✅ Prefabs creados");
        }

        /// <summary>
        /// Crea el prefab del jugador
        /// </summary>
        private static void CreatePlayerPrefab()
        {
            GameObject player = new GameObject("Player");

            // Componentes básicos
            SpriteRenderer sr = player.AddComponent<SpriteRenderer>();
            Rigidbody2D rb = player.AddComponent<Rigidbody2D>();
            CapsuleCollider2D col = player.AddComponent<CapsuleCollider2D>();

            // Scripts del jugador
            player.AddComponent<SuperJoshua.Player.PlayerController>();
            player.AddComponent<SuperJoshua.Player.PlayerStateMachine>();

            // Configuración del Rigidbody2D
            rb.gravityScale = 3f;
            rb.freezeRotation = true;

            // Configuración del Collider
            col.size = new Vector2(0.6f, 0.9f);

            // GroundCheck
            GameObject groundCheck = new GameObject("GroundCheck");
            groundCheck.transform.SetParent(player.transform);
            groundCheck.transform.localPosition = new Vector3(0, -0.5f, 0);

            // Asignar sprite
            Sprite joshuaSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Player/joshua_idle.png");
            if (joshuaSprite != null)
            {
                sr.sprite = joshuaSprite;
            }

            // Configurar tag y layer
            player.tag = "Player";
            player.layer = LayerMask.NameToLayer("Player");

            // Guardar prefab
            if (!Directory.Exists("Assets/Prefabs"))
            {
                Directory.CreateDirectory("Assets/Prefabs");
            }

            PrefabUtility.SaveAsPrefabAsset(player, "Assets/Prefabs/Player.prefab");
            DestroyImmediate(player);
        }

        /// <summary>
        /// Crea los prefabs de power-ups
        /// </summary>
        private static void CreatePowerUpPrefabs()
        {
            // Estrella
            CreatePowerUpPrefab("Star", "Assets/Sprites/PowerUps/star.png", typeof(SuperJoshua.PowerUps.StarPowerUp));

            // Moneda
            CreatePowerUpPrefab("Coin", "Assets/Sprites/PowerUps/coin.png", typeof(SuperJoshua.PowerUps.CoinPowerUp));

            // Anillo
            CreatePowerUpPrefab("Ring", "Assets/Sprites/PowerUps/ring.png", typeof(SuperJoshua.PowerUps.RingPowerUp));
        }

        /// <summary>
        /// Crea un prefab de power-up específico
        /// </summary>
        private static void CreatePowerUpPrefab(string name, string spritePath, System.Type scriptType)
        {
            GameObject powerUp = new GameObject(name);

            // Componentes básicos
            SpriteRenderer sr = powerUp.AddComponent<SpriteRenderer>();
            CircleCollider2D col = powerUp.AddComponent<CircleCollider2D>();

            // Script específico
            powerUp.AddComponent(scriptType);

            // Configuración del Collider
            col.isTrigger = true;
            col.radius = 0.4f;

            // Asignar sprite
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
            if (sprite != null)
            {
                sr.sprite = sprite;
            }

            // Configurar tag y layer
            powerUp.tag = "PowerUp";
            powerUp.layer = LayerMask.NameToLayer("PowerUps");

            // Guardar prefab
            PrefabUtility.SaveAsPrefabAsset(powerUp, $"Assets/Prefabs/{name}.prefab");
            DestroyImmediate(powerUp);
        }

        /// <summary>
        /// Crea prefabs del entorno
        /// </summary>
        private static void CreateEnvironmentPrefabs()
        {
            // Plataforma
            GameObject platform = new GameObject("Platform");

            SpriteRenderer sr = platform.AddComponent<SpriteRenderer>();
            BoxCollider2D col = platform.AddComponent<BoxCollider2D>();

            // Asignar sprite
            Sprite platformSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Environment/platform.png");
            if (platformSprite != null)
            {
                sr.sprite = platformSprite;
            }

            // Configurar collider
            col.size = new Vector2(2f, 0.5f);

            // Configurar tag y layer
            platform.tag = "Ground";
            platform.layer = LayerMask.NameToLayer("Ground");

            // Guardar prefab
            PrefabUtility.SaveAsPrefabAsset(platform, "Assets/Prefabs/Platform.prefab");
            DestroyImmediate(platform);
        }

        /// <summary>
        /// Configura una escena demo completa y jugable
        /// </summary>
        private static void SetupDemoScene()
        {
            Debug.Log("🗺️ Configurando escena demo...");

            // Limpiar escena actual
            GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
            foreach (GameObject obj in allObjects)
            {
                if (obj.name != "Main Camera")
                {
                    DestroyImmediate(obj);
                }
            }

            // Configurar cámara
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                mainCamera.orthographic = true;
                mainCamera.orthographicSize = 5f;
                mainCamera.backgroundColor = new Color(0.4f, 0.7f, 1f); // Azul cielo
            }

            // Crear GameManager
            GameObject gameManager = new GameObject("GameManager");
            gameManager.AddComponent<SuperJoshua.GameManager.GameManager>();

            // Instanciar jugador
            GameObject playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Player.prefab");
            if (playerPrefab != null)
            {
                GameObject player = PrefabUtility.InstantiatePrefab(playerPrefab) as GameObject;
                player.transform.position = new Vector3(0, 2, 0);
            }

            // Crear plataformas
            GameObject platformPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Platform.prefab");
            if (platformPrefab != null)
            {
                // Plataforma principal
                GameObject platform1 = PrefabUtility.InstantiatePrefab(platformPrefab) as GameObject;
                platform1.transform.position = new Vector3(0, 0, 0);
                platform1.transform.localScale = new Vector3(4, 1, 1);

                // Plataformas adicionales
                GameObject platform2 = PrefabUtility.InstantiatePrefab(platformPrefab) as GameObject;
                platform2.transform.position = new Vector3(6, 2, 0);
                platform2.transform.localScale = new Vector3(2, 1, 1);

                GameObject platform3 = PrefabUtility.InstantiatePrefab(platformPrefab) as GameObject;
                platform3.transform.position = new Vector3(-6, 1, 0);
                platform3.transform.localScale = new Vector3(2, 1, 1);

                GameObject platform4 = PrefabUtility.InstantiatePrefab(platformPrefab) as GameObject;
                platform4.transform.position = new Vector3(12, 4, 0);
                platform4.transform.localScale = new Vector3(2, 1, 1);
            }

            // Añadir power-ups
            GameObject starPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Star.prefab");
            if (starPrefab != null)
            {
                GameObject star = PrefabUtility.InstantiatePrefab(starPrefab) as GameObject;
                star.transform.position = new Vector3(6, 3.5f, 0);
            }

            GameObject coinPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Coin.prefab");
            if (coinPrefab != null)
            {
                GameObject coin1 = PrefabUtility.InstantiatePrefab(coinPrefab) as GameObject;
                coin1.transform.position = new Vector3(-5, 2.5f, 0);

                GameObject coin2 = PrefabUtility.InstantiatePrefab(coinPrefab) as GameObject;
                coin2.transform.position = new Vector3(3, 1.5f, 0);

                GameObject coin3 = PrefabUtility.InstantiatePrefab(coinPrefab) as GameObject;
                coin3.transform.position = new Vector3(12, 5.5f, 0);
            }

            GameObject ringPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Ring.prefab");
            if (ringPrefab != null)
            {
                GameObject ring1 = PrefabUtility.InstantiatePrefab(ringPrefab) as GameObject;
                ring1.transform.position = new Vector3(8, 3f, 0);

                GameObject ring2 = PrefabUtility.InstantiatePrefab(ringPrefab) as GameObject;
                ring2.transform.position = new Vector3(10, 3.5f, 0);
            }

            Debug.Log("✅ Escena demo configurada con nivel jugable");
        }
    }
}