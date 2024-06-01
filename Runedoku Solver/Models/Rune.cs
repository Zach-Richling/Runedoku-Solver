using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.CompilerServices;

namespace Runedoku_Solver.Models
{
    public class Rune
    {
        public Vector2 Position { get; set; }
        public RuneType Type { get; set; }
        public bool IsActive { get; set; }

        public Rune(RuneType type, Vector2? position = null)
        {
            Type = type;
            Position = position ?? new Vector2(0, 0);
        }

        public bool InBoundingBox(float mouseX, float mouseY, float runeWidth, float runeHeight, bool isCentered)
        {
            float topLeftBoundedX;
            float topLeftBoundedY;

            float bottomRightBoundedX;
            float bottomRightBoundedY;

            if (!isCentered)
            {
                topLeftBoundedX = Position.X;
                topLeftBoundedY = Position.Y;

                bottomRightBoundedX = Position.X + runeWidth;
                bottomRightBoundedY = Position.Y + runeHeight;
            } 
            else
            {
                topLeftBoundedX = Position.X - runeWidth / 2;
                topLeftBoundedY = Position.Y - runeHeight / 2;

                bottomRightBoundedX = Position.X + runeWidth / 2;
                bottomRightBoundedY = Position.Y + runeHeight / 2;
            }

            return (topLeftBoundedX <= mouseX && mouseX <= bottomRightBoundedX && topLeftBoundedY <= mouseY && mouseY <= bottomRightBoundedY);
        }
    }
}
