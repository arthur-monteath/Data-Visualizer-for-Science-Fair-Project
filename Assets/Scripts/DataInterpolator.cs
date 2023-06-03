using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataInterpolator : MonoBehaviour
{
    public static DataSO[] Data { get; private set; }

    private void Awake()
    {
        for(int i = 0; i < Data.Length; i++)
        {
            if (Data[i] == null)
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
            if (Data[q] != null) minData = Data[q];
        }

        for(int p = i+1; p < Data.Length; p++)
        {
            if (Data[p] != null) maxData = Data[p];
        }
        
        InterpolateValues(minData, maxData, i);
    }

    private void InterpolateValues(DataSO minData, DataSO maxData, int index)
    {
        for(int i = 0; i<Data[index].frequencies.Length; i++)
        {
            int mean = (minData.frequencies[i] + maxData.frequencies[i])/2;

            Data[index].frequencies[i] = mean;
        }
    }
}
