using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

// ReSharper disable InconsistentNaming

namespace kohanis.ComfortableInventory.Patches
{
    /// <summary>
    ///     Checks several methods where <see cref="Slot.RemoveItem">Slot.RemoveItem</see> is used for hotslot changes
    ///     to replenish it if needed
    /// </summary>
    [HarmonyPatch]
    public static class Slot_RemoveItem_MultiPatch
    {
        private static readonly MethodInfo Slot_RemoveItem_MethodInfo =
            AccessTools.Method(typeof(Slot), nameof(Slot.RemoveItem));

        private static readonly CodeInstruction RemoveItemReplacementInstruction = new CodeInstruction(OpCodes.Call,
            AccessTools.Method(typeof(Slot_RemoveItem_MultiPatch), nameof(RemoveItemReplacement)));

        private static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(BlockCreator), nameof(BlockCreator.CreateBlock));
            yield return AccessTools.Method(typeof(PlayerInventory), nameof(PlayerInventory.RemoveSelectedHotSlotItem));
        }

        internal static void RemoveItemReplacement(Slot slot, int amount, PlayerInventory instance)
        {
            // already empty-checked in originals
            var item = slot.itemInstance;
            int index = item.UniqueIndex;

            // No need to refill empty water holders
            var consumable = item.settings_consumeable;
            bool skip = consumable.FoodForm == FoodForm.Fluid && consumable.FoodType == FoodType.None;

            slot.RemoveItem(amount);

            if (!skip)
                PatchHelpers.ReplenishSlotIfNeeded(slot, index, instance);
        }

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions,
            MethodBase original)
        {
            foreach (var codeInstruction in instructions)
            {
                if (codeInstruction.Calls(Slot_RemoveItem_MethodInfo))
                {
                    // RemoveItemReplacement(slot, amount, this/null);
                    yield return new CodeInstruction(
                        original.DeclaringType == typeof(PlayerInventory)
                            ? OpCodes.Ldarg_0
                            : OpCodes.Ldnull);
                    yield return RemoveItemReplacementInstruction;
                }
                else
                {
                    yield return codeInstruction;
                }
            }
        }
    }
}