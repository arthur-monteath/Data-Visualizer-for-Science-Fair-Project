using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class DataSO : ScriptableObject
{
    public int age;

    public string[] responses;
    public int[] frequencies;
}
