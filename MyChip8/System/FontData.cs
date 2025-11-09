namespace MyChip8.SystemComponents;

/// <summary>
/// CHIP-8 built-in hexadecimal font sprites.
/// Each character is 5 bytes tall and 8 pixels (1 byte) wide.
/// Font data is typically loaded at memory address 0x000-0x04F.
/// </summary>
public static class FontData
{
    /// <summary>
    /// Gets the built-in CHIP-8 font set.
    /// Each sprite is 5 bytes representing a 5x8 pixel character.
    /// </summary>
    public static readonly byte[] Font =
    [
        // 0
        0xF0, 0x90, 0x90, 0x90, 0xF0,
        // 1
        0x20, 0x60, 0x20, 0x20, 0x70,
        // 2
        0xF0, 0x10, 0xF0, 0x80, 0xF0,
        // 3
        0xF0, 0x10, 0xF0, 0x10, 0xF0,
        // 4
        0x90, 0x90, 0xF0, 0x10, 0x10,
        // 5
        0xF0, 0x80, 0xF0, 0x10, 0xF0,
        // 6
        0xF0, 0x80, 0xF0, 0x90, 0xF0,
        // 7
        0xF0, 0x10, 0x20, 0x40, 0x40,
        // 8
        0xF0, 0x90, 0xF0, 0x90, 0xF0,
        // 9
        0xF0, 0x90, 0xF0, 0x10, 0xF0,
        // A
        0xF0, 0x90, 0xF0, 0x90, 0x90,
        // B
        0xE0, 0x90, 0xE0, 0x90, 0xE0,
        // C
        0xF0, 0x80, 0x80, 0x80, 0xF0,
        // D
        0xE0, 0x90, 0x90, 0x90, 0xE0,
        // E
        0xF0, 0x80, 0xF0, 0x80, 0xF0,
        // F
        0xF0, 0x80, 0xF0, 0x80, 0x80
    ];

    /// <summary>
    /// Size of each font character in bytes.
    /// </summary>
    public const int FontCharacterSize = 5;

    /// <summary>
    /// Starting address where font data should be loaded in memory.
    /// </summary>
    public const int FontStartAddress = 0x000;

    /// <summary>
    /// Gets the memory address for a specific font character.
    /// </summary>
    /// <param name="character">Hex character (0x0-0xF)</param>
    /// <returns>Memory address of the font sprite</returns>
    public static int GetFontAddress(byte character)
    {
        if (character > 0xF)
            character = 0xF;
        return FontStartAddress + (character * FontCharacterSize);
    }

    /// <summary>
    /// Loads the font data into the specified memory starting at FontStartAddress.
    /// </summary>
    public static void LoadIntoMemory(Memory memory)
    {
        for (int i = 0; i < Font.Length; i++)
        {
            memory.SetByteAtAddress(FontStartAddress + i, Font[i]);
        }
    }
}
