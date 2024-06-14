// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Sdf.Fonts;

/// <summary>
/// Represents a character in a bitmap font. This class cannot be inherited.
/// </summary>
public sealed class BitmapFontCharacter
{
    /// <summary>
    /// Gets the character code.
    /// </summary>
    public int Character { get; }

    /// <summary>
    /// Gets the source texture used by this character.
    /// </summary>
    public Texture2D Texture { get; }

    /// <summary>
    /// Gets the source rectangle to use when rendering this character.
    /// </summary>
    public Rectangle SourceRectangle { get; }

    /// <summary>
    /// Gets the horizontal offset for rendering the character.
    /// </summary>
    public int XOffset { get; }

    /// <summary>
    /// Gets the vertical offset for rendering the character.
    /// </summary>
    public int YOffset { get; }

    /// <summary>
    /// Gets the horizontal advance value for rendering the next character.
    /// </summary>
    public int XAdvance { get; }

    /// <summary>
    /// Gets the dictionary of kerning values for pairs of characters.
    /// </summary>
    public Dictionary<int, int> Kernings { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BitmapFontCharacter"/> class.
    /// </summary>
    /// <param name="character">The character code.</param>
    /// <param name="texture">The source texture for this character.</param>
    /// <param name="sourceRectangle">
    /// The boundary within <paramref name="texture"/> that contains the glyph for this character.
    /// </param>
    /// <param name="xOffset">The horizontal offset for rendering the character.</param>
    /// <param name="yOffset">The vertical offset for rendering the character.</param>
    /// <param name="xAdvance">The horizontal advance value for rendering the next character.</param>
    public BitmapFontCharacter(int character, Texture2D texture, Rectangle sourceRectangle, int xOffset, int yOffset, int xAdvance)
    {
        Character = character;
        Texture = texture;
        SourceRectangle = sourceRectangle;
        XOffset = xOffset;
        YOffset = yOffset;
        XAdvance = xAdvance;
        Kernings = new Dictionary<int, int>();
    }
}
