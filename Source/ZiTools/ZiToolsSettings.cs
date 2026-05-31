using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Verse;

namespace ZiTools
{
    public class ZiToolsSettings : ModSettings
    {
        public bool replaceVanillaSearch = true;

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(
                ref replaceVanillaSearch,
                "replaceVanillaSearch",
                true);
        }
    }
}
