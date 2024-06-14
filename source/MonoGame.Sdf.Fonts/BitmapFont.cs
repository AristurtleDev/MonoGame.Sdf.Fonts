// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Sdf.Fonts;

/// <summary>
/// Represents a bitmap font used for rendering text in a 2D game or application.
/// This class cannot be inherited.
/// </summary>
public sealed class BitmapFont
{
    private readonly Dictionary<int, BitmapFontCharacter> _characters;

    /// <summary>
    /// Gets the name of the typeface.
    /// </summary>
    public string Face { get; }

    /// <summary>
    /// Gets the size of the font.
    /// </summary>
    public int Size { get; }

    /// <summary>
    /// Gets the height of a line of text.
    /// </summary>
    public int LineHeight { get; }

    /// <summary>
    /// Gets or sets the spacing between letters.
    /// </summary>
    public int LetterSpacing { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to use kerning.
    /// </summary>
    public bool UseKernings { get; set; } = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="BitmapFont"/> class.
    /// </summary>
    /// <param name="face">The name of the typeface.</param>
    /// <param name="size">The size of the font.</param>
    /// <param name="lineHeight">The height of a line of text.</param>
    /// <param name="characters">The collection of font characters.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="characters"/> is <see langword="null"/>.
    /// </exception>
    public BitmapFont(string face, int size, int lineHeight, IEnumerable<BitmapFontCharacter> characters)
    {
        ArgumentNullException.ThrowIfNull(characters);

        Face = face;
        Size = size;
        LineHeight = lineHeight;
        _characters = new Dictionary<int, BitmapFontCharacter>();

        foreach (BitmapFontCharacter character in characters)
        {
            _characters.Add(character.Character, character);
        }
    }

    /// <summary>
    /// Gets the character information for the specified character.
    /// </summary>
    /// <param name="character">The character to retrieve.</param>
    /// <returns>The <see cref="BitmapFontCharacter"/> associated with the specified character.</returns>
    /// <exception cref="KeyNotFoundException">
    /// Thrown if this bitmap font does not contain a character with the specified <paramref name="character"/> value.
    /// </exception>
    public BitmapFontCharacter GetCharacter(int character) => _characters[character];

    /// <summary>
    /// Tries to get the character information for the specified character.
    /// </summary>
    /// <param name="character">The character to retrieve.</param>
    /// <param name="value">When this method returns, contains the character information if the character was found; otherwise, null.</param>
    /// <returns><see langword="true"/> if the character was found; otherwise, <see langword="false"/>.</returns>
    public bool TryGetCharacter(int character, out BitmapFontCharacter value) => _characters.TryGetValue(character, out value);

    /// <summary>
    /// Measures the size of the specified text string.
    /// </summary>
    /// <param name="text">The text string to measure.</param>
    /// <returns>The size of the text string in pixels.</returns>
    public Point MeasureString(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return Point.Zero;
        }

        Rectangle stringRectangle = GetStringRectangle(text);
        return new Point(stringRectangle.Width, stringRectangle.Height);
    }

    /// <summary>
    /// Measures the size of the specified text string.
    /// </summary>
    /// <param name="text">The text string to measure.</param>
    /// <returns>The size of the text string in pixels.</returns>
    public Point MeasureString(StringBuilder text)
    {
        if (text == null || text.Length == 0)
        {
            return Point.Zero;
        }

        Rectangle stringRectangle = GetStringRectangle(text);
        return new Point(stringRectangle.Width, stringRectangle.Height);
    }

    /// <summary>
    /// Gets the bounding rectangle of the specified text string at the default position.
    /// </summary>
    /// <param name="text">The text string to measure.</param>
    /// <returns>The bounding rectangle of the text string.</returns>
    public Rectangle GetStringRectangle(string text)
    {
        return GetStringRectangle(text, Vector2.Zero);
    }

    /// <summary>
    /// Gets the bounding rectangle of the specified text string at the specified position.
    /// </summary>
    /// <param name="text">The text string to measure.</param>
    /// <param name="position">The position of the text string.</param>
    /// <returns>The bounding rectangle of the text string.</returns>
    public Rectangle GetStringRectangle(string text, Vector2 position)
    {
        var glyphs = GetGlyphs(text, position);
        Rectangle rectangle = new Rectangle((int)position.X, (int)position.Y, 0, LineHeight);

        foreach (var glyph in glyphs)
        {
            if (glyph.Character != null)
            {
                var right = glyph.Position.X + glyph.Character.SourceRectangle.Width;

                if (right > rectangle.Right)
                    rectangle.Width = (int)(right - rectangle.Left);
            }

            if (glyph.CharacterID == '\n')
                rectangle.Height += LineHeight;
        }

        return rectangle;
    }

    /// <summary>
    /// Gets the bounding rectangle of the specified text string at the specified position.
    /// </summary>
    /// <param name="text">The text string to measure.</param>
    /// <param name="position">The position of the text string.</param>
    /// <returns>The bounding rectangle of the text string.</returns>
    public Rectangle GetStringRectangle(StringBuilder text, Vector2? position = null)
    {
        Vector2 position1 = position ?? new Vector2();
        var glyphs = GetGlyphs(text, position1);
        Rectangle rectangle = new Rectangle((int)position1.X, (int)position1.Y, 0, LineHeight);

        foreach (var glyph in glyphs)
        {
            if (glyph.Character != null)
            {
                var right = glyph.Position.X + glyph.Character.SourceRectangle.Width;

                if (right > rectangle.Right)
                    rectangle.Width = (int)(right - rectangle.Left);
            }

            if (glyph.CharacterID == '\n')
                rectangle.Height += LineHeight;
        }

        return rectangle;
    }

    internal struct BitmapFontGlyph
    {
        public int CharacterID;
        public Vector2 Position;
        public BitmapFontCharacter Character;
    }

    internal StringGlyphEnumerable GetGlyphs(string text, Vector2? position = null)
    {
        return new StringGlyphEnumerable(this, text, position);
    }

    internal StringBuilderGlyphEnumerable GetGlyphs(StringBuilder text, Vector2? position)
    {
        return new StringBuilderGlyphEnumerable(this, text, position);
    }

    internal struct StringGlyphEnumerable : IEnumerable<BitmapFontGlyph>
    {
        private readonly StringGlyphEnumerator _enumerator;

        public StringGlyphEnumerable(BitmapFont font, string text, Vector2? position)
        {
            _enumerator = new StringGlyphEnumerator(font, text, position);
        }

        public StringGlyphEnumerator GetEnumerator()
        {
            return _enumerator;
        }

        IEnumerator<BitmapFontGlyph> IEnumerable<BitmapFontGlyph>.GetEnumerator()
        {
            return _enumerator;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _enumerator;
        }
    }

    internal struct StringGlyphEnumerator : IEnumerator<BitmapFontGlyph>
    {
        private readonly BitmapFont _font;
        private readonly string _text;
        private int _index;
        private readonly Vector2 _position;
        private Vector2 _positionDelta;
        private BitmapFontGlyph _currentGlyph;
        private BitmapFontGlyph? _previousGlyph;

        object IEnumerator.Current
        {
            get
            {
                // casting a struct to object will box it, behaviour we want to avoid...
                throw new InvalidOperationException();
            }
        }

        public BitmapFontGlyph Current => _currentGlyph;

        public StringGlyphEnumerator(BitmapFont font, string text, Vector2? position)
        {
            _font = font;
            _text = text;
            _index = -1;
            _position = position ?? new Vector2();
            _positionDelta = new Vector2();
            _currentGlyph = new BitmapFontGlyph();
            _previousGlyph = null;
        }

        public bool MoveNext()
        {
            if (++_index >= _text.Length)
                return false;

            var character = GetUnicodeCodePoint(_text, ref _index);
            _currentGlyph.CharacterID = character;
            _font.TryGetCharacter(character, out _currentGlyph.Character);
            _currentGlyph.Position = _position + _positionDelta;

            if (_currentGlyph.Character != null)
            {
                _currentGlyph.Position.X += _currentGlyph.Character.XOffset;
                _currentGlyph.Position.Y += _currentGlyph.Character.YOffset;
                _positionDelta.X += _currentGlyph.Character.XAdvance + _font.LetterSpacing;
            }

            if (_font.UseKernings && _previousGlyph?.Character != null)
            {
                if (_previousGlyph.Value.Character.Kernings.TryGetValue(character, out var amount))
                {
                    _positionDelta.X += amount;
                    _currentGlyph.Position.X += amount;
                }
            }

            _previousGlyph = _currentGlyph;

            if (character != '\n')
                return true;

            _positionDelta.Y += _font.LineHeight;
            _positionDelta.X = 0;
            _previousGlyph = null;

            return true;
        }

        private static int GetUnicodeCodePoint(string text, ref int index)
        {
            return char.IsHighSurrogate(text[index]) && ++index < text.Length
                ? char.ConvertToUtf32(text[index - 1], text[index])
                : text[index];
        }

        public void Dispose()
        {
        }

        public void Reset()
        {
            _positionDelta = new Vector2();
            _index = -1;
            _previousGlyph = null;
        }
    }

    internal struct StringBuilderGlyphEnumerable : IEnumerable<BitmapFontGlyph>
    {
        private readonly StringBuilderGlyphEnumerator _enumerator;

        public StringBuilderGlyphEnumerable(BitmapFont font, StringBuilder text, Vector2? position)
        {
            _enumerator = new StringBuilderGlyphEnumerator(font, text, position);
        }

        public StringBuilderGlyphEnumerator GetEnumerator()
        {
            return _enumerator;
        }

        IEnumerator<BitmapFontGlyph> IEnumerable<BitmapFontGlyph>.GetEnumerator()
        {
            return _enumerator;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _enumerator;
        }
    }

    internal struct StringBuilderGlyphEnumerator : IEnumerator<BitmapFontGlyph>
    {
        private readonly BitmapFont _font;
        private readonly StringBuilder _text;
        private int _index;
        private readonly Vector2 _position;
        private Vector2 _positionDelta;
        private BitmapFontGlyph _currentGlyph;
        private BitmapFontGlyph? _previousGlyph;

        object IEnumerator.Current
        {
            get
            {
                // casting a struct to object will box it, behaviour we want to avoid...
                throw new InvalidOperationException();
            }
        }

        public BitmapFontGlyph Current => _currentGlyph;

        public StringBuilderGlyphEnumerator(BitmapFont font, StringBuilder text, Vector2? position)
        {
            _font = font;
            _text = text;
            _index = -1;
            _position = position ?? new Vector2();
            _positionDelta = new Vector2();
            _currentGlyph = new BitmapFontGlyph();
            _previousGlyph = null;
        }

        public bool MoveNext()
        {
            if (++_index >= _text.Length)
                return false;

            var character = GetUnicodeCodePoint(_text, ref _index);
            _currentGlyph = new BitmapFontGlyph
            {
                CharacterID = character,
                Character = _font.GetCharacter(character),
                Position = _position + _positionDelta
            };

            if (_currentGlyph.Character != null)
            {
                _currentGlyph.Position.X += _currentGlyph.Character.XOffset;
                _currentGlyph.Position.Y += _currentGlyph.Character.YOffset;
                _positionDelta.X += _currentGlyph.Character.XAdvance + _font.LetterSpacing;
            }

            if (_font.UseKernings && _previousGlyph.HasValue && _previousGlyph.Value.Character != null)
            {
                int amount;
                if (_previousGlyph.Value.Character.Kernings.TryGetValue(character, out amount))
                {
                    _positionDelta.X += amount;
                    _currentGlyph.Position.X += amount;
                }
            }

            _previousGlyph = _currentGlyph;

            if (character != '\n')
                return true;

            _positionDelta.Y += _font.LineHeight;
            _positionDelta.X = _position.X;
            _previousGlyph = null;

            return true;
        }

        private static int GetUnicodeCodePoint(StringBuilder text, ref int index)
        {
            return char.IsHighSurrogate(text[index]) && ++index < text.Length
                ? char.ConvertToUtf32(text[index - 1], text[index])
                : text[index];
        }

        public void Dispose()
        {
        }

        public void Reset()
        {
            _positionDelta = new Vector2();
            _index = -1;
            _previousGlyph = null;
        }
    }

    /// <inheritdoc/>
    public override string ToString() => $"{Face}";

    /// <summary>
    /// Loads a <see cref="BitmapFont"/> from the specified file.
    /// </summary>
    /// <param name="graphicsDevice">The graphics device used for rendering.</param>
    /// <param name="path">The path to the font file.</param>
    /// <returns>The loaded <see cref="BitmapFont"/>.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the <paramref name="graphicsDevice"/> is  <see langword="null"/>.
    /// </exception>
    /// <exception cref="FileNotFoundException">
    /// Thrown if no file exists at the <paramref name="path"/> specified.
    /// </exception>
    public static BitmapFont FromFile(GraphicsDevice graphicsDevice, string path)
    {
        using FileStream stream = File.OpenRead(path);
        return FromStream(graphicsDevice, stream);
    }

    /// <summary>
    /// Loads a <see cref="BitmapFont"/> from the specified stream.
    /// </summary>
    /// <param name="graphicsDevice">The graphics device used for rendering.</param>
    /// <param name="stream">The stream containing the font data.</param>
    /// <returns>The loaded <see cref="BitmapFont"/>.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if either the <paramref name="graphicsDevice"/> or <paramref name="stream"/> parameters are
    /// <see langword="null"/>.
    /// </exception>
    public static BitmapFont FromStream(GraphicsDevice graphicsDevice, FileStream stream)
    {
        using BmfTextLoader loader = new BmfTextLoader(graphicsDevice, stream);
        return loader.Load();
    }
}
