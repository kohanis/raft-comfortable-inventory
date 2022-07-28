using HarmonyLib;

// ReSharper disable InconsistentNaming

namespace kohanis.ComfortableInventory.Patches
{
    /// <summary>
    ///     Patches <see cref="Slot.IncrementUses">Slot.IncrementUses</see> to replenish items if needed
    /// </summary>
    [HarmonyPatch(typeof(Slot), "IncrementUses")]
    internal class Slot_IncrementUses_Patch
    {
        private static void Prefix(Slot __instance, int amountOfUsesToAdd, out int __state, Inventory ___inventory)
        {
            if (amountOfUsesToAdd < 0 && !__instance.IsEmpty &&
                (___inventory as PlayerInventory)?.hotbar.IsSelectedHotSlot(__instance) == true)
                __state = __instance.itemInstance.UniqueIndex;
            else
                __state = -1;
        }

        private static void Postfix(Slot __instance, int __state, Inventory ___inventory)
        {
            if (__state == -1)
                return;

            PatchHelpers.ReplenishSlotIfNeeded(__instance, __state, ___inventory as PlayerInventory);
        }
    }
}