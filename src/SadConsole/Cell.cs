﻿#if XNA
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endif

namespace SadConsole
{
    using System;
    using System.Linq;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents an individual glyph on the screen with a foreground, background, and mirror effect.
    /// </summary>
    [DataContract]
    public class Cell
    {
        /// <summary>
        /// Modifies the look of a cell with additional character. 
        /// </summary>
        [DataMember]
        public CellDecorator[] Decorators { get; internal set; } = new CellDecorator[0];

        /// <summary>
        /// The foreground color of this cell.
        /// </summary>
        [DataMember]
        public Color Foreground;

        /// <summary>
        /// The background color of this cell.
        /// </summary>
        [DataMember]
        public Color Background;

        /// <summary>
        /// The glyph index from a font for this cell.
        /// </summary>
        [DataMember]
        public int Glyph;

        /// <summary>
        /// The mirror effect for this cell.
        /// </summary>
        [DataMember]
        public SpriteEffects Mirror;

        /// <summary>
        /// When true, indicates this cell should be drawn.
        /// </summary>
        [DataMember]
        public bool IsVisible = true;

        /// <summary>
        /// A temporary state to backup and restore a cell.
        /// </summary>
        [DataMember]
        public CellState? State { get; internal set; }

        /// <summary>
        /// Creates a cell with a white foreground, black background, glyph 0, and no mirror effect.
        /// </summary>
        public Cell() : this(Color.White, Color.Black, 0, SpriteEffects.None) { }

        /// <summary>
        /// Creates a cell with the specified foreground, black background, glyph 0, and no mirror effect.
        /// </summary>
        /// <param name="foreground">Foreground color.</param>
        public Cell(Color foreground) : this(foreground, Color.Black, 0, SpriteEffects.None) { }

        /// <summary>
        /// Creates a cell with the specified foreground, specified background, glyph 0, and no mirror effect.
        /// </summary>
        /// <param name="foreground">Foreground color.</param>
        /// <param name="background">Background color.</param>
        public Cell(Color foreground, Color background) : this(foreground, background, 0, SpriteEffects.None) { }

        /// <summary>
        /// Creates a cell with the specified foreground, background, and glyph, with no mirror effect.
        /// </summary>
        /// <param name="foreground">Foreground color.</param>
        /// <param name="background">Background color.</param>
        /// <param name="glyph">The glyph index.</param>
        public Cell(Color foreground, Color background, int glyph) : this(foreground, background, glyph, SpriteEffects.None) { }

        /// <summary>
        /// Creates a cell with the specified foreground, background, glyph, and mirror effect.
        /// </summary>
        /// <param name="foreground">Foreground color.</param>
        /// <param name="background">Background color.</param>
        /// <param name="glyph">The glyph index.</param>
        /// <param name="mirror">The mirror effect.</param>
        public Cell(Color foreground, Color background, int glyph, SpriteEffects mirror)
        {
            Foreground = foreground;
            Background = background;
            Glyph = glyph;
            Mirror = mirror;
        }

        /// <summary>
        /// Copies the visual appearance to the specified cell. This includes foreground, background, glyph, and mirror effect.
        /// </summary>
        /// <param name="cell">The target cell to copy to.</param>
        public void CopyAppearanceTo(Cell cell)
        {
            cell.Foreground = this.Foreground;
            cell.Background = this.Background;
            cell.Glyph = this.Glyph;
            cell.Mirror = this.Mirror;
            cell.Decorators = Decorators.Length != 0 ? Decorators.ToArray() : new CellDecorator[0];
        }

        /// <summary>
        /// Sets the foreground, background, glyph, and mirror effect to the same as the specified cell.
        /// </summary>
        /// <param name="cell">The target cell to copy from.</param>
        public void CopyAppearanceFrom(Cell cell)
        {
            this.Foreground = cell.Foreground;
            this.Background = cell.Background;
            this.Glyph = cell.Glyph;
            this.Mirror = cell.Mirror;
            this.Decorators = cell.Decorators.ToArray();
        }

        /// <summary>
        /// Resets the foreground, background, glyph, and mirror effect.
        /// </summary>
        public void Clear()
        {
            Foreground = Color.White;
            Background = Color.Black;
            Glyph = 0;
            Mirror = SpriteEffects.None;

            if (Decorators.Length != 0)
                Decorators = new CellDecorator[0];
        }

        /// <summary>
        /// Draws a single cell using the specified SpriteBatch.
        /// </summary>
        /// <param name="batch">Rendering batch.</param>
        /// <param name="position">Pixel position on the screen to render.</param>
        /// <param name="size">Rendering size of the cell.</param>
        /// <param name="font">Font used to draw the cell.</param>
        public void Draw(SpriteBatch batch, Point position, Point size, Font font)
        {
            Draw(batch, new Rectangle(position.X, position.Y, size.X, size.Y), font);
        }

        /// <summary>
        /// Draws a single cell using the specified SpriteBatch.
        /// </summary>
        /// <param name="batch">Rendering batch.</param>
        /// <param name="drawingRectangle">Where on the sreen to draw the cell, in pixels.</param>
        /// <param name="font">Font used to draw the cell.</param>
        public void Draw(SpriteBatch batch, Rectangle drawingRectangle, Font font)
        {
            if (Background != Color.Transparent)
                batch.Draw(font.FontImage, drawingRectangle, font.GlyphRects[font.SolidGlyphIndex], Background, 0f, Vector2.Zero, SpriteEffects.None, 0.3f);

            if (Foreground != Color.Transparent)
                batch.Draw(font.FontImage, drawingRectangle, font.GlyphRects[Glyph], Foreground, 0f, Vector2.Zero, Mirror, 0.4f);

            foreach (var decorator in Decorators)
                if (decorator.Color != Color.Transparent)
                    batch.Draw(font.FontImage, drawingRectangle, font.GlyphRects[decorator.Glyph], decorator.Color, 0f, Vector2.Zero, decorator.Mirror, 0.5f);
        }

        /// <summary>
        /// Saves the current state of this cell to the <see cref="State"/> property.
        /// </summary>
        public void SaveState() => State = new CellState(Foreground, Background, Glyph, Mirror, IsVisible, Decorators);

        /// <summary>
        /// Saves the current state of this cell to the provided state variable.
        /// </summary>
        public void SaveState(out CellState state) => state = new CellState(Foreground, Background, Glyph, Mirror, IsVisible, Decorators);

        /// <summary>
        /// Restores the state of this cell from the <see cref="State"/> property.
        /// </summary>
        public void RestoreState()
        {
            if (State.HasValue)
            {
                Foreground = State.Value.Foreground;
                Background = State.Value.Background;
                Glyph = State.Value.Glyph;
                Mirror = State.Value.Mirror;
                IsVisible = State.Value.IsVisible;
                Decorators = State.Value.Decorators.ToArray();
                State = null;
            }
        }

        /// <summary>
        /// Restores the state of this cell from the <paramref name="state"/> parameter.
        /// </summary>
        public void RestoreState(ref CellState state)
        {
            Foreground = state.Foreground;
            Background = state.Background;
            Glyph = state.Glyph;
            Mirror = state.Mirror;
            IsVisible = state.IsVisible;
            Decorators = state.Decorators.ToArray();
        }

        /// <summary>
        /// Resets the <see cref="State"/> to nothing.
        /// </summary>
        public void ClearState() => State = null;

        /// <summary>
        /// Returns a new cell with the same properties as this one.
        /// </summary>
        /// <returns>The new cell.</returns>
        public Cell Clone() => new Cell(Foreground, Background, Glyph, Mirror) { IsVisible = this.IsVisible, Decorators = Decorators.ToArray() };

        /// <summary>
        /// Compares if the cell is the same as the state.
        /// </summary>
        /// <param name="left">A cell.</param>
        /// <param name="right">A cell state.</param>
        /// <returns>True when they match.</returns>
        public static bool operator ==(Cell left, CellState right)
        {
            return left != null &&
                   left.Background == right.Background &&
                   left.Foreground == right.Foreground &&
                   left.Glyph == right.Glyph &&
                   left.Mirror == right.Mirror &&
                   left.IsVisible == right.IsVisible &&
                   left.Decorators.SequenceEqual(right.Decorators);
        }

        /// <summary>
        /// Compares if the cell is different from the state.
        /// </summary>
        /// <param name="left">A cell.</param>
        /// <param name="right">A cell state.</param>
        /// <returns>True when are different.</returns>
        public static bool operator !=(Cell left, CellState right)
        {
            return left == null ||
                   left.Background != right.Background ||
                   left.Foreground != right.Foreground ||
                   left.Glyph != right.Glyph ||
                   left.Mirror != right.Mirror ||
                   left.IsVisible != right.IsVisible &&
                   !left.Decorators.SequenceEqual(right.Decorators);
        }
    }
}
