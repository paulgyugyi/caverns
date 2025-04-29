using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClusterHex
{
    public Cluster cluster;
    public int hex;

    public ClusterHex(Cluster cluster, int hex)
    {
        this.cluster = cluster;
        this.hex = hex;
    }
}
