// Ignore Spelling: supertrains

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.GameContent;

namespace supertrains.src
{
    public class HandcarEntity : Entity, IWrenchOrientable
    {
        void IWrenchOrientable.Rotate(EntityAgent byEntity, BlockSelection blockSel, int dir)
        {
            // TODO ...
        }
    }
}
