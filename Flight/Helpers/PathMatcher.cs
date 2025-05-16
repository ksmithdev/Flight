using System.IO;

/// <summary>
/// A collection of helper methods for pattern matching on file paths.
/// </summary>
public static class PathMatcher
{
    /// <summary>
    /// Returns whether the supplied pattern matches the supplied path.
    /// </summary>
    /// <param name="path">The path to match against.</param>
    /// <param name="pattern">The pattern to match.</param>
    /// <returns>Whether the path matches the pattern.</returns>
    public static bool IsMatch(string path, string pattern)
    {
        if (string.IsNullOrEmpty(pattern) || string.IsNullOrEmpty(path))
        {
            return false;
        }

        // Handle special cases
        if (pattern == "*.*")
        {
            return true; // Match all files
        }

        if (pattern == "*")
        {
            return true; // Match any file name
        }

        // Convert to lowercase for case-insensitive matching
        path = path.ToLower();
        pattern = pattern.ToLower();

        // Split pattern into filename and extension
        var patternParts = pattern.Split('.');
        string patternFileName = patternParts[0];
        string? patternExtension = patternParts.Length > 1 ? patternParts[1] : null;

        // Get actual file name and extension
        var fileInfo = new FileInfo(path);
        string fileName = fileInfo.Name;
        string fileExtension = fileInfo.Extension;

        // Match file name
        if (!MatchPattern(fileName, patternFileName))
        {
            return false;
        }

        // Match extension if specified
        if (patternExtension != null && !MatchPattern(fileExtension, patternExtension))
        {
            return false;
        }

        return true;
    }

    private static bool MatchPattern(string input, string pattern)
    {
        if (string.IsNullOrEmpty(pattern))
        {
            return true;
        }

        if (string.IsNullOrEmpty(input))
        {
            return false;
        }

        // Handle simple cases
        if (pattern == "*")
        {
            return true;
        }

        if (pattern == "?")
        {
            return input.Length == 1;
        }

        // Handle wildcards
        var patternChars = pattern.ToCharArray();
        var inputChars = input.ToCharArray();

        int pIndex = 0;
        int iIndex = 0;

        while (pIndex < patternChars.Length && iIndex < inputChars.Length)
        {
            if (patternChars[pIndex] == '*')
            {
                // Skip any number of characters in input
                pIndex++;
                continue;
            }
            else if (patternChars[pIndex] == '?')
            {
                // Match any single character
                pIndex++;
                iIndex++;
                continue;
            }
            else if (patternChars[pIndex] == inputChars[iIndex])
            {
                // Match exact character
                pIndex++;
                iIndex++;
            }
            else
            {
                // No match
                return false;
            }
        }

        // Handle remaining pattern
        while (pIndex < patternChars.Length && patternChars[pIndex] == '*')
        {
            pIndex++;
        }

        return pIndex == patternChars.Length && iIndex == inputChars.Length;
    }
}