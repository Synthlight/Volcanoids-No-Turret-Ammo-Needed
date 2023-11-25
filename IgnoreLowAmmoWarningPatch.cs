using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;

namespace No_Turret_Ammo_Needed {
    [HarmonyPatch]
    [UsedImplicitly]
    public static class IgnoreLowAmmoWarningPatch {
        [HarmonyTargetMethod]
        [UsedImplicitly]
        public static MethodBase TargetMethod() {
            return typeof(LowAmmoWarning).GetMethod(nameof(LowAmmoWarning.HasLowAmmo), BindingFlags.NonPublic | BindingFlags.Instance);
        }

        [HarmonyPrefix]
        [UsedImplicitly]
        public static bool Prefix(ref bool __result) {
            __result = false;
            return false;
        }
    }
}