# MonoGame SDF Fonts
This is an example of rendering Signed Distance Field Fonts in MonoGame.  Someone smarter than me take this and do cool things.

## How to Install.
1. Clone the source for this repository
2. Add a reference to `/source/MonoGame.Sdf.Fonts/MonoGame.Sdf/Fonts.csproj` in your MonoGame project
3. Add the `/source/MonoGame.Sdf.Fonts/sdf-effect.fx` effect file to your project using the MGCB Editor.


## How to Create a Distance Field Font
1. Download the [libGDX Hiero](https://libgdx.com/wiki/tools/hiero) application and run it.
2. In the application, select the font you would like to use

![Select Font](./images/select-font.png)

3. Click the **[x]** on the left to remove the **Color** effect that is set by default

![Remove Color Effect](./images/remove-color-effect.png)

4. Select the **Distance Field** effect then click the **Add** button

![Add Distance Field Effect](./images/add-distance-field-effect.png)

5. Set the **Spread**, **Padding**, **Spacing**, and **Scale**, values in the following order
   1. Set the **Spread** value so that it is about half the width of the thickest line in your font, in pixels.
   2. Set all four **Padding** values to the same value you set for **Spread**.
   3. Set both the X and Y **Spacing** values to *minus* twice the **Spread** value.
   4. Last, set the **Scale** value to your desired font size.

> [!TIP]
> We set the **Scale** value last because the larger the scale, the longer Hiero takes to render the preview.  By keeping it at 1 while adjusting the other settings, the setting the **Scale** last, you won't get as many UI freezes while it rasterizes the preview.

![Set Values](./images/set-values.png)

6. Click **File** then **Save BMFont files (text)** from the top menu and to export it.  This will export a `.fnt` file and the associated `.png` texture files that go with it.

![Export BMFont File](./images//export.png)

## How To Use

1. Add the `.fnt` file and it's associated textures to your project using the MGCB Editor, but set their build action to **Copy** (even the texture).  See the section above for how to generate these files for use.
2. Wherever you load your content, load the `.fnt` file and the `sdf-effect` shader

```cs
//  Load the .fnt file from disk
using (FileStream stream = TitleContainer.OpenStream("Content/my-font.fnt") as FileStream)
{
   _font = BitmapFont.FromStream(GraphicsDevice, stream);
}

//  Load the SDF font shader effect
_sdfEffect = Content.Load<Effect>("sdf-effect");
```

3. Before rendering, set the required **Spread** and **Scale** properties of the effect

```cs
// Spread is the spread value you set in Hiero
_sdfEffect.Parameters["Spread"].SetValue(4.0f);

// Scale is the scale at which you are rendering the font when using the SpriteBatch.DrawString command
_sdfEffect.Parameters["Scale"].SetValue(1.0f);
```

4. Finally, load the effect as the parameter in the `SpriteBatch.Begin` then render your text using the `SpriteBatch.DrawString` extension methods

```cs
_spriteBatch.Begin(effect: _sdfEffect);
_spriteBatch.DrawString(_bitmapFont, "Hello World", new Vector2(0, 100), Color.White, 0.0f, Vector2.Zero, scale: 5.0f, 0);
_spriteBatch.End();
```

See the [example project](./example/Game1.cs) for a complete example of how to use.

## License
**MonoGame.Sdf.Fonts** is licensed under the **MIT License**. Please refer to the [LICENSE](./LICENSE) file for full license text.

## Credits
- [MonoGame.Extended](https://github.com/craftworkgames/MonoGame.Extended).  The initial work by the original maintainers and contributors here on rendering BMfont and my work on parsing the AngleCode Bitmap Font Text file format.
- [libGDX](https://libgdx.com/) for the [Hiero tool](https://libgdx.com/wiki/tools/hiero) which provides a method of exporting Distance Field fonts in the AngleCode Bitmap Font file format and the [rendering documentation](https://libgdx.com/wiki/graphics/2d/fonts/distance-field-fonts) where the initial shader code was modified from.
