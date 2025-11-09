# CHIP-8 Technical Specification

## Overview

CHIP-8 is an interpreted low-level programming language and virtual machine specification designed by Joseph Weisbecker in 1977 for the COSMAC VIP computer. It has become a popular choice for learning emulator development due to its simplicity and completeness as a machine model.

## Memory Architecture

The system contains **4 KB (4096 bytes)** of RAM indexed from `0x000` to `0xFFF`:

| Address Range | Purpose |
|--------------|---------|
| `0x000-0x1FF` | Reserved for system use (interpreter and font data) |
| `0x000-0x04F` | Built-in font data for hexadecimal characters 0-F (5 bytes per character) |
| `0x200-0xFFF` | Program and data space (standard entry point: `0x200`) |
| `0x600-0xFFF` | Alternative entry point for ETI 660 programs |

**Note:** Instructions must begin at even memory addresses.

## Register Set

### General Purpose Registers
- **V0-VF**: 16 general-purpose 8-bit registers
  - `VF` is used as a flag register for carry/borrow and collision detection
  - Programs should not use `VF` directly as it's modified by various instructions

### Special Registers
- **I**: 16-bit index register (typically stores memory addresses)
- **PC**: 16-bit program counter (points to current instruction)
- **SP**: 8-bit stack pointer (indicates top of stack)

### Timers
- **DT**: 8-bit delay timer (decrements at 60 Hz when > 0)
- **ST**: 8-bit sound timer (decrements at 60 Hz; triggers buzzer when > 0)

## Stack

A LIFO (Last In, First Out) structure holding 16-bit values:
- Supports at least **16 levels** of nested subroutine calls
- Used for managing return addresses during `CALL` and `RET` instructions

## Display System

### Specifications
- **Resolution**: 64×32 pixels
- **Color**: Monochrome (black and white)
- **Coordinate System**:
  - Origin `[0,0]` at top-left
  - Maximum `[63,31]` at bottom-right
  - Coordinates wrap at boundaries (modulo operation)

### Graphics
- **Sprites**: 8 pixels wide, 1-15 pixels tall
- **Encoding**: Each sprite row is 1 byte (8 bits = 8 pixels)
- **Drawing Method**: XOR operation with existing display content
  - Pixels are toggled (on→off, off→on)
  - Collision detection: `VF` set to 1 if any pixel turns from on to off

## Input System

### Keyboard Layout
16-key hexadecimal keypad (keys 0x0-0xF):

```
Original Layout:    Modern Keyboard Mapping:
1 2 3 C             1 2 3 4
4 5 6 D             Q W E R
7 8 9 E             A S D F
A 0 B F             Z X C V
```

## Timers

Both timers operate independently at **60 Hz**:

### Delay Timer (DT)
- Decrements automatically at 60 Hz when > 0
- Readable and writable via instructions
- Used for timing game events

### Sound Timer (ST)
- Decrements at 60 Hz when > 0
- Activates system buzzer/beep while ST > 0
- Writable only (programs set it, hardware decrements it)

## Instruction Set

All instructions are **2 bytes long**, stored **most-significant byte first** (big-endian).

### Instruction Format Notation
- `NNN`: 12-bit address
- `NN` or `KK`: 8-bit constant
- `N`: 4-bit constant
- `X` and `Y`: 4-bit register identifiers (0-F)

### Complete Instruction Set (36 Instructions)

#### System & Flow Control

| Opcode | Mnemonic | Description |
|--------|----------|-------------|
| `00E0` | `CLS` | Clear the display |
| `00EE` | `RET` | Return from subroutine (pop PC from stack) |
| `0NNN` | `SYS NNN` | Jump to machine code routine (ignored in modern interpreters) |
| `1NNN` | `JP NNN` | Jump to address NNN |
| `2NNN` | `CALL NNN` | Call subroutine at NNN (push PC to stack, then jump) |
| `BNNN` | `JP V0, NNN` | Jump to address NNN + V0 |

#### Conditional Operations

| Opcode | Mnemonic | Description |
|--------|----------|-------------|
| `3XNN` | `SE VX, NN` | Skip next instruction if VX == NN |
| `4XNN` | `SNE VX, NN` | Skip next instruction if VX ≠ NN |
| `5XY0` | `SE VX, VY` | Skip next instruction if VX == VY |
| `9XY0` | `SNE VX, VY` | Skip next instruction if VX ≠ VY |

#### Register Operations

| Opcode | Mnemonic | Description |
|--------|----------|-------------|
| `6XNN` | `LD VX, NN` | Set VX = NN |
| `7XNN` | `ADD VX, NN` | Set VX = VX + NN (no carry flag) |
| `8XY0` | `LD VX, VY` | Set VX = VY |
| `8XY4` | `ADD VX, VY` | Set VX = VX + VY, VF = carry (1 if > 255, else 0) |
| `8XY5` | `SUB VX, VY` | Set VX = VX - VY, VF = NOT borrow (1 if VX > VY, else 0) |
| `8XY7` | `SUBN VX, VY` | Set VX = VY - VX, VF = NOT borrow (1 if VY > VX, else 0) |

#### Bitwise Operations

| Opcode | Mnemonic | Description |
|--------|----------|-------------|
| `8XY1` | `OR VX, VY` | Set VX = VX OR VY |
| `8XY2` | `AND VX, VY` | Set VX = VX AND VY |
| `8XY3` | `XOR VX, VY` | Set VX = VX XOR VY |
| `8XY6` | `SHR VX` | Set VX = VX >> 1, VF = least significant bit before shift |
| `8XYE` | `SHL VX` | Set VX = VX << 1, VF = most significant bit before shift |

#### Memory & Index Register

| Opcode | Mnemonic | Description |
|--------|----------|-------------|
| `ANNN` | `LD I, NNN` | Set I = NNN |
| `FX1E` | `ADD I, VX` | Set I = I + VX |
| `FX29` | `LD F, VX` | Set I to location of sprite for digit VX (I = VX × 5) |
| `FX33` | `LD B, VX` | Store BCD representation of VX at I, I+1, I+2 |
| `FX55` | `LD [I], VX` | Store registers V0-VX in memory starting at I |
| `FX65` | `LD VX, [I]` | Load registers V0-VX from memory starting at I |

#### Graphics

| Opcode | Mnemonic | Description |
|--------|----------|-------------|
| `DXYN` | `DRW VX, VY, N` | Draw N-byte sprite at (VX, VY); VF = collision (1 if any pixel turned off) |

#### Random

| Opcode | Mnemonic | Description |
|--------|----------|-------------|
| `CXNN` | `RND VX, NN` | Set VX = random byte AND NN |

#### Keyboard Input

| Opcode | Mnemonic | Description |
|--------|----------|-------------|
| `EX9E` | `SKP VX` | Skip next instruction if key with value VX is pressed |
| `EXA1` | `SKNP VX` | Skip next instruction if key with value VX is NOT pressed |

#### Timers

| Opcode | Mnemonic | Description |
|--------|----------|-------------|
| `FX07` | `LD VX, DT` | Set VX = delay timer value |
| `FX0A` | `LD VX, K` | Wait for key press, store key value in VX (blocking) |
| `FX15` | `LD DT, VX` | Set delay timer = VX |
| `FX18` | `LD ST, VX` | Set sound timer = VX |

## Built-in Font Data

CHIP-8 includes built-in sprites for hexadecimal digits 0-F. Each character is 5 bytes tall and 8 pixels wide:

```
Character 0: 0xF0, 0x90, 0x90, 0x90, 0xF0
Character 1: 0x20, 0x60, 0x20, 0x20, 0x70
Character 2: 0xF0, 0x10, 0xF0, 0x80, 0xF0
Character 3: 0xF0, 0x10, 0xF0, 0x10, 0xF0
Character 4: 0x90, 0x90, 0xF0, 0x10, 0x10
Character 5: 0xF0, 0x80, 0xF0, 0x10, 0xF0
Character 6: 0xF0, 0x80, 0xF0, 0x90, 0xF0
Character 7: 0xF0, 0x10, 0x20, 0x40, 0x40
Character 8: 0xF0, 0x90, 0xF0, 0x90, 0xF0
Character 9: 0xF0, 0x90, 0xF0, 0x10, 0xF0
Character A: 0xF0, 0x90, 0xF0, 0x90, 0x90
Character B: 0xE0, 0x90, 0xE0, 0x90, 0xE0
Character C: 0xF0, 0x80, 0x80, 0x80, 0xF0
Character D: 0xE0, 0x90, 0x90, 0x90, 0xE0
Character E: 0xF0, 0x80, 0xF0, 0x80, 0xF0
Character F: 0xF0, 0x80, 0xF0, 0x80, 0x80
```

Font data is typically stored at memory addresses `0x000-0x04F` (80 bytes total).

## Critical Implementation Notes

### 1. Draw Instruction (DXYN)
The most complex operation:
1. Read N bytes from memory starting at address I
2. Each byte represents one row of the sprite (8 pixels)
3. XOR each pixel with the display at coordinates (VX mod 64, VY mod 32)
4. Set VF = 1 if ANY pixel changed from on to off (collision detection)
5. Coordinates wrap at screen boundaries

### 2. Coordinate Wrapping
Display coordinates wrap using modulo:
- X coordinate: `x % 64`
- Y coordinate: `y % 32`

### 3. VF Register Behavior
Automatically set by certain instructions:
- **8XY4**: Carry flag (1 if result > 255)
- **8XY5**: NOT borrow (1 if VX > VY)
- **8XY7**: NOT borrow (1 if VY > VX)
- **8XY6**: LSB before shift
- **8XYE**: MSB before shift
- **DXYN**: Collision flag (1 if any pixel turned off)

Programs should not use VF for general purpose storage.

### 4. Instruction Execution Speed
- Original CHIP-8: ~500-1000 Hz (instructions per second)
- Modern implementations vary widely
- Timers MUST run at exactly 60 Hz regardless of CPU speed

### 5. Memory Access
- All memory reads/writes are 8-bit (byte-sized)
- Instructions are 16-bit but fetched as two consecutive bytes
- Big-endian byte order for multi-byte values

### 6. Stack Implementation
- Stack grows upward (SP increments on push)
- Minimum 16 levels deep
- Stores 16-bit return addresses

### 7. Program Loading
- Programs start at address `0x200` (512 decimal)
- Font data must be pre-loaded at `0x000-0x04F`
- Maximum program size: 3584 bytes (4096 - 512)

## Quirks and Variations

Different CHIP-8 implementations have subtle differences. The specification above follows the most common "vanilla" CHIP-8 behavior. Notable variations include:

### CHIP-48 / SUPER-CHIP
- Higher resolution (128×64)
- Additional instructions
- Different shift behavior (8XY6/8XYE)

### Modern Quirks
- **Memory access**: Some interpreters increment I after FX55/FX65
- **Shift operations**: Some use VY as source, others ignore it
- **Display wait**: Some interpreters wait for VBlank before drawing
- **Clipping**: Some clip sprites at screen edges instead of wrapping

## References

- Cowgod's Chip-8 Technical Reference v1.0
- Tobias V. Langhoff's Guide to making a CHIP-8 emulator
- Matthew Mikolay's Mastering CHIP-8

---

*This specification is based on the original CHIP-8 interpreter for the COSMAC VIP.*
