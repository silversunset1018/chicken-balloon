using System.Collections.Generic;
using Fresvii.AppSteroid;
using UnityEngine;
using System.Collections;

public class FresviiSimpleSample : MonoBehaviour
{
    public void ShowGUI()
    {
        FASGui.ShowGUI(FASGui.Mode.All, FASGui.Mode.Forum);
    }   
}
