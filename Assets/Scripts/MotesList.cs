// from https://stackoverflow.com/questions/3717028/access-list-from-another-class
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotesList : MonoBehaviour {
    public static List<MoteScript> Motes = new List<MoteScript>();
    public void Update() {
        // Debug.Log(Motes.Count);
    }
}