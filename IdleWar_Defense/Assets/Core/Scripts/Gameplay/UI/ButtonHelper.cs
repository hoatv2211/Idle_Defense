using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Components;

public class ButtonHelper : MonoBehaviour
{
    [SerializeField] private GameObject iconOn;
    [SerializeField] private GameObject iconOff;
    public void ChangeValue(bool isOn)
    {
        iconOn?.SetActive(isOn);
        iconOff?.SetActive(!isOn);
    }
}
