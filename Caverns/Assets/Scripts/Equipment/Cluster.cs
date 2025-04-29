using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Cluster
{
    public bool[] validHexes = new bool[Hexmap.ClusterSize];

    public Vector3Int center = Vector3Int.zero;

    public Cluster()
    {
        Init(Hexmap.ClusterSize);
    }

    public Cluster(int capacity)
    {
        Init(capacity);
    }

    public void Init(int hexLimit)
    {
        for (int i = 0; i < Hexmap.ClusterSize; i++)
        {
            if (i < hexLimit)
            {
                validHexes[i] = true;
            }
            else
            {
                validHexes[i] = false;
            }
        }
    }
}
