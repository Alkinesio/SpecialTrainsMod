using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;

namespace SuperTrains
{
    public class SuperTrainsMod : ModSystem
    {

        private string modId = ModInfo.ToModID("supertrains");

        public override void Start(ICoreAPI api)
        {
            base.Start(api);
            api.RegisterBlockClass("simplerails", typeof(SimpleRailsBlock));
        }

        public string getModID() { return modId; }
    }
}
