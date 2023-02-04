using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NoiseGenerator
{
    public static float[] GenerateNoiseMap(int width, int height, int seed, List<GeneratorSetting> terrainSettings)
    {
        float[] noiseMap = new float[width * height];
        System.Random prng = new System.Random(seed);
        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                noiseMap[z * width + x] = CalcHeight(x, z, terrainSettings);             
            }
        }
        return noiseMap;
    }

    private static float CalcHeight(int x, int z, List<GeneratorSetting> terrainSettings)
    {
        float y = 0;
        foreach (GeneratorSetting setting in terrainSettings)
        {
            y += CalcHeight(x, z, setting);
        }
        return y;
    }

    private static float CalcHeight(int x, int z, GeneratorSetting setting)
    {
        float xCoord = x * setting.scaleX;
        float zCoord = z * setting.scaleZ;
        float noise = Mathf.PerlinNoise(xCoord, zCoord);
        float y = Mathf.Lerp(setting.minHeight, setting.maxHeight, noise);
        return y;
    }
}
