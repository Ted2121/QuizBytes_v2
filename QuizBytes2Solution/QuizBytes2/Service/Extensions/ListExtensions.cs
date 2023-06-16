namespace QuizBytes2.Service.Extensions;

/// <summary>
/// Fisher-Yates shuffling algorithm for a generic list
/// </summary>
public static class ListExtensions
{
    private static Random random = new Random();

    public static void Shuffle<T>(this List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            list.Swap(i, j);
        }
    }

    private static void Swap<T>(this List<T> list, int i, int j)
    {
        T temp = list[i];
        list[i] = list[j];
        list[j] = temp;
    }
}