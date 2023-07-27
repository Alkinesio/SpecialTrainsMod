using System;
using System.Diagnostics;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.ServerMods.NoObf;

namespace SuperTrains.Utilities
{
    public static class Blocks
    {
        #region About faces

        /// <returns>Block where is facing to (It is never null).</returns>
        public static Block getNeighborBlockAtFace(IWorldAccessor world, BlockPos position, BlockFacing atFace)
        {
            return world.BlockAccessor.GetBlock(position.AddCopy(atFace));
        }

        /// <returns>A block that is to North-East of the given position.</returns>
        public static Block getBlockToNE(IWorldAccessor world, BlockPos position)
        {
            return world.BlockAccessor.GetBlock(
                position + new BlockPos(
                    Blocks.DirectionToCoordinates(
                        new BlockFacing[] { BlockFacing.NORTH, BlockFacing.EAST }
                        )
                    )
                );
        }

        /// <returns>A block that is to North-West of the given position.</returns>
        public static Block getBlockToNW(IWorldAccessor world, BlockPos position)
        {
            return world.BlockAccessor.GetBlock(
                position + new BlockPos(
                    Blocks.DirectionToCoordinates(
                        new BlockFacing[] { BlockFacing.NORTH, BlockFacing.WEST }
                        )
                    )
                );
        }

        /// <returns>A block that is to South-West of the given position.</returns>
        public static Block getBlockToSW(IWorldAccessor world, BlockPos position)
        {
            return world.BlockAccessor.GetBlock(
                position + new BlockPos(
                    Blocks.DirectionToCoordinates(
                        new BlockFacing[] { BlockFacing.SOUTH, BlockFacing.WEST }
                        )
                    )
                );
        }

        /// <returns>A block that is to South-East of the given position.</returns>
        public static Block getBlockToSE(IWorldAccessor world, BlockPos position)
        {
            return world.BlockAccessor.GetBlock(
                position + new BlockPos(
                    Blocks.DirectionToCoordinates(
                        new BlockFacing[] { BlockFacing.SOUTH, BlockFacing.EAST }
                        )
                    )
                );
        }

        /// <returns>Blocks at North and East faces of a block where is to the given position.</returns>
        public static Block[] getBlocksOnNE(IWorldAccessor world, BlockPos position)
        {
            Block[] blocks = { getNeighborBlockAtFace(world, position, BlockFacing.NORTH), getNeighborBlockAtFace(world, position, BlockFacing.EAST) };
            return blocks;
        }

        /// <returns>Blocks at North and West faces of a block where is to the given position.</returns>
        public static Block[] getBlocksOnNW(IWorldAccessor world, BlockPos position)
        {
            Block[] blocks = { getNeighborBlockAtFace(world, position, BlockFacing.NORTH), getNeighborBlockAtFace(world, position, BlockFacing.WEST) };
            return blocks;
        }

        /// <returns>Blocks at South and West faces of a block where is to the given position.</returns>
        public static Block[] getBlocksOnSW(IWorldAccessor world, BlockPos position)
        {
            Block[] blocks = { getNeighborBlockAtFace(world, position, BlockFacing.SOUTH), getNeighborBlockAtFace(world, position, BlockFacing.WEST) };
            return blocks;
        }

        /// <returns>Blocks at South and East faces of a block where is to the given position.</returns>
        public static Block[] getBlocksOnSE(IWorldAccessor world, BlockPos position)
        {
            Block[] blocks = { getNeighborBlockAtFace(world, position, BlockFacing.SOUTH), getNeighborBlockAtFace(world, position, BlockFacing.EAST) };
            return blocks;
        }

        /// <returns>Blocks at Up and Down faces of a block where is to the given position.</returns>
        public static Block[] getBlocksOnUD(IWorldAccessor world, BlockPos position)
        {
            Block[] blocks = { getNeighborBlockAtFace(world, position, BlockFacing.UP), getNeighborBlockAtFace(world, position, BlockFacing.DOWN) };
            return blocks;
        }

        /// <returns>Block at East face of a block where is to the given position.</returns>
        public static Block getBlockToEast(IWorldAccessor world, BlockPos position)
        {
            return getNeighborBlockAtFace(world, position, BlockFacing.EAST);
        }

        /// <returns>Block at North face of a block where is to the given position.</returns>
        public static Block getBlockToNorth(IWorldAccessor world, BlockPos position)
        {
            return getNeighborBlockAtFace(world, position, BlockFacing.NORTH);
        }

        /// <returns>Block at West face of a block where is to the given position.</returns>
        public static Block getBlockToWest(IWorldAccessor world, BlockPos position)
        {
            return getNeighborBlockAtFace(world, position, BlockFacing.WEST);
        }

        /// <returns>Block at South face of a block where is to the given position.</returns>
        public static Block getBlockToSouth(IWorldAccessor world, BlockPos position)
        {
            return getNeighborBlockAtFace(world, position, BlockFacing.SOUTH);
        }

        /// <returns>Block at Up face of a block where is to the given position.</returns>
        public static Block getBlockToUp(IWorldAccessor world, BlockPos position)
        {
            return getNeighborBlockAtFace(world, position, BlockFacing.UP);
        }

        /// <returns>Block at Down face of a block where is to the given position.</returns>
        public static Block getBlockToDown(IWorldAccessor world, BlockPos position)
        {
            return getNeighborBlockAtFace(world, position, BlockFacing.DOWN);
        }

        /// <summary>
        /// Get direction from an angle (expressed in radiant).
        /// </summary>
        /// <returns>The closest direction (array of two block faces) from given angle (45 degree = north-east).
        /// <br/>In the case of horizontal directions (N,E,S,W) the second element will be null.
        /// <br/>There is an improbable case that returns null in both elements.
        /// </returns>
        public static BlockFacing[] DirectionsFromAngle(float radiant)
        {
            int num = GameMath.Mod((int)Math.Round(radiant * (180f / (float)Math.PI) / 45f), 8);
            BlockFacing[] faces = new BlockFacing[2];

            switch (num)
            {
                case 0:
                    faces[0] = BlockFacing.EAST;
                    faces[1] = null;
                    break;
                case 1:
                    faces[0] = BlockFacing.NORTH;
                    faces[1] = BlockFacing.EAST;
                    break;
                case 2:
                    faces[0] = BlockFacing.NORTH;
                    faces[1] = null;
                    break;
                case 3:
                    faces[0] = BlockFacing.NORTH;
                    faces[1] = BlockFacing.WEST;
                    break;
                case 4:
                    faces[0] = BlockFacing.WEST;
                    faces[1] = null;
                    break;
                case 5:
                    faces[0] = BlockFacing.SOUTH;
                    faces[1] = BlockFacing.WEST;
                    break;
                case 6:
                    faces[0] = BlockFacing.SOUTH;
                    faces[1] = null;
                    break;
                case 7:
                    faces[0] = BlockFacing.SOUTH;
                    faces[1] = BlockFacing.EAST;
                    break;
                default:
                    faces[0] = null;
                    faces[1] = null;
                    break;
            }

            return faces;
        }

        /// <summary>
        /// Get oblique direction from an angle (expressed in radiant).
        /// </summary>
        /// <returns>The closest oblique direction (array of two block faces) from given angle (45 degree = north-east).
        /// <br/>There is an improbable case that returns null in both elements.
        /// </returns>
        public static BlockFacing[] ObliqueFromAngle(float radiant)
        {
            int num = GameMath.Mod((int)Math.Round((radiant * (180f / (float)Math.PI) + 45) / 90f), 4);
            BlockFacing[] faces = new BlockFacing[2];

            switch (num)
            {
                case 0:
                    faces[0] = BlockFacing.NORTH;
                    faces[1] = BlockFacing.EAST;
                    break;
                case 1:
                    faces[0] = BlockFacing.NORTH;
                    faces[1] = BlockFacing.WEST;
                    break;
                case 2:
                    faces[0] = BlockFacing.SOUTH;
                    faces[1] = BlockFacing.WEST;
                    break;
                case 3:
                    faces[0] = BlockFacing.SOUTH;
                    faces[1] = BlockFacing.EAST;
                    break;
                default:
                    faces[0] = null;
                    faces[1] = null;
                    break;
            }

            return faces;
        }

        /// <summary>
        /// Convert a direction in Its string equivalence (direction is defined as array of two faces, the second one can be null).
        /// </summary>
        /// <param name="direction"></param>
        /// <returns>In low case the equivalent value of direction (like 'ne' for North-East).
        /// <br/>Could return null for invalid cases.
        /// </returns>
        public static String DirectionToString(BlockFacing[] direction)
        {
            if (direction.Length != 2)
                return null;

            if (direction[0] == BlockFacing.NORTH && direction[1] == BlockFacing.EAST)
                return "ne";
            if (direction[0] == BlockFacing.NORTH && direction[1] == BlockFacing.WEST)
                return "nw";
            if (direction[0] == BlockFacing.SOUTH && direction[1] == BlockFacing.EAST)
                return "se";
            if (direction[0] == BlockFacing.SOUTH && direction[1] == BlockFacing.WEST)
                return "sw";
            if (direction[0] == BlockFacing.EAST)
                return "e";
            if (direction[0] == BlockFacing.NORTH)
                return "n";
            if (direction[0] == BlockFacing.WEST)
                return "w";
            if (direction[0] == BlockFacing.SOUTH)
                return "s";

            return null;
        }

        /// <summary>
        /// Convert a string (low case like 'ne') in Its direction equivalence.
        /// </summary>
        /// <param name="text"></param>
        /// <returns>
        /// Could return null for invalid cases, else an array of two block faces (the second one could be null).
        /// </returns>
        public static BlockFacing[] StringToDirection(String text)
        {
            if (text == null || text.Length != 2)
                return null;

            BlockFacing[] direction = new BlockFacing[2];

            if (text == "ne")
            {
                direction[0] = BlockFacing.NORTH;
                direction[1] = BlockFacing.EAST;
                return direction;
            }
            if (text == "nw")
            {
                direction[0] = BlockFacing.NORTH;
                direction[1] = BlockFacing.WEST;
                return direction;
            }
            if (text == "se")
            {
                direction[0] = BlockFacing.SOUTH;
                direction[1] = BlockFacing.EAST;
                return direction;
            }
            if (text == "sw")
            {
                direction[0] = BlockFacing.NORTH;
                direction[1] = BlockFacing.WEST;
                return direction;
            }
            if (text == "e")
            {
                direction[0] = BlockFacing.EAST;
                direction[1] = null;
                return direction;
            }
            if (text == "n")
            {
                direction[0] = BlockFacing.NORTH;
                direction[1] = null;
                return direction;
            }
            if (text == "w")
            {
                direction[0] = BlockFacing.WEST;
                direction[1] = null;
                return direction;
            }
            if (text == "s")
            {
                direction[0] = BlockFacing.SOUTH;
                direction[1] = null;
                return direction;
            }

            return null;
        }

        /// <summary>
        /// Convert a block face to coordinates (x, y, z).
        /// </summary>
        /// <returns>Return a Vec3i (int x, int y, int z). Vec3i.Zero if not valid.</returns>
        public static Vec3i FaceToCoordinates(BlockFacing face)
        {
            if (face == BlockFacing.EAST)
                return new Vec3i(1, 0, 0);
            if (face == BlockFacing.NORTH)
                return new Vec3i(0, 0, -1);
            if (face == BlockFacing.WEST)
                return new Vec3i(-1, 0, 0);
            if (face == BlockFacing.SOUTH)
                return new Vec3i(0, 0, 1);
            if (face == BlockFacing.UP)
                return new Vec3i(0, 1, 0);
            if (face == BlockFacing.DOWN)
                return new Vec3i(0, -1, 0);

            return Vec3i.Zero;
        }

        /// <summary>
        /// Convert a direction to coordinates (x, y not supported, z).
        /// </summary>
        /// <returns>Return a Vec3i (int x, 0, int z). Vec3i.Zero if not valid.</returns>
        public static Vec3i DirectionToCoordinates(BlockFacing[] direction)
        {
            if (direction[0] == BlockFacing.NORTH && direction[1] == BlockFacing.EAST)
                return new Vec3i(1, 0, -1);
            if (direction[0] == BlockFacing.NORTH && direction[1] == BlockFacing.WEST)
                return new Vec3i(-1, 0, -1);
            if (direction[0] == BlockFacing.SOUTH && direction[1] == BlockFacing.WEST)
                return new Vec3i(-1, 0, 1);
            if (direction[0] == BlockFacing.SOUTH && direction[1] == BlockFacing.EAST)
                return new Vec3i(1, 0, 1);
            if (direction[0] == BlockFacing.NORTH)
                return new Vec3i(0, 0, -1);
            if (direction[0] == BlockFacing.WEST)
                return new Vec3i(-1, 0, 0);
            if (direction[0] == BlockFacing.SOUTH)
                return new Vec3i(0, 0, 1);
            if (direction[0] == BlockFacing.EAST)
                return new Vec3i(1, 0, 0);

            return Vec3i.Zero;
        }

        #endregion
    }

    public static class Directions
    {

        /// <returns>True if the oblique direction (NE, NW, SW, SE) is correct.</returns>
        public static bool isValidObliqueDirection(BlockFacing[] direction)
        {
            if (direction.Length != 2)
                return false;

            if (direction[0] == BlockFacing.NORTH && direction[1] == BlockFacing.EAST)
                return true;
            if (direction[0] == BlockFacing.NORTH && direction[1] == BlockFacing.WEST)
                return true;
            if (direction[0] == BlockFacing.SOUTH && direction[1] == BlockFacing.WEST)
                return true;
            if (direction[0] == BlockFacing.SOUTH && direction[1] == BlockFacing.EAST)
                return true;

            return false;
        }

    }

    public static class Rails
    {
        #region About faces

        /// <summary>
        /// Count how many rails (simple) are there at South and West faces of a given block.
        /// </summary>
        /// <returns>An integer that represents quantity of the simple rails.</returns>
        public static int countRailsOnSW(IWorldAccessor world, BlockPos position)
        {
            int counter = 0;
            Block[] blocks = Blocks.getBlocksOnSW(world, position);
            for (int i = 0; i < blocks.Length; i++)
                if (blocks[i] is SimpleRailsBlock)
                    counter++;
            return counter;
        }

        /// <summary>
        /// Count how many rails (simple) are there at North and East faces of a given block.
        /// </summary>
        /// <returns>An integer that represents quantity of the simple rails.</returns>
        public static int countRailsOnNE(IWorldAccessor world, BlockPos position)
        {
            int counter = 0;
            Block[] blocks = Blocks.getBlocksOnNE(world, position);
            for (int i = 0; i < blocks.Length; i++)
                if (blocks[i] is SimpleRailsBlock)
                    counter++;
            return counter;
        }

        /// <summary>
        /// Count how many rails (simple) are there at North and West faces of a given block.
        /// </summary>
        /// <returns>An integer that represents quantity of the simple rails.</returns>
        public static int countRailsOnNW(IWorldAccessor world, BlockPos position)
        {
            int counter = 0;
            Block[] blocks = Blocks.getBlocksOnNW(world, position);
            for (int i = 0; i < blocks.Length; i++)
                if (blocks[i] is SimpleRailsBlock)
                    counter++;
            return counter;
        }

        /// <summary>
        /// Count how many rails (simple) are there at South and East faces of a given block.
        /// </summary>
        /// <returns>An integer that represents quantity of the simple rails.</returns>
        public static int countRailsOnSE(IWorldAccessor world, BlockPos position)
        {
            int counter = 0;
            Block[] blocks = Blocks.getBlocksOnSE(world, position);
            for (int i = 0; i < blocks.Length; i++)
                if (blocks[i] is SimpleRailsBlock)
                    counter++;
            return counter;
        }

        /// <summary>
        /// Count how many rails (simple) are there at Up and Down faces of a given block.
        /// </summary>
        /// <returns>An integer that represents quantity of the simple rails.</returns>
        public static int countRailsOnUD(IWorldAccessor world, BlockPos position)
        {
            int counter = 0;
            Block[] blocks = Blocks.getBlocksOnUD(world, position);
            for (int i = 0; i < blocks.Length; i++)
                if (blocks[i] is SimpleRailsBlock)
                    counter++;
            return counter;
        }

        /// <returns>True if there is a simple rails block at East face of the block.</returns>
        public static bool isThereRailBlockToEast(IWorldAccessor world, BlockPos position)
        {
            return Blocks.getBlockToEast(world, position) is SimpleRailsBlock;
        }

        /// <returns>True if there is a simple rails block at North face of the block.</returns>
        public static bool isThereRailBlockToNorth(IWorldAccessor world, BlockPos position)
        {
            return Blocks.getBlockToNorth(world, position) is SimpleRailsBlock;
        }

        /// <returns>True if there is a simple rails block at West face of the block.</returns>
        public static bool isThereRailBlockToWest(IWorldAccessor world, BlockPos position)
        {
            return Blocks.getBlockToWest(world, position) is SimpleRailsBlock;
        }

        /// <returns>True if there is a simple rails block at South face of the block.</returns>
        public static bool isThereRailBlockToSouth(IWorldAccessor world, BlockPos position)
        {
            return Blocks.getBlockToSouth(world, position) is SimpleRailsBlock;
        }

        /// <returns>True if there is a simple rails block at North-East of the given position.</returns>
        public static bool isThereRailBlockToNE(IWorldAccessor world, BlockPos position)
        {
            return Blocks.getBlockToNE(world, position) is SimpleRailsBlock;
        }

        /// <returns>True if there is a simple rails block at North-West of the given position.</returns>
        public static bool isThereRailBlockToNW(IWorldAccessor world, BlockPos position)
        {
            return Blocks.getBlockToNW(world, position) is SimpleRailsBlock;
        }

        /// <returns>True if there is a simple rails block at South-West of the given position.</returns>
        public static bool isThereRailBlockToSW(IWorldAccessor world, BlockPos position)
        {
            return Blocks.getBlockToSW(world, position) is SimpleRailsBlock;
        }

        /// <returns>True if there is a simple rails block at South-East of the given position.</returns>
        public static bool isThereRailBlockToSE(IWorldAccessor world, BlockPos position)
        {
            return Blocks.getBlockToSE(world, position) is SimpleRailsBlock;
        }

        #endregion
        #region About generics

        /// <returns>True if the given block is a rails block.</returns>
        public static bool isBlockRailsBlock(Block block)
        {
            return block is SimpleRailsBlock;
        }

        /// <returns>True if there is a simple rails block at the given position.</returns>
        public static bool isThereRailBlock(IWorldAccessor world, BlockPos position)
        {
            return world.BlockAccessor.GetBlock(position.X, position.Y, position.Z) is SimpleRailsBlock;
        }

        /// <returns>True if the rail block is flat, else false.</returns>
        public static bool isFlatRailBlock(Block block)
        {
            // First check that provided block is a simple rails block
            if (!(block is SimpleRailsBlock))
                return false;

            // Then check that the code of the block is actually that of a flat rail block
            switch (block.Code.ToString().Split('-')[1])
            {
                case "flat_ns":
                    return true;
                case "flat_we":
                    return true;
            }

            return false;

        }

        /// <returns>
        /// A string that indicates the direction ('ns' or 'we') if the block is a flat rails block. 
        /// <br/>Could return null if there are problems.
        /// </returns>
        public static String getFlatRailsDirection(Block block)
        {
            // First check that provided block is a simple rails block
            if (!isFlatRailBlock(block))
                return null;

            // Then check that the code of the block is actually that of a flat rail block
            switch (block.Code.ToString().Split('-')[1])
            {
                case "flat_ns":
                    return "ns";
                case "flat_we":
                    return "we";
            }

            return null;

        }

        /// <returns>True if the rail block is curved, else false.</returns>
        public static bool isCurvedRailBlock(Block block)
        {
            // First check that provided block is a simple rails block
            if (!(block is SimpleRailsBlock))
                return false;

            // Then check that the code of the block is actually that of a curved rail block
            switch (block.Code.ToString().Split('-')[1])
            {
                case "curved_ne":
                    return true;
                case "curved_nw":
                    return true;
                case "curved_sw":
                    return true;
                case "curved_se":
                    return true;
            }

            return false;

        }

        /// <returns>
        /// A string that indicates the direction ('ne', 'nw', 'sw' or 'se') if the block is a curved rails block. 
        /// <br/>Could return null if there are problems.
        /// </returns>
        public static String getCurvedRailBlockDirection(Block block)
        {
            // First check that provided block is a simple rails block
            if (!(block is SimpleRailsBlock) || !isCurvedRailBlock(block))
                return null;

            // Then check that the code of the block is actually that of a curved rail block
            switch (block.Code.ToString().Split('-')[1])
            {
                case "curved_ne":
                    return "ne";
                case "curved_nw":
                    return "nw";
                case "curved_sw":
                    return "sw";
                case "curved_se":
                    return "se";
            }

            return null;
        }

        /// <returns>True if the rail block is raised, else false.</returns>
        public static bool isRaisedRailBlock(Block block)
        {
            // First check that provided block is a simple rails block
            if (!(block is SimpleRailsBlock))
                return false;

            // Then check that the code of the block is actually that of a raised rail block
            switch (block.Code.ToString().Split('-')[1])
            {
                case "raised_e":
                    return true;
                case "raised_n":
                    return true;
                case "raised_w":
                    return true;
                case "raised_s":
                    return true;
            }

            return false;

        }

        /// <returns>True if the rail block is curved or raised, else false.</returns>
        public static bool isCurvedOrRaisedBlock(Block block)
        {
            return isCurvedRailBlock(block) || isRaisedRailBlock(block);
        }

        #endregion
    }
}