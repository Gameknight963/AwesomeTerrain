using Il2Cpp;
using Il2CppColorful;
using MelonLoader;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace AwesomeTerrain
{
    [RegisterTypeInIl2Cpp]
    public class WaterVolume : MonoBehaviour
    {
        private Camera _cam;
        private Color _defaultColor;
        private GaussianBlur _blur;
        private bool _underwater = false;
        GameObject water;

        private PostProcessVolume _volume;
        private ColorGrading _colorGrading;

        private void Start()
        {
            _cam = Camera.main;
            _defaultColor = _cam.backgroundColor;
            _blur = _cam.GetComponent<GaussianBlur>();
            _blur.enabled = false;

            water = GameObject.Find("Water");

            GameObject volumeObj = new GameObject("UnderwaterVolume");
            volumeObj.layer = 31;
            _volume = volumeObj.AddComponent<PostProcessVolume>();
            _volume.isGlobal = true;
            _volume.priority = 10; // higher than existing volumes
            _volume.profile = ScriptableObject.CreateInstance<PostProcessProfile>();
            _colorGrading = _volume.profile.AddSettings<ColorGrading>();
            _colorGrading.colorFilter.Override(new Color(0.1f, 0.3f, 0.6f));
            _volume.enabled = false;
        }

        private void Update()
        {
            bool submerged = _cam.transform.position.y < water.transform.position.y;
            if (submerged == _underwater) return;
            _underwater = submerged;

            if (_underwater)
            {
                _volume.enabled = true;
                _blur.enabled = true;
            }
            else
            {
                _volume.enabled = false;
                _blur.enabled = false;
            }
        }
    }
}
