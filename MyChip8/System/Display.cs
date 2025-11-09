namespace MyChip8.SystemComponents;

/// <summary>
/// CHIP-8 Display: 64x32 monochrome (black/white) pixels.
/// Drawing uses XOR mode - pixels are toggled when drawn.
/// </summary>
public class Display
{
    public const int Width = 64;
    public const int Height = 32;

    private readonly bool[,] _pixels = new bool[Width, Height];

    /// <summary>
    /// Gets the pixel state at the specified coordinates.
    /// </summary>
    public bool GetPixel(int x, int y)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height)
            return false;
        return _pixels[x, y];
    }

    /// <summary>
    /// Sets the pixel state at the specified coordinates.
    /// </summary>
    public void SetPixel(int x, int y, bool value)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height)
            return;
        _pixels[x, y] = value;
    }

    /// <summary>
    /// Clears the entire display (sets all pixels to off/false).
    /// </summary>
    public void Clear()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                _pixels[x, y] = false;
            }
        }
    }

    /// <summary>
    /// Draws a sprite at the specified coordinates using XOR mode.
    /// Returns true if any pixel was turned off (collision detection).
    /// </summary>
    /// <param name="x">X coordinate (wraps around screen)</param>
    /// <param name="y">Y coordinate (wraps around screen)</param>
    /// <param name="spriteData">Array of bytes, each representing 8 pixels (1 bit per pixel)</param>
    /// <returns>True if any pixel collision occurred (pixel turned off)</returns>
    public bool DrawSprite(int x, int y, byte[] spriteData)
    {
        bool collision = false;

        for (int row = 0; row < spriteData.Length; row++)
        {
            byte spriteByte = spriteData[row];
            int yPos = (y + row) % Height; // Wrap vertically

            for (int col = 0; col < 8; col++)
            {
                // Check if this bit is set in the sprite
                bool spritePixel = (spriteByte & (0x80 >> col)) != 0;

                if (spritePixel)
                {
                    int xPos = (x + col) % Width; // Wrap horizontally

                    // Check for collision (pixel was on and will be turned off)
                    if (_pixels[xPos, yPos])
                    {
                        collision = true;
                    }

                    // XOR the pixel
                    _pixels[xPos, yPos] ^= true;
                }
            }
        }

        return collision;
    }

    /// <summary>
    /// Gets a copy of the current display buffer for rendering.
    /// </summary>
    public bool[,] GetBuffer()
    {
        var buffer = new bool[Width, Height];
        Array.Copy(_pixels, buffer, _pixels.Length);
        return buffer;
    }
}
