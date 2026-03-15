using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
    public class InputBindingUIGenerator : MonoBehaviour
    {
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private RebindActionUI actionRowPrefab;
        [SerializeField] private Transform container;
        [SerializeField] private List<string> actionMapsToExclude = new List<string> { "UI" };

        private void Start()
        {
            LoadBindingOverrides();
            PopulateUI();
        }

        public GameObject PopulateUI()
        {
            foreach (Transform child in container)
            {
                Destroy(child.gameObject);
            }
            
            GameObject firstRowObject = null;
            
            InputActionAsset inputActionAsset = playerInput.actions;

            foreach (InputActionMap map in inputActionAsset.actionMaps)
            {
                if (actionMapsToExclude.Contains(map.name))
                {
                    continue;
                }

                foreach (InputAction action in map.actions)
                {
                    var rowObj = Instantiate(actionRowPrefab, container);
                    var rebindUI = rowObj.GetComponent<RebindActionUI>();
                    rebindUI.Initialize(action, playerInput);
                    
                    if (firstRowObject == null)
                    {
                        firstRowObject = rowObj.gameObject;
                    }
                }
            }
            
            return firstRowObject;
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