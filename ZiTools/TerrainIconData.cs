using Verse;
using UnityEngine;

namespace ZiTools
{
	// class for drawing a terrain icon
	public class TerrainIconData : IDBIcon
	{
		private TerrainDef _terrainDef;

		public TerrainIconData(TerrainDef entDef)
		{
			_terrainDef = entDef;
		}

		public void DrawIcon(Rect outerRect)
		{
			Widgets.DefIcon(outerRect, _terrainDef);
		}
	}
}
