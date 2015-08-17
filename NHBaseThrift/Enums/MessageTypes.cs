namespace NHBaseThrift.Enums
{
    /// <summary>
    ///     Message operation types in Thrift protocol.
    /// </summary>
    public enum MessageTypes
    {
        Call = 1,
        Reply = 2,
        Exception = 3,
        Oneway = 4
    }
}