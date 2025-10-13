// Attach this script to your ScrollView's Content object or a manager object

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
    public class InputBindingUIGenerator : MonoBehaviour
    {
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private GameObject actionRowPrefab;
        [SerializeField] private Transform container;
        [SerializeField] private List<string> actionMapsToExclude = new List<string> { "UI" };

        private void Start()
        {
            LoadBindingOverrides();
            PopulateUI();
        }

        private void PopulateUI()
        {
            foreach (Transform child in container)
            {
                Destroy(child.gameObject);
            }

            InputActionAsset inputActionAsset = playerInput.actions;

            foreach (InputActionMap map in inputActionAsset.actionMaps)
            {
                if (actionMapsToExclude.Contains(map.name))
                {
                    continue;
                }

                foreach (InputAction action in map.actions)
                {
                    GameObject rowObj = Instantiate(actionRowPrefab, container);
                    var rebindUI = rowObj.GetComponent<RebindActionUI>();
                
                    // *** THE CHANGE IS HERE ***
                    // Pass both the action AND the playerInput reference
                    rebindUI.Initialize(action, playerInput);
                }
            }
        }
    
        private void LoadBindingOverrides()
        {
            string rebinds = PlayerPrefs.GetString("keyRebinds");
            if (!string.IsNullOrEmpty(rebinds))
            {
                playerInput.actions.LoadBindingOverridesFromJson(rebinds);
            }
        }
    }
}