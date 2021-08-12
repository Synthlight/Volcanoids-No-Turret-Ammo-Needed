using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;

namespace No_Turret_Ammo_Needed {
    [HarmonyPatch]
    [UsedImplicitly]
    public static class TurretAmmoPatch1 {
        [HarmonyTargetMethod]
        [UsedImplicitly]
        public static MethodBase TargetMethod() {
            return typeof(WeaponReloaderOnlineCargo).GetMethod(nameof(WeaponReloaderOnlineCargo.LoadAmmo), BindingFlags.Public | BindingFlags.Instance);
        }

        [HarmonyPrefix]
        [UsedImplicitly]
        public static bool Prefix(ref WeaponReloaderOnlineCargo __instance, ref bool __result, ref int ___m_loadedAmmoAmount, ref AmmoDefinition ___m_loadedAmmo, ref AmmoDefinition ammo) {
            if (___m_loadedAmmo != ammo || ___m_loadedAmmoAmount != __instance.AmmoCapacity) {
                ___m_loadedAmmo       = ammo;
                ___m_loadedAmmoAmount = __instance.AmmoCapacity;
                __instance.SetDirtyBit(uint.MaxValue);
                __result = true;
                return false;
            }
            __result = false;
            return false;
        }
    }

    [HarmonyPatch]
    [UsedImplicitly]
    public static class TurretAmmoPatch2 {
        [HarmonyTargetMethod]
        [UsedImplicitly]
        public static MethodBase TargetMethod() {
            return typeof(WeaponReloaderOnlineCargo).GetMethod("HasAmmo", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        [HarmonyPrefix]
        [UsedImplicitly]
        public static bool Prefix(ref bool __result) {
            __result = true;
            return false;
        }
    }

    [HarmonyPatch]
    [UsedImplicitly]
    public static class TurretAmmoPatch3 {
        [HarmonyTargetMethod]
        [UsedImplicitly]
        public static MethodBase TargetMethod() {
            return typeof(WeaponReloaderOnlineCargo).GetMethod("TryGetNextAmmo", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        [HarmonyPrefix]
        [UsedImplicitly]
        public static bool Prefix(ref WeaponReloaderOnlineCargo __instance, ref AmmoDefinition ammo, ref bool __result) {
            ammo     = __instance.Ammunition[0];
            __result = true;
            return false;
        }
    }

    [HarmonyPatch]
    [UsedImplicitly]
    public static class TurretAmmoPatch4 {
        [HarmonyTargetMethod]
        [UsedImplicitly]
        public static MethodBase TargetMethod() {
            return typeof(WeaponReloaderOnlineCargo).GetMethod(nameof(WeaponReloaderOnlineCargo.UnloadAmmo), BindingFlags.Public | BindingFlags.Instance);
        }

        [HarmonyPrefix]
        [UsedImplicitly]
        public static bool Prefix(ref WeaponReloaderOnlineCargo __instance, ref bool __result, ref int ___m_loadedAmmoAmount, ref AmmoDefinition ___m_loadedAmmo) {
            if (___m_loadedAmmoAmount > 0 && ___m_loadedAmmo != null) {
                ___m_loadedAmmo       = null;
                ___m_loadedAmmoAmount = 0;
                __instance.SetDirtyBit(uint.MaxValue);
            }
            __result = true;
            return false;
        }
    }
}