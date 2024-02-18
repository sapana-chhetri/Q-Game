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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SChhetriQGame
{
    public partial class ControlPanelForm : Form
    {
        /// <summary>
        /// Constructor for control panel
        /// </summary>
        public ControlPanelForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// This click event handler show the design form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDesign_Click(object sender, EventArgs e)
        {
            QGameDesignForm f= new QGameDesignForm();
            f.Show();
        }

        /// <summary>
        /// This click event handler close the form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

		private void btnPlay_Click(object sender, EventArgs e)
		{
            PlayForm f= new PlayForm();
            f.Show();
		}
	}
}
