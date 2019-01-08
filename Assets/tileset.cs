using System;
using UnityEngine.Assertions;
namespace easy_tileset
{
    
    public enum tile_pos_type
    {
        TOP_LEFT_TILE,
        TOP_CENTER_TILE,
        TOP_RIGHT_TILE,

        CENTER_LEFT_TILE,
        CENTER_TILE,
        CENTER_RIGHT_TILE,

        BOTTOM_LEFT_TILE,
        BOTTOM_CENTER_TILE,
        BOTTOM_RIGHT_TILE,

        CENTER_TOP_LEFT_TILE,
        CENTER_TOP_RIGHT_TILE,
        CENTER_BOTTOM_LEFT_TILE,
        CENTER_BOTTOM_RIGHT_TILE,

        TILE_POS_COUNT

    }


    public class tile_type_layout  {
        public tile_pos_type type;
        public int[] E;

        public tile_type_layout() {
            E = new int[9];
        }
    }


    public class TileLayouts
    {
        public tile_type_layout[] layouts;
        public int count;
        public TileLayouts() {
            layouts = new tile_type_layout[10];
        }


        public void AddLayout(int[] values, tile_pos_type type)
        {
            tile_type_layout layout = this.layouts[this.count++];

            layout.type = type;

            for (int i = 0; i < 9; ++i)
            {
                layout.E[i] = values[i];
            }

        }

        public TileLayouts easyTile_initLayouts()
        {
            TileLayouts layouts = new TileLayouts();
            int[] a = new int[9]{0, 0, 0, 0, 1, 1, 0, 1, 0};


            AddLayout(a, tile_pos_type.TOP_LEFT_TILE);

            int[] b = new int[9]{0, 0, 0,
            1, 1, 1,
            0, 1, 0};
            AddLayout(b, tile_pos_type.TOP_CENTER_TILE);

            int[] c = new int[9]{0, 0, 0,
            1, 1, 0,
            0, 1, 0};
            AddLayout(c, tile_pos_type.TOP_RIGHT_TILE);

            int[] d = new int[9]{0, 1, 0,
            0, 1, 1,
            0, 1, 0};
            AddLayout(d, tile_pos_type.CENTER_LEFT_TILE);

            int[] e = new int[9]{0, 1, 0,
            1, 1, 0,
            0, 1, 0};
            AddLayout(e, tile_pos_type.CENTER_RIGHT_TILE);

            int[] f = new int[9]{0, 0, 0,
            0, 1, 0,
            0, 0, 0};
            AddLayout(f, tile_pos_type.CENTER_TILE);

            int[] g = new int[9]{0, 1, 0,
            1, 1, 1,
            0, 1, 0};
            AddLayout(g, tile_pos_type.CENTER_TILE);

            int[] h = new int[9]{0, 1, 0,
            1, 1, 0,
            0, 0, 0};
            AddLayout(h, tile_pos_type.BOTTOM_RIGHT_TILE);

            int[] i = new int[9]{0, 1, 0,
            0, 1, 1,
            0, 0, 0};
            AddLayout(i, tile_pos_type.BOTTOM_LEFT_TILE);

            int[] j = new int[9]{0, 1, 0,
            1, 1, 1,
            0, 0, 0};
            AddLayout(j, tile_pos_type.BOTTOM_CENTER_TILE);

            return layouts;

        }

        private bool IsEqual_PosTile(int X, int Y, int[] spots, tile_type_layout layout) {
            bool result = (layout.E[Y * 3 + X] == spots[Y * 3 + X]);
            return result;
        }


        public tile_pos_type GetTileType(int[] spots)
        {
            tile_pos_type result = tile_pos_type.TOP_LEFT_TILE;
            Assert.IsTrue(count == 10);

            for (int i = 0; i < this.count; ++i)
            {
                tile_type_layout layout = this.layouts[i];

                if (IsEqual_PosTile(0, 1, spots, layout) &&
                   IsEqual_PosTile(2, 1, spots, layout) &&
                   IsEqual_PosTile(1, 0, spots, layout) &&
                   IsEqual_PosTile(1, 2, spots, layout))
                {
                    result = layout.type;
                    if (result == tile_pos_type.CENTER_TILE)
                    {
                        if (spots[0] == 0)
                        {
                            result = tile_pos_type.CENTER_TOP_LEFT_TILE;
                        }
                        else if (spots[2] == 0)
                        {
                            result = tile_pos_type.CENTER_TOP_RIGHT_TILE;
                        }
                        else if (spots[6] == 0)
                        {
                            result = tile_pos_type.CENTER_BOTTOM_LEFT_TILE;
                        }
                        else if (spots[8] == 0)
                        {
                            result = tile_pos_type.CENTER_BOTTOM_RIGHT_TILE;
                        }
                    }
                    break;
                }

            }

            //Assert(Result != NULL_TILE);
            return result;
        }
    }
}
