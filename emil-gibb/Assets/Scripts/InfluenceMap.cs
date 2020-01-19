using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfluenceMap 
{
    int x;
    int y;
    float[,] mapScores;
    public InfluenceMap(int sizeX, int sizeY)
    {
        x = sizeX;
        y = sizeY;
        mapScores = new float[x, y];
    }

    public void GenerateInfluenceMap(PlayGrid playGrid, List<Unit> EnemyUnits, List<Unit> FriendlyUnits)
    {
        for(int i = 0; i < x; i++)
        {
            for(int j = 0; j < y; j++)
            {
                mapScores[i, j] = playGrid.DjikstraInfluence(new Vector2Int(i, j));
            }
        }
    }

    public void GetDistance()
    {

    }

    public void GetWeight()
    {

    }
}
