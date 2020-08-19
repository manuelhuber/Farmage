using System;

namespace Features.Resources {
[Serializable]
public struct Cost {
    public int cash;

    public static Cost operator -(Cost a) {
        return new Cost {cash = -a.cash};
    }

    public static Cost operator +(Cost a, Cost b) {
        return new Cost {cash = a.cash + b.cash};
    }

    public static Cost operator -(Cost a, Cost b) {
        return a + -b;
    }

    public static bool operator <=(Cost a, Cost b) {
        // a <= b
        return a.cash <= b.cash;
    }

    public static bool operator >=(Cost a, Cost b) {
        return b <= a;
    }

    public override string ToString() {
        return $"Cash: {cash}";
    }
}
}