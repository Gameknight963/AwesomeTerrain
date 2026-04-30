using MelonLoader;
using MelonLoader.Utils;
using UnityEngine;
using System.IO;

namespace AwesomeTerrain
{
    [RegisterTypeInIl2Cpp]
    public class TerrainGenerator : MonoBehaviour
    {
        private float _terrainYOffset;

        private void Start()
        {
            int resolution = 256;
            float size = 500f;
            float height = 60f;

            float offsetX = Random.Range(0f, 9999f);
            float offsetZ = Random.Range(0f, 9999f);

            Vector3[] vertices = new Vector3[(resolution + 1) * (resolution + 1)];
            Vector2[] uvs = new Vector2[vertices.Length];

            for (int z = 0; z <= resolution; z++)
            {
                for (int x = 0; x <= resolution; x++)
                {
                    int i = z * (resolution + 1) + x;
                    float fx = (float)x / resolution;
                    float fz = (float)z / resolution;

                    float nx = fx * size + offsetX;
                    float nz = fz * size + offsetZ;

                    float h = 0f;
                    h += Mathf.PerlinNoise(nx * 0.003f, nz * 0.003f) * 0.6f;
                    h += Mathf.PerlinNoise(nx * 0.01f, nz * 0.01f) * 0.3f;
                    h += Mathf.PerlinNoise(nx * 0.04f, nz * 0.04f) * 0.1f;

                    vertices[i] = new Vector3(fx * size - size * 0.5f, h * height, fz * size - size * 0.5f);
                    uvs[i] = new Vector2(fx, fz);
                }
            }

            int[] triangles = new int[resolution * resolution * 6];
            int t = 0;
            for (int z = 0; z < resolution; z++)
            {
                for (int x = 0; x < resolution; x++)
                {
                    int i = z * (resolution + 1) + x;
                    triangles[t++] = i;
                    triangles[t++] = i + resolution + 1;
                    triangles[t++] = i + 1;
                    triangles[t++] = i + 1;
                    triangles[t++] = i + resolution + 1;
                    triangles[t++] = i + resolution + 2;
                }
            }

            float cx = size * 0.5f + offsetX;
            float cz = size * 0.5f + offsetZ;
            float centerH = 0f;
            centerH += Mathf.PerlinNoise(cx * 0.003f, cz * 0.003f) * 0.6f;
            centerH += Mathf.PerlinNoise(cx * 0.01f, cz * 0.01f) * 0.3f;
            centerH += Mathf.PerlinNoise(cx * 0.04f, cz * 0.04f) * 0.1f;

            Mesh mesh = new Mesh();
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uvs;
            mesh.RecalculateNormals();

            string texturePath = Path.Combine(MelonEnvironment.ModsDirectory, "mszbhop", "grass.png");
            byte[] bytes = File.ReadAllBytes(texturePath);
            Texture2D tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);

            ImageConversion.LoadImage(tex, bytes);
            GameObject terrainObj = new GameObject("ProceduralTerrain");
            terrainObj.AddComponent<MeshFilter>().mesh = mesh;
            terrainObj.transform.parent = transform;
            MeshRenderer renderer = terrainObj.AddComponent<MeshRenderer>();
            renderer.material = new Material(Shader.Find("Standard"));
            renderer.material.mainTexture = tex;
            renderer.material.mainTextureScale = new Vector2(50f, 50f);
            terrainObj.AddComponent<MeshCollider>().sharedMesh = mesh;

            _terrainYOffset = -(centerH * height) - 1;
            terrainObj.transform.position = new Vector3(0, _terrainYOffset, 0);

            GameObject waterObj = GameObject.CreatePrimitive(PrimitiveType.Plane);
            waterObj.name = "Water";
            waterObj.transform.parent = transform;
            waterObj.transform.position = new Vector3(0, -5f, 0);
            waterObj.transform.localScale = new Vector3(50f, 1f, 50f);

            // i couldnt figure out how to disable backface culling
            GameObject waterBack = GameObject.CreatePrimitive(PrimitiveType.Plane);
            waterBack.name = "WaterBack";
            waterBack.transform.parent = waterObj.transform;
            waterBack.transform.localPosition = Vector3.zero;
            waterBack.transform.localRotation = Quaternion.Euler(180f, 0f, 0f);
            waterBack.transform.localScale = Vector3.one;

            Material waterMat = new Material(Shader.Find("Standard"));
            waterMat.color = new Color(0.1f, 0.4f, 0.8f, 0.6f);
            waterMat.SetFloat("_Mode", 3f);
            waterMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            waterMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            waterMat.SetInt("_ZWrite", 0);
            waterMat.EnableKeyword("_ALPHABLEND_ON");
            waterMat.renderQueue = 3000;
            waterMat.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            byte[] normalBytes = File.ReadAllBytes(Path.Combine(MelonEnvironment.ModsDirectory, "AwesomeTerrain", "water.png"));
            Texture2D normalTex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            ImageConversion.LoadImage(normalTex, normalBytes);
            waterMat.SetTexture("_BumpMap", normalTex);
            waterMat.EnableKeyword("_NORMALMAP");
            waterMat.mainTextureScale = new Vector2(20f, 20f);
            waterMat.SetTextureScale("_BumpMap", new Vector2(20f, 20f));
            waterObj.GetComponent<MeshRenderer>().material = waterMat;
            Object.Destroy(waterObj.GetComponent<Collider>());
            waterObj.AddComponent<WaterAnimator>();

            waterBack.GetComponent<MeshRenderer>().material = waterMat;
            Object.Destroy(waterBack.GetComponent<Collider>());
            waterBack.AddComponent<WaterAnimator>();
        }
    }
}
