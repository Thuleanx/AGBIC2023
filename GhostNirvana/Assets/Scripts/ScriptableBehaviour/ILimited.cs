namespace ScriptableBehaviour {

public interface ILimited<T> {
    public T Value {get; set;}
    public void CheckAndCorrectLimit();
}

}
