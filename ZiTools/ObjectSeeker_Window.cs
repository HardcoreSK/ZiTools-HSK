using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace ZiTools
{
    public class ObjectSeeker_Window : Window
    {
        private Vector2 _scrollPosition = new Vector2();
        private string _text;
        private ObjectsDatabase _objectsDatabase;
        private float _headerHeight;

        public override Vector2 InitialSize => new Vector2(400f, 280f);

        public ObjectsDatabase ODB
        {
            get
            {
                if (_objectsDatabase == null)
                    _objectsDatabase = ZiTools_GameComponent.GetObjectsDatabase();

                return _objectsDatabase;
            }
        }

        public static void DrawWindow()
        {
            Find.WindowStack.Add(new ObjectSeeker_Window());
        }

        public ObjectSeeker_Window() : base()
        {
            doCloseX = true;
            preventDrawTutor = true;
            draggable = true;
            preventCameraMotion = false;
            closeOnAccept = false;
            _headerHeight = Text.CalcSize("Header").y;
        }

        protected override void SetInitialSizeAndPosition()
        {
            windowRect = new Rect(UI.screenWidth - InitialSize.x, UI.screenHeight - InitialSize.y - 150f, InitialSize.x, InitialSize.y);
        }

        public override void PreOpen()
        {
            base.PreOpen();
            ODB.Update();
            ODB.Clear();
        }

        public override void DoWindowContents(Rect inRect)
        {
            var units = ODB.GetUnitsByWord(_text);
            var unitsCount = units.Count();

            float textFieldH = 35f, buttonWidth = 50f, buttonHeigth = 50f, buttonX = inRect.xMax - 2f * (buttonWidth + 1f);
            Text.Font = GameFont.Medium;
            Rect titleRect = new Rect(inRect) { height = Text.LineHeight + 7f };
            Rect textFieldRect = new Rect(titleRect.x, titleRect.yMax, buttonX - textFieldH - 5f, textFieldH);
            Rect updateButtonRect = new Rect(textFieldRect.xMax + 2f, textFieldRect.y, textFieldH, textFieldH);

            Widgets.Label(titleRect, "ZiT_ObjectsSeekerLabel".Translate());
            if (Widgets.ButtonImageWithBG(updateButtonRect, Textures.Refresh))
            {
                ODB.Update();
                SoundDefOf.PageChange.PlayOneShotOnCamera();
            }
            TooltipHandler.TipRegion(updateButtonRect, "ZiT_UpdateButtonLabel".Translate());
            MouseoverSounds.DoRegion(updateButtonRect, SoundDefOf.Mouseover_Category);
            _text = Widgets.TextField(textFieldRect, _text); //it's here for bigger font
            Text.Font = GameFont.Small;

            float lineHeight = Text.LineHeight;
            Rect catButtRect = new Rect(buttonX, inRect.yMax - (buttonHeigth + 1f) * 4f, buttonWidth, buttonHeigth);
            var categoryName = $"{ODB.SelectedCategoryName} ({unitsCount})";
            Vector2 categorySize = Text.CalcSize(categoryName);
            Widgets.Label(new Rect(catButtRect.x + (inRect.xMax - catButtRect.x - categorySize.x) / 2f, (catButtRect.y - categorySize.y) / 2f, categorySize.x, categorySize.y), categoryName);

            for (int i = 0; i < 8; i++) //categories tab
            {
                CategoryOfObjects currentCategory = ODB.GetCategoryViaInt(i);
                if (ODB.SelectedCategory == currentCategory)
                {
                    GUI.color = Widgets.WindowBGFillColor;
                    GUI.DrawTexture(catButtRect, BaseContent.BlackTex);
                    GUI.color = new Color(1f, 0.9f, 0f);
                    Widgets.DrawBox(catButtRect, 3);
                    GUI.color = Color.white;
                }
                else
                {
                    GUI.color = Widgets.WindowBGFillColor;
                    GUI.DrawTexture(catButtRect, BaseContent.WhiteTex);
                    GUI.color = Color.white;
                    Widgets.DrawBox(catButtRect, 2);
                }

                if (Widgets.ButtonImage(catButtRect.ScaledBy(0.85f), ODB.GetCategoryTexture(currentCategory)))
                {
                    ODB.SelectedCategory = currentCategory;
                    SoundDefOf.Click.PlayOneShotOnCamera();
                }
                TooltipHandler.TipRegion(catButtRect, ODB.NamesOfCategoriesDict[currentCategory]);
                MouseoverSounds.DoRegion(catButtRect, SoundDefOf.Mouseover_Category);
                catButtRect.x = catButtRect.xMax + 1f;
                if (i % 2 == 1)
                {
                    catButtRect.x = buttonX;
                    catButtRect.y += catButtRect.height + 1f;
                }
            }

            Rect mainRect = new Rect(inRect) { yMin = textFieldRect.yMax, xMax = catButtRect.x };
            if (!ODB.IsSelectedCategoryHaveObjects())
            {
                Widgets.Label(mainRect, "ZiT_NotFoundString".Translate(ODB.SelectedCategoryName));
                return;
            }

            // Draw header
            var headerItemName = "ZiT_NameLabel".Translate();
            var headerItemCount = ODB.SelectedCategory == CategoryOfObjects.Corpses ? "ZiT_TimeUntilRotted".Translate() : "ZiT_CellsCountLabel".Translate();
            var headerRect = new Rect(mainRect) { height = _headerHeight };

            Widgets.Label(headerRect.LeftPartPixels(Text.CalcSize(headerItemName).x), headerItemName);
            Widgets.Label(headerRect.RightPartPixels(Text.CalcSize(headerItemCount).x + 16f /* scroll width */), headerItemCount);

            // Draw body
            var bodyRect = new Rect(mainRect) { yMin = headerRect.yMax };
            Rect scrollRect = new Rect(0.0f, 0.0f, bodyRect.width - 16f, unitsCount * lineHeight);
            Widgets.BeginScrollView(bodyRect, ref _scrollPosition, scrollRect, true);
            GUI.BeginGroup(scrollRect);

            var lineNum = 1;
            var lineRect = new Rect(scrollRect) { height = lineHeight };
            foreach (var unit in units)
            {
                try
                {
                    DrawObjectsList(lineRect, unit, lineNum);
                }
                catch (Exception ex)
                {
                    Log.ErrorOnce($"Objects seeker: {unit.Label} object throws the error. " + ex, unit.Label.GetHashCode());
                }
                lineRect.y += lineHeight;
                lineNum++;
            }

            GUI.EndGroup();
            Widgets.EndScrollView();
        }

        public override void PreClose()
        {
            base.PreClose();
            ODB.Clear();
        }

        public void ToggleFavourite(DBUnit unit)
        {
            if (!ODB.UnitsInFavourites.Contains(unit))
                ODB.UnitsInFavourites.Add(unit);
            else
                ODB.UnitsInFavourites.Remove(unit);
        }

        private void DrawObjectsList(Rect inRect, DBUnit unit, int lineNum)
        {
            string label = unit.Label;
            string param = unit.Parameter;

            Rect rectImage = new Rect(inRect.x, inRect.y, inRect.height, inRect.height);
            Rect rectParam = new Rect(inRect.width - Text.CalcSize(param).x - rectImage.width, inRect.y, Text.CalcSize(param).x, inRect.height);
            Rect rectLabel = new Rect(rectImage.xMax + 2f, inRect.y, inRect.width - 2 * rectImage.width - rectParam.width, inRect.height);
            Rect rectFavButton = new Rect(rectImage) { x = rectParam.xMax };
            Rect rectSearchButton = new Rect(inRect) { width = inRect.width - rectFavButton.width };

            if (lineNum % 2 == 0)
            {
                Widgets.DrawHighlight(inRect);
            }

            Widgets.Label(rectLabel.LeftPartPixels(rectLabel.width), label);
            Widgets.Label(rectParam.RightPartPixels(rectParam.width), param);

            TooltipHandler.TipRegion(rectLabel, label);
            if (unit == ODB.UnitToSeek)
                Widgets.DrawHighlightSelected(rectSearchButton);
            else
                Widgets.DrawHighlightIfMouseover(rectSearchButton);

            if (Widgets.ButtonInvisible(rectSearchButton))
            {
                if (unit != ODB.UnitToSeek)
                {
                    ODB.UnitToSeek = unit;
                    MapMarksManager.SetMarks(MapMarksManager.ObjectSeeker_MarkDef, ODB.Positions);
                    ObjectsDatabase.DoUpdateAction();
                    SoundDefOf.Designate_PlanAdd.PlayOneShotOnCamera();
                }
                else
                {
                    ODB.Clear();
                    SoundDefOf.Designate_PlanRemove.PlayOneShotOnCamera();
                }
            }

            unit.Icon?.DrawIcon(rectImage);

            var favColor = ODB.UnitsInFavourites.Contains(unit) ? Color.yellow : Color.white;
            if (Widgets.ButtonImage(rectFavButton.ScaledBy(0.85f), ODB.GetCategoryTexture(CategoryOfObjects.Favorites), favColor))
            {
                ToggleFavourite(unit);
                SoundDefOf.Tick_Tiny.PlayOneShotOnCamera();
            }
        }
    }
}
