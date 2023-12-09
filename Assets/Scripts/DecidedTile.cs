using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecidedTile : MonoBehaviour
{
    [SerializeField] private int commonality = 1;
    public int Commonality => commonality;
    
    [Header("Neighbors")]
    
    [SerializeField] private bool hasTop;
    public bool HasTop => hasTop;
    [SerializeField] private bool hasBottom;
    public bool HasBottom => hasBottom;
    [SerializeField] private bool hasLeft;
    public bool HasLeft => hasLeft;
    [SerializeField] private bool hasRight;
    public bool HasRight => hasRight;
}
