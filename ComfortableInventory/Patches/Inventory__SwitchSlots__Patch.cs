using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using HarmonyLib;
using kohanis.ComfortableInventory.Reflected;

// ReSharper disable InconsistentNaming

namespace kohanis.ComfortableInventory.Patches
{
    /// <summary>
    ///     Allows switching equipment of same type (except backpacks)
    /// </summary>
    [HarmonyPatch(typeof(Inventory), "SwitchSlots")]
    internal static class Inventory__SwitchSlots__Patch
    {
        private static readonly MethodInfo ShouldDenySwapPatch_MethodInfo =
            AccessTools.Method(typeof(Inventory__SwitchSlots__Patch), nameof(ShouldDenySwapPatch));
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static EquipSlotType ShouldDenySwapPatch(EquipSlotType origTag, Slot fromSlot, Slot toSlot)
        {
            var fromSlotEquipType = fromSlot.itemInstance.settings_equipment.EquipType;
            var toSlotEquipType = toSlot.itemInstance.settings_equipment.EquipType;

            if (fromSlotEquipType == EquipSlotType.Backpack || toSlotEquipType == EquipSlotType.Backpack)
                return EquipSlotType.Backpack;

            if (fromSlotEquipType == toSlotEquipType)
                return EquipSlotType.None;

            return origTag;
        }

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codeMatcher = new CodeMatcher(instructions)
                .MatchStartForward(
                    new CodeMatch(OpCodes.Call, MethodInfos.Slot_Equip__GetEquipSlotWithTag)
                );

            if (codeMatcher.IsInvalid)
            {
                ComfortableInventory.LogError("Inventory.SwitchSlots patch failed");
                return null;
            }

            return codeMatcher
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new CodeInstruction(OpCodes.Ldarg_2),
                    new CodeInstruction(OpCodes.Call, ShouldDenySwapPatch_MethodInfo)
                )
                .Instructions();
        }
    }
}