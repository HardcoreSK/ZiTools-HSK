using UnityEngine;
using Verse;

namespace ZiTools
{
	// class for drawing a thing icon
	public class ThingIconData : IDBIcon
	{
		private Thing _thing;

		public ThingIconData(Thing thing)
		{
			_thing = thing;
		}

		public void DrawIcon(Rect outerRect)
		{
			if (_thing != null)
				Widgets.ThingIcon(outerRect, _thing);
		}
	}
}
