using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SChhetriQGame
{
    public class Tile: PictureBox
    {
        public int Row { get; set; }
        public int Col { get; set; }
        public int TileType { get; set; }
        public Tile(int row, int col, int tileType )
        {
            this.Row = row;
            this.Col = col;
            this.TileType = tileType;

            // Set the SizeMode property to StretchImage
            this.SizeMode = PictureBoxSizeMode.StretchImage;
            this.Padding = Padding.Empty;
            this.Margin= Padding.Empty;
        }
    }
}
