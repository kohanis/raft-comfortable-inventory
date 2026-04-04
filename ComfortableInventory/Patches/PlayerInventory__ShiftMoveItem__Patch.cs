using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using kohanis.ComfortableInventory.Reflected;

// ReSharper disable InconsistentNaming

namespace kohanis.ComfortableInventory.Patches
{
    /// <summary>
    ///     Shift-move whole stack
    /// </summary>
    [HarmonyPatch(typeof(PlayerInventory), nameof(PlayerInventory.ShiftMoveItem))]
    internal static class PlayerInventory__ShiftMoveItem__Patch
    {
        private static readonly HashSet<OpCode> longBranchCodes = new HashSet<OpCode>
            { OpCodes.Brfalse, OpCodes.Brtrue, OpCodes.Brfalse_S, OpCodes.Brtrue_S };

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions,
            ILGenerator generator)
        {
            var codeMatcher = new CodeMatcher(instructions, generator)
                .MatchStartForward(
                    new CodeMatch(ins => longBranchCodes.Contains(ins.opcode)),
                    new CodeMatch(OpCodes.Ldarg_0),
                    new CodeMatch(OpCodes.Ldfld, FieldInfos.PlayerInventory__hotbar),
                    new CodeMatch(OpCodes.Ldarg_1),
                    new CodeMatch(OpCodes.Callvirt, MethodInfos.Hotbar__ContainsSlot),
                    new CodeMatch(OpCodes.Brfalse_S)
                );
            
            if (codeMatcher.IsInvalid)
            {
                ComfortableInventory.LogError("PlayerInventory.ShiftMoveItem patch failed");
                return null;
            }

            var branchEndLabel = (Label)codeMatcher.Operand;

            codeMatcher
                .Advance(1)
                .CreateLabel(out var loopLabel)
                .Advance(5)
                .SearchForward(ins => ins.labels.Contains(branchEndLabel));
            
            if (codeMatcher.IsInvalid)
            {
                ComfortableInventory.LogError("PlayerInventory.ShiftMoveItem patch failed");
                return null;
            }

            codeMatcher.MatchEndBackwards(
                new CodeMatch(OpCodes.Callvirt, MethodInfos.Inventory__StackSlots),
                new CodeMatch(OpCodes.Ret)
            );
            
            if (codeMatcher.IsInvalid)
            {
                ComfortableInventory.LogError("PlayerInventory.ShiftMoveItem patch failed");
                return null;
            }
            
            codeMatcher
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new CodeInstruction(OpCodes.Callvirt, MethodInfos.Slot__IsEmpty__Getter),
                    new CodeInstruction(OpCodes.Brfalse, loopLabel)
                )
                .MatchEndBackwards(
                    new CodeMatch(OpCodes.Callvirt, MethodInfos.Inventory__MoveSlotToEmpty),
                    new CodeMatch(OpCodes.Ret)
                );
            
            if (codeMatcher.IsInvalid)
            {
                ComfortableInventory.LogError("PlayerInventory.ShiftMoveItem patch failed");
                return null;
            }

            return codeMatcher
                .Insert(
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new CodeInstruction(OpCodes.Callvirt, MethodInfos.Slot__IsEmpty__Getter),
                    new CodeInstruction(OpCodes.Brfalse, loopLabel)
                )
                .Instructions();
        }
    }
}