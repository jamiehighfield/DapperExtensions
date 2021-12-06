using System;
using System.Data;

namespace SharpVNC.Client.Core.Connection
{
    /// <summary>
    /// Implement this interface to create a database connection wrapper.
    /// </summary>
    public interface IDatabaseConnection : IDbConnection, IDisposable { }
}