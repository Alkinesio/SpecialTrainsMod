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
            // Starts API
            base.Start(api);

            // Rails registration
            api.RegisterBlockClass("simplerails", typeof(SimpleRailsBlock));
            // Handcar registration
            api.RegisterBlockClass("handcar", typeof(Handcar));
            api.RegisterEntity("handcarEntity", typeof(HandcarEntity));
            // TODO ...
        }

        public string GetModID() => modId;
    }
}
