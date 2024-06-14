// Copyright (c) Craftwork Games. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGame.Sdf.Fonts;

internal sealed class BmfTextLoader : IDisposable
{
    private bool _isDisposed;
    private GraphicsDevice _graphicsDevice;
    private FileStream _stream;

    public BmfTextLoader(GraphicsDevice graphicsDevice, Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(graphicsDevice);
        if (stream is not FileStream fileStream)
        {
            throw new ArgumentException($"{nameof(stream)} must be a {nameof(FileStream)}.", nameof(stream));
        }
        _stream = fileStream;
        _graphicsDevice = graphicsDevice;

    }

    ~BmfTextLoader() => Dispose(false);

    public BitmapFont Load()
    {
        string directory = Path.GetDirectoryName(_stream.Name);
        string face = string.Empty;
        int size = default;
        int lineHeight = default;
        List<Texture2D> pages = new List<Texture2D>();
        Dictionary<int, BitmapFontCharacter> characters = new Dictionary<int, BitmapFontCharacter>();

        using StreamReader reader = new StreamReader(_stream);
        string line = default;
        while ((line = reader.ReadLine()) != null)
        {
            List<string> tokens = GetTokens(line);
            if (tokens.Count == 0) { continue; }

            string blockType = tokens[0];
            switch (blockType)
            {
                case "info":
                    {
                        for (int i = 1; i < tokens.Count; i++)
                        {
                            string[] attributes = tokens[i].Split('=');
                            if (attributes.Length != 2)
                            {
                                continue;
                            }

                            if (attributes[0].Equals("face", StringComparison.OrdinalIgnoreCase))
                            {
                                face = attributes[1].Replace("\"", string.Empty, StringComparison.Ordinal);
                            }

                            if (attributes[0].Equals("size", StringComparison.OrdinalIgnoreCase))
                            {
                                size = Convert.ToInt16(attributes[1], CultureInfo.InvariantCulture);
                            }

                            if (!string.IsNullOrEmpty(face) && size != 0)
                            {
                                break;
                            }
                        }
                    }
                    break;

                case "common":
                    {
                        for (int i = 1; i < tokens.Count; i++)
                        {
                            string[] attributes = tokens[i].Split('=');
                            if (attributes.Length != 2)
                            {
                                continue;
                            }

                            if (attributes[0].Equals("lineHeight", StringComparison.OrdinalIgnoreCase))
                            {
                                lineHeight = Convert.ToUInt16(attributes[1], CultureInfo.InvariantCulture);
                                break;
                            }
                        }
                    }
                    break;

                case "page":
                    {
                        for (int i = 1; i < tokens.Count; i++)
                        {
                            string[] attributes = tokens[i].Split('=');
                            if (attributes.Length != 2)
                            {
                                continue;
                            }

                            if (attributes[0].Equals("file", StringComparison.OrdinalIgnoreCase))
                            {
                                string page = attributes[1].Replace("\"", string.Empty, StringComparison.Ordinal);
                                Texture2D texture = Texture2D.FromFile(_graphicsDevice, Path.Combine(directory, page), DefaultColorProcessors.PremultiplyAlpha);
                                pages.Add(texture);
                            }
                        }
                    }
                    break;

                case "char":
                    {
                        int id = default;
                        int x = default;
                        int y = default;
                        int width = default;
                        int height = default;
                        int xOffset = default;
                        int yOffset = default;
                        int xAdvance = default;
                        int page = default;

                        for (int i = 1; i < tokens.Count; i++)
                        {

                            string[] attributes = tokens[i].Split('=');
                            if (attributes.Length != 2)
                            {
                                continue;
                            }

                            switch (attributes[0])
                            {
                                case "id":
                                    id = Convert.ToInt32(attributes[1], CultureInfo.InvariantCulture);
                                    break;
                                case "x":
                                    x = Convert.ToInt32(attributes[1], CultureInfo.InvariantCulture);
                                    break;
                                case "y":
                                    y = Convert.ToInt32(attributes[1], CultureInfo.InvariantCulture);
                                    break;
                                case "width":
                                    width = Convert.ToInt32(attributes[1], CultureInfo.InvariantCulture);
                                    break;
                                case "height":
                                    height = Convert.ToInt32(attributes[1], CultureInfo.InvariantCulture);
                                    break;
                                case "xoffset":
                                    xOffset = Convert.ToInt32(attributes[1], CultureInfo.InvariantCulture);
                                    break;
                                case "yoffset":
                                    yOffset = Convert.ToInt32(attributes[1], CultureInfo.InvariantCulture);
                                    break;
                                case "xadvance":
                                    xAdvance = Convert.ToInt32(attributes[1], CultureInfo.InvariantCulture);
                                    break;
                                case "page":
                                    page = Convert.ToInt32(attributes[1], CultureInfo.InvariantCulture);
                                    break;
                            }
                        }

                        Rectangle sourceRectangle = new Rectangle(x, y, width, height);
                        BitmapFontCharacter character = new BitmapFontCharacter(id, pages[page], sourceRectangle, xOffset, yOffset, xAdvance);
                        characters.Add(id, character);
                    }
                    break;

                case "kerning":
                    {
                        int first = default;
                        int second = default;
                        int amount = default;

                        for (int i = 1; i < tokens.Count; i++)
                        {
                            string[] attributes = tokens[i].Split('=');
                            if (attributes.Length != 2)
                            {
                                continue;
                            }

                            switch (attributes[0])
                            {
                                case "first":
                                    first = Convert.ToInt32(attributes[1], CultureInfo.InvariantCulture);
                                    break;
                                case "second":
                                    second = Convert.ToInt32(attributes[1], CultureInfo.InvariantCulture);
                                    break;
                                case "amount":
                                    amount = Convert.ToInt32(attributes[1], CultureInfo.InvariantCulture);
                                    break;
                            }
                        }

                        if (characters.TryGetValue(first, out BitmapFontCharacter character))
                        {
                            character.Kernings.Add(second, amount);
                        }
                    }
                    break;
            }
        }

        return new BitmapFont(face, size, lineHeight, characters.Values);
    }

    private static List<string> GetTokens(string line)
    {
        List<string> tokens = new List<string>();
        StringBuilder currentToken = new StringBuilder();
        bool inQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == ' ' && !inQuotes)
            {
                if (currentToken.Length > 0)
                {
                    tokens.Add(currentToken.ToString());
                    currentToken.Clear();
                }
            }
            else if (c == '"')
            {
                inQuotes = !inQuotes;
            }
            else
            {
                currentToken.Append(c);
            }
        }

        if (currentToken.Length > 0)
        {
            tokens.Add(currentToken.ToString());
        }

        return tokens;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool isDisposing)
    {
        if (_isDisposed)
        {
            return;
        }

        if (isDisposing)
        {
            _graphicsDevice = null;
            _stream = null;
        }

        _isDisposed = true;
    }
}
