using PhantomPixelStudio.Utility;
using Kisei.Utility;
using UnityEngine;

namespace Kisei.Player
{
    public class PlayerInstanceHUB : Singleton<PlayerInstanceHUB>
    {
        private GameObject mainPlayerObject;
        private PlayerController playerController;
        private PlayerInventory playerInventory;
        private Character playerCharacter;


        protected override void InternalInit()
        {
            if (mainPlayerObject == null) mainPlayerObject = gameObject;

            GetComponents();
            ValidateComponents();
        }
        private void GetComponents()
        {
            playerController = mainPlayerObject.GetComponent<PlayerController>();
            playerInventory = mainPlayerObject.GetComponent<PlayerInventory>();
            playerCharacter = mainPlayerObject.GetComponent<Character>();
        }

        private void ValidateComponents()
        {
            if (mainPlayerObject == null) this.LogError("Player object is null");
            if (playerController == null) this.LogError("Player Controller is null");
            if (playerInventory == null) this.LogError("Player Inventory is null");
            if (playerCharacter == null) this.LogError("Player Character is null");

        }


        #region Getters

        public GameObject MainPlayerObject => mainPlayerObject;
        public PlayerController PlayerController => playerController;
        public PlayerInventory PlayerInventory => playerInventory;
        public Character PlayerCharacter => playerCharacter;

        #endregion

    }
}
