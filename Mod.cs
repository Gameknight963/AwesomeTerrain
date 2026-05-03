using Il2Cpp;
using MelonLoader;
using UnityEngine;

namespace AwesomeTerrain
{
    public class Mod : MelonMod
    {
        public static GameObject Kiri;
        private static TerrainGenerator generator;
        
        public static int Seed { get; private set; }

        public static void SetSeedAndGenerate(int newSeed)
        {
            Seed = newSeed;
            generator?.Generate(newSeed);
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (sceneName != "Version 1.9 POST")
            {
                generator = null;
                Seed = 0;
                return;
            }

            GameObject terrainGen = new GameObject("TerrainGenerator");
            generator = terrainGen.AddComponent<TerrainGenerator>();
            
            if (!Networking.Available)
                SetSeedAndGenerate(Random.Range(int.MinValue, int.MaxValue));

            GameObject.Find("World/House/Doors/ExitFrame/ExitDoor")?.SetActive(false);
            GameObject.Find("World/Game/Acts/Hello Mita/Interactables 1/I Exit Door 1 ")?.SetActive(false);
            GameObject.Find("World/Game/Acts/Quality Time/Interactables 2/I ExitDoor 2")?.SetActive(false);

            OOBProtection oob = GameObject.FindObjectOfType<OOBProtection>();
            oob.limit = -300;

            GameObject playerCamera = GameObject.Find("playerCamera");
            playerCamera.GetComponent<WorldOutline>().enabled = false;

            RenderSettings.fogColor = new Color(1f, 1f, 1f, 1f);
            RenderSettings.fogDensity = 0.01f;
        }
    }
}
