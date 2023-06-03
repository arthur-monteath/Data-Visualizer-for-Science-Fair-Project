using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataInterpreter : MonoBehaviour
{
    public static DataInterpreter Instance { get; private set; }

    private string[] texts;
    private int[] values;

    int total;

    public string[] GetTexts()
    {
        return DataInterpolator.Data[0].responses;
    }

    /*public int[] GetPercentages()
    {
        int[] percentages = new int[GetValues().Length];

        return a;
    }*/

    /*public int[][] GetValues()
    {
        int X = DataInterpolator.Data.Length; // X = Age
        int Y = Data // Y = Frequency of response
        int[][] values = new int[DataInterpolator.Data.Length][];

        for(int i = 0; i<DataInterpolator.Data.Length)

        return DataInterpolator.Data[];
    }*/


    void Start()
    {

    }

    void Update()
    {
        
    }
}
