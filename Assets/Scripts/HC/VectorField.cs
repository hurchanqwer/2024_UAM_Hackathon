using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System;
using System.Linq;
using UnityEditor.Build.Content;

public class VectorFieldManager : MonoBehaviour
{
    public TextAsset[] csvFiles;

    public List<Vector3> positions = new List<Vector3>();
    public List<Vector3> velocities = new List<Vector3>();
    public List<float> speeds = new List<float>();
    public List<GameObject> createdVector = new List<GameObject>();
    public GameObject arrowPrefab;
    public Material[] arrowMaterial;
    GameObject parent;
    public bool isInit = true;

    private void Awake()
    {
        for (int i = 0; i < csvFiles.Length; i++)
        {

            ParseCSV(csvFiles[i].text);
        }

        Debug.Log($"Total Positions: {positions.Count}");
        Debug.Log($"Total Velocities: {velocities.Count}");
        Debug.Log($"Total Speeds: {speeds.Count}");

    }
    private void OnDisable()
    {
        if (parent != null)
        {
            parent.SetActive(false);
        }
    }


    public void CreateVectorField()
    {
        if (isInit)
        {
            parent = Instantiate(new GameObject("VectorParent"), Vector3.zero, Quaternion.identity);

           
            for (int j = 0; j < positions.Count; j++)
            {
                float speed = Mathf.Round(speeds[j]);

                if (j % 2 == 0)
                {
                    createdVector.Add(Instantiate(arrowPrefab, positions[j], Quaternion.identity, parent.transform));
                    createdVector[createdVector.Count - 1].transform.GetChild(0).rotation = Quaternion.LookRotation(velocities[j]);
                    
                  
                    if (speed > 12)
                    {
                        SetMaterial(3); // arrowMaterial[3] 
                    }
                    else if (speed > 8)
                    {
                        SetMaterial(2); // arrowMaterial[2] 
                    }
                    else if (speed > 4)
                    {
                        SetMaterial(1); // arrowMaterial[1] 
                    }
                    else if (speed > 0)
                    {
                        SetMaterial(0); // arrowMaterial[0] 
                    }
                }
            }
        }
        else { parent.SetActive(true); }

        isInit = false; 
    }
    void ParseCSV(string csvText)
    {
       
        string[] lines = System.Text.RegularExpressions.Regex.Split(csvText, "\r\n|\r|\n");
        int header = 0;
        for (int j = 0; j < lines.Length; j++)
        {
            if (string.IsNullOrWhiteSpace(lines[j])) continue;

       
            if (header < 2)
            {
                header++;
                continue;
            }

            string[] values = lines[j].Split(','); 

            if (values.Length < 8) continue; 
            // xzy -> xyz
            if (float.TryParse(values[1], out float x) &&
                float.TryParse(values[3], out float y) &&
                float.TryParse(values[2], out float z) &&
                float.TryParse(values[4], out float vx) &&
                float.TryParse(values[6], out float vy) &&
                float.TryParse(values[5], out float vz) &&
                float.TryParse(values[7], out float speed))
            {
                positions.Add(transform.position + new Vector3(x, y, z));
                velocities.Add(new Vector3(vx, vy, vz));
                speeds.Add(speed);
            }
            else
            {
                Debug.LogWarning($"Invalid data format in line: {lines[j]}");
            }
        }

    }
    void SetMaterial(int materialIndex)
    {
        // Ensure index is within the valid range
        if (createdVector.Count > 0 && materialIndex >= 0 && materialIndex < arrowMaterial.Length)
        {
            createdVector[createdVector.Count - 1].transform.GetChild(0).GetComponent<MeshRenderer>().material = arrowMaterial[materialIndex];
        }
        else
        {
            Debug.LogWarning("Invalid material index or createdVector is empty.");
        }
    }
    public void VectorVisiableMode()
    {
        SystemManager.Instance.isVisiable = !SystemManager.Instance.isVisiable;
        for (int i = 0; i < createdVector.Count; i++)
        {
            createdVector[i].transform.GetChild(0).gameObject.SetActive(SystemManager.Instance.isVisiable);
        }
    }
 
    
}
