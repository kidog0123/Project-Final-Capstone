using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SessionSpawnPoints : MonoBehaviour
{

    [SerializeField] private Transform[] points = null;
    private bool initialized = false;
    private int orderedIndex = -1;

    private static SessionSpawnPoints singleton = null;

    public static SessionSpawnPoints Singleton
    {
        get
        {
            if (singleton == null)
            {
                singleton = FindFirstObjectByType<SessionSpawnPoints>();
                singleton.Initialize();
            }
            return singleton;
        }
    }

    private void OnDestroy()
    {
        if (singleton == this)
        {
            singleton = null;
        }
    }

    private void Initialize()
    {
        if (initialized) { return; }
        initialized = true;
        orderedIndex = -1;
    }

    private void OnDrawGizmos()
    {
        if (points != null)
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < points.Length; i++)
            {
                if (points[i] != null)
                {
                    Gizmos.DrawSphere(points[i].position, 0.1f);
                }

            }
        }
    }

    public Vector3 GetSpawnPosition(int index)
    {
        if (index >= 0 && index < points.Length && points[index] != null)
        {
            return points[index].position;
        }
        return Vector3.zero;
    }

    public Vector3 GetSpawnPositionRandom()
    {
        return GetSpawnPosition(UnityEngine.Random.Range(0, points.Length));
    }

    public Vector3 GetSpawnPositionOrdered()
    {
        orderedIndex++;
        if (orderedIndex >= points.Length)
        {
            orderedIndex = 0;
        }
        return GetSpawnPosition(orderedIndex);
    }

}