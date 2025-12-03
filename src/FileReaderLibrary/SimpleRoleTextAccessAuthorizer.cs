using System;
using System.Collections.Generic;

namespace FileReaderLibrary
{
    /// <summary>
    /// Simple role-based authorizer for text: 'admin' can read any path; other roles must be explicitly allowed.
    /// </summary>
    public class SimpleRoleTextAccessAuthorizer : ITextAccessAuthorizer
    {
        private readonly HashSet<string> _allowedPaths;

        public SimpleRoleTextAccessAuthorizer(IEnumerable<string>? allowedPaths = null)
        {
            _allowedPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (allowedPaths != null)
            {
                foreach (var p in allowedPaths)
                {
                    if (!string.IsNullOrWhiteSpace(p))
                        _allowedPaths.Add(p);
                }
            }
        }

        public bool CanRead(string path, string role)
        {
            if (string.IsNullOrWhiteSpace(role)) return false;
            if (string.IsNullOrWhiteSpace(path)) return false;

            if (string.Equals(role, "admin", StringComparison.OrdinalIgnoreCase))
                return true;

            return _allowedPaths.Contains(path);
        }
    }
}
