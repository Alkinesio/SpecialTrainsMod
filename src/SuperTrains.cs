// Ignore Spelling: api

using supertrains.src;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

namespace SuperTrains
{
    public class SuperTrainsMod : ModSystem
    {

        private readonly string modId = ModInfo.ToModID("supertrains");

        public override void Start(ICoreAPI api)
        {
            base.Start(api);
            api.RegisterBlockClass("simplerails", typeof(SimpleRailsBlock));
            api.RegisterBlockClass("handcarItem", typeof(HandcarItem));
            api.RegisterEntity("handcarEntity", typeof(HandcarEntity));
            api.RegisterBlockClass("handcarBlock", typeof(Handcar));
        }

        public string GetModID() { return modId; }
    }
}
