using UnityEngine;

public static class MeshGenerator {
    public class MeshData
    {
        public Vector3[] Vertices { get; set; }
        public Vector2[] Uvs { get; set; }
        public int[] Triangles { get; set; }

        private int triangleIndex;

        public MeshData(int width, int height)
        {
            Vertices = new Vector3[width * height];
            Uvs = new Vector2[Vertices.Length];
            Triangles = new int[(width - 1) * (height - 1) * 6];
        }

        public void AddTriangle(int a, int b, int c)
        {
            Triangles[triangleIndex] = a;
            Triangles[triangleIndex + 1] = b;
            Triangles[triangleIndex + 2] = c;
            triangleIndex += 3;
        }

        public Mesh GenerateMesh()
        {
            Mesh mesh = new Mesh
            {
                vertices = Vertices,
                triangles = Triangles,
                uv = Uvs
            };
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }
    }

    public static MeshData GenerateTerrainMesh(int width, int height, int scale, float[] heightMap)
    {
        MeshData meshData = new MeshData(width, height);
        for (int i = 0, y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                meshData.Vertices[i] = new Vector3(x, heightMap[y * width + x], y) * scale;
                meshData.Uvs[i] = new Vector2(x / (float)width, y / (float)height);

                if (x < width - 1 && y < height - 1)
                {
                    meshData.AddTriangle(i, i + width , i + 1);
                    meshData.AddTriangle(i + width, i + width + 1, i + 1);
                }
                i++;
            }
        }
        return meshData;
    }
}
