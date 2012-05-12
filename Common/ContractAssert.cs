using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace VibrantUtils
{
    internal static class ContractAssert
    {
        public static void NotNull(Expression<Action> op, string paramName, bool ignoreTrace = false)
        {
            Action act = op.Compile();
            ArgumentNullException argEx = Assert.Throws<ArgumentNullException>(() => act());
            VerifyArgEx(argEx, paramName, ignoreTrace ? null : op);
        }

        public static void OutOfRange(Expression<Action> op, string paramName, bool ignoreTrace = false)
        {
            InvalidArgument<ArgumentOutOfRangeException>(op, paramName, ignoreTrace);
        }

        public static void InvalidArgument<T>(Expression<Action> op, string paramName, bool ignoreTrace = false) where T : ArgumentException
        {
            Action act = op.Compile();
            T argEx = Assert.Throws<T>(() => act());
            VerifyArgEx(argEx, paramName, ignoreTrace ? null : op);
        }

        public static void NotNullOrEmpty(Expression<Action<string>> op, string paramName, bool ignoreTrace = false)
        {
            Action<string> act = op.Compile();
            VerifyNotNullOrEmpty(Assert.Throws<ArgumentException>(() => act(null)), paramName, ignoreTrace ? null : op);
            VerifyNotNullOrEmpty(Assert.Throws<ArgumentException>(() => act(String.Empty)), paramName, ignoreTrace ? null : op);
        }

        private static void VerifyNotNullOrEmpty(ArgumentException argumentException, string paramName, LambdaExpression op)
        {
            Assert.Equal(
                ToFullArgExMessage(String.Format(CommonResources.Argument_NotNullOrEmpty, paramName), paramName),
                argumentException.Message);
            VerifyArgEx(argumentException, paramName, op);
        }

        private static void VerifyArgEx(ArgumentException argumentException, string paramName, LambdaExpression op)
        {
            if (op != null && op.Body.NodeType == ExpressionType.Call)
            {
                // Check and make sure that call is on the top of the stack after removing Requires
                var expected = ((MethodCallExpression)op.Body).Method;
                StackTrace stack = new StackTrace(argumentException);
                var frame = stack.GetFrames().SkipWhile(f => f.GetMethod().DeclaringType.FullName == typeof(Requires).FullName).FirstOrDefault();
                var actual = frame.GetMethod();
                Assert.True(actual != null, "Unable to find stack frame.");
                Assert.True(String.Equals(expected.DeclaringType.FullName + "." + expected.Name, actual.DeclaringType.FullName + "." + actual.Name),
                            "Expected exception was thrown at an unexpected site. If this is intentional, pass ignoreTrace = true to ContractAssert method");
            }

            Assert.Equal(paramName, argumentException.ParamName);
        }

        private static string ToFullArgExMessage(string message, string paramName)
        {
            return String.Format("{0}\r\nParameter name: {1}", message, paramName);
        }
    }
}
