using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;

public class CombineTexMesh : MonoBehaviour
{
    // 使用前需要设置这些
    static string combineRootName = "MeshesToCombine";
    static string combinedGameObjectName = "combined_go";
    //string texPath = "Assets/_textures_for_combine";
    static string combinedPath = "Assets/will_tools/outputs/combined_mesh";
    static string sceneName = "combine_mesh_demo";


    // 合并mesh和贴图
    // 使用要求: 1. mesh的材质使用相同的shader，且都只有mainTex可用 2. 各贴图需设置为 Reader/Write Enabled 3. 顶部要求的路径要填好
#if UNITY_EDITOR
    [MenuItem("Will Tools/Combine Meshes")]

    private static void CombineMeshesAndTextures()
    {

        CombineMesh(SceneManager.GetSceneByName(sceneName));

    }

    private static void CombineMesh(Scene scene)
    {
        GameObject combineGo = new GameObject(combinedGameObjectName);
        GameObject[] allGameObjects = scene.GetRootGameObjects();
        for (int object_index = 0; object_index < allGameObjects.Length; object_index++)
        {
            // 判断场景中是否有标记为要合并的mesh
            if (allGameObjects[object_index].name.Equals(combineRootName))
            {
                MeshFilter[] allFilter = allGameObjects[object_index].GetComponentsInChildren<MeshFilter>();
                MeshRenderer[] allRender = allGameObjects[object_index].GetComponentsInChildren<MeshRenderer>();
                CombineInstance[] combineMeshes = new CombineInstance[allFilter.Length];
                Material[] materials = new Material[allFilter.Length];
                List<Texture2D> textures = new List<Texture2D>();

                for (int i = 0; i < allFilter.Length; i++)
                {
                    materials[i] = allRender[i].sharedMaterial;
                    combineMeshes[i].mesh = allFilter[i].sharedMesh;
                    combineMeshes[i].transform = allFilter[i].transform.localToWorldMatrix;
                    //usList.Add(allFilter[i].sharedMesh.uv);
                    textures.Add((Texture2D)materials[i].mainTexture);
                }

                // 创建用来作为新mesh的mainTexture
                Texture2D mainTexture = null;

                // 新的uv原点
                Rect[] uvs = null;

                // 将指定路径的贴图先合并到一张大贴图中
                if (textures.Count > 0)
                {
                    Texture2D tempTex = new Texture2D(2048, 2048);
                    uvs = tempTex.PackTextures(textures.ToArray(), 0,2048);
                    tempTex.Apply();
                    Texture2D rCombineTex = new Texture2D(tempTex.width, tempTex.height, TextureFormat.ARGB32, false);
                    rCombineTex.SetPixels32(tempTex.GetPixels32());
                    rCombineTex.Apply();
                    byte[] bytes = rCombineTex.EncodeToPNG();
                    File.WriteAllBytes(combinedPath + "/combineTex.png", bytes);
                    AssetDatabase.Refresh(ImportAssetOptions.ImportRecursive);
                    rCombineTex = AssetDatabase.LoadAssetAtPath<Texture2D>(combinedPath + "/combineTex.png");
                    mainTexture = rCombineTex;
                }

                // 合并mesh
                MeshFilter combineFilter = combineGo.AddComponent<MeshFilter>();
                MeshRenderer combineRender = combineGo.AddComponent<MeshRenderer>();
                combineFilter.sharedMesh = new Mesh();
                combineFilter.sharedMesh.CombineMeshes(combineMeshes);
                Vector2[] newUV = new Vector2[combineFilter.sharedMesh.vertices.Length];

                // 为新mesh重新设置uv
                int count = 0;
                for (int filter_index = 0; filter_index < allFilter.Length; filter_index++)
                {
                    float scaleX = ((float)(textures[filter_index].width) / mainTexture.width);
                    float scaleY = ((float)(textures[filter_index].height) / mainTexture.height);
                    for (int j = 0; j < allFilter[filter_index].sharedMesh.vertices.Length; j++)
                    {
                        newUV[count] = new Vector2((float)(uvs[filter_index].xMin + allFilter[filter_index].sharedMesh.uv[j].x * scaleX),
                                                   (float)(uvs[filter_index].yMin + allFilter[filter_index].sharedMesh.uv[j].y * scaleY));
                        count++;
                    }
                }
                combineFilter.sharedMesh.uv = newUV;

                // 为新mesh设置material
                Material material = Instantiate(materials[0]);
                material.shader = materials[0].shader;
                material.mainTexture = mainTexture;
                combineRender.sharedMaterial = material;

                // 将mesh和material保存到文件系统
                AssetDatabase.CreateAsset(material, combinedPath + "/material.mat");
                AssetDatabase.CreateAsset(combineFilter.sharedMesh, combinedPath + "/mesh.asset");
                combineGo.transform.SetParent(allGameObjects[object_index].transform);
            }
        }
    }


    private static void CombineTex_Mesh()
    {
        // 从本地加载贴图并合并贴图
        //List<Texture2D> textures = new List<Texture2D>();

        //string[] rGuids = AssetDatabase.FindAssets("t:Texture2D", new string[] { texPath });
        //for (int guid = 0; guid < rGuids.Length; guid++)
        //{
        //    string assetPath = AssetDatabase.GUIDToAssetPath(rGuids[guid]);
        //    Texture2D rTex = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath) as Texture2D;
        //    textures.Add(rTex);
        //}
        //if (textures.Count > 0)
        //{
        //    // 将指定路径的贴图先合并到一张大贴图中
        //    Texture2D tempTex = new Texture2D(2048, 2048);
        //    Rect[] uvs = tempTex.PackTextures(textures.ToArray(), 0);
        //    tempTex.Apply();
        //    Texture2D rCombineTex = new Texture2D(tempTex.width, tempTex.height, TextureFormat.ARGB32, false);
        //    rCombineTex.SetPixels32(tempTex.GetPixels32());
        //    rCombineTex.Apply();
        //    byte[] bytes = rCombineTex.EncodeToPNG();
        //    File.WriteAllBytes(combindPath + "/combineTex.png", bytes);
        //    AssetDatabase.Refresh(ImportAssetOptions.ImportRecursive);
        //    rCombineTex = AssetDatabase.LoadAssetAtPath<Texture2D>(combindPath + "/combineTex.png");

        //}

    }


    #endif


}
     