using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class AgeSlider : MonoBehaviour
{
    [SerializeField] private Slider minSlider, maxSlider;

    private void Update()
    {
        if(minSlider.value < maxSlider.value+4)
        {
            minSlider.value = maxSlider.value+4;
        }
    }
}
