using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.MathTools;
using Vintagestory.GameContent;


namespace SuperTrains
{
    public class SimpleRailsBlock : Block
    {
        /// <summary>Try and place the block.</summary>
        /// <returns>True if the blocked is correctly placed.</returns>
        public override bool TryPlaceBlock(IWorldAccessor world, IPlayer byPlayer, ItemStack itemstack, BlockSelection blockSel, ref string failureCode)
        {
            // Check if can place the block (by default, returns with error in the case)
            if (!this.CanPlaceBlock(world, byPlayer, blockSel, ref failureCode))
            {
                return false;
            }

            // Set orientation (flip if horizontal/vertical)
            BlockFacing targetFacing = Block.SuggestedHVOrientation(byPlayer, blockSel)[0];

            // Check for curves, raised and/or other types
            for (int i = 0; i < BlockFacing.HORIZONTALS.Length; i++)
            {
                BlockFacing facing = BlockFacing.HORIZONTALS[i];
                if (this.TryAttachPlaceToHorizontal(world, byPlayer, blockSel.Position, facing, targetFacing))
                {
                    return true;
                }
            }

            // Place the block with got values
            Block blockToPlace;
            if (targetFacing.Axis == EnumAxis.Z)
            {
                blockToPlace = world.GetBlock(base.CodeWithParts("flat_ns"));
            }
            else
            {
                blockToPlace = world.GetBlock(base.CodeWithParts("flat_we"));
            }
            blockToPlace.DoPlaceBlock(world, byPlayer, blockSel, itemstack);
            return true;
        }

        private bool TryAttachPlaceToHorizontal(IWorldAccessor world, IPlayer byPlayer, BlockPos position, BlockFacing toFacing, BlockFacing targetFacing)
        {
            // Make a copy of the block close to It where is facing (neighbor)
            BlockPos neibPos = position.AddCopy(toFacing);
            Block neibBlock = world.BlockAccessor.GetBlock(neibPos);

            // Check if the close block is a simple rail or not (returns in the case)
            // TODO: Place the rails if on this block there are other rails
            if (!(neibBlock is SimpleRailsBlock))
            {
                return false;
            }

            // etc ...
            BlockFacing fromFacing = toFacing.Opposite;
            BlockFacing[] neibDirFacings = this.getFacingsFromType(neibBlock.Variant["type"]);
            if (world.BlockAccessor.GetBlock(neibPos.AddCopy(neibDirFacings[0])) is SimpleRailsBlock && world.BlockAccessor.GetBlock(neibPos.AddCopy(neibDirFacings[1])) is SimpleRailsBlock)
            {
                return false;
            }
            BlockFacing neibFreeFace = this.getOpenedEndedFace(neibDirFacings, world, position.AddCopy(toFacing));
            if (neibFreeFace == null)
            {
                return false;
            }
            Block blockToPlace = this.getRailBlock(world, "curved_", toFacing, targetFacing);
            if (blockToPlace != null)
            {
                return this.placeIfSuitable(world, byPlayer, blockToPlace, position);
            }
            string dirs = neibBlock.Variant["type"].Split(new char[]
            {
                '_'
            })[1];
            BlockFacing neibKeepFace = (dirs[0] == neibFreeFace.Code[0]) ? BlockFacing.FromFirstLetter(dirs[1]) : BlockFacing.FromFirstLetter(dirs[0]);
            Block block = this.getRailBlock(world, "curved_", neibKeepFace, fromFacing);
            if (block == null)
            {
                return false;
            }
            block.DoPlaceBlock(world, byPlayer, new BlockSelection

            {
                Position = position.AddCopy(toFacing),
                Face = BlockFacing.UP
            }, null);
            return false;
        }

        private bool placeIfSuitable(IWorldAccessor world, IPlayer byPlayer, Block block, BlockPos pos)
        {
            string failureCode = "";
            BlockSelection blockSel = new BlockSelection
            {
                Position = pos,
                Face = BlockFacing.UP
            };
            if (block.CanPlaceBlock(world, byPlayer, blockSel, ref failureCode))
            {
                block.DoPlaceBlock(world, byPlayer, blockSel, null);
                return true;
            }
            return false;
        }

        private Block getRailBlock(IWorldAccessor world, string prefix, BlockFacing dir0, BlockFacing dir1)
        {
            Block block = world.GetBlock(base.CodeWithParts(prefix + dir0.Code[0].ToString() + dir1.Code[0].ToString()));
            if (block != null)
            {
                return block;
            }
            return world.GetBlock(base.CodeWithParts(prefix + dir1.Code[0].ToString() + dir0.Code[0].ToString()));
        }

        private BlockFacing getOpenedEndedFace(BlockFacing[] dirFacings, IWorldAccessor world, BlockPos blockPos)
        {
            if (!(world.BlockAccessor.GetBlock(blockPos.AddCopy(dirFacings[0])) is SimpleRailsBlock))
            {
                return dirFacings[0];
            }
            if (!(world.BlockAccessor.GetBlock(blockPos.AddCopy(dirFacings[1])) is SimpleRailsBlock))
            {
                return dirFacings[1];
            }
            return null;
        }

        private BlockFacing[] getFacingsFromType(string type)
        {
            string codes = type.Split(new char[]
            {
                '_'
            })[1];
            return new BlockFacing[]
            {
                BlockFacing.FromFirstLetter(codes[0]),
                BlockFacing.FromFirstLetter(codes[1])
            };
        }
    }
}