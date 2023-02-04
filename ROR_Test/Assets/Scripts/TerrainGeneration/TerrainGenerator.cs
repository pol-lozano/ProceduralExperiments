using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct GeneratorSetting
{
    public float scaleX, scaleZ;
    public float minHeight, maxHeight;
}

public class TerrainGenerator : MonoBehaviour
{
    [SerializeField][Range(1,256)] private int width, height;
    [SerializeField] private int scale;
    [SerializeField] private string seed;
    public List<GeneratorSetting> terrainSettings;

    [Header("References")]
    public MeshFilter meshFilter;
    public MeshCollider meshCollider;

    [ContextMenu("Generate")]
    public void GenerateMap()
    {
        float[] noise = NoiseGenerator.GenerateNoiseMap(width, height, seed.GetHashCode(),terrainSettings);
        Mesh mesh = MeshGenerator.GenerateTerrainMesh(width, height, scale, noise).GenerateMesh();
        meshFilter.sharedMesh = mesh;
        meshCollider.sharedMesh = mesh;
    }
}