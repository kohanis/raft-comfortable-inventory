using System.Reflection;
using HarmonyLib;
using kohanis.ComfortableInventory.Patches;
using UnityEngine;

namespace kohanis.ComfortableInventory
{
    public class ComfortableInventory : Mod
    {
        private const string HarmonyID = "kohanis.ComfortableInventory";
        private const string LogPrefix = "<color=teal>Comfortable Inventory</color>";
        private Harmony harmony;

        public void Start()
        {
            harmony = new Harmony(HarmonyID);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            Slot_RemoveItem_MultiPatch.Patch(harmony);

            Log("mod has been loaded!");
        }

        public void OnModUnload()
        {
            harmony.UnpatchAll(HarmonyID);

            Log("mod has been unloaded!");
        }

        internal static void Log(string message) =>
            Debug.Log($"{LogPrefix}: {message}");
    }
}