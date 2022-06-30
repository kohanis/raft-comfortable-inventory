using HarmonyLib;
using UnityEngine;
using UnityEngine.EventSystems;

// ReSharper disable InconsistentNaming

namespace kohanis.ComfortableInventory.Patches
{
    [HarmonyPatch(typeof(Slot), "OnPointerEnter")]
    internal class Slot_OnPointerEnter_Patch
    {
        private static void Prefix(Slot __instance, PointerEventData eventData)
        {
            if (!Input.GetKey(KeyCode.LeftShift) || __instance.IsEmpty ||
                !eventData.eligibleForClick) // It should be when you hold LMB. I hope
                return;

            var itemInstance = __instance.itemInstance;
            var stackable = itemInstance.settings_Inventory.Stackable;

            // Calling Slot.OnPointerDown until all items inside have moved or there is no room left
            int amount;
            do
            {
                amount = itemInstance.Amount;
                __instance.OnPointerDown(eventData);
            } while (stackable && amount != itemInstance.Amount);
        }
    }
}