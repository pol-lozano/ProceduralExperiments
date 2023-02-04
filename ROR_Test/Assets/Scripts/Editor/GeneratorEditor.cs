using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public struct GeneratorSetting
{
    public float scaleX, scaleZ;
    public float minHeight, maxHeight;
}

public class GeneratorEditor : EditorWindow
{
    public GameObject terrain;
    public int sizeX, sizeZ;
    public int scale;
    public List<GeneratorSetting> terrainSettings;

    //Terrain Mesh stuff
    private Mesh mesh;

    private Vector3[] vertices;
    private Vector2[] uvs;
    private Color[] colors;
    private int[] triangles;

    [MenuItem("Window/Generator Editor")]
    static void Init()
    {
        GetWindow(typeof(GeneratorEditor)).Show();
    }

    private void OnGUI()
    {
        terrain = EditorGUILayout.ObjectField("terrain", terrain, typeof(GameObject), true) as GameObject;
        sizeX = EditorGUILayout.IntField("sizeX", sizeX);
        sizeZ = EditorGUILayout.IntField("sizeZ", sizeZ);
        scale = EditorGUILayout.IntField("scale", scale);

        Field("terrainSettings");

        if (GUILayout.Button("Create Terrain"))
        {
            MakeTerrain();
        }
    }

    private void MakeTerrain()
    {
        UnityEngine.Random.InitState(124354166);
        terrain = terrain ? terrain : new GameObject("Terrain");
        if (!terrain.GetComponent<MeshRenderer>())
        {
            terrain.AddComponent<MeshRenderer>();
        }

        MeshFilter filter = terrain.GetComponent<MeshFilter>();
        if (!filter)
        {
            filter = terrain.AddComponent<MeshFilter>();
        }

        if (!mesh)
        {
            mesh = new Mesh();
        }

        filter.mesh = mesh;

        vertices = new Vector3[(sizeX + 1) * (sizeZ + 1)];
        uvs = new Vector2[vertices.Length];

        for (int i = 0, z = 0; z <= sizeZ; z++)
        {
            for (int x = 0; x <= sizeX; x++)
            {
                float y = CalcHeight(x, z);
                vertices[i] = new Vector3(x, y, z) * scale;
                uvs[i] = new Vector2(x / (float)sizeX, z / (float)sizeZ);

                i++;
            }
        }

        TriangulateMesh();
        UpdateMesh();
    }

    private void TriangulateMesh()
    {
        mesh.vertices = vertices;
        triangles = new int[sizeX * sizeZ * 6];

        int tris = 0;
        int vert = 0;

        for (int z = 0; z < sizeZ; z++)
        {
            for (int x = 0; x < sizeX; x++)
            {
                triangles[tris++] = vert;
                triangles[tris++] = vert + sizeX + 1;
                triangles[tris++] = vert + 1;
                triangles[tris++] = vert + 1;
                triangles[tris++] = vert + sizeX + 1;
                triangles[tris++] = vert + sizeX + 2;
                vert++;
            }
            vert++;
        }
    }

    private void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.colors = colors;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        MeshCollider meshCollider = terrain.GetComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;
    }

    private float CalcHeight(int x, int z)
    {
        float y = 0;
        foreach (GeneratorSetting setting in terrainSettings)
        {
            y += CalcHeight(x, z, setting);
        }
        return y;
    }
    private float CalcHeight(int x, int z, GeneratorSetting setting)
    {
        float xCoord = x * setting.scaleX;
        float zCoord = z * setting.scaleZ;
        float noise = Mathf.PerlinNoise(xCoord, zCoord);
        float y = Mathf.Lerp(setting.minHeight, setting.maxHeight, noise);
        return y;
    }

    private void Field(string property)
    {
        SerializedObject serializedObject = new SerializedObject(this);
        SerializedProperty serializedProperty = serializedObject.FindProperty(property);
        EditorGUILayout.PropertyField(serializedProperty, true);
        serializedObject.ApplyModifiedProperties();
    }
}
