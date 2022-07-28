using HarmonyLib;
using UnityEngine;
using UnityEngine.EventSystems;

// ReSharper disable InconsistentNaming

namespace kohanis.ComfortableInventory.Patches
{
    /// <summary>
    ///     "Shift-click" or "Drop" hovered <see cref="Slot" />s while holding LMB and corresponding button(s)
    /// </summary>
    [HarmonyPatch(typeof(Slot), "OnPointerEnter")]
    internal class Slot_OnPointerEnter_Patch
    {
        private static void Prefix(Slot __instance, PointerEventData eventData, Inventory ___inventory)
        {
            if (__instance.IsEmpty || !eventData.eligibleForClick)
                return;

            if (MyInput.GetButton("Drop"))
            {
                var playerInventory =
                    ___inventory as PlayerInventory ?? ___inventory.secondInventory as PlayerInventory;
                if (playerInventory != null)
                {
                    playerInventory.DropItem(__instance);
                    return;
                }
            }

            if (Input.GetKey(KeyCode.LeftShift))
                ___inventory.ShiftMoveItem(__instance, eventData);
        }
    }
}