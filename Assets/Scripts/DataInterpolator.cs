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
            if (data[i] != null)
            {
                Interpolate(i);
            }
        }
    }

    private void Interpolate(int i)
    {

    }
}
