using Verse;
using UnityEngine;

namespace ZiTools
{
	// class for drawing a terrain icon
	public class TerrainIconData : IDBIcon
	{
		public const float Padding = 2f;
		private TerrainDef _terrainDef;

		public TerrainIconData(TerrainDef entDef)
		{
			_terrainDef = entDef;
		}

		public void DrawIcon(Rect outerRect)
		{
			if (_terrainDef != null)
			{
				var iconRect = new Rect(outerRect);
				iconRect.x += Padding;
				iconRect.y += Padding;
				iconRect.width -= Padding + Padding;
				iconRect.height -= Padding + Padding;

				Widgets.DefIcon(iconRect, _terrainDef);
            }
		}
	}
}
