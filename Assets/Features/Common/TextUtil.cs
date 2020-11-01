namespace Features.Common {
public static class TextUtil {
    public static string PercentageString(float counter, float denominator) {
        var progressPercentage = denominator < 0.1 ? 0 : counter / denominator * 100;
        return $"{progressPercentage:00}%";
    }
}
}