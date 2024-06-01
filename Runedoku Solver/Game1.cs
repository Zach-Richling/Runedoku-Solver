using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Runedoku_Solver.Extensions;
using Runedoku_Solver.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Runedoku_Solver
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        //Side bar display runes
        private SideBar _sideBar;

        //Main Sudoku Grid
        private Sudoku _sudoku;

        //Textures for runes
        private Dictionary<RuneType, Texture2D> _runeTextures = new Dictionary<RuneType, Texture2D>();

        private float _runeWidth;
        private float _runeHeight;

        private int _sidebarPadding = 10;

        private RuneType _playerRuneType = RuneType.None;

        private event EventHandler<MouseEventArgs> LeftMouseDown;
        private event EventHandler<MouseEventArgs> RightMouseDown;

        private Keys? _lastKeyPressed;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += OnResize;
        }

        protected override void Initialize()
        {
            //Initialize sidebar runes
            _sideBar = new SideBar(_graphics.PreferredBackBufferHeight, _sidebarPadding);
            _sideBar.Runes.Add(new Rune(RuneType.Air) { IsActive = true });
            _sideBar.Runes.Add(new Rune(RuneType.Mind));
            _sideBar.Runes.Add(new Rune(RuneType.Water));
            _sideBar.Runes.Add(new Rune(RuneType.Earth));
            _sideBar.Runes.Add(new Rune(RuneType.Fire));
            _sideBar.Runes.Add(new Rune(RuneType.Body));
            _sideBar.Runes.Add(new Rune(RuneType.Chaos));
            _sideBar.Runes.Add(new Rune(RuneType.Law));
            _sideBar.Runes.Add(new Rune(RuneType.Death));

            //Initialize sudoku grid
            _sudoku = new Sudoku(9);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            //Load all textures
            _runeTextures.Add(RuneType.Air, Content.Load<Texture2D>("Air_Rune"));
            _runeTextures.Add(RuneType.Mind, Content.Load<Texture2D>("Mind_Rune"));
            _runeTextures.Add(RuneType.Water, Content.Load<Texture2D>("Water_Rune"));
            _runeTextures.Add(RuneType.Earth, Content.Load<Texture2D>("Earth_Rune"));
            _runeTextures.Add(RuneType.Fire, Content.Load<Texture2D>("Fire_Rune"));
            _runeTextures.Add(RuneType.Body, Content.Load<Texture2D>("Body_Rune"));
            _runeTextures.Add(RuneType.Chaos, Content.Load<Texture2D>("Chaos_Rune"));
            _runeTextures.Add(RuneType.Law, Content.Load<Texture2D>("Law_Rune"));
            _runeTextures.Add(RuneType.Death, Content.Load<Texture2D>("Death_Rune"));

            _runeWidth = _runeTextures.Select(x => x.Value.Width).Max();
            _runeHeight = _runeTextures.Select(x => x.Value.Height).Max();

            LeftMouseDown += (a, b) => _sideBar.CheckClicked(b.MouseX, b.MouseY, _runeWidth, _runeHeight);
            LeftMouseDown += (a, b) => _sudoku.CheckClicked(b.MouseX, b.MouseY, _runeWidth, _runeHeight, false, _sideBar.ActiveRuneType);
            RightMouseDown += (a, b) => _sudoku.CheckClicked(b.MouseX, b.MouseY, _runeWidth, _runeHeight, true, _sideBar.ActiveRuneType);
        }

        protected override void Update(GameTime gameTime)
        {
            var keyboardState = Keyboard.GetState();
            var mouseState = Mouse.GetState();

            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                LeftMouseDown(this, new MouseEventArgs(mouseState.X, mouseState.Y));
            }
            else if (mouseState.RightButton == ButtonState.Pressed)
            {
                RightMouseDown(this, new MouseEventArgs(mouseState.X, mouseState.Y));
            }

            if (IsKeyUp(keyboardState, Keys.R))
            {
                _sudoku.InitializeEmptyGrid();
            }
            else if (IsKeyUp(keyboardState, Keys.Q))
            {
                _sudoku.PlaceActive = true;
            }
            else if (IsKeyUp(keyboardState, Keys.W))
            {
                _sudoku.PlaceActive = false;
            }
            else if (IsKeyUp(keyboardState, Keys.S))
            {
                Task.Run(() => _sudoku.Solve());
            }
            else if (IsKeyUp(keyboardState, Keys.Escape))
            {
                Exit();
            } else if (IsKeyUp(keyboardState, Keys.G))
            {
                _sudoku.GenerateGrid();
            }

            _lastKeyPressed = keyboardState.GetPressedKeys().FirstOrDefault();

            base.Update(gameTime);
        }

        private bool IsKeyUp(KeyboardState keyboardState, Keys key)
        {
            return !keyboardState.IsKeyDown(key) && _lastKeyPressed == key;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Gray);

            var sideBarWidth = _sidebarPadding * 2 + (int)_runeWidth;

            _spriteBatch.Begin();

            //Draw sidebar background
            _spriteBatch.DrawRectangle(0, 0, sideBarWidth, _graphics.PreferredBackBufferHeight, Color.DarkGray);

            //Adjust sidebar to current screen height
            _sideBar.NormalizeRunePositions(_graphics.PreferredBackBufferHeight, _runeTextures);

            //Draw all side bar items
            _sideBar.Draw(_spriteBatch, _runeTextures, _runeWidth, _runeHeight);

            //Adjust sudoku grid to current screen dimensions
            _sudoku.NormalizeRuneGrid(
                startingX: sideBarWidth,
                availableWidth: _graphics.PreferredBackBufferWidth - sideBarWidth,
                availableHeight: _graphics.PreferredBackBufferHeight
            );

            //Draw sudoku grid
            _sudoku.Draw(_spriteBatch, _runeTextures, _runeWidth, _runeHeight, sideBarWidth);

            //Draw sidebar separator
            _spriteBatch.DrawRectangle(sideBarWidth, 0, 2, _graphics.PreferredBackBufferHeight, Color.Black);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void OnResize(object sender, EventArgs e)
        {
            _graphics.PreferredBackBufferWidth = _graphics.GraphicsDevice.Viewport.Width;
            _graphics.PreferredBackBufferHeight = _graphics.GraphicsDevice.Viewport.Height;
            _graphics.ApplyChanges();
        }
    }
}
