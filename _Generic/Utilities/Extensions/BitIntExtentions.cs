namespace SpiritWorlds.Utilities {
  public static class BitIntExtentions {
    public static int TurnBitOn(this int value, int bitToTurnOn) {
      return (value | bitToTurnOn);
    }

    public static int TurnBitOff(this int value, int bitToTurnOff) {
      return (value & ~bitToTurnOff);
    }

    public static int FlipBit(this int value, int bitToFlip) {
      return (value ^ bitToFlip);
    }
  }
}
