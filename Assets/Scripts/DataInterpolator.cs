using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataInterpolator : MonoBehaviour
{
    [SerializeField] private DataSO[] data;

    [SerializeField] public static DataSO[] Data { get; private set; }

    private void Awake()
    {
        Data = data;

        for(int i = 0; i < Data.Length; i++)
        {
            if (Data[i] == null)
            {
                Debug.Log(i);
                InterpolateMissingData(i);
            }
        }
    }

    private void InterpolateMissingData(int i)
    {
        DataSO minData = null, maxData = null;

        for(int q = i-1; q >= 0; q--)
        {
            if (Data[q] != null) minData = Data[q];
        }

        for(int p = i+1; p < Data.Length; p++)
        {
            if (Data[p] != null) maxData = Data[p];
        }

        Debug.Log("Min: " + minData + " Max: " + maxData);
        
        InterpolateValues(minData, maxData, i);
    }

    private void InterpolateValues(DataSO minData, DataSO maxData, int index)
    {
        Data[index] = ScriptableObject.CreateInstance<DataSO>();

        data[index].age = index + 5;

        Data[index].responses = Data[0].responses;

        for (int i = 0; i<Data[0].frequencies.Length; i++)
        {
            double mean = (minData.frequencies[i] + maxData.frequencies[i])/2.0;

            Data[index].frequencies[i] = mean;
        }
    }
}
