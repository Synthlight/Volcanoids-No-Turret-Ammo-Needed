using Base_Mod;
using JetBrains.Annotations;

namespace No_Turret_Ammo_Needed {
    [UsedImplicitly]
    public class Plugin : BaseGameMod {
        protected override string ModName    => "No-Turret-Ammo-Needed";
        protected override bool   UseHarmony => true;
    }
}