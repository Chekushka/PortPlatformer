using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class CharacterCreationSystem : MonoBehaviour
{
    
    [Range(0, 11)][SerializeField] private int currentBodyIndex;
    [Range(0, 11)][SerializeField] private int currentHeadIndex;
    [Header("Meshes")]
    [SerializeField] private GameObject[] bodies;
    [SerializeField] private GameObject[] heads;
    

    public void ChangeCharacter()
    {
        
    }
}
