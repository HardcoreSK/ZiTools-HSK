using Verse;
using RimWorld;
using UnityEngine;

namespace ZiTools
{
	// class for drawing a thing icon
	public class ThingIconData : IDBIcon
	{
		private Color drawColor;
		private readonly float resolvedIconAngle;
		private readonly Texture resolvedIcon;

		public const float ThingIconSize = 50f;

		public ThingIconData(Thing thing)
		{
			this.drawColor = thing.DrawColor;
			this.resolvedIconAngle = 0f;
			if (!thing.def.uiIconPath.NullOrEmpty())
			{
				this.resolvedIcon = thing.def.uiIcon;
				this.resolvedIconAngle = thing.def.uiIconAngle;
			}
			else if (thing is Pawn || thing is Corpse)
			{
				if (!(thing is Pawn pawn))
				{
					pawn = ((Corpse)thing).InnerPawn;
				}
				if (!pawn.RaceProps.Humanlike)
				{
					this.resolvedIcon = PortraitsCache.Get(pawn, new Vector2(ThingIconSize, ThingIconSize), new Rot4());
				}
				else
				{
					this.resolvedIcon = PortraitsCache.Get(pawn, new Vector2(ThingIconSize, ThingIconSize), Rot4.East);
				}
			}
			else
			{
				this.resolvedIcon = thing.Graphic.ExtractInnerGraphicFor(thing).MatAt(thing.def.defaultPlacingRot, null).mainTexture;
			}
			resolvedIcon = resolvedIcon ?? BaseContent.WhiteTex;
		}

		public void DrawIcon(Rect outerRect)
		{
			//t = t.GetInnerIfMinified();
			GUI.color = this.drawColor;
			Widgets.DrawTextureRotated(outerRect, this.resolvedIcon, this.resolvedIconAngle);
			GUI.color = Color.white;
		}
	}
}