using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Battleships
{
    class Ship
    {
        private int size;
        private int remaining_parts; 
        private int row;
        private int column;
        private bool isHorizontal;
        private PictureBox myPicBox;
        public Ship(int size, int row, int column, bool isHorizontal, PictureBox myPicBox)
        {
            this.row = row;
            this.column = column;
            this.myPicBox = myPicBox;
            this.isHorizontal = isHorizontal;
            this.size = size;
            this.remaining_parts = size;
        }
        public int GetRemainingParts
        {
            get
            {
                return this.remaining_parts;
            }
        }
        public void reducePart()
        {
            this.remaining_parts--;
        }
    }
}
