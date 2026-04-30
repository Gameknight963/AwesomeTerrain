using Il2Cpp;
using MelonLoader;
using UnityEngine;

namespace AwesomeTerrain
{
    public class Mod : MelonMod
    {
        public static GameObject Kiri;

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (sceneName != "Version 1.9 POST") return;

            GameObject terrainGen = new GameObject("TerrainGenerator");
            terrainGen.AddComponent<TerrainGenerator>();
            GameObject.Find("World/House/Doors/ExitFrame/ExitDoor").SetActive(false);
            GameObject.Find("World/Game/Acts/Hello Mita/Interactables 1/I Exit Door 1 ").SetActive(false);
            GameObject.Find("World/Game/Acts/Quality Time/Interactables 2/I ExitDoor 2").SetActive(false);

            OOBProtection oob = GameObject.FindObjectOfType<OOBProtection>();
            oob.limit = -300;

            GameObject playerCamera = GameObject.Find("playerCamera");
            playerCamera.GetComponent<WorldOutline>().enabled = false;

            playerCamera.AddComponent<WaterVolume>();

            RenderSettings.fogColor = new Color(1f, 1f, 1f, 1f);
            RenderSettings.fogDensity = 0.01f;
        }
    }
}
