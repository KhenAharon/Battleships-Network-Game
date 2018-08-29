using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;

namespace Battleships
{
    class Logic
    {
        private int [,] UserGameBoard = new int [10, 10];
        private Ship[] shipArr;
        public Logic()
        {
            for (int i = 0; i < 100; i++)
            {
                UserGameBoard[i / 10, i % 10] = 0;
                shipArr = new Ship[5];
            }
        }
        public void CreateShipInArr(int shipNo,int size,int row,int column,bool isShipHorizontal,PictureBox tempPic)
        {
            this.shipArr[shipNo - 1] = new Ship(size, row, column, isShipHorizontal, tempPic);
        }
        public void AddShipToMat(int shipNo, int size, int row, int column, bool isHorizontal)
        {
            if (isHorizontal)
            {
                for (int i = 0; i < size; i++)
                {
                    UserGameBoard[row, column] = shipNo;
                    column++;
                }
            }
            else 
            {
                for (int i = 0; i < size; i++)
                {
                    UserGameBoard[row, column] = shipNo;
                    row++;
                }
            }
        }
        public bool CheckWetherShipAlreadyLocated(int size, int row, int column, bool isHorizontal)
        {
            if (isHorizontal) 
            {
                for (int i = 0; i < size; i++)
                {
                    if (UserGameBoard[row, column] != 0)
                        return true;
                    column++;
                }
            }
            else 
            {
                for (int i = 0; i < size; i++)
                {
                    if (UserGameBoard[row, column] != 0)
                        return true;
                    row++;
                }
            }
            return false;
        }
        public bool FireSquare(int row, int column)
        {
            int squareValue = this.UserGameBoard[row, column];
            if (squareValue < 6 && squareValue > 0) 
            {
                this.shipArr[squareValue - 1].reducePart();
                return true;
            }
            return false;
        }
        public bool isLastPartOfShipFired(int row, int col)
        {
            int squareValue = this.UserGameBoard[row, col];
            this.UserGameBoard[row, col] = -1; 
            if (this.shipArr[squareValue - 1].GetRemainingParts == 0)
            {
                return true;
            }
            return false;
        }
        public bool AreAllShipsBlew
        {
            get
            {
                if(CheckWhetherAllShipBlew())
                {
                    return true;
                }
                else
                    return false;
            }
        }
        private bool CheckWhetherAllShipBlew()
        {
            for (int i = 0; i < this.shipArr.Length; i++)
            {
                if (this.shipArr[i].GetRemainingParts != 0)
                    return false;
            }
            return true;
        }
        public bool CheckIfShipIsNotNearOtherShip(int size,int row, int col, bool horizontal)
        {
            if(horizontal)
            {
                for (int i = col; i < col+size; i++)
                {
                    if(row!=0)
                        if (UserGameBoard[row - 1, i] != 0)
                            return false;
                    if(row!=9)
                        if (UserGameBoard[row + 1, i] != 0)
                            return false;
                }
                if (col != 0)
                {
                    if (UserGameBoard[row, col - 1] != 0)
                        return false;
                    if(row!=0)
                        if (UserGameBoard[row-1, col - 1] != 0)
                            return false;
                    if(row!=9)
                        if (UserGameBoard[row+1, col - 1] != 0)
                            return false;//
                }
                if (col+size<=9)
                {
                    if(UserGameBoard[row, col + size] != 0)
                        return false;
                    if(row!=0)
                        if (UserGameBoard[row-1, col + size] != 0)
                            return false;
                    if(row!=9)
                        if (UserGameBoard[row+1, col + size] != 0)
                            return false;
                }
                return true;
            }
            else
            {
                for (int i = row ; i < row + size; i++)
                {
                    if (col != 0)
                        if (UserGameBoard[i, col-1] != 0)
                            return false;
                    if (col != 9)
                        if (UserGameBoard[i, col+1] != 0)
                            return false;
                }
                if (row != 0)
                {
                    if (UserGameBoard[row-1, col] != 0)
                        return false;
                    if(col!=0)
                        if (UserGameBoard[row - 1, col-1] != 0)
                            return false;
                    if(col!=9)
                        if (UserGameBoard[row - 1, col+1] != 0)
                            return false;
                }
                if (row + size <= 9)
                {
                    if (UserGameBoard[row+size, col] != 0)
                        return false;
                    if(col!=0)
                        if (UserGameBoard[row + size, col-1] != 0)
                            return false;
                    if(col!=9)
                        if (UserGameBoard[row + size, col+1] != 0)
                            return false;
                }
                return true;
            }
        }
    }
}
