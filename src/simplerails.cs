using SuperTrains.Utilities;
using System;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.GameContent;

using Blocks = SuperTrains.Utilities.Blocks;


namespace SuperTrains
{
    public class SimpleRailsBlock : Block
    {

        // APIs
        ICoreAPI API;
        ICoreServerAPI sAPI; // Client side
        ICoreClientAPI cAPI; // Server side

        public override void OnLoaded(ICoreAPI api)
        {
            base.OnLoaded(api);
            this.API = api;

            if (api.Side == EnumAppSide.Server)
            {
                sAPI = api as ICoreServerAPI;
            }
            else
            {
                cAPI = api as ICoreClientAPI;
            }
        }

        /// <summary>Try and place the block.</summary>
        /// <returns>True if the blocked is correctly placed.</returns>
        public override bool TryPlaceBlock(IWorldAccessor world, IPlayer byPlayer, ItemStack itemstack, BlockSelection blockSel, ref string failureCode)
        {
            // Check if can place the block (by default, returns with error in the case)
            if (!this.CanPlaceBlock(world, byPlayer, blockSel, ref failureCode))
            {
                return false;
            }

            // Cannot place rails on other rails
            if (Rails.countRailsOnUD(world, blockSel.Position) > 0)
            {
                return false;
            }

            // Set orientation (flip if horizontal/vertical)
            BlockFacing targetFacing = Block.SuggestedHVOrientation(byPlayer, blockSel)[0];

            // Check for curves
            if (this.TryPlaceCurve(world, byPlayer, blockSel.Position, BlockFacing.HORIZONTALS))
            {
                return true;
            }

            // Place the block as flat with got values
            world.GetBlock(base.CodeWithParts(targetFacing.Axis == EnumAxis.Z ? "flat_ns" : "flat_we"))
                .DoPlaceBlock(world, byPlayer, blockSel, itemstack);
            return true;
        }

        public override bool OnBlockBrokenWith(IWorldAccessor world, Entity byEntity, ItemSlot itemslot, BlockSelection blockSel, float dropQuantityMultiplier = 1)
        {
            // Call default method
            if (base.OnBlockBrokenWith(world, byEntity, itemslot, blockSel, dropQuantityMultiplier) == false)
                return false;

            // Update linked rails (set horizontal/vertical orientation)
            // TODO ...
            return true;
        }

        /// <summary>
        /// Try to place rails as curve type if possible, else nothing will happen.
        /// </summary>
        /// <returns>True if placed as curve.</returns>
        private bool TryPlaceCurve(IWorldAccessor world, IPlayer byPlayer, BlockPos position, BlockFacing[] blockFacing)
        {
            // Set counting on oblique faces (NE, SE, SW, NW)
            int NERails = Rails.countRailsOnNE(world, position);
            int SERails = Rails.countRailsOnSE(world, position);
            int SWRails = Rails.countRailsOnSW(world, position);
            int NWRails = Rails.countRailsOnNW(world, position);

            // Returns if there are not enough rails on oblique faces to make a curve
            // For example if there are two rails (on North and East faces) then can make the curve
            if (NERails < 2 && SERails < 2 && SWRails < 2 && NWRails < 2)
                return false;

            // Set the target curve (NE, SE, SW, NW)
            string targetCurve = null;
            // Get first player orientation (used for close rails count greater than 2 pieces)
            string playerFacing = Blocks.DirectionToString(Blocks.ObliqueFromAngle(GameMath.Mod(byPlayer.Entity.Pos.Yaw, (float)Math.PI * 2f)));
            /* Additional info: 
                    - x: placing curve;
                    - X: rails;
                    - -: not rails;
            */
            /* Case NESW:
             * - X -
             * X x X    => Will depend from player facing
             * - X -
             */
            if (NERails >= 2 && NWRails >= 2 && SERails >= 2 && SWRails >= 2)
            {
                targetCurve = playerFacing;
            }
            /* Case NEW:
            * - X -
            * X x X    => Will depend from player facing
            * - - -
            */
            else if (NERails >= 2 && NWRails >= 2)
            {
                if (playerFacing == "sw")
                {
                    targetCurve = "nw";
                }
                else if (playerFacing == "se")
                {
                    targetCurve = "ne";
                }
                else
                {
                    targetCurve = playerFacing;
                }
            }
            /* Case NES:
            * - X -
            * - x X    => Will depend from player facing
            * - X -
            */
            else if (NERails >= 2 && SERails >= 2)
            {
                if (playerFacing == "nw")
                {
                    targetCurve = "ne";
                }
                else if (playerFacing == "sw")
                {
                    targetCurve = "se";
                }
                else
                {
                    targetCurve = playerFacing;
                }
            }
            /* Case ESW:
            * - - -
            * X x X    => Will depend from player facing
            * - X -
            */
            else if (SERails >= 2 && SWRails >= 2)
            {
                if (playerFacing == "ne")
                {
                    targetCurve = "se";
                }
                else if (playerFacing == "nw")
                {
                    targetCurve = "sw";
                }
                else
                {
                    targetCurve = playerFacing;
                }
            }
            /* Case NSW:
            * - X -
            * X x -    => Will depend from player facing
            * - X -
            */
            else if (NWRails >= 2 && SWRails >= 2)
            {
                if (playerFacing == "ne")
                {
                    targetCurve = "nw";
                }
                else if (playerFacing == "se")
                {
                    targetCurve = "sw";
                }
                else
                {
                    targetCurve = playerFacing;
                }
            }
            /* Case NW:
            * - X -
            * X x -    => Will be NW
            * - - -
            */
            else if (NWRails >= 2)
            {
                targetCurve = "nw";
            }
            /* Case NE:
            * - X -
            * - x X    => Will be NE
            * - - -
            */
            else if (NERails >= 2)
            {
                targetCurve = "ne";
            }
            /* Case SE:
            * - - -
            * - x X    => Will be SE
            * - X -
            */
            else if (SERails >= 2)
            {
                targetCurve = "se";
            }
            /* Case SW:
            * - - -
            * X x -    => Will be SW
            * - X -
            */
            else if (SWRails >= 2)
            {
                targetCurve = "sw";
            }
            // Impossible case
            else
            {
                return false;
            }

            // Finally place the curve and returns true if everything is fine
            Block curveToPlace = world.GetBlock(base.CodeWithParts("curved_" + targetCurve));
            if (curveToPlace != null)
            {
                // If placed correctly the curve
                if (this.placeIfSuitable(world, byPlayer, curveToPlace, position))
                {
                    // Update linked rails (set horizontal/vertical orientation)
                    BlockFacing[] orientation = new BlockFacing[2];
                    switch (targetCurve)
                    {
                        case "ne":
                            orientation[0] = BlockFacing.NORTH;
                            orientation[1] = BlockFacing.EAST;
                            world.BlockAccessor.ExchangeBlock(world.BlockAccessor.GetBlock(base.CodeWithParts("flat_ns")).Id, position + new BlockPos(Blocks.FaceToCoordinates(orientation[0])));
                            world.BlockAccessor.ExchangeBlock(world.BlockAccessor.GetBlock(base.CodeWithParts("flat_we")).Id, position + new BlockPos(Blocks.FaceToCoordinates(orientation[1])));
                            break;
                        case "nw":
                            orientation[0] = BlockFacing.NORTH;
                            orientation[1] = BlockFacing.WEST;
                            world.BlockAccessor.ExchangeBlock(world.BlockAccessor.GetBlock(base.CodeWithParts("flat_ns")).Id, position + new BlockPos(Blocks.FaceToCoordinates(orientation[0])));
                            world.BlockAccessor.ExchangeBlock(world.BlockAccessor.GetBlock(base.CodeWithParts("flat_we")).Id, position + new BlockPos(Blocks.FaceToCoordinates(orientation[1])));
                            break;
                        case "sw":
                            orientation[0] = BlockFacing.SOUTH;
                            orientation[1] = BlockFacing.WEST;
                            world.BlockAccessor.ExchangeBlock(world.BlockAccessor.GetBlock(base.CodeWithParts("flat_ns")).Id, position + new BlockPos(Blocks.FaceToCoordinates(orientation[0])));
                            world.BlockAccessor.ExchangeBlock(world.BlockAccessor.GetBlock(base.CodeWithParts("flat_we")).Id, position + new BlockPos(Blocks.FaceToCoordinates(orientation[1])));
                            break;
                        case "se":
                            orientation[0] = BlockFacing.SOUTH;
                            orientation[1] = BlockFacing.EAST;
                            world.BlockAccessor.ExchangeBlock(world.BlockAccessor.GetBlock(base.CodeWithParts("flat_ns")).Id, position + new BlockPos(Blocks.FaceToCoordinates(orientation[0])));
                            world.BlockAccessor.ExchangeBlock(world.BlockAccessor.GetBlock(base.CodeWithParts("flat_we")).Id, position + new BlockPos(Blocks.FaceToCoordinates(orientation[1])));
                            break;
                    }

                    return true;
                }
            }

            // Impossible case
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
                return block.DoPlaceBlock(world, byPlayer, blockSel, null);
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

        /// <returns>Facing North or East if the block is simple rails (else null).</returns>
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