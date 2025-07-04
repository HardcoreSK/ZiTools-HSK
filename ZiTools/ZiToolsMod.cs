using HarmonyLib;
using Verse;

namespace ZiTools
{
    public class ZiToolsMod : Mod
    {
        public ZiToolsMod(ModContentPack content) : base(content)
        {
            var harmony = new Harmony("rimworld.maxzicode.zitools.mainconstructor");
            harmony.PatchAll();
        }
    }
}
