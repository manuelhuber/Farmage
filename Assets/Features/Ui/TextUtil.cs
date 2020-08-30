namespace Features.Ui {
public static class TextUtil {
    public static string getPercentage(float top, float bottom) {
        var progressPercentage = bottom < 0.1 ? 0 : top / bottom * 100;
        return $"{progressPercentage:00}%";
    }
}
}