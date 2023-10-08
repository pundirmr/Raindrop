using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Lumos/Raindrops/Settings Asset", fileName = "SettingsAsset")]
public class SettingsAsset : ScriptableObject
{
    public float baseDropletSpeed;
    public float baseSpawnPeriod;

    public int lives;
    public int pointsPerDroplet;
    public int sunOccuranceFrequency;
}
