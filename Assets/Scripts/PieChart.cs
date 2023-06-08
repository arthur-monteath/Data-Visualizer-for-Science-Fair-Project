using Min_Max_Slider;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PieChart : MonoBehaviour
{
    [SerializeField] private Color[] colors;

    [SerializeField] private Transform[] chartPieces;
    [SerializeField] private Transform[] chartTexts;

    private void Start()
    {
        Refresh(5,86);
    }

    public void Refresh(System.Single minAge, System.Single maxAge)
    {
        int range = (int)(maxAge - minAge);

        int length = DataInterpolator.Data[0].frequencies.Length;

        double[] initialPercentages = new double[length];

        for (int k = 0; k < length; k++)
        {
            double sum = 0.0;

            Debug.Log("minAge:" + minAge + " | maxAge:" + maxAge);

            for (int i = (int)minAge-5; i <= maxAge-5; i++)
            {
                Debug.Log("i: " + i + " | k: " + k);
                sum += DataInterpolator.Frequencies[i][k];
            }

            initialPercentages[k] = sum / range;
        }

        double[] finalPercentages = new double[length];

        double sumOfPercentages = 0.0;

        for(int i = 0; i < length; i++)
        {
            sumOfPercentages += initialPercentages[i];
        }

        for(int i = 0; i < length; i++)
        {
            finalPercentages[i] = initialPercentages[i] / sumOfPercentages;
        }

        double percentagesUsed = 0.0;

        for (int i = 0; i < length; i++)
        {
            float rotation = (float)(percentagesUsed * 360);

            percentagesUsed += finalPercentages[i];

            Debug.Log(finalPercentages[i]);

            Transform chartPieceInstance = chartPieces[i];

            chartPieceInstance.rotation = Quaternion.Euler(new Vector3(0, 0, -rotation));

            chartPieceInstance.GetComponent<Image>().fillAmount = (float)finalPercentages[i];

            chartPieceInstance.GetComponent<Image>().color = colors[i];

            Transform chartText = chartTexts[i];

            chartText.GetComponent<Text>().text = "" + DataInterpolator.Data[0].responses[i] + " => " + Math.Round(finalPercentages[i]*100, 2) + "%";

            chartText.GetComponentInChildren<Image>().color = colors[i];

            chartText.gameObject.SetActive(true);

            chartPieceInstance.gameObject.SetActive(true);
        }
    }
}
