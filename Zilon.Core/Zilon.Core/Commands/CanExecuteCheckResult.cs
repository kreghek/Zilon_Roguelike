using System;
using System.Collections.Generic;

namespace Zilon.Core.Commands
{
    public struct CanExecuteCheckResult : IEquatable<CanExecuteCheckResult>
    {
        public string FailureReason { get; set; }
        public bool IsSuccess { get; set; }

        public static CanExecuteCheckResult CreateFailed(string failureReason)
        {
            return new CanExecuteCheckResult { IsSuccess = false, FailureReason = failureReason };
        }

        public static CanExecuteCheckResult CreateSuccessful()
        {
            return new CanExecuteCheckResult { IsSuccess = true };
        }

        public bool Equals(CanExecuteCheckResult other)
        {
            return IsSuccess == other.IsSuccess && FailureReason == other.FailureReason;
        }

        public override bool Equals(object obj)
        {
            return obj is CanExecuteCheckResult result && Equals(result);
        }

        public override int GetHashCode()
        {
            var hashCode = 1458907301;
            hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(FailureReason);
            hashCode = (hashCode * -1521134295) + IsSuccess.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(CanExecuteCheckResult left, CanExecuteCheckResult right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(CanExecuteCheckResult left, CanExecuteCheckResult right)
        {
            return !(left == right);
        }
    }
}