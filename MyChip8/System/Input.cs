namespace MyChip8.SystemComponents;

/// <summary>
/// CHIP-8 Input: 16-key hexadecimal keypad (0x0-0xF).
/// Original layout:     Modern keyboard mapping:
/// 1 2 3 C              1 2 3 4
/// 4 5 6 D              Q W E R
/// 7 8 9 E              A S D F
/// A 0 B F              Z X C V
/// </summary>
public class Input
{
    private readonly bool[] _keyStates = new bool[16];
    private int? _lastKeyPressed;

    /// <summary>
    /// Checks if a specific key is currently pressed.
    /// </summary>
    /// <param name="key">Key value (0x0-0xF)</param>
    /// <returns>True if key is pressed</returns>
    public bool IsKeyPressed(byte key)
    {
        if (key > 0xF)
            return false;
        return _keyStates[key];
    }

    /// <summary>
    /// Sets the state of a specific key.
    /// </summary>
    /// <param name="key">Key value (0x0-0xF)</param>
    /// <param name="pressed">True if pressed, false if released</param>
    public void SetKeyState(byte key, bool pressed)
    {
        if (key > 0xF)
            return;

        _keyStates[key] = pressed;
        if (pressed)
        {
            _lastKeyPressed = key;
        }
    }

    /// <summary>
    /// Waits for and returns the next key press.
    /// Returns null if no key has been pressed yet.
    /// </summary>
    /// <returns>The key value (0x0-0xF) or null</returns>
    public byte? WaitForKeyPress()
    {
        if (_lastKeyPressed.HasValue)
        {
            byte key = (byte)_lastKeyPressed.Value;
            _lastKeyPressed = null;
            return key;
        }
        return null;
    }

    /// <summary>
    /// Checks if any key is currently pressed.
    /// </summary>
    public bool AnyKeyPressed()
    {
        for (int i = 0; i < 16; i++)
        {
            if (_keyStates[i])
                return true;
        }
        return false;
    }

    /// <summary>
    /// Resets all key states.
    /// </summary>
    public void Clear()
    {
        for (int i = 0; i < 16; i++)
        {
            _keyStates[i] = false;
        }
        _lastKeyPressed = null;
    }
}
