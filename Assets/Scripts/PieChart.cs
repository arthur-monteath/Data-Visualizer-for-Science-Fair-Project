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

        for (int k = 0; k < range; k++)
        {
            double sum = 0.0;

            for (int i = (int)minAge; i <= maxAge; i++)
            {
                sum += DataInterpolator.Data[i].frequencies[k];
            }

            finalPercentages[k] = sum / length;
        }

        double percentagesUsed = 0.0;

        for (int i = 0; i < range; i++)
        {
            float rotation = (float)(percentagesUsed * 360);

            percentagesUsed += finalPercentages[i];

            Transform chartPiece = Instantiate(this.chartPiece);

            chartPiece.rotation.SetEulerAngles(0, 0, rotation);

            chartPiece.GetComponent<Image>().fillAmount = (float)finalPercentages[i];

            chartPiece.gameObject.SetActive(true);
        }
    }
}
