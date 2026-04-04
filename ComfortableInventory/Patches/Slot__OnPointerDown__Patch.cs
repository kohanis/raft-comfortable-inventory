using HarmonyLib;
using UnityEngine;
using UnityEngine.EventSystems;

// ReSharper disable InconsistentNaming

namespace kohanis.ComfortableInventory.Patches
{
    /// <summary>
    ///     Moves all items of same type as <see cref="Slot" /> item when clicked while holding Left Alt and Left Shift
    /// </summary>
    [HarmonyPatch(typeof(Slot), nameof(Slot.OnPointerDown))]
    internal static class Slot__OnPointerDown__Patch
    {
        private static bool Prefix(Slot __instance, PointerEventData eventData, Inventory ___inventory)
        {
            if (__instance.IsEmpty || !(Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftAlt)))
                return true;

            var secondInventory = ___inventory.secondInventory;
            if (secondInventory is Inventory_ResearchTable)
                return true;

            var allSlots = ___inventory.allSlots;
            int start = 0,
                end = allSlots.Count;

            // from/to hotbar
            if (secondInventory == null && ___inventory is PlayerInventory playerInventory)
            {
                var hotslotCount = playerInventory.hotslotCount;
                if (playerInventory.hotbar.ContainsSlot(__instance))
                    end = hotslotCount;
                else
                    start = hotslotCount;
            }

            var uniqueIndex = __instance.itemInstance.UniqueIndex;
            for (var i = start; i < end; i++)
            {
                var slot = allSlots[i];

                if (slot.itemInstance?.UniqueIndex != uniqueIndex)
                    continue;

                ___inventory.ShiftMoveItem(slot);

                if (slot.itemInstance != null)
                    break;
            }

            return false;
        }
    }
}