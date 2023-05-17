using Sharprompt;
using System;
using System.Collections.Generic;
using System.IO;

namespace DevConsole.Infrastructure.Services;

public class Prompts
{
    public T Input<T>(string message)
    {
        return Prompt.Input<T>(message);
    }

    public string Select(string message, ICollection<string> items, int pageSize = 25)
    {
        try
        {
            return Prompt.Select(message, items, pageSize);
        }
        catch (IOException)
        {
            // Fallback prompt - fails in Git Bash
            Console.WriteLine(message);

            foreach (var item in items)
            {
                Console.WriteLine($"- {item}");
            }

            string result;

            do
            {
                Console.Write("> ");
                result = Console.ReadLine()!;
            } while (!items.Contains(result));

            return result;
        }
    }

    public IEnumerable<string> MultiSelect(string message, ICollection<string> items, int minimum = 1,
                                           int maximum = int.MaxValue, int pageSize = 25)
    {
        try
        {
            return Prompt.MultiSelect(message, items, pageSize, minimum, maximum);
        }
        catch (IOException)
        {
            // Fallback prompt to single selection - fails in Git Bash
            return new[] { Select(message, items) };
        }
    }

    public bool Confirm(string message, bool? defaultValue = null)
    {
        try
        {
            return Prompt.Confirm(message, defaultValue);
        }
        catch (IOException)
        {
            // Fallback prompt - fails in Git Bash
            Console.WriteLine(message);

            var result = Console.ReadLine();

            return result?.ToLowerInvariant().StartsWith("y") == true;
        }
    }
}