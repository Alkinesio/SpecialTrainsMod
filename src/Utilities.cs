using System;
using System.Diagnostics;
using Vintagestory.API.Common;
using Vintagestory.API.MathTools;

namespace SuperTrains.Utilities
{

    public static class Debugging
    {
        public static void Message(string content)
        {
            Debug.WriteLine(content);
        }
    }

    public static class Blocks
    {
        #region About faces

        /// <returns>Block where is facing to (It is never null).</returns>
        public static Block GetNeighborBlockAtFace(IWorldAccessor world, BlockPos position, BlockFacing atFace)
        {
            return world.BlockAccessor.GetBlock(position.AddCopy(atFace));
        }

        /// <returns>A block that is to North-East of the given position.</returns>
        public static Block GetBlockToNE(IWorldAccessor world, BlockPos position)
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
        public static Block GetBlockToNW(IWorldAccessor world, BlockPos position)
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
        public static Block GetBlockToSW(IWorldAccessor world, BlockPos position)
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
        public static Block GetBlockToSE(IWorldAccessor world, BlockPos position)
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
        public static Block[] GetBlocksOnNE(IWorldAccessor world, BlockPos position)
        {
            Block[] blocks = { GetNeighborBlockAtFace(world, position, BlockFacing.NORTH), GetNeighborBlockAtFace(world, position, BlockFacing.EAST) };
            return blocks;
        }

        /// <returns>Blocks at North and West faces of a block where is to the given position.</returns>
        public static Block[] GetBlocksOnNW(IWorldAccessor world, BlockPos position)
        {
            Block[] blocks = { GetNeighborBlockAtFace(world, position, BlockFacing.NORTH), GetNeighborBlockAtFace(world, position, BlockFacing.WEST) };
            return blocks;
        }

        /// <returns>Blocks at South and West faces of a block where is to the given position.</returns>
        public static Block[] GetBlocksOnSW(IWorldAccessor world, BlockPos position)
        {
            Block[] blocks = { GetNeighborBlockAtFace(world, position, BlockFacing.SOUTH), GetNeighborBlockAtFace(world, position, BlockFacing.WEST) };
            return blocks;
        }

        /// <returns>Blocks at South and East faces of a block where is to the given position.</returns>
        public static Block[] GetBlocksOnSE(IWorldAccessor world, BlockPos position)
        {
            Block[] blocks = { GetNeighborBlockAtFace(world, position, BlockFacing.SOUTH), GetNeighborBlockAtFace(world, position, BlockFacing.EAST) };
            return blocks;
        }

        /// <returns>Blocks at Up and Down faces of a block where is to the given position.</returns>
        public static Block[] GetBlocksOnUD(IWorldAccessor world, BlockPos position)
        {
            Block[] blocks = { GetNeighborBlockAtFace(world, position, BlockFacing.UP), GetNeighborBlockAtFace(world, position, BlockFacing.DOWN) };
            return blocks;
        }

        /// <returns>Block at East face of a block where is to the given position.</returns>
        public static Block GetBlockToEast(IWorldAccessor world, BlockPos position)
        {
            return GetNeighborBlockAtFace(world, position, BlockFacing.EAST);
        }

        /// <returns>Block at North face of a block where is to the given position.</returns>
        public static Block GetBlockToNorth(IWorldAccessor world, BlockPos position)
        {
            return GetNeighborBlockAtFace(world, position, BlockFacing.NORTH);
        }

        /// <returns>Block at West face of a block where is to the given position.</returns>
        public static Block GetBlockToWest(IWorldAccessor world, BlockPos position)
        {
            return GetNeighborBlockAtFace(world, position, BlockFacing.WEST);
        }

        /// <returns>Block at South face of a block where is to the given position.</returns>
        public static Block GetBlockToSouth(IWorldAccessor world, BlockPos position)
        {
            return GetNeighborBlockAtFace(world, position, BlockFacing.SOUTH);
        }

        /// <returns>Block at Up face of a block where is to the given position.</returns>
        public static Block GetBlockToUp(IWorldAccessor world, BlockPos position)
        {
            return GetNeighborBlockAtFace(world, position, BlockFacing.UP);
        }

        /// <returns>Block at Down face of a block where is to the given position.</returns>
        public static Block GetBlockToDown(IWorldAccessor world, BlockPos position)
        {
            return GetNeighborBlockAtFace(world, position, BlockFacing.DOWN);
        }

        /// <returns>Block at Up-East face of a block where is to the given position.</returns>
        public static Block GetBlockToUE(IWorldAccessor world, BlockPos position)
        {
            return GetNeighborBlockAtFace(world, position + new BlockPos(FaceToCoordinates(BlockFacing.UP)), BlockFacing.EAST);
        }

        /// <returns>Block at Up-North face of a block where is to the given position.</returns>
        public static Block GetBlockToUN(IWorldAccessor world, BlockPos position)
        {
            return GetNeighborBlockAtFace(world, position + new BlockPos(FaceToCoordinates(BlockFacing.UP)), BlockFacing.NORTH);
        }

        /// <returns>Block at Up-West face of a block where is to the given position.</returns>
        public static Block GetBlockToUW(IWorldAccessor world, BlockPos position)
        {
            return GetNeighborBlockAtFace(world, position + new BlockPos(FaceToCoordinates(BlockFacing.UP)), BlockFacing.WEST);
        }

        /// <returns>Block at Up-South face of a block where is to the given position.</returns>
        public static Block GetBlockToUS(IWorldAccessor world, BlockPos position)
        {
            return GetNeighborBlockAtFace(world, position + new BlockPos(FaceToCoordinates(BlockFacing.UP)), BlockFacing.SOUTH);
        }

        /// <returns>Block at Down-East face of a block where is to the given position.</returns>
        public static Block GetBlockToDE(IWorldAccessor world, BlockPos position)
        {
            return GetNeighborBlockAtFace(world, position + new BlockPos(FaceToCoordinates(BlockFacing.DOWN)), BlockFacing.EAST);
        }

        /// <returns>Block at Down-North face of a block where is to the given position.</returns>
        public static Block GetBlockToDN(IWorldAccessor world, BlockPos position)
        {
            return GetNeighborBlockAtFace(world, position + new BlockPos(FaceToCoordinates(BlockFacing.DOWN)), BlockFacing.NORTH);
        }

        /// <returns>Block at Down-West face of a block where is to the given position.</returns>
        public static Block GetBlockToDW(IWorldAccessor world, BlockPos position)
        {
            return GetNeighborBlockAtFace(world, position + new BlockPos(FaceToCoordinates(BlockFacing.DOWN)), BlockFacing.WEST);
        }

        /// <returns>Block at Down-South face of a block where is to the given position.</returns>
        public static Block GetBlockToDS(IWorldAccessor world, BlockPos position)
        {
            return GetNeighborBlockAtFace(world, position + new BlockPos(FaceToCoordinates(BlockFacing.DOWN)), BlockFacing.SOUTH);
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
        public static bool IsValidObliqueDirection(BlockFacing[] direction)
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
        public static int CountRailsOnSW(IWorldAccessor world, BlockPos position)
        {
            int counter = 0;
            Block[] blocks = Blocks.GetBlocksOnSW(world, position);
            for (int i = 0; i < blocks.Length; i++)
                if (blocks[i] is SimpleRailsBlock)
                    counter++;
            return counter;
        }

        /// <summary>
        /// Count how many rails (simple) are there at North and East faces of a given block.
        /// </summary>
        /// <returns>An integer that represents quantity of the simple rails.</returns>
        public static int CountRailsOnNE(IWorldAccessor world, BlockPos position)
        {
            int counter = 0;
            Block[] blocks = Blocks.GetBlocksOnNE(world, position);
            for (int i = 0; i < blocks.Length; i++)
                if (blocks[i] is SimpleRailsBlock)
                    counter++;
            return counter;
        }

        /// <summary>
        /// Count how many rails (simple) are there at North and West faces of a given block.
        /// </summary>
        /// <returns>An integer that represents quantity of the simple rails.</returns>
        public static int CountRailsOnNW(IWorldAccessor world, BlockPos position)
        {
            int counter = 0;
            Block[] blocks = Blocks.GetBlocksOnNW(world, position);
            for (int i = 0; i < blocks.Length; i++)
                if (blocks[i] is SimpleRailsBlock)
                    counter++;
            return counter;
        }

        /// <summary>
        /// Count how many rails (simple) are there at South and East faces of a given block.
        /// </summary>
        /// <returns>An integer that represents quantity of the simple rails.</returns>
        public static int CountRailsOnSE(IWorldAccessor world, BlockPos position)
        {
            int counter = 0;
            Block[] blocks = Blocks.GetBlocksOnSE(world, position);
            for (int i = 0; i < blocks.Length; i++)
                if (blocks[i] is SimpleRailsBlock)
                    counter++;
            return counter;
        }

        /// <summary>
        /// Count how many rails (simple) are there at Up and Down faces of a given block.
        /// </summary>
        /// <returns>An integer that represents quantity of the simple rails.</returns>
        public static int CountRailsOnUD(IWorldAccessor world, BlockPos position)
        {
            int counter = 0;
            Block[] blocks = Blocks.GetBlocksOnUD(world, position);
            for (int i = 0; i < blocks.Length; i++)
                if (blocks[i] is SimpleRailsBlock)
                    counter++;
            return counter;
        }

        /// <returns>True if there is a simple rails block at East face of the block.</returns>
        public static bool IsThereRailBlockToEast(IWorldAccessor world, BlockPos position)
        {
            return Blocks.GetBlockToEast(world, position) is SimpleRailsBlock;
        }

        /// <returns>True if there is a simple rails block at North face of the block.</returns>
        public static bool IsThereRailBlockToNorth(IWorldAccessor world, BlockPos position)
        {
            return Blocks.GetBlockToNorth(world, position) is SimpleRailsBlock;
        }

        /// <returns>True if there is a simple rails block at West face of the block.</returns>
        public static bool IsThereRailBlockToWest(IWorldAccessor world, BlockPos position)
        {
            return Blocks.GetBlockToWest(world, position) is SimpleRailsBlock;
        }

        /// <returns>True if there is a simple rails block at South face of the block.</returns>
        public static bool IsThereRailBlockToSouth(IWorldAccessor world, BlockPos position)
        {
            return Blocks.GetBlockToSouth(world, position) is SimpleRailsBlock;
        }

        /// <returns>True if there is a simple rails block at Up face of the block.</returns>
        public static bool IsThereRailBlockToUp(IWorldAccessor world, BlockPos position)
        {
            return Blocks.GetBlockToUp(world, position) is SimpleRailsBlock;
        }

        /// <returns>True if there is a simple rails block at Down face of the block.</returns>
        public static bool IsThereRailBlockToDown(IWorldAccessor world, BlockPos position)
        {
            return Blocks.GetBlockToDown(world, position) is SimpleRailsBlock;
        }

        /// <returns>True if there is a simple rails block at North-East of the given position.</returns>
        public static bool IsThereRailBlockToNE(IWorldAccessor world, BlockPos position)
        {
            return Blocks.GetBlockToNE(world, position) is SimpleRailsBlock;
        }

        /// <returns>True if there is a simple rails block at North-West of the given position.</returns>
        public static bool IsThereRailBlockToNW(IWorldAccessor world, BlockPos position)
        {
            return Blocks.GetBlockToNW(world, position) is SimpleRailsBlock;
        }

        /// <returns>True if there is a simple rails block at South-West of the given position.</returns>
        public static bool IsThereRailBlockToSW(IWorldAccessor world, BlockPos position)
        {
            return Blocks.GetBlockToSW(world, position) is SimpleRailsBlock;
        }

        /// <returns>True if there is a simple rails block at South-East of the given position.</returns>
        public static bool IsThereRailBlockToSE(IWorldAccessor world, BlockPos position)
        {
            return Blocks.GetBlockToSE(world, position) is SimpleRailsBlock;
        }

        #endregion
        #region About generics

        /// <returns>True if the given block is a rails block.</returns>
        public static bool IsBlockRailsBlock(Block block)
        {
            return block is SimpleRailsBlock;
        }

        /// <returns>True if there is a simple rails block at the given position.</returns>
        public static bool IsThereRailBlock(IWorldAccessor world, BlockPos position)
        {
            return world.BlockAccessor.GetBlock(position.X, position.Y, position.Z) is SimpleRailsBlock;
        }

        /// <returns>Null if there is not, else the simple rails block EAST at the given position.</returns>
        public static SimpleRailsBlock GetRailBlockToEast(IWorldAccessor world, BlockPos position)
        {
            if (!Rails.IsThereRailBlockToEast(world, position))
            {
                return null;
            }

            return Blocks.GetBlockToEast(world, position) as SimpleRailsBlock;
        }

        /// <returns>Null if there is not, else the simple rails block NORTH at the given position.</returns>
        public static SimpleRailsBlock GetRailBlockToNorth(IWorldAccessor world, BlockPos position)
        {
            if (!Rails.IsThereRailBlockToNorth(world, position))
            {
                return null;
            }

            return Blocks.GetBlockToNorth(world, position) as SimpleRailsBlock;
        }

        /// <returns>Null if there is not, else the simple rails block WEST at the given position.</returns>
        public static SimpleRailsBlock GetRailBlockToWest(IWorldAccessor world, BlockPos position)
        {
            if (!Rails.IsThereRailBlockToWest(world, position))
            {
                return null;
            }

            return Blocks.GetBlockToWest(world, position) as SimpleRailsBlock;
        }

        /// <returns>Null if there is not, else the simple rails block SOUTH at the given position.</returns>
        public static SimpleRailsBlock GetRailBlockToSouth(IWorldAccessor world, BlockPos position)
        {
            if (!Rails.IsThereRailBlockToSouth(world, position))
            {
                return null;
            }

            return Blocks.GetBlockToSouth(world, position) as SimpleRailsBlock;
        }

        /// <returns>Null if there is not, else the simple rails block UP at the given position.</returns>
        public static SimpleRailsBlock GetRailBlockToUp(IWorldAccessor world, BlockPos position)
        {
            if (!Rails.IsThereRailBlockToUp(world, position))
            {
                return null;
            }

            return Blocks.GetBlockToUp(world, position) as SimpleRailsBlock;
        }

        /// <returns>Null if there is not, else the simple rails block DOWN at the given position.</returns>
        public static SimpleRailsBlock GetRailBlockToDown(IWorldAccessor world, BlockPos position)
        {
            if (!Rails.IsThereRailBlockToDown(world, position))
            {
                return null;
            }

            return Blocks.GetBlockToDown(world, position) as SimpleRailsBlock;
        }

        /// <returns>Null if there is not, else the simple rails block at the given position.</returns>
        public static SimpleRailsBlock GetRailBlock(IWorldAccessor world, BlockPos position)
        {
            if (!Rails.IsThereRailBlock(world, position))
            {
                return null;
            }

            return world.BlockAccessor.GetBlock(position.X, position.Y, position.Z) as SimpleRailsBlock;
        }

        /// <returns>True if the rail block is flat, else false.</returns>
        public static bool IsFlatRailBlock(Block block)
        {
            // First check that provided block is a simple rails block
            if (!(block is SimpleRailsBlock))
                return false;

            // Then check that the code of the block is actually that of a flat rail block
            switch (block.LastCodePart())
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
        public static String GetFlatRailsDirection(Block block)
        {
            // First check that provided block is a simple rails block
            if (!IsFlatRailBlock(block))
                return null;

            // Then check that the code of the block is actually that of a flat rail block
            switch (block.LastCodePart())
            {
                case "flat_ns":
                    return "ns";
                case "flat_we":
                    return "we";
            }

            return null;

        }

        /// <returns>True if the rail block is curved, else false.</returns>
        public static bool IsCurvedRailBlock(Block block)
        {
            // First check that provided block is a simple rails block
            if (!(block is SimpleRailsBlock))
                return false;

            // Then check that the code of the block is actually that of a curved rail block
            switch (block.LastCodePart())
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
        public static String GetCurvedRailBlockDirection(Block block)
        {
            // First check that provided block is a simple rails block
            if (!(block is SimpleRailsBlock) || !IsCurvedRailBlock(block))
                return null;

            // Then check that the code of the block is actually that of a curved rail block
            switch (block.LastCodePart())
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

        /// <returns>True if the rail block is raised (or raised continuous), else false.</returns>
        public static bool IsRaisedRailBlock(Block block)
        {
            // First check that provided block is a simple rails block
            if (!(block is SimpleRailsBlock))
                return false;

            // Then check that the code of the block is actually that of a raised rail block
            switch (block.LastCodePart())
            {
                case "raised_e":
                case "raised_continuous_e":
                    return true;
                case "raised_n":
                case "raised_continuous_n":
                    return true;
                case "raised_w":
                case "raised_continuous_w":
                    return true;
                case "raised_s":
                case "raised_continuous_s":
                    return true;
            }

            return false;

        }

        /// <returns>True if the rail block is curved or raised, else false.</returns>
        public static bool IsCurvedOrRaisedBlock(Block block)
        {
            return IsCurvedRailBlock(block) || IsRaisedRailBlock(block);
        }

        /// <returns>
        /// A string that indicates the direction ('e', 'n', 'w' or 's') if the block is a raised rails block (or raised continuous). 
        /// <br/>Could return null if there are problems.
        /// </returns>
        public static String GetRaisedRailsDirection(Block block)
        {
            // First check that provided block is a simple rails block
            if (!IsRaisedRailBlock(block))
                return null;

            // Then check that the code of the block is actually that of a raised rail block
            switch (block.LastCodePart())
            {
                case "raised_e":
                case "raised_continuous_e":
                    return "e";
                case "raised_n":
                case "raised_continuous_n":
                    return "n";
                case "raised_w":
                case "raised_continuous_w":
                    return "w";
                case "raised_s":
                case "raised_continuous_s":
                    return "s";
            }

            return null;

        }

        #endregion
    }

}