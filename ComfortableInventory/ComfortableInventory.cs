using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace kohanis.ComfortableInventory
{
    public class ComfortableInventory : Mod
    {
        private const string HarmonyID = "kohanis.ComfortableInventory";
        private const string LogPrefix = "[Comfortable Inventory] ";
        private Harmony _harmony;

        public void Start()
        {
            _harmony = new Harmony(HarmonyID);
            _harmony.PatchAll(Assembly.GetExecutingAssembly());

            Log("Mod has been loaded!");
        }

        public void OnModUnload()
        {
            _harmony.UnpatchAll(HarmonyID);

            Log("Mod has been unloaded!");
        }

        [ConsoleCommand("fixBackpack", "fixes backpack glitch when items are in inactive slots.")]
        public static void fixBackpack()
        {
            var inv = RAPI.GetLocalPlayer()?.Inventory;
            if (inv is null)
            {
                Log("this command can only be used with loaded player");
                return;
            }

            foreach (var slot in inv.allSlots)
            {
                if (!slot.active && !slot.IsEmpty)
                    inv.DropItem(slot);
            }
        }

        internal static void Log(string message)
        {
            Debug.Log($"{LogPrefix}: {message}");
        }
    }
}