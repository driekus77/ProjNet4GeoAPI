namespace ProjNet.Wkt.Builders
{
    /// <summary>
    /// Base for Builder(s).
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Builder<T>
    {
        /// <summary>
        /// The final Build action.
        /// </summary>
        /// <returns></returns>
        public abstract T Build();
    }
}
