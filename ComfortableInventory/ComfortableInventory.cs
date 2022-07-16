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

        internal static void Log(string message)
        {
            Debug.Log($"{LogPrefix}: {message}");
        }
    }
}