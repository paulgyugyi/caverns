using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice
{
    public static int Roll(int numDice)
    {
        int result = 0;

        for (int i = 0; i < numDice; i++)
        {
            result += Random.Range(1, 5);
        }
        return result;
    }
}
