using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Runedoku_Solver.Models
{
    public class SideBar
    {
        public List<Rune> Runes { get; set; } = new List<Rune>();

        private float _availableHeight;
        private bool _hasNormalized = false;

        private readonly float _leftPadding;

        public RuneType ActiveRuneType { get; private set; } = RuneType.Air;

        public SideBar(float availableHeight, float leftPadding)
        {
            _availableHeight = availableHeight;
            _leftPadding = leftPadding;
        }

        public void Draw(SpriteBatch batch, Dictionary<RuneType, Texture2D> textures, float runeWidth, float runeHeight)
        {
            foreach (var rune in Runes)
            {
                batch.Draw(textures[rune.Type], rune.Position, rune.IsActive ? Color.Gray : Color.White);
            }
        }
        public void CheckClicked(float mouseX, float mouseY, float runeWidth, float runeHeight)
        {
            foreach (var rune in Runes)
            {
                if (rune.InBoundingBox(mouseX, mouseY, runeWidth, runeHeight, false))
                {
                    //If a rune on the sidebar was clicked, deselect all and select the clicked one.
                    Runes.ForEach(x => x.IsActive = false);
                    ActiveRuneType = rune.Type;

                    rune.IsActive = true;
                }
            }
        }

        public void NormalizeRunePositions(float availableHeight, Dictionary<RuneType, Texture2D> textures)
        {
            if (_hasNormalized && _availableHeight == availableHeight)
            {
                return;
            }

            _availableHeight = availableHeight;

            var heightNeeded = textures.Select(x => x.Value.Height).Sum();

            var padding = (availableHeight - heightNeeded) / (Runes.Count + 1);

            var previousHeight = 0f;

            foreach (var rune in Runes)
            {
                rune.Position = new Vector2(_leftPadding, previousHeight + padding);
                previousHeight = rune.Position.Y + textures[rune.Type].Height;
            }

            _hasNormalized = true;
        }
    }
}
