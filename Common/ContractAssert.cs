using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace VibrantUtils
{
    internal static class ContractAssert
    {
        public static void NotNull(Action op, string paramName)
        {
            ArgumentNullException argEx = Assert.Throws<ArgumentNullException>(() => op());
            VerifyArgEx(argEx, paramName);
        }

        public static void OutOfRange(Action op, string paramName)
        {
            InvalidArgument<ArgumentOutOfRangeException>(op, paramName);
        }

        public static void InvalidArgument<T>(Action op, string paramName) where T : ArgumentException
        {
            T argEx = Assert.Throws<T>(() => op());
            VerifyArgEx(argEx, paramName);
        }

        public static void NotNullOrEmpty(Action<string> op, string paramName)
        {
            VerifyNotNullOrEmpty(Assert.Throws<ArgumentException>(() => op(null)), paramName);
            VerifyNotNullOrEmpty(Assert.Throws<ArgumentException>(() => op(String.Empty)), paramName);
        }

        private static void VerifyNotNullOrEmpty(ArgumentException argumentException, string paramName)
        {
            Assert.Equal(
                ToFullArgExMessage(String.Format(CommonResources.Argument_NotNullOrEmpty, paramName), paramName),
                argumentException.Message);
            VerifyArgEx(argumentException, paramName);
        }

        private static void VerifyArgEx(ArgumentException argumentException, string paramName)
        {
            Assert.Equal(paramName, argumentException.ParamName);
        }

        private static string ToFullArgExMessage(string message, string paramName)
        {
            return String.Format("{0}\r\nParameter name: {1}", message, paramName);
        }
    }
}
