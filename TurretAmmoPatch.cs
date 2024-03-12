using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

namespace No_Turret_Ammo_Needed;

[HarmonyPatch]
[UsedImplicitly]
public static class TurretAmmoPatch1 {
    public const bool ENABLE_LOGS = false;

    public static readonly List<GUID> TURRET_AMMO_ITEMS = [
        GUID.Parse("f46cdfb659dd5f3428f5a1c9c1fe7d32"), // Pistol Turret Ammo
        GUID.Parse("cdd3ca2c103709d4395f67d08bef56f9"), // Shotgun Turret Ammo
        GUID.Parse("3bd794833a3c801498d50cb745f51734"), // SMG Turret Ammo
        GUID.Parse("66a6ec9baa4d140438c316b93c639a72"), // Sniper Turret Ammo
        GUID.Parse("6af41273ef98e88499f8a7d4e71430c6"), // Mortar Turret Grenade
        GUID.Parse("68125db51cd1b714d8cd8f1dd1e1d374")
    ];

    [HarmonyTargetMethod]
    [UsedImplicitly]
    public static MethodBase TargetMethod() {
        return typeof(WeaponReloaderAmmo).GetMethod(nameof(WeaponReloaderAmmo.LoadAmmo), BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy, null, [typeof(InventoryBase), typeof(ItemDefinition), typeof(int)], null);
    }

    [HarmonyPrefix]
    [UsedImplicitly]
    public static bool Prefix(ref WeaponReloaderAmmo __instance, ref bool __result, ref ItemDefinition ammo) {
        if (!IsTurretAmmo("LoadAmmo", __instance)) return true;

        if (__instance.m_loadedAmmo != ammo || __instance.m_loadedAmmoAmount != __instance.AmmoCapacity) {
            __instance.SetLoadedAmmo(ammo, __instance.AmmoCapacity);
            __result = true;
            return false;
        }
        __result = false;
        return false;
    }

    public static bool IsTurretAmmo(string source, WeaponReloaderAmmo instance) {
#pragma warning disable CS0162
        if (ENABLE_LOGS) {
            Debug.Log(source);
            Debug.Log($"    Parent name: {instance.gameObject.name}");
            Debug.Log($"    Instance type: {instance.GetType()}");

            if (instance.LoadedAmmoItem == null) {
                if (ENABLE_LOGS) Debug.Log("    Unable to determine ammo type, ignoring it and treating as not a turret.");
                return false;
            }

            Debug.Log($"    Ammo type: {instance.LoadedAmmoItem.Name}");
        }

        if (instance.LoadedAmmoItem != null && TURRET_AMMO_ITEMS.Contains(instance.LoadedAmmoItem.AssetId)) {
            if (ENABLE_LOGS) Debug.Log("    It's a turret!");
            return true;
        }

        return false;
#pragma warning restore CS0162
    }
}

[HarmonyPatch]
[UsedImplicitly]
public static class TurretAmmoPatch2 {
    [HarmonyTargetMethod]
    [UsedImplicitly]
    public static MethodBase TargetMethod() {
        return typeof(WeaponReloaderDefault).GetMethod(nameof(WeaponReloaderDefault.HasAmmo), BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
    }

    [HarmonyPrefix]
    [UsedImplicitly]
    public static bool Prefix(ref WeaponReloaderDefault __instance, ref bool __result) {
        if (__instance is not WeaponReloaderAmmo ammoInstance) return true;
        if (!TurretAmmoPatch1.IsTurretAmmo("HasAmmo", ammoInstance)) return true;

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
        return typeof(WeaponReloaderDefault).GetMethod(nameof(WeaponReloaderDefault.TryGetNextAmmo), BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
    }

    [HarmonyPrefix]
    [UsedImplicitly]
    public static bool Prefix(ref WeaponReloaderDefault __instance, ref ItemDefinition ammo, ref bool __result) {
        if (__instance is not WeaponReloaderAmmo ammoInstance) return true;
        if (!TurretAmmoPatch1.IsTurretAmmo("TryGetNextAmmo", ammoInstance)) return true;

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
        return typeof(WeaponReloaderAmmo).GetMethod(nameof(WeaponReloaderAmmo.UnloadAmmo), BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
    }

    [HarmonyPrefix]
    [UsedImplicitly]
    public static bool Prefix(ref WeaponReloaderAmmo __instance, ref bool __result) {
        if (!TurretAmmoPatch1.IsTurretAmmo("UnloadAmmo", __instance)) return true;

        if (__instance.m_loadedAmmoAmount > 0 && __instance.m_loadedAmmo != null) {
            __instance.SetLoadedAmmo(null, 0);
        }
        __result = true;
        return false;
    }
}