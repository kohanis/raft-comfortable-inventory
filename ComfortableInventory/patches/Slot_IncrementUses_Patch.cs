using HarmonyLib;

// ReSharper disable InconsistentNaming

namespace kohanis.ComfortableInventory.Patches
{
    [HarmonyPatch(typeof(Slot), "IncrementUses")]
    internal class Slot_IncrementUses_Patch
    {
        private static void Prefix(Slot __instance, int amountOfUsesToAdd, out int __state)
        {
            __state = -1;

            if (amountOfUsesToAdd >= 0 || __instance.IsEmpty || !PatchHelpers.IsSelectedHotbarSlot(__instance))
                return;


            __state = __instance.itemInstance.UniqueIndex;
        }

        private static void Postfix(Slot __instance, in int __state)
        {
            if (__state == -1)
                return;

            PatchHelpers.RefillSlotIfNeeded(__instance, __state);
        }
    }
}