using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transition : MonoBehaviour
{
    public void TransitionClick()
    {
        if (PetriNet.isManualSimulation)
        {
            PetriNet.findConnectedPositionManual(this.gameObject);
        }
    }
}
