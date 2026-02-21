using UnityEngine;

namespace AbilitySystemTool
{
    public readonly struct ValidationMessage
    {
        public readonly ValidationSeverity Severity;
        public readonly string Message;
        public readonly Object Context;

        public ValidationMessage(ValidationSeverity severity, string message, UnityEngine.Object context = null)
        {
            Severity = severity;
            Message = message;
            Context = context;
        }
    }
}