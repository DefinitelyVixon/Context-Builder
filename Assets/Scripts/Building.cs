using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{

    public enum Type {Residential, Commercial, Industrial}
    
    public bool isActive;
    public int level;

    public List<Building> connectedInputs;
    public List<Building> connectedOutputs;
    
    void Start()
    {
        connectedInputs = new List<Building>();
        connectedOutputs = new List<Building>();
        level = 1;
        isActive = true;
    }
    
}

public class ResidentialBuilding : Building 
{
    void Update()
    {
                
    }
}
public class CommercialBuilding : Building
{
    void Update()
    {
                
    }
}
public class IndustrialBuilding : Building
{
    void Update()
    {
                
    }
}
