using UnityEngine;
using System.Collections.Generic;

[ExecuteAlways]
public class ScaleVolumeSpawner : MonoBehaviour
{
    public GameObject[] prefabs;
    public int count = 100;
    public float minDistance = 2f;
    public bool autoUpdate = true;

    private List<GameObject> spawned = new List<GameObject>();
    private Vector3 lastScale;
    private Vector3 lastPosition;

    void Update()
    {
        if (!autoUpdate) return;
        if (Application.isPlaying) return;

        if (transform.localScale != lastScale || transform.position != lastPosition)
        {
            Generate();
            lastScale = transform.localScale;
            lastPosition = transform.position;
        }
    }

    public void Generate()
    {
        Clear();

        if (prefabs == null || prefabs.Length == 0)
            return;

        Vector3 center = transform.position;
        Vector3 size = transform.localScale;

        List<Vector3> usedPositions = new List<Vector3>();

        for (int i = 0; i < count; i++)
        {
            Vector3 randomPos = new Vector3(
                Random.Range(center.x - size.x / 2, center.x + size.x / 2),
                center.y + 50f,
                Random.Range(center.z - size.z / 2, center.z + size.z / 2)
            );

            RaycastHit hit;
            if (Physics.Raycast(randomPos, Vector3.down, out hit))
            {
                bool tooClose = false;

                foreach (Vector3 pos in usedPositions)
                {
                    if (Vector3.Distance(pos, hit.point) < minDistance)
                    {
                        tooClose = true;
                        break;
                    }
                }

                if (tooClose)
                    continue;

                GameObject prefab = prefabs[Random.Range(0, prefabs.Length)];
                if (prefab == null)
                    continue;

                GameObject obj = Instantiate(prefab, hit.point, Quaternion.Euler(0, Random.Range(0, 360), 0));
                obj.transform.parent = transform;

                spawned.Add(obj);
                usedPositions.Add(hit.point);
            }
        }
    }

    public void Clear()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }

        spawned.Clear();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
}
