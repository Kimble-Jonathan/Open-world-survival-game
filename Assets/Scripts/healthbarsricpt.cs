using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class healthbarsricpt : MonoBehaviour
{

    public Slider slider;
    public void setMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
    }
    
    public void setHeath(int health)
    {
        slider.value = health;
    }
}
