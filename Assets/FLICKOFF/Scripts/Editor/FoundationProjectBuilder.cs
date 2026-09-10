#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FlickOff.Editor
{
    /// <summary>
    /// Creates and validates the deterministic camera parity lab skeleton.
    /// This is editor scaffolding, not gameplay implementation.
    /// </summary>
    public static class FoundationProjectBuilder
    {
        private const string LabScenePath = "Assets/FLICKOFF/Scenes/Labs/CameraParityLab.unity";

        [MenuItem("FLICK OFF/Build Foundation Scene")]
        public static void BuildFoundation()
        {
            ConfigureProjectSettings();

            foreach (string folder in new[]
            {
                "Assets/FLICKOFF/Art",
                "Assets/FLICKOFF/Cameras",
                "Assets/FLICKOFF/Environment",
                "Assets/FLICKOFF/Props",
                "Assets/FLICKOFF/VFX",
                "Assets/FLICKOFF/Audio",
                "Assets/FLICKOFF/Materials",
                "Assets/FLICKOFF/Prefabs",
                "Assets/FLICKOFF/Interaction",
                "Assets/FLICKOFF/Player",
                "Assets/FLICKOFF/Scrap",
                "Assets/FLICKOFF/World",
                "Assets/FLICKOFF/Scenes/Bootstrap",
                "Assets/FLICKOFF/Scenes/Labs",
                "Assets/FLICKOFF/Scripts/Runtime/Cameras",
                "Assets/FLICKOFF/Scripts/Runtime/Interaction",
                "Assets/FLICKOFF/Scripts/Runtime/Physics",
                "Assets/FLICKOFF/Scripts/Runtime/Player",
                "Assets/FLICKOFF/Scripts/Runtime/Scrap",
                "Assets/FLICKOFF/Scripts/Runtime/Quota",
                "Assets/FLICKOFF/Scripts/Runtime/Persistence",
                "Assets/FLICKOFF/Scripts/Editor/Settings"
            })
            {
                EnsureFolder(folder);
            }

            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Lab_Ground";
            ground.transform.position = Vector3.zero;
            ground.transform.localScale = new Vector3(4f, 1f, 4f);

            GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            player.name = "PlayerTestAnchor";
            player.transform.position = new Vector3(0f, 1f, -18f);

            GameObject target = GameObject.CreatePrimitive(PrimitiveType.Cube);
            target.name = "TargetTestAnchor";
            target.transform.position = new Vector3(0f, 1f, 18f);
            target.transform.localScale = new Vector3(1.5f, 2f, 1.5f);

            GameObject occluder = GameObject.CreatePrimitive(PrimitiveType.Cube);
            occluder.name = "KnownOccluder";
            occluder.transform.position = new Vector3(0f, 2f, 0f);
            occluder.transform.localScale = new Vector3(4f, 4f, 1f);

            GameObject cameraMount = new GameObject("FlickCameraMount");
            cameraMount.transform.position = new Vector3(0f, 3.49f, 8f);
            cameraMount.transform.rotation = Quaternion.Euler(0f, 180f, 0f);

            GameObject viewCamera = new GameObject("LabDebugCamera");
            Camera camera = viewCamera.AddComponent<Camera>();
            viewCamera.transform.position = new Vector3(0f, 6f, -28f);
            viewCamera.transform.rotation = Quaternion.Euler(10f, 0f, 0f);
            camera.nearClipPlane = 0.1f;
            camera.farClipPlane = 500f;

            GameObject lightObject = new GameObject("Lab_KeyLight");
            Light light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.1f;
            lightObject.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

            GameObject notes = new GameObject("CameraParityLab_Notes");
            notes.transform.position = new Vector3(0f, 0.01f, 0f);

            if (!EditorSceneManager.SaveScene(scene, LabScenePath))
            {
                throw new InvalidOperationException("Could not save the camera parity lab scene.");
            }

            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(LabScenePath, true)
            };

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("FLICK OFF foundation scene created at " + LabScenePath);
        }

        public static void ValidateFoundation()
        {
            if (EditorSettings.serializationMode != SerializationMode.ForceText)
            {
                throw new InvalidOperationException("Unity asset serialization is not Force Text.");
            }

            if (VersionControlSettings.mode != "Visible Meta Files")
            {
                throw new InvalidOperationException("Unity external version control is not Visible Meta Files.");
            }

            if (PlayerSettings.productName != FlickOffFoundation.ProjectName)
            {
                throw new InvalidOperationException("Unity product name is not FLICK OFF.");
            }

            if (PlayerSettings.colorSpace != ColorSpace.Linear)
            {
                throw new InvalidOperationException("Unity color space is not Linear.");
            }

            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(LabScenePath) == null)
            {
                throw new InvalidOperationException("CameraParityLab.unity is missing or invalid.");
            }

            Debug.Log("FLICK OFF foundation validation passed.");
        }

        private static void ConfigureProjectSettings()
        {
            EditorSettings.serializationMode = SerializationMode.ForceText;
            VersionControlSettings.mode = "Visible Meta Files";
            PlayerSettings.companyName = FlickOffFoundation.ProjectName;
            PlayerSettings.productName = FlickOffFoundation.ProjectName;
            PlayerSettings.colorSpace = ColorSpace.Linear;
        }

        private static void EnsureFolder(string path)
        {
            string[] parts = path.Split('/');
            string current = parts[0];

            for (int index = 1; index < parts.Length; index++)
            {
                string next = current + "/" + parts[index];
                if (!AssetDatabase.IsValidFolder(next))
                {
                    AssetDatabase.CreateFolder(current, parts[index]);
                }

                current = next;
            }
        }
    }
}
#endif
