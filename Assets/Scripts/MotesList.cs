using System.Collections.Generic;
using UnityEngine;
// this script's one and only purpose is to hold the list of all the objects in each level. I tried having this list be a part of MoteScript.cs and have the player be in charge of keeping track of everyone, but it was easier to just have everyone look for this script seperately so in the unfortunate circumstance where the player tragically fails a level, the motes don't get lost and stop interacting with each other.
public class MotesList : MonoBehaviour {
    public static List<MoteScript> Motes = new List<MoteScript>(); // example of public static list from https://stackoverflow.com/questions/3717028/access-list-from-another-class
}