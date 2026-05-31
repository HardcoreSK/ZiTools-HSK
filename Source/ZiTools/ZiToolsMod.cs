using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace ZiTools
{
    public class ZiToolsMod : Mod
    {
        public static ZiToolsSettings Settings { get; private set; }

        public ZiToolsMod(ModContentPack content) : base(content)
        {
            Settings = GetSettings<ZiToolsSettings>();
        }

        public override string SettingsCategory()
        {
            return "ZiTools";
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listing = new Listing_Standard();
            listing.Begin(inRect);

            listing.CheckboxLabeled(
                "Replace vanilla search",
                ref Settings.replaceVanillaSearch);

            listing.End();
        }
    }
}
