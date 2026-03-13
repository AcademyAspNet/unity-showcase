using System;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [Header("Entity Settings")]

    [field: SerializeField]
    public string Name { get; set; } = "Entity";
}