using Verse;

namespace ZiTools
{
    public class ZiToolsSettings : ModSettings
    {
        public bool hideOldMapSearchIcon = true;
        public bool showArrowsToSelectedObject = true;
        public int maxArrowsInScreen = 100;
        public bool arrowsOffscreenOnly = true;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref hideOldMapSearchIcon, "HideOldMapSearchIcon", true);
            Scribe_Values.Look(ref showArrowsToSelectedObject, "ShowArrowsToSelectedObject", true);
            Scribe_Values.Look(ref maxArrowsInScreen, "MaxArrowsInScreen", 100);
            Scribe_Values.Look(ref arrowsOffscreenOnly, "ArrowsOffscreenOnly", true);
        }
    }
}
