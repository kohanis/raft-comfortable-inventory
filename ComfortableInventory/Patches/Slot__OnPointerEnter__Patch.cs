using HarmonyLib;
using UnityEngine;
using UnityEngine.EventSystems;

// ReSharper disable InconsistentNaming

namespace kohanis.ComfortableInventory.Patches
{
    /// <summary>
    ///     Perform "Shift-click" or "Drop" actions while holding LMB and moving pointer
    /// </summary>
    [HarmonyPatch(typeof(Slot), nameof(Slot.OnPointerEnter))]
    internal static class Slot__OnPointerEnter__Patch
    {
        private static void Prefix(Slot __instance, PointerEventData eventData, Inventory ___inventory)
        {
            if (__instance.IsEmpty || !eventData.eligibleForClick)
                return;

            if (MyInput.GetButton("Drop"))
            {
                var playerInventory = ___inventory as PlayerInventory ??
                                      ___inventory.secondInventory as PlayerInventory;
                if (playerInventory != null) playerInventory.DropItem(__instance);
            }
            else if (Input.GetKey(KeyCode.LeftShift))
            {
                ___inventory.ShiftMoveItem(__instance);
            }
        }
    }
}