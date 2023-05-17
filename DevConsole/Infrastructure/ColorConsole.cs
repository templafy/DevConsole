using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DevConsole.Infrastructure;

public class ColorConsole
{
    public static void WriteLine(string str, ConsoleColor? color = null)
    {
        lock (Console.Out)
        {
            using (ColorChanger.Change(color))
            {
                Console.WriteLine(str);
            }
        }
    }

    public static void Write(string str, ConsoleColor? color = null)
    {
        using (ColorChanger.Change(color))
        {
            Console.Write(str);
        }
    }

    public static void WriteFailure(string? message = null)
    {
        lock (Console.Out)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                Write(message + "... ");
            }

            WriteLine("X", ConsoleColor.Red);
        }
    }

    public static void WriteSuccess(string? message = null)
    {
        lock (Console.Out)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                Write(message + "... ");
            }

            WriteLine("✓", ConsoleColor.Green);
        }
    }

    public static bool CheckFails(string title, bool failureCondition)
    {
        return CheckFails(title, () => failureCondition);
    }

    public static bool CheckFails(string title, Func<bool> failureCondition)
    {
        Console.Write($"Checking {title}... ");

        var failed = failureCondition();
        if (failed)
        {
            WriteFailure();
        }
        else
        {
            WriteSuccess();
        }

        return failed;
    }

    public static async Task<bool> CheckFails(string title, Func<Task<bool>> failureCondition)
    {
        Console.Write($"Checking {title}... ");

        var failed = await failureCondition();
        if (failed)
        {
            WriteFailure();
        }
        else
        {
            WriteSuccess();
        }

        return failed;
    }

    public static void WriteCommand(string command)
    {
        if (string.IsNullOrWhiteSpace(command))
        {
            return;
        }

        try
        {
            lock (Console.Out)
            {
                Write(DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss"), ConsoleColor.DarkGray);
                WriteLine($"> {command}", ConsoleColor.Cyan);
            }
        }
        finally
        {
            Console.ResetColor();
        }
    }

    public static string? FormatTerminalLink(string? url, string? title = null)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return url;
        }

        if (Console.IsInputRedirected)
        {
            return url;
        }

        title ??= url;
        return $"\x1B]8;;{url}\x1B\\{title}\x1B]8;;\x1B\\";
    }

    public static string StripAnsiCodes(string str)
    {
        if (string.IsNullOrWhiteSpace(str))
        {
            return str;
        }

        return Regex.Replace(str, @"\x1B\[[^@-~]*[@-~]", string.Empty);
    }

    private class ColorChanger : IDisposable
    {
        private readonly ConsoleColor _originalForegroundColor;

        public ColorChanger(ConsoleColor? foregroundColor)
        {
            _originalForegroundColor = Console.ForegroundColor;

            if (foregroundColor.HasValue)
            {
                Console.ForegroundColor = foregroundColor.Value;
            }
        }

        public void Dispose()
        {
            Console.ForegroundColor = _originalForegroundColor;
        }

        public static IDisposable Change(ConsoleColor? foregroundColor)
        {
            return new ColorChanger(foregroundColor);
        }
    }
}