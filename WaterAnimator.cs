using MelonLoader;
using UnityEngine;

namespace mszbhop
{
    [RegisterTypeInIl2Cpp]
    public class WaterAnimator : MonoBehaviour
    {
        private MeshRenderer _renderer;

        private void Start()
        {
            _renderer = GetComponent<MeshRenderer>();
        }

        private void Update()
        {
            float offset = Time.time * 0.02f;
            _renderer.material.mainTextureOffset = new Vector2(offset, offset * 0.7f);
            _renderer.material.SetTextureOffset("_BumpMap", new Vector2(offset, offset * 0.7f));
        }
    }
}
