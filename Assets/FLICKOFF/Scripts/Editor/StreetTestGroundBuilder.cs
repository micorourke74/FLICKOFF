#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FlickOff.Editor
{
    /// <summary>
    /// Builds an original, collider-friendly street blockout for gameplay testing.
    /// It intentionally uses Unity primitives instead of third-party meshes.
    /// </summary>
    public static class StreetTestGroundBuilder
    {
        private const string ScenePath = "Assets/FLICKOFF/Scenes/Labs/StreetTestGround.unity";

        [MenuItem("FLICK OFF/Build Original Street Test Ground")]
        public static void BuildStreetTestGround()
        {
            EnsureFolder("Assets/FLICKOFF/Scenes/Labs");
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            GameObject root = new GameObject("StreetTestGround");
            root.AddComponent<FlickPrototypeGame>();
            CreateCube("Road_EastWest", new Vector3(0f, -0.08f, 0f), new Vector3(120f, 0.16f, 12f), root.transform);
            CreateCube("Road_NorthSouth", new Vector3(0f, -0.08f, 0f), new Vector3(12f, 0.16f, 120f), root.transform);
            CreateCube("Ground", new Vector3(0f, -0.2f, 0f), new Vector3(120f, 0.2f, 120f), root.transform);

            for (int side = -1; side <= 1; side += 2)
            {
                CreateCube("Sidewalk_EastWest_" + side, new Vector3(0f, 0.08f, side * 8f), new Vector3(120f, 0.16f, 2f), root.transform);
                CreateCube("Sidewalk_NorthSouth_" + side, new Vector3(side * 8f, 0.08f, 0f), new Vector3(2f, 0.16f, 120f), root.transform);
            }

            int buildingIndex = 0;
            for (int x = -2; x <= 2; x++)
            {
                for (int z = -2; z <= 2; z++)
                {
                    if (Mathf.Abs(x) <= 1 || Mathf.Abs(z) <= 1)
                    {
                        continue;
                    }

                    float height = 7f + ((buildingIndex % 3) * 3f);
                    Vector3 position = new Vector3(x * 20f, height * 0.5f, z * 20f);
                    Vector3 scale = new Vector3(13f, height, 13f);
                    CreateCube("Building_Blockout_" + buildingIndex++, position, scale, root.transform);
                }
            }

            for (int index = -4; index <= 4; index++)
            {
                if (index == 0)
                {
                    continue;
                }

                CreateStreetLight(new Vector3(index * 12f, 0f, 6.5f), root.transform);
                CreateStreetLight(new Vector3(6.5f, 0f, index * 12f), root.transform);
            }

            CreateFlickCamera(new Vector3(-27f, 0f, 15f), 90f, "FlickCamera_Northwest", root.transform);
            CreateFlickCamera(new Vector3(27f, 0f, -15f), 270f, "FlickCamera_Southeast", root.transform);
            CreateFlickCamera(new Vector3(15f, 0f, 27f), 180f, "FlickCamera_Northeast", root.transform);
            CreateCube("KnownOccluder_Block", new Vector3(0f, 3f, 24f), new Vector3(8f, 6f, 3f), root.transform);

            GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            player.name = "PlayerVehicle_TestProxy";
            player.transform.SetParent(root.transform);
            player.transform.position = new Vector3(0f, 1f, -35f);
            player.transform.localScale = new Vector3(1.5f, 0.75f, 2.4f);
            player.layer = 2;
            FlickPrototypePlayer playerController = player.AddComponent<FlickPrototypePlayer>();

            GameObject playerView = new GameObject("PlayerView");
            playerView.transform.SetParent(player.transform);
            playerView.transform.localPosition = new Vector3(0f, 0.45f, 0f);
            Camera playerCamera = playerView.AddComponent<Camera>();
            playerCamera.fieldOfView = 75f;
            playerCamera.nearClipPlane = 0.05f;
            playerCamera.farClipPlane = 500f;
            playerCamera.tag = "MainCamera";
            playerController.ConfigureView(playerView.transform);

            GameObject cameraObject = new GameObject("StreetTestGround_OverviewCamera");
            cameraObject.transform.SetParent(root.transform);
            cameraObject.transform.position = new Vector3(0f, 34f, -42f);
            cameraObject.transform.rotation = Quaternion.LookRotation(new Vector3(0f, -20f, 35f), Vector3.up);
            Camera camera = cameraObject.AddComponent<Camera>();
            camera.enabled = false;
            camera.fieldOfView = 58f;
            camera.nearClipPlane = 0.1f;
            camera.farClipPlane = 500f;

            GameObject lightObject = new GameObject("StreetTestGround_KeyLight");
            lightObject.transform.SetParent(root.transform);
            lightObject.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            Light light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.2f;

            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Skybox;
            RenderSettings.ambientIntensity = 0.8f;
            if (!EditorSceneManager.SaveScene(scene, ScenePath))
            {
                throw new InvalidOperationException("Could not save " + ScenePath);
            }

            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
            bool alreadyInBuildSettings = false;
            foreach (EditorBuildSettingsScene buildScene in scenes)
            {
                if (buildScene.path == ScenePath)
                {
                    alreadyInBuildSettings = true;
                    break;
                }
            }

            if (!alreadyInBuildSettings)
            {
                EditorBuildSettingsScene[] updated = new EditorBuildSettingsScene[scenes.Length + 1];
                Array.Copy(scenes, updated, scenes.Length);
                updated[updated.Length - 1] = new EditorBuildSettingsScene(ScenePath, true);
                EditorBuildSettings.scenes = updated;
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Original FLICK OFF street test ground created at " + ScenePath);
        }

        [MenuItem("FLICK OFF/Open Street Test Ground")]
        public static void OpenStreetTestGround()
        {
            Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            if (!scene.IsValid())
            {
                throw new InvalidOperationException("Could not open " + ScenePath);
            }

            Debug.Log("Opened original FLICK OFF street test ground at " + ScenePath);
        }

        private static void CreateFlickCamera(Vector3 position, float compassHeading, string name, Transform parent)
        {
            GameObject pole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pole.name = name;
            pole.transform.SetParent(parent);
            pole.transform.position = position + new Vector3(0f, 2f, 0f);
            pole.transform.localScale = new Vector3(0.35f, 2f, 0.35f);
            FlickCameraActor actor = pole.AddComponent<FlickCameraActor>();
            actor.Configure(name, compassHeading, position);

            GameObject head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            head.name = name + "_Head";
            head.transform.SetParent(pole.transform);
            head.transform.localPosition = new Vector3(0f, 1.9f, 0f);
            head.transform.localScale = new Vector3(0.8f, 0.55f, 0.8f);
            pole.transform.rotation = Quaternion.Euler(0f, FlickCameraMath.CompassHeadingToGtaHeading(compassHeading), 0f);
        }

        private static void CreateStreetLight(Vector3 position, Transform parent)
        {
            GameObject pole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pole.name = "StreetLight_Pole";
            pole.transform.SetParent(parent);
            pole.transform.position = position + new Vector3(0f, 2.5f, 0f);
            pole.transform.localScale = new Vector3(0.12f, 2.5f, 0.12f);

            GameObject lamp = GameObject.CreatePrimitive(PrimitiveType.Cube);
            lamp.name = "StreetLight_Lamp";
            lamp.transform.SetParent(pole.transform);
            lamp.transform.localPosition = new Vector3(0.5f, 2.35f, 0f);
            lamp.transform.localScale = new Vector3(1f, 0.12f, 0.12f);
        }

        private static GameObject CreateCube(string name, Vector3 position, Vector3 scale, Transform parent)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = name;
            cube.transform.SetParent(parent);
            cube.transform.position = position;
            cube.transform.localScale = scale;
            return cube;
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
