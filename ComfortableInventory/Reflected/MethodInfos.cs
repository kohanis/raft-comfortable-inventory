using System.Reflection;
using HarmonyLib;


// ReSharper disable InconsistentNaming

namespace kohanis.ComfortableInventory.Reflected
{
    internal static class MethodInfos
    {
        public static readonly MethodInfo Inventory__RemoveItemUses =
            AccessTools.Method(typeof(Inventory), nameof(Inventory.RemoveItemUses));

        public static readonly MethodInfo Hotbar__ContainsSlot =
            AccessTools.Method(typeof(Hotbar), nameof(Hotbar.ContainsSlot));
                
        public static readonly MethodInfo Inventory__MoveSlotToEmpty =
            AccessTools.Method(typeof(Inventory), "MoveSlotToEmpty");

        public static readonly MethodInfo Inventory__StackSlots =
            AccessTools.Method(typeof(Inventory), "StackSlots");

        public static readonly MethodInfo Slot__IncrementUses =
            AccessTools.Method(typeof(Slot), nameof(Slot.IncrementUses));

        public static readonly MethodInfo Slot_Equip__GetEquipSlotWithTag =
            AccessTools.Method(typeof(Slot_Equip), nameof(Slot_Equip.GetEquipSlotWithTag));
        
        public static readonly MethodInfo Slot__IsEmpty__Getter =
            AccessTools.PropertyGetter(typeof(Slot), nameof(Slot.IsEmpty));

        public static readonly MethodInfo LiquidFuelManager__GetFilteredFuelValueSoFromItem =
            AccessTools.Method(typeof(LiquidFuelManager), nameof(LiquidFuelManager.GetFilteredFuelValueSoFromItem));

        public static readonly MethodInfo Tank__HandleAddFuel =
            AccessTools.Method(typeof(Tank), "HandleAddFuel");

        public static readonly MethodInfo Tank__ModifyTank =
            AccessTools.Method(typeof(Tank), nameof(Tank.ModifyTank));
    }
}