using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using nialsorva.FCEEasyMods;
using nialscorva.FCEEasyMods.Behaviors;

namespace nialscorva.FCEEasyMods
{
    using StateCoroutine = System.Collections.Generic.IEnumerator<ModMachineEntity.StateReturn>;
    using Coroutine = System.Collections.Generic.IEnumerator<int>;

    public class DisplayPanelMachine : ModMachineEntity//, PowerConsumerInterface
    {
        private GameObject ViewObject;
        TextMesh readout;
        TextMesh dropshadow;

        HoverBehavior hover;

        int scrollIndex = 0;
        public string HeaderLine { get; protected set; }
        public List<string> Lines { get { return lines; }
            set
            {
                lines = value;
                dirtyLines = true;
            }
        }

        List<string> lines = new List<string>();
        bool dirtyLines = false;

        ItemFilterSet filter = new ItemFilterSet(
            ItemFilters.ALL,
            new ItemDelegateFilter("Bars", (i) => ItemFilters.IsBar(i)),
            new ItemDelegateFilter("Crafted", (i) => ItemFilters.IsCrafted(i) && ! ItemFilters.IsBar(i)),
            new ItemDelegateFilter("Fuel", (i) => ItemFilters.IsBiomass(i) || ItemFilters.IsHighCalorie(i)),
            new ItemDelegateFilter("Smeltable", (i) => ItemFilters.IsSmeltable(i)),
            new ItemDelegateFilter("Ingredients", (i) => ItemFilters.IsCrystal(i) || ItemFilters.IsGems(i)),
            new ItemDelegateFilter("Organic Items", ItemFilters.IsOrganic)
        );

        readonly Vector3 DISPLAY_SCALE = new Vector3(2.5f, 6.0f, 2.5f);

        public DisplayPanelMachine(ModCreateSegmentEntityParameters mcsep)
          : base(mcsep)
        {
            hover = new HoverBehavior(this);
            StartAnimationCoroutine(AnimateFading());
            StartAnimationCoroutine(AnimateTextUpdate());
        }

        protected override void InitializeGameObjects()
        {
            log("Initializing GameObjects");
            if (primaryGO== null)
            {
                log("InitializeGameObject called with null primaryGO");
                return;
            }

            ViewObject = primaryGO.GetDescendantGameObject("_Readout");
            if (ViewObject == null)
            {
                log("Could not find _Readout object.  eSegmentEntity={0}, objectType={1}", this.mType, this.mObjectType);
                return;
            }
            ViewObject.SetActive(false);
            ViewObject.transform.localScale = Vector3.zero;

            // Scale text down by the amount that we scaled the display up
            // Then make the text 7.5% of that because it seems to look right and I have no idea how text sizes. 
            Vector3 invertedDisplayScale= 0.075f * new Vector3(1f / DISPLAY_SCALE.x, 1f / DISPLAY_SCALE.y, 1f / DISPLAY_SCALE.z);
            
            readout = ViewObject.GetDescendantGameObject("ReadoutText").GetComponent<TextMesh>();
            readout.transform.localScale = invertedDisplayScale;
            readout.transform.localPosition = new Vector3(readout.transform.localPosition.x, readout.transform.localPosition.y, readout.transform.localPosition.z-0.01f);

            dropshadow = ViewObject.GetDescendantGameObject("ReadoutText_DS").GetComponent<TextMesh>();
            dropshadow.transform.localScale = invertedDisplayScale;
            SetSignText("Searching for MassStorageCrate...");
            log("Display Panel readout localposition={0}, dropshadow={1}", readout.transform.localPosition, dropshadow.transform.localPosition);
        }

        public void SetSignText(string text)
        {
            readout.text = text;
            dropshadow.text = text;
        }

        public Coroutine AnimateFading()
        {
            float readoutScale = 0.0f;
            float targetScale = 1.0f;
            while(true)
            {
                if(hover.IsActive() && readoutScale < targetScale)
                {
                    readoutScale = Math.Min(targetScale, readoutScale + Time.deltaTime);
                    ViewObject.transform.localScale = readoutScale * DISPLAY_SCALE;

                    ViewObject.SetActive(true);
                }

                if (!hover.IsActive() && readoutScale > 0.0f)
                {
                    readoutScale = Math.Max(0.1f, readoutScale - Time.deltaTime);
                    ViewObject.transform.localScale = readoutScale * DISPLAY_SCALE;

                    if (readoutScale < 0.1f)
                    {
                        ViewObject.SetActive(false);
                        readoutScale = 0.0f;
                    }
                }
                
                yield return 0;
            }
        }

        

        public Coroutine AnimateTextUpdate()
        {
            //Display string definition
            while (true)
            {
                if (hover.IsActive() && dirtyLines)
                {
                    int maxLines = 10;
                    string str1 = HeaderLine;

                    // grab a reference to lines instead of locking on it.  UpdateContentTotals running on the low-freq thread
                    // can change it without breaking our iteration
                    List<string> textLines = Lines;
                    dirtyLines = false;
                    int lineCount = textLines.Count();
                    if (lineCount > 0)
                    {
                        scrollIndex = scrollIndex % lineCount;
                        if (scrollIndex < 0) scrollIndex += lineCount;

                        int printLines = Math.Min(lineCount, maxLines);
                        for (int i = 0; i < printLines; ++i)
                        {
                            str1 += textLines[(scrollIndex + i) % lineCount] + "\n";
                        }
                    }
                    SetSignText(str1);
                }
                yield return 100;
            }
        }
    }
}