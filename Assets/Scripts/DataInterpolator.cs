using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataInterpolator : MonoBehaviour
{
    [SerializeField] private DataSO[] data;

    [SerializeField] public static DataSO[] Data { get; private set; }
    public static double[][] Frequencies { get; private set; }
    
    private void Awake()
    {
        Frequencies = new double[82][];

        Data = data;

        for(int i = 0; i < data.Length; i++)
        {
            if (data[i] == null)
            {
                InterpolateMissingData(i);
            }
            else
            {
                Frequencies[i] = data[i].frequencies;
            }
        }
    }

    private void InterpolateMissingData(int i)
    {
        DataSO minData = null, maxData = null;

        for (int q = i - 1; q >= 0; q--)
        {
            if (Data[q] != null)
            {
                minData = Data[q];
                break;
            }
        }

        for (int p = i + 1; p < Data.Length; p++)
        {
            if (Data[p] != null)
            {
                maxData = Data[p];
                break;
            }
        }

        Debug.Log("Min: " + minData + " Max: " + maxData);
        
        InterpolateValues(minData, maxData, i);
    }

    private void InterpolateValues(DataSO minData, DataSO maxData, int index)
    {
        Frequencies[index] = new double[Data[0].frequencies.Length];
        for (int i = 0; i<Data[0].frequencies.Length; i++)
        {
            double mean = (minData.frequencies[i] + maxData.frequencies[i])/2.0;

            Frequencies[index][i] = mean;
        }
    }
}
