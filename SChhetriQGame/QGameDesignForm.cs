/*
 * SChhetriQGame
 * Assignment 2
 * Revision History
 *     Sapana Chhetri, 2023-10-27: Created
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using QGameToolbox;

namespace SChhetriQGame
{
    public partial class QGameDesignForm : Form
    {
        private int row;
        private int col;
        private bool levelCreated = false;
        private int selectedTool = -1;
        private Dictionary<PictureBox, int> pictureBoxTools = new Dictionary<PictureBox, int>(); // To store picturebox control and selected tool
        private int wallCount = 0;
        private int boxCount = 0;
        private int doorCount = 0;

        /// <summary>
        /// Constructor for the QGameDesignForm class,
        ///  subscribes to the qGameToolbox's ToolSelected event
        /// </summary>
        public QGameDesignForm()
        {
            InitializeComponent();
            qGameToolbox.ToolSelected += QGameToolbox_ToolSelected;
        }

        /// <summary>
        ///  This event handler triggered upon the selection of a tool in the QGameToolbox and
        ///  updates the selectedTool value based on the received tool integer value.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="selectedTool">The integer value representing the selected tool</param>
        private void QGameToolbox_ToolSelected(object sender, int selectedTool)
        {
            this.selectedTool = selectedTool;
        }

        /// <summary>
        /// Handles the click event of the 'Generate' button.
        /// Checks if a level is created; if yes, prompts the user to create a new level,
        /// Validates input values for row and column, creating a grid of PictureBox in flowlayoutr panel
        /// Displays error messages for incorrect inputs.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGenerate_Click(object sender, EventArgs e)
        {
            if (levelCreated)
            {
                DialogResult result = MessageBox.Show("Do you want to create a new level?\n If you do, the current level will be lost.", "Qgame", MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                {
                    ResetToolCounts(); // Reset tool counts
                    flowLayoutPanel.Controls.Clear(); // Clear previous PictureBoxes (if any)
                    levelCreated = false; // Reset the level creation flag
                }
                else
                {
                    return; // Return without creating a new level
                }
            }
            try
            {
                if (int.TryParse(txtRow.Text, out row))
                {
                    if (row <= 0)
                    {
                        throw new ArgumentException("No of row must be greater than 0");
                    }

                    if (int.TryParse(txtColumn.Text, out  col))
                    {
                        if (col <= 0)
                        {
                            throw new ArgumentException("No of column must be greater than 0");
                        }

                        GeneratePictureBoxGrid(row, col);
                    }
                    else
                    {
                        throw new FormatException("No of column should be an integer");
                    }
                }
                else
                {
                    throw new FormatException("No of row should be an integer");
                }
            }
            catch (FormatException ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in row and column: " + ex.Message);
            }
        }

        /// <summary>
        /// This method generate grid of picturebox based on no of row and column, 
        ///  and adding each picturebox inside the flowlayout panel.
        /// </summary>
        /// <param name="row">The no of rows</param>
        /// <param name="col">The no oc column</param>
        public void GeneratePictureBoxGrid(int row, int col)
        {
           
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    PictureBox pictureBox = new PictureBox();

                    //Calculate the size of PictureBox
                    int picBoxWidth = flowLayoutPanel.Width / col;
                    int picBoxHeight = flowLayoutPanel.Height / row;

                    pictureBox.Size = new System.Drawing.Size(picBoxWidth, picBoxHeight);
                    pictureBox.BorderStyle = BorderStyle.Fixed3D;
                    pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                   
                    // Set margin and padding to eliminate spacing
                    pictureBox.Margin = new Padding(0);
                    pictureBox.Padding = new Padding(0);

                    // Assiginingn a unique name to each PictureBox
                    pictureBox.Name = $"PictureBox{i}{j}";

                    // Event handler for clicking a PictureBox
                    pictureBox.Click += PictureBox_Click;

                    flowLayoutPanel.Controls.Add(pictureBox);

                }

            }
            levelCreated= true;// to indicate grid of picturebox is generated.
        }

        /// <summary>
        /// This event handles the click event for the PictureBox elements in the grid,
        /// this manages the selected tool count for each picturebox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PictureBox_Click(object sender, EventArgs e)
        {
            PictureBox clickedPictureBox = (PictureBox)sender;

            if (!pictureBoxTools.ContainsKey(clickedPictureBox))
            {
                pictureBoxTools.Add(clickedPictureBox, selectedTool);
            }
            else
            {
                int initialTool = pictureBoxTools[clickedPictureBox];
                UpdateToolCount(initialTool);
                pictureBoxTools[clickedPictureBox] = selectedTool;
               
            }
            Debug.WriteLine($"name of click:{ clickedPictureBox}");
            
            Debug.WriteLine($"they are :{pictureBoxTools}");
            Debug.WriteLine($"name:{clickedPictureBox.Name}");
            int latestTool = selectedTool;

            if (latestTool == 0)
            {
                // Load null image into the clicked PictureBox
                clickedPictureBox.Image = null;
            }
            else
            {
                switch (latestTool)
                {
                    case 1:
                        // Load wall image into the clicked PictureBox
                        clickedPictureBox.Image = Resource.Wall;
                        wallCount++;
                        break;
                    case 2:
                        // Load greenbox image into the clicked PictureBox
                        clickedPictureBox.Image = Resource.Greenbox;
                        boxCount++;
                        break;
                    case 3:
                        // Load redbox image into the clicked PictureBox
                        clickedPictureBox.Image = Resource.Redbox;
                        boxCount++;
                        break;
                    case 4:
                        // Load greendoor image into the clicked PictureBox
                        clickedPictureBox.Image = Resource.Greendoor;
                        doorCount++;
                        break;
                    case 5:
                        // Load reddoor image into the clicked PictureBox
                        clickedPictureBox.Image = Resource.Reddoor;
                        doorCount++;
                        break;
                    default:
                        // If no tool is selected, display an error message
                        MessageBox.Show("Please select a tool from the toolbox.");
                        break;
                }

            }
        }

        /// <summary>
        /// Updates the count of the selected tool after a tool change.
        /// </summary>
        /// <param name="tool">The tool whose count needs to be updated.</param>
        private void UpdateToolCount(int tool)
        {
            switch (tool)
            {
                case 1:
                    wallCount--;
                    break;
                case 2:
                    boxCount--;
                    break;
                case 3:
                    boxCount--;
                    break;
                case 4:
                    doorCount--;
                    break;
                case 5:
                    doorCount--;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Thhis method reset the count for wall,door and box. 
        /// </summary>
        private void ResetToolCounts()
        {
             wallCount = 0;
             doorCount= 0;
             boxCount= 0;
        }

        /// <summary>
        /// This click event handles save created grid to file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult r = dlgSave.ShowDialog();

            if (r == DialogResult.OK)
            {
                try
                {
                    string fName = dlgSave.FileName + ".txt";
                    Save(fName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error in file save: " + ex.Message);
                }
            }

        }

        /// <summary>
        /// This method saves the no of row , no of column and picturebox containg selected tool with in that file.
        /// </summary>
        /// <param name="fName">The name of the file</param>
        private void Save(string fName)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(fName))
                {
                    writer.WriteLine($"{row}\n{col}");
                    for (int i = 0; i < row; i++)
                    {
                        for (int j = 0; j < col; j++)
                        {
                            string pictureBoxName = $"PictureBox{i}{j}";
                            PictureBox currentPictureBox = FindPictureBoxByName(pictureBoxName);

                            if (currentPictureBox != null && pictureBoxTools.ContainsKey(currentPictureBox))
                            {
                                int selectedTool = pictureBoxTools[currentPictureBox];
                                writer.WriteLine($"{i}\n{j}\n{selectedTool}");
                            }
                            else
                            {
                                writer.WriteLine($"{i}\n{j}\n0"); // No tool selected
                            }
                        }
                    }

                    MessageBox.Show($"File saved successfully!\nNo of wall:{wallCount}\nNo of door:{doorCount}\nNo of box:{boxCount}", "QGame");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in file save: " + ex.Message);
            }
        }

        /// <summary>
        /// This method find PictureBox control by its name within the FlowLayoutPanel.
        /// </summary>
        /// <param name="name">The name of the PictureBox to find.</param>
        /// <returns>The PictureBox control with the specified name, if found; otherwise, returns null.</returns>
        private PictureBox FindPictureBoxByName(string name)
        {
            foreach (Control control in flowLayoutPanel.Controls)
            {
                if (control is PictureBox pictureBox && pictureBox.Name == name)
                {
                    return pictureBox;
                }
            }
            return null;
        }

        /// <summary>
        /// This event handler close qgamedesign form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
