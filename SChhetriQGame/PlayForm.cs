using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace SChhetriQGame
{
    public partial class PlayForm : Form
    {
        // Declare a variable to count moves
        private int moveCount = 0;
        int numbersOfBoxes;
        Tile[,] tiles;
        private int numRows, numCols;
        int numberofRemaningBoxes;
        private int lastSelectedtiletype;
        private Tile lastSelectedTile;
        private const int INIT_LEFT = 20;
        private const int INIT_TOP = 20;
        private const int TILE_WIDTH = 50;
        private const int TILE_HEIGHT = 50;
        private Tile selectedTile;
        public PlayForm()
        {
            InitializeComponent();
            EnableButton();
            txtNoOfMoves.Text=0.ToString();
            txtNoOfRemaningBox.Text=0.ToString();
        }

        private void loadGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlgOpen = new OpenFileDialog();
            dlgOpen.Filter = "Text Files|*.txt";

            if (dlgOpen.ShowDialog() == DialogResult.OK)
            {
                ResetGame();
                // User selected a file, load the game
                string selectedFilePath = dlgOpen.FileName;
                LoadLevel(selectedFilePath);
            }

        }
        private void EnableButton()
        {
            btnDown.Enabled = false;
            btnUp.Enabled = false;
            btnLeft.Enabled = false;
            btnRight.Enabled = false;
        }
        private void DisableButton()
        {
            btnDown.Enabled = true;
            btnUp.Enabled = true;
            btnLeft.Enabled = true;
            btnRight.Enabled = true;
        }
        private void LoadLevel(string selectedFilePath)
        {
            try
            {
                string[] lines = File.ReadAllLines(selectedFilePath);
                Debug.WriteLine($" length:{lines.Length}");

                // Parse number of rows and columns
                numRows = int.Parse(lines[0]);
                numCols = int.Parse(lines[1]);
                Debug.WriteLine($"row {numRows} col {numCols}");

                // Create a 2D array of PictureBoxes representing the game grid
                tiles = new Tile[numRows, numCols];

                for (int i = 2; i < lines.Length; i += 3)
                {
                    try
                    {

                        int row = int.Parse(lines[i]);
                        int col = int.Parse(lines[i + 1]);
                        int toolType = int.Parse(lines[i + 2]);
                        Debug.WriteLine($" row:{row}{col}{toolType}");

                        // Create a Tile instance
                        Tile tile = new Tile(row, col, toolType)
                        {
                            // Set the position and size of the Tile
                            Width = flowLayoutPanel1.Size.Width / numCols,
                            Height = flowLayoutPanel1.Size.Height / numRows,
                            Left = Width * col,
                            Top = Height * row,
                            Image = GetTileImage(toolType)

                        };



                        // Attach the event handler to the Click event of the tile
                        tile.Click += Tile_Click;
                        // Add the Tile to
                        flowLayoutPanel1.Controls.Add(tile);

                        // Add the Tile to the tiles array (checking bounds)
                        if (row >= 0 && row < numRows && col >= 0 && col < numCols)
                        {
                            tiles[row, col] = tile;

                        }





                    }
                    catch (FormatException ex)
                    {
                        Debug.WriteLine($"Error parsing values: {ex.Message}");
                    }


                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading level: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            BoxCount();
            // Reset move count when loading a new level
           
            UpdateMovesCount();
            DisableButton();

        }

        private void Tile_Click(object sender, EventArgs e)
        {
            // Handle the tile click event
            if (sender is Tile clickedTile)
            {
                // Set the clicked tile as the selected tile
                selectedTile = clickedTile;
                lastSelectedTile = selectedTile;
                Debug.WriteLine($"tile click {selectedTile.Row}{selectedTile.Col}");
            }

        }

        private Image GetTileImage(int toolType)
        {
            if (toolType == 0)
            {
                // Load null image into the clicked PictureBox
                return null;
            }
            else if (toolType == 1)
            {
                return Resource.Wall;
            }
            else if (toolType == 2)
            {
                return Resource.Greenbox;
            }
            else if (toolType == 3)
            {
                return Resource.Redbox;
            }
            else if (toolType == 4)
            {
                return Resource.Greendoor;
            }
            else if (toolType == 5)
                return Resource.Reddoor;
            else
                return null;


        }


        private void btnLeft_Click(object sender, EventArgs e)
        {
            if (selectedTile == null)
            {
                MessageBox.Show("click to select", "Qgame");

            }
            else if (selectedTile.TileType == 2 || selectedTile.TileType == 3)
            {
                MoveTileHorizontically( -1); // Move to the left
                UpdateAllTileImages();
                PrintTileInfo();
            }
            //else
            //{
            //    MessageBox.Show("please select box", "Qgame");
            //}
        }

        private void btnRight_Click(object sender, EventArgs e)
        {
            if (selectedTile == null)
            {
                MessageBox.Show("click to select", "Qgame");

            }
            else if (selectedTile.TileType == 2 || selectedTile.TileType == 3)
            {
                MoveTileHorizontically( +1); // Move to the left
                UpdateAllTileImages();

                PrintTileInfo();
            }
         

        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            if (selectedTile == null)
            {
                MessageBox.Show("click to select", "Qgame");

            }
            else if (selectedTile.TileType == 2 || selectedTile.TileType == 3)
            {
                Debug.WriteLine($"clickedtileposition {selectedTile.Row} {selectedTile.Col}");
                MoveTileVertically(-1); // Move up
                UpdateAllTileImages();

                PrintTileInfo();
            }
       
        }


        private void btnDown_Click(object sender, EventArgs e)
        {
            if (selectedTile == null)
            {
                MessageBox.Show("click to select", "Qgame");

            }
            else if (selectedTile.TileType == 2 || selectedTile.TileType == 3)
            {
                MoveTileVertically(1); // Move down
                UpdateAllTileImages();

                PrintTileInfo();
            }
      

        }

        private void MoveTileHorizontically( int horizontalDirection)
        {
            if (selectedTile != null)
            {
                int currentRow = selectedTile.Row;
                int currentCol = selectedTile.Col;
                int clickedTileType = selectedTile.TileType;

                //currentRow = selectedTile.Row; // Reset currentRow for horizontal movement

                // Move horizontally
                while (currentCol + horizontalDirection >= 0 && currentCol + horizontalDirection < numCols)
                {
                    Tile adjacentTile = GetTile(currentRow, currentCol + horizontalDirection);

                    if (adjacentTile != null)
                    {
                        Debug.WriteLine($"Checking tile at ({currentRow}, {currentCol + horizontalDirection}): TileType = {adjacentTile.TileType}");

                        if (adjacentTile.TileType == 0)
                        {
                            MoveTile(selectedTile, currentRow, currentCol + horizontalDirection, clickedTileType);
                            currentCol += horizontalDirection;
                            Debug.WriteLine($"Moved to ({currentRow}, {currentCol})");
                        }
                        else if ((clickedTileType == 3 && adjacentTile.TileType == 5) || (clickedTileType == 2 && adjacentTile.TileType == 4))
                        {
                            // Red box moving onto red door or green box moving onto green door
                            Debug.WriteLine($"Moved onto door at ({currentRow}, {currentCol + horizontalDirection})");
                            MoveTile(selectedTile, currentRow, currentCol + horizontalDirection, 0);
                            break;
                        }
                        else if (adjacentTile.TileType == 1 || adjacentTile.TileType == 2 || adjacentTile.TileType == 3 || adjacentTile.TileType == 4 || adjacentTile.TileType == 5)
                        {
                            Debug.WriteLine("Encountered wall or wrong door. Stopping.");
                            tiles[adjacentTile.Row, adjacentTile.Col - horizontalDirection].TileType = clickedTileType;
                            selectedTile = tiles[currentRow, currentCol];
                            break;
                        }
                        else
                        {
                            Debug.WriteLine($"Tile to the left/right is null. Stopping.");
                            break;
                        }
                    }
                }
                // Increment the move count
                moveCount++;
                BoxCount();
                UpdateMovesCount();
            }

        }
        private void MoveTileVertically(int verticalDirection)
        {
            if (selectedTile != null)
            {
                int currentRow = selectedTile.Row;
                int currentCol = selectedTile.Col;
                int clickedTileType = selectedTile.TileType;

                // Move vertically
                while (currentRow + verticalDirection >= 0 && currentRow + verticalDirection < numRows)
                {
                    Tile adjacentTile = GetTile(currentRow + verticalDirection, currentCol);

                    if (adjacentTile != null)
                    {
                        Debug.WriteLine($"Checking tile at ({currentRow + verticalDirection}, {currentCol}): TileType = {adjacentTile.TileType}");

                        if (adjacentTile.TileType == 0)
                        {
                            MoveTile(selectedTile, currentRow + verticalDirection, currentCol, clickedTileType);
                            currentRow += verticalDirection;
                            Debug.WriteLine($"Moved to ({currentRow}, {currentCol})");
                        }
                        else if ((clickedTileType == 3 && adjacentTile.TileType == 5) || (clickedTileType == 2 && adjacentTile.TileType == 4))
                        {
                            // Red box moving onto red door or green box moving onto green door
                            Debug.WriteLine($"Moved onto door at ({currentRow + verticalDirection}, {currentCol})");
                            MoveTile(selectedTile, currentRow + verticalDirection, currentCol, 0);
                            break;
                        }
                        else if (adjacentTile.TileType == 1 || adjacentTile.TileType == 2 || adjacentTile.TileType == 3 || adjacentTile.TileType == 4 || adjacentTile.TileType == 5)
                        {
                            Debug.WriteLine("Encountered wall or wrong door. Stopping.");
                            tiles[adjacentTile.Row - verticalDirection, adjacentTile.Col].TileType = clickedTileType;
                            selectedTile = tiles[currentRow, currentCol];
                            break;
                        }
                        else
                        {
                            Debug.WriteLine($"Tile above/below is null. Stopping.");
                            break;
                        }
                    }
                }
                // Increment the move count
                moveCount++;
                BoxCount();
                UpdateMovesCount();
              
            }
        }

        private void MoveTile(Tile tile, int newRow, int newCol, int oldTileType)
        {
            Tile prevTile = GetTile(tile.Row, tile.Col);

            if (prevTile != null)
            {
                tiles[tile.Row, tile.Col].TileType = 0;
            }

            UpdateAllTileImages();

            if (tiles[newRow, newCol] != null && tiles[newRow, newCol].TileType == 1)
            {
                if (newCol + 1 < numCols && tiles[newRow, newCol + 1].TileType != 1)
                {
                    Tile adjacentTile = GetTile(newRow, newCol + 1);

                    if (adjacentTile != null)
                    {
                        adjacentTile.TileType = tile.TileType;
                        Debug.WriteLine($"Setting TileType of tile at ({newRow}, {newCol + 1}) to {tile.TileType}");
                    }
                }

                Debug.WriteLine("Encountered wall. Stopping.");
            }
            else
            {
                tiles[tile.Row, tile.Col].TileType = 0;
                Debug.WriteLine($"Old tile type: {selectedTile.TileType}");
            }

            UpdateAllTileImages();
        }

        private void UpdateAllTileImages()
        {
            foreach (Tile tile in tiles)
            {
                if (tile != null)
                {
                    tile.Image = GetTileImage(tile.TileType);
                }

            }
        }
        private Tile GetTile(int row, int col)
        {
            if (row >= 0 && row < numRows && col >= 0 && col < numCols)
            {
                return tiles[row, col];
            }
            return null;
        }



        private void PrintTileInfo()
        {
            Debug.WriteLine("Tile Information:");
            for (int row = 0; row < numRows; row++)
            {
                for (int col = 0; col < numCols; col++)
                {
                    Tile tile = tiles[row, col];
                    if (tile != null)
                    {
                        Debug.WriteLine($"Row: {tile.Row}, Column: {tile.Col}, TileType: {tile.TileType}");
                    }
                }
            }
        }
        private void BoxCount()
        {
            numberofRemaningBoxes = 0; // Reset count

            for (int row = 0; row < numRows; row++)
            {
                for (int col = 0; col < numCols; col++)
                {
                    Tile tile = tiles[row, col];
                    if (tile != null && (tile.TileType == 2 || tile.TileType == 3))
                    {
                        numberofRemaningBoxes++;
                    }
                }
            }

            txtNoOfRemaningBox.Text = numberofRemaningBoxes.ToString();

            if(numberofRemaningBoxes == 0)
            {
                MessageBox.Show("Congratulation!!\nGame end", "Qgame");
                ResetGame();
                Debug.WriteLine($"{moveCount}");

            }
        }

        // Add this method to update the move count
        private void UpdateMovesCount()
        {
            // Update the move count label or any other UI element as needed
            txtNoOfMoves.Text = moveCount.ToString();
        }
        private void ResetGame()
        {
            // Clear the existing tiles
            flowLayoutPanel1.Controls.Clear();

            // Reset other game-related variables
            moveCount = 0;
            //UpdateMovesCount();
            selectedTile = null;
            numbersOfBoxes = 0;
            numberofRemaningBoxes = 0;
            txtNoOfMoves.Text = "0";
            txtNoOfRemaningBox.Text = "0";

          
            // Enable or disable buttons based on your requirements
            EnableButton();
        }



    }



}

