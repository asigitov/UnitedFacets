using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LineRendererExtended : MonoBehaviour
{

    public LineRenderer LineRendererToExtend;
    public Vector3[] positions = new Vector3[0];

    void Awake()
    {
        LineRendererToExtend.SetVertexCount(positions.Length);
        for (int i = 0; i < positions.Length; i++)
        {
            LineRendererToExtend.SetPosition(i, positions[i]);
        }
    }

    public void SetVertexCount(int count)
    {
        System.Array.Resize<Vector3>(ref positions, count);
        LineRendererToExtend.SetVertexCount(count);
    }

    public void SetPosition(int index, Vector3 position)
    {
        if (positions.Length > index)
        {
            positions[index] = position;
            LineRendererToExtend.SetPosition(index, position);
        }
    }

    public int GetVertexCount()
    {
        return positions.Length;
    }

    public bool GetPosition(int index, ref Vector3 position)
    {
        if (positions.Length > index)
        {
            position = positions[index];
            return true;
        }
        else
        {
            return false;
        }
    }
}
