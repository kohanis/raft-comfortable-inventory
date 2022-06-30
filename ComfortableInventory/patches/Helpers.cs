using HarmonyLib;

namespace kohanis.ComfortableInventory.Patches
{
    internal static class Helpers
    {
        private static readonly AccessTools.FieldRef<Slot, Inventory> SlotInventoryGetter =
            AccessTools.FieldRefAccess<Slot, Inventory>(AccessTools.Field(typeof(Slot), "inventory"));

        private static readonly FastInvokeHandler MoveSlotToEmptyInvoker =
            MethodInvoker.GetHandler(AccessTools.Method(typeof(PlayerInventory), "MoveSlotToEmpty"));

        private static readonly FastInvokeHandler SwitchSlotsInvoker =
            MethodInvoker.GetHandler(AccessTools.Method(typeof(PlayerInventory), "SwitchSlots"));

        // Faster, but needs Harmony 2.0.2

        // public delegate void MoveSlotToEmpty(Inventory inventory, Slot fromSlot, Slot toSlot, int amount);
        // public delegate void SwitchSlots(Inventory inventory, Slot fromSlot, Slot toSlot);
        //
        // private static readonly MoveSlotToEmpty MoveSlotToEmptyDelegate =
        //     AccessTools.MethodDelegate<MoveSlotToEmpty>(AccessTools.Method(typeof(PlayerInventory), "MoveSlotToEmpty"));
        // private static readonly SwitchSlots SwitchSlotsDelegate =
        //     AccessTools.MethodDelegate<SwitchSlots>(AccessTools.Method(typeof(PlayerInventory), "SwitchSlots"));

        internal static bool IsSelectedHotbarSlot(Slot slot)
        {
            var inventory = SlotInventoryGetter(slot) as PlayerInventory;
            return inventory != null && slot == inventory.GetSelectedHotbarSlot();
        }

        internal static void RefillSlotIfNeeded(Slot slot, int originalUniqueIndex,
            PlayerInventory playerInventory = null)
        {
            if (slot.itemInstance?.UniqueIndex == originalUniqueIndex)
                return;

            var inventory = playerInventory ? playerInventory : SlotInventoryGetter(slot) as PlayerInventory;
            if (inventory is null)
                return;

            foreach (var localSlot in inventory.allSlots)
            {
                // last condition will never be triggered in current implementation, but let it be
                if (localSlot.IsEmpty || localSlot.slotType == SlotType.Hotbar || localSlot == slot)
                    continue;

                var slotItemInstance = localSlot.itemInstance;
                if (slotItemInstance.UniqueIndex != originalUniqueIndex)
                    continue;

                if (slot.IsEmpty)
                    MoveSlotToEmptyInvoker(inventory, new object[] { localSlot, slot, slotItemInstance.Amount });
                else
                    SwitchSlotsInvoker(inventory, new object[] { localSlot, slot });
                break;
            }
        }
    }
}