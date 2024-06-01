using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Runedoku_Solver.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Runedoku_Solver.Models
{
    public class Sudoku
    {
        private bool _hasNormalized;

        private float _availableWidth;
        private float _availableHeight;
        private float _heightPadding;
        private float _widthPadding;

        private int _size;
        public Rune[,] Grid { get; }

        public bool PlaceActive { get; set; } = false;

        public Sudoku(int size)
        {
            _size = size;
            Grid = new Rune[size, size];
            InitializeEmptyGrid();
        }

        public void InitializeEmptyGrid()
        {
            var rand = new Random();
            for (int r = 0; r < _size; r++)
            {
                for (int c = 0; c < _size; c++)
                {
                    Grid[r, c] = new Rune(RuneType.None);
                }
            }

            _hasNormalized = false;
        }

        public void GenerateGrid()
        {
            InitializeEmptyGrid();
            int numRevealed = 24;

            var rand = new Random();

            var curRevealed = 0;
            while(curRevealed < numRevealed)
            {
                var r = rand.Next(0, _size - 1);
                var c = rand.Next(0, _size - 1);

                for (var i = 1; i <= _size; i++)
                {
                    var runeType = (RuneType)i;
                    if (IsValid(r, c, runeType))
                    {
                        Grid[r, c].Type = runeType;
                        Grid[r, c].IsActive = true;
                        curRevealed++;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mouseX"></param>
        /// <param name="mouseY"></param>
        /// <param name="runeWidth"></param>
        /// <param name="runeHeight"></param>
        /// <param name="mouseButton">False is left mouse, True is right mouse.</param>
        /// <param name="activeType"></param>
        public void CheckClicked(float mouseX, float mouseY, float runeWidth, float runeHeight, bool mouseButton, RuneType activeType)
        {
            for (int r = 0; r < _size; r++)
            {
                for (int c = 0; c < _size; c++)
                {
                    var rune = Grid[r, c];
                    if (rune.InBoundingBox(mouseX, mouseY, runeWidth, runeHeight, true))
                    {
                        if (mouseButton && rune.IsActive)
                        {
                            break;
                        }

                        if (mouseButton)
                        {
                            rune.Type = RuneType.None;
                            break;
                        }
                        else if (IsValid(r, c, activeType))
                        {
                            rune.Type = activeType;
                            rune.IsActive = PlaceActive;
                            break;
                        }
                    }
                }
            }
        }

        private bool IsInColumn(int col, RuneType type)
        {
            for (int r = 0; r < _size; r++)
            {
                if (Grid[r, col].Type == type)
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsInRow(int row, RuneType type)
        {
            for (int c = 0; c < _size; c++)
            {
                if (Grid[row, c].Type == type)
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsInBox(int row, int col, RuneType type)
        {
            var rStart = row - row % 3;
            var cStart = col - col % 3;

            for (int r = rStart; r < rStart + 3; r++)
            {
                for (int c = cStart; c < cStart + 3; c++)
                {
                    if (Grid[r, c].Type == type)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool IsValid(int row, int col, RuneType type)
        {
            return !IsInColumn(col, type) && !IsInRow(row, type) && !IsInBox(row, col, type);
        }

        public bool Solve() => SolveInner(0, 0);
        private bool SolveInner(int row, int col)
        {
            if (row == _size - 1 && col == _size)
            {
                return true;
            }

            if (col == _size)
            {
                row++;
                col = 0;
            }

            if (Grid[row, col].Type != RuneType.None)
            {
                return SolveInner(row, col + 1);
            }

            for (int typeIndex = 1; typeIndex <= _size; typeIndex++)
            {
                var runeType = (RuneType)typeIndex;
                if (IsValid(row, col, runeType))
                {
                    Grid[row, col].Type = runeType;

                    if (SolveInner(row, col + 1))
                    {
                        return true;
                    }
                }

                Grid[row, col].Type = RuneType.None;
            }

            return false;
        }

        public void NormalizeRuneGrid(float startingX, float availableWidth, float availableHeight)
        {
            if (_hasNormalized && availableWidth == _availableWidth && availableHeight == _availableHeight)
            {
                return;
            }

            _availableWidth = availableWidth;
            _availableHeight = availableHeight;

            for (int r = 0; r < _size; r++)
            {
                for (int c = 0; c < _size; c++)
                {
                    var rune = Grid[r, c];
                    rune.Position = new Vector2(
                        (startingX + (_availableWidth / _size) * r) + (_availableWidth / _size) / 2,
                        ((_availableHeight / _size) * c) + (_availableHeight / _size) / 2
                    );
                }
            }

            _hasNormalized = true;
        }

        public void Draw(SpriteBatch batch, Dictionary<RuneType, Texture2D> textures, float runeWidth, float runeHeight, float startingX)
        {
            //Draw backgrounds for groups of 3 by 3 cells
            for (int r = 0; r < _size; r++)
            {
                for (int c = 0; c < _size; c++)
                {
                    if (((r >= 3 && r <= 5) || (c >= 3 && c <= 5))
                        && !(r >= 3 && r <= 5 && c >= 3 && c <= 5))
                    {
                        batch.DrawRectangle((int)(startingX + (_availableWidth / _size) * r) + 1, (int)((_availableHeight / _size) * c) + 1, (int)_availableWidth / _size, (int)_availableHeight / _size, Color.LightGray);
                    }
                }
            }

            for (int r = 0; r < _size; r++)
            {
                //Draw vertical grid lines
                if (r != 0) 
                {
                    batch.DrawRectangle((int)(startingX + (_availableWidth / _size) * r), 0, 1, (int)_availableHeight, Color.Black);
                }

                for (int c = 0; c < _size; c++)
                {
                    var rune = Grid[r, c];
                    //Draw all rune textures centered on their position
                    if (rune.Type != RuneType.None) 
                    {
                        batch.Draw(
                            textures[rune.Type],
                            rune.Position,
                            null,
                            rune.IsActive ? Color.Gray : Color.White,
                            0f,
                            new Vector2(runeWidth / 2, runeHeight / 2),
                            Vector2.One,
                            SpriteEffects.None,
                            0f
                        );
                    }
                }

                //Draw horizontal grid lines
                if (r != 0) 
                {
                    batch.DrawRectangle((int)startingX, (int)((_availableHeight / _size) * r), (int)_availableWidth, 1, Color.Black);
                }
            }
        }
    }
}
