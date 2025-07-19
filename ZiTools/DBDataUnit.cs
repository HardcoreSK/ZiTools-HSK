using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace ZiTools
{
    public class DBUnit : IExposable
    {
        private string _label;
        private HashSet<Thing> _things = new HashSet<Thing>();
        
        public readonly List<IntVec3> Locations = new List<IntVec3>();

        public DBUnit(string label)
        {
            _label = label;
            CleanData();
        }

        public DBUnit() : this(string.Empty) { }

        public string Label { get => _label; }

        public string Parameter { get; set; }

        public IDBIcon Icon { get; set; }

        public int Count => _things.Sum(t => t.stackCount);
        public int Stacks => _things.Count;

        public int CorpseTime { get; private set; }

        public void AddThing(Thing thing)
        {
            if (_things.Contains(thing))
                return;

            _things.Add(thing);
        }

        public void SetPatameter(CategoryOfObjects category)
        {
            switch (category)
            {
                case CategoryOfObjects.Corpses:
                    if (CorpseTime > 0 && CorpseTime < int.MaxValue)
                        Parameter = CorpseTime.ToStringTicksToDays();
                    else
                        Parameter = "-";
                    Parameter += $" ({Locations.Count})";
                    break;
                case CategoryOfObjects.Terrains:
                    Parameter = Locations.Count.ToString();
                    break;
                default:
                    Parameter = Count.ToString();
                    if (Count != Stacks)
                        Parameter = $"{Count} ({Stacks})";
                    break;
            }
        }

        public void CheсkAndSetCorpseTime(int ticks)
        {
            if (ticks > 0 && ticks < CorpseTime)
                CorpseTime = ticks;
        }

        public void CleanData()
        {
            _things.Clear();
            Locations.Clear();
            CorpseTime = int.MaxValue;
            Parameter = "0";
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref _label, "ZiT_Unit.Label");
        }
    }
}
