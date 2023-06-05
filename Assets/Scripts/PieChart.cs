using Min_Max_Slider;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PieChart : MonoBehaviour
{
    [SerializeField] private Transform chartPiece;

    private void Start()
    {
        Refresh(5,86);
    }

    public void Refresh(System.Single minAge, System.Single maxAge)
    {
        int range = (int)(maxAge - minAge);

        int length = DataInterpolator.Data[0].frequencies.Length;

        double[] finalPercentages = new double[length];

        for (int k = 0; k < length; k++)
        {
            double sum = 0.0;

            Debug.Log("minAge:" + minAge + " | maxAge:" + maxAge);

            for (int i = (int)minAge-5; i <= maxAge-5; i++)
            {
                Debug.Log("i: " + i + " | k: " + k);
                sum += DataInterpolator.Frequencies[i][k];
            }

            finalPercentages[k] = sum / length;
        }

        double percentagesUsed = 0.0;

        for (int i = 0; i < length; i++)
        {
            float rotation = (float)(percentagesUsed * 360);

            percentagesUsed += finalPercentages[i];

            Transform chartPieceInstance = Instantiate(this.chartPiece);

            chartPieceInstance.rotation.SetEulerAngles(0, 0, rotation);

            chartPieceInstance.GetComponent<Image>().fillAmount = (float)finalPercentages[i];

            chartPieceInstance.gameObject.SetActive(true);
        }
    }
}
