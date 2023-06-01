using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataInterpolator : MonoBehaviour
{
    [SerializeField] private DataSO[] data;

    private void Awake()
    {
        for(int i = 0; i < data.Length; i++)
        {
            if (data[i] == null)
            {
                InterpolateMissingData(i);
            }
        }
    }

    private void InterpolateMissingData(int i)
    {
        DataSO minData = null, maxData = null;

        for(int q = i-1; q > 0; q--)
        {
            if (data[q] != null) minData = data[q];
        }

        for(int p = i+1; p < data.Length; p++)
        {
            if (data[p] != null) maxData = data[p];
        }
        
        InterpolateValues(minData, maxData, i);
    }

    private void InterpolateValues(DataSO minData, DataSO maxData, int index)
    {

    }
}
