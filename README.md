# CompareBitmaps
Класс с методами для сравнения bitmap.
1. CompareBitmapsFastUnsafe - самый быстрый способ сравнить bitmaps. За быстроту приходится платить unsafe кодом.
2. CompareBitmapsFast - наиболее быстрый способ без unsafe кода. Немного проигрывает CompareBitmapsFastUnsafe.
3. CompareBitmapsMemoryStream - сравнивает bitmap используя MemoryStream. Сравнивает весь файл, а не отдельные пиксели. Медленнее, чем CompareBitmapsFast.
4. CompareBitmapsLazy - самый неэффективный и медленный способ сравнения из-за GetPixel.
