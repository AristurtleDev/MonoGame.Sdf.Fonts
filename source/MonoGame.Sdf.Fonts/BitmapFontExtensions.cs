// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Sdf.Fonts;

/// <summary>
/// Provides extension methods for drawing text using <see cref="BitmapFont"/> and <see cref="SpriteBatch"/>.
/// </summary>
public static class BitmapFontExtensions
{
    /// <summary>
    /// Draws the specified text string at the specified position using the specified font and default settings.
    /// </summary>
    /// <param name="spriteBatch">The sprite batch used for rendering.</param>
    /// <param name="font">The bitmap font to use for rendering.</param>
    /// <param name="text">The text string to draw.</param>
    /// <param name="position">The position to draw the text at.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if either <paramref name="spriteBatch"/> or <paramref name="font"/> are <see langword="null"/>.
    /// </exception>
    public static void DrawString(this SpriteBatch spriteBatch, BitmapFont font, string text, Vector2 position) =>
        DrawString(spriteBatch, font, text, position, Color.White, 0.0f, Vector2.Zero, 1.0f, 0.0f);

    /// <summary>
    /// Draws the specified text string at the specified position using the specified font and color.
    /// </summary>
    /// <param name="spriteBatch">The sprite batch used for rendering.</param>
    /// <param name="font">The bitmap font to use for rendering.</param>
    /// <param name="text">The text string to draw.</param>
    /// <param name="position">The position to draw the text at.</param>
    /// <param name="color">The color of the text.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if either <paramref name="spriteBatch"/> or <paramref name="font"/> are <see langword="null"/>.
    /// </exception>
    public static void DrawString(this SpriteBatch spriteBatch, BitmapFont font, string text, Vector2 position, Color color) =>
        DrawString(spriteBatch, font, text, position, color, 0.0f, Vector2.Zero, 1.0f, 0.0f);

    /// <summary>
    /// Draws the specified text string at the specified position using the specified font and scale.
    /// </summary>
    /// <param name="spriteBatch">The sprite batch used for rendering.</param>
    /// <param name="font">The bitmap font to use for rendering.</param>
    /// <param name="text">The text string to draw.</param>
    /// <param name="position">The position to draw the text at.</param>
    /// <param name="scale">The scale of the text.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if either <paramref name="spriteBatch"/> or <paramref name="font"/> are <see langword="null"/>.
    /// </exception>
    public static void DrawString(this SpriteBatch spriteBatch, BitmapFont font, string text, Vector2 position, float scale) =>
        DrawString(spriteBatch, font, text, position, Color.White, 0.0f, Vector2.Zero, scale, 0.0f);

    /// <summary>
    /// Draws the specified text string at the specified position using the specified font, color, and scale.
    /// </summary>
    /// <param name="spriteBatch">The sprite batch used for rendering.</param>
    /// <param name="font">The bitmap font to use for rendering.</param>
    /// <param name="text">The text string to draw.</param>
    /// <param name="position">The position to draw the text at.</param>
    /// <param name="color">The color of the text.</param>
    /// <param name="scale">The scale of the text.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if either <paramref name="spriteBatch"/> or <paramref name="font"/> are <see langword="null"/>.
    /// </exception>
    public static void DrawString(this SpriteBatch spriteBatch, BitmapFont font, string text, Vector2 position, Color color, float scale) =>
        DrawString(spriteBatch, font, text, position, color, 0.0f, Vector2.Zero, scale, 0.0f);

    /// <summary>
    /// Draws the specified text string at the specified position using the specified font, color, rotation, origin, scale, and layer depth.
    /// </summary>
    /// <param name="spriteBatch">The sprite batch used for rendering.</param>
    /// <param name="font">The bitmap font to use for rendering.</param>
    /// <param name="text">The text string to draw.</param>
    /// <param name="position">The position to draw the text at.</param>
    /// <param name="color">The color of the text.</param>
    /// <param name="rotation">The rotation of the text in radians.</param>
    /// <param name="origin">The origin of the text.</param>
    /// <param name="scale">The scale of the text.</param>
    /// <param name="layerDepth">The layer depth of the text.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if either <paramref name="spriteBatch"/> or <paramref name="font"/> are <see langword="null"/>.
    /// </exception>
    public static void DrawString(this SpriteBatch spriteBatch, BitmapFont font, string text, Vector2 position, Color color, float rotation, Vector2 origin, float scale, float layerDepth)
    {
        ArgumentNullException.ThrowIfNull(spriteBatch);
        ArgumentNullException.ThrowIfNull(font);
        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        var glyphs = font.GetGlyphs(text, position);

        foreach (var glyph in glyphs)
        {
            if (glyph.Character == null)
            {
                continue;
            }

            Vector2 characterOrigin = position - glyph.Position + origin;
            spriteBatch.Draw(glyph.Character.Texture, position, glyph.Character.SourceRectangle, color, rotation, characterOrigin, scale, SpriteEffects.None, layerDepth);
        }
    }

    /// <summary>
    /// Draws the specified text string at the specified position using the specified font and default settings.
    /// </summary>
    /// <param name="spriteBatch">The sprite batch used for rendering.</param>
    /// <param name="font">The bitmap font to use for rendering.</param>
    /// <param name="text">The text string to draw.</param>
    /// <param name="position">The position to draw the text at.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if either <paramref name="spriteBatch"/> or <paramref name="font"/> are <see langword="null"/>.
    /// </exception>
    public static void DrawString(this SpriteBatch spriteBatch, BitmapFont font, StringBuilder text, Vector2 position) =>
        DrawString(spriteBatch, font, text, position, Color.White, 0.0f, Vector2.Zero, 1.0f, 0.0f);

    /// <summary>
    /// Draws the specified text string at the specified position using the specified font and color.
    /// </summary>
    /// <param name="spriteBatch">The sprite batch used for rendering.</param>
    /// <param name="font">The bitmap font to use for rendering.</param>
    /// <param name="text">The text string to draw.</param>
    /// <param name="position">The position to draw the text at.</param>
    /// <param name="color">The color of the text.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if either <paramref name="spriteBatch"/> or <paramref name="font"/> are <see langword="null"/>.
    /// </exception>
    public static void DrawString(this SpriteBatch spriteBatch, BitmapFont font, StringBuilder text, Vector2 position, Color color) =>
        DrawString(spriteBatch, font, text, position, color, 0.0f, Vector2.Zero, 1.0f, 0.0f);

    /// <summary>
    /// Draws the specified text string at the specified position using the specified font and scale.
    /// </summary>
    /// <param name="spriteBatch">The sprite batch used for rendering.</param>
    /// <param name="font">The bitmap font to use for rendering.</param>
    /// <param name="text">The text string to draw.</param>
    /// <param name="position">The position to draw the text at.</param>
    /// <param name="scale">The scale of the text.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if either <paramref name="spriteBatch"/> or <paramref name="font"/> are <see langword="null"/>.
    /// </exception>
    public static void DrawString(this SpriteBatch spriteBatch, BitmapFont font, StringBuilder text, Vector2 position, float scale) =>
        DrawString(spriteBatch, font, text, position, Color.White, 0.0f, Vector2.Zero, scale, 0.0f);

    /// <summary>
    /// Draws the specified text string at the specified position using the specified font, color, and scale.
    /// </summary>
    /// <param name="spriteBatch">The sprite batch used for rendering.</param>
    /// <param name="font">The bitmap font to use for rendering.</param>
    /// <param name="text">The text string to draw.</param>
    /// <param name="position">The position to draw the text at.</param>
    /// <param name="color">The color of the text.</param>
    /// <param name="scale">The scale of the text.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if either <paramref name="spriteBatch"/> or <paramref name="font"/> are <see langword="null"/>.
    /// </exception>
    public static void DrawString(this SpriteBatch spriteBatch, BitmapFont font, StringBuilder text, Vector2 position, Color color, float scale) =>
        DrawString(spriteBatch, font, text, position, color, 0.0f, Vector2.Zero, scale, 0.0f);

    /// <summary>
    /// Draws the specified text string at the specified position using the specified font, color, rotation, origin, scale, and layer depth.
    /// </summary>
    /// <param name="spriteBatch">The sprite batch used for rendering.</param>
    /// <param name="font">The bitmap font to use for rendering.</param>
    /// <param name="text">The text string to draw.</param>
    /// <param name="position">The position to draw the text at.</param>
    /// <param name="color">The color of the text.</param>
    /// <param name="rotation">The rotation of the text in radians.</param>
    /// <param name="origin">The origin of the text.</param>
    /// <param name="scale">The scale of the text.</param>
    /// <param name="layerDepth">The layer depth of the text.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if either <paramref name="spriteBatch"/> or <paramref name="font"/> are <see langword="null"/>.
    /// </exception>
    public static void DrawString(this SpriteBatch spriteBatch, BitmapFont font, StringBuilder text, Vector2 position, Color color, float rotation, Vector2 origin, float scale, float layerDepth)
    {
        ArgumentNullException.ThrowIfNull(spriteBatch);
        ArgumentNullException.ThrowIfNull(font);
        if (text is null || text.Length == 0)
        {
            return;
        }

        BitmapFont.StringBuilderGlyphEnumerable glyphs = font.GetGlyphs(text, position);
        foreach (BitmapFont.BitmapFontGlyph glyph in glyphs)
        {
            if (glyph.Character == null)
                continue;
            var characterOrigin = position - glyph.Position + origin;
            spriteBatch.Draw(glyph.Character.Texture, position, glyph.Character.SourceRectangle, color, rotation, characterOrigin, scale, SpriteEffects.None, layerDepth);
        }
    }
}
