using System;
using System.Text;

namespace Ordering.WebApi.Extensions
{
    public static class ExceptionExtensions
    {
        public static string Messages(this Exception exception)
        {
            var message = new StringBuilder();
            message.Append(exception.GetType() + ": " + exception.Message);

            var innerException = exception.InnerException;

            while (innerException != null)
            {
                message.Append(" -> " + innerException.Message);

                innerException = innerException.InnerException;
            }

            return message.ToString();
        }
    }
}
