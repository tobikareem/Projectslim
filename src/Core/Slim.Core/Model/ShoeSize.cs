namespace Slim.Core.Model
{
    public class ShoeSize
    {
        public bool IsMale { get; set; }

        public static double[] MensNgSize => new []
        {
            38, 39, 39.5, 40, 40.5, 41, 41.5, 42, 42.5,
            43, 43.5, 44, 44.5, 45, 45.5, 46, 46.5, 47
        };

        public static double[] MensEuSize => new []
        {
            38, 39, 39.5, 40, 40.5, 41, 41.5, 42, 42.5,
            43, 43.5, 44, 44.5, 45, 45.5, 46, 46.5, 47
        };

        public static double[] MensUkSize => new []
        {
            4, 5, 5.5, 6, 6.5, 7, 7.5, 8, 8.5, 9, 9.5,
            10, 10.5, 11, 11.5, 12, 12.5, 13
        };

        public static double[] MensUsSize => new []
        {
            5, 6, 6.5, 7, 7.5, 8, 8.5, 9, 9.5, 10, 10.5,
            11, 11.5, 12, 12.5, 13, 13.5, 14
        };

        public static double[] WomensNgSize => new []
        {
            35, 36, 36.5, 37, 37.5, 38, 38.5, 39,
            39.5, 40, 40.5, 41
        };

        public static double[] WomensEuSize => new []
        {
            35, 36, 36.5, 37, 37.5, 38, 38.5, 39,
            39.5, 40, 40.5, 41
        };

        public static double[] WomensUkSize => new []
        {
            2, 3, 3.5, 4, 4.5, 5, 5.5, 6, 6.5, 7, 7.5, 8
        };

        public static double[] WomensUsSize => new []
        {
            5, 6, 6.5, 7, 7.5, 8, 8.5, 9, 9.5, 10, 10.5, 11
        };
    }
}
