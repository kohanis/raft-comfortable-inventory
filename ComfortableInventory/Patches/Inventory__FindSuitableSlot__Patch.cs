﻿using HarmonyLib;
using kohanis.ComfortableInventory.Reflected.Delegates;

// ReSharper disable InconsistentNaming

namespace kohanis.ComfortableInventory.Patches
{
    /// <summary>
    ///     Prefer inventory feature
    /// </summary>
    [HarmonyPatch(typeof(Inventory), "FindSuitableSlot")]
    internal static class Inventory__FindSuitableSlot__Patch
    {
        private static bool Prefix(Inventory __instance, Item_Base stackableItem, ref Slot __result)
        {
            if (!Config.PreferInventory || stackableItem == null || !(__instance is PlayerInventory self))
                return true;

            var subCategory = stackableItem.settings_recipe.SubCategory;
            if ((Config.PreferHotbarHealing && subCategory == "Healing") ||
                (Config.PreferHotbarEquipment && stackableItem.IsEquipment()) ||
                (Config.PreferHotbarTools && stackableItem.IsTool()) ||
                (Config.PreferHotbarCookedFood && stackableItem.IsCookedFood()) ||
                (Config.PreferHotbarFreshWater && stackableItem.IsFreshWater()) ||
                (Config.PreferHotbarFoodContainers && stackableItem.IsFoodContainers()) ||
                (Config.PreferHotbarBuildable &&
                 (stackableItem.settings_buildable.Placeable || subCategory == "Batteries")))
                return true;


            if (stackableItem.settings_Inventory.Stackable)
                __result = self.FindSuitableSlotReflected(0, self.hotslotCount, stackableItem);

            if (__result == null || __result.IsEmpty)
            {
                var slot = self.FindSuitableSlotReflected(self.hotslotCount, self.allSlots.Count, stackableItem);

                if (slot != null)
                    __result = slot;
            }

            return false;
        }
    }
}