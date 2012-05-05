using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace VibrantCommandLine
{
    public interface IConsole
    {
        int CursorLeft { get; set; }
        TextWriter ErrorWriter { get; }
        int WindowWidth { get; set; }

        void Write(object value);
        void Write(string value);
        void Write(string format, params object[] args);

        void WriteLine();
        void WriteLine(object value);
        void WriteLine(string value);
        void WriteLine(string format, params object[] args);

        void WriteError(object value);
        void WriteError(string value);
        void WriteError(string format, params object[] args);

        void WriteWarning(string value);
        void WriteWarning(bool prependWarningText, string value);
        void WriteWarning(string value, params object[] args);
        void WriteWarning(bool prependWarningText, string value, params object[] args);

        bool Confirm(string description);

        void PrintJustified(int startIndex, string text);
        void PrintJustified(int startIndex, string text, int maxWidth);
    }
}
