namespace GemMatch {
    public interface ICloneable<T> where T : class {
        public T Clone();
    }
}