//==================================================================================================
//
// TerrainGenerator v.1 - To generate (natural looking) height maps of terrain.
//
// Copyright (C) 2008  Leo Vandriel
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
//==================================================================================================

using System;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace TerrainGenerator
{
    // HeightMap consists a float 2D array containing the height map and some functionality to
    // access, up-sample, down-sample, and generate rendering data.
    class HeightMap
    {
        // set up mask for sharpening, used in up-sampling
        static float[] maskSharp = new float[] { -1, 9, 9, -1 };

        // set up mask for smoothing, used in down-sampling
        static float[] maskSmooth = new float[] { 1, 2, 1 };

        // height map data
        private float[,] heightMap = null;

        // statistical variance of height data
        private float variance = 0;
        public float Variance
        {
            get { return variance; }
        }

        // vertex buffer data
        private CustomVertex.PositionNormalTextured[] vertices = null;
        public CustomVertex.PositionNormalTextured[] Vertices
        {
            get { return vertices; }
        }

        // index buffer data
        private short[] indices = null;
        public short[] Indices
        {
            get { return indices; }
        }

        public int VerticesCount
        {
            get { return vertices.Length; }
        }

        public int IndicesCount
        {
            get { return indices.Length; }
        }

        public int Width
        {
            get { return heightMap.GetLength(0); }
        }

        public int Height
        {
            get { return heightMap.GetLength(1); }
        }


        // constructor

        public HeightMap()
        {
            heightMap = new float[,] { { 0f, 0f }, { 0f, 0f }, };
        }

        // manually set data
        public void SetMap(float[,] map)
        {
            heightMap = map;
        }

        // create a heightMap with more detail including some random variance, up-sample double size
        public void UpSample(float[] DNA)
        {
            if (heightMap == null)
                return;

            int width = heightMap.GetLength(0) * 2 - 1;
            int height = heightMap.GetLength(1) * 2 - 1;
            int maskSize = maskSharp.Length;

            float[,] newMap = new float[width, height];

            // normalize maskSharp
            float maskSum = 0;
            for (int i = 0; i < maskSize; i++)
            {
                maskSum += maskSharp[i];
            }
            for (int i = 0; i < maskSize; i++)
            {
                maskSharp[i] /= maskSum;
            }

            // set up mask2D for sharpening
            float[,] maskSharp2D = new float[maskSize, maskSize];
            for (int y = 0; y < maskSize; y++)
                for (int x = 0; x < maskSize; x++)
                {
                    maskSharp2D[x, y] = maskSharp[x] * maskSharp[y];
                }

            // the amount of random noise to be added to the height map
            int level = -2;
            for (int size = width > height ? height : width; size > 0; size = size >> 1)
                level++;
            float randomness = Toolbox.Math.Tan(1.2f * DNA[level % DNA.Length]);

            // up-sample
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                {
                    newMap[x, y] = 0;

                    if (x % 2 == 0 && y % 2 == 0)
                    {
                        // just copy originals
                        newMap[x, y] = heightMap[x / 2, y / 2];
                    }
                    else if (x % 2 == 0)
                    {
                        if (y > 1 && y < height - 2)
                        {
                            // use 1D sharpening mask
                            for (int i = 0; i < maskSize; i++)
                            {
                                newMap[x, y] +=
                                    heightMap[x / 2, (y + i * 2 - maskSize + 1) / 2] * maskSharp[i];
                            }
                        }
                        else
                        {
                            newMap[x, y] = .5f
                                * (heightMap[x / 2, (y - 1) / 2] + heightMap[x / 2, (y + 1) / 2]);
                        }
                    }
                    else if (y % 2 == 0)
                    {
                        if (x > 1 && x < width - 2)
                        {
                            // use 1D sharpening mask
                            for (int i = 0; i < maskSize; i++)
                            {
                                newMap[x, y] +=
                                    heightMap[(x + i * 2 - maskSize + 1) / 2, y / 2] * maskSharp[i];
                            }
                        }
                        else
                        {
                            newMap[x, y] = .5f
                                * (heightMap[(x - 1) / 2, y / 2] + heightMap[(x + 1) / 2, y / 2]);
                        }
                    }
                    else
                    {
                        if (x > 1 && x < width - 2 && y > 1 && y < height - 2)
                        {
                            // use 2D sharpening mask
                            for (int masky = 0; masky < maskSize; masky++)
                                for (int maskx = 0; maskx < maskSize; maskx++)
                                {
                                    newMap[x, y] +=
                                        heightMap[  (x + maskx * 2 - maskSize + 1) / 2,
                                                    (y + masky * 2 - maskSize + 1) / 2]
                                        * maskSharp2D[maskx, masky];
                                }
                        }
                        else
                        {
                            newMap[x, y] = .25f * (
                                  heightMap[(x - 1) / 2, (y - 1) / 2]
                                + heightMap[(x + 1) / 2, (y - 1) / 2]
                                + heightMap[(x - 1) / 2, (y + 1) / 2]
                                + heightMap[(x + 1) / 2, (y + 1) / 2]);
                        }
                    }

                    // correct variance
                    newMap[x, y] *= 2f;

                    // apply noise
                    newMap[x, y] += randomness * Toolbox.Random.Float();
                }

            // set new height map data
            heightMap = newMap;

            NormalizeHeightMap();
        }

        // create a heightMap with less detail, down-sample to half size
        public void DownSample()
        {
            if (heightMap == null)
                return;

            int width = (heightMap.GetLength(0) + 1);
            int height = (heightMap.GetLength(1) + 1);
            int maskSize = maskSmooth.Length;
            int halfMaskSize = maskSize / 2;

            if (width % 2 != 0 || height % 2 != 0)
                return;

            width /= 2;
            height /= 2;

            float[,] newMap = new float[width, height];

            // normalize maskSmooth
            float maskSum = 0;
            for (int i = 0; i < maskSize; i++)
            {
                maskSum += maskSmooth[i];
            }
            for (int i = 0; i < maskSize; i++)
            {
                maskSmooth[i] /= maskSum;
            }

            // set up maskSmooth2D for smoothing
            float[,] maskSmooth2D = new float[maskSize, maskSize];
            for (int y = 0; y < maskSize; y++)
                for (int x = 0; x < maskSize; x++)
                {
                    maskSmooth2D[x, y] = maskSmooth[x] * maskSmooth[y];
                }

            // down-sample
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                {
                    newMap[x, y] = 0;
                    for (int masky = 0; masky < maskSize; masky++)
                        for (int maskx = 0; maskx < maskSize; maskx++)
                        {
                            int tempx = 2 * x + maskx - halfMaskSize,
                                tempy = 2 * y + masky - halfMaskSize;
                            if (tempx >= 0 && tempx < heightMap.GetLength(0)
                                && tempy >= 0 && tempy < heightMap.GetLength(1))
                            {
                                newMap[x, y] += .5f * heightMap[tempx, tempy]
                                    * maskSmooth2D[maskx, masky];
                            }
                        }
                }

            // set new height map data
            heightMap = newMap;

            NormalizeHeightMap();
        }

        // translate (no scaling) height map to average height == 0, store variance
        public void NormalizeHeightMap()
        {
            int width = heightMap.GetLength(0);
            int height = heightMap.GetLength(1);

            float sum = 0;
            float sumsq = 0;

            // calculate height average and variance
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                {
                    sum += heightMap[x, y];
                    sumsq += heightMap[x, y] * heightMap[x, y];
                }
            sum /= width * height;
            sumsq /= width * height;
            variance = (sumsq - sum * sum) / width / height;

            // translate mapping
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                {
                    heightMap[x, y] -= sum;
                }
        }

        // compute render data; vertex and index buffer
        public void CalcVerticesIndices(float terrainSize)
        {
            int width = Width;
            int height = Height;
            int max = width > height ? width : height;
            float scale = terrainSize / (float)(max - 1);

            // vertices
            vertices = new CustomVertex.PositionNormalTextured[width * height];
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                {
                    int arrayIndex = y * width + x;
                    Vector3 position = new Vector3(
                        (float)x * scale, (float)heightMap[x, y] * scale, (float)y * scale);
                    vertices[arrayIndex] = new CustomVertex.PositionNormalTextured(
                        position, new Vector3(), x / (float) width, y/(float) height);
                }

            // indices and normals
            indices = new short[(width - 1) * (height - 1) * 6];
            for (int y = 0; y < (height - 1); y++)
                for (int x = 0; x < (width - 1); x++)
                {
                    int arrayIndex = (y * (width - 1) + x) * 6;
                    int vertexIndex = y * width + x;

                    indices[arrayIndex] = (short)vertexIndex;
                    indices[arrayIndex + 1] = (short)(vertexIndex + 1);
                    indices[arrayIndex + 2] = (short)(vertexIndex + width);
                    indices[arrayIndex + 3] = (short)(vertexIndex + width);
                    indices[arrayIndex + 4] = (short)(vertexIndex + 1);
                    indices[arrayIndex + 5] = (short)(vertexIndex + width + 1);
                }

            int faces = indices.Length / 3;
            int verts = vertices.Length;

            Vector3[] vertexNormals = new Vector3[verts];
            for (int i = 0; i < verts; i++)
            {
                vertexNormals[i] = new Vector3(0, 0, 0);
            }

            // compute the face normals as average of adjacent edges normals (yes, that is primitive)
            for (int i = 0; i < faces; i++)
            {
                Vector3 edge0 = vertices[indices[i * 3]].Position
                    - vertices[indices[i * 3 + 1]].Position;
                Vector3 edge1 = vertices[indices[i * 3 + 1]].Position
                    - vertices[indices[i * 3 + 2]].Position;

                Vector3 faceNormal = Vector3.Normalize(Vector3.Cross(edge1, edge0));

                vertexNormals[indices[i * 3]] += faceNormal;
                vertexNormals[indices[i * 3 + 1]] += faceNormal;
                vertexNormals[indices[i * 3 + 2]] += faceNormal;
            }

            // normalize to normals
            for (int i = 0; i < verts; i++)
            {
                vertices[i].Normal = Vector3.Normalize(vertexNormals[i]);
            }
        }
    }
}
