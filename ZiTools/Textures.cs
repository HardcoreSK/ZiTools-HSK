using UnityEngine;
using Verse;

namespace ZiTools
{
    [StaticConstructorOnStartup]
    public static class Textures
    {
        public static Texture2D Search;
        public static Texture2D Refresh;
        public static Texture2D IconFavorites;
        public static Texture2D IconAll;
        public static Texture2D IconBuildings;
        public static Texture2D IconTerrains;
        public static Texture2D IconPlants;
        public static Texture2D IconPawns;
        public static Texture2D IconCorpses;
        public static Texture2D IconOthers;

        static Textures()
        {
            Search = ContentFinder<Texture2D>.Get("UI/Lupa(not Pupa)");
            Refresh = ContentFinder<Texture2D>.Get("UI/Update Button", true);
            IconFavorites = ContentFinder<Texture2D>.Get("UI/Favourite Button");
            IconAll = ContentFinder<Texture2D>.Get("UI/All Button");
            IconBuildings = ContentFinder<Texture2D>.Get("UI/Designators/Deconstruct");
            IconTerrains = ContentFinder<Texture2D>.Get("UI/Designators/RemoveFloor");
            IconPlants = ContentFinder<Texture2D>.Get("Things/Plant/TreeOak/TreeOakA");
            IconPawns = ContentFinder<Texture2D>.Get("Things/Pawn/Animal/Muffalo/Muffalo_east");
            IconCorpses = ContentFinder<Texture2D>.Get("Things/Mote/ThoughtSymbol/Skull");
            IconOthers = ContentFinder<Texture2D>.Get("Things/Item/Chunk/ChunkSlag/MetalDebrisB");
        }
    }
}
