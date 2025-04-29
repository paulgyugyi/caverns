using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentCluster : Cluster
{
    public Dictionary<Equipment, int> allEquipment = new Dictionary<Equipment, int>();
    public Dictionary<int, Equipment> hexEquipment = new Dictionary<int, Equipment>();
    public Dictionary<int, ClusterHex> hexEquipmentShape = new Dictionary<int, ClusterHex>();
    public Stats.StatType actionStat;

    public bool AutoAdd(Equipment equipment)
    {
        //Debug.Log("Trying to find a place for " + equipment.Name);
        bool[] slotNeeded = equipment.Shape.validHexes;
        // Try to fit at each position
        int fitHex = -1;
        for (int center = 0; center < Hexmap.ClusterSize; center++)
        {
            //Debug.Log("Trying slot " + center);
            if (TestFit(equipment, center))
            {
                //Debug.Log(equipment.Name + " fits at slot 0!");
                fitHex = center;
                break;
            }
        }
        if (fitHex == -1)
        {
            //Debug.LogWarning(equipment.Name + " did not fit at any location!");
            return false;
        }
        Add(equipment, fitHex);
        return true;
    }

    public void Add(Equipment equipment, int centerSlot)
    {
        //Debug.Log("Adding " + equipment.Name + " at slot " + centerSlot);
        bool[] spaceUsed = equipment.Shape.validHexes;
        for (int space = 0; space < Hexmap.ClusterSize; space++)
        {
            if (!spaceUsed[space])
            {
                //Debug.Log("skipping used space  " + space);
                continue;
            }
            // determine action cluster slot corresponding to equipment space
            int slot;
            if (Hexmap.InBounds(centerSlot, space, out slot))
            {
                //Debug.Log("occuping slot " + slot + " with space " + space);
                hexEquipment[slot] = equipment;
                hexEquipmentShape[slot] = new ClusterHex(equipment.Shape, space);
            }
            else
            {
                Debug.LogWarning("Space " + space + " is outside of cluster bounds, rejected.");
                return;
            }
        }
        allEquipment[equipment] = centerSlot;
    }

    public void RemoveAll()
    {
        foreach ((Equipment equipment, int centerSlot) in  allEquipment) {
            Remove(equipment);
        }
    }

    public void Remove(Equipment equipment)
    {
        int centerSlot = allEquipment[equipment];
        //Debug.Log("Removing " + equipment.Name + " from slot " + centerSlot);
        bool[] spaceUsed = equipment.Shape.validHexes;
        for (int space = 0; space < Hexmap.ClusterSize; space++)
        {
            if (!spaceUsed[space])
            {
                //Debug.Log("skipping used space  " + space);
                continue;
            }
            // determine action cluster slot corresponding to equipment space
            int slot;
            if (Hexmap.InBounds(centerSlot, space, out slot))
            {
                //Debug.Log("emptying slot " + slot + " from space " + space);
                hexEquipment.Remove(slot);
                hexEquipmentShape.Remove(slot);
            }
            else
            {
                Debug.LogWarning("Space " + space + " is outside of cluster bounds, rejected.");
            }
        }
        allEquipment.Remove(equipment);
    }

    public bool TestFit(Equipment equipment, int centerSlot)
    {
        //Debug.Log("Test fitting " + equipment.Name + " at slot " + centerSlot);
        bool[] spaceUsed = equipment.Shape.validHexes;
        for (int space = 0; space < Hexmap.ClusterSize; space++)
        {
            if (!spaceUsed[space])
            {
                //Debug.Log("skipping used space  " + space);
                continue;
            }

            // determine action cluster slot corresponding to equipment space
            int slot;
            if (Hexmap.InBounds(centerSlot, space, out slot))
            {
                //Debug.Log("Trying slot " + slot + " with space " + space);
                if (!validHexes[slot])
                {
                    // invalid slot
                    return false;
                }
                ClusterHex currentEquipmentShape = null;             
                if (hexEquipmentShape.TryGetValue(slot, out currentEquipmentShape))
                {
                    // occupied slot
                    return false;
                }
            }
            else
            {
                // extends beyond action cluster slots
                //Debug.Log("Space " + space + " is outside of cluster bounds, rejected.");
                return false;
            }
        }
        return true;
    }

    public Dictionary<Equipment, int> GetEquipment()
    {
        return allEquipment;
    }

}
