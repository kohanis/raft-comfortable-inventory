using System.Reflection;
using HarmonyLib;

// ReSharper disable InconsistentNaming

namespace kohanis.ComfortableInventory.Patches
{
    public static class Slot_RemoveItem_MultiPatch
    {
        //
        // Targets
        //
        private static readonly MethodInfo PlayerInventory_target =
            AccessTools.Method(typeof(PlayerInventory), nameof(PlayerInventory.RemoveSelectedHotSlotItem));

        private static readonly MethodInfo Inventory_target =
            AccessTools.Method(typeof(Inventory), nameof(Inventory.RemoveItem));

        private static readonly MethodInfo BlockCreator_target =
            AccessTools.Method(typeof(BlockCreator), nameof(BlockCreator.CreateBlock));

        //
        // Patches
        //
        private static readonly HarmonyMethod commonPrefix =
            new HarmonyMethod(typeof(Slot_RemoveItem_MultiPatch), nameof(CommonPrefix));

        private static readonly HarmonyMethod commonPostfix =
            new HarmonyMethod(typeof(Slot_RemoveItem_MultiPatch), nameof(CommonPostfix));

        private static readonly HarmonyMethod inventory_Prefix =
            new HarmonyMethod(typeof(Slot_RemoveItem_MultiPatch), nameof(Inventory_Prefix));

        private static readonly HarmonyMethod blockCreator_Prefix =
            new HarmonyMethod(typeof(Slot_RemoveItem_MultiPatch), nameof(BlockCreator_Prefix));

        internal static void Patch(Harmony harmony)
        {
            harmony.Patch(PlayerInventory_target, commonPrefix, commonPostfix);
            harmony.Patch(Inventory_target, inventory_Prefix, commonPostfix);
            harmony.Patch(BlockCreator_target, blockCreator_Prefix, commonPostfix);
        }

        //
        // Patch bodies
        //
        private static void CommonPrefix(PlayerInventory __instance, out StateData? __state)
        {
            __state = null;

            if (__instance is PlayerInventory playerInventory &&
                playerInventory.GetSelectedHotbarSlot() is Slot slot &&
                slot.itemInstance is ItemInstance item)
            {
                // No need to refill empty water holders
                var consumable = item.settings_consumeable;
                if (consumable.FoodForm != FoodForm.Fluid || consumable.FoodType != FoodType.None)
                    __state = new StateData(playerInventory, slot, item.UniqueIndex);
            }
        }

        private static void CommonPostfix(StateData? __state)
        {
            if (__state is StateData stateData)
                Helpers.RefillSlotIfNeeded(stateData.slot, stateData.index, stateData.playerInventory);
        }

        private static void Inventory_Prefix(Inventory __instance, out StateData? __state)
            => CommonPrefix(__instance as PlayerInventory, out __state);

        private static void BlockCreator_Prefix(out StateData? __state)
            => CommonPrefix(RAPI.GetLocalPlayer().Inventory, out __state);


        private readonly struct StateData
        {
            public StateData(PlayerInventory playerInventory, Slot slot, int index)
            {
                this.playerInventory = playerInventory;
                this.slot = slot;
                this.index = index;
            }

            public readonly PlayerInventory playerInventory;
            public readonly Slot slot;
            public readonly int index;
        }
    }
}