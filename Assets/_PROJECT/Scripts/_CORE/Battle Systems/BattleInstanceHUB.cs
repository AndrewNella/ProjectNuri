using UnityEngine;
using PhantomPixelStudio.Utility;
using Kisei.Utility;

namespace Kisei.BattleSystem
{

    public class BattleInstanceHUB : Singleton<BattleInstanceHUB>
    {
        private GameObject battleObject;
        private BattleController mainBattleController;
        private BattleMenuControl mainBattleUIController;
        private BattleFunctions battleLogicController;
        private InventoryUI battleInventoryUI;

        protected override void InternalInit()
        {
            if (battleObject == null) battleObject = gameObject;

            GetComponents();
            ValidateComponents();
        }
        private void GetComponents()
        {
            mainBattleController = battleObject.GetComponent<BattleController>();
            mainBattleUIController = battleObject.GetComponentInChildren<BattleMenuControl>();
            battleInventoryUI = battleObject.GetComponentInChildren<InventoryUI>(true);
            battleLogicController = battleObject.GetComponent<BattleFunctions>();


        }

        private void ValidateComponents()
        {
            if (battleObject == null) this.LogError("Battle  object is null");
            if (mainBattleController == null) this.LogError("Battle Controller is null");
            if (mainBattleUIController == null) this.LogError("Battle UI is null");
            if (battleLogicController == null) this.LogError("Battle Logic is null");
            if (battleInventoryUI == null) this.LogError("Battle Inventory is null");

        }


        #region Getters

        public GameObject MainBattleObject => battleObject;
        public BattleController BattleController => mainBattleController;
        public BattleMenuControl BattleUI => mainBattleUIController;
        public BattleFunctions BattleLogic => battleLogicController;
        public InventoryUI BattleInventory => battleInventoryUI;

        #endregion
    }
}
