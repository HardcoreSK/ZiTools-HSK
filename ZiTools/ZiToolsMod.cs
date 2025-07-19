using HarmonyLib;
using Verse;

namespace ZiTools
{
    public class ZiToolsMod : Mod
    {
        public static ZiToolsSettings Settings { get; set; }
        
        public ZiToolsMod(ModContentPack content) : base(content)
        {
            Settings = GetSettings<ZiToolsSettings>();
            
            var harmony = new Harmony("rimworld.maxzicode.zitools.mainconstructor");
            harmony.PatchAll();
        }
    }
}
