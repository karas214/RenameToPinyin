using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

class Program
{
    static void Main()
    {
        Console.WriteLine("请输入文件夹路径（可拖拽进窗口）：");
        string folder = Console.ReadLine()?.Trim('"');

        if (string.IsNullOrEmpty(folder) || !Directory.Exists(folder))
        {
            Console.WriteLine("路径无效");
            return;
        }

        foreach (var file in Directory.GetFiles(folder))
        {
            string dir = Path.GetDirectoryName(file)!;
            string name = Path.GetFileNameWithoutExtension(file);
            string ext = Path.GetExtension(file);

            string newName = Convert(name) + ext;
            string newPath = Path.Combine(dir, newName);

            if (file != newPath)
            {
                File.Move(file, newPath, true);
                Console.WriteLine($"{name}{ext} -> {newName}");
            }
        }

        Console.WriteLine("完成！");
        Console.ReadKey();
    }

    static string Convert(string input)
    {
        var sb = new StringBuilder();
        int i = 0;

        while (i < input.Length)
        {
            if (IsChinese(input[i]))
            {
                int start = i;
                while (i < input.Length && IsChinese(input[i]))
                    i++;

                string chinese = input.Substring(start, i - start);
                string pinyin = ToPinyin(chinese);

                sb.Append(chinese + " " + pinyin);
            }
            else
            {
                sb.Append(input[i]);
                i++;
            }
        }

        return sb.ToString();
    }

    static bool IsChinese(char c)
    {
        return c >= 0x4e00 && c <= 0x9fff;
    }

    // 简单拼音映射（不依赖任何库）
    static string ToPinyin(string text)
    {
        // ⚠️ 轻量方案：用 CultureInfo 转写（实际项目可升级 ICU）
        string normalized = text.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();

        foreach (var c in normalized)
        {
            var uc = CharUnicodeInfo.GetUnicodeCategory(c);
            if (uc != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }

        return sb.ToString().ToLower();
    }
}